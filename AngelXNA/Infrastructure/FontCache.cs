using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AngelXNA.Infrastructure
{
    public class FontCache
    {
        private static FontCache s_Instance = new FontCache();

        private Dictionary<String, SpriteFont> _theCache = new Dictionary<string, SpriteFont>();

        public static FontCache Instance
        {
            get { return s_Instance; }
        }

        protected FontCache()
        {
            RegisterFont("Fonts/Inconsolata24", "Default");
        }

        public string[] GetRegisteredFonts()
        {
            return _theCache.Keys.ToArray();
        }

        public void RegisterFont(string filename, string nickname)
        {
            if (_theCache.ContainsKey(nickname))
                UnregisterFont(nickname);

            SpriteFont font = World.Instance.Game.Content.Load<SpriteFont>(filename);

            _theCache.Add(nickname, font);
        }

        public bool UnregisterFont(string nickname)
        {
            if(!_theCache.ContainsKey(nickname))
                return false;

            _theCache.Remove(nickname);
            return true;
        }

        public SpriteFont this[string key]
        {
            get { return _theCache[key]; }
        }

        public Vector2 GetTextExtents(string text, string nickname)
        {
            if(!_theCache.ContainsKey(nickname))
                return Vector2.Zero;

            SpriteFont font = _theCache[nickname];
            return font.MeasureString(text);
        }
    }
}
