/* Copyright (C) 2008 Samuel Fredrickson <kinghajj@gmail.com>
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

/* IniPatternMather.cs - "Tokenizer" for INI files.
 * 
 * This class uses regular expressions to break up lines of an INI file into
 * their important parts, in effect tokenizing them for the parser.
 */

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Mini
{
    /// <summary>
    /// The possible patterns in an INI.
    /// </summary>
    public enum IniPatternKind
    {
        Comment,
        Section,
        Setting,
        None,
    }

    /// <summary>
    /// Finds and matches patterns of INI files.
    /// </summary>
    public class IniPatternMatcher
    {
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
        private Match last_match = null;
        private StreamReader stream;
        private string last_comment, last_name, last_value;

        public IniPatternMatcher(StreamReader input)
        {
            stream = input;
        }

        /// <summary>
        /// Gets the kind of the next pattern in the stream, and stores the
        /// matched data.
        /// </summary>
        public IniPatternKind GetNextPattern()
        {
            IniPatternKind kind = IniPatternKind.None;
            Group group;

            // Reset the comment, name, and value to empty.
            last_comment = last_name = last_value = string.Empty;

            // Read a line from the stream and try to match it with a pattern.
            if(!stream.EndOfStream)
            {
                string line = stream.ReadLine();
                kind = Find(line);
            }

            // If the match was successful,
            if(last_match != null)
            {
                // If the match produced a comment, save it.
                if((group = last_match.Groups["comment"]) != null)
                    last_comment = group.Value;
                // If the match produced a name, save it.
                if((group = last_match.Groups["name"]) != null)
                    last_name = group.Value;
                // If the match produced a value, save it.
                if((group = last_match.Groups["value"]) != null)
                    last_value = group.Value;
            }

            return kind;
        }

        /// <summary>
        /// Returns true if the stream is finished.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                return stream.EndOfStream;
            }
        }

        /// <summary>
        /// The last parsed comment.
        /// </summary>
        public string LastComment
        {
            get
            {
                return last_comment;
            }
        }

        /// <summary>
        /// The most recently parsed name.
        /// </summary>
        public string LastName
        {
            get
            {
                return last_name;
            }
        }

        /// <summary>
        /// The most recently parsed value.
        /// </summary>
        public string LastValue
        {
            get
            {
                return last_value;
            }
        }

        /// <summary>
        /// Returns the kind of pattern that matches the input.
        /// </summary>
        private IniPatternKind Find(string input)
        {
            var found_kind = IniPatternKind.None;

            // Try to match the input against all know INI kinds.
            foreach(IniPatternKind kind in Enum.GetValues(typeof(IniPatternKind)))
            {
                // If there's a pattern for this kind,
                string pattern = GetPattern(kind);
                if(pattern != null)
                {
                    // Try to match it against the pattern for that kind.
                    Match match = Regex.Match(input, pattern);
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
        private string GetPattern(IniPatternKind kind)
        {
            string pattern = null;

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
    }
}
