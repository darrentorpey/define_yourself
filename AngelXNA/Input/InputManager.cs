using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AngelXNA.Infrastructure;
using AngelXNA.Infrastructure.Console;

namespace AngelXNA.Input
{
    public class InputManager
    {

        const string INPUT_BINDING_FILENAME = "input_bindings";

        public enum MouseButton
        {
            Left,
            Right,
            Middle
        }

        public enum GamePadButton
        {
            A,
            B,
            X,
            Y,
            Back,
            Start,
            BigButton,
            LeftShoulder,
            RightShoulder,
            LeftStick,
            RightStick
        }

        Dictionary<Keys, InputBinding> _keyBindingTable = new Dictionary<Keys, InputBinding>();
        Dictionary<GamePadButton, InputBinding> _buttonBindingTable = new Dictionary<GamePadButton, InputBinding>();
        List<IMouseListener> _mouseListeners = new List<IMouseListener>();
        List<IKeyListener> _keyListeners = new List<IKeyListener>();

        private static InputManager s_Instance;

        public static InputManager Instance
        {
            get {
                if (null == s_Instance)
                {
                    s_Instance = new InputManager();
                    
                    // Load bindings from config file
                    loadInputBindings();
                }
                return s_Instance; 
            }
        }

        private static Dictionary<string, GamePadButton> _buttonMapping = new Dictionary<string, GamePadButton>()
        {
            {"PAD_A", GamePadButton.A},
            {"PAD_B", GamePadButton.B},
            {"PAD_X", GamePadButton.X},
            {"PAD_Y", GamePadButton.Y},
            {"PAD_BACK", GamePadButton.Back},
            {"PAD_START", GamePadButton.Start},
            {"PAD_BIG_BUTTON", GamePadButton.BigButton},
            {"PAD_LEFT_SHOULDER", GamePadButton.LeftShoulder},
            {"PAD_RIGHT_SHOULDER", GamePadButton.RightShoulder},
            {"PAD_LEFT_STICK", GamePadButton.LeftStick},
            {"PAD_RIGHT_STICK", GamePadButton.RightStick}
        };

