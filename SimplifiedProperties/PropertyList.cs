/*
 * Author: Tristan Chambers
 * Date: Thursday, November 7, 2013
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.Heroic-Intentions.net
 */
using System;
using System.Collections.Generic;

namespace SMPL.Props {
    /// <summary>
    /// Property list.
    /// </summary>
    public class PropertyList : PropertyEntry {

        #region Properties

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public string[] Keys {
            get {
                return _keys.ToArray();
            }
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public PropertyEntry[] Entries {
            get {
                return _entries.ToArray();
            }
        }

        #endregion // Properties

        #region Private Variables

        private List<string> _keys;
        private List<PropertyEntry> _entries;
        private int _lastLoadedIndex;

        #endregion // Private Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyList"/> class.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public PropertyList(string @key) 
            : this(@key, string.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyList"/> class.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="comment">Comment.</param>
        public PropertyList(string @key, string @comment)
            : base(@key, string.Empty, @comment) {
            _keys = new List<string>();
            _entries = new List<PropertyEntry>();
        }

        /// <summary>
        /// Adds a new property entry to the list.
        /// Note that this does not mark the property to save.
        /// </summary>
        /// <param name="file">The property file that is the parent to the property.</param>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Value.</param>
        /// <param name="comment">Comment.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void NewEntry<T>(string @key, T @defaultValue, string @comment) {
            AddEntry(new PropertyEntry(@key, @defaultValue.ToString(), @comment));
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="entry">Entry.</param>
        public PropertyEntry AddEntry(PropertyEntry @entry) {
            if(@entry == null)
                throw new PropertyException("A null entry was added to a property list.", this);

            var alias = @entry.Key.Equals("#") || @entry.Key.Equals(@"\n") ?
                        @entry.Key : Tools.GetAlias(@entry.Key, Keys);
            if(_lastLoadedIndex + 1 >= _keys.Count)
                _keys.Add(alias);
            else
                _keys.Insert(_lastLoadedIndex + 1, alias);
            @entry.Key = alias;
            if(_lastLoadedIndex + 1 >= _entries.Count)
                _entries.Add(@entry);
            else
                _entries.Insert(_lastLoadedIndex + 1, @entry);

            _lastLoadedIndex = _keys.Count - 1;
            return @entry;
        }

        /// <summary>
        /// Contains the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        public bool Contains(string @key) {
            return _keys.Contains(@key);
        }

        public int IndexOf(PropertyEntry @entry) {
            return _entries.IndexOf(@entry);
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="key">Key.</param>
        public PropertyEntry GetEntry(string @key) {
            PropertyEntry entry = null;

            for(int i = 0; i < _keys.Count; i++) {
                if(_keys[i].Equals(@key) && _entries[i].GetType() == typeof(PropertyEntry)) {
                    entry = _entries[i];
                }
            }

            return entry;
        }

        /// <summary>
        /// Builds the and read.
        /// </summary>
        /// <returns>The and read.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="comment">Comment.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T LoadEntry<T>(string @key, T @defaultValue) {
            return LoadEntry<T>(@key, @defaultValue, string.Empty);
        }

        /// <summary>
        /// Builds the and read.
        /// </summary>
        /// <returns>The and read.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="comment">Comment.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T LoadEntry<T>(string @key, T @defaultValue, string @comment) {
            T val = @defaultValue;

            if(Contains(@key)) {
                var entry = GetEntry(@key);
                entry.Loaded = true;
                val = entry.GetValue<T>();
            } else {
                var entry = AddEntry(new PropertyEntry(@key, @defaultValue.ToString(), @comment));
                entry.Loaded = true;
            }

            return val;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="key">Key.</param>
        public PropertyList GetList(string @key) {
            PropertyList list = null;

            for(int i = 0; i < _keys.Count; i++) {
                if(_keys[i].Equals(@key) && _entries[i].GetType() == typeof(PropertyList)) {
                    list = (PropertyList) _entries[i];
                }
            }

            return list;
        }

        /// <summary>
        /// Gets/Creates a list and marks it as loaded.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="key">Key.</param>
        /// <param name="comment">Comment.</param>
        public PropertyList LoadList(string @key, string @comment) {
            PropertyList list = GetList(@key);

            if(list == null) {
                list = new PropertyList(@key, @comment);
                AddEntry(list);
                list.Loaded = true;
            }

            list.Loaded = true;

            return list;
        }
    }
}









