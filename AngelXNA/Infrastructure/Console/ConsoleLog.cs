using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure.Logging;

namespace AngelXNA.Infrastructure.Console
{
    public class ConsoleLog : IDeveloperLog
    {
        private List<string> _logLines = new List<string>();
        
        public int NumLogLines()
        {
            return _logLines.Count; 
        }

        public string GetLogLine(int lineIndex)
        {
            if (_logLines.Count == 0)
                return "";

            // TODO: Need an integer version of Clamp to avoid LHS
            lineIndex = (int)MathHelper.Clamp( lineIndex, 0, _logLines.Count - 1 );
            return _logLines[lineIndex];
        }

        #region IDeveloperLog Members

        public void Log(string val)
        {
            //Split by lines
            string[] splitByLines = val.Split('\n');
            for (int i = 0; i < splitByLines.Length; i++)
            {
                _logLines.Add(splitByLines[i]);
            }
        }

        #endregion
    }
}
