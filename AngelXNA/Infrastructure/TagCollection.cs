using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;

namespace AngelXNA.Infrastructure
{
    public class TagCollection
    {
        protected static TagCollection s_Instance = new TagCollection();

        private Dictionary<string, List<Actor>> _tagMappings = new Dictionary<string,List<Actor>>();

        protected TagCollection()
        {

        }

        public static TagCollection Instance
        {
            get { return s_Instance; }
        }

        public Actor[] GetObjectsTagged(string asTag)
        {
            List<Actor> retList = null;
            _tagMappings.TryGetValue(asTag, out retList);

            if (retList != null)
                return retList.ToArray();
            
            return null;
        }

        public string[] GetTagList()
        {
            return _tagMappings.Keys.ToArray();
        }

        public void AddObjectToTagList(Actor aActor, string asTag)
        {
            if (!_tagMappings.ContainsKey(asTag))
                _tagMappings.Add(asTag, new List<Actor>());

            _tagMappings[asTag].Add(aActor);
        }

        public void RemoveObjectFromTagList(Actor aActor, string asTag)
        {
            _tagMappings[asTag].Remove(aActor);
            if (_tagMappings[asTag].Count == 0)
                _tagMappings.Remove(asTag);
        }
    }
}
