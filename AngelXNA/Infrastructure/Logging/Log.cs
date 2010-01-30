namespace AngelXNA.Infrastructure.Logging
{
    public class Log : CompoundLog
    {
        private static Log s_Instance = null;
        private static FileLog s_FileLog = new FileLog();

        private static bool s_LogToSystem = true;
        private static bool s_LogToFile = true;
        private static bool s_LogToConsole = true;

        public bool LogToSystem
        {
            get { return s_LogToSystem; }
            set
            {
                s_LogToSystem = value;
                if (value == false)
                {
                    s_Instance.RemoveLog(SystemLog.Instance);
                }
                else
                {
                    s_Instance.AddLog(SystemLog.Instance);
                }
            }
        }

        public bool LogToFile {
            get { return s_LogToFile; }
            set {
                s_LogToFile = value;
                if (value == false)
                {
                    s_Instance.RemoveLog(s_FileLog);
                }
                else
                {
                    s_Instance.AddLog(s_FileLog);
                }
            }
        }

        public bool LogToConsole
        {
            get { return s_LogToConsole; }
            set
            {
                s_LogToConsole = value;
                if (value == false)
                {
                    s_Instance.RemoveLog(ConsoleLog.Instance);
                }
                else
                {
                    s_Instance.AddLog(ConsoleLog.Instance);
                }
            }
        }

        public static Log Instance
        {
            get {
                if (null == s_Instance)
                {
                    s_Instance = new Log();
                    s_Instance.AddLog(s_FileLog);
                    s_Instance.AddLog(SystemLog.Instance);
                    s_Instance.AddLog(ConsoleLog.Instance);
                }
                return s_Instance;
            }
        }
    }
}