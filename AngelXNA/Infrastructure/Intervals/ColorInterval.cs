using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure.Logging;

namespace AngelXNA.Infrastructure.Intervals
{
    class ColorInterval : Interval<Color>
    {
        public ColorInterval(Color start, Color end, float duration, bool smooth)
            : base(start, end, duration, smooth) {}

        public override Color Step(float dt)
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
                    _current = new Color(MathHelper.SmoothStep(((float)_start.R) / 255.0f, ((float)_end.R) / 255.0f, stepRatio), MathHelper.SmoothStep(((float)_start.G) / 255.0f, ((float)_end.G) / 255.0f, stepRatio), MathHelper.SmoothStep(((float)_start.B) / 255.0f, ((float)_end.B) / 255.0f, stepRatio), MathHelper.SmoothStep(((float)_start.A) / 255.0f, ((float)_end.A) / 255.0f, stepRatio));
                }
                else
                {
                    // Simple LERP
                    _current = Color.Lerp(_start, _end, stepRatio);
                }
            }
            return _current;
        }
    }
}
