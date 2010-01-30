using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AngelXNA.Infrastructure;
using AngelXNA.Util;
using Box2DX.Collision;
using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;

using Angel = AngelXNA.Infrastructure;
using Box2D = Box2DX.Dynamics;
using Box2DX.Common;

namespace AngelXNA.AI
{
    public class SpatialGraphKDNode
    {
        private static Texture2D s_defaultTexture;
        private static BasicEffect s_Effect;

        public BoundingBox2D BBox;
        public SpatialGraphKDNode LHC;
        public SpatialGraphKDNode RHC;
        public SpatialGraphKDNode Parent;
        public SpatialGraph Tree;

        public int Index;
        public int Depth;
        public bool Blocked;

        public List<SpatialGraphKDNode> Neighbors = new List<SpatialGraphKDNode>();
        public List<bool> NeighborLOS = new List<bool>();

        public bool HasChildren
        {
            get { return LHC != null && RHC != null; }
        }

        public SpatialGraphKDNode(BoundingBox2D bb, SpatialGraphKDNode parent)
        {
            BBox = bb;
            Parent = parent;
        }

        public void GetGridPoints(out List<Vector2> points, out int xPoints, out int yPoints)
        {
            xPoints = 0;
            yPoints = 0;

            Vector2 vSmallestDimensions = Tree.SmallestDimensions;
	        Vector2 vMyBoxDimensions = BBox.Max - BBox.Min;

	        /*
	        if( vSmallestDimensions == vMyBoxDimensions )
	        {
		        xPoints = 1;
		        yPoints = 1;
		        points.push_back( BBox.Centroid() );
		        return;
	        }
	        */

	        xPoints = (int)(vMyBoxDimensions.X / vSmallestDimensions.X);
	        yPoints = (int)(vMyBoxDimensions.Y / vSmallestDimensions.Y);
	        points = new List<Vector2>(xPoints*yPoints);

	        Vector2 vBottomLeftStartBox = new Vector2( BBox.Min.X, BBox.Max.Y - vSmallestDimensions.Y );
	        BoundingBox2D startBox = new BoundingBox2D( vBottomLeftStartBox, vBottomLeftStartBox + vSmallestDimensions);
	        BoundingBox2D checkBox = startBox;

	        for( int yDim = 0; yDim < yPoints; ++yDim )
	        {
		        for( int xDim = 0; xDim < xPoints; ++xDim )
		        {
			        points.Add( checkBox.Centroid() );

			        checkBox.Min.X += vSmallestDimensions.X;
			        checkBox.Max.X += vSmallestDimensions.X;
		        }

		        checkBox.Min.X = startBox.Min.X;
		        checkBox.Max.X = startBox.Max.X;

		        checkBox.Min.Y -= vSmallestDimensions.Y;
		        checkBox.Max.Y -= vSmallestDimensions.Y;
	        }
        }

        public void Render(Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            if (HasChildren)
            {
                LHC.Render(aCamera, aDevice, aBatch);
                RHC.Render(aCamera, aDevice, aBatch);
                return;
            }

            if (SpatialGraphManager.Instance.DrawBlocked )
            {
                if (Blocked)
                    BBox.Render(new Color(1.0f, 0.0f, 0.0f, 0.25f), aCamera, aDevice, aBatch);
            }

            if (SpatialGraphManager.Instance.DrawBounds)
            {
                // TODO; Render Outline of a BBox
            }

            Vector2 centroid = BBox.Centroid();

	        if( SpatialGraphManager.Instance.DrawNodeIndex )
	        {
		        Vector2 screenCenter = aCamera.WorldToScreen( centroid );
		        aBatch.Begin();
                aBatch.DrawString(FontCache.Instance["ConsoleSmall"], Index.ToString(), screenCenter,  new Color(0, 1, 1, 1));
                aBatch.End();
	        }

	        if( SpatialGraphManager.Instance.DrawGraph && !Blocked )
	        {
                if(s_Effect == null)
                {
                    s_Effect = new BasicEffect(aDevice, null);
                    s_Effect.VertexColorEnabled = true;
                    s_Effect.World = Matrix.Identity;
                }


		        List<VertexPositionColor> lineList = new List<VertexPositionColor>();

		        for( int i = 0; i < Neighbors.Count; i++ )
		        {
			        if( Neighbors[i].Blocked || !NeighborLOS[i] )
				        continue;
			        //Draw lines
			        Vector2 neighbor = Neighbors[i].BBox.Centroid();
			        neighbor = centroid + ((neighbor - centroid) * 0.6f);

                    lineList.Add(new VertexPositionColor(
                        new Vector3(centroid.X, centroid.Y, 0.0f), Color.Red));
                    lineList.Add(new VertexPositionColor(
                        new Vector3(neighbor.X, neighbor.Y, 0.0f), Color.Red));
		        }

                VertexPositionColor[] lineArray = lineList.ToArray();

                s_Effect.View = aCamera.View;
                s_Effect.Projection = aCamera.Projection;
                s_Effect.Begin();
                for (int i = 0; i < s_Effect.CurrentTechnique.Passes.Count; ++i)
                {
                    s_Effect.CurrentTechnique.Passes[i].Begin();
                    aDevice.VertexDeclaration = new VertexDeclaration(aDevice, VertexPositionColor.VertexElements);
                    aDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, lineArray, 0, lineArray.Length / 2);
                    s_Effect.CurrentTechnique.Passes[i].End();
                }
                s_Effect.End();
	        }

