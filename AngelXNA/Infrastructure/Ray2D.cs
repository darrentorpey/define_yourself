using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.Infrastructure
{
    public struct Ray2D
    {
        public Vector2 Position;
        public Vector2 Direction;

        public Ray2D(Vector2 aPosition, Vector2 aDirection)
        {
            Position = aPosition;
            Direction = aDirection;
        }

        public static Ray2D CreateRayFromTo(ref Vector2 vFrom, ref Vector2 vTo)
        {
            return new Ray2D(vFrom, Vector2.Normalize(vTo - vFrom));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Ray2D))
                return false;

            return this == (Ray2D)obj;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Direction.GetHashCode();
        }

        public static bool operator ==(Ray2D LHS, Ray2D RHS)
        {
            return LHS.Direction == RHS.Direction &&
                LHS.Position == RHS.Direction;
        }

        public static bool operator !=(Ray2D LHS, Ray2D RHS)
        {
            return LHS.Direction != RHS.Direction ||
                LHS.Position != RHS.Position;
        }
    }
}
