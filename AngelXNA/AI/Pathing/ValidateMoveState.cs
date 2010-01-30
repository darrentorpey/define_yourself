using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI.Pathing
{
    internal class ValidateMoveState : PathFinder.FindNextMoveState
    {
        public override string Name
        {
            get { return "Validate"; }
        }

        public override bool Update(ref PathFinderMove move)
        {
            //Make sure we can keep heading to our current subgoal
		    List<Vector2> currentPath = CurrentPath;
		    int currentPathIndex = CurrentPathIndex;

		    //has the destination changed
		    Vector2 vDest = currentPath[currentPath.Count-1];
		    if( vDest == CurrentDestination )
		    {
			    if( SpatialGraphManager.Instance.CanGo( CurrentPosition, currentPath[currentPathIndex] ) )
			    {
				    SetNewState( PathFinder.MoveState.Follow );
				    return true;
			    }
                else if (!SpatialGraphManager.Instance.CanGo(CurrentPosition, CurrentPosition))
			    {
				    //are we blocked
				    SetNewState( PathFinder.MoveState.Recover );
				    return true;
			    }
		    }

		    //otherwise, try again
            SetNewState(PathFinder.MoveState.Start);
		    return true;
        }
    }
}
