using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Infrastructure.Console
{
    public class ConsoleItem
    {
        protected string _id;
        protected int _flags = 0;

        public string Id { get { return _id; } }
        public virtual string Description { get { return _id; } }
        public virtual bool IgnoreForAutoComplete { get { return false; } }

        public ConsoleItem(string id)
        {
            _id = id;
        }

        public void SetFlag(int flag)
        {
            _flags |= flag;
        }

        public void ClearFlag(int flag)
        {
            _flags &= ~flag;
        }

        public bool HasFlag(int flag)
        {
            return (_flags & flag) != 0;
        }
    }
}
