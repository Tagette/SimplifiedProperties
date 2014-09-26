/*
 * Author: Tristan Chambers
 * Date: Thursday, November 7, 2013
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.Heroic-Intentions.net
 */
using System;
using System.IO;
using System.Text;

namespace SMPL.Props {
    /// <summary>
    /// A class that manages the file manipulation for the Simplified Properties config system.
    /// </summary>
    public class PropertyFile {

        #region Properties

        /// <summary>
        /// Gets the path to the config file.
        /// </summary>
        public string Path { 
            get {
                return _fileInfo.Directory.FullName;
            }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName {
            get {
                return _fileInfo.Name;
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public PropertyList Properties { get; protected set; }

        /// <summary>
        /// Gets or sets whether this <see cref="SimplifiedProperties.PropertyFile"/> loads all of the 
        /// properties for saving. This prevents entries from being commented when saving.
        /// </summary>
        /// <value>If <c>true</c> all entries wil be marked as loaded when they are read.</value>
        public bool LoadAll { get; set; }

        #endregion // Properties

        private FileInfo _fileInfo;
        internal int currentLineNumber = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyFile"/> class.
        /// If the file does not exist, it will create one.
        /// </summary>
        /// <param name="path">The path to the config file.</param>
        public PropertyFile(string @path) {
            // Get the file info.
            _fileInfo = new FileInfo(@path);
        }

        /// <summary>
        /// Load the config file and reads it's contents.
        /// </summary>
        public void Load() {
            if(!_fileInfo.Exists)
                _fileInfo.Create().Close();
            using(TextReader reader = _fileInfo.OpenText()) {
                var list = new PropertyList(string.Empty);
                list.Loaded = true;
                while(@reader.Peek() != -1) {
                    currentLineNumber++;
                    string line = @reader.ReadLine().Trim();

                    PropertyEntry entry = null;

                    if(line.StartsWith("#") || line.StartsWith("="))
                        entry = LoadComment(line, list);
                    else if(line.Equals(string.Empty))
                        entry = LoadNewLine(list);
                    else if(line.Contains("="))
                        entry = LoadEntry(line, list);
                    else if(line.Contains("{"))
                        entry = LoadList(line, list);
                    else if(line.Contains("}") && list.Parent != null)
                        list = list.Parent;
                    else
                        entry = LoadComment(line, list);

                    if(entry != null) {
                        entry.LineNumber = currentLineNumber;
                        entry.Loaded = LoadAll;
                        list.AddEntry(entry);
                        if(entry is PropertyList)
                            list = (PropertyList) entry;
                    }

                }
                reader.Close();
                if(list.Parent != null)
                    throw new PropertyException("Missing closing curly brace.", list);
                Properties = list.Root;
            }
        }

        /// <summary>
        /// Loads the list.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="reader">Reader.</param>
        /// <param name="parent">Parent.</param>
        protected PropertyList LoadList(string @line, PropertyList @parent) {
            var braceIndex = @line.IndexOf("{");
            var commentIndex = @line.IndexOf("#");
            if(commentIndex < 0)
                commentIndex = @line.Length;

            string key = @line.Substring(0, braceIndex - 1).Trim(' ', '\'');
            string comment = string.Empty;
            if(commentIndex + 1 < @line.Length)
                comment = @line.Substring(commentIndex + 1).Trim();

            return new PropertyList(key, comment) {
                Parent = @parent
            };
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        /// <returns>The entry.</returns>
        /// <param name="line">Line.</param>
        protected PropertyEntry LoadEntry(string @line, PropertyList @parent) {
            @line = @line.Replace(@"\#", "%$HASH$%");
            var equalsIndex = @line.IndexOf('=');
            var commentIndex = @line.IndexOf('#');
            if(commentIndex < 0)
                commentIndex = @line.Length;

            string key = @line.Substring(0, equalsIndex - 1).Trim(' ', '\'');
            string value = @line.Substring(equalsIndex + 1, commentIndex - equalsIndex - 1).Trim(' ', '\'')
                .Replace("%$HASH$%", "#");
            string comment = string.Empty;
            if(commentIndex + 1 < @line.Length)
                comment = @line.Substring(commentIndex + 1).Trim();

            return new PropertyEntry(Tools.GetAlias(key, @parent.Keys), value, comment) {
                Parent = @parent
            };
        }

        /// <summary>
        /// Loads the comment.
        /// </summary>
        /// <returns>The comment.</returns>
        /// <param name="line">Line.</param>
        protected PropertyEntry LoadComment(string @line, PropertyList @parent) {
            string key = "#";
            string value = @line.StartsWith("#") ? @line.Substring(1).Trim() : @line;
            string comment = string.Empty;

            return new PropertyEntry(key, value, comment) {
                Parent = @parent
            };
        }

        /// <summary>
        /// Loads the new line.
        /// </summary>
        /// <returns>The new line.</returns>
        protected PropertyEntry LoadNewLine(PropertyList @parent) {
            return new PropertyEntry(@"\n", string.Empty) {
                Parent = @parent
            };
        }

        /// <summary>
        /// Save the config file with it's new values.
        /// </summary>
        public void Save() {
            using(TextWriter writer = _fileInfo.CreateText()) {
                SaveList(writer, Properties);
                writer.Close();
            }
        }

        /// <summary>
        /// Saves the list.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="list">List.</param>
        protected void SaveList(TextWriter @writer, PropertyList @list) {
            if(!string.IsNullOrEmpty(@list.Key)) {
                if(!@list.Loaded)
                    writer.Write("# ");
                for(int i = 0; i < @list.IndentLevel && @list.Loaded; i++)
                    writer.Write("    ");
                writer.Write(@list.Key + " {");
                if(!string.IsNullOrEmpty(@list.Comment))
                    writer.Write("   # " + @list.Comment);
                writer.WriteLine();
            }
            foreach(var entry in @list.Entries) {
                if(entry is PropertyList) {
                    SaveList(@writer, (PropertyList) entry);
                } else if(!@list.Loaded) {
                    SaveComment(writer, entry);
                } else if(entry.Key.Equals(@"\n")) {
                    SaveNewLine(writer);
                } else if(entry.Key.Equals("#") || !entry.Loaded) {
                    SaveComment(writer, entry);
                } else {
                    SaveEntry(writer, entry);
                }
            }
            if(!string.IsNullOrEmpty(@list.Key)) {
                if(!@list.Loaded)
                    writer.Write("# ");
                for(int i = 0; i < @list.IndentLevel && @list.Loaded; i++)
                    writer.Write("    ");
                writer.WriteLine("}");
            }
        }

        /// <summary>
        /// Saves the entry.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="entry">Entry.</param>
        protected void SaveEntry(TextWriter @writer, PropertyEntry @entry) {
            for(int i = 0; i < @entry.IndentLevel && @entry.Loaded; i++)
                writer.Write("    ");
            var line = entry.Key + " = '" + entry.StringValue.Replace("#", @"\#") + "'";
            if(!entry.Comment.Equals(string.Empty))
                line += "   # " + entry.Comment;
            writer.WriteLine(line);
        }

        /// <summary>
        /// Saves the comment.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="entry">Entry.</param>
        protected void SaveComment(TextWriter @writer, PropertyEntry @entry) {
            for(int i = 0; i < @entry.IndentLevel && @entry.Loaded; i++)
                writer.Write("    ");
            var comment = @entry.Key.Equals("#") ? @entry.StringValue : @entry.ToString();
            var line = "# " + comment;
            writer.WriteLine(line);
        }

        /// <summary>
        /// Saves the new line.
        /// </summary>
        /// <param name="writer">Writer.</param>
        protected void SaveNewLine(TextWriter @writer) {
            @writer.WriteLine();
        }
    }
}

