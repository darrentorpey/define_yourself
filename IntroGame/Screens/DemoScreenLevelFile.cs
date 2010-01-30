using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure;
using AngelXNA.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGame.Screens
{
    public class DemoScreenLevelFile : DemoScreen
    {
        private TextActor t1;
        private TextActor t2;
        private TextActor t3;

        public override void Start()
        {
            //Loads the file from Config\Levels\level_demo.lvl
	        ActorFactory.Instance.LoadLevel("level_demo");

	        //Since the Actors were just added directly to the world,
	        //  we don't have handles to them. The level definition
	        //  gave them the tag "spawned," so we can get them that way.
	        Actor[] spawnedActors = TagCollection.Instance.GetObjectsTagged("spawned");
	        foreach(Actor a in spawnedActors)
	        {
		        //Can check Individual actors for tags as well.
		        if (a.IsTagged("left-tilted")) 
		        {
			        a.Rotation = 25.0f;
		        }
		        else if (a.IsTagged("right-tilted"))
		        {
			        a.Rotation = -25.0f;
		        }
		        
                //Applying tags
		        a.Tag("rotated");

		        //Removing tags
		        a.Untag("spawned");
	        }

	        //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        t1 = new TextActor("Console", "These Actors were placed and tagged (\"left-tilted\"");
	        t1.Position = new Vector2(0f, 5.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1);
	        t2 = new TextActor("Console", "and \"right-tilted\") using a Level file.");
	        t2.Position = new Vector2(0.0f, 4.5f);
	        t2.TextAlignment = TextActor.Alignment.Center;
            World.Instance.Add(t2);
	        t3 = new TextActor("Console", "Then their rotations were set based on those tags.");
	        t3.Position = new Vector2(0f, -4.5f);
	        t3.TextAlignment = TextActor.Alignment.Center;
            World.Instance.Add(t3);
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenLevelFile.cs, level_demo.lvl");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
            World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t1);
	        _objects.Add(t2);
	        _objects.Add(t3);

            foreach(Actor a in spawnedActors)
                _objects.Add(a);
	        #endregion
        }
    }
}
