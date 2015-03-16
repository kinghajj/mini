/* Copyright (C) 2014 Samuel Fredrickson <samfredrickson@gmail.com>
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

namespace Mini
{
    /// <summary>
    /// Represents a section in an INI document.
    /// </summary>
    public class IniSection : IniPart, ICollection<IniSetting>, IEquatable<IniSection>
    {
        private readonly OrderedDictionaryList<string, IniPart> _parts = new OrderedDictionaryList<string, IniPart>();

        #region Constructors
        /// <summary>
        /// Create a new INI section with the given name, comment, and document.
        /// </summary>
        /// <param name="name">
        /// The section's name.
        /// </param>
        /// <param name="comment">
        /// The section's comment.
        /// </param>
        internal IniSection(string name, string comment)
            : base(1)
        {
            Comment = comment;
            Name = name;
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Adds a setting to the section.
        /// </summary>
        /// <param name="item">The setting to add to the section.</param>
        public void Add(IniSetting item)
        {
            AddPart(item);
        }

        /// <summary>
        /// Removes all parts from a section.
        /// </summary>
        public void Clear()
        {
            _parts.Clear();
        }

        /// <summary>
        /// Determines whether a section contains a setting.
        /// </summary>
        /// <param name="item">The setting to locate in the section.</param>
        /// <returns>true if the setting is found; otherwise false.</returns>
        public bool Contains(IniSetting item)
        {
            return HasSetting(item.Key);
        }

        /// <summary>
        /// Copies the settings of the section to an Array, starting at a
        /// particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the settings
        /// copied from the section. The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        public void CopyTo(IniSetting[] array, int arrayIndex)
        {
            foreach(var setting in this)
            {
                if(arrayIndex >= array.Length) break;
                array[arrayIndex++] = setting;
            }
        }

        public override bool Equals(IniPart other)
        {
            return Equals(other as IniSection);
        }

        public bool Equals(IniSection other)
        {
            return !ReferenceEquals(other, null) && ((IEnumerable<IniPart>) _parts).SequenceEqual(other._parts);
        }

        /// <summary>
        /// Determines whether an IniSection contains an IniSetting with the
        /// given key.
        /// </summary>
        /// <param name="key">The key of the setting to find.</param>
        /// <returns>true if a setting found, else false.</returns>
        public bool HasSetting(string key)
        {
            return _parts.ContainsKey(key);
        }

        /// <summary>
        /// Removes the first occurrence of a specific setting from the section.
        /// </summary>
        /// <param name="item">
        /// The setting to remove from the section.
        /// </param>
        /// <returns>
        /// true if the setting was successfully removed from the section;
        /// otherwise, false. This method also returns false if item is not
        /// found in the original section.
        /// </returns>
        public bool Remove(IniSetting item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Removes a setting by its key.
        /// </summary>
        /// <param name="settingKey">The key of the setting to remove.</param>
        public bool Remove(string settingKey)
        {
            return _parts.Remove(settingKey);
        }
        #endregion

        #region Internal
        /// <summary>
        /// Adds a part to a section's list of INI parts.
        /// </summary>
        /// <param name="part">The part to add.</param>
        internal void AddPart(IniPart part)
        {
            _parts.AddUnkeyed(part);
        }

        /// <summary>
        /// Writes the section to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        override internal void Write(TextWriter writer)
        {
            WriteNewLines(writer, NewLines);
            IniComment.Write(Comment, writer, false);
            writer.WriteLine("[{0}]", Name);
            foreach(var part in _parts.Values)
                part.Write(writer);
        }
        #endregion

        #region Private
        /// <summary>
        /// Finds a setting by key.
        /// </summary>
        /// <param name="key">The key of the setting to find.</param>
        /// <returns>The found setting or null.</returns>
        private IniSetting FindSetting(string key)
        {
            IniPart ret;
            _parts.TryGetValue(key, out ret);
            return ret as IniSetting;
        }
        #endregion
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
            return _parts.Values.OfType<IniSetting>().GetEnumerator();
        }

        /// <summary>
        /// A silly required function.
        /// </summary>
        /// <returns>An enumerator of IniSettings.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Gets a section's setting via its key.
        /// </summary>
        /// <remarks>
        /// If a setting with the given key does not exist, it will be created
        /// and set to the empty string.
        /// </remarks>
        public IniSetting this[string key]
        {
            get
            {
                var found = FindSetting(key);

                // if not, create it and add it.
                if(found == null)
                {
                    found = new IniSetting(key, string.Empty);
                    _parts.Add(key, found);
                }

                return found;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or set a section's comment.
        /// </summary>
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
                return _parts.Count;
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

        /// <summary>
        /// Get or set a section's name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        #endregion
    }
}
