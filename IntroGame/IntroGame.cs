using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using AngelXNA;
using AngelXNA.Infrastructure;
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Actors;
using AngelXNA.Input;

namespace IntroGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class IntroGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public IntroGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            IsMouseVisible = true;
        }

        // This method demonstrates how the main log can be easily reconfigured
        private void demoLogging()
        {
            Log.Instance.Log("This has been recorded in the system  log, console log, and default file log");
            Log.Instance.LogToConsole = false;
            Log.Instance.Log("This has been recorded in the system log and the default file logs");
            Log.Instance.LogToSystem = false;
            Log.Instance.Log("This has been recorded in only the default file log");
            Log.Instance.LogToSystem = true;
            Log.Instance.LogToConsole = true;
            Log.Instance.LogToFile = false;
            Log.Instance.Log("This has been recorded in to the system log and the console log");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            World.Instance.Initialize(this);

            //we're going to be using the built-in physics
            World.Instance.SetupPhysics();

            //add the default grid so we can see where our Actors are in the world
            World.Instance.Add(new GridActor());

            // The GameManager is our DemoGameManager singleton
            // all the cool stuff happens there -- check it out. 
            World.Instance.GameManager = new DemoGameManager();

            // Add the AngelComponent once your setup is done, and Angel will
            // take care of updating and drawing everything for you.
            Components.Add(new AngelComponent(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {            
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            // That's it!  the AngelComponent takes care of the rest!

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }
    }
}