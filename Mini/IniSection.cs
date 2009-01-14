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

/* IniSection.cs - Represents a section in an INI file.
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Mini
{
    public class IniSection : IniPart, IEnumerable<IniSetting>
    {
        private List<IniPart> parts;

        #region Constructors
        /// <summary>
        /// Create a new INI section with the given name, comment, and file.
        /// </summary>
        /// <param name="name">
        /// The section's name.
        /// </param>
        /// <param name="comment">
        /// The section's comment.
        /// </param>
        /// <param name="ini">
        /// The file to which the section belongs.
        /// </param>
        internal IniSection(string name, string comment)
        {
            parts = new List<IniPart>();
            Comment = comment;
            Name = name;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Removes a setting from the section.
        /// </summary>
        /// <param name="section">The setting to remove.</param>
        public void Remove(IniSetting setting)
        {
            parts.Remove(setting);
        }

        // <summary>
        /// Removes a setting by its key.
        /// </summary>
        /// <param name="sectionName">The key of the setting to remove.</param>
        public void Remove(string settingKey)
        {
            Remove(FindSetting(settingKey));
        }

        /// <summary>
        /// Adds a part to a section's list of INI parts.
        /// </summary>
        /// <param name="part">The part to add.</param>
        internal void AddPart(IniPart part)
        {
            parts.Add(part);
        }

        /// <summary>
        /// Removes a part from a section's list of INI parts.
        /// </summary>
        /// <param name="part">The part to remove.</param>
        internal void RemovePart(IniPart part)
        {
            parts.Remove(part);
        }

        /// <summary>
        /// Writes the section to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        override internal void Write(StreamWriter writer)
        {
            new IniComment(Comment).Write(writer, false);
            writer.WriteLine("[{0}]", Name);
            foreach(var part in parts)
                part.Write(writer);
            writer.WriteLine();
        }

        /// <summary>
        /// Finds a setting by key.
        /// </summary>
        /// <param name="name">The key of the setting to find.</param>
        /// <returns>The found setting or null.</returns>
        private IniSetting FindSetting(string key)
        {
            return parts.Find(
                part => part is IniSetting ?
                            (part as IniSetting).Key.Equals(key) :
                            false) as IniSetting;
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an iterator for the section's settings.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator`1"/>
        /// </returns>
        public IEnumerator<IniSetting> GetEnumerator()
        {
            foreach(var part in parts)
                if(part is IniSetting)
                    yield return (IniSetting)part;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Indexer
        /// <value>
        /// Gets a section's setting via its key.
        /// </value>
        public IniSetting this[string key]
        {
            get
            {
                IniSetting found = FindSetting(key);

                // if not, create it and add it.
                if(found == null)
                {
                    found = new IniSetting(key, string.Empty);
                    parts.Add(found);
                }

                return found;
            }
        }

        /// <summary>
        /// Gets a section's setting via its index.
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
        /// <value>
        /// Get or set a section's comment.
        /// </value>
        public string Comment
        {
            get;
            set;
        }

        /// <value>
        /// Get or set a section's name.
        /// </value>
        public string Name { get; set; }
        #endregion
    }
}
