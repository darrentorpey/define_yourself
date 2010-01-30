using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngelXNA.Actors
{
    public class GridActor : Renderable
    {
        private Vector2 _minCoord = new Vector2(-100.0f, -100.0f);
        private Vector2 _maxCoord = new Vector2(100.0f, 100.0f);

        // XNA specific rendering stuff
        private int _primativeCount;
        private BasicEffect _effect;
        private VertexBuffer _verts;

        public Color LineColor { get; set; }
        public Color AxisColor { get; set; }
        public float Interval { get; set; }
        
        public GridActor()
        {
            LineColor = new Color(.76f, .84f, 1.0f);
            AxisColor = new Color(1.0f, .41f, .6f);
            Interval = 1.0f;
        }

        public GridActor(Color lines, Color axis, float interval)
        {
            LineColor = lines;
            AxisColor = axis;
            Interval = interval;
        }

        public override void Update(GameTime aTime)
        {
            
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            if (_verts == null)
            {
                CreateRenderElements(aDevice);
            }

            _effect.View = aCamera.View;
            _effect.Projection = aCamera.Projection;
            _effect.Begin();
            for (int i = 0; i < _effect.CurrentTechnique.Passes.Count; ++i)
            {
                _effect.CurrentTechnique.Passes[i].Begin();
                aDevice.VertexDeclaration = new VertexDeclaration(aDevice, VertexPositionColor.VertexElements);
                aDevice.Vertices[0].SetSource(_verts, 0, VertexPositionColor.SizeInBytes);
                aDevice.DrawPrimitives(PrimitiveType.LineList, 0, _primativeCount);
                _effect.CurrentTechnique.Passes[i].End();
            }
            _effect.End();
        }

        private void CreateRenderElements(GraphicsDevice aDevice)
        {
            _primativeCount = (int)((_maxCoord.X - _minCoord.X) / Interval);
            _primativeCount += (int)((_maxCoord.Y - _minCoord.Y) / Interval);
            _primativeCount += 2;   // For the axis
            int vertexCount = _primativeCount * 2;

            VertexPositionColor[] points = new VertexPositionColor[vertexCount];
            int i = 0;
            for (float f = _minCoord.X; f < _maxCoord.X; f += Interval, i += 2)
            {
                points[i].Color = LineColor;
                points[i].Position = new Vector3(f, _minCoord.Y, 0.0f);

                points[i + 1].Color = LineColor;
                points[i + 1].Position = new Vector3(f, _maxCoord.Y, 0.0f);
            }
            for (float f = _minCoord.Y; f < _maxCoord.Y; f += Interval, i += 2)
            {
                points[i].Color = LineColor;
                points[i].Position = new Vector3(_minCoord.X, f, 0.0f);

                points[i + 1].Color = LineColor;
                points[i + 1].Position = new Vector3(_maxCoord.X, f, 0.0f);
            }

            points[i].Color = AxisColor;
            points[i].Position = new Vector3(_minCoord.X, 0.0f, 0.0f);
            points[i + 1].Color = AxisColor;
            points[i + 1].Position = new Vector3(_maxCoord.X, 0.0f, 0.0f);
            points[i + 2].Color = AxisColor;
            points[i + 2].Position = new Vector3(0.0f, _minCoord.Y, 0.0f);
            points[i + 3].Color = AxisColor;
            points[i + 3].Position = new Vector3(0.0f, _maxCoord.Y, 0.0f);
            
            _verts = new VertexBuffer(aDevice, typeof(VertexPositionColor), vertexCount, BufferUsage.WriteOnly);
            _verts.SetData<VertexPositionColor>(points);

            _effect = new BasicEffect(aDevice, null);
            _effect.VertexColorEnabled = true;
            _effect.World = Matrix.Identity;
        }
    }
}
