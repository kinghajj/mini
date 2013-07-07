/* Copyright (C) 2013 Samuel Fredrickson <kinghajj@gmail.com>
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

namespace Mini
{
    /// <summary>
    /// Specifies how a property is serialized to an INI document.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IniValueAttribute : Attribute
    {
        /// <summary>
        /// Specify how this property is serialized to an INI document.
        /// </summary>
        /// <param name="section">The section of the INI in which this value lies.</param>
        /// <param name="key">The key that holds this value.</param>
        /// <param name="defaultValue">A default value, used during deserialization if none is set in the source
        /// document.</param>
        public IniValueAttribute(string section, string key, string defaultValue = "")
        {
            Section = section;
            Key = key;
            DefaultValue = defaultValue;
        }

        public string Section { get; set; }
        public string Key { get; set; }
        public string DefaultValue { get; set; }
    }
}
