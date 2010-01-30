using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework;

namespace AngelXNA.Util
{
    public class DrawUtil
    {
        private static BasicEffect _Effect2D;
        
        private static void Initialize(Camera aCamera, GraphicsDevice aDevice)
        {
            _Effect2D = new BasicEffect(aDevice, null);
            Matrix proj = Matrix.CreateOrthographicOffCenter(0.0f, aCamera.WindowWidth, aCamera.WindowHeight, 0.0f, -1.0f, 1.0f);
            
            _Effect2D.Projection = proj;
            _Effect2D.VertexColorEnabled = true;
        }

        public static void DrawTile(Camera aCamera, GraphicsDevice aDevice, int xPos, int yPos, int width, int height, Color aColor)
        {
            if (_Effect2D == null)
                Initialize(aCamera, aDevice);

            // TODO: All of this has to be fixed.  This is the most ineffecient drawing code ever.
            Matrix world = Matrix.CreateTranslation(new Vector3(xPos, yPos, 0.0f));

            _Effect2D.World = world;

            VertexPositionColor[] tileVerts = new VertexPositionColor[] {
                new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), aColor),
                new VertexPositionColor(new Vector3(width, 0.0f, 0.0f), aColor),
                new VertexPositionColor(new Vector3(width, height, 0.0f), aColor),
                new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), aColor),
                new VertexPositionColor(new Vector3(width, height, 0.0f), aColor),
                new VertexPositionColor(new Vector3(0.0f, height, 0.0f), aColor)
            };

            _Effect2D.Begin();
            foreach (EffectPass pass in _Effect2D.CurrentTechnique.Passes)
            {
                pass.Begin();
                aDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, tileVerts, 0, 2);
                pass.End();
            }
            _Effect2D.End();
        }
    }
}
