/*
 * Author: Tristan Chambers
 * Date: Wednesday, April 1, 2015
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.PaperHatStudios.com
 */
using System;

namespace SMPL.Props
{
    public class ArrayEntry : PropertyEntry
    {
        public ArrayEntry(string value)
            : this(value, string.Empty)
        {
        }

        public ArrayEntry(string value, string comment)
            : base("-", value, comment)
        {
        }

        public override string ToString()
        {
            string toString = string.IsNullOrEmpty(Comment)
                ? string.Format("- '{0}'", StringValue)
                : string.Format("- '{0}'   # {1}", StringValue, Comment);
            for (int i = 0; i < IndentLevel; i++)
            {
                toString += "    " + toString;
            }
            return toString;
        }
    }
}

