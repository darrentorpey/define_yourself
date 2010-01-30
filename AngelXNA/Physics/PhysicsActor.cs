using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Actors;
using AngelXNA.Infrastructure;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Messaging;

using Box2DX.Dynamics;

using Angel = AngelXNA.Infrastructure;
using Box2D = Box2DX.Dynamics;
using Box2DX.Collision;
using AngelXNA.Util;


namespace AngelXNA.Physics
{
    public delegate void CollisionHandler(PhysicsActor actorA, PhysicsActor actorB, ref ContactPoint contactPoint);

    public class PhysicsActor : Actor
    {
        public enum ShapeType
        {
            Box,
            Circle
        }

        protected float _fDensity = 1.0f;
        protected float _fFriction = 0.3f;
        protected float _fRestitution = 0.0f;
        protected bool _bIsSensor = false;
        protected int _iGroupIndex = 0;
        protected int _iCollisionFlags = -1;
        protected bool _bFixedRotation = false;

        protected Body _physBody;

        public Body Body
        {
            get { return _physBody; }
        }

        [ConsoleProperty]
        public override Vector2 Size
        {
            get { return base.Size; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: SetSize() had no effect - don't change this actor after physics have been initialized.");
                    return;
                }

                base.Size = value;
            }
        }

        [ConsoleProperty]
        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: SetPosition() had no effect - don't change this actor after physics have been initialized.");
                    return;
                }

                base.Position = value;
            }
        }

        [ConsoleProperty]
        public override float Rotation
        {
            get { return base.Rotation; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: SetRotation() had no effect - don't change this actor after physics have been initialized.");
                    return;
                }

                base.Rotation = value;
            }
        }

        [ConsoleProperty]
        public float Density
        {
            get { return _fDensity; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: SetDensity had no effect - don't change this actor after physics has been initialized.");
                    return;
                }

                _fDensity = value;
            }
        }

        [ConsoleProperty]
        public float Friction
        {
            get { return _fFriction; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: SetFriction had no effect - don't change this actor after physics has been initialized.");
                    return;
                }

                _fFriction = value;
            }
        }

        [ConsoleProperty]
        public float Restitution
        {
            get { return _fRestitution; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: set_Restitution had no effect - don't change this actor after physics has been initialized.");
                    return;
                }

                _fRestitution = value;
            }
        }

        [ConsoleProperty]
        public bool IsSensor
        {
            get { return _bIsSensor; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: set_IsSensor had no effect - don't change this actor after physics has been initialized.");
                    return;
                }

                _bIsSensor = value;
            }
        }

        [ConsoleProperty]
        public int GroupIndex
        {
            get { return _iGroupIndex; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: set_GroupIndex had no effect - don't change this actor after physics has been initialized.");
                    return;
                }

                _iGroupIndex = value;
            }
        }

        [ConsoleProperty]
        public int CollisionFlags
        {
            get { return _iCollisionFlags; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: set_CollisionFlags had no effect - don't change this actor after physics has been initialized.");
                    return;
                }

                _iCollisionFlags = value;
            }
        }

        [ConsoleProperty]
        public bool FixedRotation
        {
            get { return _bFixedRotation; }
            set
            {
                if (_physBody != null)
                {
                    Log.Instance.Log("WARNING: set_FixedRotation had no effect - don't change this actor after physics has been initialized.");
                    return;
                }

                _bFixedRotation = value;
            }
        }

        [ConsoleProperty]
        public bool AutoInitPhysics { get; set; }

        public event CollisionHandler Collision;

        public PhysicsActor()
            : base()
        {
            AutoInitPhysics = true;
        }

        public override void AddedToWorld()
        {
            base.AddedToWorld();

            if (AutoInitPhysics)
                InitPhysics();
        }

        public virtual void InitPhysics()
        {
            if (_physBody != null)
            {
                Log.Instance.Log("WARNING: Call to InitPhysics had no effect.  Actor has already had physics initialized.");
                return;
            }

            ShapeType shapeType = ShapeType.Box;
            Box2D.World physicsWorld = Angel.World.Instance.PhysicsWorld;

            ShapeDef shape;
            switch (shapeType)
            {
                case ShapeType.Box:
                    PolygonDef box = new PolygonDef();
                    box.SetAsBox(0.5f * Size.X, 0.5f * Size.Y);
                    shape = box;
                    break;
                case ShapeType.Circle:
                    CircleDef circle = new CircleDef();
                    circle.Radius = 0.5f * Size.X;
                    shape = circle;
                    break;
                default:
                    Log.Instance.Log("InitPhysics(): Invalid shape type given");
                    return;
            }

            shape.Density = _fDensity;
            shape.Friction = _fFriction;
            shape.Restitution = _fRestitution;
            shape.IsSensor = _bIsSensor;
            if (_iCollisionFlags != -1)
            {
                //shape->maskBits = (short)collisionFlags;
                //shape->categoryBits = (short)collisionFlags;
            }

            // InitShape( shape );

            BodyDef bodyDef = new BodyDef();
            bodyDef.UserData = this;
            bodyDef.Position = this.Position;
            bodyDef.Angle = MathHelper.ToRadians(-Rotation);
            bodyDef.FixedRotation = _bFixedRotation;


            _physBody = physicsWorld.CreateBody(bodyDef);
            _physBody.CreateShape(shape);
            _physBody.SetMassFromShapes();

            CustomInitPhysics();
        }

        public void InitPhysics(float density, float friction, float restitution)
        {
            if (_physBody != null)
            {
                Log.Instance.Log("WARNING: Call to InitPhysics had no effect.  Actor has already had physics initialized.");
                return;
            }

            _fDensity = density;
            _fFriction = friction;
            _fRestitution = restitution;
            InitPhysics();
        }

        public void InitPhysics(float density, float friction, float restitution, ShapeType shapeType)
        {
            if (_physBody != null)
            {
                Log.Instance.Log("WARNING: Call to InitPhysics had no effect.  Actor has already had physics initialized.");
                return;
            }

            _fDensity = density;
            _fFriction = friction;
            _fRestitution = restitution;
            InitPhysics();
        }

        public void InitPhysics(float density, float friction, float restitution,
            ShapeType shapeType, bool isSensor, int groupIndex, int collisionFlags, bool fixedRotation)
        {
            if (_physBody != null)
            {
                Log.Instance.Log("WARNING: Call to InitPhysics had no effect.  Actor has already had physics initialized.");
                return;
            }

            _fDensity = density;
            _fFriction = friction;
            _fRestitution = restitution;
            _bIsSensor = isSensor;
            _iGroupIndex = groupIndex;
            _iCollisionFlags = collisionFlags;
            _bFixedRotation = fixedRotation;
            InitPhysics();
        }

        public virtual void CustomInitPhysics() { }
        public virtual bool HandlesCollisionEvents() { return false; }

        public void DestroyPhysics()
        {
            CustonDestroyPhysics();

            if (_physBody != null)
            {
                _physBody.SetUserData(null);
                Box2D.World world = Angel.World.Instance.PhysicsWorld;
                world.DestroyBody(_physBody);

                _physBody = null;
            }
        }

        public virtual void CustonDestroyPhysics() { }

        public void ApplyForce(Vector2 force, Vector2 point)
        {
            if (_physBody != null)
                _physBody.ApplyForce(force, point + Position);
        }

        // apply a local space force on the object
        public void ApplyLocalForce(Vector2 force, Vector2 point)
        {
            if (_physBody != null)
                _physBody.ApplyForce(_physBody.GetWorldVector(force), point + Position);
        }

        public void ApplyTorque(float torque)
        {
            if (_physBody != null)
                _physBody.ApplyTorque(torque);
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 point)
        {
            if (_physBody != null)
                _physBody.ApplyImpulse(impulse, point);
        }

        public void SyncPosRot()
        {
            base.Position = _physBody.Position;
            base.Rotation = -MathHelper.ToDegrees(_physBody.Angle);
        }

        public override void RemovedFromWorld()
        {
            DestroyPhysics();

            base.RemovedFromWorld();
        }

        internal virtual void OnCollision(PhysicsActor otherActor, ref ContactPoint point)
        {
            if (Collision != null)
                Collision(this, otherActor, ref point);
        }

        [ConsoleMethod]
        public static new PhysicsActor Create()
        {
            return new PhysicsActor();
        }
    }
}