        // Load all the keyboard and gamepad bindings as defined in the input bindings configuration file
        private static void loadInputBindings()
        {
            const char commentChar = ';';

            List<String> configLines = new List<string>();

            StringBuilder sb = new StringBuilder(@"Config\");
            sb.Append(INPUT_BINDING_FILENAME);
            sb.Append(".conf");
            string inputConfigFilename = sb.ToString();

            if (FileUtils.GetLinesFromFile(inputConfigFilename, configLines))
            {
                // Ignore commented lines (and partial lines)
                for (int i = 0; i < configLines.Count; ++i)
                {
                    string consoleInput = configLines[i].Trim();
                    // If this line starts with commentChar, skip it
                    if (consoleInput.Length == 0 || consoleInput[0] == commentChar)
                        continue;

                    // Otherwise, check to see if there's a trailing comment
                    int commentIndex = consoleInput.IndexOf(commentChar);
                    if (commentIndex > 0)
                        consoleInput = consoleInput.Substring(commentIndex);

                    string[] pairs = consoleInput.Split(new char[] { ':', '=' });
                    string keyString = pairs[0].Trim();
                    string command = pairs[1].Trim();

                    if(_buttonMapping.ContainsKey(keyString))
                    {
                        GamePadButton button = _buttonMapping[keyString];
                        InputManager.Instance.BindButton(button, command);
                    }
                    else
                    {
                        // Key is not one of our XBox gamepad buttons...
                        // Handle the binding it it's for a key
                        try
                        {
                            Keys key = (Keys)Enum.Parse(typeof(Keys), keyString, true);
                            InputManager.Instance.BindKey(key, command); // "ECHO '" + keyString + "' was pressed"
                        }
                        catch (ArgumentException)
                        {
                            // Key is not in XNA's set of keyboard keys
                            // ... nothing left to do
                        }
                    }
                }
            }
            else
            {
                DeveloperConsole.Instance.AddToConsoleLog("Couldn't open file: " + inputConfigFilename);
            }
        }

        public void RegisterMouseListener(IMouseListener mouseListener)
        {
            _mouseListeners.Add(mouseListener);
        }

        public void UnregisterMouseListener(IMouseListener mouseListener)
        {
            _mouseListeners.Remove(mouseListener);
        }

        public void RegisterKeyListerer(IKeyListener keyListener)
        {
            _keyListeners.Add(keyListener);
        }

        public void UnregisterKeyListener(IKeyListener keyListener)
        {
            _keyListeners.Remove(keyListener);
        }

        public void BindButton(GamePadButton button, string command)
        {
            // TODO: Allow input bindings to be either a message send
            // or a console command.
            if (command.Length == 0)
                return;

            InputBinding binding;
            if (!_buttonBindingTable.ContainsKey(button))
            {
                binding = new InputBinding();
                _buttonBindingTable.Add(button, binding);
            }
            else
            {
                binding = _buttonBindingTable[button];
            }

            if (command.StartsWith("-"))
            {
                command = command.Remove(0, 1);
                binding.ReleaseMessage = command;
            }
            else
            {
                if (command.StartsWith("+"))
                    command = command.Remove(0, 1);

                binding.PressMessage = command;
            }
        }

        public void BindKey(Keys key, string command)
        {
            if (command.Length == 0)
                return;

            InputBinding binding;
            if (!_keyBindingTable.ContainsKey(key))
            {
                binding = new InputBinding();
                _keyBindingTable.Add(key, binding);
            }
            else
            {
                binding = _keyBindingTable[key];
            }

            if (command.StartsWith("-"))
            {
                command = command.Remove(0, 1);
                binding.ReleaseMessage = command;
            }
            else
            {
                if (command.StartsWith("+"))
                    command = command.Remove(0, 1);

                binding.PressMessage = command;
            }

        }

        public void UnbindKey(Keys key) 
        {
            _keyBindingTable.Remove(key);
        }

        public void MouseButtonAction(Vector2 position, MouseButton button, ButtonState state)
        {
            foreach (IMouseListener listener in _mouseListeners)
            {
                if (state == ButtonState.Pressed)
                {
                    listener.MouseDownEvent(position, button);
                }
                else
                {
                    listener.MouseUpEvent(position, button);
                }
            }
        }

        public void MouseMotionAction(int screenPosX, int screenPosY)
        {
            foreach (IMouseListener listener in _mouseListeners)
            {
                listener.MouseMotionEvent(screenPosX, screenPosY);
            }
        }

        public bool OnKeyDown(Keys key)
        {
            foreach (IKeyListener listener in _keyListeners)
            {
                listener.OnKeyDown(key);
            }

            if (!_keyBindingTable.ContainsKey(key))
            {
                return false;
            }
            InputBinding binding = _keyBindingTable[key];
            binding.OnPress();
            return true;
        }

        public bool OnKeyUp(Keys key)
        {
            foreach (IKeyListener listener in _keyListeners)
            {
                listener.OnKeyUp(key);
            }

            if (!_keyBindingTable.ContainsKey(key))
            {
                return false;
            }
            InputBinding binding = _keyBindingTable[key];
            binding.OnRelease();
            return true;
        }

        public bool IsKeyDown(Keys key, PlayerIndex playerIndex)
        {
            return Keyboard.GetState(playerIndex).IsKeyDown(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return IsKeyDown(key, PlayerIndex.One);
        }

        public bool OnButtonDown(GamePadButton button)
        {
            if (!_buttonBindingTable.ContainsKey(button))
            {
                return false;
            }
            InputBinding binding = _buttonBindingTable[button];
            binding.OnPress();
            return true;
        }

        public bool OnButtonUp(GamePadButton button)
        {
            if (!_buttonBindingTable.ContainsKey(button))
            {
                return false;
            }
            InputBinding binding = _buttonBindingTable[button];
            binding.OnRelease();
            return true;
        }
    }
}
