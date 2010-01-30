using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure;
using AngelXNA.Actors;
using AngelXNA.Messaging;
using Microsoft.Xna.Framework.Audio;

namespace IntroGame.Screens
{
    public class DemoScreenMessagePassing : DemoScreen
    {
        private SoundEffect sound;
        private PhysicsActor p1;
        private PhysicsActor p2;
        private PhysicsActor p3;

        private TextActor t1;

        public DemoScreenMessagePassing()
        {
            // Sets up the objects.
            Setup();

            //Subscribe the DemoScreen to some specific messages. We'll receive notification
            // when these messages get broadcast. Look below at the ReceiveMessage() function,
            // which is part of the MessageListener interface, to see how to handle the messages. 
            Switchboard.Instance["ScreenStarted"] += new MessageHandler(ReceiveMessage);

            sound = World.Instance.Game.Content.Load<SoundEffect>(@"Sounds\sprong");
        }

        public override void Start()
        {
            //Kick off one message -- it'll get physics going on this screen. 
	        Switchboard.Instance.Broadcast(new Message("ScreenStarted"));

	        //Add the ground actor so they'll have something to bounce off. 
	        p3 = new PhysicsActor();
	        p3.Position = new Vector2(0.0f, -11.0f);
	        p3.Size = new Vector2(30.0f, 5.0f);
	        p3.Color = new Color(0.0f, 1.0f, 0.0f);
            p3.Density = 0.0f;
            p3.Friction = 0.1f;

	        World.Instance.Add(p3);

	        //Demo housekeeping below this point. 
            #region Demo Housekeeping
	        String outputText = "These actors are responding to Messages\nthat we're sending through our central Switchboard.";
	        outputText += "\n\nYou can have actors respond to and broadcast arbitrary messages,\nwhich makes it easy to handle events in your game.";
	        outputText += "\n\n\n(Those actors have been hanging out up there this whole time,\nwaiting for the message that this screen had started\nbefore they dropped in.)";
	        t1 = new TextActor("Console", outputText);
	        t1.Position = new Vector2(0.0f, 3.5f);
	        t1.TextAlignment = TextActor.Alignment.Center;
	        World.Instance.Add(t1);
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenMessagePassing.cs");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc);
            _objects.Add(fileLoc);
            _objects.Add(t1);
            _objects.Add(p1);
            _objects.Add(p2);
            _objects.Add(p3);
            #endregion
        }

        public override void Stop()
        {
            base.Stop();
            Setup();
        }

        private void Setup()
        {
            //Create a new physics actor, but don't initialize its physics just yet. We'll start 
            // physics in response to some messages later on. 
            p1 = new PhysicsActor();
            p1.Size = new Vector2(1.0f, 1.0f);
            p1.Color = new Color(1.0f, 0.0f, 1.0f);
            p1.Position = new Vector2(-8.0f, 12.6f);
            p1.Rotation = 5.0f;
            p1.AutoInitPhysics = false;  // Don't initialize physics when the object is added to the world.

            //Make a friend for him, and don't initialize its physics when it gets added to the world.
            p2 = new PhysicsActor();
            p2.Size = new Vector2(1.0f, 1.0f);
            p2.Color = new Color(0.0f, 0.0f, 1.0f);
            p2.Position = new Vector2(8.0f, 12.6f);
            p2.AutoInitPhysics = false;

            //Add them all to the world. When we add them they are assigned names, which are
            // guaranteed to be unique. You can also assign your own name to any Actor with
            // SetName(), which returns the unique name it was actually given. (Numbers will 
            // be appended if the name was already taken.)
            // 
            //There's also a static function Actor::GetNamed() that you pass a string name
            // and will return either the Actor with that name or NULL. 
            World.Instance.Add(p1);
            World.Instance.Add(p2);

            //This message is interesting -- all collision messages take the form "CollisionWith"
            // plus the name of the actor colliding. Note that this means if you change the name
            // of the actor, you need to change your subscriptions if you want to hear about 
            // collisions. 
            Switchboard.Instance["CollisionWith" + p1.Name] += new MessageHandler(ReceiveMessage);
            Switchboard.Instance["CollisionWith" + p2.Name] += new MessageHandler(ReceiveMessage);
        }

        private void ReceiveMessage(Message message)
        {
            //Respond to the ScreenStarted message that we sent when the screen started. 
            if (message.MessageName == "ScreenStarted")
            {
                p1.InitPhysics(0.8f, 0.5f, 0.7f, PhysicsActor.ShapeType.Box);
            }

            //When the first actor collides, we kick off the physics for the second actor. 
            if (message.MessageName == "CollisionWith" + p1.Name)
            {
                // Only init the physics if it isn't already initialized.
                //   *weird* things happen if you initialize it a second time.
                if (p2.Body == null)
                {
                    p2.InitPhysics(0.8f, 0.5f, 0.7f, PhysicsActor.ShapeType.Box);
                }

                //If you need more data about the collision, collision messages always come *from*
                // the actor colliding with the one you care about, and carry a pointer to the 
                // relevant Box2D contact point, which contains more information like the normal force,
                // position, tangent, etc. 
                // 
                //You can get these from GetSender() and the GetValue() function, since the 
                // message getting delivered is really a templated TypedMessage<b2ContactPoint*>
                Vector2 vel = p1.Body.LinearVelocity;
                if (Math.Abs(vel.Y) > 5.0f)
                {
                    //We do the check on the actor's speed so that it only makes a sound when dropping
                    // at a certain rate. Otherwise, the bounce noise will play every time it "makes 
                    // contact" with the ground as it settles. This leads to the bad kind of cacophany. 
                    sound.Play();
                }
            }
            else if (message.MessageName == "CollisionWith" + p2.Name)
            {
                Vector2 vel = p2.Body.LinearVelocity;
                if (Math.Abs(vel.Y) > 5.0f)
                    sound.Play();
            }
        }
    }
}
