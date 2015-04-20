using System;

namespace SMPL.Props
{
    public class NewLineEntry : PropertyEntry
    {

        public NewLineEntry()
            : base("\n", string.Empty)
        {
        }

        public override string StringValue
        {
            get
            {
                return "\n";
            }
        }

        public override string ToString()
        {
            return "\n";
        }
    }
}

