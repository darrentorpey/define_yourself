using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngelXNA.Infrastructure
{
    public abstract class Renderable
    {
        protected bool _deleteMe = false;
	    protected int _layer;

        public bool Destroyed
        {
            get { return _deleteMe; }
        }

        public int Layer
        {
            get { return _layer; }
            internal set { _layer = value; }
        }

	    public void Destroy() 
	    {
		    if( Destroyed )
			    return;
		    PreDestroy(); 
		    _deleteMe = true;
	    }

        public abstract void Update(GameTime aTime);
        public abstract void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch);

        public virtual void AddedToWorld() { }
        public virtual void RemovedFromWorld() { }
        protected virtual void PreDestroy() { }
    }
}
