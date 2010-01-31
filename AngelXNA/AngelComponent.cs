using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Infrastructure;
using AngelXNA.Input;
using Microsoft.Xna.Framework.Graphics;

namespace AngelXNA
{
    public class AngelComponent : DrawableGameComponent
    {
        public SpriteBatch _spriteBatch;
        public KeyboardState _previousKeyboardState;
        public MouseState _previousMouseState;

        private GamePadState[] _previousGamePadStates = new GamePadState[4];
        private GamePadState[] _gamePadStates = new GamePadState[4];

        public AngelComponent(Game game)
            : base(game)
        {

        }

        private PlayerIndex getPlayerIndex(int index) {
            return (PlayerIndex)index;
        }

        public override void Initialize()
        {
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();
            
            for (int i = 0; i < 4; i++)
            {
                _previousGamePadStates[i] = GamePad.GetState(getPlayerIndex(i));
            }

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            World.Instance.TearDown();

            base.Dispose(disposing);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            #region Keyboard Input
            KeyboardState currentKeyboardState = Keyboard.GetState();
            foreach (Keys key in currentKeyboardState.GetPressedKeys())
            {
               // if (!_previousKeyboardState.IsKeyDown(key))
                    KeyDown(key);
            }

            foreach (Keys key in _previousKeyboardState.GetPressedKeys())
            {
                if (!currentKeyboardState.IsKeyDown(key))
                    KeyUp(key);
            }

            _previousKeyboardState = currentKeyboardState;
            #endregion

            #region Mouse Input
            MouseState currentMouseState = Mouse.GetState();
            Vector2 mouseLoc = new Vector2(currentMouseState.X, currentMouseState.Y);
            if (_previousMouseState == null)
                _previousMouseState = currentMouseState;

            // Only report mouse down if the mouse is inide the client bounds
            if (currentMouseState.X > 0 && currentMouseState.Y > 0 &&
                currentMouseState.X < Game.Window.ClientBounds.Width &&
                currentMouseState.Y < Game.Window.ClientBounds.Height)
            {
                if (currentMouseState.LeftButton != _previousMouseState.LeftButton)
                {
                    InputManager.Instance.MouseButtonAction(mouseLoc, InputManager.MouseButton.Left, currentMouseState.LeftButton);
                }

                if (currentMouseState.MiddleButton != _previousMouseState.MiddleButton)
                {
                    InputManager.Instance.MouseButtonAction(mouseLoc, InputManager.MouseButton.Middle, currentMouseState.MiddleButton);
                }

                if (currentMouseState.RightButton != _previousMouseState.RightButton)
                {
                    InputManager.Instance.MouseButtonAction(mouseLoc, InputManager.MouseButton.Right, currentMouseState.RightButton);
                }

                if (mouseLoc.X != _previousMouseState.X || mouseLoc.Y != _previousMouseState.Y)
                {
                    InputManager.Instance.MouseMotionAction(currentMouseState.X, currentMouseState.Y);
                }
            }

            _previousMouseState = currentMouseState;
            #endregion

            #region XBox 360 GamePad Input
            for (int i = 0; i < 4; i++)
            {
                _gamePadStates[i] = GamePad.GetState(getPlayerIndex(i));
                GamePadButtons buttons = _gamePadStates[i].Buttons;
                GamePadButtons prevButtons = _previousGamePadStates[i].Buttons;

                #region Check each button in turn for changes
                if (buttons.A != prevButtons.A)
                {
                    registerButtonPress(buttons.A, InputManager.GamePadButton.A);
                }
                if (buttons.B != prevButtons.B)
                {
                    registerButtonPress(buttons.B, InputManager.GamePadButton.B);
                }
                if (buttons.X != prevButtons.X)
                {
                    registerButtonPress(buttons.X, InputManager.GamePadButton.X);
                }
                if (buttons.Y != prevButtons.Y)
                {
                    registerButtonPress(buttons.Y, InputManager.GamePadButton.Y);
                }
                if (buttons.Back != prevButtons.Back)
                {
                    registerButtonPress(buttons.Back, InputManager.GamePadButton.Back);
                }
                if (buttons.Start != prevButtons.Start)
                {
                    registerButtonPress(buttons.Start, InputManager.GamePadButton.Start);
                }
                if (buttons.BigButton != prevButtons.BigButton)
                {
                    registerButtonPress(buttons.BigButton, InputManager.GamePadButton.BigButton);
                }
                if (buttons.LeftShoulder != prevButtons.LeftShoulder)
                {
                    registerButtonPress(buttons.LeftShoulder, InputManager.GamePadButton.LeftShoulder);
                }
                if (buttons.RightShoulder != prevButtons.RightShoulder)
                {
                    registerButtonPress(buttons.RightShoulder, InputManager.GamePadButton.RightShoulder);
                }
                if (buttons.LeftStick != prevButtons.LeftStick)
                {
                    registerButtonPress(buttons.LeftStick, InputManager.GamePadButton.LeftStick);
                }
                if (buttons.RightStick != prevButtons.RightStick)
                {
                    registerButtonPress(buttons.RightStick, InputManager.GamePadButton.RightStick);
                }
                #endregion

                _previousGamePadStates[i] = _gamePadStates[i];
            }
            #endregion

            World.Instance.Simulate(gameTime);

            base.Update(gameTime);
        }

        private void registerButtonPress(ButtonState state, InputManager.GamePadButton button)
        {
            if (state == ButtonState.Pressed)
            {
                InputManager.Instance.OnButtonDown(button);
            }
            else
            {
                InputManager.Instance.OnButtonUp(button);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            World.Instance.Render(gameTime, GraphicsDevice, _spriteBatch);

            base.Draw(gameTime);
        }

        private void KeyDown(Keys key)
        {
            // TODO: We already have this?  Why check it again?
            KeyboardState currentKeyboardState = Keyboard.GetState();
            bool bShiftHeld = currentKeyboardState.IsKeyDown(Keys.LeftShift) | currentKeyboardState.IsKeyDown(Keys.RightShift); 
            if (DeveloperConsole.Instance.GetInput(key, bShiftHeld))
                return;

            if (InputManager.Instance.OnKeyDown(key))
                return;

            if (key == DeveloperConsole.Instance.ToggleConsoleKey)
                DeveloperConsole.Instance.Enabled = true;
        }

        private void KeyUp(Keys key)
        {
            if (DeveloperConsole.Instance.Enabled)
                return;

            if (InputManager.Instance.OnKeyUp(key))
                return;
        }
    }
}
