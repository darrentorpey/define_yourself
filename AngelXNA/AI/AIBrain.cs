using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI
{
    public class AIBrain
    {
        protected Dictionary<string, AIBrainState> _brainStateTable = new Dictionary<string, AIBrainState>();
        protected AIBrainState _current = null;
        
        public Sentient Actor { get; set; }

        public void AddState(string asId, AIBrainState aState)
        {
            string useId = asId.ToUpper();
            aState.Initialize(this);

            if (_brainStateTable.ContainsKey(asId))
                _brainStateTable[asId] = aState;
            else
                _brainStateTable.Add(asId, aState);
        }

        public virtual void Update(GameTime aTime)
        {
            if (_brainStateTable.Count == 0)
            {
                Actor.InitializeBrain();
                Actor.StartBrain();
            }
            if (_current != null)
                _current.Update(aTime);
        }

        public virtual void GotoState(string asId)
        {
            string useId = asId.ToUpper();
            AIBrainState nextState;
            if (_brainStateTable.ContainsKey(useId))
            {
                AIBrainState lastState = null;
                nextState = _brainStateTable[useId];
                if (_current != null)
                {
                    lastState = _current;
                    _current.EndState(nextState);
                }

                _current = nextState;
                _current.BeginState(lastState);
            }
        }

        public void Render()
        {
            //CONSOLE_DECLAREVAR( ai_drawbrain );
            //if( ai_drawbrain->GetIntVal() == 0 )
            //    return;

            //if( _current != _brainStateTable.end() )
            //{
            //    Vector2 screenCenter = MathUtil::WorldToScreen( GetActor()->GetPosition().X, GetActor()->GetPosition().Y );
            //    //Print some vals
            //    glColor3f(0,0.f,1.f);
            //    DrawGameText( (*_current).first, "ConsoleSmall", (int)screenCenter.X, (int)screenCenter.Y );
            //}
        }

        public void GotoNullState()
        {
            if (_current != null)
                _current.EndState(null);
            
            _current = null;
        }
    }
}
