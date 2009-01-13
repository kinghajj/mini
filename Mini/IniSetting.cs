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

/* IniSetting.cs - Represents a setting in an INI file.
 */

using System.IO;

namespace Mini
{
    public class IniSetting : IniPart
    {
        private IniSection section;

        #region Constructors
        /// <summary>
        /// Creates a new setting with a key, value, and section.
        /// </summary>
        /// <param name="key">
        /// The setting's key.
        /// </param>
        /// <param name="_value">
        /// The setting's value.
        /// </param>
        /// <param name="section">
        /// The section to which the setting belongs.
        /// </param>
        internal IniSetting(string key, string _value, IniSection section)
        {
            Comment = string.Empty;
            Key = key;
            Value = _value;
            this.section = section;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Removes a setting from its section.
        /// </summary>
        public void Remove()
        {
            section.RemovePart(this);
        }

        /// <summary>
        /// Writes the setting to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        override internal void Write(StreamWriter writer)
        {
            new IniComment(Comment).Write(writer, false);
            writer.WriteLine("{0}={1}", Key, Value);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the setting's comment.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <value>
        /// Gets or sets a setting's key.
        /// </value>
        public string Key { get; set; }

        /// <value>
        /// Gets a setting's section.
        /// </value>
        public IniSection Section { get { return section; } }

        /// <value>
        /// Gets or sets a settings's value.
        /// </value>
        public string Value { get; set; }
        #endregion
    }
}
