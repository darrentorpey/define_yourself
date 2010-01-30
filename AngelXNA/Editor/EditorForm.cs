#if WINDOWS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AngelXNA.Infrastructure;
using AngelXNA.Input;
using Microsoft.Xna.Framework;
using AngelXNA.Actors;
using AngelXNA.Physics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AngelXNA.Editor
{
    public partial class EditorForm : Form, IMouseListener, IKeyListener
    {
        private Actor _selectedObject;
        private Vector2 _HeldOffset;

        private bool _bObjectHeld = false;
        private bool _bCameraHeld = false;
        private Vector2 _screenHeldPosition;
        private Vector3 _cameraHeldPosition;
        private bool _bPhysicsActorWasInitialized = false;

        Dictionary<string, ConstructorInfo> _baseActorMap = new Dictionary<string, ConstructorInfo>();

        public EditorForm()
        {
            InitializeComponent();

            this.Disposed += new EventHandler(EditorForm_Disposed);
            this.Shown += new EventHandler(EditorForm_Shown);

            _ddlSimulate.SelectedItem = World.Instance.IsSimulating ? "On" : "Off";
            _ddlSimulate.SelectedIndexChanged += new EventHandler(_ddlSimulate_SelectedIndexChanged);

            InputManager.Instance.RegisterKeyListerer(this);
            
            RebindActorDefinitions();
            RebindBaseActors();
        }

        void EditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _selectedObject != null)
            {
                World.Instance.Remove(_selectedObject);
                _selectedObject = null;
            }
        }

        void _ddlSimulate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ddlSimulate.SelectedItem == "On")
                World.Instance.IsSimulating = true;
            else
                World.Instance.IsSimulating = false;
        }

        public Actor Selected
        {
            get { return _selectedObject; }
            set
            {
                if (value != _selectedObject)
                {
                    OnObjectDeselect();
                    _selectedObject = value;
                    OnSelectedObjectChanged();
                }
            }
        }

        public void RebindActorDefinitions()
        {
            string dir = ActorFactory.Instance.ActorTemplateDirectory;
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            FileInfo[] files = dirInfo.GetFiles("*.adf", SearchOption.AllDirectories);

            string[] definitions = new string[files.Length];
            for (int i = 0; i < files.Length; ++i)
            {
                definitions[i] = Path.GetFileNameWithoutExtension(files[i].Name);
            }
            Array.Sort(definitions);

            _lbActorDefinitions.DataSource = definitions;
        }

        public void RebindBaseActors()
        {
            BindBaseActorAssembly(Assembly.GetExecutingAssembly());
            BindBaseActorAssembly(World.Instance.Game.GetType().Assembly);

            string[] baseActorNames = _baseActorMap.Keys.ToArray();
            Array.Sort(baseActorNames);
            _lbBaseActors.DataSource = baseActorNames;
        }

        private void BindBaseActorAssembly(Assembly assm)
        {
            foreach (Type type in assm.GetExportedTypes())
            {
                if (typeof(Actor).IsAssignableFrom(type))
                {
                    // See if it has a default constructor
                    ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);
                    if (info != null)
                        _baseActorMap.Add(type.Name, info);
                }
            }
        }

        private void OnObjectDeselect()
        {
            if (_bPhysicsActorWasInitialized)
            {
                PhysicsActor phys = _selectedObject as PhysicsActor;
                if (phys != null)
                    phys.InitPhysics(); // Reinitialize physics with current values

                _bPhysicsActorWasInitialized = false;
            }
        }

        

        private void OnSelectedObjectChanged()
        {
            // Special case for physics actors.  Remove them from the world before manipulating them
            PhysicsActor physActor = Selected as PhysicsActor;
            if (physActor != null && physActor.Body != null)
            {
                physActor.DestroyPhysics();
                _bPhysicsActorWasInitialized = true;
            }

            // This is to prevent a problem with closing the form.  If both
            // the form and the selected object are null, don't change either
            if(_pgObjProperties.SelectedObject != null || Selected != null)
                _pgObjProperties.SelectedObject = Selected;
        }

        private void EditorForm_Disposed(object sender, EventArgs e)
        {
            Selected = null;
            InputManager.Instance.UnregisterMouseListener(this);
            InputManager.Instance.UnregisterKeyListener(this);
            World.Instance.EndEditing();
        }

        void EditorForm_Shown(object sender, EventArgs e)
        {
            InputManager.Instance.RegisterMouseListener(this);
        }

        #region IMouseListener Members

        public void MouseDownEvent(Vector2 screenCoordinates, InputManager.MouseButton button)
        {
            Selected  = World.Instance.FindAt((int)screenCoordinates.X, (int)screenCoordinates.Y) as Actor;
            if (Selected != null)
            {
                Vector2 worldCoordinates = World.Instance.Camera.ScreenToWorld((int)screenCoordinates.X, (int)screenCoordinates.Y);
                _HeldOffset = Selected.Position - worldCoordinates;
                _bObjectHeld = true;
            }
            else
            {
                _bCameraHeld = true;
                _screenHeldPosition = screenCoordinates;
                _cameraHeldPosition = World.Instance.Camera.Position;
            }
        }

        public void MouseUpEvent(Vector2 screenCoordinates, InputManager.MouseButton button)
        {
            _bObjectHeld = false;
            _bCameraHeld = false;
        }

        public void MouseMotionEvent(int screenPosX, int screenPosY)
        {
            if (_bObjectHeld)
            {
                Vector2 worldCoordinates = World.Instance.Camera.ScreenToWorld(screenPosX, screenPosY);
                Selected.Position = worldCoordinates + _HeldOffset;
                _pgObjProperties.Refresh();
            }
            else if (_bCameraHeld)
            {
                // Because of DirectX handedness, X is correct, but Y is backwords.  Trust me, this is correct.
                Vector2 screenOffset = new Vector2(_screenHeldPosition.X - screenPosX, screenPosY - _screenHeldPosition.Y);
                Vector2 worldOffset = World.Instance.Camera.ScreenSizeToWorldSize(screenOffset);
                World.Instance.Camera.Position = _cameraHeldPosition + new Vector3(worldOffset, 0.0f);
            }
        }

        #endregion

        private void btnSaveActorDefinition_Click(object sender, EventArgs e)
        {
            InputDialog dialog = new InputDialog("Actor Definition Name", Selected.ActorDefinition);
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK && dialog.Value.Length > 0)
            {
                ActorFactory.Instance.SerializeActorDefinition(dialog.Value, Selected);
                RebindActorDefinitions();
            }
        }

        private void _btnAddActor_Click(object sender, EventArgs e)
        {
            Actor theActor;
            if (_tcActorDefinitions.SelectedTab == _tpBaseActors)
            {
                ConstructorInfo info = _baseActorMap[(string)_lbBaseActors.SelectedItem];
                theActor = (Actor)info.Invoke(null);
            }
            else
            {
                string definition = (string)_lbActorDefinitions.SelectedItem;
                theActor = ActorFactory.Instance.CreateActor(definition);
            }

            // Find the center of the screen
            Camera cam = World.Instance.Camera;
            Vector2 centerScreen = cam.ScreenToWorld(cam.WindowWidth / 2, cam.WindowHeight / 2);
            theActor.Position = centerScreen;

            World.Instance.Add(theActor);
            Selected = theActor;
        }

        #region IKeyListener Members

        public void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.Delete)
            {
                if (_selectedObject != null)
                {
                    World.Instance.Remove(_selectedObject);
                    Selected = null;
                }
            }
        }

        public void OnKeyUp(Microsoft.Xna.Framework.Input.Keys key)
        {
            
        }

        #endregion
    }
}

#endif