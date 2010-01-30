using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Messaging;

using AngelXNA;
using AngelXNA.Actors;
using AngelXNA.Infrastructure;

using DefineYourself.Skills;

namespace DefineYourself
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ClientGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SkillWeb skillWeb;

        public ClientGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            // Set the window size, as desired
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            // Set the mouse cursor to be visible
            IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        ///  related content.  Calling base.Initialize will iterate through any components
        ///  and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            World.Instance.Initialize(this);

            // Add the default grid so we can see where our Actors are in the world
            World.Instance.Add(new GridActor());

            InitializeMap();

            InitializeSkillWeb();
            skillWeb = new SkillWeb();

            // Set the camera up somewhere "above" the grid
            World.Instance.Camera.Position = new Vector3(0.0f, 0.0f, 400.0f);

            // Add the AngelComponent once your setup is done, and Angel will
            // take care of updating and drawing everything for you.
            Components.Add(new AngelComponent(this));

            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected void InitializeSkillWeb()
        {
            // This is Darren's turf, to start

        }

        protected void InitializeMap()
        {
            // Tim, this is for you! (plz delete this line)

            // campus map
            Actor campus = new Actor();
            campus.Size = new Vector2(512.0f, 768.0f);
            campus.Position = new Vector2(-256.0f, 0.0f);
            campus.Color = new Color(1.0f, 0.0f, 0.0f);
            campus.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(campus);


            // tech web/tree
            Actor tech = new Actor();
            tech.Size = new Vector2(512.0f, 768.0f);
            tech.Position = new Vector2(256.0f, 0.0f);
            tech.Color = new Color(0.0f, 0.0f, 1.0f);
            tech.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(tech);

            // building 1
            Actor building1 = new Actor();
            building1.Size = new Vector2(100.0f, 100.0f);
            building1.Position = new Vector2(-256.0f, -192.0f);
            building1.Color = new Color(0.0f, 1.0f, 1.0f);
            building1.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(building1);

            // building 2
            Actor building2 = new Actor();
            building2.Size = new Vector2(100.0f, 100.0f);
            building2.Position = new Vector2(-256.0f, 192.0f);
            building2.Color = new Color(1.0f, 1.0f, 0.0f);
            building2.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(building2);

            // Player 1
            Actor player1 = new Actor();
            player1.Size = new Vector2(30.0f, 30.0f);
            player1.Position = new Vector2(-256.0f, 0.0f);
            player1.Color = new Color(0.0f, 0.0f, 0.0f);
            player1.DrawShape = Actor.ActorDrawShape.Circle;
            World.Instance.Add(player1);
            player1.Name = "Player 1";

            // player 1 input
            DeveloperConsole.Instance.ItemManager.AddCommand("UpPressed", new ConsoleCommandHandler(MoveUp));
            DeveloperConsole.Instance.ItemManager.AddCommand("DownPressed", new ConsoleCommandHandler(MoveDown));
            DeveloperConsole.Instance.ItemManager.AddCommand("LeftPressed", new ConsoleCommandHandler(MoveLeft));
            DeveloperConsole.Instance.ItemManager.AddCommand("RightPressed", new ConsoleCommandHandler(MoveRight));

            Switchboard.Instance["UpPressed"] += new MessageHandler(x => MoveUp(null));
            Switchboard.Instance["DownPressed"] += new MessageHandler(x => MoveDown(null));
            Switchboard.Instance["LeftPressed"] += new MessageHandler(x => MoveLeft(null));
            Switchboard.Instance["RightPressed"] += new MessageHandler(x => MoveRight(null));

            // Player 2

        }

        public object MoveUp(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.Y < 380.0f && p1.Position.Y >= -380.0f)
                p1.MoveTo(new Vector2(p1.Position.X, p1.Position.Y + 5.0f), 0.0f, false);
            
            return null;
        }

        public object MoveDown(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.Y <= 380.0f && p1.Position.Y > -380.0f)
                p1.MoveTo(new Vector2(p1.Position.X, p1.Position.Y - 5.0f), 0.0f, false);

            return null;
        }

        public object MoveLeft(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.X < -10.0f && p1.Position.X >= -510.0f)
                p1.MoveTo(new Vector2(p1.Position.X - 5.0f, p1.Position.Y), 0.0f, false);

            return null;
        }

        public object MoveRight(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.X < -20.0f && p1.Position.X >= -512.0f)
                p1.MoveTo(new Vector2(p1.Position.X + 5.0f, p1.Position.Y), 0.0f, false);

            return null;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        ///  all content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
