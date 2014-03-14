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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Mini
{
    /// <summary>
    /// The shared base class of <c>IniSerializer</c>.
    /// </summary>
    /// <remarks>
    /// Static fields in generic classes are duplicated for each combination of type parameters. If, however, a generic
    /// class derives from a non-generic class with static fields, then they are shared between all combinations. Thus,
    /// this class implements real work, and <c>IniSerializer</c> simply provides a type- and thread-safe interface.
    /// </remarks>
    public class IniSerializerBase
    {
        private struct Target
        {
            public PropertyInfo Property { get; set; }
            public Attribute Attribute { get; set; }
        }

        private static readonly IDictionary<Type, IList<Target>> Targets = new Dictionary<Type, IList<Target>>();
        private static readonly ISet<Type> Completed = new HashSet<Type>();
        private static readonly ISet<Type> Processing = new HashSet<Type>();
        private static readonly Type StringType = typeof (string);

        internal protected static readonly object Lock = new object();

        internal protected static object DeserializeType(Type type, IniDocument ini)
        {
            var obj = Activator.CreateInstance(type);
            foreach(var target in Targets[type])
            {
                var propType = target.Property.PropertyType;
                object val;
                var containerAttr = target.Attribute as IniValueContainerAttribute;
                var valAttr = target.Attribute as IniValueAttribute;
                // if this property is a container, recurse to deserialize it
                if(containerAttr != null)
                    val = DeserializeType(propType, ini);
                // otherwise, extract this property's value from the ini
                else if(valAttr != null)
                {
                    var valString = ini[valAttr.Section][valAttr.Key].Value;
                    if (string.IsNullOrEmpty(valString))
                        valString = valAttr.DefaultValue;
                    var typeConverter = TypeDescriptor.GetConverter(propType);
                    val = typeConverter.ConvertFromString(valString);
                }
                // ??? internal error!
                else throw new Exception();
                // set the property to whichever value was computed
                target.Property.SetValue(obj, val, BindingFlags.SetProperty, null, null, null);
            }
            return obj;
        }

        internal protected static void SerializeType(Type type, object obj, IniDocument ini)
        {
            foreach (var target in Targets[type])
            {
                var val = target.Property.GetValue(obj, BindingFlags.GetProperty, null, null, null);
                var containerAttr = target.Attribute as IniValueContainerAttribute;
                var valAttr = target.Attribute as IniValueAttribute;
                if(containerAttr != null)
                    SerializeType(target.Property.PropertyType, val, ini);
                else if(valAttr != null)
                {
                    var typeConverter = TypeDescriptor.GetConverter(target.Property.PropertyType);
                    var valString = typeConverter.ConvertToString(val);
                    ini[valAttr.Section][valAttr.Key].Value = valString;
                }
                // ??? internal error!
                else throw new Exception();
            }
        }

        internal protected static void Process(Type type)
        {
            // if already completed or processing this type, ignore
            if (Completed.Contains(type) || Processing.Contains(type))
                return;
            Contract.Requires(type.GetCustomAttributes(false).Any(attr => attr.GetType() == typeof(IniValueContainerAttribute)));
            // otherwise, now we are processing this type
            Processing.Add(type);
            var targetList = new List<Target>();
            Targets.Add(type, targetList);

            foreach (var prop in type.GetProperties())
            {
                // analyze the property's type's attributes to decide how to handle it
                Attribute targetAttr;
                var propType = prop.PropertyType;
                var containerAttr = propType.GetCustomAttributes(false).OfType<IniValueContainerAttribute>().FirstOrDefault();
                // if this type is another container, recurse on it
                if (containerAttr != null)
                {
                    Process(propType);
                    targetAttr = containerAttr;
                }
                else
                {
                    var propTypeConverter = TypeDescriptor.GetConverter(propType);
                    // ignore types that can't be converted to/from strings
                    if (!propTypeConverter.CanConvertFrom(StringType) || !propTypeConverter.CanConvertTo(StringType))
                        continue;
                    // otherwise, just create a target entry for this property
                    targetAttr = prop.GetCustomAttributes(false).OfType<IniValueAttribute>().FirstOrDefault();
                }
                if (targetAttr != null)
                    targetList.Add(new Target { Property = prop, Attribute = targetAttr });
            }

            // and now we're finished with the type
            Processing.Remove(type);
            Completed.Add(type);
        }

        internal IniSerializerBase()
        {
        }
    }
}
