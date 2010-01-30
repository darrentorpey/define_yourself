using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure;
using AngelXNA.Actors;
using Microsoft.Xna.Framework.Input;

namespace IntroGame.Screens
{
    public class DemoScreenPhysicsActor : DemoScreen
    {
        private PhysicsActor p1;
        private PhysicsActor p2;

        private TextActor t1;

        public override void Start()
        {
            p1 = new PhysicsActor();
	        //PhysicsActors have all the same attributes as regular ones...
	        p1.Size = Vector2.One;
	        p1.Color = new Color(1.0f, 0.0f, 1.0f, 1.0f);

	        //...but with a little bit of magic pixie dust
            p1.Density = 0.8f; //density (0.0f will make it an immovable object)
            p1.Friction = 0.5f;
            p1.Restitution = 0.7f;
	        
	        p2 = new PhysicsActor();
	        p2.Position = new Vector2(0.0f, -11.0f);
	        p2.Size = new Vector2(30.0f, 5.0f);
	        p2.Color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            p2.Density = 0.0f;
            p2.Friction = 0.1f;

	        //NOTE: After you call InitPhysics (or the world calls it automatically), you 
            // can't directly set an Actor's position, or rotation -- you've turned those 
            // over to the physics engine. You can't change the size, either, since that 
            // would mess up the simulation.

	        World.Instance.Add(p1);
	        World.Instance.Add(p2);

            //Demo housekeeping below this point. 
	        #region Demo Housekeeping
	        t1 = new TextActor("Console", "These Actors use physics. Press [B].");
	        t1.Position = new Vector2(0.0f, 3.5f);
            t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1);
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenPhysicsActor.cs");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t1);
	        _objects.Add(p1);
	        _objects.Add(p2);
	        #endregion
        }

        public override void Update(GameTime aTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.Buttons.B == ButtonState.Pressed)
            {
                //punch it upwards
                p1.ApplyForce(new Vector2(0, 20.0f), Vector2.Zero);
            }
        }
    }
}
