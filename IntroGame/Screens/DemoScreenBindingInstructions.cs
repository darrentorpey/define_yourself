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
    public class DemoScreenBindingInstructions : DemoScreen
    {
        public override void Start()
        {
            //Some TextActors pointing you to other files that do cool stuff. 
            TextActor t1 = new TextActor("Console", "(While we're looking at config files, check out autoexec.cfg.");
            t1.Position = new Vector2(0f, 3.5f);
            t1.TextAlignment = TextActor.Alignment.Center;
            TextActor t2 = new TextActor("Console", "It shows how to do controller and keyboard binding, and sets some console");
            t2.Position = new Vector2(0, 2);
            t2.TextAlignment = TextActor.Alignment.Center;
            TextActor t3 = new TextActor("Console", "variables that get used in code.)");
            t3.Position = new Vector2(0f, .5f);
            t3.TextAlignment = TextActor.Alignment.Center;

            World.Instance.Add(t1);
            World.Instance.Add(t2);
            World.Instance.Add(t3);

            //Demo housekeeping below this point. 
            #region Demo housekeeping
            TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenBindingInstructions.cs");
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