	        if( SpatialGraphManager.Instance.DrawGridPoints )
	        {
                if (s_defaultTexture == null)
                    s_defaultTexture = Angel.World.Instance.Game.Content.Load<Texture2D>("white");
                
		        Color pointColor = Color.Red;
		        List<Vector2> gridPoints;
		        int xPoints, yPoints;
		        GetGridPoints(out gridPoints, out xPoints, out yPoints);

                aBatch.Begin();
		        for( int i = 0; i < gridPoints.Count; i++ )
		        {
                    Vector2 screenLoc = aCamera.WorldToScreen(gridPoints[i]);
                    Rectangle rect = new Rectangle((int)screenLoc.X, (int)screenLoc.Y, 5, 5);
                    aBatch.Draw(s_defaultTexture, rect, pointColor);
		        }
                aBatch.End();
	        }
        }
    }

    public class SpatialGraph
    {
        private int _depth;
        private float _entityWidth;
        private Vector2 _smallestDimensions;
        SpatialGraphKDNode _root;
        uint[] _dirMasks = new uint[4];

        public int Depth { get { return _depth; } }
        public Vector2 SmallestDimensions { get { return _smallestDimensions; } }

        public SpatialGraph(float entityWidth, ref BoundingBox2D startBox)
        {
            _entityWidth = entityWidth;
            float maxDimension = MathHelper.Max(startBox.Max.Y - startBox.Min.Y, startBox.Max.X - startBox.Min.X);
            int depth = 0;
            while (maxDimension > _entityWidth)
            {
                maxDimension /= 2.0f;
                depth += 2;
            }
            _depth = depth > 1 ? depth : 1;
            if (_depth > 24)
                _depth = 24;

            uint depthMask = ~(0xFFFFFFFF << _depth);

            _dirMasks[0] = 0x1;
            _dirMasks[1] = 0x2;
            _dirMasks[2] = 0xaaaaaaaa & depthMask;
            _dirMasks[3] = _dirMasks[2] >> 1;

            _root = CreateTree(_depth + 1, ref startBox, null, 0);

            //Get smallest dimension
            _smallestDimensions = startBox.Max - startBox.Min;
            for (int i = 0; i < _depth; i++)
            {
                if (i % 2 != 0)
                    _smallestDimensions.Y *= 0.5f;
                else
                    _smallestDimensions.X *= 0.5f;
            }

            ComputeNeighbors(_root);
            ValidateNeighbors(_root);
        }

        public SpatialGraphKDNode FindNode(SpatialGraphKDNode node, ref BoundingBox2D bbox)
        {
            if (node == null)
                return null;
            //check if this bbox fits entirely within our node
            if (node.BBox.Contains(ref bbox) == BoundingBox2D.ContainmentType.Within)
            {
                //Check our children
                SpatialGraphKDNode retVal = FindNode(node.LHC, ref bbox);
                if (retVal != null)
                    return retVal;
                retVal = FindNode(node.RHC, ref bbox);
                if (retVal != null)
                    return retVal;

                //otherwise, return ourselves
                return node;
            }

            return null;
        }

        public SpatialGraphKDNode FindNode(SpatialGraphKDNode node, ref Vector2 point)
        {
            if (node == null)
                return null;
            //check if this bbox fits entirely within our node
            if (node.BBox.Contains(ref point))
            {
                //Check our children
                SpatialGraphKDNode retVal = FindNode(node.LHC, ref point);
                if (retVal != null)
                    return retVal;
                retVal = FindNode(node.RHC, ref point);
                if (retVal != null)
                    return retVal;

                //otherwise, return ourselves
                return node;

            }

            return null;
        }

        public SpatialGraphKDNode FindNode(ref BoundingBox2D bbox)
        {
            return FindNode(_root, ref bbox);
        }

        public SpatialGraphKDNode FindNode(ref Vector2 point)
        {
            return FindNode(_root, ref point);
        }

        public bool CanGo(ref Vector2 vFrom, ref Vector2 vTo)
        {
            //Get source cell
            SpatialGraphKDNode sourceNode = FindNode(ref vFrom);
            if (sourceNode == null || sourceNode.Blocked)
                return false;

            SpatialGraphKDNode destNode = FindNode(ref vTo);
            if (destNode == null || destNode.Blocked)
                return false;

            return CanGoInternal(ref vFrom, ref vTo, sourceNode, destNode);
        }

        public void Render(Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            bool bDrawAny = SpatialGraphManager.Instance.DrawBounds || SpatialGraphManager.Instance.DrawGridPoints 
                || SpatialGraphManager.Instance.DrawGraph || SpatialGraphManager.Instance.DrawBlocked;

            if (bDrawAny && _root != null)
            {
                _root.Render(aCamera, aDevice, aBatch);
            }
        }        

        private SpatialGraphKDNode CreateTree(int depth, ref BoundingBox2D bbox)
        {
            return CreateTree(depth, ref bbox, null, 0);
        }

        private SpatialGraphKDNode CreateTree(int depth, ref BoundingBox2D bbox, SpatialGraphKDNode parent, int index)
        {
            SpatialGraphKDNode node = new SpatialGraphKDNode(bbox, parent);
	        node.Tree = this;
	        node.Blocked = false;

	        //query physics to see if we're blocked
	        node.Blocked = IsBlocked( ref bbox );

	        //Calculate my index
	        if( parent != null )
	            node.Index = index;
	        else
	            node.Index = 0;
	        
	        //Bail out if we reach max depth
	        depth--;
	        node.Depth = _depth - depth;
	        if (depth > 0 && node.Blocked )
	        {
		        BoundingBox2D LHSbbox, RHSbbox;
                MathUtil.SplitBoundingBox(ref bbox, depth % 2 == 0 ? MathUtil.AABBSplittingAxis.Y : MathUtil.AABBSplittingAxis.X, out LHSbbox, out RHSbbox);
		        node.LHC = CreateTree(depth, ref LHSbbox, node, node.Index << 1);
		        node.RHC = CreateTree(depth, ref RHSbbox, node, (node.Index << 1) + 1);

		        uint iMask = ~(0xFFFFFFFF << depth );
		        //If I have children, pad my index
		        node.Index = (int)((((uint)node.Index) << depth) | iMask);

		        //If all my children are blocked, then destroy my children
		        if( IsFullyBlocked(node) )
		        {
			        node.LHC = null;
			        node.RHC = null;
		        }
	        }

	        return node;
        }

        private void AddNeighbor(SpatialGraphKDNode node, Vector2 pos)
        {
            SpatialGraphKDNode neighbor = node.Tree.FindNode(ref pos);
            if (neighbor != null)
            {
                //Add unique
                for (int i = 0; i < node.Neighbors.Count; i++)
                {
                    if (node.Neighbors[i] == neighbor)
                        return;
                }

                node.Neighbors.Add(neighbor);
                node.NeighborLOS.Add(true);
            }
        }

        private void ComputeNeighbors(SpatialGraphKDNode node)
        {
            if( node.HasChildren )
            {
	            ComputeNeighbors(node.LHC);
	            ComputeNeighbors(node.RHC);
	            return;
            }

            Vector2 checkN = Vector2.UnitY * _smallestDimensions.Y;
            Vector2 checkS = Vector2.UnitY * -_smallestDimensions.Y;
            Vector2 checkE = Vector2.UnitX * _smallestDimensions.X;
            Vector2 checkW = Vector2.UnitX * -_smallestDimensions.X;

            Vector2 centroid = node.BBox.Centroid();

            List<Vector2> gridPoints;
            int xPoints, yPoints;
            node.GetGridPoints(out gridPoints, out xPoints, out yPoints );

            //Check north neighbors
            for( int i = 0; i < xPoints; i++ )
            {
	            AddNeighbor( node, gridPoints[GetColumnMajorIndex(i,0,xPoints)] + checkN );
            }

            //Check south neighbors
            for( int i = 0; i < xPoints; i++ )
            {
	            AddNeighbor( node, gridPoints[GetColumnMajorIndex(i,yPoints-1,xPoints)] + checkS );
            }

            //Check east neighbors
            for( int i = 0; i < yPoints; i++ )
            {
	            AddNeighbor( node, gridPoints[GetColumnMajorIndex(xPoints-1,i,xPoints)] + checkE );
            }

            //Check west neighbors
            for( int i = 0; i < yPoints; i++ )
            {
	            AddNeighbor( node, gridPoints[GetColumnMajorIndex(0,i,xPoints)] + checkW );
            }
        }

        private void ValidateNeighbors(SpatialGraphKDNode node)
        {
            if (node.HasChildren)
            {
                ValidateNeighbors(node.LHC);
                ValidateNeighbors(node.RHC);
                return;
            }
            //Validate neighbors
            for (int i = 0; i < node.Neighbors.Count; i++)
            {
                //Todo, incorporate entity width
                if (!CanGoNodeToNode(node, node.Neighbors[i]))
                {
                    node.NeighborLOS[i] = false;
                }
            }
        }
        
        private bool IsFullyBlocked(SpatialGraphKDNode node)
        {
            if (node == null)
                return true;

            return node.Blocked && IsFullyBlocked(node.LHC) && IsFullyBlocked(node.RHC);
        }

        private bool CanGoInternal(ref Vector2 vFrom, ref Vector2 vTo, SpatialGraphKDNode sourceNode, SpatialGraphKDNode destNode)
        {
            //If source is dest, we definitely can go (we're both within bounding box)
	        if( sourceNode == destNode )
		        return true;

	        Vector2 vUseFrom = vFrom;
	        Vector2 vUseTo = vTo;

	        NudgePointOnPlane( ref sourceNode.BBox, ref vUseFrom );
	        NudgePointOnPlane( ref destNode.BBox, ref vUseTo );

	        Ray2D ray = Ray2D.CreateRayFromTo( ref vUseFrom, ref vUseTo );

	        Dictionary<SpatialGraphKDNode, int> NodeList = new Dictionary<SpatialGraphKDNode,int>();
	        SpatialGraphKDNode current = sourceNode;

	        while( true )
	        {
		        //Mark current as visited
		        NodeList[current] = 1;
		        SpatialGraphKDNode nearestNeighbor = null;
                float fNearestNeighborDistance = float.MaxValue;
		        //iterate over currents neighbors to see if they intersect the ray
		        for( int i = 0; i < current.Neighbors.Count; ++i )
		        {
			        SpatialGraphKDNode neighbor = current.Neighbors[i];

			        //Ignore neighbors we've already visited
			        if( NodeList.ContainsKey(neighbor) )
				        continue;

			        float fDistanceAlongRay;
			        if( neighbor.BBox.Intersects( ref ray, out fDistanceAlongRay ) )
			        {
				        if( fDistanceAlongRay < fNearestNeighborDistance )
				        {
					        fNearestNeighborDistance = fDistanceAlongRay;
					        nearestNeighbor = neighbor;
				        }
			        }
		        }

		        //If we couldn't find a nearest neighbor, or the neighbor is blocked bail out
		        if( nearestNeighbor == null || nearestNeighbor.Blocked )
			        break;

		        //If the nearest neighbor is our destination, we found it!
		        if( nearestNeighbor == destNode )
			        return true;

		        //otherwise, check our neighbor
		        current = nearestNeighbor;
	        }

	        return false;
        }

        private bool CanGoNodeToNode(SpatialGraphKDNode sourceNode, SpatialGraphKDNode destNode)
        {
            Vector2 sourceCenter = sourceNode.BBox.Centroid();
            Vector2 destCenter = destNode.BBox.Centroid();
            return CanGoInternal(ref sourceCenter, ref destCenter, sourceNode, destNode);
        }

        private static bool IsBlocked(ref BoundingBox2D bbox)
        {
            const int maxShapeCount = 1024;
	        AABB physBounds;
	        physBounds.LowerBound = new Vector2( bbox.Min.X, bbox.Min.Y ); 
	        physBounds.UpperBound = new Vector2( bbox.Max.X, bbox.Max.Y );
	        
            Shape[] tempShapes = new Shape[maxShapeCount];
	        int numBroadphase = Angel.World.Instance.PhysicsWorld.Query( physBounds, tempShapes, maxShapeCount );

	        //No bodies here
	        if( numBroadphase == 0 )
		        return false;

	        PolygonDef shapeBoundsDef = new PolygonDef();
	        shapeBoundsDef.VertexCount = 4;
	        shapeBoundsDef.Vertices[0] = new Vector2( physBounds.LowerBound.X, physBounds.LowerBound.Y );
	        shapeBoundsDef.Vertices[1] = new Vector2( physBounds.UpperBound.X, physBounds.LowerBound.Y );
	        shapeBoundsDef.Vertices[2] = new Vector2( physBounds.UpperBound.X, physBounds.UpperBound.Y );
	        shapeBoundsDef.Vertices[3] = new Vector2( physBounds.LowerBound.X, physBounds.UpperBound.Y );

	        BodyDef fakeBodyDef = new BodyDef();
	        //b2Vec2 center = physBounds.lowerBound + (0.5f * shapeBoundsDef.extents);
	        fakeBodyDef.Position = Vector2.Zero;
	        Body fakeBody = new Body( fakeBodyDef, Angel.World.Instance.PhysicsWorld );
	        PolygonShape shapeBounds = new PolygonShape( shapeBoundsDef );

	        for( int i = 0; i < numBroadphase; i++ )
	        {
		        Shape Sh = tempShapes[i];
		        if( Sh.Type == ShapeType.PolygonShape  )
		        {
			        PolygonShape PolyShape = (PolygonShape)Sh;

			        Manifold m0 = new Manifold();
                    XForm xf1 = fakeBody.XForm;
                    XForm xf2 = PolyShape.Body.XForm;
                    Collision.CollidePolygons(ref m0, shapeBounds, ref xf1, PolyShape, ref xf2);

			        if( m0.PointCount > 0 )
				        return true;

		        }
		        else if( Sh.Type == ShapeType.CircleShape )
		        {
			        CircleShape CircleShape = (CircleShape)Sh;
			        Manifold m0 = new Manifold();
			        Collision.CollidePolygonAndCircle( ref m0, shapeBounds, fakeBody.XForm, CircleShape, CircleShape.Body.XForm );
			        if( m0.PointCount > 0 )
				        return true;
		        }
	        }

	        return false;
        }

        private static void NudgePointOnPlane( ref BoundingBox2D BBox, ref Vector2 vPointOnPlane )
        {
            //Get off the x planes
	        if( vPointOnPlane.X == BBox.Min.X )
	        {
		        vPointOnPlane.X += MathUtil.Epsilon;
	        }
	        else if( vPointOnPlane.X == BBox.Max.X )
	        {
		        vPointOnPlane.X -= MathUtil.Epsilon;
	        }

	        //Get off the Y planes
	        if( vPointOnPlane.Y == BBox.Min.Y )
	        {
		        vPointOnPlane.Y += MathUtil.Epsilon;
	        }
	        else if( vPointOnPlane.Y == BBox.Max.Y )
	        {
		        vPointOnPlane.Y -= MathUtil.Epsilon;
	        }
        }

        private static int GetColumnMajorIndex(int wantX, int wantY, int maxX)
        {
            return (wantY * maxX) + wantX;
        }
    }
}
