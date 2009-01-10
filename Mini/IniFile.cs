/* Copyright (C) 2008 Samuel Fredrickson <kinghajj@gmail.com>
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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mini
{
    public class IniFile : IEnumerable<IniSection>
    {
        internal List<IniSection> sections;

        #region Constructors
        /// <summary>
        /// Creates a new, blank INI file.
        /// </summary>
        public IniFile()
        {
            sections = new List<IniSection>();
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
            var parser = new IniParser(this, stream);
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
            Path = path;
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
            var writer = new StreamWriter(path);
            foreach(var section in this)
                section.Write(writer);
            writer.Close();
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
            foreach(IniSection section in sections)
                yield return section;
        }
        
        /// <summary>
        /// Silly required function.
        /// </summary>
        /// <returns>An enumerator of IniSections</returns>
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
                IniSection found = null;

                // try to find the section.
                foreach(IniSection section in this)
                    if(section.Name.Equals(name))
                        found = section;

                // if not, create it and add it.
                if(found == null)
                {
                    found = new IniSection(name, string.Empty, this);
                    sections.Add(found);
                }

                return found;
            }
        }

        /// <summary>
        /// Gets a file's section via its index.
        /// </summary>
        public IniSection this[int index]
        {
            get
            {
                return sections[index];
            }
        }
        #endregion

        #region Properties
        /// <value>
        /// Get an INI file's path.
        /// </value>
        public string Path { get; set; }
        #endregion
    }
}
