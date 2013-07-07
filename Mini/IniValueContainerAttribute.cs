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
