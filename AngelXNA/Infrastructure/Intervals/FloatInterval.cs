using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.Infrastructure.Intervals
{
    class FloatInterval : Interval<float>
    {
        public FloatInterval(float start, float end, float duration, bool smooth)
            : base(start, end, duration, smooth) {}

        public override float Step(float dt)
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
                    _current = MathHelper.SmoothStep(_start, _end, stepRatio);
                }
                else
                {
                    // Simple LERP
                    _current = MathHelper.Lerp(_start, _end, stepRatio);
                }
            }
            return _current;
        }
    }
}
