using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure;
using AngelXNA.Actors;
using Microsoft.Xna.Framework;
using AngelXNA.AI;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Messaging;
using Microsoft.Xna.Framework.Input;
using AngelXNA.Input;

namespace IntroGame.Screens
{
    public class MazeFinder : Actor
    {
        private List<Vector2> _pathPoints = new List<Vector2>();
        private int _pathIndex = 0;

        public MazeFinder()
        {
            Color = Color.Red;
            Size = new Vector2(0.75f, 0.75f);
            Switchboard.Instance["MazeFinderPathPointReached"] += ReceiveMessage;
            Switchboard.Instance["MouseDown"] += ReceiveMessage;
        }

	    public void GoTo(Vector2 newDestination)
        {
            List<Vector2> pathTest = new List<Vector2>();
            SpatialGraphManager.Instance.GetPath(Position, newDestination, ref pathTest);

            if (pathTest.Count > 0)
            {
                _pathPoints = pathTest;
                _pathIndex = 0;
                GetToNextPoint();
            }
        }

	    public void ReceiveMessage(Message message)
        {
            if (message.MessageName == "MazeFinderPathPointReached")
	        {
		        if (_pathIndex < _pathPoints.Count - 1)
		        {
			        GetToNextPoint();
		        }
	        }
	        else if (message.MessageName == "MouseDown")
	        {
                // TODO: Support typed messages
                //TypedMessage<Vec2i> *m = (TypedMessage<Vec2i>*)message;
                //Vec2i screenCoordinates = m->GetValue();
                //Vector2 worldCoordinates = MathUtil::ScreenToWorld(screenCoordinates);
                //GoTo(worldCoordinates);
	        }
        }

        private void GetToNextPoint()
        {
            Vector2 next = _pathPoints[++_pathIndex];
	        float distance = Vector2.Distance(Position, next);
	        //Want this guy to move at a constant rate of 8.0 units per second
	        float time = distance / 8.0f;
	        MoveTo(next, time, false, "MazeFinderPathPointReached");
        }
    };

    public class DemoScreenPathfinding : DemoScreen, IMouseListener
    {
        private MazeFinder _mf;

        public override void Start()
        {
            //Set up our obstacle course
	        ActorFactory.Instance.LoadLevel("maze");

	        //Create the bounding box that will limit the pathfinding search area
	        BoundingBox2D bounds = new BoundingBox2D(new Vector2(-20, -20), new Vector2(20, 20));
        	
	        //Create our pathfinding graph. In our 2D worlds, this is a relatively fast
	        // operation -- you shouldn't be doing it every frame, but recalculating every
	        // so often if your world has changed is not inappropriate. 
	        SpatialGraphManager.Instance.CreateGraph(
		        0.75f, //The size of the entity you want to pathfind (so the generator
		               //  can know how small a space can be and still have it fit.)
		        ref bounds //The search area
	        );
        	
	        //Create a MazeFinder (class definition below), and put him in the bottom
	        //  left corner of the maze
	        _mf = new MazeFinder();
	        _mf.Position = new Vector2(-11.5f, -8.0f);
            World.Instance.Add(_mf, 2);
        	
	        //Send him to the upper right, watch him scurry
            _mf.GoTo(Vector2.Zero);

            // Register this object as the mouse listener
            InputManager.Instance.RegisterMouseListener(this);
        	       	
	        //Demo housekeeping below this point. 
	        #region Demo housekeeping
	        String description = "This little dude is pathfinding through the area.";
	        description += "\n\nClick the mouse to give him a new target.";
	        description += "\n\nPress [B] to see the pathfinding graph.";
	        TextActor t = new TextActor("Console", description);
	        t.TextAlignment = TextActor.Alignment.Center;
	        t.Position = new Vector2(0.0f, -5.0f);
	        World.Instance.Add(t, 2);
	        TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenPathfinding.cs");
	        fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
	        fileLoc.Color = new Color(.3f, .3f, .3f);
	        World.Instance.Add(fileLoc, 2);
	        _objects.Add(fileLoc);
	        _objects.Add(t);
            _objects.Add(_mf);
	        
            Actor[] walls = TagCollection.Instance.GetObjectsTagged("maze_wall");
	        foreach(Actor a in walls)
		        _objects.Add(a);
	        #endregion
        }

        public override void Update(GameTime aTime)
        {
            base.Update(aTime);

            GamePadState state = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
            if (state.IsConnected && state.Buttons.B == ButtonState.Pressed
                || keyState.IsKeyDown(Keys.B))
            {
                SpatialGraphManager.Instance.DrawGraph = true;
            }
            else
            {
                SpatialGraphManager.Instance.DrawGraph = false;
            }
        }

        public override void Stop()
        {
            SpatialGraphManager.Instance.DrawGraph = false;
            _mf = null;
            base.Stop();
        }

        #region IMouseListener Members

        public void MouseDownEvent(Vector2 screenCoordinates, InputManager.MouseButton button)
        {
            if (_mf != null && button == InputManager.MouseButton.Left)
            {
                Vector2 worldCoords = World.Instance.Camera.ScreenToWorld((int)screenCoordinates.X, (int)screenCoordinates.Y);
                _mf.GoTo(worldCoords);
            }
        }

        public void MouseUpEvent(Vector2 screenCoordinates, InputManager.MouseButton button)
        {
            
        }

        public void MouseMotionEvent(int screenPosX, int screenPosY)
        {
            
        }

        #endregion
    }
}
