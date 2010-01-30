using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Infrastructure.Logging
{
    public class SystemLog : IDeveloperLog
    {
        // Logs the given message to the System.Console
        public void Log(string message)
        {
            System.Console.WriteLine(message);
        }

        private SystemLog() {  }

        private static SystemLog s_Instance = new SystemLog();

        public static SystemLog Instance
        {
            get
            {
                return s_Instance;
            }
        }
    }
}
