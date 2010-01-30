using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Util;
using Microsoft.Xna.Framework.Graphics;

namespace AngelXNA.Infrastructure
{
    public struct BoundingBox2D
    {
        private static Texture2D s_defaultTexture;

        public enum ContainmentType
        {
            Disjoint,
            Within,
            Intersects
        }

        public Vector2 Min;
        public Vector2 Max;

        public BoundingBox2D(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public Vector2 Centroid()
        {
            return Min + (Max - Min) / 2.0f;
        }

        public void GetCorners(out Vector2[] corners)
        {
            corners = new Vector2[4] {
                new Vector2(Min.X, Min.Y),
                new Vector2(Min.X, Max.Y),
                new Vector2(Max.X, Max.Y),
                new Vector2(Max.X, Min.Y)
            };
        }

        public bool Intersetcts(ref BoundingBox box)
        {
            if ((Max.X < box.Min.X) || (Min.X > box.Max.X))
            {
                return false;
            }
            if ((Max.Y < box.Min.Y) || (Min.Y > box.Max.Y))
            {
                return false;
            }

            return true;
        }

        public bool Intersects(ref Ray2D ray, out float distanceAlongRay)
        {
	        distanceAlongRay = 0.0f;
	        float maxValue = float.MaxValue;
	        if (Math.Abs(ray.Direction.X) < MathUtil.Epsilon)
	        {
		        if ((ray.Position.X < Min.X) || (ray.Position.X > Max.X))
		        {
			        return false;
		        }
	        }
	        else
	        {
		        float invMag = 1.0f / ray.Direction.X;
		        float minProj = (Min.X - ray.Position.X) * invMag;
		        float maxProj = (Max.X - ray.Position.X) * invMag;
		        if (minProj > maxProj)
		        {
			        float temp = minProj;
			        minProj = maxProj;
			        maxProj = temp;
		        }
		        distanceAlongRay = Math.Max(minProj, distanceAlongRay);
		        maxValue = Math.Min(maxProj, maxValue);
		        if (distanceAlongRay > maxValue)
		        {
			        return false;
		        }
	        }

	        if (Math.Abs(ray.Direction.Y) < MathUtil.Epsilon)
	        {
		        if ((ray.Position.Y < Min.Y) || (ray.Position.Y > Max.Y))
		        {
			        return false;
		        }
	        }
	        else
	        {
		        float invMag = 1.0f / ray.Direction.Y;
		        float minProj = (Min.Y - ray.Position.Y) * invMag;
		        float maxProj = (Max.Y - ray.Position.Y) * invMag;
		        if (minProj > maxProj)
		        {
			        float temp = minProj;
			        minProj = maxProj;
			        maxProj = temp;
		        }
		        distanceAlongRay = Math.Max(minProj, distanceAlongRay);
		        maxValue = Math.Min(maxProj, maxValue);
		        if (distanceAlongRay > maxValue)
		        {
			        return false;
		        }
	        }

	        return true;
        }

        public ContainmentType Contains(ref BoundingBox2D box)
        {
            if ((Max.X < box.Min.X) || (Min.X > box.Max.X))
	        {
		        return ContainmentType.Disjoint;
	        }
	        if ((Max.Y < box.Min.Y) || (Min.Y > box.Max.Y))
	        {
		        return ContainmentType.Disjoint;
	        }
	        if ((((Min.X <= box.Min.X) && (box.Max.X <= Max.X)) && ((Min.Y <= box.Min.Y) && (box.Max.Y <= Max.Y))))
	        {
		        return ContainmentType.Within;
	        }
            return ContainmentType.Intersects;
        }

        public bool Contains(ref Vector2 point)
        {
            if ((((Min.X <= point.X) && (point.X <= Max.X)) && ((Min.Y <= point.Y) && (point.Y <= Max.Y))))
            {
                return true;
            }
            return false;
        }

        public void Render(Color aColor, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            if (s_defaultTexture == null)
                s_defaultTexture = World.Instance.Game.Content.Load<Texture2D>("white");

            Vector2 screenPos = aCamera.WorldToScreen(Centroid());
            Vector2 screenSize = aCamera.WorldSizeToScreenSize(Max - Min);
            Rectangle destRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)screenSize.X, (int)screenSize.Y);

            aBatch.Begin();
            aBatch.Draw(s_defaultTexture, destRect, null, aColor, 0.0f,
               new Vector2(s_defaultTexture.Width / 2, s_defaultTexture.Height / 2), 
               SpriteEffects.None, 0.0f);
            aBatch.End();
        }

        public static BoundingBox2D CreateMerged(ref BoundingBox2D original, ref BoundingBox2D additional)
        {
            return new BoundingBox2D(Vector2.Min(original.Min, additional.Min), Vector2.Max(original.Max, additional.Max));
        }

        public static BoundingBox2D CreateFromPoints(ref Vector2[] points)
        {
            if( points.Length < 1 )
		        return new BoundingBox2D(Vector2.Zero, Vector2.Zero);

	        BoundingBox2D retVal = new BoundingBox2D( new Vector2(float.MaxValue, float.MaxValue), 
                new Vector2(float.MinValue, float.MinValue));
	        for( int i = 0 ; i < points.Length; ++i )
	        {
		        retVal.Min = Vector2.Min(retVal.Min, points[i]);
                retVal.Max = Vector2.Max(retVal.Max, points[i]);
	        }
	        return retVal;
        }
    }
}
