using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2DX.Dynamics;
using AngelXNA.Physics;
using AngelXNA.Actors;
using AngelXNA.Messaging;

namespace AngelXNA.Infrastructure
{
    /// <summary>
    /// This class is for internal use by the world to manage contacts
    /// occuring in the physics system and pushing info around for said 
    /// collisions
    /// </summary>
    internal class CollisionManager : ContactListener
    {
        private struct CollisionPair
        {
            public Actor a1;
            public Actor a2;

            public CollisionPair(Actor a1, Actor a2)
            {
                this.a1 = a1;
                this.a2 = a2;
            }
        }

        private int _iMaxContatPoints = 2048;
        private int _iContactPointCount = 0;

        private Dictionary<CollisionPair, bool> _currentTouches = new Dictionary<CollisionPair, bool>();

        public int MaxContactPoints
        {
            get { return _iMaxContatPoints; }
            set {  _iMaxContatPoints = value; }
        }

        public void Clear()
        {
            _iContactPointCount = 0;
            _currentTouches.Clear();
        }

        public override void Add(ref ContactPoint point)
        {
            BufferContactPoint(ref point);
        }

        public override void Persist(ref ContactPoint point)
        {
            BufferContactPoint(ref point);
        }

        protected void BufferContactPoint(ref ContactPoint point)
        {
            if (_iContactPointCount == _iMaxContatPoints)
                return;

            // Angel buffers contact points, I don't know why.  We don't.
            PhysicsActor pa1 = point.Shape1.Body.GetUserData() as PhysicsActor;
            PhysicsActor pa2 = point.Shape2.Body.GetUserData() as PhysicsActor;
            if (pa1 == null || pa2 == null)
                return;

            CollisionPair pair = new CollisionPair(pa1, pa2);            
            if (!_currentTouches.ContainsKey(pair))
            {
                pa1.OnCollision(pa2, ref point);
            
                if (Switchboard.Instance["CollisionWith" + pa1.Name] != null)
                {
                    Switchboard.Instance.Broadcast(new Message("CollisionWith" + pa1.Name));
                    _currentTouches.Add(pair, true);
                }
            }

            pair = new CollisionPair(pa2, pa1);
            if (!_currentTouches.ContainsKey(pair))
            {
                pa2.OnCollision(pa1, ref point);
            
                if (Switchboard.Instance["CollisionWith" + pa2.Name] != null)
                {
                    Switchboard.Instance.Broadcast(new Message("CollisionWith" + pa2.Name));
                    _currentTouches.Add(pair, true);
                }
            }

            _iContactPointCount++;
        }
    }
}
