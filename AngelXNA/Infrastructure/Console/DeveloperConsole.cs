using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Util;
using System.Reflection;

namespace AngelXNA.Infrastructure.Console
{
    public class DeveloperConsole
    {
        // Constants!
        private const float c_fConsoleAlpha = 0.75f;
        private const float c_fScreenHeightPercent = 0.5f;
        private const int c_iTextBoxBorder = 4;
        private const int c_iMaxAutoCompleteLines = 7;

        private static char[] s_NumberSymbolMatching = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };
        private static Color s_ConsoleColor = new Color(0.0f, 0.0f, 0.0f, c_fConsoleAlpha);
        private static Color s_ConsoleTextColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        private static Color s_ConsoleBorderColor = new Color(0.0f, 1.0f, 0.0f, c_fConsoleAlpha/2);

        private static DeveloperConsole s_Instance = null;

        public static DeveloperConsole Instance
        {
            get 
            {
                if (s_Instance == null)
                    s_Instance = new DeveloperConsole();
                return s_Instance; 
            }
        }

        private ConsoleItemManager _manager = new ConsoleItemManager();
        private ConsoleLog _consoleLog = new ConsoleLog();
        private int _consoleLogPos;
        private string _currentInput = "";
        private List<string> _inputHistory = new List<string>();
        private int _inputHistoryPos;
        private bool _inConfigFileOp = false;
        private List<string> _autoCompleteList = new List<string>();

        public bool Enabled { get; set; }
        public bool IsReadingConfigFile { get { return _inConfigFileOp; } }
        public Keys ToggleConsoleKey { get { return Keys.OemTilde; } }
        public object CurrentScope { get; set; }

        public ConsoleItemManager ItemManager { get { return _manager; } }

        protected DeveloperConsole()
        {
            FontCache.Instance.RegisterFont("Fonts\\Inconsolata12", "ConsoleSmall");

            // Scan all loaded assemblies for anything tagged ConsoleMethod
#if WINDOWS
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);

            foreach (Assembly assm in AppDomain.CurrentDomain.GetAssemblies())
            {
                _manager.LoadMembersFromAssembly(assm);
            }
#else
            _manager.LoadMembersFromAssembly(Assembly.GetExecutingAssembly());
            _manager.LoadMembersFromAssembly(World.Instance.Game.GetType().Assembly);
#endif


            // Add all other global methods
            _manager.AddCommand("ExecuteFile", x => {
                VerifyArgs(x, typeof(string));
                ExecuteFile((string)x[0]);
                return null;
            });
            _manager.AddCommand("ExecConfigFile", x => {
                VerifyArgs(x, typeof(string));
                ExecConfigFile((string)x[0]);
                return null;
            });
            _manager.AddCommand("CCmdList", x => {
                string filter = null;
                if (x.Length > 0)
                {
                    VerifyArgs(x, typeof(string));
                    filter = (string)x[0];
                }
                CCmdList(filter);
                return null;
            });
            _manager.AddCommand("CVarList", x => {
                string filter = null;
                if (x.Length > 0)
                {
                    VerifyArgs(x, typeof(string));
                    filter = (string)x[0];
                }
                CVarList(filter);
                return null;
            });
            _manager.AddCommand("Echo", x => {
                string outString;
                if (x[0] is string)
                    outString = (string)x[0];
                else
                    outString = x[0].ToString();
                Echo(outString);
                return null;
            });
            _manager.AddCommand("Using", x => {
                VerifyArgs(x, typeof(object));
                Using(x[0]);
                return null;
            });
            _manager.AddCommand("EndUsing", x => {
                return EndUsing();
            });

            _manager.AddType(new ConsoleType(typeof(Color), "Color", 
                // Color Serializer
                x => { 
                    Vector4 value = ((Color)x).ToVector4();
                    return String.Format("{0}, {1}, {2}, {3}", value.X, value.Y, value.Z, value.W);
                },
                // Color Deserialiser
                x => {
                    VerifyArgs(x, typeof(float), typeof(float), typeof(float), typeof(float));
                    return new Color((float)x[0], (float)x[1], (float)x[2], (float)x[3]);
                }));
            
