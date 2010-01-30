using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Infrastructure
{
    public class ValueCache<Type>
    {
        public delegate void UpdateHandler(ref Type value);

        private UpdateHandler _del;
        private bool _dirty = false;
        private Type _cachedValue;

        public Type Value
        {
            get
            {
                if (_dirty)
                {
                    _del(ref _cachedValue);
                    _dirty = false;
                }
                return _cachedValue;
            }
        }

        public ValueCache(UpdateHandler del)
        {
            _del = del;
        }

        public void Dirty()
        {
            _dirty = true;
        }
    }
}
