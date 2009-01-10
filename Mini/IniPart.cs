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

/* IniPart.cs - An abstract class that represents part of an INI file.
 * 
 * The only thing that really unites the various parts of an INI is that they
 * can be written to an output stream, so that's all that's required currently.
 * Later, other methods may be required.
 */

using System;
using System.IO;

namespace Mini
{
    public abstract class IniPart
    {
        abstract internal void Write(StreamWriter writer);
    }
}
