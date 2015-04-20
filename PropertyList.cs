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
        /// <param name="file">File.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public PropertyList(string key)
            : this(key, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyList"/> class.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="comment">Comment.</param>
        public PropertyList(string key, string comment)
            : base(key, string.Empty, comment)
        {
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
        public void NewEntry<T>(string key, T defaultValue, string comment)
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
        public void AddArrayEntry<T>(T value)
        {
            AddArrayEntry(value, null);
        }

        /// <summary>
        /// Adds a new array entry to the list.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comment">The comment.</param>
        public void AddArrayEntry<T>(T value, string comment)
        {
            AddEntry(new ArrayEntry(value + "", comment));
        }

        /// <summary>
        /// Adds a new line entry.
        /// </summary>
        public void NewLine()
        {
            AddEntry(new NewLineEntry(){ Loaded = true });
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

            for (int i = 0; i < _keys.Count; i++)
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
        public bool SetEntryAt(int index, PropertyEntry entry)
        {
            if (index < 0 || index > _entries.Count)
                return false;
            _keys[index] = entry.Key;
            _entries[index] = entry;
            Modified = true;
            return true;
        }

        /// <summary>
        /// Removes an entry from the list using it's key.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="key">Key.</param>
        public PropertyEntry RemoveEntry(string key)
        {
            return RemoveEntry(GetEntry(key));
        }

        /// <summary>
        /// Removes an entry from the list.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="entry">Entry.</param>
        public PropertyEntry RemoveEntry(PropertyEntry entry)
        {
            if (entry == null)
                return null;
            int index = IndexOf(entry);
            _keys.RemoveAt(index);
            _entries.RemoveAt(index);
            return entry;
        }

        /// <summary>
        /// Gets the value of an entry or creates the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public PropertyEntry GetOrCreateEntry(string key, object defaultValue)
        {
            return GetOrCreateEntry(key, defaultValue, string.Empty);
        }

        /// <summary>
        /// Gets the value of an entry or creates the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="comment">The comment for the entry.</param>
        public PropertyEntry GetOrCreateEntry(string key, object defaultValue, string comment)
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
        public PropertyEntry SetOrCreateEntry(string key, object newValue)
        {
            return SetOrCreateEntry(key, newValue, string.Empty);
        }

        /// <summary>
        /// Sets the value of an entry or creates the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comment">The comment for the entry.</param>
        public PropertyEntry SetOrCreateEntry(string key, object newValue, string comment)
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
        public T LoadEntry<T>(string key, T defaultValue)
        {
            return LoadEntry<T>(key, defaultValue, string.Empty);
        }

        /// <summary>
        /// Gets the value of an entry or creates the entry if it does not exist.
        /// This will mark the entry as loaded. See PropertyFile.CommentUnloadedEntries.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="comment">The comment for the entry.</param>
        public T LoadEntry<T>(string key, T defaultValue, string comment)
        {
            var entry = GetOrCreateEntry(key, defaultValue, comment);
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
        /// Loads an array of values from the list. This will only load values
        /// that can be parsed to the specified type.
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
            PropertyList list = GetList(key);

            if (list == null)
            {
                list = new PropertyList(key, comment);
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
            PropertyList list = GetOrCreateList(key, comment);

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
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
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