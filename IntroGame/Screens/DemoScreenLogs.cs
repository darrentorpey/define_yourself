using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Actors;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGame.Screens
{
    public class DemoScreenLogs : DemoScreen
    {
        public FileLog _fileLog = new FileLog( FileLog.MakeLogFileName("LogFile") );

        public override void Start()
        {
            // DeveloperConsole.Instance.ItemManager.AddFromObject(this);
            DeveloperConsole.Instance.ItemManager.AddCommand("PrintToFileLog", x => {
                _fileLog.Log(x[0].ToString());
                return null;
            });

            //Just some TextActors explaining what to do. 
            TextActor t1 = new TextActor("Console", "The console also supports a log.  Type \"ECHO <your message>\"");
            t1.Position = new Vector2(0.0f, 3.5f);
            t1.TextAlignment = TextActor.Alignment.Center;
            TextActor t2 = new TextActor("Console", "Type 'PrintToFileLog(\"<your message>\")' to print to Logs\\Logfile.log.");
            t2.Position = new Vector2(0f, 2f);
            t2.TextAlignment = TextActor.Alignment.Center;
            TextActor t3 = new TextActor("Console", "(This is particularly useful when trying to debug your config files.)");
            t3.Position = new Vector2(0f, -1f);
            t3.TextAlignment = TextActor.Alignment.Center;

            World.Instance.Add(t1);
            World.Instance.Add(t2);
            World.Instance.Add(t3);

            //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenLogs.cs, LogFile.log");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
            _objects.Add(t1);
            _objects.Add(t2);
            _objects.Add(t3);
	        #endregion
        }
    }
}
