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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Mini
{
    public class IniSection : IEnumerable<IniSetting>
    {
        internal IniFile ini;
        private List<IniSetting> settings;

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
        internal IniSection(string name, string comment, IniFile ini)
        {
            this.ini = ini;
            Comment = comment;
            Name = name;
            settings = new List<IniSetting>();
            // -- TODO --
        }

        #region Methods
        /// <summary>
        /// Removes a section from its file.
        /// </summary>
        public void Remove()
        {
            ini.sections.Remove(this);
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
            foreach(IniSetting setting in settings)
                yield return setting;
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
                IniSetting found = null;

                // try to find the section.
                foreach(IniSetting setting in this)
                    if(setting.Key.Equals(key))
                        found = setting;

                // if not, create it and add it.
                if(found == null)
                {
                    found = new IniSetting(key, string.Empty, this);
                    settings.Add(found);
                }

                return found;
            }
        }

        /// <summary>
        /// Gets a section's setting via its index.
        /// </summary>
        public IniSetting this[int index]
        {
            get
            {
                return settings[index];
            }
        }
        #endregion

        #region Properties
        /// <value>
        /// Get or set a section's comment.
        /// </value>
        public string Comment { get; set; }

        /// <value>
        /// Get or set a section's name.
        /// </value>
        public string Name { get; set; }

        /// <value>
        /// Get a section's file.
        /// </value>
        public IniFile Ini { get { return ini; } }
        #endregion

        internal void Write(StreamWriter writer)
        {
            if(Comment != String.Empty)
                foreach(var comment in
                        Comment.Split(Environment.NewLine.ToCharArray()))
                    writer.WriteLine("; {0}", comment);

            writer.WriteLine("[{0}]", Name);
            foreach(var setting in this)
                setting.Write(writer);
            writer.WriteLine();
        }
    }
}
