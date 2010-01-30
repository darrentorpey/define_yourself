using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI
{
    public delegate void AIEventHandler( AIEvent aFromEvent );

    public class AIEvent
    {
        private AIBrain _brain;

        public event AIEventHandler Triggered;

        public AIBrain Brain 
        { 
            protected get { return _brain; }
            set { _brain = value; }
        }

        protected Sentient Actor
        {
            get { return _brain.Actor; }
        }

        public virtual void Stop() { }
        public virtual void Update(GameTime aTime) { }

        protected virtual void IssueCallback()
        {
            if (Triggered != null)
                Triggered(this);
        }
    }
}
