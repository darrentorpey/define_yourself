using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGame.Screens
{
    public class DemoScreenInstructions : DemoScreen
    {
        public override void Start()
        {
            //Just some text actors to give instructions. Nothing much to see here. 
	        TextActor t = new TextActor("Console", "This demo is designed to be super simple. Maybe too much so.");
	        t.Position = new Vector2(0.0f, 3.5f);
	        t.TextAlignment = TextActor.Alignment.Center;
	        TextActor t2 = new TextActor("Console", "Each example is self-contained within the file shown at the bottom left.");
	        t2.Position = new Vector2(0, 2);
	        t2.TextAlignment = TextActor.Alignment.Center;
	        TextActor t3 = new TextActor("Console", "The files are pretty thoroughly commented, so check them out to see how we do things.");
	        t3.Position = new Vector2(0, 0.5f);
	        t3.TextAlignment = TextActor.Alignment.Center;
	        TextActor t4 = new TextActor("Console", "Press [A] on the 360 Controller to go to the next example, and Back to go back.");
	        t4.Position = new Vector2(0, -3.5f);
	        t4.TextAlignment = TextActor.Alignment.Center;

	        World.Instance.Add(t);
	        World.Instance.Add(t2);
	        World.Instance.Add(t3);
	        World.Instance.Add(t4);

	        //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenInstructions.cs");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
            World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t);
	        _objects.Add(t2);
	        _objects.Add(t3);
	        _objects.Add(t4);
	        #endregion
        }
    }
}
