using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Infrastructure.Console
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleMethodAttribute : Attribute
    {
        public string Name = null;
        public ConsoleCommand.Flags Flags = ConsoleCommand.Flags.None;

        public ConsoleMethodAttribute()
        {

        }

        public ConsoleMethodAttribute(string asName)
        {
            Name = asName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ConsolePropertyAttribute : Attribute
    {
        public string Name = null;

        public ConsolePropertyAttribute()
        {

        }

        public ConsolePropertyAttribute(string asName)
        {
            Name = asName;
        }
    }
}
