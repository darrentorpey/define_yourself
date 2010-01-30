using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI.Pathing
{
    internal class StartRecoverMoveState : PathFinder.FindNextMoveState
    {
        public override string Name
        {
            get { return "StartRecover"; }
        }

        public override bool Update(ref PathFinderMove move)
        {
            //are we back in pathable space?
	        if( SpatialGraphManager.Instance.IsInPathableSpace( CurrentPosition ) )
	        {
		        SetNewState( PathFinder.MoveState.Start );
		        return true;
	        }

	        //find the nearest non-blocked neighbor I can move to
	        Vector2 vGoTo;
	        if( SpatialGraphManager.Instance.FindNearestNonBlocked( CurrentPosition, out vGoTo) )
	        {
		        move.MoveDir = Vector2.Normalize( vGoTo - CurrentPosition );
	        }
	        else
	        {
		        move.LastResult = PathFinder.MoveResult.PathNotFound;
	        }

	        return false;
        }
    }
}
