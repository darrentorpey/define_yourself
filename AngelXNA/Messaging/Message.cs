using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Messaging
{
    public struct Message
    {
        private string _messageName;
        // TODO: Angel used MessageListener, should we?
        private object _sender;

        public string MessageName
        {
            get { return _messageName; }
        }

        public object Sender
        {
            get { return _sender; }
        }

        public Message(string messageName)
            : this(messageName, null)
        {

        }

        public Message(string messageName, object sender)
        {
            _messageName = messageName;
            _sender = sender;
        }
    }
}
