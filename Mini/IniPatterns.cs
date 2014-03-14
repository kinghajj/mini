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
        Eof,
    }

    /// <summary>
    /// Contains information on a INI pattern.
    /// </summary>
    internal struct IniPattern
    {
        /// <summary>
        /// Gets or sets the comment of an INI pattern.
        /// </summary>
        internal string Comment { get; set; }

        /// <summary>
        /// Gets or sets the kind of an INI pattern.
        /// </summary>
        internal IniPatternKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the name of an INI pattern.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of an INI pattern.
        /// </summary>
        internal string Value { get; set; }
    }

    /// <summary>
    /// Finds and matches patterns of INI documents.
    /// </summary>
    /// <remarks>
    /// This class uses regular expressions to break up lines of an INI document
    /// into their important parts, in effect tokenizing them for the parser.
    /// </remarks>
    internal class IniPatterns : IEnumerable<IniPattern>
    {
        private Match _lastMatch;
        private readonly TextReader _stream;

        #region Constructors
        /// <summary>
        /// Creates an IniPatterns reader to match patterns found in the given
        /// stream.
        /// </summary>
        /// <param name="input">The stream to read patterns from.</param>
        public IniPatterns(TextReader input)
        {
            _stream = input;
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

            // Read a line from the stream and try to match it with a pattern.
            // If the match was successful,
            var kind = MatchWithPattern(_stream.ReadLine());
            if(kind != IniPatternKind.None && kind != IniPatternKind.Eof)
            {
                Group group;
                // If the match produced a comment, save it.
                if((group = _lastMatch.Groups["comment"]) != null)
                    comment = group.Value;
                // If the match produced a name, save it.
                if((group = _lastMatch.Groups["name"]) != null)
                    name = group.Value;
                // If the match produced a value, save it.
                if((group = _lastMatch.Groups["value"]) != null)
                    value = group.Value;
            }

            return new IniPattern
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
            var foundKind = IniPatternKind.None;

            // Nothing left to do!
            if (input == null)
            {
                return IniPatternKind.Eof;
            }

            // Try to match the input against all known INI kinds.
            foreach (var pattern in Patterns)
            {
                var match = pattern.Item1.Match(input);
                if (!match.Success)
                    continue;
                foundKind = pattern.Item2;
                _lastMatch = match;
                break;
            }

            return foundKind;
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an enumerator for the document's sections.
        /// </summary>
        /// <returns>
        /// An enumerator of patterns processed from the input stream.
        /// </returns>
        public IEnumerator<IniPattern> GetEnumerator()
        {
            while(true)
            {
                var pattern = GetNextPattern();
                if (pattern.Kind == IniPatternKind.Eof)
                    yield break;
                yield return pattern;
            }
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
        private static readonly Regex Comment =
            new Regex(@"^\s*;+\s?(?<comment>.*)$", RegexOptions.Compiled);
        /* Sections start with any number of spaces, then a [, then any number
         * of spaces, then one or more characters, which are the name, then any
         * number of spaces, then a ], then any number of spaces, then possibly
         * any number of semicolons, then possibly any number of characters,
         * which are the comment, then the string ends.
         */
        private static readonly Regex Section = new Regex(
            @"^\s*\[\s*(?<name>[\w\s\.]*\w)\s*\]\s*;*\s?(?<comment>.*)$",
            RegexOptions.Compiled);
        private static readonly Regex Setting =
            /* Settings start with any number of spaces, then one or more word
             * characters, which are the name, then any number of spaces, an
             * equal sign, then any number of spaces...
             */
            new Regex(@"^\s*(?<name>.+)\s*=\s*" +
            /* ...then any number of characters, which are the value, then by
             * any number of spaces.
             */
            @"(?<value>.*)\s*$", RegexOptions.Compiled);

        private static readonly Tuple<Regex, IniPatternKind>[] Patterns =
            {
                Tuple.Create(Comment, IniPatternKind.Comment),
                Tuple.Create(Section, IniPatternKind.Section),
                Tuple.Create(Setting, IniPatternKind.Setting),
            };
        #endregion
    }
}
