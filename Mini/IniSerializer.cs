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
