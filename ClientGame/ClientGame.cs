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
//using AngelXNA.Infrastructure.Logging;

using DefineYourself.Skills;

namespace DefineYourself
{
    public class MessageObject
    {
        public Actor bldg { get; set; }
        public Actor play { get; set; }
        public bool boolie { get; set; }
        public bool justFinished { get; set; }
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
        TextActor statusTextP1, statusTextP2;
        Boolean p1Earning = false;
        Boolean p2Earning = false;
        TextActor clock;

        private float _fGameMinutesElapsed = 0.0f;
        private float _fTimeLastedLastTime = 0.0f;

        private static float TIME_YOU_HAVE_TO_WIN = 5.0f;

        public enum GameState
        {
            Running = 0,
            Start = 1,
            Paused = 2,
            Help = 3,
            Victory = 4,
            GameOver = 5
        }
        public GameState CurrentGameState { get; set; }

        public ClientGame()
        {
            CurrentGameState = GameState.Start;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            


            // Set the window size, as desired
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            // Set the mouse cursor to be visible
            IsMouseVisible = true;

            // TV: initialize a list of Actors representing the buildings the player can interact with
            buildingList = new List<Actor>();
            MapSpot.BuildingList = buildingList;

            SoundState.LoadContent(Content);
            MusicState.LoadContent(Content);
        }

        public TextActor StatusTextP1 { get { return statusTextP1; } }
        public TextActor StatusTextP2 { get { return statusTextP2; } }

        private Actor startSheet;
        private Actor pauseSheet;
        private Actor endSheet;
        private TextActor pTextP1;
        private TextActor startSheetText;
        private TextActor endSheetText;
        private TextActor endSheetScoreP1;
        private TextActor endSheetScoreP2;

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

            pTextP1 = new TextActor("Small", "PAUSED");
            World.Instance.Add(pTextP1, 3);

            startSheetText = new TextActor("Small", "");
            World.Instance.Add(startSheetText, 3);

            endSheetText = new TextActor("Small", "Define Yourself IS OVER");
            World.Instance.Add(endSheetText, 3);

            endSheetScoreP1 = new TextActor("Small", "SCORE 1");
            endSheetScoreP1.Position = new Vector2(-300, 200);
            World.Instance.Add(endSheetScoreP1, 3);

            endSheetScoreP2 = new TextActor("Small", "SCORE 2");
            endSheetScoreP2.Position = new Vector2(300, 200);
            World.Instance.Add(endSheetScoreP2, 3);

            pauseSheet = new Actor();
            pauseSheet.Size = new Vector2(1024.0f, 768.0f);
            pauseSheet.Position = new Vector2(0.0f, 0.0f);
            pauseSheet.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(pauseSheet, 2);

            startSheet = new Actor();
            startSheet.Size = new Vector2(1024.0f, 768.0f);
            startSheet.Position = new Vector2(0.0f, 0.0f);
            startSheet.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(startSheet, 2);

            endSheet = new Actor();
            endSheet.Size = new Vector2(1024.0f, 768.0f);
            endSheet.Position = new Vector2(0.0f, 0.0f);
            endSheet.DrawShape = Actor.ActorDrawShape.Square;
            World.Instance.Add(endSheet, 2);

            InitializeMap();
            
            InitializeSkillWeb();

            clock = new TextActor("Tiny", "Time Left: ");
            clock.Color = Color.Black;
            clock.Position = new Vector2(-120, 370);
            World.Instance.Add(clock);

            // Set the camera up somewhere "above" the grid
            World.Instance.Camera.Position = new Vector3(0.0f, 0.0f, 400.0f);

            // Add the AngelComponent once your setup is done, and Angel will
            // take care of updating and drawing everything for you.
            Components.Add(new AngelComponent(this));

            // TODO: Add your initialization logic here

            MusicState.Instance.Play();
            MusicState.Instance.Stop();

            base.Initialize();
        }

        Texture2D iconTextureOne;

