using System;

namespace SMPL.Props
{
    public class BlockCommentEntry : CommentEntry
    {
        public BlockCommentEntry(string comment)
            :base(comment)
        {
        }

        public override string ToString()
        {
            string toString = string.Format("/*{0}*/", Comment);
            for (int i = 0; i < IndentLevel; i++)
            {
                toString = "    " + toString;
            }
            return toString;
        }
    }
}

