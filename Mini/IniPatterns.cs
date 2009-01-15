/* Copyright (C) 2009 Samuel Fredrickson <kinghajj@gmail.com>
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

/* IniPatterns.cs - "Tokenizer" for INI files.
 * 
 * This class uses regular expressions to break up lines of an INI file into
 * their important parts, in effect tokenizing them for the parser.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Mini
{
    /// <summary>
    /// The possible patterns in an INI.
    /// </summary>
    internal enum IniPatternKind
    {
        Comment,
        Section,
        Setting,
        None,
    }

    /// <summary>
    /// Contains information on a parsed INI pattern.
    /// </summary>
    internal class IniPattern
    {
        internal string Comment { get; set; }

        internal IniPatternKind Kind { get; set; }

        internal string Name { get; set; }

        internal string Value { get; set; }
    }

    /// <summary>
    /// Finds and matches patterns of INI files.
    /// </summary>
    internal class IniPatterns : IEnumerable<IniPattern>
    {
        private Match last_match;
        private StreamReader stream;

        #region Constructors
        public IniPatterns(StreamReader input)
        {
            stream = input;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the kind of the next pattern in the stream, and stores the
        /// matched data.
        /// </summary>
        private IniPattern GetNextPattern()
        {
            string comment = string.Empty,
                   name    = string.Empty,
                   value   = string.Empty;
            IniPatternKind kind;

            // Read a line from the stream and try to match it with a pattern.
            // If the match was successful,
            if( (kind = MatchWithPattern(stream.ReadLine())) != IniPatternKind.None)
            {
                Group group;
                // If the match produced a comment, save it.
                if((group = last_match.Groups["comment"]) != null)
                    comment = group.Value;
                // If the match produced a name, save it.
                if((group = last_match.Groups["name"]) != null)
                    name = group.Value;
                // If the match produced a value, save it.
                if((group = last_match.Groups["value"]) != null)
                    value = group.Value;
            }

            return new IniPattern()
            {
                Comment = comment,
                Kind    = kind,
                Name    = name,
                Value   = value,
            };
        }

        /// <summary>
        /// Returns the kind of pattern that matches the input.
        /// </summary>
        private IniPatternKind MatchWithPattern(string input)
        {
            var found_kind = IniPatternKind.None;

            // Try to match the input against all know INI kinds.
            foreach(IniPatternKind kind in
                    Enum.GetValues(typeof(IniPatternKind)))
            {
                // If there's a pattern for this kind,
                string pattern = GetPattern(kind);
                if(!string.IsNullOrEmpty(pattern))
                {
                    // Try to match it against the pattern for that kind.
                    var match = Regex.Match(input, pattern);
                    // If the match works, store it.
                    if(match.Success)
                    {
                        found_kind = kind;
                        last_match = match;
                        break;
                    }
                }
            }

            return found_kind;
        }

        /// <summary>
        /// Returns the pattern string for a kind of pattern.
        /// </summary>
        private static string GetPattern(IniPatternKind kind)
        {
            string pattern = string.Empty;

            switch(kind)
            {
                case IniPatternKind.Comment:
                    pattern = comment;
                    break;
                case IniPatternKind.Section:
                    pattern = section;
                    break;
                case IniPatternKind.Setting:
                    pattern = setting;
                    break;
            }

            return pattern;
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an iterator for the file's sections.
        /// </summary>
        /// <returns>
        /// An enumerator of patterns processed from the input stream.
        /// </returns>
        public IEnumerator<IniPattern> GetEnumerator()
        {
            while(!stream.EndOfStream)
                yield return GetNextPattern();
        }

        /// <summary>
        /// Silly required function.
        /// </summary>
        /// <returns>An enumerator of IniPatterns.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Regular Expressions
        /* Comments start with any number of spaces, then a semicolon, possibly
         * followed by a space, then any number of characters, which are the
         * comment, then the string ends.
         */
        private static string comment =
            @"^\s*;+\s?(?<comment>.*)$";
        /* Sections start with any number of spaces, then a [, then any number
         * of spaces, then one or more characters, which are the name, then any
         * number of spaces, then a ], then any number of spaces, then possibly
         * any number of semicolons, then possibly any number of characters,
         * which are the comment, then the string ends.
         */
        private static string section =
            @"^\s*\[\s*(?<name>[\w\s\.]*\w)\s*\]\s*;*\s?(?<comment>.*)$";
        private static string setting =
            /* Settings start with any number of spaces, then one or more word
             * characters, which are the name, then any number of spaces, an
             * equal sign, then any number of spaces...
             */
            @"^\s*(?<name>\w+)\s*=\s*" +
            /* ...then any number of non-semicolon characters, which are the
             * value, then by any number of spaces...
             */
            @"(?<value>[^\;]*)\s*" +
            /* ...then by any number of semicolons, possibly a space, then any
             * number of characters, which are the comment, then the string
             * ends.
             */
            @";*\s?(?<comment>.*)$";
        #endregion
    }
}
