/* Copyright (C) 2010 Samuel Fredrickson <kinghajj@gmail.com>
 * 
 * This file is part of Mini, an INI library for the .NET framework.
 *
 * Mini is free software: you can redistribute it and/or modify it under the
 * terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 2.1 of the License, or (at your option)
 * any later version.
 *
 * Mini is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Mini. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mini
{
    /// <summary>
    /// Represents an INI document.
    /// </summary>
    /// <remarks>
    /// This is the core class of the library. It is the one and only entrypoint
    /// into the library, and, hopefully, is easy to use.
    /// </remarks>
    public class IniDocument : IEnumerable<IniSection>
    {
        private List<IniPart> parts;

        #region Constructors
        /// <summary>
        /// Creates a new, blank INI document.
        /// </summary>
        public IniDocument()
        {
            parts    = new List<IniPart>();
            Path     = string.Empty;
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Opens an INI document from a stream.
        /// </summary>
        /// <param name="stream">
        /// The stream from which to read the INI document.
        /// This stream will be closed before this method completes.
        /// </param>
        public IniDocument(TextReader stream)
            : this()
        {
            Parse(stream);
            stream.Close();
        }

        /// <summary>
        /// Opens an INI document from a path with the given encoding.
        /// </summary>
        /// <param name="path">
        /// The path to the INI document.
        /// </param>
        /// <param name="encoding">
        /// An encoding with which to interpret the INI document.
        /// </param>
        public IniDocument(string path, Encoding encoding)
            : this(new StreamReader(path, encoding))
        {
            Path     = path;
            Encoding = encoding;
        }

        /// <summary>
        /// Opens an INI document from a path with the default encoding.
        /// </summary>
        /// <param name="path">
        /// The path to the INI document.
        /// </param>
        public IniDocument(string path)
            : this(path, Encoding.Default)
        {
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Determines whether an IniDocument contains an IniSection with the given
        /// name.
        /// </summary>
        /// <param name="name">The name of the section to find.</param>
        /// <returns>true if the section found, else false.</returns>
        public bool HasSection(string name)
        {
            return this.Any(section => section.Name == name);
        }

        /// <summary>
        /// Removes a section from the INI document.
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
        /// Saves the document using its stored path.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// Throw when Path is null or empty.
        /// </exception>
        public void Write()
        {
            if(string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path is null or empty.");
            Write(Path);
        }

        /// <summary>
        /// Saves the document using the given path.
        /// </summary>
        /// <param name="path">The path to save the document to.</param>
        public void Write(string path)
        {
            using(var writer = new StreamWriter(path, false, Encoding))
                Write(writer);
        }

        /// <summary>
        /// Writes the document using the given TextWriter.
        /// </summary>
        /// <param name="writer">The TextWriter to write the document to.
        /// </param>
        public void Write(TextWriter writer)
        {
            parts.ForEach(part => part.Write(writer));
        }
        #endregion

        #region Private
        /// <summary>
        /// Finds a section by name.
        /// </summary>
        /// <param name="name">The name of the section to find.</param>
        /// <returns>The found section or null.</returns>
        private IniSection FindSection(string name)
        {
            return parts.OfType<IniSection>().FirstOrDefault(s => s.Name == name);
        }

        /// <summary>
        /// Parses an input stream into as an INI document.
        /// </summary>
        /// <param name="stream">The stream to parse.</param>
        private void Parse(TextReader stream)
        {
            string comment = string.Empty;
            IniSection section = null;
            IniSetting setting = null;
            int newlines = 0;

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
                        // Get this section by its name.
                        section = this[pattern.Name];
                        // Set its comment and value.
                        section.Comment = JoinComments(section.Comment,
                                                       comment,
                                                       pattern.Comment);
                        section.Newlines = newlines;
                        newlines = 0;
                        comment = string.Empty;
                        break;
                    case IniPatternKind.Setting:
                        // If we're within a section,
                        if(section != null)
                        {
                            // Get this setting by its key.
                            setting = section[pattern.Name];
                            // Set its comment and value.
                            setting.Comment = JoinComments(setting.Comment,
                                                           comment,
                                                           pattern.Comment);
                            setting.Value = pattern.Value;
                            setting.Newlines = newlines;
                            newlines = 0;
                            // Erase the old built-up comment.
                            comment = string.Empty;
                        }
                        break;
                    case IniPatternKind.None:
                        // If there's a stored comment, add it then clear it.
                        if(!string.IsNullOrEmpty(comment))
                        {
                            var new_comment = new IniComment(comment);
                            new_comment.Newlines = newlines;
                            if(section != null)
                                section.AddPart(new_comment);
                            else
                                parts.Add(new_comment);
                            comment = string.Empty;
                            newlines = 0;
                        }
                        newlines++;
                        break;
                }
            }
        }

        /// <summary>
        /// Joins together multiple comments.
        /// </summary>
        /// <param name="previous">A section's or setting's previous comment.</param>
        /// <param name="built">The current built-up comment.</param>
        /// <param name="last">The last parsed comment.</param>
        /// <returns>A new, joined comment.</returns>
        private static string JoinComments(string previous, string built,
                                           string last)
        {
            return string.Join(Environment.NewLine,
                               new string[] { previous, built, last });
        }
        #endregion
        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an iterator for the document's sections.
        /// </summary>
        /// <returns>
        /// An enumerator of sections of the INI document.
        /// </returns>
        public IEnumerator<IniSection> GetEnumerator()
        {
            return parts.OfType<IniSection>().GetEnumerator();
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
        /// <summary>
        /// Gets a document's section via its name.
        /// </summary>
        /// <remarks>
        /// If a section with the given name does not exist, it will be created
        /// and have no settings.
        /// </remarks>
        public IniSection this[string name]
        {
            get
            {
                IniSection found = FindSection(name);

                // if not, create it and add it.
                if(found == null)
                    parts.Add(found = new IniSection(name, string.Empty));

                return found;
            }
        }

        /// <summary>
        /// Gets a document's part via its index.
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
        public Encoding Encoding
        {
            get;
            set;
        }

        /// <summary>
        /// Get an INI document's path.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an enumeration of the document's parts.
        /// </summary>
        public IEnumerable<IniPart> Parts
        {
            get
            {
                return parts;
            }
        }

        #endregion
    }
}
