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
    public class DemoScreenCollisionLevelFile : DemoScreen
    {
        private TextActor t1;
        private TextActor t2;
        private TextActor t3;
        private TextActor t4;

        public override void Start()
        {
            //Loads the file from Config\Levels\collisionlevel_demo.lvl
            ActorFactory.Instance.LoadLevel("collisionlevel_demo");

	        //All the magic happens in the level file!
            
            //Demo housekeeping below this point. 
            t1 = new TextActor("Console", "These Actors were also placed using a level file.");
	        t1.Position = new Vector2(0.0f, 4.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1, 10);
	        t2 = new TextActor("Console", "Their physics-related properties came from actor definitions.");
	        t2.Position = new Vector2(0.0f, -3.5f);
	        t2.TextAlignment = TextActor.Alignment.Center;;
	        World.Instance.Add(t2, 10);
	        t3 = new TextActor("Console", "They respond to sound in a data driven way.");
	        t3.Position = new Vector2(0.0f, -4.5f);
	        t3.TextAlignment = TextActor.Alignment.Center;;
	        World.Instance.Add(t3, 10);
	        t4 = new TextActor("Console", "If the only collision response you need is sound, this is easier.");
	        t4.Position = new Vector2(0.0f, -5.5f);
	        t4.TextAlignment = TextActor.Alignment.Center; ;

            #region Demo Housekeeping
            World.Instance.Add(t4, 10);
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenCollisionLevelFile.cs, collisionlevel_demo.lvl,");
	        TextActor fileLoc2 = new TextActor("ConsoleSmall", "      ground_actor.adf, physics_event_actor.adf");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 735);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        fileLoc2.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc2.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc, 10);
            World.Instance.Add(fileLoc2, 10);
	        _objects.Add(fileLoc);
            _objects.Add(fileLoc2);
            _objects.Add(t1);
            _objects.Add(t2);
            _objects.Add(t3);
            _objects.Add(t4);
            
            Actor[] spawnedActors = TagCollection.Instance.GetObjectsTagged("spawned");
            foreach (Actor a in spawnedActors)
                _objects.Add(a);
            #endregion
        }

    }
}
