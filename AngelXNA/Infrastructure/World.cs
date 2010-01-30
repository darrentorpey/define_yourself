using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Physics;
using AngelXNA.AI;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Messaging;
using AngelXNA.Actors;

using Box2DX.Dynamics;
using Box2DX.Collision;
using System.IO;
using System.Threading;

#if WINDOWS
using AngelXNA.Editor;
using System.Windows.Forms;
#endif

namespace AngelXNA.Infrastructure
{
    //public delegate void CollisionHandler(Geom geom1, Geom geom2, float normalImpulse, Vector2 hitLocation);

    public class World
    {
        private struct RenderableLayerPair
        {
            public Renderable _renderable;
            public int _layer;

            public RenderableLayerPair(Renderable aRenderable, int aiLayer)
            {
                _renderable = aRenderable;
                _layer = aiLayer;
            }
        }

        private static World s_Instance = new World();

        private bool _simulateOn = true;
        private bool _initialized = false;
        private bool _elementsLocked = false;

        // Threaded physics
        private bool _physicsSetUp = false;
        private Box2DX.Dynamics.World _physicsWorld;
        private volatile bool _bThreadedPhysics = true;
        private Thread _physicsThread;
        private float _lastDt;
        private AutoResetEvent _runPhysics = new AutoResetEvent(false);
        private AutoResetEvent _waitPhysics = new AutoResetEvent(true);

        private Dictionary<int, LinkedList<Renderable>> _layers = new Dictionary<int, LinkedList<Renderable>>();
        private LinkedList<RenderableLayerPair> _deferredAdds = new LinkedList<RenderableLayerPair>();
        private List<RenderableLayerPair> _deferredLayerChanges = new List<RenderableLayerPair>();
        private List<Renderable> _deferredRemoves = new List<Renderable>();

        private Game _game;
        private GameManager _gameManager;
        private Camera _camera;
        private CollisionManager _collisionManager = new CollisionManager();

#if WINDOWS
        private EditorForm _editor;
#endif

        public static World Instance
        {
            get { return s_Instance; }
        }

        public Box2DX.Dynamics.World PhysicsWorld
        {
            get { return _physicsWorld; }
        }

        public Game Game
        {
            get { return _game; }
        }

        public GameManager GameManager
        {
            get { return _gameManager; }
            set
            {
                if (_gameManager != null || value == null)
                {
                    // TODO: "ERROR: Can only have one game manager!"
                    return;
                }

                _gameManager = value;
            }
        }

        public Camera Camera
        {
            get { return _camera; }
        }

        public bool IsSimulating
        {
            get { return _simulateOn; }
            set { _simulateOn = value; }
        }

        protected World()
        {
            
        }

        public bool Initialize(Game game)
        {
            return Initialize(game, 1024, 768, "AngelXNA Engine");
        }

        public bool Initialize(Game game, int windowWidth, int windowHeight, string windowName)
        {
            if (_initialized)
                return false;

            _game = game;

            _camera = new Camera(windowWidth, windowHeight, new Vector3(0, 0, 10), new Vector3(0, 0, -10));

            if (File.Exists(string.Format("Config\\{0}.cfg", "autoexec")))
            {
                DeveloperConsole.Instance.ExecConfigFile("autoexec");
            }

            ConsoleVariable var = DeveloperConsole.Instance.ItemManager.GetCVar("theWorld");
            var.Value = this;
            //CONSOLE_DECLARECMDSTATIC(UnloadAll, UnloadAllStatic);
            //CONSOLE_DECLARECMDSTATIC(ReloadLevel, ReloadLevel);

#if WINDOWS
            DeveloperConsole.Instance.ItemManager.AddCommand("BeginEditing", x => { BeginEditing(); return null; });
            DeveloperConsole.Instance.ItemManager.AddCommand("EndEditing", x => { EndEditing(); return null; });

            Switchboard.Instance["BeginEditing"] = new MessageHandler(x => BeginEditing());
#endif
            _initialized = true;
            return true;
        }

#if WINDOWS
        public void BeginEditing()
        {
            if (_editor == null)
            {
                _simulateOn = false;
                _editor = new EditorForm();
                _editor.Show(Form.FromChildHandle(_game.Window.Handle));
            }
        }

