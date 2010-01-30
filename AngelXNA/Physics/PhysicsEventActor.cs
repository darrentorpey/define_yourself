using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Actors;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Collisions;
using Box2DX.Dynamics;

namespace AngelXNA.Physics
{
    public class PhysicsEventActor : PhysicsActor
    {
        private string _collisionId;
        private List<PhysicsEventActor> _collisions = new List<PhysicsEventActor>();
        private Dictionary<string, List<ICollisionResponse>> _collisionResponseTable = new Dictionary<string, List<ICollisionResponse>>();

        [ConsoleProperty]
        public string CollisionId
        {
            set { _collisionId = value.ToUpper(); }
        }

        public PhysicsEventActor()
        {

        }

        public override void Update(GameTime aTime)
        {
            ProcessCollisions();
            base.Update(aTime);
        }

        [ConsoleMethod]
        public virtual void RegisterCollisionResponse(string key, ICollisionResponse colResponse)
        {
            key = key.ToUpper();
            if(!_collisionResponseTable.ContainsKey(key))
                _collisionResponseTable.Add(key, new List<ICollisionResponse>());
            

            _collisionResponseTable[key].Add(colResponse);
        }

        public virtual void RemoveCollisionResponse(string key, ICollisionResponse colResponse)
        {
            if(_collisionResponseTable.ContainsKey(key))
            {
                _collisionResponseTable[key].Remove(colResponse);
                if(_collisionResponseTable[key].Count == 0)
                    _collisionResponseTable.Remove(key);
            }
        }

        public override string ToString()
        {
            return String.Format("PhysicsEventActor<{0}>", Name);
        }

        public virtual void OnNamedEvent(string eventId) { }

        internal override void OnCollision(PhysicsActor otherActor, ref ContactPoint point)
        {
            if (otherActor is PhysicsEventActor)
            {
                AddCollision((PhysicsEventActor)otherActor);
            }

            base.OnCollision(otherActor, ref point);
        }

        protected void AddCollision(PhysicsEventActor otherActor)
        {
            _collisions.Add(otherActor);
        }

        protected virtual void ProcessCollisions()
        {
            ProcessCollisionsInternal();
            _collisions.Clear();
        }

        protected virtual void ProcessCollisionsInternal()
        {
            for (int i = 0; i < _collisions.Count; ++i)
            {
                PhysicsEventActor striker = _collisions[i];
                if (striker._collisionId != null &&
                    _collisionResponseTable.ContainsKey(striker._collisionId))
                {
                    foreach (ICollisionResponse response in _collisionResponseTable[striker._collisionId])
                    {
                        response.Execute(this, striker);
                    }
                }
            }
        }

        [ConsoleMethod]
        public static new PhysicsEventActor Create()
        {
            return new PhysicsEventActor();
        }
    }
}
