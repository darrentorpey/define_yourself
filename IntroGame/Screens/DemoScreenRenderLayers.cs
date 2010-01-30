using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework.Input;

namespace IntroGame.Screens
{
    public class DemoScreenRenderLayers : DemoScreen
    {
        private Actor a1;
        private Actor a2;
        private TextActor t1;
        private TextActor t2;

        public override void Start()
        {
            a1 = new Actor();
            a1.Size = new Vector2(5.0f, 5.0f);
            a1.Color = Color.Blue;
            a1.Position = new Vector2(-1f, -1f);

            a2 = new Actor();
            a2.Size = new Vector2(5.0f, 5.0f);
            a2.Color = Color.Red;
            a2.Position = new Vector2(1f, 1f);

            World.Instance.Add(a1, 0); //Adding this actor to layer 0
            World.Instance.Add(a2, 1); //Adding this actor to layer 1
            
            //For your game, you will may want to use an
            //  enum for these values so you don't have to
            //  keep the integers straight. 

            t1 = new TextActor("Console", "These Actors overlap.");
	        t1.Position = new Vector2(0f, 5.5f);
            t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1);
	        t2 = new TextActor("Console", "Use the controller's bumper buttons to change their layer ordering.");
	        t2.Position = new Vector2(0f, 4.5f);
            t2.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t2);

            //Demo housekeeping below this point. 
	        #region Demo Housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenRenderLayers.cs");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t1);
	        _objects.Add(t2);
	        _objects.Add(a1);
	        _objects.Add(a2);
	        #endregion
        }

        public override void Update(GameTime aTime)
        {
            //NOTE: a2 has been added to layer one, so this function moves a1 around it.
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (padState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                World.Instance.UpdateLayer(a1, 0); //moves the actor to the requested layer
            }
            else if (padState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                World.Instance.UpdateLayer(a1, 2);
            }
        }
    }
}
