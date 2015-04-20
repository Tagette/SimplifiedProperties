/*
 * Author: Tristan Chambers
 * Date: Thursday, November 7, 2013
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.PaperHatStudios.com
 */
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SMPL.Props
{
    /// <summary>
    /// A class that manages the file manipulation for the Simplified Properties config system.
    /// </summary>
    public class PropertyFile
    {

        public const string REGEX_GROUP_START = @"^(?'r'\s*\/\*[ \n]*(?'c'[\s\S]+?)[ \n]*\*\/[ \n]*)";
        public const string REGEX_GROUP_END = @"^\s*}\s*";
        public const string REGEX_KEY_VALUE = @"^(?'r'\s*'?(?'k'.+?)'?[ \n]*=[ \n]*'?(?'v'.+?)'?[ \n]*(?:(?:#|\/\/) *(?'c'.*?)[ \n]*)?)(?:\n|'.+?'[ \n]*\{|\/\*|'.+'[ \n]*=|-[ \n]*'|\})";
        public const string REGEX_ARRAY_VALUE = @"^(?'r'\s*-[ \n]*'(?'v'.+?)'?[ \n]*(?:(?:#|\/\/) *(?'c'.*)[ \n]*)?)(?:\n|'.+?'[ \n]*\{|\/\*|'.+'[ \n]*=|-[ \n]*'|\})";
        public const string REGEX_COMMENT = @"^(?'r'\s*(?:#|\/\/) *(?'c'.*?))";
        public const string REGEX_BLOCK_COMMENT = @"^(?'r'\s*\/\*[ \n]*(?'c'[\s\S]+?)[ \n]*\*\/[ \n]*)";
        public const string REGEX_CONCAT = @"^(?'r'\s*\+[ \n]*'?(?'v'.+?)'?[ \n]*(?:(?:#|\/\/) *(?'c'.*?)[ \n]*)?)(?:\n|'.+?'[ \n]*\{|\/\*|'.+'[ \n]*=|-[ \n]*'|\})";
        public const string REGEX_NEW_LINE = @"(?<=\n)[ ]*(?'r'\n)";
        public const string REGEX_INVALID = @"^(?'r'.*)(?:\n|'.+?'[ \n]*\{|\/\*|'.+'[ \n]*=|-[ \n]*'|\})";

        #region Properties

        /// <summary>
        /// Gets the path to the config file.
        /// </summary>
        public string Path
        { 
            get
            {
                return _fileInfo.Directory.FullName;
            }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileInfo.Name;
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public PropertyList Properties { get; protected set; }

        /// <summary>
        /// If true, entries that have not been loaded yet will be commented.
        /// </summary>
        public bool CommentUnloadedEntries { get; set; }

        #endregion // Properties

        private FileInfo _fileInfo;
        internal int currentLineNumber = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplifiedProperties.PropertyFile"/> class.
        /// If the file does not exist, it will create one.
        /// </summary>
        /// <param name="path">The path to the config file.</param>
        public PropertyFile(string @path)
        {
            // Get the file info.
            _fileInfo = new FileInfo(@path);
        }

        /// <summary>
        /// Load the config file and reads it's contents.
        /// </summary>
        public void Load()
        {
            if (!_fileInfo.Exists)
                _fileInfo.Create().Close();
            using (TextReader reader = _fileInfo.OpenText())
            {

                var list = new PropertyList(string.Empty);
                list.Loaded = true;

                Regex groupStartRegex = new Regex(REGEX_GROUP_START);
                Regex keyValueRegex = new Regex(REGEX_KEY_VALUE);
                Regex arrayValueRegex = new Regex(REGEX_ARRAY_VALUE);
                Regex blockCommentRegex = new Regex(REGEX_COMMENT);
                Regex commentRegex = new Regex(REGEX_COMMENT);
                Regex concatRegex = new Regex(REGEX_CONCAT);
                Regex groupEndRegex = new Regex(REGEX_GROUP_END);
                Regex newLineRegex = new Regex(REGEX_NEW_LINE);
                Regex invalidRegex = new Regex(REGEX_INVALID);

                string allText = reader.ReadToEnd();
                PropertyEntry previousValueEntry = null;

                while (!allText.IsNullEmptyOrWhite())
                {
                    PropertyEntry newEntry = null;
                    string result = null;
                    var groupStartGroups = groupStartRegex.Match(allText).Groups;
                    if (groupStartGroups["r"].Success)
                    {
                        result = groupStartGroups["r"].Value;
                        string key = groupStartGroups["k"].Value;
                        string comment = groupStartGroups["c"].Value;
                        var newList = new PropertyList(key, comment);
                        newList.Parent = list;
                        list = newList;
                        previousValueEntry = null;
                    }
                    var keyValueGroups = keyValueRegex.Match(allText).Groups;
                    if (result == null && keyValueGroups["r"].Success)
                    {
                        result = keyValueGroups["r"].Value;
                        string key = keyValueGroups["k"].Value;
                        string value = keyValueGroups["v"].Value;
                        string comment = keyValueGroups["c"].Value;
                        newEntry = new PropertyEntry(key, value, comment);
                        previousValueEntry = newEntry;
                    }
                    var arrayValueGroups = arrayValueRegex.Match(allText).Groups;
                    if (result == null && arrayValueGroups["r"].Success)
                    {
                        result = arrayValueGroups["r"].Value;
                        string value = arrayValueGroups["v"].Value;
                        string comment = arrayValueGroups["c"].Value;
                        newEntry = new ArrayEntry(value, comment);
                        previousValueEntry = newEntry;
                    }
                    var concatGroups = concatRegex.Match(allText).Groups;
                    if (result == null && concatGroups["r"].Success)
                    {
                        if (previousValueEntry != null)
                        {
                            result = concatGroups["r"].Value;
                            string value = concatGroups["v"].Value;
                            string comment = concatGroups["c"].Value;
                            if (previousValueEntry is ConcatenatedEntry)
                            {
                                (previousValueEntry as ConcatenatedEntry).AddConcat(value, comment);
                            }
                            else
                            {
                                list.RemoveEntry(previousValueEntry);
                                newEntry = new ConcatenatedEntry(
                                    previousValueEntry.Key, 
                                    new string[]{ previousValueEntry.StringValue, value },
                                    new string[]{ previousValueEntry.Comment, comment });
                                newEntry.LineNumber = previousValueEntry.LineNumber;
                                previousValueEntry = newEntry;
                            }
                        }
                    }
                    var blockCommentGroups = blockCommentRegex.Match(allText).Groups;
                    if (result == null && blockCommentGroups["r"].Success)
                    {
                        result = blockCommentGroups["r"].Value;
                        string comment = blockCommentGroups["c"].Value;
                        newEntry = new BlockCommentEntry(comment);
                    }
                    var commentGroups = commentRegex.Match(allText).Groups;
                    if (result == null && commentGroups["r"].Success)
                    {
                        result = commentGroups["r"].Value;
                        string comment = commentGroups["c"].Value;
                        newEntry = new CommentEntry(comment);
                    }
                    var groupEndGroups = groupEndRegex.Match(allText).Groups;
                    if (result == null && groupEndGroups["r"].Success)
                    {
                        result = groupEndGroups["r"].Value;
                        string comment = groupEndGroups["c"].Value;
                        list.CloseComment = comment;
                        list = list.Parent;
                        previousValueEntry = null;
                    }
                    var newLineGroups = newLineRegex.Match(allText).Groups;
                    if (result == null && newLineGroups["r"].Success)
                    {
                        result = newLineGroups["r"].Value;
                        newEntry = new NewLineEntry();
                    }
                    var invalidGroups = invalidRegex.Match(allText).Groups;
                    if (result == null)
                    {
                        result = invalidGroups["r"].Value;
                    }
                    if (newEntry != null)
                    {
                        if (newEntry.LineNumber == 0)
                            newEntry.LineNumber = currentLineNumber;
                        newEntry.Loaded = !CommentUnloadedEntries;
                        list.AddEntry(newEntry);
                    }

                    currentLineNumber += Tools.Count('\n', result);
                    allText = allText.Remove(0, result.Length);
                }
                if (list.Parent != null)
                    throw new PropertyException("Missing closing curly brace.", list);
                Properties = list.Root;
            }
        }

        /// <summary>
        /// Save the config file with it's new values.
        /// </summary>
        public void Save()
        {
            if (Properties == null)
                return;
            
            if (_fileInfo.Exists)
                _fileInfo.Delete();
            
            using (StreamWriter writer = _fileInfo.CreateText())
            {
                writer.AutoFlush = true;
                writer.Write(Properties.ToString());
            }
        }
    }
}

