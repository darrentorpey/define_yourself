using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AngelXNA.Infrastructure.Logging
{
#if WINDOWS
    public class FileLog : IDeveloperLog
    {
        const string _defaultLoggingDir = "Logs";

        private string _filename; // The full name of this logger's file

        public FileLog() : this(MakeLogFileName("default")) 
        {
        }

        public FileLog(string filename)
        {
            FileUtils.MakeDirIfNeeded(Path.GetDirectoryName(filename));

            _filename = filename;
            Log("Opened on: " + DateTime.Now);
        }

        // Logs the given message to this logger's log file
        public void Log(string message)
        {
            List<string> strings = new List<string>();
            strings.Add(message);
            FileUtils.AppendLineToFile(_filename, strings);
        }

        // Gives the full filename for a log with the given name, using the default directory
        public static string MakeLogFileName(string logName)
        {
            return _defaultLoggingDir + "/" + logName + ".log";
        }
    }
#else
    public class FileLog : IDeveloperLog
    {
        public FileLog()
        {
        }

        public FileLog(string filename)
        {
     
        }

        // Logs the given message to this logger's log file
        public void Log(string message)
        {
        
        }

        // Gives the full filename for a log with the given name, using the default directory
        public static string MakeLogFileName(string logName)
        {
            return "";
        }

        // Initializes a log with the given name (in the default location) and returns its full filename
        public static string InitLog(string logName)
        {
            return "";
        }
    }
#endif
}
