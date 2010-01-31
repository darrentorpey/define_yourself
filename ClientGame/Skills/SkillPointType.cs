using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefineYourself.Skills
{
    public class SkillPointType
    {
        public Types Type { get; set; }
        public int Value { get; set; }

        public enum Types
        {
            Red,
            Blue
        }

        public SkillPointType(Types type, int value)
        {
            Type = type;
            Value = value;
        }
    }
}
