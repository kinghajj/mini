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
using System.Linq;

namespace Mini
{
    /// <summary>
    /// A dictionary-list hybrid that maintains an order for items inserted.
    /// </summary>
    /// <remarks>
    /// <para>The purpose of this data structure is primarily to be a dictionary, but it is also a list because items
    /// can be inserted without a key. When <c>Values</c> is retrieved, the values in the dictionary and list are
    /// combined and sorted by their indices; therefore, it's an O(n log n) operation.</para>
    /// <para><c>System.UInt64</c> is the type of the items' indices, so overflow shouldn't be a problem in
    /// practice. This data structure is not thread-safe.</para>
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class OrderedDictionaryList<TKey, TValue> : ICollection<TValue>, IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, Node> _dict = new Dictionary<TKey, Node>();
        private readonly ICollection<Node> _unkeyed = new LinkedList<Node>();
        private ulong _lastIndex;

        /// <summary>
        /// Safely create a new node to be inserted into the dictionary.
        /// </summary>
        /// <param name="value">The value to put into the node.</param>
        /// <returns>A newly-created Node containing the value.</returns>
        private Node CreateNode(TValue value)
        {
            return new Node {Value = value, Index = _lastIndex++};
        }

        /// <summary>
        /// Internally, the dictionary's values are actually Nodes.
        /// It also stores an index so that the nodes can be ordered.
        /// </summary>
        private class Node : IComparable<Node>
        {
            public TValue Value;
            public ulong Index;

            public int CompareTo(Node other)
            {
                return Index.CompareTo(other.Index);
            }
        }

        /// <summary>
        /// Not all values need to be associated with a key,
        /// but unkeyed values are still ordered correctly.
        /// </summary>
        /// <param name="value">The unkeyed value to add.</param>
        public void AddUnkeyed(TValue value)
        {
            _unkeyed.Add(CreateNode(value));
        }

        /// <summary>
        /// Get a sequence of all values in the order in which they were added.
        /// </summary>
        /// <remarks>
        /// This concatenates then sorts the values, so it requires O(n log n) time.
        /// This sorting occurs on an array that is a copy of the internal nodes,
        /// requiring O(n) space.
        /// </remarks>
        public IEnumerable<TValue> Values
        {
            get
            {
                var nodes = _dict.Values.Concat(_unkeyed).ToArray();
                Array.Sort(nodes);
                return nodes.Select(n => n.Value);
            }
        }

        #region IDictionary implementation
        public void Add(TKey key, TValue value)
        {
            _dict.Add(key, CreateNode(value));
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _dict.Keys; }
        }

        public bool Remove(TKey key)
        {
            return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Node node;
            var ret = _dict.TryGetValue(key, out node);
            value = node != null ? node.Value : default(TValue);
            return ret;
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return Values.ToList();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _dict[key].Value;
            }
            set
            {
                _dict[key] = CreateNode(value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dict.Clear();
            _unkeyed.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dict.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var kvp in this)
            {
                if (arrayIndex >= array.Length)
                    break;
                array[i] = kvp;
            }
        }

        public int Count
        {
            get { return _dict.Count + _unkeyed.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _dict.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return
                _dict.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value.Value)).
                Concat(_unkeyed.Select(node => new KeyValuePair<TKey, TValue>(default(TKey), node.Value))).
                GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region ICollection implementation
        public void Add(TValue item)
        {
            AddUnkeyed(item);
        }

        public bool Contains(TValue item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TValue item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
        #endregion
    }
}
