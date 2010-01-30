using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework.Input;

namespace IntroGame.Screens
{
    public class DemoScreenMovingActor : DemoScreen
    {
        public Actor a;
        public TextActor t1;
        public TextActor t2;
        public TextActor t3;

        public override void Start()
        {
            a = new Actor();
            a.Size = new Vector2(4.0f, 4.0f);
            a.Color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
            World.Instance.Add(a);

            t1 = new TextActor("Console", "This Actor gets moved around by the left thumbstick. Try it.");
	        t1.Position = new Vector2(0, 3.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        t2 = new TextActor("Console", "Press [B] to rotate him.");
	        t2.Position = new Vector2(0, -4);
	        t2.TextAlignment = TextActor.Alignment.Center;
	        t3 = new TextActor("Console", "(The camera is a movable Actor, too -- right thumbstick.)");
	        t3.Position = new Vector2(0, -8);
	        t3.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1);
	        World.Instance.Add(t2);
	        World.Instance.Add(t3);

            //Demo housekeeping below this point. 
	        #region Demo Housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenMovingActor.cs");
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

        public override void Update(GameTime aTime)
        {
            //Update position based on thumbstick
            
            //NOTE: by default, the thumbstick has a dead zone around the middle where
            //  it will report position as 0. This prevents jitter when the stick isn't
            //  being touched.

            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            Vector2 position = new Vector2(
                3.0f * padState.ThumbSticks.Left.X,
                3.0f * padState.ThumbSticks.Left.Y
            );

            //Update the position with our calculated values. 
            a.Position = position;

            //Every tick, update the rotation if B is held down
            if (padState.Buttons.B == ButtonState.Pressed)
            {
                a.Rotation = a.Rotation + (90.0f * (float)aTime.ElapsedGameTime.TotalSeconds); //90 degrees per second
                if (a.Rotation > 360.0f)
                {
                    a.Rotation = a.Rotation - 360.0f;
                }
            }


            //Doing the same math we did above for the regular Actor, but this
            //  time applying the position changes to the Camera singleton. 
            Vector3 camPos = new Vector3(
                5.0f * padState.ThumbSticks.Right.X,
                5.0f * padState.ThumbSticks.Right.Y,
                World.Instance.Camera.Position.Z
            );

            World.Instance.Camera.Position = camPos;
        }
    }
}