            _manager.AddType(new ConsoleType(typeof(Vector2), "Vector2", 
                // Vector2 serializer
                x => {
                    StringBuilder sb = new StringBuilder();
                    Vector2 value = (Vector2)x;
                    return String.Format("{0}, {1}", value.X, value.Y);
                },
                // Vector2 Deserialzer
                x => {
                    VerifyArgs(x, typeof(float), typeof(float));
                    return new Vector2((float)x[0], (float)x[1]);
                }));
        }

        public void Update(GameTime aTime)
        {

        }

        public void Render(Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            // TODO: This is a mess, both a mess from Angel originally and because
            // we're hacking together rendering at the moment.
            if (!Enabled)
                return;

            SpriteFont font = FontCache.Instance["ConsoleSmall"];

            // First step, draw the console background, textbox, etc.
            int consoleWidth = aCamera.WindowWidth;
            int consoleBGHeight = (int)(aCamera.WindowHeight * c_fScreenHeightPercent);

            // Console log
            DrawUtil.DrawTile(aCamera, aDevice, 0, 0, consoleWidth, consoleBGHeight, s_ConsoleColor);

            // Console textbox border
            int textBoxPlusBorderHeight = font.LineSpacing + c_iTextBoxBorder;
            DrawUtil.DrawTile(aCamera, aDevice, 0, consoleBGHeight, consoleWidth, textBoxPlusBorderHeight, s_ConsoleBorderColor);

	        //Draw text box
            int textBoxHeight = textBoxPlusBorderHeight - c_iTextBoxBorder;
            int textBoxWidth = consoleWidth - c_iTextBoxBorder;
            int textBoxXPos = (consoleWidth - textBoxWidth) / 2;
            int textBoxYPos = consoleBGHeight + c_iTextBoxBorder / 2;

            DrawUtil.DrawTile(aCamera, aDevice, textBoxXPos, textBoxYPos, textBoxWidth, textBoxHeight, s_ConsoleColor);

            aBatch.Begin();
            
            // First render log text
            int logXPos = c_iTextBoxBorder;
            int logYPos = consoleBGHeight - c_iTextBoxBorder - font.LineSpacing;
            if (_consoleLog.NumLogLines() > 0)
            {
                for (int i = _consoleLogPos; i >= 0 && logYPos > 0; i--)
                {
                    aBatch.DrawString(font, _consoleLog.GetLogLine(i), new Vector2(logXPos, logYPos), Color.White);
                    logYPos -= font.LineSpacing;
                }
            }

            // Current input
	        string printInput = "<) ";
	        printInput += _currentInput + "_";

            int textYPos = textBoxYPos + (textBoxHeight - font.LineSpacing) / 2;
            aBatch.DrawString(font, printInput, new Vector2(textBoxXPos, textYPos), s_ConsoleTextColor);

            aBatch.End();

	        //Draw autocomplete if applicable
            if (_autoCompleteList.Count > 0)
            {
                int autoCompletePadding = 6;
                int numAutoCompleteLines = Math.Min(c_iMaxAutoCompleteLines, _autoCompleteList.Count);
                int autoCompleteStartY = textBoxYPos + textBoxPlusBorderHeight;
                
                int autoCompleteXPos = textBoxXPos + aCamera.WindowHeight / 24;
                int autoCompleteBoxXPos = autoCompleteXPos - c_iTextBoxBorder;

                DrawUtil.DrawTile(aCamera, aDevice, autoCompleteBoxXPos, autoCompleteStartY, consoleWidth - autoCompleteBoxXPos, numAutoCompleteLines * (font.LineSpacing + autoCompletePadding), s_ConsoleColor);

                aBatch.Begin();
                Vector2 outPos = new Vector2((float)autoCompleteXPos, (float)autoCompleteStartY);
                for (int i = 0; i < numAutoCompleteLines; i++)
                {
                    if (_autoCompleteList.Count > c_iMaxAutoCompleteLines - 1 && i == c_iMaxAutoCompleteLines - 1)
                        aBatch.DrawString(font, "...", new Vector2(autoCompleteXPos, (int)outPos.Y + autoCompletePadding / 2), s_ConsoleTextColor);
                    else
                        aBatch.DrawString(font, _autoCompleteList[i], new Vector2(autoCompleteXPos, (int)outPos.Y + autoCompletePadding / 2), s_ConsoleTextColor);
                    outPos.Y += font.LineSpacing + autoCompletePadding;
                }
                aBatch.End();
            }
        }

        public bool GetInput(Keys aKey, bool abShiftHeld)
        {
            if (!Enabled)
                return false;

            if(aKey == ToggleConsoleKey)
            {
                Enabled = false;
            }

            // Slight change from AngelCPP funcitonality.  If there's something in
            // the current input, remove it before closing the console.
            switch(aKey)
            {
                case Keys.Escape:
                    if(_currentInput != "")
                        SetCurrentInput("");
                    else
                        Enabled = false;
                    break;

                case Keys.Up: AdvanceInputHistory(-1); break;
                case Keys.Down: AdvanceInputHistory(1); break;
                case Keys.PageUp: AdvanceConsoleLog(-1); break;
                case Keys.PageDown: AdvanceConsoleLog(1); break;
                case Keys.Enter: AcceptCurrentInput(); break;
                case Keys.Tab: AcceptAutoComplete(); break;
                case Keys.Back: 
                    // TODO: C# must have a better way to remove the last line of
                    // something like this.
                    int size = _currentInput.Length;
                    if (size > 0)
                    {
                        SetCurrentInput(_currentInput.Substring(0, size - 1));
                    }
                    break;
                // From here down are text keys.  OEM keys are difficult,
                // so we're supplying their translations.
                case Keys.OemPeriod: _currentInput += '.'; break;
                case Keys.OemComma: _currentInput += ','; break;
                case Keys.OemMinus: 
                    if(abShiftHeld)
                        _currentInput += '_'; 
                    else
                        _currentInput += '-'; 
                    break;
                case Keys.OemQuestion: _currentInput += '/'; break;
                case Keys.OemQuotes:
                    if (abShiftHeld)
                        _currentInput += "\"";
                    else
                        _currentInput += "'";
                    break;
                case Keys.OemPlus:
                    if (abShiftHeld)
                        _currentInput += "+";
                    else
                        _currentInput += "=";
                    break;
                default:
                    if (IsTextKey(aKey))
                    {
                        char theKey = (char)aKey;
                        // Digit to symbol matching...
                        if (Char.IsDigit(theKey) && abShiftHeld)
                            theKey = s_NumberSymbolMatching[theKey - '0'];
                        else if (!abShiftHeld)
                            theKey = Char.ToLower(theKey);
                        _currentInput += (char)theKey;
                        SetCurrentInput(_currentInput);
                    }
                    break;
            }

            return true;
        }

        public void ExecuteInConsole(string input)
        {
            ExecuteInConsole(input, false);
        }

        public void ExecuteInConsole(string input, bool bNoLog)
        {
            if (input.Length == 0)
                return;

            ConsoleParser parser = new ConsoleParser(this);
            parser.Execute(input);
        }

        public object ExecuteFile(string fileName)
        {
            const char commentChar = ';';

            //Readonly writes are allowed during config file load
	        bool bCached = _inConfigFileOp;
	        _inConfigFileOp = true;
	        
	        List<string> lines = new List<string>();
	        if( FileUtils.GetLinesFromFile(fileName, lines ) )
	        {
		        //Ignore commented lines (and partial lines)
		        for( int i = 0; i < lines.Count; ++i)
		        {
			        string consoleInput = lines[i].Trim();
			        //If this line starts with commentChar, skip it
			        if( consoleInput.Length == 0 || consoleInput[0] == commentChar )
				        continue;

			        //Otherwise, check to see if there's a trailing comment
                    int commentIndex = consoleInput.IndexOf(commentChar);
                    if(commentIndex > 0)
                        consoleInput = consoleInput.Substring(0, commentIndex);

			        ExecuteInConsole( consoleInput );
		        }
	        }
	        else
	        {
		        AddToConsoleLog("Couldn't open file: " + fileName);

                _inConfigFileOp = bCached;
                throw new System.IO.FileNotFoundException("File not found: " + fileName);
	        }

	        _inConfigFileOp=bCached;

            return null;
        }

        public object ExecConfigFile(string fileName)
        {
            return ExecuteFile(string.Format("Config\\{0}.cfg", fileName));
        }

        public object CCmdList(string filter)
        {
            List<string> out_list = new List<string>();

            string trimmed = filter == null ? String.Empty : filter.Trim();

            AddToConsoleLog(" ");
            AddToConsoleLog(" ");

            if (trimmed.Length > 0)
                AddToConsoleLog("Listing all ConsoleCommands that begin with " + trimmed);
            else
                AddToConsoleLog("Listing all ConsoleCommands");

            AddToConsoleLog("-------------");
            
            _manager.GetConsoleCommandIds(trimmed, ref out_list);
            AddToConsoleLog(out_list);
            
            return null;
        }

        public object CVarList(string filter)
        {
            List<string> out_list = new List<string>();

            string trimmed = filter == null ? String.Empty : filter.Trim();

            AddToConsoleLog(" ");
            AddToConsoleLog(" ");
            if (trimmed.Length > 0)
                AddToConsoleLog("Listing all ConsoleVariables that begin with " + trimmed);
            else
                AddToConsoleLog("Listing all ConsoleVariables");
            AddToConsoleLog("-------------");
            
            _manager.GetConsoleVariableIds(trimmed, ref out_list);
            AddToConsoleLog(out_list);

            return null;
        }

        public object Echo(string input)
        {
            AddToConsoleLog(input);
            return null;
        }

        public object Using(object var)
        {
            if (CurrentScope != null)
                AddToConsoleLog("Using() called when another object was already in scope.");
            else
                CurrentScope = var;

            return null;
        }

        public object EndUsing()
        {
            object oldObject = CurrentScope;
            if (CurrentScope == null)
                AddToConsoleLog("EndUsing() called when nothing was being used.");
            else
                CurrentScope = null;

            return oldObject;
        }

        public void WriteConfigCvars()
        {
            //Don't write out to config files if we're in the middle of reading one
            if (_inConfigFileOp)
                return;

            List<string> configCvars = new List<string>();
            _manager.SerializeConfigCVars(configCvars);
            String configVarFileName = "Config//ConfigCvars.cfg";
            if (FileUtils.WriteLinesToFile(configVarFileName, configCvars))
            {
                AddToConsoleLog("Wrote config cvars out to: " + configVarFileName);
            }
        }

        public void AddToConsoleLog(string input)
        {
            bool bAtBottomOfConsole = false;
            if (_consoleLogPos >= _consoleLog.NumLogLines() - 1)
                bAtBottomOfConsole = true;

            _consoleLog.Log(input);
            if (bAtBottomOfConsole)
                AdvanceConsoleLog(_consoleLog.NumLogLines());
        }

        public void AddToConsoleLog(List<string> refList)
        {
            for (int i = 0; i < refList.Count; i++)
            {
                AddToConsoleLog(refList[i]);
            }
        }