        public void EndEditing()
        {
            if (_editor != null)
            {
                // If EndEditing was called by the form closing, don't try to call it again.
                if(!_editor.Disposing)
                    _editor.Close();
                _editor = null;
            }

            // Regardless of the Form's status, start simulating again
            _simulateOn = true;
        }
#endif

        public void TearDown()
        {
            if (_bThreadedPhysics)
            {
                _bThreadedPhysics = false;
                _runPhysics.Set();
            }
        }

        public bool SetupPhysics()
        {
            return SetupPhysics(new Vector2(0, -10), new Vector2(100.0f, 100.0f), new Vector2(-100.0f, -100.0f));
        }

        public bool SetupPhysics(Vector2 gravity, Vector2 maxVertex, Vector2 minVertex)
        {
            if (_physicsSetUp)
                return false;

            AABB bounds = new AABB();
            bounds.LowerBound = minVertex;
            bounds.UpperBound = maxVertex;
            _physicsWorld = new Box2DX.Dynamics.World(bounds, gravity, true);

            if (_bThreadedPhysics)
            {
                _physicsThread = new Thread(new ThreadStart(
                    delegate()
                    {
#if XBOX
                        Thread.CurrentThread.SetProcessorAffinity(4);
#endif
                        while (_bThreadedPhysics)
                        {
                            _runPhysics.WaitOne();
                            if (_bThreadedPhysics)  // If this is now false, we're shutting down
                            {
                                RunPhysics(_lastDt);
                                _waitPhysics.Set();
                            }
                        }
                    }
                ));
                _physicsThread.Start();
            }

            // TODO: Physics debug draw, console variables, etc.
            //_physicsWorld->SetListener(this);

            //_physicsDebugDraw = new /*Physics*/DebugDraw();
            //_physicsWorld->SetDebugDraw(_physicsDebugDraw);

            //CONSOLE_DECLAREVAR(phys_gravity);
            //if (phys_gravity->HasVal())
            //{
            //    Vector2 grav = phys_gravity->GetVector2Val();
            //    _physicsWorld->m_gravity = b2Vec2(grav.X, grav.Y);
            //}

            _physicsWorld.SetContactListener(_collisionManager);
            
            _physicsSetUp = true;
            return true;
        }

        [ConsoleMethod]
        public void Add(Renderable newElement)
        {
            Add(newElement, 0);
        }

        public void Add(Renderable newElement, int layer)
        {
            // If we're not locked, add directly to _elements.
            if (!_elementsLocked)
            {
                newElement.Layer = layer;
                if (_layers.ContainsKey(layer))
                    _layers[layer].AddLast(newElement);
                else
                {
                    LinkedList<Renderable> list = new LinkedList<Renderable>();
                    list.AddLast(newElement);
                    _layers.Add(layer, list);
                }

                newElement.AddedToWorld();
            }
            // If we're locked, add to _deferredAdds and we'll add the new
            // Renderable after we're done updating all the _elements.
            else
            {
                RenderableLayerPair addMe = new RenderableLayerPair(newElement, layer);
                _deferredAdds.AddLast(addMe);
            }
        }

        [ConsoleMethod]
        public void Remove(Renderable oldElement)
        {
            Remove(oldElement, false);
        }

