/*
 * Author: Tristan Chambers
 * Date: Thursday, November 7, 2013
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.Heroic-Intentions.net
 */
using System;

namespace SMPL.Props {
    /// <summary>
    /// Property entry.
    /// </summary>
    public class PropertyEntry {

        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string StringValue { get; internal set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public bool BoolValue { get { return GetValue<bool>(); } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public byte ByteValue { get { return GetValue<byte>(); } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public short ShortValue { get { return GetValue<short>(); } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int IntValue { get { return GetValue<int>(); } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public float FloatValue { get { return GetValue<float>(); } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double DoubleValue { get { return GetValue<double>(); } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public long LongValue { get { return GetValue<long>(); } }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; internal set; }

        private bool _loaded;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SimplifiedProperties.PropertyEntry"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public virtual bool Loaded { 
            get { 
                return _loaded && (Parent == null || Parent.Loaded);
            } 
            internal set { 
                _loaded = value;
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SimplifiedProperties.PropertyEntry"/> is modified.
        /// </summary>
        /// <value><c>true</c> if modified; otherwise, <c>false</c>.</value>
        public bool Modified { get; internal set; }

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        public PropertyList Root { 
            get { 
                PropertyList list = this is PropertyList ? (PropertyList) this : Parent;
                while(list.Parent != null) {
                    list = list.Parent;
                }
                return list;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public PropertyList Parent { get; set; }

        /// <summary>
        /// Gets the indent level.
        /// </summary>
        /// <value>The indent level.</value>
        public int IndentLevel {
            get {
                int _indentLevel = 0;
                PropertyList list = Parent;
                while(list != null) {
                    list = list.Parent;
                    _indentLevel++;
                }
                return _indentLevel - 1; // Subtract one since the first list isn't indented.
            }
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>The line number.</value>
        public int LineNumber { get; internal set; }

        #endregion // Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyEntry"/> class.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public PropertyEntry(string @key, string stringValue)
            : this(@key, stringValue, string.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyEntry"/> class.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="comment">Comment.</param>
        public PropertyEntry(string @key, string stringValue, string @comment) {
            Key = @key;
            StringValue = stringValue;
            Comment = @comment;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <typeparam name="T">The type of value to get.</typeparam>
        public T GetValue<T>() {
            T ret = default(T);
            try {
                ret = (T) Convert.ChangeType(StringValue, typeof(T));
            } catch(Exception) {
                throw new PropertyException("Tried to get a " + typeof(T).Name + " from '" + StringValue + "'", this);
            }
            return ret;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="newValue">New value.</param>
        public void SetValue(object @newValue) {
            StringValue = @newValue.ToString();
            Modified = true;
        }

        public override string ToString() {
            return string.IsNullOrEmpty(Comment) ?
                Key + " = '" + StringValue + "'" :
                Key + " = '" + StringValue + "'   # " + Comment;
        }
    }
}














