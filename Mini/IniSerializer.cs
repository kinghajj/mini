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
using System.Linq;

namespace Mini
{
    /// <summary>
    /// Serializes objects to and from INI documents.
    /// </summary>
    /// <typeparam name="T">The type of object to serialize.</typeparam>
    public class IniSerializer<T> : IniSerializerBase where T : new()
    {
        private readonly Type _type;

        public IniSerializer()
        {
            _type = typeof (T);
            var hasIniValueContainerAttr =
                _type.GetCustomAttributes(false).Any(attr => attr.GetType() == typeof (IniValueContainerAttribute));
            if(!hasIniValueContainerAttr)
                throw new InvalidOperationException(
                    string.Format("Type \"{0}\" does not have the IniValueContainerAttribute.", _type));
            lock (Lock)
            {
                Process(_type);
            }
        }

        /// <summary>
        /// Deserialize an object from the given INI document.
        /// </summary>
        /// <param name="ini">The source INI document.</param>
        /// <returns>An object whose properties were deserialized from the INI document.</returns>
        public T Deserialize(IniDocument ini)
        {
            lock (Lock)
            {
                return (T) DeserializeType(_type, ini);
            }
        }

        /// <summary>
        /// Serialize an object to the given INI document.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="ini">The document in which to serialize.</param>
        public void Serialize(T obj, IniDocument ini)
        {
            lock (Lock)
            {
                SerializeType(_type, obj, ini);
            }
        }
    }
}
