using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure;
using AngelXNA.Actors;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGame.Screens
{
    public class DemoScreenStart : DemoScreen
    {
        public override void Start()
        {
            TextActor t = new TextActor("Console", "Welcome to Angel. This is a quick demo of what we can do.");
            t.Position = new Vector2(0, 3.5f);
            t.TextAlignment = TextActor.Alignment.Center;
            
            TextActor t2 = new TextActor("Console", "(press [A] on the 360 controller or space bar to continue)");
            t2.Position = new Vector2(0, 2);
            t2.TextAlignment = TextActor.Alignment.Center;

            World.Instance.Add(t);
            World.Instance.Add(t2);

            //Demo housekeeping below this point. 
	        #region Demo housekeeping
            TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenStart.cs");
            fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
            fileLoc.Color = new Color(.3f, .3f, .3f);
            World.Instance.Add(fileLoc);
            _objects.Add(fileLoc);
            _objects.Add(t);
            _objects.Add(t2);
	        #endregion
        }
    }
}
