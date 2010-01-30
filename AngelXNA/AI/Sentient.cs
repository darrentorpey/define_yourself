using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Physics;
using Microsoft.Xna.Framework;
using AngelXNA.AI.Pathing;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure;

namespace AngelXNA.AI
{
    public class Sentient : PhysicsEventActor
    {
        private PathFinder _pathFinder = new PathFinder();
        private AIBrain _brain = new AIBrain();

        public PathFinder PathFinder
        {
            get { return _pathFinder; }
        }

        public Sentient()
        {
            _brain.Actor = this;
        }

        public override void Update(GameTime time)
        {
            _brain.Update(time);
            base.Update(time);
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            base.Render(aTime, aCamera, aDevice, aBatch);
            _pathFinder.Render();
            _brain.Render();
        }

	    public override void OnNamedEvent( string asEventId ) {}
	    public virtual void InitializeBrain() {}
	    public virtual void StartBrain() {}
    }
}
