using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI.Pathing
{
    public struct PathFinderMove
    {
        public Vector2 MoveDir;
        public Vector2 NextSubgoalPos;
        public PathFinder.MoveResult LastResult;
    }

    public class PathFinder
    {
        public enum MoveResult
        {
            PathNotFound,
            PathFound,
            Arrived,
        };

        public enum MoveState
        {
            Start,
            Validate,
            Follow,
            Recover,
            StartRecover,

            Count
        };

        public abstract class FindNextMoveState
        {
            protected PathFinder _pathFinder;

            public abstract string Name { get; }

            protected List<Vector2> CurrentPath { get { return _pathFinder._currentPath; } }
            protected Vector2 CurrentPosition { get { return _pathFinder._currentPos; } }
            protected Vector2 CurrentDestination { get { return _pathFinder._currentDest; } }
            protected int CurrentPathIndex 
            { 
                get { return _pathFinder._currentPathIndex; }
                set { _pathFinder._currentPathIndex = value; }
            }
            public float CurrentArrivalDist { get { return _pathFinder._arrivalDist; } }
            
            public virtual void Initialize(PathFinder pathFinder)
            {
                _pathFinder = pathFinder;
            }

            public abstract bool Update(ref PathFinderMove move);
            public virtual void BeginState(PathFinder.MoveState lastState) { }
            public virtual void EndState(PathFinder.MoveState nextState) { }

            protected void SetNewState(PathFinder.MoveState newState)
            {
                _pathFinder.SetNewState(newState);
            }
        }

        private List<Vector2> _currentPath = new List<Vector2>();
        private int _currentPathIndex = -1;
        private MoveState _currentState = MoveState.Start;
        private Vector2 _currentPos;
        private Vector2 _currentDest;
        private float _arrivalDist = 0.0f;

        private FindNextMoveState[] _states = new FindNextMoveState[(int)MoveState.Count];

        public PathFinder()
        {
            InitializeStates();
        }

        public void FindNextMove(ref Vector2 from, ref Vector2 to, float arrivalDist, ref PathFinderMove move)
        {
            _currentPos = from;
            _currentDest = to;
            _arrivalDist = arrivalDist;

            move.LastResult = MoveResult.PathFound;
            while (true)
            {
                if (!GetCurrentState().Update(ref move))
                    break;
            }
        }

        public void Render()
        {
            // TODO: PathFinder render
        }

        private void InitializeStates()
        {
            
        }

        private FindNextMoveState GetCurrentState()
        {
            return _states[(int)_currentState];
        }

        private void SetNewState(MoveState newState)
        {
            if (newState == _currentState)
                return;

            if ((int)newState < 0 || (int)newState >= (int)MoveState.Count)
                return;

            _currentState = newState;
        }
    }
}
