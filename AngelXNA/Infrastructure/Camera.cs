using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Actors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AngelXNA.Infrastructure
{
    public class Camera : Actor
    {
        private int _windowWidth;
        private int _windowHeight;
        private Vector3 _position;
        private Vector3 _viewCenter;
        private float _aperture = MathHelper.PiOver2;

        private Matrix _view;
        private Matrix _projection;
        private Matrix _spriteBatchView;

        public new Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RecalculateMatrices();
            }
        }

        public Vector3 ViewCenter 
        {
            get { return _viewCenter; }
            set
            {
                _viewCenter = value;
                RecalculateMatrices();
            }
        }

        public float ViewRadius
        {
            get
            {
                float sideAngle = _aperture / 2.0f;
                return (float)(Math.Tan(sideAngle) * Math.Abs(Position.Z));
            }
        }

        public int WindowWidth
        {
            get { return _windowWidth; }
        }

        public int WindowHeight
        {
            get { return _windowHeight; }
        }

        public Matrix Projection { get { return _projection; } }
        public Matrix View { get { return _view; } }
        public Matrix SpriteBatchView { get { return _spriteBatchView; } }

        public Camera(int aiWindowWidth, int aiWindowHeight, Vector3 aPosition, Vector3 aViewCenter)
        {
            _position = aPosition;
            _viewCenter = aViewCenter;
            Resize(aiWindowWidth, aiWindowHeight);
        }

        public void Resize(int aiWindowWidth, int aiWindowHeight)
        {
            if ((_windowHeight != aiWindowHeight) || (_windowWidth != aiWindowWidth))
            {
                _windowHeight = aiWindowHeight;
                _windowWidth = aiWindowWidth;

                // TODO: Set Viewport?
                // glViewport(0, 0, _windowWidth, _windowHeight);
            }

            RecalculateMatrices();
        }

        public Vector2 WorldToScreen(Vector2 position)
        {
            position.X -= Position.X;
            position.Y -= Position.Y;

            float worldWidth, worldHeight;
            int screenWidth = _windowWidth;
            int screenHeight = _windowHeight;
            float aspect = (float)screenWidth / (float)screenHeight;
            if (screenWidth > screenHeight)
            {
                //window is wider than it is tall; radius goes with height
                worldHeight = ViewRadius * 2.0f;
                worldWidth = worldHeight * aspect;
            }
            else
            {
                //window is taller than it is wide; radius goes with width
                worldWidth = ViewRadius * 2.0f;
                worldHeight = worldWidth / aspect;
            }

            float screenX = screenWidth * ((position.X / worldWidth) + 0.5f);
            float screenY = screenHeight - (screenHeight * (0.5f + (position.Y / worldHeight)));

            return new Vector2(screenX, screenY);
        }

        public Vector2 ScreenToWorld(int x, int y)
        {
	        float worldWidth, worldHeight;
	        int screenWidth = _windowWidth;
	        int screenHeight = _windowHeight;
	        float aspect = (float)screenWidth / (float)screenHeight;
	        if (screenWidth > screenHeight)
	        {
		        //window is wider than it is tall; radius goes with height
		        worldHeight = ViewRadius * 2.0f;
		        worldWidth = worldHeight * aspect;
	        }
	        else
	        {
		        //window is taller than it is wide; radius goes with width
		        worldWidth = ViewRadius * 2.0f;
		        worldHeight = worldWidth / aspect;
	        }

	        float worldX = ( ((float)x / (float)screenWidth) - 0.5f ) * worldWidth;
	        float worldY = ( 0.5f - ((float)y / (float)screenHeight) ) * worldHeight;
        	
            return new Vector2(worldX + Position.X, worldY + Position.Y);
        }

        public Vector2 ScreenSizeToWorldSize(Vector2 size)
        {
            float worldWidth, worldHeight;
            int screenWidth = _windowWidth;
            int screenHeight = _windowHeight;
            float aspect = (float)screenWidth / (float)screenHeight;
            if (screenWidth > screenHeight)
            {
                //window is wider than it is tall; radius goes with height
                worldHeight = ViewRadius * 2.0f;
                worldWidth = worldHeight * aspect;
            }
            else
            {
                //window is taller than it is wide; radius goes with width
                worldWidth = ViewRadius * 2.0f;
                worldHeight = worldWidth / aspect;
            }
            
            return new Vector2((size.X / screenWidth) * worldWidth, (size.Y / screenHeight) * worldHeight);

        }

        public Vector2 WorldSizeToScreenSize(Vector2 size)
        {
            // Calculate the camera's translation into a pixel translation
            float worldWidth, worldHeight;
            int screenWidth = _windowWidth;
            int screenHeight = _windowHeight;
            float aspect = (float)screenWidth / (float)screenHeight;
            if (screenWidth > screenHeight)
            {
                //window is wider than it is tall; radius goes with height
                worldHeight = ViewRadius * 2.0f;
                worldWidth = worldHeight * aspect;
            }
            else
            {
                //window is taller than it is wide; radius goes with width
                worldWidth = ViewRadius * 2.0f;
                worldHeight = worldWidth / aspect;
            }

            Vector2 position = new Vector2(
                screenWidth * ((size.X / worldWidth)),
                screenHeight * ((size.Y / worldHeight))
            );

            return position;
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            
        }

        private void RecalculateMatrices()
        {
            _view = Matrix.CreateLookAt(Position, Position + ViewCenter, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(_aperture, (float)_windowWidth / (float)_windowHeight,
                0.001f, 200.0f);

             // Calculate the camera's translation into a pixel translation
            float worldWidth, worldHeight;
            int screenWidth = _windowWidth;
            int screenHeight = _windowHeight;
            float aspect = (float)screenWidth / (float)screenHeight;
            if (screenWidth > screenHeight)
            {
                //window is wider than it is tall; radius goes with height
                worldHeight = ViewRadius * 2.0f;
                worldWidth = worldHeight * aspect;
            }
            else
            {
                //window is taller than it is wide; radius goes with width
                worldWidth = ViewRadius * 2.0f;
                worldHeight = worldWidth / aspect;
            }

            Vector2 position = new Vector2(
                screenWidth * ((-_position.X / worldWidth)),
                screenHeight * ((_position.Y / worldHeight))
            );

            _spriteBatchView = Matrix.CreateTranslation(new Vector3(position, 0.0f));
        }
    }
}
