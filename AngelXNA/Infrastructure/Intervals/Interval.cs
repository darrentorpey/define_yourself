using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AngelXNA.Util;

namespace AngelXNA.Infrastructure.Intervals
{
    public abstract class Interval<T>
    {
        protected T _start;
        protected T _end;
        protected T _current;

        protected float _duration;
        protected float _timer;

        protected bool _shouldStep;
        protected bool _smoothStep;

        public Interval(T start, T end, float duration)
            : this(start, end, duration, false)
        {
        }

        public Interval(T start, T end, float duration, bool smooth) {
            _start = start;
            _end = end;
            _current = start;
            _duration = duration;
            _timer = 0.0f;
            _shouldStep = true;
            _smoothStep = smooth;
        }

        public T Current { get { return _current; } set { _current = value; } }

        public bool ShouldStep { get { return _shouldStep; } set { _shouldStep = value; } }

        public abstract T Step(float dt);
    }
}
