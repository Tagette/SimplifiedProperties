/*
 * Author: Tristan Chambers
 * Date: Wednesday, April 1, 2015
 * Email: Tristan.Chambers@hotmail.com
 * Website: Tristan.PaperHatStudios.com
 */
using System;
using System.Text;

namespace SMPL.Props
{
    public class ConcatenatedEntry : PropertyEntry
    {
        /// <summary>
        /// Gets or sets the concat strings.
        /// </summary>
        /// <value>The concat strings.</value>
        public string[] ConcatStrings { get; set; }

        /// <summary>
        /// Gets or sets the concat comments.
        /// </summary>
        /// <value>The concat comments.</value>
        public string[] ConcatComments { get; set; }

        public ConcatenatedEntry(string key, string[] concatStrings, string[] concatComments)
            : base(key, string.Empty)
        {
            ConcatStrings = concatStrings;
            ConcatComments = concatComments;
        }

        /// <summary>
        /// Gets or sets the concatenated string value.
        /// </summary>
        /// <value>The string value.</value>
        public override string StringValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ConcatStrings.Length; i++)
                {
                    var concat = ConcatStrings[i];
                    sb.Append(concat);
                }
                return base.StringValue;
            }
        }

        /// <summary>
        /// Adds a string to the concat array.
        /// </summary>
        /// <param name="concat">The string to concatenate.</param>
        public void AddConcat(string concat)
        {
            AddConcat(concat, string.Empty);
        }

        /// <summary>
        /// Adds a string to the concat array.
        /// </summary>
        /// <param name="concat">The string to concatenate.</param>
        public void AddConcat(string concat, string comment)
        {
            var concatStrings = ConcatStrings;
            Array.Resize(ref concatStrings, ConcatStrings.Length + 1);
            ConcatStrings = concatStrings;

            var concatComments = ConcatComments;
            Array.Resize(ref concatComments, ConcatStrings.Length);
            ConcatComments = concatComments;

            ConcatStrings[ConcatStrings.Length - 1] = concat ?? string.Empty;
            ConcatComments[ConcatComments.Length - 1] = comment ?? string.Empty;
        }

        public override string ToString()
        {
            if (ConcatStrings == null || ConcatStrings.Length == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} = '{1}'", Key, ConcatStrings[0]);
            for (int i = 1; i < ConcatStrings.Length; i++)
            {
                var concat = ConcatStrings[i];
                sb.AppendLine();
                for (int j = 0; j < IndentLevel; j++)
                {
                    sb.Append("    ");
                }
                sb.AppendFormat("+ '{0}'", concat);
                if (!string.IsNullOrEmpty(ConcatComments[i]))
                {
                    sb.AppendFormat("   # {0}", ConcatComments[i]);
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

