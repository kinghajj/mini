using System;

namespace Mini
{
    /// <summary>
    /// Signifies that a class contains properties with <c>IniValue</c> attributes.
    /// </summary>
    /// <remarks>
    /// Any class that has properties with the <c>IniValue</c> attribute must have this
    /// one for <c>IniSerializer</c> to work.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class IniValueContainerAttribute : Attribute
    {
    }
}
