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
