using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI.Pathing
{
    internal class FollowMoveState : PathFinder.FindNextMoveState
    {
        public override string Name
        {
            get { return "Follow"; }
        }

        public override bool Update(ref PathFinderMove move)
        {
            //check our current path index
		    const int iLookAheadCount = 3;

		    List<Vector2> currentPath = CurrentPath;
		    int nextPathIndex = CurrentPathIndex;

		    //Are we at our goal index
		    if( nextPathIndex == currentPath.Count - 1 )
		    {
			    move.NextSubgoalPos = currentPath[nextPathIndex];
			    //check distance to goal
			    float sqDist = Vector2.DistanceSquared( CurrentPosition, move.NextSubgoalPos );
			    float arrivalDistSq = CurrentArrivalDist;
			    arrivalDistSq *= arrivalDistSq;
			    if( sqDist <= arrivalDistSq )
			    {
				    //don't set move dir (we've arrived)
				    move.LastResult = PathFinder.MoveResult.Arrived;
				    SetNewState( PathFinder.MoveState.Validate );
				    return false;
			    }
		    }
		    else
		    {
			    //otherwise, see if we can advance our next subgoal
			    for( int i = 0 ; i < iLookAheadCount && (nextPathIndex+1) < currentPath.Count; i++ )
			    {
                    if (SpatialGraphManager.Instance.CanGo(CurrentPosition, currentPath[nextPathIndex + 1]))
				    {
					    ++nextPathIndex;
				    }
			    }

			    CurrentPathIndex = nextPathIndex;
			    move.NextSubgoalPos = currentPath[nextPathIndex];
		    }

		    //Move dir is normalized towards next subgoal
		    move.MoveDir = Vector2.Normalize( move.NextSubgoalPos - CurrentPosition );
		    //we're done this round
		    SetNewState( PathFinder.MoveState.Validate );
		    return false;
        }
    }
}
