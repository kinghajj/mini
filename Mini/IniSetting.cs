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

using System.IO;

namespace Mini
{
    public class IniSetting
    {
        private IniSection section;

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
            Key = key;
            Value = _value;
            this.section = section;
            this.Comment = string.Empty;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the setting's comment.
        /// </summary>
        public string Comment { get; set; }
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

        internal void Write(StreamWriter writer)
        {
            if(Comment != string.Empty)
            {
                writer.Write("; ");
                foreach(char c in Comment)
                {
                    if(c == '\n')
                        writer.Write("\n; ");
                    else
                        writer.Write(c);
                }
                writer.WriteLine();
            }
            writer.WriteLine("{0} = {1}", Key, Value);
        }
    }
}
