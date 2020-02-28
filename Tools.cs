/*
 * Author: Tristan Chambers
 * Date: Thursday, November 8, 2013
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.PaperHatStudios.com
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace SMPL.Props
{
    public static class Tools
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <returns>The alias.</returns>
        /// <param name="key">Key.</param>
        /// <param name="keys">An array of keys.</param>
        public static string GetAlias(string @key, IEnumerable @keys)
        {
            var alias = @key;
            var count = 2;
            bool contains = false;
            do
            {
                contains = false;
                foreach (string eachKey in @keys)
                {
                    if (eachKey.Equals(alias))
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains)
                {
                    alias = @key + count;
                    count++;
                }
            } while(contains);
            return alias;
        }

        /// <summary>
        /// Count the occurances of a char in a string.
        /// </summary>
        /// <param name="searchFor">The character.</param>
        /// <param name="searchWhat">The string.</param>
        public static int Count(char searchFor, string searchWhat)
        {
            int count = 0;

            for (int i = 0; i < searchWhat.Length; i++)
                if (searchWhat[i] == searchFor)
                    count++;

            return count;
        }

        public static int IndexOfRecurring(this string @searchStr, char @compareChar, int @startIndex, int @count)
        {
            int index = -1;
            int num = 0;
            for (int i = startIndex; i < searchStr.Length; i++)
            {
                if (searchStr[i] == compareChar)
                {
                    num++;
                    if (num == count)
                    {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }

        public static bool IsNullEmptyOrWhite(this string value)
        {
            return value == null || value.Trim() == string.Empty;
        }

        public static void Log(string text)
        {
            string fileName = "output.txt";
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Close();
            }

            File.AppendAllText(fileName, text);
        }

        public static void Log(string format, params object[] objects)
        {
            Log(string.Format(format, objects));
        }

        public static void LogLine(string text)
        {
            Log(text + "\n");
        }

        public static void LogLine(string format, params object[] objects)
        {
            LogLine(string.Format(format, objects));
        }
    }
}

