/* Copyright (C) 2009 Samuel Fredrickson <kinghajj@gmail.com>
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

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Mini
{
    /// <summary>
    /// Represents a section in an INI file.
    /// </summary>
    public class IniSection : IniPart, ICollection<IniSetting>
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
        /// Adds a setting to the section.
        /// </summary>
        /// <param name="setting">The setting to add to the section.</param>
        public void Add(IniSetting setting)
        {
            AddPart(setting);
        }

        /// <summary>
        /// Removes all parts from a section.
        /// </summary>
        public void Clear()
        {
            parts.Clear();
        }

        /// <summary>
        /// Determines whether a section contains a setting.
        /// </summary>
        /// <param name="find">The setting to locate in the section.</param>
        /// <returns>true if the setting is found; otherwise false.</returns>
        public bool Contains(IniSetting find)
        {
            foreach(var setting in this)
                if(setting == find)
                    return true;
            return false;
        }

        /// <summary>
        /// Copies the settings of the section to an Array, starting at a
        /// particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the settings
        /// copied from the section. The Array must have zero-based indexing.
        /// </param>
        /// <param name="i">
        /// The zero-based index in array at which copying begins.
        /// </param>
        public void CopyTo(IniSetting[] array, int i)
        {
            foreach(var setting in this)
            {
                if(i >= array.Length) break;
                array[i++] = setting;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific setting from the section.
        /// </summary>
        /// <param name="section">
        /// The setting to remove from the section.
        /// </param>
        /// <returns>
        /// true if the setting was successfully removed from the section;
        /// otherwise, false. This method also returns false if item is not
        /// found in the original section.
        /// </returns>
        public bool Remove(IniSetting setting)
        {
            return parts.Remove(setting);
        }

        // <summary>
        /// Removes a setting by its key.
        /// </summary>
        /// <param name="sectionName">The key of the setting to remove.</param>
        public bool Remove(string settingKey)
        {
            return Remove(FindSetting(settingKey));
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
            foreach(var setting in this)
                if(setting.Key == key)
                    return setting;
            return null;
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an iterator for the section's settings.
        /// </summary>
        /// <returns>
        /// An enumerator of settings of the section.
        /// </returns>
        public IEnumerator<IniSetting> GetEnumerator()
        {
            foreach(var part in parts)
            {
                var setting = part as IniSetting;
                if(setting != null)
                    yield return setting;
            }
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

        /// <summary>
        /// Gets the number of parts contained in the section.
        /// </summary>
        public int Count
        {
            get
            {
                return parts.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the section is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <value>
        /// Get or set a section's name.
        /// </value>
        public string Name { get; set; }
        #endregion
    }
}
