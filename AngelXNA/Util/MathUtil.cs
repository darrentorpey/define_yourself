using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework;

namespace AngelXNA.Util
{
    public class MathUtil
    {
        public enum AABBSplittingAxis
        {
            X,
            Y
        }

        public const float Epsilon = 0.000001f;

        public static Random s_MyRandom = new Random();

        public static int Clamp(int aiValue, int aiMin, int aiMax)
        {
            return Math.Max(aiMin, Math.Min(aiMax, aiValue));
        }

        public static void SplitBoundingBox(ref BoundingBox2D source, AABBSplittingAxis axis, out BoundingBox2D LHS, out BoundingBox2D RHS)
        {
            LHS = source;
            RHS = source;

            switch (axis)
            {
                case AABBSplittingAxis.X:
                    LHS.Max.X = MathHelper.Lerp(LHS.Min.X, LHS.Max.X, 0.5f);
                    RHS.Min.X = LHS.Max.X;
                    break;
                case AABBSplittingAxis.Y:
                    LHS.Max.Y = MathHelper.Lerp(LHS.Min.Y, LHS.Max.Y, 0.5f);
                    RHS.Min.Y = LHS.Max.Y;
                    break;
            }
        }

        public static float RandomFloat(float maximum)
        {
            const float bigNumber = 10000.0f;
            float randFloat = (float)s_MyRandom.Next((int)bigNumber);
            randFloat = randFloat / bigNumber;
            return randFloat * maximum;
        }

        public static float RandomFloatInRange(float max, float min)
        {
            return RandomFloat(max - min) + min;
        }

        public static float RandomFloatWithError(float target, float error)
        {
            return RandomFloatInRange(target - error, target + error);

        }
    }
}