        private void Remove(Renderable oldElement, bool isLayerChange)
        {
            if (oldElement == null)
                return;

            if (_elementsLocked)
            {
                _deferredRemoves.Add(oldElement);
                return;
            }

            // First, make sure that it isn't deferred in the _deferredAdds list.
            for (LinkedListNode<RenderableLayerPair> node = _deferredAdds.First;
                node != null; node = node.Next)
            {
                if (node.Value._renderable == oldElement)
                {
                    _deferredAdds.Remove(node);

                    if(!isLayerChange)
                        node.Value._renderable.RemovedFromWorld();

                    return;
                }
            }

            // If we didn't find it in the deferred list, find/remove it from the layers.
            bool found = false;
            // Find the layer that matches the elements layer.
            LinkedList<Renderable> layer = _layers[oldElement.Layer];
            // Found the layer (list of renderables).
            // First, make sure that it isn't deferred in the _deferredAdds list.
            for (LinkedListNode<Renderable> node = layer.First;
                node != null; node = node.Next)
            {
                // Found it.
                if (node.Value == oldElement)
                {
                    // Remove the element.
                    layer.Remove(node);

                    if(!isLayerChange)
                        node.Value.RemovedFromWorld();

                    found = true;
                    // Nothing else to do.
                    break;
                }
            }

            if (!found)
            {
                //TODO: log or error handle
                Log.Instance.Log("[World] Remove(): Entity was not found: " + oldElement.ToString());
            }
        }

        public Renderable FindAt(int iscreenX, int iscreenY)
        {
            int[] layerOrder = new int[_layers.Keys.Count];
            _layers.Keys.CopyTo(layerOrder, 0);
            Array.Sort(layerOrder);

            for (int i = 0; i < layerOrder.Length; ++i)
            {
                LinkedList<Renderable> currentLayer = _layers[layerOrder[i]];
                foreach (Renderable renderable in currentLayer)
                {
                    // TODO: Shouldn't Renderable have position and size?
                    Actor actor = renderable as Actor;
                    if (actor != null)
                    {
                        Vector2 position = Camera.WorldToScreen(actor.Position); 
                        Vector2 size = Camera.WorldSizeToScreenSize(actor.Size);
                        Rectangle bounds = new Rectangle((int)(position.X - size.X / 2), 
                            (int)(position.Y - size.Y / 2), (int)size.X, (int)size.Y);
                        if(bounds.Contains(iscreenX, iscreenY))
                            return actor;
                    }
                }
            }

            return null;
        }

        public void UpdateLayer(Renderable element, int newLayer)
        {
            if (element.Layer == newLayer)
                return;

            RenderableLayerPair layerChange;
            layerChange._layer = newLayer;
            layerChange._renderable = element;
            _deferredLayerChanges.Add(layerChange);
        }

        public void Simulate(GameTime aGameTime)
        {
            if (_physicsSetUp && _simulateOn && _bThreadedPhysics)
            {
                _waitPhysics.WaitOne();
                SyncPhyiscs();
            }

            // system updates
            DeveloperConsole.Instance.Update( aGameTime );
            // theControllerManager.UpdateState();

            if (_gameManager != null)
                _gameManager.Update(aGameTime);

            if (_simulateOn)
            {
                // Deliver any messages that have been queued from the last frame. 
                Switchboard.Instance.SendAllMessages();

                //Clear out the collision contact points
                _collisionManager.Clear();

                //rb - Flag that the _elements array is locked so we don't try to add any
                // new actors during the update.
                _elementsLocked = true;
                    // this means phisics updates can't remove themselves from
                    // the world or spawn new physics objects. This is a huge assumption
                    if (_bThreadedPhysics)
                    {
                        _lastDt = (float)aGameTime.ElapsedGameTime.TotalSeconds;
                        _runPhysics.Set();
                    }

                    UpdateRenderables(aGameTime);
                    CleanupRenderables();
                _elementsLocked = false;

                // Now that we're done updating the list, allow any deferred Adds to be processed.
                ProcessDeferredAdds();
                ProcessDeferredLayerChanges();
                ProcessDeferredRemoves();

                if (_physicsSetUp && !_bThreadedPhysics)
                {
                    RunPhysics((float)aGameTime.ElapsedGameTime.TotalSeconds);
                    SyncPhyiscs();
                }

                Switchboard.Instance.Update(aGameTime);
                //if there are any system updates that still need to be run, put them here
            }
        }

