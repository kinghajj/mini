/* Copyright (C) 2010 Samuel Fredrickson <kinghajj@gmail.com>
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

using System.IO;

namespace Mini
{
    /// <summary>
    /// An abstract class that represents part of an INI document.
    /// </summary>
    /// <remarks>
    /// The only thing that really unites the various parts of an INI is that
    /// they can be written to an output stream, so that's all that's required
    /// currently. Later, other methods may be required.
    /// </remarks>
    public abstract class IniPart
    {
        /// <summary>
        /// Writes the necessary number of newlines to the text writer.
        /// </summary>
        /// <param name="writer"></param>
        protected static void WriteNewlines(TextWriter writer, int newlines)
        {
            //System.Console.WriteLine("Writing {0} newlines.", newlines);
            for(int i = 0; i < newlines; ++i)
                writer.WriteLine();
        }

        /// <summary>
        /// The most abstract constructor for all INI parts.
        /// </summary>
        /// <param name="newlines">How many newlines should occur before the part.</param>
        internal IniPart(int newlines)
        {
            // Assume that there should be a newline
            Newlines = newlines;
        }

        /// <summary>
        /// Writes this part of an INI document to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        abstract internal void Write(TextWriter writer);

        /// <summary>
        /// The number of newlines that occur before the part.
        /// </summary>
        internal int Newlines { get; set; }
    }
}
