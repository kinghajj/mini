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

namespace Mini
{
    class IniComment : IniPart
    {
        #region Constructors
        internal IniComment(string comment)
            : this(comment, false)
        {
        }

        internal IniComment(string comment, bool alone)
        {
            Comment = comment;
            Alone = alone;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes the comment to an output stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        override internal void Write(System.IO.StreamWriter writer)
        {
            if(!String.IsNullOrEmpty(Comment))
                foreach(var comment in
                        Comment.Split(Environment.NewLine.ToCharArray()))
                    writer.WriteLine("; {0}", comment);
            if(Alone)
                writer.WriteLine();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns true if the comment is by itself and does not belong to
        /// another part.
        /// </summary>
        internal bool Alone
        {
            get;
            private set;
        }

        /// <summary>
        /// Get or set the comment as a string.
        /// </summary>
        internal string Comment
        {
            get;
            set;
        }
        #endregion
    }
}
