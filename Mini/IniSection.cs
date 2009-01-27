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

/* IniSection.cs - Represents a section in an INI file.
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Mini
{
    public class IniSection : IniPart, IDictionary<string, string>
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
        /// Adds a setting in the form of a KeyValuePair.
        /// </summary>
        /// <param name="pair">The pair to add.</param>
        public void Add(KeyValuePair<string, string> pair)
        {
            this[pair.Key] = pair.Value;
        }

        /// <summary>
        /// Adds or updates a setting in the form of two strings.
        /// </summary>
        /// <param name="key">The key of the setting to add.</param>
        /// <param name="value">The value of the setting to add.</param>
        public void Add(string key, string value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Clears all held settings.
        /// </summary>
        public void Clear()
        {
            parts.Clear();
        }

        /// <summary>
        /// Determines whether a held setting matches the KeyValuePair.
        /// </summary>
        /// <param name="pair">The pair to compare against.</param>
        /// <returns>
        /// True if there is a setting that matches the given pair.
        /// </returns>
        public bool Contains(KeyValuePair<string, string> pair)
        {
            var match = new IniSetting(pair.Key, pair.Value);

            foreach(var setting in GetSettings())
                if(setting.Equals(match))
                    return true;

            return false;
        }

        /// <summary>
        /// Determines whether a held setting has the given key.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <returns>True if such a setting is held.</returns>
        public bool ContainsKey(string key)
        {
            foreach(var setting in GetSettings())
                if(setting.Key == key)
                    return true;

            return false;
        }

        /// <summary>
        /// Copies the held settings to an array of KeyValuePair at an index.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="i">The index at which to start copying.</param>
        public void CopyTo(KeyValuePair<string, string>[] array, int i)
        {
            foreach(var setting in GetSettings())
            {
                if(i >= array.Length)
                    break;
                array[i] = new KeyValuePair<string,string>(setting.Key,
                                                           setting.Value);
            }
        }

        /// <summary>
        /// Removes the setting that matches the given KeyValuePair.
        /// </summary>
        /// <param name="pair">The KeyValuePair to remove.</param>
        /// <returns>True if the setting was removed successfully.</returns>
        public bool Remove(KeyValuePair<string, string> pair)
        {
            var match = new IniSetting(pair.Key, pair.Value);

            foreach(var setting in GetSettings())
                if(setting.Equals(match))
                    return Remove(setting);

            return false;
        }

        /// <summary>
        /// Removes a setting by its key.
        /// </summary>
        /// <param name="sectionName">The key of the setting to remove.</param>
        public bool Remove(string settingKey)
        {
            return Remove(FindSetting(settingKey));
        }

        /// <summary>
        /// Tries to get a setting's value by its key.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <param name="value">
        /// Where to put the found value. Set to null if not found.
        /// </param>
        /// <returns>True if the value was found and set.</returns>
        public bool TryGetValue(string key, out string value)
        {
            foreach(var setting in GetSettings())
                if(setting.Key == key)
                {
                    value = setting.Value;
                    return true;
                }

            value = null;
            return false;
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
        /// Creates a setting with the given key if it doesn't exist.
        /// </summary>
        /// <param name="key">The key of the setting to create.</param>
        internal void CreateSetting(string key)
        {
            if(!Keys.Contains(key))
                parts.Add(new IniSetting(key, string.Empty));
        }

        /// <summary>
        /// Finds a setting by key.
        /// </summary>
        /// <param name="name">The key of the setting to find.</param>
        /// <returns>The found setting or null.</returns>
        internal IniSetting FindSetting(string key)
        {
            foreach(var part in parts)
            {
                var setting = part as IniSetting;
                if(setting != null && setting.Key.Equals(key))
                    return setting;
            }
            return null;
        }

        /// <summary>
        /// Removes a setting from the section.
        /// </summary>
        /// <param name="section">The setting to remove.</param>
        internal bool Remove(IniSetting setting)
        {
            return parts.Remove(setting);
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
        /// Gets an enumerator of the settings held by this section.
        /// </summary>
        /// <returns>
        /// An enumerator of the settings held by this section.
        /// </returns>
        private IEnumerable<IniSetting> GetSettings()
        {
            foreach(var part in parts)
            {
                var partSetting = part as IniSetting;
                if(partSetting != null)
                    yield return partSetting;
            }
        }

        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an iterator for the section's settings.
        /// </summary>
        /// <returns>
        /// An enumerator of settings of the section.
        /// </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach(var setting in GetSettings())
                yield return new KeyValuePair<string, string>(setting.Key,
                                                              setting.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Indexer
        /// <value>
        /// Gets or sets a section's setting via its key.
        /// </value>
        public string this[string key]
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

                return found.Value;
            }

            set
            {
                var found = FindSetting(key);
                if(found == null)
                {
                    found = new IniSetting(key, value);
                    parts.Add(found);
                }
                else
                    found.Value = value;
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
        /// Gets the number of held settings.
        /// </summary>
        public int Count
        {
            get
            {
                return parts.Count;
            }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of the keys of the held settings.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                List<string> keys = new List<string>(parts.Count);

                foreach(var setting in GetSettings())
                    keys.Add(setting.Key);

                return keys;
            }
        }

        /// <summary>
        /// Gets a collection of the values of the held settings.
        /// </summary>
        public ICollection<string> Values
        {
            get
            {
                List<string> values = new List<string>(parts.Count);

                foreach(var setting in GetSettings())
                    values.Add(setting.Value);

                return values;
            }
        }

        /// <value>
        /// Get or set a section's name.
        /// </value>
        public string Name { get; set; }
        #endregion
    }
}
