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

using System;
using System.IO;

namespace Mini
{
    /// <summary>
    /// Represents a comment in an INI document.
    /// </summary>
    /// <remarks>
    /// Usually, these objects are only created for "lone" comments, because
    /// other INI parts keep track of their comments separately, but they do use
    /// this class to write their comments to an output stream.
    /// </remarks>
    public class IniComment : IniPart
    {
        #region Constructors
        /// <summary>
        /// Construct a comment with the given text.
        /// </summary>
        /// <param name="comment">The text to use as a comment.</param>
        internal IniComment(string comment)
            : base(1)
        {
            Comment = comment;
        }
        #endregion

        #region Methods
        #region Internal
        /// <summary>
        /// Writes the comment to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        override internal void Write(TextWriter writer)
        {
            Write(writer, false);
        }

        /// <summary>
        /// Writes the comment to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="endWithNewline">Whether to write a newline after the
        /// comment.</param>
        internal void Write(TextWriter writer, bool endWithNewline)
        {
            Write(Comment, writer, endWithNewline, Newlines);
        }

        /// <summary>
        /// Writes a string comment to an output stream.
        /// </summary>
        /// <param name="comment">The comment to write.</param>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="endWithNewline">Whether to write a newline after the
        /// comment.</param>
        internal static void Write(string comment, TextWriter writer, bool endWithNewline)
        {
            Write(comment, writer, endWithNewline, 0);
        }

        /// <summary>
        /// Writes a string comment to an output stream.
        /// </summary>
        /// <param name="comment">The comment to write.</param>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="endWithNewline">Whether to write a newline after the
        /// comment.</param>
        /// <param name="newlines">How many newlines to write before the comment.</param>
        internal static void Write(string comment, TextWriter writer, bool endWithNewline, int newlines)
        {
            var newline = Environment.NewLine.ToCharArray();

            WriteNewlines(writer, newlines);
            if(!String.IsNullOrEmpty(comment))
                foreach(var c in comment.Split(newline, StringSplitOptions.RemoveEmptyEntries))
                    writer.WriteLine("; {0}", c);

            if(endWithNewline)
                writer.WriteLine();
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }
        #endregion
    }
}
