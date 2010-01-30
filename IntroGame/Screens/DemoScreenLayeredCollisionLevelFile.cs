using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGame.Screens
{
    public class DemoScreenLayeredCollisionLevelFile : DemoScreen
    {
        private TextActor t1;
        private TextActor t2;
        private TextActor t3;

        public override void Start()
        {
            //Loads the file from Config\Levels\layeredcollisionlevel_demo.lvl
	        ActorFactory.Instance.LoadLevel("layeredcollisionlevel_demo");

	        //All the magic happens in the level file!
            t1 = new TextActor("Console", "These new Actors were assigned layers in the level file.");
	        t1.Position = new Vector2(0.0f, 5.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1, 10);
	        t2 = new TextActor("Console", "Layer names are defined in autoexec.cfg (note variables starting with \"layer_\"),");
	        t2.Position = new Vector2(0.0f, 4.5f);
	        t2.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t2, 10);
	        t3 = new TextActor("Console", "and assigned to Actors by ActorFactorySetLayerName <layerName>.");
	        t3.Position = new Vector2(0.0f, 3.5f);
	        t3.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t3, 10);

            #region Demo housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenLayeredCollisionLevelFile.cs, layeredcollisionlevel_demo.lvl,");
	        TextActor fileLoc2 = new TextActor("ConsoleSmall", "      autoexec.cfg");
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
	        
            Actor[] spawnedActors = TagCollection.Instance.GetObjectsTagged("spawned");
            foreach (Actor a in spawnedActors)
                _objects.Add(a);
	        #endregion
        }
    }
}