        protected void InitializeSkillWeb()
        {
            SkillWeb.Instance.Game = this;

            DeveloperConsole.Instance.ItemManager.AddCommand("TestUpdateSkills", new ConsoleCommandHandler(TestUpdateSkills));
            DeveloperConsole.Instance.ItemManager.AddCommand("Quit", new ConsoleCommandHandler(QuitFromConsole));

            iconTextureOne = Content.Load<Texture2D>("map");
            iconTextureOne = Content.Load<Texture2D>("tech_tree");
            iconTextureOne = Content.Load<Texture2D>("titlescreen");
            iconTextureOne = Content.Load<Texture2D>("avatar1");
            iconTextureOne = Content.Load<Texture2D>("avatar2");
            iconTextureOne = Content.Load<Texture2D>("eng_icon");
            iconTextureOne = Content.Load<Texture2D>("lib_icon");
            iconTextureOne = Content.Load<Texture2D>("sci_icon");
            iconTextureOne = Content.Load<Texture2D>("soc_icon");
            iconTextureOne = Content.Load<Texture2D>("sports_icon");
            iconTextureOne = Content.Load<Texture2D>("stu_icon");


            //node_first.Location = new Vector2(10, 0);
            //node_first.Actor.Position = new Vector2(100, 100);
            //World.Instance.Add(node_first.Actor);

            ActorFactory.Instance.LoadLevel("first_few");
            //Actor[] spawnedActors = TagCollection.Instance.GetObjectsTagged("skill_node");
            //if (spawnedActors != null)
            //{
            //    foreach (Actor a in spawnedActors)
            //    {
            //        //a.Position = new Vector2(30, 30);
            //        World.Instance.Add(a);
            //    }
            //}

            //SkillNode node;
            
            //node = SkillNode.Create();
            //node.Location = new Vector2(50, 100);

            //node = SkillNode.Create();
            //node.Location = new Vector2(100, 100);
            //node.Color = new Color(1.0f, 1.0f, 0.0f);

            //node = SkillNode.Create();
            //node.Location = new Vector2(-100, 150);
            //node.Color = new Color(1.0f, 0.5f, 0.0f);

            foreach (SkillNode _node in SkillWeb.Instance.Nodes)
            {
                World.Instance.Add(_node.Actor);

                Actor p1_bar = new Actor();
                p1_bar.Position = new Vector2(_node.Actor.Position.X - 30, _node.Actor.Position.Y);
                p1_bar.Size = new Vector2(10, 40);
                p1_bar.Color = Color.White;
                p1_bar.DrawShape = Actor.ActorDrawShape.Square;
                World.Instance.Add(p1_bar);
                _node.P1_ProgBar = p1_bar;
                _node.P1_Points = 20;

                Actor p2_bar = new Actor();
                p2_bar.Position = new Vector2(_node.Actor.Position.X + 30, _node.Actor.Position.Y);
                p2_bar.Size = new Vector2(10, 40);
                p2_bar.Color = Color.Black;
                p2_bar.DrawShape = Actor.ActorDrawShape.Square;
                World.Instance.Add(p2_bar);
                _node.P2_ProgBar = p2_bar;
                _node.P2_Points = 10;

                SpriteFont helveticaTiny;
                SpriteFont helveticaSmall;
                helveticaTiny = Content.Load<SpriteFont>("fonts/HelveticaTiny");
                helveticaSmall = Content.Load<SpriteFont>("fonts/HelveticaSmall");

                FontCache.Instance.RegisterFont("fonts\\HelveticaTiny", "Tiny");
                FontCache.Instance.RegisterFont("fonts\\HelveticaSmall", "Small");

                //TextActor nameText = new TextActor("Tiny", _node.Name, TextActor.Alignment.Center);
                //nameText.Position = new Vector2(_node.Actor.Position.X - 20, _node.Actor.Position.Y - 30);
                //World.Instance.Add(nameText);
            }

            Switchboard.Instance["Collision"] += new MessageHandler(x => UpdateSkillProgress(x.Sender));
        }

        [ConsoleMethod]
        public object TestUpdateSkills(object[] aParams)
        {
            SkillNode node = skillWeb.Nodes.First();
            SkillNode node_two = skillWeb.Nodes[1];
            node.SkillPointsModifier.GetModDels()[SkillPointType.Types.Blue] = x => { return x + 4; };
            node_two.SkillPointsModifier.GetModDels()[SkillPointType.Types.Red] = x => { return x + 8; };
            SkillPointSet skillPointSet = new SkillPointSet(0, 0);
            Log.Instance.Log("===");
            skillPointSet.Report();
            Log.Instance.Log("---");
            node.UpdateValues(skillPointSet);
            skillPointSet.Report();
            Log.Instance.Log("---");
            node_two.UpdateValues(skillPointSet);
            skillPointSet.Report();
            Log.Instance.Log("===");
            Log.Instance.Log("");

            return null;
        }

        public object QuitFromConsole(object[] aParams)
        {
            this.Exit();
            return null;
        }

