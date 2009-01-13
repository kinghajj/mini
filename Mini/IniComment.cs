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

/* IniComment.cs - Represents a comment in an INI file.
 * 
 * Usually, these objects are only created for "lone" comments, because other
 * INI parts keep track of their comments separately, but they do use this class
 * to write their comments to an output stream.
 */

using System;
using System.IO;

namespace Mini
{
    class IniComment : IniPart
    {
        #region Constructors
        internal IniComment(string comment)
        {
            Comment = comment;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes the comment to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        override internal void Write(StreamWriter writer)
        {
            Write(writer, true);
        }

        /// <summary>
        /// Writes the comment to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="endWithNewline">Whether to write a newline after the comment.</param>
        internal void Write(StreamWriter writer, bool endWithNewline)
        {
            var newline = Environment.NewLine.ToCharArray();

            if(!String.IsNullOrEmpty(Comment))
                foreach(var comment in Comment.Split(newline, StringSplitOptions.RemoveEmptyEntries))
                    writer.WriteLine("; {0}", comment);

            if(endWithNewline)
                writer.WriteLine();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the comment as a string.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }
        #endregion
    }
}
