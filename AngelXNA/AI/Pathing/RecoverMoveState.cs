using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI.Pathing
{
    internal class RecoverMoveState : PathFinder.FindNextMoveState
    {
        public override string Name
        {
            get { return "Recover"; }
        }

        public override bool Update(ref PathFinderMove move)
        {
            //are we back in pathable space?
		    if( SpatialGraphManager.Instance.IsInPathableSpace( CurrentPosition ) )
		    {
			    SetNewState( PathFinder.MoveState.Follow );
			    return true;
		    }

		    List<Vector2> currentPath = CurrentPath;
		    int currentPathIndex = CurrentPathIndex;

		    //otherwise, head toward our current pathnode
		    move.MoveDir = Vector2.Normalize( currentPath[currentPathIndex] - CurrentPosition );

		    return false;
        }
    }
}