        protected void InitializeMap()
        {
            // Tim, this is for you! (plz delete this line)

            // campus map panel
            Actor campus = new Actor();
            campus.Size = new Vector2(512.0f, 768.0f);
            campus.Position = new Vector2(-256.0f, 0.0f);
            campus.DrawShape = Actor.ActorDrawShape.Square;
            campus.SetSprite("map");
            World.Instance.Add(campus);

            // tech web/tree panel
            Actor tech = new Actor();
            tech.Size = new Vector2(512.0f, 768.0f);
            tech.Position = new Vector2(256.0f, 0.0f);
            //tech.Color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
            tech.DrawShape = Actor.ActorDrawShape.Square;
            tech.SetSprite("tech_tree");
            World.Instance.Add(tech);

            // Player 1
            Actor player1 = new Actor();
            player1.Size = new Vector2(14.0f, 35.0f);
            player1.Position = new Vector2(-450.0f, 350.0f);
            player1.Color = new Color(1.0f, 1.0f, 1.0f);
            player1.DrawShape = Actor.ActorDrawShape.Square;
            player1.SetSprite("avatar1");
            World.Instance.Add(player1);
            player1.Name = "Player 1";

            // player 1 input
            DeveloperConsole.Instance.ItemManager.AddCommand("UpPressed", new ConsoleCommandHandler(MoveUp2));
            DeveloperConsole.Instance.ItemManager.AddCommand("DownPressed", new ConsoleCommandHandler(MoveDown2));
            DeveloperConsole.Instance.ItemManager.AddCommand("LeftPressed", new ConsoleCommandHandler(MoveLeft2));
            DeveloperConsole.Instance.ItemManager.AddCommand("RightPressed", new ConsoleCommandHandler(MoveRight2));

            Switchboard.Instance["UpPressed"] += new MessageHandler(x => MoveUp2(null));
            Switchboard.Instance["DownPressed"] += new MessageHandler(x => MoveDown2(null));
            Switchboard.Instance["LeftPressed"] += new MessageHandler(x => MoveLeft2(null));
            Switchboard.Instance["RightPressed"] += new MessageHandler(x => MoveRight2(null));

            // Player 2
            Actor player2 = new Actor();
            player2.Size = new Vector2(14.0f, 35.0f);
            player2.Position = new Vector2(-60.0f, -350.0f);
            player2.Color = new Color(0.0f, 0.0f, 0.0f);
            player2.DrawShape = Actor.ActorDrawShape.Square;
            player2.SetSprite("avatar2");
            World.Instance.Add(player2);
            player2.Name = "Player 2";

            // Player 2 input
            DeveloperConsole.Instance.ItemManager.AddCommand("WPressed", new ConsoleCommandHandler(MoveUp));
            DeveloperConsole.Instance.ItemManager.AddCommand("SPressed", new ConsoleCommandHandler(MoveDown));
            DeveloperConsole.Instance.ItemManager.AddCommand("APressed", new ConsoleCommandHandler(MoveLeft));
            DeveloperConsole.Instance.ItemManager.AddCommand("DPressed", new ConsoleCommandHandler(MoveRight));

            Switchboard.Instance["WPressed"] += new MessageHandler(x => MoveUp(null));
            Switchboard.Instance["SPressed"] += new MessageHandler(x => MoveDown(null));
            Switchboard.Instance["APressed"] += new MessageHandler(x => MoveLeft(null));
            Switchboard.Instance["DPressed"] += new MessageHandler(x => MoveRight(null));


            // Quick Keyboard Hack
            DeveloperConsole.Instance.ItemManager.AddCommand("Pause", new ConsoleCommandHandler(Pause));
            DeveloperConsole.Instance.ItemManager.AddCommand("UnPause", new ConsoleCommandHandler(UnPause));
            DeveloperConsole.Instance.ItemManager.AddCommand("Quit", new ConsoleCommandHandler(QuitFromConsole));
            DeveloperConsole.Instance.ItemManager.AddCommand("Start", new ConsoleCommandHandler(Start));
            DeveloperConsole.Instance.ItemManager.AddCommand("Victory", new ConsoleCommandHandler(Victory));
            DeveloperConsole.Instance.ItemManager.AddCommand("Lose", new ConsoleCommandHandler(Lose));
            Switchboard.Instance["Pause"] += new MessageHandler(x => Pause(null));
            Switchboard.Instance["UnPause"] += new MessageHandler(x => UnPause(null));
            Switchboard.Instance["Quit"] += new MessageHandler(x => QuitFromConsole(null));
            Switchboard.Instance["Start"] += new MessageHandler(x => Start(null));
            Switchboard.Instance["Victory"] += new MessageHandler(x => Victory(null));
            Switchboard.Instance["Lose"] += new MessageHandler(x => Lose(null));

            // P1 Status
            statusTextP1 = new TextActor("Small", "");
            statusTextP1.Position = new Vector2(-480.0f, -350.0f);
            statusTextP1.Name = "P1_Status";
            World.Instance.Add(statusTextP1);
            Switchboard.Instance["Collision"] += new MessageHandler(x => TreeListener(x.Sender));

            // P2 Status
            statusTextP2 = new TextActor("Small", "");
            statusTextP2.Position = new Vector2(-200.0f, -350.0f);
            statusTextP2.Name = "P2_Status";
            World.Instance.Add(statusTextP2);
            Switchboard.Instance["Collision"] += new MessageHandler(x => TreeListener(x.Sender));
            Switchboard.Instance["SkillUpdate"] += new MessageHandler(x => UpdateListener(x.Sender));

            SoundState.Instance.SoundOff();
   
            // Add (invisible) map squares
            ActorFactory.Instance.LoadLevel("map_spots");
        }

