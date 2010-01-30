using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngelXNA.Infrastructure
{
    public class GameManager : Renderable
    {
        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch) { }
        public override void Update(GameTime aTime) { }
        public virtual bool IsProtectedFromUnloadAll(Renderable renderable) { return false; }
    }
}
