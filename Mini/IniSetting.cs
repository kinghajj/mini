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
using System.IO;

namespace Mini
{
    /// <summary>
    /// Represents a setting in an INI document.
    /// </summary>
    public class IniSetting : IniPart, IEquatable<IniSetting>
    {
        #region Constructors
        /// <summary>
        /// Creates a new setting with a key, value, and section.
        /// </summary>
        /// <param name="key">
        /// The setting's key.
        /// </param>
        /// <param name="value">
        /// The setting's value.
        /// </param>
        internal IniSetting(string key, string value)
            : base(0)
        {
            Comment = string.Empty;
            Key = key;
            Value = value;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tests whether two IniSettings are equal.
        /// </summary>
        /// <param name="other">The other setting to compare.</param>
        /// <returns>true if they are equal, else false.</returns>
        public bool Equals(IniSetting other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// Tests whether an IniSetting equals an object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if they are equal, else false.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as IniSection;
            return ReferenceEquals(this, obj) ||
                (other != null && Equals(other));
        }

        /// <summary>
        /// Gets the hash code of an IniSetting.
        /// </summary>
        /// <returns>The setting's hash code.</returns>
        public override int GetHashCode()
        {
            return Comment.GetHashCode() ^ Key.GetHashCode() ^ Value.GetHashCode();
        }

        /// <summary>
        /// Compares two IniSettings for equality.
        /// </summary>
        /// <param name="a">The first IniSetting.</param>
        /// <param name="b">The second IniSetting.</param>
        /// <returns>true if they are equal, else false.</returns>
        public static bool operator ==(IniSetting a, IniSetting b)
        {
            if(ReferenceEquals(a, null) ||
               ReferenceEquals(b, null))
                return ReferenceEquals(a, null) && ReferenceEquals(b, null);

            return a.Equals(b);
        }

        /// <summary>
        /// Compares two IniSettings for inequality.
        /// </summary>
        /// <param name="a">The first IniSetting.</param>
        /// <param name="b">The second IniSetting.</param>
        /// <returns>true if they are not equal, else false.</returns>
        public static bool operator !=(IniSetting a, IniSetting b)
        {
            if(ReferenceEquals(a, null) ||
               ReferenceEquals(b, null))
                return !(ReferenceEquals(a, null) && ReferenceEquals(b, null));

            return !a.Equals(b);
        }

        /// <summary>
        /// Writes the setting to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        override internal void Write(TextWriter writer)
        {
            WriteNewLines(writer, NewLines);
            IniComment.Write(Comment, writer, false);
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

        /// <summary>
        /// Gets or sets a setting's key.
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a settings's value.
        /// </summary>
        public string Value
        {
            get;
            set;
        }
        #endregion
    }
}
