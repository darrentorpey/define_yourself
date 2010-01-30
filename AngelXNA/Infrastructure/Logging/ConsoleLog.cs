using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure.Console;

namespace AngelXNA.Infrastructure.Logging
{
    public class ConsoleLog : IDeveloperLog
    {
        public void Log(string message)
        {
            DeveloperConsole.Instance.AddToConsoleLog(message);
        }

        private ConsoleLog() {  }

        private static ConsoleLog s_Instance = new ConsoleLog();

        public static ConsoleLog Instance
        {
            get
            {
                return s_Instance;
            }
        }
    }
}
