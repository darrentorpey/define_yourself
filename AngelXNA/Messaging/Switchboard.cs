using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Infrastructure;
using AngelXNA.Infrastructure.Logging;

namespace AngelXNA.Messaging
{
    public delegate void MessageHandler(Message message);

    public class Switchboard
    {
        #region Singleton implementation
        private static Switchboard s_Instance = new Switchboard();

        public static Switchboard Instance
        {
            get { return s_Instance; }
        }
        #endregion

        private struct MessageTimer
        {
            public Message _message;
            public float _timeRemaining;

            public MessageTimer(Message message, float timeRemaining)
            {
                _message = message;
                _timeRemaining = Math.Max(0.0f, timeRemaining);
            }

            public void Tick(float dt)
            {
                _timeRemaining -= dt;
            }
        }

        private Dictionary<string, MessageHandler> _subscriptions = new Dictionary<string,MessageHandler>();
        private Queue<Message> _messages = new Queue<Message>();
        private LinkedList<MessageTimer> _delayedMessages = new LinkedList<MessageTimer>();

        public void Broadcast(Message message)
        {
            if (World.Instance.IsSimulating)
                _messages.Enqueue(message);
            else
                Log.Instance.Log("[DEBUG] Ignoring message sent while world is not simulating: " + message.ToString());
        }

        public void DefferedBroadcast(Message message, float delay)
        {
	        _delayedMessages.AddLast(new MessageTimer(message, delay));
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (LinkedListNode<MessageTimer> node = _delayedMessages.First; node != null;)
            {
                node.Value.Tick(dt);
                if (node.Value._timeRemaining <= 0.0f)
                {
                    Broadcast(node.Value._message);
                    node = node.Next;
                    _delayedMessages.Remove(node);
                }
                else
                    node = node.Next;
            }
        }

        public void SendAllMessages()
        {
            while (_messages.Count > 0)
	        {
                Message nextMessage = _messages.Dequeue();
                if(_subscriptions.ContainsKey(nextMessage.MessageName))
                {
                    MessageHandler handler = _subscriptions[nextMessage.MessageName];
                    if(handler != null)
                        handler(nextMessage);
                }
	        }
        }

        public MessageHandler this[string asMessage]
        {
            get
            {
                if (_subscriptions.ContainsKey(asMessage))
                    return _subscriptions[asMessage];
                return null;
            }
            set
            {
                if (_subscriptions.ContainsKey(asMessage))
                {
                    if (value == null)
                    {
                        _subscriptions.Remove(asMessage);
                    }
                    else
                    {
                        _subscriptions[asMessage] = value;
                    }
                }
                else if (value != null)
                {
                    _subscriptions.Add(asMessage, value);
                }
            }
        }
    }
}