        public void UpdateRenderables(GameTime aGameTime)
        {
            // This is very different from the original Angel implementation, but hopefully accomplishes
            // the same thing?
            int[] layerOrder = new int[_layers.Keys.Count];
            _layers.Keys.CopyTo(layerOrder, 0);
            Array.Sort(layerOrder);

            for(int i = 0; i < layerOrder.Length; ++i)
            {
                LinkedList<Renderable> currentLayer = _layers[layerOrder[i]];
                foreach (Renderable renderable in currentLayer)
                {
                    renderable.Update(aGameTime);
                }
            }
        }

        public delegate void ActorAction(Actor a);
        public void ForEachActor(ActorAction myDelegate)
        {
            int[] layerOrder = new int[_layers.Keys.Count];
            _layers.Keys.CopyTo(layerOrder, 0);
            Array.Sort(layerOrder);

            for (int i = 0; i < layerOrder.Length; ++i)
            {
                LinkedList<Renderable> currentLayer = _layers[layerOrder[i]];
                foreach (Renderable renderable in currentLayer)
                {
                    Actor theActor = renderable as Actor;
                    if (theActor != null)
                        myDelegate(theActor);
                }
            }
        }

        public void Render(GameTime aTime, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            // Setup the camera matrix.
            // theCamera.Render();

            DrawRenderables(aTime, aDevice, aBatch);

            if (_gameManager != null)
                _gameManager.Render(aTime, _camera, aDevice, aBatch);

            //Render debug information
            SpatialGraphManager.Instance.Render(_camera, aDevice, aBatch);

            //Draw developer console
            DeveloperConsole.Instance.Render(_camera, aDevice, aBatch);
        }

        public void DrawRenderables(GameTime aTime, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            // This is very different from the original Angel implementation, but hopefully accomplishes
            // the same thing?
            int[] layerOrder = new int[_layers.Keys.Count];
            _layers.Keys.CopyTo(layerOrder, 0);
            Array.Sort(layerOrder);

            for (int i = 0; i < layerOrder.Length; ++i)
            {
                LinkedList<Renderable> currentLayer = _layers[layerOrder[i]];
                foreach (Renderable renderable in currentLayer)
                {
                    renderable.Render(aTime, _camera, aDevice, aBatch);
                }
            }
        }

        public void CleanupRenderables()
        {
            int[] layerOrder = new int[_layers.Keys.Count];
            _layers.Keys.CopyTo(layerOrder, 0);
            Array.Sort(layerOrder);

            for (int i = 0; i < layerOrder.Length; ++i)
            {
                LinkedList<Renderable> currentLayer = _layers[layerOrder[i]];
                LinkedListNode<Renderable> currentNode = currentLayer.First;
                while (currentNode != null)
                {
                    if (currentNode.Value.Destroyed)
                    {
                        LinkedListNode<Renderable> removedNode = currentNode;
                        currentNode = currentNode.Next;
                        currentLayer.Remove(removedNode);
                        removedNode.Value.RemovedFromWorld();
                    }
                    else
                        currentNode = currentNode.Next;
                }
            }
        }

        public void RunPhysics(float lastDt)
        {
            if (!_physicsSetUp)
                return;

            // more iterations -> more stability, more cpu
            // tune to your liking...
            _physicsWorld.Step(lastDt, 8, 6);
            
        }

        private void SyncPhyiscs()
        {
            foreach (Body b in _physicsWorld.BodyList)
            {
                PhysicsActor physActor = b.GetUserData() as PhysicsActor;
                if (physActor != null)
                    physActor.SyncPosRot();
            }
        }

        protected void ProcessDeferredAdds()
        {
            foreach (RenderableLayerPair pair in _deferredAdds)
            {
                Add(pair._renderable, pair._layer);
            }
            _deferredAdds.Clear();
        }

        protected void ProcessDeferredLayerChanges()
        {
	        //TODO: use appropriate layer
            foreach(RenderableLayerPair pair in _deferredLayerChanges)
            {
	            Remove(pair._renderable, true);
		        Add( pair._renderable, pair._layer );
	        }
	        _deferredLayerChanges.Clear();
        }

        protected void ProcessDeferredRemoves()
        {
	        foreach(Renderable renderable in _deferredRemoves)
            {
	            Remove(renderable);
	        }
	        _deferredRemoves.Clear();
        }
    }
}
