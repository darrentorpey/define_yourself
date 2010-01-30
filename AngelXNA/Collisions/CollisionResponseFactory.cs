using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Collisions
{
    public class CollisionResponseFactory
    {
        public delegate ICollisionResponse CreateCollisionResponseHandler(string[] input);

        private static CollisionResponseFactory s_Instance = new CollisionResponseFactory();

        public static CollisionResponseFactory Instance
        {
            get { return s_Instance; }
        }

        public CollisionResponseFactory()
        {
            AddDelegate("PlaySoundCollisionResponse", PlaySoundCollisionResponse.FactoryMethod);
        }

        private Dictionary<string, CreateCollisionResponseHandler> _table = new Dictionary<string, CreateCollisionResponseHandler>();

        public void AddDelegate(string id, CreateCollisionResponseHandler factoryFunc)
        {
            if (_table.ContainsKey(id))
                _table[id] = factoryFunc;
            else
                _table.Add(id, factoryFunc);
        }

        public ICollisionResponse CreateCollisionResponse(string id, string[] input)
        {
            ICollisionResponse retVal = null;
            if(_table.ContainsKey(id))
            {
                retVal = _table[id](input);
            }

            return retVal;
        }
    }
}
