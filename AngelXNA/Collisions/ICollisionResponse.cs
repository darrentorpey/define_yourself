using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Physics;

namespace AngelXNA.Collisions
{
    public interface ICollisionResponse
    {
        void Execute(PhysicsEventActor struck, PhysicsEventActor striker);
    }
}
