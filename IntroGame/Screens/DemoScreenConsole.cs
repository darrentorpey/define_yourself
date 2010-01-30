using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using AngelXNA.Infrastructure.Console;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework.Graphics;

namespace IntroGame.Screens
{
    public class DemoScreenConsole : DemoScreen
    {
        private Actor a;

        private TextActor t1;
        private TextActor t2;
        private TextActor t3;

        public override void Start()
        {
            //Place the actor from a definition file
            a = ActorFactory.Instance.CreateActor("simple_actor");
            World.Instance.Add(a);

            // Definitions for the global namespace in the console are dirty, but
            // anything else should be nice and clean.
            DeveloperConsole.Instance.ItemManager.AddCommand("AddTexture", x => {
                a.SetSprite("Images\\angel");
                return null;
            });

            DeveloperConsole.Instance.ItemManager.AddCommand("ChangeSize", x => {
                DeveloperConsole.VerifyArgs(x, typeof(float));
                a.Size = new Vector2((float)x[0], (float)x[0]);
                return null;
            });

            t1 = new TextActor("Console", "This demo shows off the console.");
            t1.Position = new Vector2(0, -3.5f);
            t1.TextAlignment = TextActor.Alignment.Center;
            t2 = new TextActor("Console", "Press ~ to open it up. Execute \"AddTexture()\", enjoying the tab-completion.");
            t2.Position = new Vector2(0, -4.5f);
            t2.TextAlignment = TextActor.Alignment.Center;
            t3 = new TextActor("Console", "Then try executing \"ChangeSize(3.14)\" or whatever number suits your fancy.");
            t3.Position = new Vector2(0, -5.5f);
            t3.TextAlignment = TextActor.Alignment.Center;
            World.Instance.Add(t1);
            World.Instance.Add(t2);
            World.Instance.Add(t3);

            //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenConsole.cs, simple_actor.adf");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t1);
	        _objects.Add(t2);
	        _objects.Add(t3);
	        _objects.Add(a);
	        #endregion
        }

        public void Add_Texture()
        {
            a.SetSprite("Images\\angel");
        }

        [ConsoleMethod]
        public void Change_Size(float fsize)
        {
            a.Size = new Vector2(fsize, fsize);
        }
    }
}
