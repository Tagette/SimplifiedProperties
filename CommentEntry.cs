using System;

namespace SMPL.Props
{
    public class CommentEntry : PropertyEntry
    {
        public CommentEntry(string comment)
            : base("#", string.Empty, comment)
        {
        }

        public override string StringValue
        {
            get
            {
                return Comment;
            }
        }

        public override string ToString()
        {
            string toString = string.Format("# {0}", Comment);
            for (int i = 0; i < IndentLevel; i++)
            {
                toString = "    " + toString;
            }
            return toString;
        }
    }
}