#if WINDOWS
        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            _manager.LoadMembersFromAssembly(args.LoadedAssembly);
        }
#endif

        private bool IsTextKey(Keys key)
        {
            char text = (char)key;
            if (text >= ' ' && text <= '}')
                return true;

            return false;
        }

        private void AcceptCurrentInput()
        {
            ExecuteInConsole(_currentInput);

            UpdateInputHistory(_currentInput);
            SetCurrentInput("");
        }

        private void AdvanceInputHistory(int byVal)
        {
            if (_inputHistory.Count == 0)
                return;

            //If we're at the bottom of our input history, do nothing
            int lastInputIndex = _inputHistory.Count - 1;

            _inputHistoryPos += byVal;
            if (_inputHistoryPos > lastInputIndex)
            {
                _inputHistoryPos = _inputHistory.Count;
                SetCurrentInput("");
                return;
            }
            else if (_inputHistoryPos < 0)
                _inputHistoryPos = 0;

            //otherwise, write over our current input
            SetCurrentInput(_inputHistory[_inputHistoryPos]);
        }

        private void UpdateInputHistory(string input)
        {
            //If we're already in the input history, remove the current entry and add to end
            if(_inputHistoryPos < _inputHistory.Count)
                _inputHistory.RemoveAt(_inputHistoryPos);
           
            _inputHistory.Add(input);
            _inputHistoryPos = _inputHistory.Count;
        }
        
        private void UpdateAutoComplete()
        {
            _autoCompleteList.Clear();
            _manager.GetConsoleItemIds(_currentInput.Trim(), ref _autoCompleteList);
        }

        private void AcceptAutoComplete()
        {
            if (_autoCompleteList.Count == 0)
                return;

            string[] conItemDef = _autoCompleteList[0].Split('=');
            SetCurrentInput(conItemDef[0].TrimEnd());
        }

        private void AdvanceConsoleLog(int byVal)
        {
            int numLines = _consoleLog.NumLogLines();
            if (numLines == 0)
                return;

            _consoleLogPos += byVal;
            if (_consoleLogPos >= numLines)
                _consoleLogPos = numLines - 1;
            else if (_consoleLogPos < 0)
                _consoleLogPos = 0;
        }

        private void SetCurrentInput(string asInput)
        {
            _currentInput = asInput;
            UpdateAutoComplete();
        }

        public static void VerifyArgs(object[] aParams, params Type[] aTypes)
        {
            if (aParams.Length != aTypes.Length)
                throw new InvalidOperationException("Invalid number of parameters.");

            for (int i = 0; i < aParams.Length; ++i)
            {
                if (!aTypes[0].IsInstanceOfType(aParams[i]))
                    throw new InvalidOperationException("Parameter " + i + " does not match request type " + aTypes[i]);
            }
        }
    }
}
