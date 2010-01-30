using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure;
using AngelXNA.Util;
using Microsoft.Xna.Framework.Content;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Messaging;
using AngelXNA.Infrastructure.Intervals;
using AngelXNA.Infrastructure.Logging;
using System.ComponentModel;
using System.IO;

#if WINDOWS
using System.Drawing.Design;
using AngelXNA.Editor;
#endif

namespace AngelXNA.Actors
{
    public class Actor : Renderable
    {
        public const int c_MaxSpriteFrames = 64;
        static string[] c_StringSplit = new string[] { ", " };

        public enum SpriteAnimationType
        {
            None,
            Loop,
            PingPong,
            OneShot
        };

        public enum ActorDrawShape
        {
            Square,
            Circle
        }

        private static Dictionary<string, Actor> s_NameList = new Dictionary<string,Actor>();
        
        // Rendering 
        protected static Texture2D s_defaultTexture;

        private int _spriteNumFrames = 0;
        private float _spriteAnimDelay;
	    private float _spriteCurrentFrameDelay;
        private List<string> _tags = new List<string>();
	    private SpriteAnimationType _spriteAnimType;
        private int _spriteAnimDirection;
        private string _currentAnimName;
        private string _name;
        private string _actorDefinition;
        private string _sSpriteFile;

        protected Texture2D[] _spriteTextures = new Texture2D[c_MaxSpriteFrames];
        protected int _spriteCurrentFrame = 0;

        private Vector2 _UVUpLeft;
        private Vector2 _UVLowRight;

        // Intervals; used for simple animations
        private Vector2Interval _positionInterval; String _positionIntervalMessage;
        private FloatInterval _rotationInterval; String _rotationIntervalMessage;
        private Vector2Interval _sizeInterval; String _sizeIntervalMessage;
        private ColorInterval _colorInterval; String _colorIntervalMessage;

        [ConsoleProperty]
#if WINDOWS
        [Editor(typeof(XnaColorEditor), typeof(UITypeEditor))]
#endif
        public Color Color { get; set; }

        [ConsoleProperty]
        public virtual Vector2 Size { get; set; }

        [ConsoleProperty]
        public virtual Vector2 Position { get; set; }

        [ConsoleProperty]
        public virtual float Rotation { get; set; }
        
        
        public ActorDrawShape DrawShape { get; set; }

        public string ActorDefinition
        {
            get { return _actorDefinition; }
            internal set { _actorDefinition = value; }
        }

        [ConsoleProperty]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null || value.Length == 0)
                    value = "Actor";

                if (Actor.GetNamed(value) == null)
                {
                    _name = value;
                }
                else
                {
                    int icounter = 1;
                    string iteratedName;
                    do
                    {
                        iteratedName = value + icounter++;
                    } while (Actor.GetNamed(iteratedName) != null);

                    _name = iteratedName;
                }

                s_NameList.Add(_name, this);
            }
        }

        [ConsoleProperty]
#if WINDOWS
        [TypeConverter(typeof(ContentConverter))]
