/* Copyright (C) 2009 Samuel Fredrickson <kinghajj@gmail.com>
 * 
 * This file is part of Mini, an INI library for the .NET framework.
 *
 * Mini is free software: you can redistribute it and/or modify it under the
 * terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option) any
 * later version.
 *
 * Mini is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Mini. If not, see <http://www.gnu.org/licenses/>.
 */

/* IniFile.cs - Represents an INI file.
 * 
 * This is the core class of the library. It is the one and only entrypoint into
 * the library, and, hopefully, is easy to use.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mini
{
    public class IniFile : IEnumerable<IniSection>
    {
        private List<IniPart> parts;

        #region Constructors
        /// <summary>
        /// Creates a new, blank INI file.
        /// </summary>
        public IniFile()
        {
            parts    = new List<IniPart>();
            Path     = string.Empty;
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Opens an INI file from a stream.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="StreamReader"/> from which to read the INI file.
        /// This stream will be closed.
        /// </param>
        public IniFile(StreamReader stream)
            : this()
        {
            string comment = string.Empty;
            IniSection section = null;
            IniSetting setting = null;

            foreach(var pattern in new IniPatterns(stream))
            {
                switch(pattern.Kind)
                {
                    case IniPatternKind.Comment:
                        comment = JoinComments(string.Empty,
                                               comment,
                                               pattern.Comment);
                        break;
                    case IniPatternKind.Section:
                        section = this[pattern.Name];
                        section.Comment = JoinComments(section.Comment,
                                                       comment,
                                                       pattern.Comment);
                        comment = string.Empty;
                        break;
                    case IniPatternKind.Setting:
                        if(section != null)
                        {
                            setting = section[pattern.Name];
                            setting.Comment = JoinComments(setting.Comment,
                                                           comment,
                                                           pattern.Comment);
                            setting.Value = pattern.Value;
                            comment = string.Empty;
                        }
                        break;
                    case IniPatternKind.None:
                        // If there's a stored comment, add it then clear it.
                        if(!string.IsNullOrEmpty(comment))
                        {
                            var new_comment = new IniComment(comment);
                            if(section != null)
                                section.AddPart(new_comment);
                            else
                                parts.Add(new_comment);
                            comment = string.Empty;
                        }
                        break;
                }
            }
            stream.Close();
        }

        /// <summary>
        /// Opens an INI file from a path with the given encoding.
        /// </summary>
        /// <param name="path">
        /// A <see cref="System.String"/> path to the INI file.
        /// </param>
        /// <param name="encoding">
        /// An <see cref="Encoding"/> with which to interpret the INI file.
        /// </param>
        public IniFile(string path, Encoding encoding)
            : this(new StreamReader(path, encoding))
        {
            Path     = path;
            Encoding = encoding;
        }

        /// <summary>
        /// Opens an INI file from a path.
        /// </summary>
        /// <param name="path">
        /// A <see cref="System.String"/> path to the INI file.
        /// </param>
        public IniFile(string path)
            : this(path, Encoding.Default)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Removes a section from the INI file.
        /// </summary>
        /// <param name="section">The section to remove.</param>
        public void Remove(IniSection section)
        {
            parts.Remove(section);
        }

        /// <summary>
        /// Removes a section by its name.
        /// </summary>
        /// <param name="sectionName">The name of the section to remove.</param>
        public void Remove(string sectionName)
        {
            Remove(FindSection(sectionName));
        }

        /// <summary>
        /// Saves the file using its path.
        /// </summary>
        public void Save()
        {
            SaveAs(Path);
        }

        /// <summary>
        /// Saves the file using the given path.
        /// </summary>
        /// <param name="path">
        /// The path to save the file.
        /// </param>
        public void SaveAs(string path)
        {
            using(var writer = new StreamWriter(path, false, Encoding))
                foreach(var part in parts)
                    part.Write(writer);
        }

        /// <summary>
        /// Removes a part from a files's list of INI parts.
        /// </summary>
        /// <param name="part">The part to remove.</param>
        internal void RemovePart(IniPart part)
        {
            parts.Remove(part);
        }

        /// <summary>
        /// Finds a section by name.
        /// </summary>
        /// <param name="name">The name of the section to find.</param>
        /// <returns>The found section or null.</returns>
        private IniSection FindSection(string name)
        {
            return parts.Find(
                part => (part is IniSection) ?
                            (part as IniSection).Name.Equals(name) :
                            false) as IniSection;
        }

        /// <summary>
        /// Joins together multiple comments.
        /// </summary>
        /// <param name="current">A section's or setting's current comment.</param>
        /// <param name="built">The current built-up comment.</param>
        /// <param name="last">The last parsed comment.</param>
        /// <returns>A new, joined comment.</returns>
        private static string JoinComments(string current, string built,
                                           string last)
        {
            return
                string.IsNullOrEmpty(current)
                    ? (string.IsNullOrEmpty(built)
                          ? last
                          : string.Concat(built, Environment.NewLine, last))
                    : string.Concat(current, Environment.NewLine, built, last);
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an iterator for the file's sections.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator`1"/>
        /// </returns>
        public IEnumerator<IniSection> GetEnumerator()
        {
            foreach(var part in parts)
                if(part is IniSection)
                    yield return (IniSection)part;
        }
        
        /// <summary>
        /// Silly required function.
        /// </summary>
        /// <returns>An enumerator of IniParts.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Indexers
        /// <value>
        /// Gets a file's section via its name.
        /// </value>
        public IniSection this[string name]
        {
            get
            {
                IniSection found = FindSection(name);

                // if not, create it and add it.
                if(found == null)
                {
                    found = new IniSection(name, string.Empty);
                    parts.Add(found);
                }

                return found;
            }
        }

        /// <summary>
        /// Gets a file's part via its index.
        /// </summary>
        public IniPart this[int index]
        {
            get
            {
                return parts[index];
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the INI's encoding.
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <value>
        /// Get an INI file's path.
        /// </value>
        public string Path { get; set; }
        #endregion
    }
}
