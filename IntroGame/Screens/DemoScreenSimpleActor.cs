using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure;
using AngelXNA.Actors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IntroGame.Screens
{
    public class DemoScreenSimpleActor : DemoScreen
    {
        private TextActor t;
        private Actor a;

        public DemoScreenSimpleActor()
        {

        }

        public override void Start()
        {
            //Adding an actor to the world is very simple
	        a = new Actor();
	        a.Size = new Vector2(5.0f, 5.0f);
	        a.Color = Color.Black;
            World.Instance.Add(a);

            t = new TextActor("Console", "Here's a simple Actor. (Press [B] to change it.)");
	        t.Position = new Vector2(0.0f, 3.5f);
	        t.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t);        

	        //Demo housekeeping below this point. 
	        #region Demo Housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenSimpleActor.cs");
            fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t);
	        _objects.Add(a);
	        #endregion
        }

        public override void Update(GameTime aTime)
        {
	        //Here we're doing some input processing and altering the Actor based on it.
            // We can use the XNA basic controller stuff no problem.
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.Buttons.B == ButtonState.Pressed)
            {
                a.Color = new Color(1.0f, 0.0f, 1.0f, .5f); 
                a.ClearSpriteInfo(); //removes any texture that might have been assigned
                t.DisplayString = "Now it's purple and translucent. Press [Y] to give it a texture.";
            }
            if (currentState.Buttons.Y == ButtonState.Pressed)
            {
                a.Color = Color.White; //(white and opaque so the texture comes through fully)
                a.ClearSpriteInfo();
                a.SetSprite("Images\\angel"); // Use XNA content specifiers over filenames.  Can load anything in your content proj
                t.DisplayString = "Pretty easy. You can do animations as well. Press [X] to check it out.";
            }
            if (currentState.Buttons.X == ButtonState.Pressed)
            {
                a.Color = Color.White;
                a.LoadSpriteFrames("Images\\numbers\\angel_01");
                a.PlaySpriteAnimation(
                    0.5f, 			//amount of time between frames
                    Actor.SpriteAnimationType.Loop,		//other options are PingPong and OneShot
                    0,				//starting frame
                    4,				//ending frame
                    "AngelNumbers"	//name of the animation so you can get the event when it finishes
                );
                t.DisplayString = "You can also change the speed and looping behavior if you want. ([A] to move on.)";
            }
        }
    }
}
