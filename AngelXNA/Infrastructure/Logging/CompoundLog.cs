using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Infrastructure.Logging
{
    public class CompoundLog : IDeveloperLog
    {
        private List<IDeveloperLog> _logs = new List<IDeveloperLog>();

        public CompoundLog() {  }

        // Logs the given message to all logs in this composite set
        public void Log(string message)
        {
            foreach (IDeveloperLog log in _logs)
            {
                log.Log(message);
            }
        }

        // Adds the given log to this composite set
        public void AddLog(IDeveloperLog newLog)
        {
            // Do nothing if the log is already a part of this composite set
            if (_logs.Contains(newLog))
                return;

            _logs.Add(newLog);
        }

        // Removes the specified log from this composite set
        public void RemoveLog(IDeveloperLog log)
        {
            // Do nothing if the log isn't in this composite set
            if (!_logs.Contains(log))
                return;

            _logs.Remove(log);
        }
    }
}
