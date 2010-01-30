using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.AI.Pathing
{
    internal class StartMoveState : PathFinder.FindNextMoveState
    {
        public override string Name
        {
            get { return "Start"; }
        }

        public override bool Update(ref PathFinderMove move)
        {
            if (!SpatialGraphManager.Instance.IsInPathableSpace(CurrentPosition))
		    {
			    SetNewState(PathFinder.MoveState.StartRecover);
			    return true;
		    }
		    //find path
		    List<Vector2> path = CurrentPath;
            path.Clear();
            bool retVal = SpatialGraphManager.Instance.GetPath(CurrentPosition, CurrentDestination, ref path);

		    if( retVal )
		    {
			    move.LastResult = PathFinder.MoveResult.PathFound;
			    CurrentPathIndex = 0;

			    SetNewState( PathFinder.MoveState.Follow );
			    //Keep ticking
			    return true;
		    }
		    else
		    {
                move.LastResult = PathFinder.MoveResult.PathNotFound;
		    }

		    //We're done if path failed
		    return false;
        }
    }
}
