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

/* IniParser.cs - The parser for the library.
 * 
 * This is the heart of the library. Without this, the rest is just a bunch of
 * meaningless structure with no data. So, this deserves some comments.
 * 
 * Because INI files are flat and line-based, reading them is fairly simple:
 * just read a line, find a pattern that matches it, parse that into the correct
 * data structures, and move on.
 * 
 * There are three kinds of patterns: comments, sections, and settings.
 * 
 * This parser uses IniPatternMather to tokenize the INI. Comments that appear
 * directly before a section or setting are joined with that section/setting's
 * "true" comment on parsing.
 * 
 * Lone comments are currently ignored.
 */

using System.IO;

namespace Mini
{
    /// <summary>
    /// Parses a stream into the INI data structures.
    /// </summary>
    public class IniParser
    {
        /// <summary>
        /// Parses the INI.
        /// </summary>
        /// <param name="ini">The IniFile object into which the stream will be
        /// parsed.</param>
        /// <param name="stream">An input stream from which the INI will be
        /// parsed.</param>
        internal static void Parse(IniFile ini, StreamReader stream)
        {
            var matcher = new IniPatternMatcher(stream);
            string comment = string.Empty;
            IniSection section = null;
            IniSetting setting = null;

            while(!matcher.EndOfStream)
            {
                var kind = matcher.GetNextPattern();

                switch(kind)
                {
                    case IniPatternKind.Comment:
                        comment = JoinComments(string.Empty, comment, matcher.LastComment);
                        break;
                    case IniPatternKind.Section:
                        section = ini[matcher.LastName];
                        section.Comment = JoinComments(section.Comment, comment, matcher.LastComment);
                        comment = string.Empty;
                        break;
                    case IniPatternKind.Setting:
                        setting = section[matcher.LastName];
                        setting.Comment = JoinComments(setting.Comment, comment, matcher.LastComment);
                        setting.Value = matcher.LastValue;
                        comment = string.Empty;
                        break;
                    case IniPatternKind.None:
                        comment = string.Empty;
                        break;
                }
            }
        }

        /// <summary>
        /// Joins together multiple comments.
        /// </summary>
        /// <param name="current">The current comment.</param>
        /// <param name="built">The current built-up comment.</param>
        /// <param name="last">The last read comment.</param>
        /// <returns>A new, joined comment.</returns>
        private static string JoinComments(string current, string built, string last)
        {
            string ret = string.Empty;

            if(current == string.Empty)
            {
                if(built == string.Empty)
                    ret = last;
                else
                    ret = built + "\n" + last;
            }
            else
                ret = current + "\n" + built + last;

            return ret;
        }
    }
}
