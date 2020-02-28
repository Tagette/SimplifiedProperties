/*
 * Author: Tristan Chambers
 * Date: Thursday, November 7, 2013
 * Email: Tristan.Chambershotmail.com
 * Website: Tristan.PaperHatStudios.com
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

namespace SMPL.Props
{
    /// <summary>
    /// Property list.
    /// </summary>
    public class PropertyList : PropertyEntry
    {

        #region Properties

        /// <summary>
        /// Gets a read-only list of keys.
        /// </summary>
        public ReadOnlyCollection<string> Keys
        {
            get
            {
                return _keys.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public ReadOnlyCollection<PropertyEntry> Entries
        {
            get
            {
                return _entries.AsReadOnly();
            }
        }

        /// <summary>
        /// The comment on the group close bracket.
        /// </summary>
        /// <value></value>
        public string CloseComment { get; set; }

        /// <summary>
        /// Determines the amount of entries in the list.
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
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
        /// <param name="key">Key.</param>
        public PropertyList(string key)
            : this(key, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyList"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="comment">Comment.</param>
        public PropertyList(string key, string comment)
            : this(key, comment, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyList"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="comment">Comment.</param>
        /// <param name="closeComment">The comment on the closing curly brace.</param>
        public PropertyList(string key, string comment, string closeComment)
            : base(key, string.Empty, comment)
        {
            CloseComment = closeComment;
            _keys = new List<string>();
            _entries = new List<PropertyEntry>();
        }

        /// <summary>
        /// Gets or sets the <see cref="SMPL.Props.PropertyList"/> with the specified Key.
        /// </summary>
        /// <param name="Key">Key.</param>
        public PropertyEntry this [string Key]
        {
            get
            {
                return GetEntry(Key);
            }
            set
            {
                if (value == null)
                {
                    Remove(Key);
                    return;
                }
                if (!Contains(value.Key))
                {
                    AddEntry(value);
                }
                else
                {
                    int index = IndexOf(value.Key);
                    SetAt(index, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SMPL.Props.PropertyList"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public PropertyEntry this [int index]
        {
            get
            {
                if (index >= Count || index < 0)
                {
                    return null;
                }
                return _entries[index];
            }
            set
            {
                if (index >= Count || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                if (value == null)
                {
                    RemoveAt(index);
                    return;
                }
                _entries[index] = value;
            }
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
        public void Add<T>(string key, T defaultValue, string comment)
        {
            AddEntry(new PropertyEntry(key, 
                    defaultValue == null
				? null
				: defaultValue.ToString(), comment));
        }

        /// <summary>
        /// Adds a new array entry to the list.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AddArrayValue<T>(T value)
        {
            AddArrayValue(value, null);
        }

        /// <summary>
        /// Adds a new array entry to the list.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comment">The comment.</param>
        public void AddArrayValue<T>(T value, string comment)
        {
            AddEntry(new ArrayEntry(value + "", comment));
        }

        /// <summary>
        /// Adds a new line entry.
        /// </summary>
        public void NewLine()
        {
            var nextEntryToLoad = this[_lastLoadedIndex + 1];
            if (nextEntryToLoad != null)
            {
                if (nextEntryToLoad is NewLineEntry)
                {
                    nextEntryToLoad.Loaded = true;
                    _lastLoadedIndex++;
                }
            }
            else
            {
                
            }
//                AddEntry(new NewLineEntry(){ Loaded = true });
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="entry">Entry.</param>
        public PropertyEntry AddEntry(PropertyEntry entry)
        {
            if (entry == null)
                throw new PropertyException("A null entry was added to a property list.", this);

            var alias = entry is CommentEntry || entry is NewLineEntry || entry is BlockCommentEntry ?
                        entry.Key : Tools.GetAlias(entry.Key, Keys);
            if (_lastLoadedIndex + 1 >= _keys.Count)
                _keys.Add(alias);
            else
                _keys.Insert(_lastLoadedIndex + 1, alias);
            entry.Key = alias;
            if (_lastLoadedIndex + 1 >= _entries.Count)
                _entries.Add(entry);
            else
                _entries.Insert(_lastLoadedIndex + 1, entry);

            entry.Parent = this;
            _lastLoadedIndex = _keys.Count - 1;
            return entry;
        }

        /// <summary>
        /// Contains the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        public bool Contains(string key)
        {
            return _keys.Contains(key);
        }

        /// <summary>
        /// Finds the index of an entry by key.
        /// </summary>
        /// <param name="key">The key.</param>
        public int IndexOf(string key)
        {
            return _keys.IndexOf(key);
        }

        /// <summary>
        /// Finds the index of an entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public int IndexOf(PropertyEntry entry)
        {
            return _entries.IndexOf(entry);
        }

        /// <summary>
        /// Gets an entry using the key.
        /// </summary>
        /// <returns>Shit.</returns>
        /// <param name="key">The key used to get the entry.</param>
        public PropertyEntry GetEntry(string key)
        {
            PropertyEntry entry = null;

            for (int i = 0; i < Count; i++)
            {
                var eachEntry = _entries[i];
                if (_keys[i].Equals(key)
                    && (eachEntry.GetType() == typeof(PropertyEntry)
                    || eachEntry.GetType() == typeof(ConcatenatedEntry)))
                {
                    entry = eachEntry;
                    break;
                }
            }

            return entry;
        }

        /// <summary>
        /// Sets the entry at an index.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="entry">Entry.</param>
        public bool SetAt(int index, PropertyEntry entry)
        {
            if (index < 0 || index > Count)
                return false;
            _keys[index] = entry.Key;
            _entries[index] = entry;
            Modified = true;
            return true;
        }

        /// <summary>
        /// Inserts an entry at an index.
        /// Returns true if successful.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        public bool InsertAt(int index, PropertyEntry entry)
        {
            if (entry == null)
                return false;
            // Note that the index can be at Count.
            if (index > Count || index < 0)
                return false;
            if (Contains(entry.Key))
                return false;
            _keys.Insert(index, entry.Key);
            _entries.Insert(index, entry);
            return true;
        }

        /// <summary>
        /// Removes an entry from the list using it's key.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="key">Key.</param>
        public PropertyEntry Remove(string key)
        {
            return Remove(GetEntry(key));
        }

        /// <summary>
        /// Removes an entry from the list.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="entry">Entry.</param>
        public PropertyEntry Remove(PropertyEntry entry)
        {
            if (entry == null)
                return null;
            int index = IndexOf(entry);
            return RemoveAt(index);
        }

        /// <summary>
        /// Removes an entry at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public PropertyEntry RemoveAt(int index)
        {
            if (index >= _entries.Count || index < 0)
            {
                return null;
            }
            var entry = _entries[index];
            _keys.RemoveAt(index);
            _entries.RemoveAt(index);
            Modified = true;
            return entry;
        }

        /// <summary>
        /// Gets the value of an entry or creates the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public PropertyEntry Get(string key, object defaultValue)
        {
            return Get(key, defaultValue, string.Empty);
        }

        /// <summary>
        /// Gets the value of an entry or creates the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="comment">The comment for the entry.</param>
        public PropertyEntry Get(string key, object defaultValue, string comment)
        {
            PropertyEntry entry;
            if (Contains(key))
            {
                entry = GetEntry(key);
            }
            else
            {
                entry = AddEntry(new PropertyEntry(key, defaultValue.ToString(), comment));
            }

            return entry;
        }

        /// <summary>
        /// Sets the value of an entry or creates the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comment">The comment for the entry.</param>
        public PropertyEntry Set(string key, object newValue)
        {
            return Set(key, newValue, string.Empty);
        }

        /// <summary>
        /// Sets the value of an entry or creates the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comment">The comment for the entry.</param>
        public PropertyEntry Set(string key, object newValue, string comment)
        {
            PropertyEntry entry;
            if (Contains(key))
            {
                entry = GetEntry(key);
                entry.StringValue = newValue + "";
            }
            else
            {
                entry = AddEntry(new PropertyEntry(key, newValue + "", comment));
            }

            return entry;
        }

        /// <summary>
        /// Gets the value of an entry or creates the entry if it does not exist.
        /// This will mark the entry as loaded. See PropertyFile.CommentUnloadedEntries.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T Load<T>(string key, T defaultValue)
        {
            return Load<T>(key, defaultValue, string.Empty);
        }

        /// <summary>
        /// Gets the value of an entry or creates the entry if it does not exist.
        /// This will mark the entry as loaded. See PropertyFile.CommentUnloadedEntries.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="comment">The comment for the entry.</param>
        public T Load<T>(string key, T defaultValue, string comment)
        {
            var entry = Get(key, defaultValue, comment);
            entry.Loaded = true;
            return entry.GetValue<T>();
        }

        /// <summary>
        /// Gets all array entries that can be parsed to the specified type.
        /// </summary>
        public List<ArrayEntry> GetArrayEntries<T>()
        {
            var arrayEntries = new List<ArrayEntry>();
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i] as ArrayEntry;
                if (entry == null)
                    continue;
                
                T val;
                if (!TryGetValue<T>(out val))
                    continue;

                arrayEntries.Add(entry);
            }
            return arrayEntries;
        }

        /// <summary>
        /// Loads an array of values from the list and marks them as loaded. 
        /// This will only return values that can be parsed to the specified type.
        /// </summary>
        public T[] LoadArray<T>()
        {
            T[] values = new T[_entries.Count];
            int addIndex = 0;
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i] as ArrayEntry;
                if (entry == null)
                    continue;

                T val;
                if (!entry.TryGetValue(out val))
                    continue;

                values[addIndex] = val;
                entry.Loaded = true;

                addIndex++;
            }

            if (addIndex < _entries.Count)
            {
                Array.Resize(ref values, addIndex);
            }
            return values;
        }

        /// <summary>
        /// Loads an array of values from the list. This will only return values
        /// that can be parsed to the specified type.
        /// </summary>
        public T[] GetArray<T>()
        {
            T[] values = new T[_entries.Count];
            int addIndex = 0;
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i] as ArrayEntry;
                if (entry == null)
                    continue;

                T val;
                if (!entry.TryGetValue(out val))
                    continue;

                values[addIndex] = val;

                addIndex++;
            }

            if (addIndex < _entries.Count)
            {
                Array.Resize(ref values, addIndex);
            }
            return values;
        }

        /// <summary>
        /// Gets an array entry by index and by type.
        /// </summary>
        /// <returns>The array entry.</returns>
        /// <param name="index">Index.</param>
        /// <typeparam name="T">The vaue type of the entry.</typeparam>
        public ArrayEntry GetArrayEntry<T>(int index)
        {
            int currentIndex = 0;
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (entry is ArrayEntry)
                {
                    try
                    {
                        if (currentIndex == index)
                        {
                            entry.GetValue<T>();
                            return entry as ArrayEntry;
                        }
                        currentIndex++;
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="key">Key.</param>
        public PropertyList GetList(string key)
        {
            PropertyList list = null;

            for (int i = 0; i < _keys.Count; i++)
            {
                var eachEntry = _entries[i];
                if (_keys[i].Equals(key) && eachEntry.GetType() == typeof(PropertyList))
                {
                    list = eachEntry as PropertyList;
                }
            }

            return list;
        }

        /// <summary>
        /// Gets/Creates a list.
        /// </summary>
        /// <param name="key">Key.</param>
        public PropertyList GetOrCreateList(string key)
        {
            return GetOrCreateList(key, null);
        }

        /// <summary>
        /// Gets/Creates a list.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="comment">Comment.</param>
        public PropertyList GetOrCreateList(string key, string comment)
        {
            return GetOrCreateList(key, comment, null);
        }

        /// <summary>
        /// Gets/Creates a list.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="comment">Comment.</param>
        /// <param name="closeComment">The comment on the closing curly brace.</param>
        public PropertyList GetOrCreateList(string key, string comment, string closeComment)
        {
            PropertyList list = GetList(key);

            if (list == null)
            {
                list = new PropertyList(key, comment, closeComment);
                AddEntry(list);
            }

            return list;
        }

        /// <summary>
        /// Gets/Creates a list and marks it as loaded.
        /// </summary>
        /// <param name="key">Key.</param>
        public PropertyList LoadList(string key)
        {
            return LoadList(key, null);
        }

        /// <summary>
        /// Gets/Creates a list and marks it as loaded.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="comment">Comment.</param>
        public PropertyList LoadList(string key, string comment)
        {
            return LoadList(key, comment, null);
        }

        /// <summary>
        /// Gets/Creates a list and marks it as loaded.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="comment">Comment.</param>
        /// <param name="closeComment">The comment on the closing curly brace.</param>
        public PropertyList LoadList(string key, string comment, string closeComment)
        {
            PropertyList list = GetOrCreateList(key, comment, closeComment);

            list.Loaded = true;

            return list;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Parent != null)
            {
                sb.AppendFormat("{0} {{", Key);
                if (!Comment.IsNullEmptyOrWhite())
                {
                    sb.AppendFormat("   # {0}", Comment);
                }
                sb.AppendLine();
            }
            var sortedEntries = _entries.OrderBy(e => e.Loaded).ToList();
            for (int i = 0; i < sortedEntries.Count; i++)
            {
                var entry = sortedEntries[i];
                sb.AppendLine(entry.ToString());
            }
            for (int i = 0; i < IndentLevel; i++)
            {
                sb.Append("    ");
            }
            if (Parent != null)
            {
                sb.Append("}");
                if (!string.IsNullOrEmpty(CloseComment))
                {
                    sb.AppendFormat("   # {0}", CloseComment);
                }
            }
            string toString = sb.ToString();
            for (int i = 0; i < IndentLevel; i++)
            {
                toString = "    " + toString;
            }
            return toString;
        }
    }
}