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
    public class DemoScreenDefFile : DemoScreen
    {
        private Actor a;
        private TextActor t1;
        private TextActor t2;

        public void CustomFunc(Actor a)
        {
            a.Rotation = 45.0f;
            a.Position = new Vector2(0, -1);
        }

        public override void Start()
        {
            //CreateActor loads up an Actor Definition file and makes the actor from it
            a = ActorFactory.Instance.CreateActor(
                "simple_actor", //the file to load from -- must be located in Config\ActorTempates and end with ".adf"
                "PurpleActor",	//the desired name of the actor
                0,				//the render layer in which to put the actor (optional)
                CustomFunc		//a custom initialization function if you want one (optional, can be NULL)
            );
            World.Instance.Add(a);

            t1 = new TextActor("Console", "This Actor was placed using an ActorTemplate file.");
	        t1.Position = new Vector2(0, 4.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1);
	        t2 = new TextActor("Console", "You can be data-driven if you want to!");
	        t2.Position = new Vector2(0, 3.5f);
	        t2.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t2);

            //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenDefFile.cs, simple_actor.adf");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t1);
	        _objects.Add(t2);
	        _objects.Add(a);
	        #endregion
        }
    }
}
