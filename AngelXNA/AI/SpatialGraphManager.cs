using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngelXNA.AI
{
    public class SpatialGraphManager
    {
        private static SpatialGraphManager s_Instance = new SpatialGraphManager();

        private SpatialGraph _spatialGraph;

        public static SpatialGraphManager Instance
        {
            get { return s_Instance; }
        }

        public bool DrawBounds { get; set; }
        public bool DrawBlocked { get; set; }
        public bool DrawGridPoints { get; set; }
        public bool DrawGraph { get; set; }
        public bool DrawNodeIndex { get; set; }

        protected SpatialGraphManager()
        {
            DrawBounds = false;
            DrawBlocked = false;
            DrawGridPoints = false;
            DrawGraph = false;
            DrawNodeIndex = false;
        }

        public void CreateGraph(float entityWidth, ref BoundingBox2D bounds)
        {
            _spatialGraph = new SpatialGraph(entityWidth, ref bounds);
        }

        public void Render(Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            if (_spatialGraph != null)
                _spatialGraph.Render(aCamera, aDevice, aBatch);
        }

        public bool GetPath(Vector2 source, Vector2 dest, ref List<Vector2> path)
        {
            if (_spatialGraph == null)
                return false;

            //Get source cell
            SpatialGraphKDNode sourceNode = _spatialGraph.FindNode(ref source);
            if (sourceNode == null)
                return false;

            SpatialGraphKDNode destNode = _spatialGraph.FindNode(ref dest);
            if (destNode == null)
                return false;

            path.Add(source);
            if (sourceNode == destNode)
            {
                path.Add(dest);
                return true;
            }

            //Compute A*
            bool retVal = ComputeAStar(sourceNode, destNode, ref path);
            if (retVal == false)
            {
                path.Clear();
                return false;
            }

            //otherwise, put dest on the pathlist
            path.Add(dest);

            return true;
        }

        public bool CanGo(Vector2 from, Vector2 to)
        {
            if (_spatialGraph == null)
                return false;

            return _spatialGraph.CanGo(ref from, ref to);
        }

        public bool IsInPathableSpace(Vector2 point)
        {
            return CanGo(point, point);
        }

        public bool FindNearestNonBlocked( Vector2 fromPoint, out Vector2 goTo )
        {
            goTo = Vector2.Zero;

            if( _spatialGraph == null )
        		return false;

            SpatialGraphKDNode currentNode = _spatialGraph.FindNode( ref fromPoint );
            if( currentNode == null )
	            return false;

            float fMinDistance = float.MaxValue;
            SpatialGraphKDNode nearestNeighbor = null;
            //otherwise, iterate over neighbors to find a non-blocked
            for( int i = 0; i < currentNode.Neighbors.Count; i++ )
            {
	            SpatialGraphKDNode neighbor = currentNode.Neighbors[i];
	            if( neighbor.Blocked )
		            continue;

	            Vector2 vDir = neighbor.BBox.Centroid() - fromPoint;
	            Ray2D ray = new Ray2D( fromPoint, Vector2.Normalize(vDir) );

	            float distanceToBBox;
	            if( neighbor.BBox.Intersects(ref ray, out distanceToBBox) )
	            {
		            if( distanceToBBox < fMinDistance )
		            {
			            fMinDistance = distanceToBBox;
			            nearestNeighbor = neighbor;
		            }
	            }
            }

            if (nearestNeighbor != null)
            {
                goTo = nearestNeighbor.BBox.Centroid();
	            return true;
            }

            return false;
        }

        private static bool ComputeAStar(SpatialGraphKDNode sourceNode, SpatialGraphKDNode destNode, ref List<Vector2> path)
        {
            AStarSearch search = new AStarSearch();
	        
	        search.SetStartAndGoalStates( sourceNode, destNode );

	        while( AStarSearch.SearchState.Searching == search.SearchStep() )
	        {

	        }

	        if( search.State == AStarSearch.SearchState.Succeeded )
	        {
		        //Get path
                path = new List<Vector2>();
                foreach (SpatialGraphKDNode node in search.Solution())
		        {
                    path.Add(node.BBox.Centroid());
		        }
		        
                return true;
	        }

	        return false;
        }
    }
}
