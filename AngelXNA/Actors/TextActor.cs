using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Util;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework.Graphics;

#if WINDOWS
using AngelXNA.Editor;
using System.ComponentModel;
#endif

namespace AngelXNA.Actors
{
    public class TextActor : Actor
    {
        public enum Alignment
        {
            Left,
            Center,
            Right
        }
        
        private struct TextNugget
        {
            public string _string;
            public Vector2 _extents;
            public Vector2 _position;
        }

        private string _font;
        private int _lineSpacing;
        private Alignment _alignment;
        private TextNugget[] _displayStrings;
        private string _rawString;

#if WINDOWS
        [TypeConverter(typeof(FontConverter))]
#endif
        public string Font 
        {
            get { return _font; }
            set
            {
                _font = value;
                CalculatePosition();
            }
        }

        public int LineSpacing 
        { 
            get { return _lineSpacing; }
            set
            {
                _lineSpacing = value;
                CalculatePosition();
            }
        }
        
        public Alignment TextAlignment 
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                CalculatePosition();
            }
        }

        public override Vector2 Position
        {
            set
            {
                base.Position = value;
                CalculatePosition();
            }
        }

        public string DisplayString 
        {
            get { return _rawString; }
            set
            {
                _rawString = value;
#if XBOX360
                string[] strings = _rawString.Split(new char[] {'\n'});
#else
                string[] strings = _rawString.Split(new string[] {"\n"}, StringSplitOptions.None);
#endif
                _displayStrings = new TextNugget[strings.Length];
                for (int i = 0; i < strings.Length; ++i)
                {
                    _displayStrings[i]._string = strings[i];
                }

                CalculatePosition();
            }
        }

        public TextActor()
            : this("Default", "Text Actor")
        {

        }

        public TextActor(string fontNickName, string displayString)
            : this(fontNickName, displayString, Alignment.Left)
        {
            
        }

        public TextActor(string fontNickName, string displayString, Alignment align)
            : this(fontNickName, displayString, align, 5)
        {

        }

        public TextActor(string fontNickName, string displayString, Alignment align, int lineSpacing )
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.Black;
            Font = fontNickName;
            DisplayString = displayString;
            TextAlignment = align;
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            // TODO: FIX ME!!! Doing this just to test text rendering for now.
            SpriteFont font = FontCache.Instance[Font];

            aBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            for (int i = 0; i < _displayStrings.Length; i++)
            {
                Vector2 position = aCamera.WorldToScreen(_displayStrings[i]._position);
                aBatch.DrawString(font, _displayStrings[i]._string, position, this.Color);
            }
            aBatch.End();
        }

        private void CalculatePosition()
        {
            if (_displayStrings == null)
                return; // No text to display, no need to calculate positions!

            Vector2 largest = Vector2.Zero;
	        for(int i = 0; i < _displayStrings.Length; ++i)
	        {
		        _displayStrings[i]._extents = FontCache.Instance.GetTextExtents(_displayStrings[i]._string, Font);
		        if (_displayStrings[i]._extents.X > largest.X)
		        {
			        largest.X = _displayStrings[i]._extents.X;
		        }
		        if (_displayStrings[i]._extents.Y > largest.Y)
		        {
			        largest.Y = _displayStrings[i]._extents.Y;
		        }
	        }

            Camera worldCamera = World.Instance.Camera;
            Vector2 actorPosition = Position;
            Vector2 screenActorPosition = worldCamera.WorldToScreen(actorPosition);
            float currentScreenY = screenActorPosition.Y;
            for (int i = 0; i < _displayStrings.Length; ++i)
            {
                // Compensate for difference in XNA vs. Angel text rendering.  Angel
                // rendered the the text at its center Y, XNA rendered top left corner.
                // Subtract half of the Y extents to correct
                float renderY = currentScreenY - _displayStrings[i]._extents.Y * 0.5f;
		        switch(TextAlignment)
		        {
                    case Alignment.Left:
                        _displayStrings[i]._position = worldCamera.ScreenToWorld((int)screenActorPosition.X, (int)currentScreenY);
			            break;
                    case Alignment.Center:
                        _displayStrings[i]._position = worldCamera.ScreenToWorld((int)(screenActorPosition.X - _displayStrings[i]._extents.X * 0.5f), (int)renderY);
			            break;
                    case Alignment.Right:
                        _displayStrings[i]._position = worldCamera.ScreenToWorld((int)(screenActorPosition.X - _displayStrings[i]._extents.X), (int)renderY);
			            break;
		        }

                currentScreenY += largest.Y + LineSpacing;
	        }

            Size = worldCamera.ScreenSizeToWorldSize(new Vector2(largest.X, (largest.Y + LineSpacing) * _displayStrings.Length));
        }
    }
}
