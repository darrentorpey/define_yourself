using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IntroGame.Screens
{
    public class DemoScreenByeBye : DemoScreen
    {
        public override void Start()
        {
            //"Goodnight, Gracie."
	        TextActor t1 = new TextActor("Console", "That's all we've got in the demo app.");
	        t1.Position = new Vector2(0, 3.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        TextActor t2 = new TextActor("Console", "Make sure to check out the documentation -- there are lots of other features.");
	        t2.Position = new Vector2(0, 2);
	        t2.TextAlignment = TextActor.Alignment.Center;
            TextActor t3 = new TextActor("Console", "http://bitbucket.org/fuzzybinary/angelxna/wiki/Home");
	        t3.Position = new Vector2(0, -1);
	        t3.TextAlignment = TextActor.Alignment.Center;

	        World.Instance.Add(t1);
	        World.Instance.Add(t2);
	        World.Instance.Add(t3);

	        //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenByeBye.cs");
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
