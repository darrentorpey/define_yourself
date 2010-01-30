using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.Infrastructure.Intervals
{
    class Vector2Interval : Interval<Vector2>
    {
        public Vector2Interval(Vector2 start, Vector2 end, float duration, bool smooth)
            : base(start, end, duration, smooth) {}

        public override Vector2 Step(float dt)
        {
            if (!_shouldStep)
            {
                return _current;
            }

            _timer += dt;

            if (_timer >= _duration)
            {
                _current = _end;
                _shouldStep = false;
            }
            else
            {
                float stepRatio = _timer / _duration;
                if (_smoothStep)
                {
                    // Smooth step
                    _current = new Vector2(MathHelper.SmoothStep(_start.X, _end.X, stepRatio), MathHelper.SmoothStep(_start.Y, _end.Y, stepRatio));
                }
                else
                {
                    // Simple LERP
                    _current = new Vector2(MathHelper.Lerp(_start.X, _end.X, stepRatio), MathHelper.Lerp(_start.Y, _end.Y, stepRatio));
                }
            }
            return _current;
        }
    }
}