        // Handles the Up arrow - Player one moves up
        public object Start(object[] aParams)
        {
            CurrentGameState = GameState.Running;
            return null;
        }

        // Handles the Up arrow - Player one moves up
        public object Victory(object[] aParams)
        {
            CurrentGameState = GameState.Victory;
            return null;
        }

        // Handles the Up arrow - Player one moves up
        public object Lose(object[] aParams)
        {
            CurrentGameState = GameState.GameOver;
            return null;
        }

        // Handles the Up arrow - Player one moves up
        public object Pause(object[] aParams)
        {
            if (CurrentGameState == GameState.Paused)
            {
                //CurrentGameState = GameState.Running;
            }
            else
            {
                CurrentGameState = GameState.Paused;
            }
            return null;
        }

        // Handles the Up arrow - Player one moves up
        public object UnPause(object[] aParams)
        {
            if (CurrentGameState == GameState.Paused)
            {
                CurrentGameState = GameState.Running;
            }
            else
            {
                //CurrentGameState = GameState.Paused;
            }
            return null;
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

        // ***************************************************************************************************************************

        // When a Collision message is broadcast, this updates the Tree TextActor with the name of the building that was collided with
        public object TreeListener(object aParams)
        {
            //if (((MessageObject)aParams).play.Name == "Player 1")
            //{
            //    statusTextP1.DisplayString = ((MessageObject)aParams).bldg.Name;
            //}
            //else
            //{
            //    statusTextP2.DisplayString = ((MessageObject)aParams).bldg.Name;
            //}
            return null;
        }

        // When a Collision message is broadcast, this updates the Tree TextActor with the name of the building that was collided with
        public object UpdateSkillProgress(object aParams)
        {
            SkillWeb.Instance.UpdateSkillProgress((MessageObject)aParams);
            return null;
        }

        public object UpdateListener(object aParams)
        {
            if (((MessageObject)aParams).play.Name == "Player 1")
            {
                if (((MessageObject)aParams).boolie)
                {
                    //statusTextP1.DisplayString = "P1 Updating!";
                    p1Earning = true;
                }
                else
                {
                    //statusTextP1.DisplayString = "P1 not earning";
                    p1Earning = false;
                }
            }
            else
            {
                if (((MessageObject)aParams).boolie)
                {
                    //statusTextP2.DisplayString = "P2 Updating!";
                    p2Earning = true;
                }
                else
                {
                    //statusTextP2.DisplayString = "P2 not earning";
                    p2Earning = false;
                }
            }
            return null;
        }

