/*
 * Author: Tristan Chambers
 * Date: Thursday, November 8, 2013
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.Heroic-Intentions.net
 */
using System;
using System.Collections.Generic;

namespace SMPL.Props {
    public static class Tools {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <returns>The alias.</returns>
        /// <param name="key">Key.</param>
        /// <param name="keys">An array of keys.</param>
        public static string GetAlias(string @key, string[] @keys) {
            var alias = @key;
            var count = 2;
            bool contains = false;
            do {
                contains = false;
                for(int i = 0; i < @keys.Length; i++) {
                    if(@keys[i].Equals(alias)) {
                        contains = true;
                        break;
                    }
                }
                if(contains) {
                    alias = @key + count;
                    count++;
                }
            } while(contains);
            return alias;
        }

        /// <summary>
        /// Count the occurances of a char in a string.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <param name="str">The string.</param>
        public static int Count(char c, string str) {
            int count = 0;

            for(int i = 0; i < str.Length; i++)
                if(str[i] == c)
                    count++;

            return count;
        }

        public static int IndexOfRecurring(this string @searchStr, char @compareChar, int @startIndex, int @count) {
            int index = -1;
            int num = 0;
            for(int i = startIndex; i < searchStr.Length; i++) {
                if(searchStr[i] == compareChar) {
                    num++;
                    if(num == count) {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }
    }
}

