using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Infrastructure.Console
{
    public class ConsoleType
    {
        public delegate string SerialzeHandler(object value);

        public Type RealType { get; set; }
        public string ConsoleName { get; set; }

        private SerialzeHandler _serializer;
        internal ConsoleCommandHandler _deserializer;

        public ConsoleType(Type realType, string consoleName, SerialzeHandler serializer, ConsoleCommandHandler deserializer)
        {
            RealType = realType;
            ConsoleName = consoleName;

            _serializer = serializer;
            _deserializer = deserializer;
        }

        public string Serialize(object value)
        {
            if (!RealType.IsInstanceOfType(value))
                throw new ArgumentException("Attempting to serialize " + value.GetType().ToString() + " with ConsoleType for " + RealType.ToString());

            StringBuilder sb = new StringBuilder(ConsoleName);
            sb.Append('(');
            sb.Append(_serializer(value));
            sb.Append(')');

            return sb.ToString();
        }
    }
}
