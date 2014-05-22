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
using System.Linq;
using System.Text;

namespace Mini
{
    /// <summary>
    /// Represents an INI document.
    /// </summary>
    /// <remarks>
    /// This is the core class of the library. It is the one and only entrypoint
    /// into the library, and, hopefully, is easy to use.
    /// </remarks>
    public class IniDocument : IEnumerable<IniSection>, IEquatable<IniDocument>
    {
        private readonly OrderedDictionaryList<string, IniPart> _parts;

        #region Constructors

        public IniDocument(Encoding encoding)
        {
            _parts   = new OrderedDictionaryList<string, IniPart>();
            Path     = "";
            Encoding = encoding;
        }

        /// <summary>
        /// Creates a new, blank INI document.
        /// </summary>
        public IniDocument()
            : this(Encoding.Default)
        {
        }

        /// <summary>
        /// Deserializes an INI document from a stream.
        /// </summary>
        /// <param name="stream">
        /// The stream from which to read the INI document.
        /// </param>
        public IniDocument(TextReader stream)
            : this()
        {
            Parse(stream);
        }

        /// <summary>
        /// Opens or creates an INI document from a path with the given encoding.
        /// </summary>
        /// <param name="path">
        /// The path to the INI document.
        /// </param>
        /// <param name="encoding">
        /// An encoding with which to interpret the INI document.
        /// </param>
        public IniDocument(string path, Encoding encoding)
            : this(encoding)
        {
            Path = path;
            using (var stream = new StreamReader(File.Open(path, FileMode.OpenOrCreate), encoding))
            {
                Parse(stream);
            }
        }

        /// <summary>
        /// Opens or creates an INI document from a path with the default encoding.
        /// </summary>
        /// <param name="path">
        /// The path to the INI document.
        /// </param>
        public IniDocument(string path)
            : this(path, Encoding.Default)
        {
        }
        #endregion

        #region Methods
        #region Public
        public bool Equals(IniDocument other)
        {
            return !ReferenceEquals(other, null) && ((IEnumerable<IniPart>) _parts).SequenceEqual(other._parts);
        }

        /// <summary>
        /// Determines whether an IniDocument contains an IniSection with the given
        /// name.
        /// </summary>
        /// <param name="name">The name of the section to find.</param>
        /// <returns>true if the section found, else false.</returns>
        public bool HasSection(string name)
        {
            return _parts.ContainsKey(name);
        }

        /// <summary>
        /// Removes a section from the INI document.
        /// </summary>
        /// <remarks>
        /// The section is only removed if the object belongs to this document.
        /// If an object from another document is passed, nothing happens.
        /// </remarks>
        /// <param name="section">The section to remove.</param>
        public void Remove(IniSection section)
        {
            var ourSection = FindSection(section.Name);
            if(ourSection == section)
                Remove(section.Name);
        }

        /// <summary>
        /// Removes a section by its name.
        /// </summary>
        /// <param name="sectionName">The name of the section to remove.</param>
        public void Remove(string sectionName)
        {
            _parts.Remove(sectionName);
        }

        /// <summary>
        /// Saves the document using its stored path.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// Throw when Path is null or empty.
        /// </exception>
        public void Write()
        {
            if(string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path is null or empty.");
            Write(Path);
        }

        /// <summary>
        /// Saves the document using the given path.
        /// </summary>
        /// <param name="path">The path to save the document to.</param>
        public void Write(string path)
        {
            using(var writer = new StreamWriter(path, false, Encoding))
                Write(writer);
        }

        /// <summary>
        /// Writes the document using the given TextWriter.
        /// </summary>
        /// <param name="writer">The TextWriter to write the document to.
        /// </param>
        public void Write(TextWriter writer)
        {
            foreach(var part in _parts.Values)
                part.Write(writer);
        }
        #endregion

        #region Private
        /// <summary>
        /// Finds a section by name.
        /// </summary>
        /// <param name="name">The name of the section to find.</param>
        /// <returns>The found section or null.</returns>
        private IniSection FindSection(string name)
        {
            IniPart part;
            _parts.TryGetValue(name, out part);
            return part as IniSection;
        }

        /// <summary>
        /// Parses an input stream into as an INI document.
        /// </summary>
        /// <param name="stream">The stream to parse.</param>
        private void Parse(TextReader stream)
        {
            var comment = string.Empty;
            var newlines = 0;
            IniSection section = null;

            foreach(var pattern in new IniPatterns(stream))
            {
                switch(pattern.Kind)
                {
                    case IniPatternKind.Comment:
                        comment = JoinComments(string.Empty,
                                               comment,
                                               pattern.Comment);
                        break;
                    case IniPatternKind.Section:
                        // Get this section by its name.
                        section = this[pattern.Name];
                        // Set its comment and value.
                        section.Comment = JoinComments(section.Comment,
                                                       comment,
                                                       pattern.Comment);
                        section.NewLines = newlines;
                        newlines = 0;
                        comment = string.Empty;
                        break;
                    case IniPatternKind.Setting:
                        // If we're within a section,
                        if(section != null)
                        {
                            // Get this setting by its key.
                            IniSetting setting = section[pattern.Name];
                            // Set its comment and value.
                            setting.Comment = JoinComments(setting.Comment,
                                                           comment,
                                                           pattern.Comment);
                            setting.Value = pattern.Value;
                            setting.NewLines = newlines;
                            newlines = 0;
                            // Erase the old built-up comment.
                            comment = string.Empty;
                        }
                        break;
                    case IniPatternKind.None:
                        // If there's a stored comment, add it then clear it.
                        if(!string.IsNullOrEmpty(comment))
                        {
                            var newComment = new IniComment(comment) {NewLines = newlines};
                            if(section != null)
                                section.AddPart(newComment);
                            else
                                _parts.AddUnkeyed(newComment);
                            comment = string.Empty;
                            newlines = 0;
                        }
                        newlines++;
                        break;
                }
            }
        }

        /// <summary>
        /// Joins together multiple comments.
        /// </summary>
        /// <param name="previous">A section's or setting's previous comment.</param>
        /// <param name="built">The current built-up comment.</param>
        /// <param name="last">The last parsed comment.</param>
        /// <returns>A new, joined comment.</returns>
        private static string JoinComments(string previous, string built,
                                           string last)
        {
            return string.Join(Environment.NewLine,
                               new[] { previous, built, last });
        }
        #endregion
        #endregion

        #region Enumerator
        /// <summary>
        /// Gets an iterator for the document's sections.
        /// </summary>
        /// <returns>
        /// An enumerator of sections of the INI document.
        /// </returns>
        public IEnumerator<IniSection> GetEnumerator()
        {
            return _parts.Values.OfType<IniSection>().GetEnumerator();
        }

        /// <summary>
        /// Silly required function.
        /// </summary>
        /// <returns>An enumerator of IniParts.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Indexers
        /// <summary>
        /// Gets a document's section via its name.
        /// </summary>
        /// <remarks>
        /// If a section with the given name does not exist, it will be created
        /// and have no settings.
        /// </remarks>
        public IniSection this[string name]
        {
            get
            {
                IniSection found = FindSection(name);

                // if not, create it and add it.
                if(found == null)
                    _parts.Add(name, found = new IniSection(name, string.Empty));

                return found;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the INI's encoding.
        /// </summary>
        public Encoding Encoding
        {
            get;
            set;
        }

        /// <summary>
        /// Get an INI document's path.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an enumeration of the document's parts.
        /// </summary>
        public IEnumerable<IniPart> Parts
        {
            get
            {
                return _parts.Values;
            }
        }
        #endregion
    }
}