        // Checks every tick whether Player 1 intersects a building
        protected void PlayerAtBuilding()
        {
            Actor p1 = Actor.GetNamed("Player 1");

            //Actor b1 = Actor.GetNamed("Building 1");
            foreach (Actor b1 in buildingList) {
                // if the player is completely within the boundary of the building, it broadcasts a message
                if  ((p1.Bounds.Max.X > b1.Bounds.Min.X && p1.Bounds.Min.X < b1.Bounds.Max.X) &&
                    ((p1.Bounds.Max.Y > b1.Bounds.Min.Y && p1.Bounds.Min.Y < b1.Bounds.Max.Y)))
                {
                   // Log.Instance.Log("Collision with " + b1.Name);
                    //text.DisplayString = b1.Name;
                    MessageObject mo = new MessageObject();//(b1, p1);
                    mo.bldg = b1;
                    mo.play = p1;
                    
                    Switchboard.Instance.Broadcast(new Message("Collision", mo));
                    //SoundState.Instance.SoundResume();
                    //SoundState.Instance.PlayPickupSound(new GameTime());
                   // MusicState.Instance.ActiveSong = 1;
                    break;
                }
                else
                {
                    //text.DisplayString = "Tree";
                   // SoundState.Instance.SoundOff();
                   // MusicState.Instance.ActiveSong = 0;
                }
            }

            Actor p2 = Actor.GetNamed("Player 2");
            //Actor b1 = Actor.GetNamed("Building 1");
            foreach (Actor b1 in buildingList)
            {
                // if the player is completely within the boundary of the building, it broadcasts a message
                if  ((p2.Bounds.Max.X > b1.Bounds.Min.X && p2.Bounds.Min.X < b1.Bounds.Max.X) &&
                    ((p2.Bounds.Max.Y > b1.Bounds.Min.Y && p2.Bounds.Min.Y < b1.Bounds.Max.Y)))
                {
                    // Log.Instance.Log("Collision with " + b1.Name);
                    //text.DisplayString = b1.Name;
                    MessageObject mo = new MessageObject();//(b1, p1);
                    mo.bldg = b1;
                    mo.play = p2;

                    Switchboard.Instance.Broadcast(new Message("Collision", mo));
                    //SoundState.Instance.SoundResume();
                    //SoundState.Instance.PlayPickupSound(new GameTime());
                   // MusicState.Instance.ActiveSong = 1;
                    break;
                }
                else
                {
                    //statusTextP2.DisplayString = "Tree";
                    // SoundState.Instance.SoundOff();
                   // MusicState.Instance.ActiveSong = 0;
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

            pTextP1.Color = new Color(0, 0, 0, 0);
            pauseSheet.Color = new Color(0, 0, 0, 0);

            startSheetText.Color = new Color(0, 0, 0, 0);

            endSheetScoreP1.Color = new Color(0, 0, 0, 0);
            endSheetScoreP2.Color = new Color(0, 0, 0, 0);
            endSheet.Color = new Color(0, 0, 0, 0);
            endSheetText.Color = new Color(0, 0, 0, 0);

            if (CurrentGameState == GameState.Paused)
            {
                pauseSheet.Color = Color.Black;
                pauseSheet.SetSprite("map");
                //pTextP1.DisplayString = "PAUSED";
                pTextP1.Color = Color.White;
            }
            else if (CurrentGameState == GameState.Start)
            {
                startSheet.SetSprite("titlescreen");
                startSheetText.Color = Color.White;
            }
            else if (CurrentGameState == GameState.Victory || CurrentGameState == GameState.GameOver)
            {
                endSheetScoreP1.Color = Color.White;
                endSheetScoreP1.DisplayString = "Player 1 gained " + SkillWeb.Instance.GetScoreP1() + " skills";
                endSheetScoreP2.Color = Color.White;
                endSheetScoreP2.DisplayString = "Player 2 gained " + SkillWeb.Instance.GetScoreP2() + " skills";
                endSheet.Color = Color.Black;
                endSheetText.Color = Color.White;
            }
            else
            {
            }

            if (CurrentGameState != GameState.Start)
            {
                startSheet.Color = new Color(0, 0, 0, 0);
            }

            statusTextP1.DisplayString = "";
            statusTextP1.Color = Color.White;
            statusTextP2.DisplayString = "";
            statusTextP2.Color = Color.Black;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (CurrentGameState != GameState.Paused)
            {
                _fGameMinutesElapsed += (float)gameTime.ElapsedGameTime.TotalMinutes;
            }

            float timeLeft = TIME_YOU_HAVE_TO_WIN - _fGameMinutesElapsed;

            if (timeLeft <= 0)
            {
                CurrentGameState = GameState.GameOver;
            }

            int totalMinutesLeft = (int)timeLeft;
            int totalSecondsLeft = (int)((timeLeft - totalMinutesLeft) * 60);

            string timeElapsed = String.Format("Time Left: {0}:{1:00}", totalMinutesLeft, totalSecondsLeft);
            clock.DisplayString = timeElapsed;

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

            //SkillWeb.Instance.Report();

            SkillWeb.Instance.Nodes.ForEach(skillNode => { skillNode.Draw(); });
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
