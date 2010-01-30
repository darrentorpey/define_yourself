using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI
{
    public class AIBrainState
    {
        protected AIBrain _brain;
        protected List<AIEvent> _eventList;

        protected Sentient Actor
        {
            get { return _brain.Actor; }
        }

        public virtual void Initialize(AIBrain aBrain)
        {
            _brain = aBrain;
        }

        public void Update(GameTime aTime)
        {
            CustomUpdate(aTime);
            for (int i = 0; i < _eventList.Count; ++i)
            {
                _eventList[i].Update(aTime);
            }
        }

        public void EndState(AIBrainState aNextState)
        {
            CustomEndState(aNextState);
            ClearEvents();
        }

        public virtual void CustomUpdate(GameTime aTime) { }
        public virtual void CustomEndState(AIBrainState aNextState) { }
        public virtual void BeginState(AIBrainState aPreviousState) { }

        protected virtual void GotoState(string asId)
        {
            _brain.GotoState(asId);
        }

        protected virtual AIEvent RegisterEvent(AIEvent aEvent)
        {
            _eventList.Add(aEvent);
            aEvent.Brain = _brain;
            return aEvent;
        }

        protected virtual void UnregsterEvent(AIEvent aEvent)
        {
            for (int i = 0; i < _eventList.Count; ++i)
            {
                if (_eventList[i] == aEvent)
                {
                    _eventList.RemoveAt(i);
                    aEvent.Stop();
                }
            }
        }

        private void ClearEvents()
        {
            for (int i = 0; i < _eventList.Count; ++i)
            {
                _eventList[i].Stop();
            }
            _eventList.Clear();
        }
    }
}
