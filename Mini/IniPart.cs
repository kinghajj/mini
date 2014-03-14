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
        /// Writes the given number of newlines to the text writer.
        /// </summary>
        /// <param name="writer">What to write the newlines to.</param>
        /// <param name="newLines">How many newlines to write.</param>
        protected static void WriteNewLines(TextWriter writer, int newLines)
        {
            //System.Console.WriteLine("Writing {0} newlines.", newlines);
            for(int i = 0; i < newLines; ++i)
                writer.WriteLine();
        }

        /// <summary>
        /// The most abstract constructor for all INI parts.
        /// </summary>
        /// <param name="newLines">How many newlines should occur before the part by default.</param>
        internal IniPart(int newLines)
        {
            // Assume that there should be a newline
            NewLines = newLines;
        }

        /// <summary>
        /// Writes this part of an INI document to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        abstract internal void Write(TextWriter writer);

        /// <summary>
        /// The number of newlines that occur before the part.
        /// </summary>
        /// <remarks>
        /// Before this was added, Mini didn't bother to remember any unimportant
        /// text formatting, since it doesn't affect the value content of the INI.
        /// However, the principle user of this library didn't like that, since the
        /// special formatting helps him read the INI more easily. So now, when an INI
        /// is being parsed, the number of newlines that occur before a part are counted
        /// and remembered, so that when the document is written back to a file, it more
        /// closely resembles the original.
        /// </remarks>
        internal int NewLines { get; set; }
    }
}
