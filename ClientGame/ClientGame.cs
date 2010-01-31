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
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Messaging;

using AngelXNA;
using AngelXNA.Actors;
using AngelXNA.Infrastructure;

using DefineYourself.Skills;

namespace DefineYourself
{
    public class MessageObject
    {
        public Actor building;
        public Actor player;

        public Actor getBuilding()
        {
            return building;
        }

        protected void setBuilding(object b)
        {
            building = (Actor)b;
            return;
        }

        public Actor getPlayer()
        {
            return player;
        }

        protected void setPlayer(object p)
        {
            player = (Actor)p;
            return;
        }
    
    };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ClientGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SkillWeb skillWeb;
        List<Actor> buildingList;
        TextActor text;

        public ClientGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            


            // Set the window size, as desired
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            // Set the mouse cursor to be visible
            IsMouseVisible = true;

            // TV: initialize a list of Actors representing the buildings the player can interact with
            buildingList = new List<Actor>();

            SoundState.LoadContent(Content);
            MusicState.LoadContent(Content);
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

            MusicState.Instance.Play();

            base.Initialize();
        }

        protected void InitializeSkillWeb()
        {
            // This is Darren's turf, to start

        }

        protected void InitializeMap()
        {
            // Tim, this is for you! (plz delete this line)

            // campus map panel
            Actor campus = new Actor();
            campus.Size = new Vector2(512.0f, 768.0f);
            campus.Position = new Vector2(-256.0f, 0.0f);
            campus.Color = new Color(1.0f, 0.0f, 0.0f);
            campus.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(campus);


            // tech web/tree panel
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
            //building1.Color = new Color(0.0f, 1.0f, 1.0f);
            //building1.Color = new Color(0.0f, 1.0f, 1.0f, 0.0f);
            building1.DrawShape = Actor.ActorDrawShape.Square;
            building1.SetSprite("Art/lib_bldg");
            building1.Name = "Library";
            buildingList.Add(building1);
            World.Instance.Add(building1);

            // building 2
            Actor building2 = new Actor();
            building2.Size = new Vector2(100.0f, 150.0f);
            building2.Position = new Vector2(-256.0f, 192.0f);
            building2.Color = new Color(1.0f, 1.0f, 0.0f);
            building2.DrawShape = Actor.ActorDrawShape.Square;
            building2.Name = "Stadium";
            buildingList.Add(building2);
            World.Instance.Add(building2);

            // Player 1
            Actor player1 = new Actor();
            player1.Size = new Vector2(30.0f, 30.0f);
            player1.Position = new Vector2(-256.0f, 0.0f);
            player1.Color = new Color(0.0f, 0.0f, 0.0f);
            player1.DrawShape = Actor.ActorDrawShape.Square;
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
            Actor player2 = new Actor();
            player2.Size = new Vector2(30.0f, 30.0f);
            player2.Position = new Vector2(-256.0f, -50.0f);
            player2.Color = new Color(1.0f, 1.0f, 1.0f);
            player2.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(player2);
            player2.Name = "Player 2";

            // Player 2 input
            DeveloperConsole.Instance.ItemManager.AddCommand("WPressed", new ConsoleCommandHandler(MoveUp2));
            DeveloperConsole.Instance.ItemManager.AddCommand("SPressed", new ConsoleCommandHandler(MoveDown2));
            DeveloperConsole.Instance.ItemManager.AddCommand("APressed", new ConsoleCommandHandler(MoveLeft2));
            DeveloperConsole.Instance.ItemManager.AddCommand("DPressed", new ConsoleCommandHandler(MoveRight2));

            Switchboard.Instance["WPressed"] += new MessageHandler(x => MoveUp2(null));
            Switchboard.Instance["SPressed"] += new MessageHandler(x => MoveDown2(null));
            Switchboard.Instance["APressed"] += new MessageHandler(x => MoveLeft2(null));
            Switchboard.Instance["DPressed"] += new MessageHandler(x => MoveRight2(null));

            // tech listener
            text = new TextActor();
            text.Position = new Vector2(256.0f, 0.0f);
            text.Name = "Tree";
            World.Instance.Add(text);
            Switchboard.Instance["Collision"] += new MessageHandler(x => TreeListener(x.Sender));

            SoundState.Instance.SoundOff();
            
        }

        // Handles the Up arrow - Player one moves up
        public object MoveUp(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.Y < 380.0f && p1.Position.Y >= -380.0f)
                p1.MoveTo(new Vector2(p1.Position.X, p1.Position.Y + 5.0f), 0.0f, false);
            
            return null;
        }

        // Handles the Down arrow - Player one moves down
        public object MoveDown(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.Y <= 380.0f && p1.Position.Y > -380.0f)
                p1.MoveTo(new Vector2(p1.Position.X, p1.Position.Y - 5.0f), 0.0f, false);

            return null;
        }

        // Handles the Left arrow - Player one moves left
        public object MoveLeft(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.X < -10.0f && p1.Position.X >= -510.0f)
                p1.MoveTo(new Vector2(p1.Position.X - 5.0f, p1.Position.Y), 0.0f, false);

            return null;
        }

        // Handles the Right arrow - Player one moves right
        public object MoveRight(object[] aParams)
        {
            Actor p1 = Actor.GetNamed("Player 1");
            if (p1.Position.X < -20.0f && p1.Position.X >= -512.0f)
                p1.MoveTo(new Vector2(p1.Position.X + 5.0f, p1.Position.Y), 0.0f, false);

            return null;
        }

        // *******************************************************************************************************************
        // Handles the Up arrow - Player 2 moves up
        public object MoveUp2(object[] aParams)
        {
            Actor p2 = Actor.GetNamed("Player 2");
            if (p2.Position.Y < 380.0f && p2.Position.Y >= -380.0f)
                p2.MoveTo(new Vector2(p2.Position.X, p2.Position.Y + 5.0f), 0.0f, false);

            return null;
        }

        // Handles the Down arrow - Player 2 moves down
        public object MoveDown2(object[] aParams)
        {
            Actor p2 = Actor.GetNamed("Player 2");
            if (p2.Position.Y <= 380.0f && p2.Position.Y > -380.0f)
                p2.MoveTo(new Vector2(p2.Position.X, p2.Position.Y - 5.0f), 0.0f, false);

            return null;
        }

        // Handles the Left arrow - Player 2 moves left
        public object MoveLeft2(object[] aParams)
        {
            Actor p2 = Actor.GetNamed("Player 2");
            if (p2.Position.X < -10.0f && p2.Position.X >= -510.0f)
                p2.MoveTo(new Vector2(p2.Position.X - 5.0f, p2.Position.Y), 0.0f, false);

            return null;
        }

        // Handles the Right arrow - Player 2 moves right
        public object MoveRight2(object[] aParams)
        {
            Actor p2 = Actor.GetNamed("Player 2");
            if (p2.Position.X < -20.0f && p2.Position.X >= -512.0f)
                p2.MoveTo(new Vector2(p2.Position.X + 5.0f, p2.Position.Y), 0.0f, false);

            return null;
        }

        // When a Collision message is broadcast, this updates the Tree TextActor with the name of the building that was collided with
        public object TreeListener(object aParams)
        {
            text.DisplayString = ((Actor)aParams).Name;
            return null;
        }

        // Checks every tick whether Player 1 intersects a building
        protected void PlayerAtBuilding()
        {
            Actor p1 = Actor.GetNamed("Player 1");

            //Actor b1 = Actor.GetNamed("Building 1");
            foreach (Actor b1 in buildingList) {
                // if the player is completely within the boundary of the building, it broadcasts a message
                if  ((p1.Bounds.Min.X > b1.Bounds.Min.X && p1.Bounds.Max.X < b1.Bounds.Max.X) &&
                    ((p1.Bounds.Min.Y > b1.Bounds.Min.Y && p1.Bounds.Max.Y < b1.Bounds.Max.Y)))
                {
                   // Log.Instance.Log("Collision with " + b1.Name);
                    //text.DisplayString = b1.Name;
                    Switchboard.Instance.Broadcast(new Message("Collision", b1));
                    //SoundState.Instance.SoundResume();
                    //SoundState.Instance.PlayPickupSound(new GameTime());
                    MusicState.Instance.ActiveSong = 1;
                    break;
                }
                else
                {
                    text.DisplayString = "Tree";
                   // SoundState.Instance.SoundOff();
                    MusicState.Instance.ActiveSong = 0;
                }
            }


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

            // TV: Checking each tick for the player to be colliding with a building
            PlayerAtBuilding();

            MusicState.Instance.Update(gameTime);
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