#endif
        public string Sprite
        {
            get { return _sSpriteFile; }
            set
            {
                // If this is in animation format, set it as an animation,
                // otherwise, set it as a sprite:
                if (IsAnimFile(value))
                    LoadSpriteFrames(value);
                else
                    SetSprite(value);
            }
        }

        [ConsoleProperty]
        public SpriteAnimationType SpriteAnimType
        {
            get { return _spriteAnimType; }
            set
            {
                PlaySpriteAnimation(_spriteAnimDelay, value);
            }
        }

        [ConsoleProperty]
        public float SpriteAnimSpeed 
        {
            get { return _spriteAnimDelay; }
            set
            {
                PlaySpriteAnimation(value, _spriteAnimType);
            }
        }

        [ConsoleProperty]
        public int SpriteAnimStartFrame { get; set; }

        [ConsoleProperty]
        public int SpriteAnimEndFrame { get; set; }

        public Actor()
        {
            Color = Color.White;
            Size = Vector2.One;
            Rotation = 0.0f;
            Position = Vector2.Zero;
            Name = "";
            DrawShape = ActorDrawShape.Square;

            SetUVs(Vector2.Zero, Vector2.One);

            _spriteTextures[0] = null;
        }

        public void SetUVs(Vector2 aUpLeft, Vector2 aLowRight)
        {
            _UVUpLeft = aUpLeft;
            _UVLowRight = aLowRight;
        }

        public void GetUVs(out Vector2 aUpLeft, out Vector2 aLowRight)
        {
            aUpLeft = _UVUpLeft;
            aLowRight = _UVLowRight;
        }

        public bool IsTagged(string asTag)
        {
            return _tags.Contains(asTag);
        }

        [ConsoleMethod]
        public void Tag(string asTag)
        {
#if XBOX360
            string[] tags = asTag.Split(',');
#else
            string[] tags = asTag.Split(c_StringSplit, StringSplitOptions.RemoveEmptyEntries);
#endif
            for (int i = 0; i < tags.Length; ++i)
            {
                tags[i] = tags[i].ToLower();
                _tags.Add(tags[i]);
                TagCollection.Instance.AddObjectToTagList(this, tags[i]);
            }
        }

        public void Untag(string asOldTag)
        {
            // TODO: Remove just first or all?
            _tags.Remove(asOldTag);
            TagCollection.Instance.RemoveObjectFromTagList(this, asOldTag);
        }

        public string[] GetTags()
        {
            return _tags.ToArray();
        }

        [ConsoleMethod]
        public void SetLayerName(string layerName)
        {
            string cvarName = "layer_" + layerName;
            ConsoleVariable cvar = DeveloperConsole.Instance.ItemManager.FindCVar(cvarName);
            if (cvar != null)
            {
                Layer = Convert.ToInt32(cvar.Value);
            }
        }

        public Texture2D GetSpriteTexture()
        {
            return GetSpriteTexture(0);
        }

        public Texture2D GetSpriteTexture(int aiFrame)
        {
            aiFrame = MathUtil.Clamp(aiFrame, 0, _spriteNumFrames - 1);

            return _spriteTextures[aiFrame];
        }

        public bool SetSprite(string asFileName)
        {
            _sSpriteFile = asFileName;

            return SetSprite(asFileName, 0);
        }

        [ConsoleMethod]
        public bool SetSprite(string asFileName, int aiFrame)
        {
            Texture2D texture = null;
#if WINDOWS
            // These are the extensions we'll try before attempting Content.Load
            string[] extensions = new string[] { ".png", ".jpg", ".bmp" };
            foreach (string ext in extensions)
            {
                string filename = "Content\\" + asFileName + ext;
                if (File.Exists(filename))
                {
                    texture = Texture2D.FromFile(World.Instance.Game.GraphicsDevice, filename);
                }
            }
#endif

            if(texture == null)
                if (File.Exists(Path.Combine("Content", asFileName + ".xnb")))
                    texture = World.Instance.Game.Content.Load<Texture2D>(asFileName);

            if(texture != null)
                SetSpriteTexture(texture, aiFrame);
            return texture != null;
        }

        public void ClearSpriteInfo()
        {
            for (int i = 0; i < _spriteNumFrames; ++i)
            {
                _spriteTextures[i] = null;
            }
            
            _spriteAnimType = SpriteAnimationType.None;
            _spriteAnimDelay = 0.0f;

            _spriteCurrentFrame = 0;
        }

        public void SetSpriteFrame(int aiFrame)
        {
            aiFrame = MathUtil.Clamp(aiFrame, 0, _spriteNumFrames - 1);

            if (_spriteTextures[aiFrame] == null)
            {
                Log.Instance.Log("SetSpriteFrame() - Warning: frame(" + aiFrame + ") has an invalid texture reference.");
            }

            _spriteCurrentFrame = aiFrame;
        }

        public void PlaySpriteAnimation(float afDelay, SpriteAnimationType aeType)
        {
            PlaySpriteAnimation(afDelay, aeType, -1, _spriteNumFrames - 1);
        }

        public void PlaySpriteAnimation(float afDelay, SpriteAnimationType aeType, int aiStartFrame, int aiEndFrame)
        {
            PlaySpriteAnimation(afDelay, aeType, aiStartFrame, aiEndFrame, null);
        }

        public void PlaySpriteAnimation(float afDelay, SpriteAnimationType aeType, int aiStartFrame, int aiEndFrame, string asAnimName)
        {
            aiStartFrame = MathUtil.Clamp(aiStartFrame, 0, _spriteNumFrames - 1);
            aiEndFrame = MathUtil.Clamp(aiEndFrame, 0, _spriteNumFrames - 1);

            _spriteAnimDirection = aiStartFrame > aiEndFrame ? -1 : 1;

            _spriteCurrentFrameDelay = _spriteAnimDelay = afDelay;
            _spriteAnimType = aeType;
            SpriteAnimStartFrame = _spriteCurrentFrame = aiStartFrame;
            SpriteAnimEndFrame = aiEndFrame;

            if (asAnimName != null)
                _currentAnimName = asAnimName;
        }

        /// <summary>
        /// We expect the name of the first image to end in _###. 
		/// The number of digits doesn't matter, but internally, we are limited 
        /// to 64 frames.  To change that limit, just change c_MaxSpriteFrames
        /// </summary>
        [ConsoleMethod]
        public void LoadSpriteFrames(string asFirstFileName)
        {
            if (!IsAnimFile(asFirstFileName))
            {
                Log.Instance.Log("LoadSpriteFrames() - Bad Format - Expecting somename_###.ext");
                Log.Instance.Log("Attempting to load single texture: " + asFirstFileName);

                if (!SetSprite(asFirstFileName, 0))
                    return;
            }
            
            // reset the number of sprite frames
            _sSpriteFile = asFirstFileName;
            _spriteNumFrames = 0;

	        int numberSeparator = asFirstFileName.LastIndexOf("_");
	        int numDigits = asFirstFileName.Length - numberSeparator - 1;

	        // If we got this far, the filename format is correct.
	        // The number string is just the digits between the '_' and the file extension (i.e. 001).
	        string numberString = asFirstFileName.Substring(numberSeparator + 1, numDigits);

	        // Get our starting numberical value.
	        int number = int.Parse(numberString);

	        // The base name is everything up to the '_' before the number (i.e. somefile_).
	        string baseFilename = asFirstFileName.Substring(0, numberSeparator + 1);

	        // Keep loading until we stop finding images in the sequence.
	        while (true)
	        {
		        // Build up the filename of the current image in the sequence.
		        string newFilename = baseFilename + numberString;

                if (!SetSprite(newFilename, _spriteNumFrames))
                    break;  // Failed, file doesn't exist
        		
		        // Verify we don't go out of range on our hard-coded frame limit per sprite.
		        if (_spriteNumFrames >= c_MaxSpriteFrames)
		        {
                    Log.Instance.Log("Maximum number of frames reached (" + c_MaxSpriteFrames + "). Bailing out...");
                    Log.Instance.Log("Increment c_MaxSpriteFrames if you need more.");
			        break;
		        }

		        // Bump the number to the next value in the sequence.
		        ++number;

		        // Serialize the numerical value to it so we can retrieve the string equivalent.
                string newNumberString = number.ToString();

		        int numLeadingZeros = 0;
		        if (newNumberString.Length < numDigits)
		        {
			        numLeadingZeros = numDigits - newNumberString.Length;
		        }

		        // Do the leading zero padding.
		        for (int i=0; i<numLeadingZeros; ++i)
		        {
			        newNumberString = '0' + newNumberString;
		        }

		        // Save off the newly formulated number string for the next image in the sequence.
		        numberString = newNumberString;
	        }
        }

        public void UpdateSpriteAnimation(GameTime aTime)
        {
            float dt = (float)aTime.ElapsedGameTime.TotalSeconds;

            if (_spriteAnimDelay > 0.0f)
            {
                _spriteCurrentFrameDelay -= dt;

                if (_spriteCurrentFrameDelay < 0.0f)
                {
                    while (_spriteCurrentFrameDelay < 0.0f)
                    {
                        switch(_spriteAnimType)
                        {
                            case SpriteAnimationType.Loop:
                                if (_spriteCurrentFrame == SpriteAnimEndFrame)
                                    _spriteCurrentFrame = SpriteAnimStartFrame;
                                else
                                    ++_spriteCurrentFrame;
                                break;
                            case SpriteAnimationType.PingPong:
                                if (_spriteAnimDirection == 1)
                                {
                                    if (_spriteCurrentFrame == SpriteAnimEndFrame)
                                    {
                                        _spriteAnimDirection = -1;
                                        _spriteCurrentFrame = SpriteAnimEndFrame - 1;
                                    }
                                    else
                                        ++_spriteCurrentFrame;

                                }
                                else
                                {
                                    if (_spriteCurrentFrame == SpriteAnimStartFrame)
                                    {
                                        _spriteAnimDirection = 1;
                                        _spriteCurrentFrame = SpriteAnimStartFrame + 1;
                                    }
                                    else
                                    {
                                        --_spriteCurrentFrame;
                                    }
                                }
                                break;
                            case SpriteAnimationType.OneShot:
                                // If we're done with our one shot and they set an animName, let them know it's done.
                                if (_spriteCurrentFrame == SpriteAnimEndFrame)
                                {
                                    // Needs to get called before callback, in case they start a new animation.
                                    _spriteAnimType = SpriteAnimationType.None;

                                    if (_currentAnimName != null && _currentAnimName.Length > 0)
                                    {
                                        AnimCallback(_currentAnimName);
                                    }
                                }
                                else
                                {
                                    _spriteCurrentFrame += _spriteAnimDirection;
                                }
                                break;
                        }

                        _spriteCurrentFrameDelay += _spriteAnimDelay;
                    }
                }
            }
        }

        public override void Update(GameTime aTime)
        {
            float dt = (float)aTime.ElapsedGameTime.TotalSeconds;

            UpdateSpriteAnimation(aTime);

            if (_positionInterval != null && _positionInterval.ShouldStep)
            {
                Position = _positionInterval.Step(dt);
                if (!_positionInterval.ShouldStep)
                {
                    if (_positionIntervalMessage != "")
                    {
                        Switchboard.Instance.Broadcast(new Message(_positionIntervalMessage, this));
                    }
                }
            }

            if (_rotationInterval != null && _rotationInterval.ShouldStep)
            {
                Rotation = _rotationInterval.Step(dt);
                if (!_rotationInterval.ShouldStep)
                {
                    if (_rotationIntervalMessage != "")
                    {
                        Switchboard.Instance.Broadcast(new Message(_rotationIntervalMessage, this));
                    }
                }
            }

            if (_colorInterval != null && _colorInterval.ShouldStep)
            {
                Color = _colorInterval.Step(dt);
                if (!_colorInterval.ShouldStep)
                {
                    if (_colorIntervalMessage != "")
                    {
                        Switchboard.Instance.Broadcast(new Message(_colorIntervalMessage, this));
                    }
                }
            }

            if (_sizeInterval != null && _sizeInterval.ShouldStep)
            {
                Size = _sizeInterval.Step(dt);
                if (!_sizeInterval.ShouldStep)
                {
                    if (_sizeIntervalMessage != "")
                    {
                        Switchboard.Instance.Broadcast(new Message(_sizeIntervalMessage, this));
                    }
                }
            }
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            // TODO: Rendering needs refactoring... a LOT.
            if (s_defaultTexture == null)
                s_defaultTexture = World.Instance.Game.Content.Load<Texture2D>("white");

            Texture2D myTexture = _spriteTextures[_spriteCurrentFrame];
            if(myTexture == null)
                myTexture = s_defaultTexture;

            Vector2 screenPos = aCamera.WorldToScreen(Position);
            Vector2 screenSize = aCamera.WorldSizeToScreenSize(Size);
            Rectangle destRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)screenSize.X, (int)screenSize.Y);

            aBatch.Begin();
            aBatch.Draw(myTexture, destRect, null, Color, MathHelper.ToRadians(Rotation),
               new Vector2(myTexture.Width / 2, myTexture.Height / 2), SpriteEffects.None, 0.0f);
            aBatch.End();
        }

        /// <summary>
        /// When an actor is removed from the world it loses all tags...
        /// Is this what we want? 
        /// </summary>
        public override void RemovedFromWorld()
        {
            for (int i = 0; i < _tags.Count; ++i)
            {
                TagCollection.Instance.RemoveObjectFromTagList(this, _tags[i]);
            }
            _tags.Clear();
            
            base.RemovedFromWorld();
        }

        public virtual void AnimCallback(string animName) { }
        public virtual void LevelUnloaded() { }

        private void SetSpriteTexture(Texture2D aTexture)
        {
            SetSpriteTexture(aTexture, 0);
        }

        private void SetSpriteTexture(Texture2D aiTexture, int aiFrame)
        {
            aiFrame = MathUtil.Clamp(aiFrame, 0, c_MaxSpriteFrames - 1);
            if (aiFrame >= _spriteNumFrames)
            {
                _spriteNumFrames = aiFrame + 1;
            }

            _spriteTextures[aiFrame] = aiTexture;
        }

        public void MoveTo(Vector2 destination, float duration, bool smooth)
        {
            MoveTo(destination, duration, smooth, "");
        }
        
        public void MoveTo(Vector2 destination, float duration, bool smooth, string onCompletionMessage)
        {
            _positionInterval = new Vector2Interval(Position, destination, duration, smooth);
            _positionIntervalMessage = onCompletionMessage;
        }

        public void RotateTo(float endingRotation, float duration, bool smooth)
        {
            RotateTo(endingRotation, duration, smooth, "");
        }

        public void RotateTo(float endingRotation, float duration, bool smooth, string onCompletionMessage)
        {
            _rotationInterval = new FloatInterval(Rotation, endingRotation, duration, smooth);
            _rotationIntervalMessage = onCompletionMessage;
        }

        public void ChangeColorTo(Color endColor, float duration, bool smooth)
        {
            ChangeColorTo(endColor, duration, smooth, "");
        }

        public void ChangeColorTo(Color endColor, float duration, bool smooth, string onCompletionMessage) {
            _colorInterval = new ColorInterval(Color, endColor, duration, smooth);
	        _colorIntervalMessage = onCompletionMessage;
        }

        public void ChangeSizeTo(Vector2 endSize, float duration, bool smooth)
        {
            ChangeSizeTo(endSize, duration, smooth, "");
        }

        public void ChangeSizeTo(Vector2 endSize, float duration, bool smooth, string onCompletionMessage) {
            _sizeInterval = new Vector2Interval(Size, endSize, duration, smooth);
	        _sizeIntervalMessage = onCompletionMessage;
        }

        public void ChangeSizeTo(float endSize, float duration, bool smooth)
        {
            ChangeSizeTo(endSize, duration, smooth, "");
        }

        public void ChangeSizeTo(float endSize, float duration, bool smooth, string onCompletionMessage) {
            ChangeSizeTo(new Vector2(endSize, endSize), duration, smooth, onCompletionMessage);
        }
        
        public static Actor GetNamed(string asName)
        {
            if(s_NameList.ContainsKey(asName))
                return s_NameList[asName];
            
            return null;
        }

        public static bool IsAnimFile(string asFileName)
        {
            int numberSeparator = asFileName.LastIndexOf("_");
            int numDigits = asFileName.Length - numberSeparator - 1;

            bool bValidNumber = true;
            // So you're saying I've got a chance?
            if (numberSeparator > 0 && numDigits > 0)
            {
                // Now see if all of the digits between _ and . are numbers (i.e. test_001.jpg).
                for (int i = 1; i <= numDigits; ++i)
                {
                    if (!Char.IsDigit(asFileName[numberSeparator + i]))
                    {
                        bValidNumber = false;
                        break;
                    }
                }
            }

            return !(numberSeparator == -1 || numDigits <= 0 || !bValidNumber);
        }

        [ConsoleMethod]
        public static Actor Create()
        {
            return new Actor();
        }
    }
}
