using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AngelXNA.Infrastructure;
using AngelXNA.Input;

namespace IntroGame.Screens
{
    public class DemoScreenParticleActor : DemoScreen, IMouseListener
    {
        private bool _isActive = false;
        private ParticleActor pa;

        private TextActor t1;
        private TextActor t2;

        private List<Color> particleColors;
        
        public override void Start()
        {
            // Create the particle actor via the Actor Definition system (.adf files)
            pa = (ParticleActor)ActorFactory.Instance.CreateActor("particle_demo", null, 0, null);
            pa.Position = Vector2.Zero;
            World.Instance.Add(pa);
	        
	        _isActive = true; //lets the mouse events know that they should care

	        //Demo housekeeping below this point. 
	        t1 = new TextActor("Console", "Here's a ParticleActor. (Try moving and clicking the mouse!)");
	        t1.Position = new Vector2(0f, 3.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
            World.Instance.Add(t1);
	        t2 = new TextActor("Console", "Press [B] to change its properties.");
	        t2.Position = new Vector2(0f, 2.5f);
	        t2.TextAlignment = TextActor.Alignment.Center;
            World.Instance.Add(t2);

            // Set up a list of colors to be cycled through when the player clicks the mouse buttons
            particleColors = new List<Color>();
            particleColors.Add(Color.DarkBlue);
            particleColors.Add(Color.GreenYellow);
            particleColors.Add(Color.Green);
            particleColors.Add(Color.Aqua);
            particleColors.Add(Color.Coral);
            particleColors.Add(Color.Yellow);

            // Register this object as the mouse listener
            InputManager.Instance.RegisterMouseListener(this);

            #region Demo Housekeeping
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenParticleActors.cs");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t1);
	        _objects.Add(t2);
	        _objects.Add(pa);
	        #endregion
        }

        public override void Stop()
        {
            base.Stop();

            // Now that DemoScreen cleaned up...
            _isActive = false;
        }

        public override void Update(GameTime aTime)
        {
            //This is the same kind of input processing we did in DemoScreenSimpleActor.cs,
            //  but here we're playing with the particles. 
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.Buttons.B == ButtonState.Pressed)
            {
                pa.Color = new Color(1.0f, 0.0f, 0.0f, .25f);
                pa.Size = new Vector2(0.5f, 0.5f);
                pa.Gravity = new Vector2(0.0f, 0.0f);
                pa.ClearSpriteInfo();
                
                t1.DisplayString = "Now it's red and translucent. Press [Y].";
                t2.DisplayString = "";
            }
            if (state.Buttons.Y == ButtonState.Pressed)
            {
                pa.Color = new Color(0, 0, 1.0f, 1.0f);
                pa.Size = new Vector2(0.5f, 0.5f);
                pa.Gravity = new Vector2(0.0f, -8.0f);
                pa.ClearSpriteInfo();

                t1.DisplayString = "Now it's blue and has density. Press [X] for the coolest thing ever.";
                t2.DisplayString = "";
            }
            if (state.Buttons.X == ButtonState.Pressed)
            {
                pa.Size = new Vector2(4.0f, 4.0f);
                pa.Color = Color.White;
                pa.Gravity = new Vector2(0.0f, 0.0f);
                pa.LoadSpriteFrames(@"Images\numbers\angel_01");
                pa.PlaySpriteAnimation(0.5f, Actor.SpriteAnimationType.Loop, 0, 4);

                t1.DisplayString = "That's right, animated textures. You love it.";
                t2.DisplayString = "";
            }
        }

        public void MouseMotionEvent(int screenPosX, int screenPosY)
        {
            if (_isActive)
            {
                pa.Position = World.Instance.Camera.ScreenToWorld(screenPosX, screenPosY);
            }
        }

        public void MouseDownEvent(Vector2 screenCoordinates, InputManager.MouseButton button)
        {
            if (_isActive)
            {
                if (button == InputManager.MouseButton.Left)
                {
                    ParticleActor oneOff = (ParticleActor)ActorFactory.Instance.CreateActor("particle_demo", null, 0, null);
		            oneOff.Color = new Color(0.0f, 0.0f, 1.0f);
		            oneOff.SetSprite("Images/triangle");
		            //We can set the position to where the mouse click happened.
		            oneOff.Position = World.Instance.Camera.ScreenToWorld((int)screenCoordinates.X, (int)screenCoordinates.Y);

                    //The system will remove itself from the world and deallocate its memory
		            //  when the lifetime ends. (If it's 0.0, it's assumed to be infinite.)
		            oneOff.SystemLifetime = 1.0f;
            		
		            //Make sure to add it to the world!
		            World.Instance.Add(oneOff);
                }
                else if (button == InputManager.MouseButton.Right)
                {
                    int currColorIndex = particleColors.IndexOf(pa.Color) - 1;
                    if (currColorIndex < 0) { currColorIndex = particleColors.Count - 1; }
                    pa.Color = particleColors[currColorIndex];
                }
                else if (button == InputManager.MouseButton.Middle)
                {
                    pa.Color = particleColors.First();
                }
            }
        }

        public void MouseUpEvent(Vector2 screenCoordinates, InputManager.MouseButton button)
        {

        }
    }
}
