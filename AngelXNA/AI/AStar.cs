using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace AngelXNA.AI
{
    public class AStarSearch
    {
        public enum SearchState
        {
            NotInitialised,
            Searching,
            Succeeded,
            Failed,
            OutOfMemory,
            Invalid
        }

        private class Node
        {
            public Node parent; // used during the search to record the parent of successor nodes
            public Node child; // used after the search for the application to view the search in reverse

            public float g; // cost of this node + it's predecessors
            public float h; // heuristic estimate of distance to goal
            public float f; // sum of cmulative cost of predecessors and self and heuristic

            public SpatialGraphKDNode _node;

            public float GoalDistanceEstimate(SpatialGraphKDNode goal)
            {
                return GetCost(goal);
            }

            public bool IsGoal(SpatialGraphKDNode goal)
            {
                return IsSameState(goal);
            }

            public bool IsSameState(SpatialGraphKDNode goal)
            {
                return _node == goal;
            }

            public void GetSuccessors(AStarSearch search, SpatialGraphKDNode parent)
            {
                for (int i = 0; i < _node.Neighbors.Count; ++i)
                {
                    SpatialGraphKDNode successor = _node.Neighbors[i];
                    if ((parent != null || parent != successor) && !successor.Blocked && _node.NeighborLOS[i])
                        search.AddSuccessor(successor);
                }
            }

            public float GetCost(SpatialGraphKDNode successor)
            {
                return Vector2.Distance(_node.BBox.Centroid(), successor.BBox.Centroid());
            }
        }

        private LinkedList<Node> _OpenList = new LinkedList<Node>();
        private List<Node> _ClosedList = new List<Node>();
        private List<Node> _Successors = new List<Node>();

        private SearchState _State = SearchState.NotInitialised;
        private int _Steps;
        private Node _Start;
        private Node _Goal;
        
        private bool _CancelRequest = false;

        public SearchState State { get { return _State; } }

        public AStarSearch()
        {

        }

        public void CancelSearch()
        {
            _CancelRequest = true;
        }

        public void SetStartAndGoalStates(SpatialGraphKDNode Start, SpatialGraphKDNode Goal)
	    {
		    _CancelRequest = false;

		    _Start = new Node();
		    _Goal = new Node();

		    _Start._node = Start;
		    _Goal._node = Goal;

		    _State = SearchState.Searching;
    		
		    // Initialise the AStar specific parts of the Start Node
		    // The user only needs fill out the state information

		    _Start.g = 0; 
		    _Start.h = _Start.GoalDistanceEstimate( _Goal._node );
		    _Start.f = _Start.g + _Start.h;
		    _Start.parent = null;

		    // Push the start node on the Open list
            _OpenList.Clear();  // This isn't in the original implementation, but it makes sense.  Aren't we starting over?

            InsertIntoOpenList(_Start);

		    // Initialise counter for search steps
		    _Steps = 0;
	    }

        /// <summary>
        /// This function is a replacement for the use of heaps in the original algorithm.
        /// it inserts nodes always in order.
        /// </summary>
        private void InsertIntoOpenList(Node node)
        {
            LinkedListNode<Node> current = _OpenList.First;
            if (current == null)
            {
                _OpenList.AddLast(node);
                return;
            }

            for(; current.Next != null; current = current.Next)
            {
                if (node.f < current.Value.f)
                {
                    _OpenList.AddBefore(current, node);
                    return;
                }
            }

            // If we got here, we're the highest cost.
            _OpenList.AddLast(node);
        }

        // Advances search one step 
	    public SearchState SearchStep()
	    {
		    Debug.Assert( _State != SearchState.NotInitialised );
            if(_State == SearchState.NotInitialised)
                return _State;

		    // Failure is defined as emptying the open list as there is nothing left to 
		    // search...
		    // New: Allow user abort
		    if( _OpenList.Count == 0 || _CancelRequest )
		    {
			    // FreeAllNodes();
			    _State = SearchState.Failed;
			    return _State;
		    }
    		
		    // Incremement step count
		    _Steps ++;

		    // Pop the best node (the one with the lowest f) 
		    Node n = _OpenList.First.Value; // get pointer to the node
            _OpenList.RemoveFirst();

		    // Check for the goal, once we pop that we're done
		    if( n.IsGoal( _Goal._node ) )
		    {
			    // The user is going to use the Goal Node he passed in 
			    // so copy the parent pointer of n 
			    _Goal.parent = n.parent;

			    // A special case is that the goal was passed in as the start state
			    // so handle that here
			    if( false == n.IsSameState( _Start._node ) )
			    {
				    //FreeNode( n );

				    // set the child pointers in each node (except Goal which has no child)
				    Node nodeChild = _Goal;
				    Node nodeParent = _Goal.parent;

				    do 
				    {
					    nodeParent.child = nodeChild;

					    nodeChild = nodeParent;
					    nodeParent = nodeParent.parent;
    				
				    } 
				    while( nodeChild != _Start ); // Start is always the first node by definition

			    }

			    // delete nodes that aren't needed for the solution
			    // FreeUnusedNodes();

			    _State = SearchState.Succeeded;

			    return _State;
		    }
		    else // not goal
		    {

			    // We now need to generate the successors of this node
			    // The user helps us to do this, and we keep the new nodes in
			    // m_Successors ...

			    _Successors.Clear(); // empty vector of successor nodes to n

			    // User provides this functions and uses AddSuccessor to add each successor of
			    // node 'n' to m_Successors
			    n.GetSuccessors( this, n.parent != null ? n.parent._node : null ); 
    			
			    // Now handle each successor to the current node ...
			    foreach(Node successor in _Successors)
			    {
				    // 	The g value for this successor ...
				    float newg = n.g + n.GetCost( successor._node );

				    // Now we need to find whether the node is on the open or closed lists
				    // If it is but the node that is already on them is better (lower g)
				    // then we can forget about this successor

				    // First linear search of open list to find node
                    Node openlist_result = null, closedlist_result = null;
				    foreach(Node node in _OpenList)
				    {
                        if(node.IsSameState( successor._node ))
                        {
                            openlist_result = node;
					        break;					
                        }
				    }

                    if (openlist_result != null)
				    {
					    // we found this state on open
                        if (openlist_result.g <= newg)
					    {
						    //FreeNode( (*successor) );
						    // the one on Open is cheaper than this one
						    continue;
					    }
				    }

                    foreach (Node node in _ClosedList)
				    {
					    if( node.IsSameState(successor._node) )
					    {
                            closedlist_result = node;
						    break;					
					    }
				    }

                    if (closedlist_result != null)
				    {
					    // we found this state on closed
					    if( closedlist_result.g <= newg )
					    {
						    // the one on Closed is cheaper than this one
						    //FreeNode( (*successor) );
						    continue;
					    }
				    }

				    // This node is the best node so far with this particular state
				    // so lets keep it and set up its AStar specific data ...

				    successor.parent = n;
				    successor.g = newg;
				    successor.h = successor.GoalDistanceEstimate( _Goal._node );
				    successor.f = successor.g + successor.h;

				    // Remove successor from closed if it was on it
				    if( closedlist_result != null )
				    {
					    _ClosedList.Remove( closedlist_result );
				    }

				    // Update old version of this node
				    if( openlist_result != null )
				    {	   
                        _OpenList.Remove( openlist_result );
				    }

				    // heap now unsorted
				    InsertIntoOpenList( successor );
			    }

			    // push n onto Closed, as we have expanded it now
			    _ClosedList.Add( n );

		    } // end else (not goal so expand)

 		    return _State; // Succeeded bool is false at this point. 
	    }

        public void AddSuccessor( SpatialGraphKDNode State )
	    {
            Node node = new Node();
		    node._node = State;

		    _Successors.Add( node );
	    }

        public IEnumerable Solution()
        {
            Node currentSolutionNode = _Start;
            while (currentSolutionNode != null)
            {
                yield return currentSolutionNode._node;
                currentSolutionNode = currentSolutionNode.child;
            }
        }
    }
}
