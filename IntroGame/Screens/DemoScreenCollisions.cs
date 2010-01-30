using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using AngelXNA.Infrastructure;
using AngelXNA.Physics;
using AngelXNA.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D = Box2DX.Dynamics;

namespace IntroGame.Screens
{
    public class DemoScreenCollisions : DemoScreen
    {
        private SoundEffect sound;
        private PhysicsActor p1;
        private PhysicsActor p2;
        private TextActor t1;

        public DemoScreenCollisions()
        {
            sound = World.Instance.Game.Content.Load<SoundEffect>(@"Sounds\sprong");
        }

        public override void Start()
        {
	        //Set up the PhysicsActors to collide
	        p1 = new PhysicsActor();
	        p1.Size = new Vector2(1.0f, 1.0f);
	        p1.Color = new Color(1.0f, 0.0f, 1.0f, 1.0f);
            p1.Density = 0.8f;
            p1.Friction = 0.5f;
            p1.Restitution = 0.7f;
            
            // Unlike in Angel 1.1 (which uses a global OnCollision handler) here, we
            // register with the pieces we want to know about collisions from
            p1.Collision += new CollisionHandler(OnCollision);

	        p2 = new PhysicsActor();
	        p2.Position = new Vector2(0.0f, -11.0f);
	        p2.Size = new Vector2(30.0f, 5.0f);
	        p2.Color = new Color(0.0f, 1.0f, 0.0f);
            p2.Density = 0.0f;
            p2.Friction = 0.1f;
            
	        World.Instance.Add(p1);
	        World.Instance.Add(p2);

	        //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        t1 = new TextActor("Console", "This example looks similar, but is responding to collisions with sound.");
	        t1.Position = new Vector2(0.0f, 3.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1);
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenCollisions.cs");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
	        _objects.Add(fileLoc);
	        _objects.Add(t1);
	        _objects.Add(p1);
	        _objects.Add(p2);
	        #endregion
        }

        private void OnCollision(PhysicsActor actorA, PhysicsActor actorB, ref Box2D.ContactPoint point)
        {
            if(Math.Abs(p1.Body.LinearVelocity.Y) > 5.0f)
                sound.Play();
        }
    }
}
