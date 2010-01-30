using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Util;
using AngelXNA.Infrastructure;
using AngelXNA.Infrastructure.Console;

namespace AngelXNA.Actors
{
    public class ParticleActor : Actor
    {
        struct Particle
        {
            public Vector2 _pos;
            public Vector2 _vel;
            public float _age;
            public float _lifetime;
            public float _rotation;
            public Color _color;
            public float _scale;
        };

        private Particle[] _particles;
	    private int		_maxParticlesAlive = 0;
	    private int		_numParticlesAlive;

	    private float	_particlesPerSecond = 20.0f;
	    private float	_generationResidue = 0.0f;

	    private float	_systemLifetime = 0.0f;	    // How long the system will live.   (seconds) Good for one shot effects.
	    private float	_particleLifetime = 2.0f;	// How long the particles will live. (seconds)

	    private float	_spreadRadians = 0.0f;		// How much the particles can deviate in direction from the system.

	    private Color   _endColor = Color.White;    // We use the Actor _color as the start color.

	    private float	_minSpeed = 2.0f;           // At emission, we choose a random speed between minSpeed and maxSpeed.
	    private float	_maxSpeed = 4.0f;

        private float   _minRotation = 0.0f;
        private float   _maxRotation = 0.0f;

	    private float   _endScale = 1.0f;    // Respects the starting aspect ratio of the sprite.

	    private Vector2 _gravity = new Vector2(0, 4.0f);

        [ConsoleProperty]
        public float MinRotation
        {
            get { return _minRotation; }
            set { _minRotation = value; }
        }

        [ConsoleProperty]
        public float MaxRotation
        {
            get { return _maxRotation; }
            set { _maxRotation = value; }
        }

        [ConsoleProperty]
        public float ParticlesPerSecond
        {
            get { return _particlesPerSecond; }
            set 
            {
                if (value < 0.0f)
                    value = 0.0f;
                _particlesPerSecond = value; 
            }
        }

        [ConsoleProperty]
        public float SystemLifetime
        {
            get { return _systemLifetime; }
            set 
            {
                if (value < 0.0f)
                    value = 0.0f;
                _systemLifetime = value; 
            }
        }

        [ConsoleProperty]
        public float ParticleLifetime
        {
            get { return _particleLifetime; }
            set 
            {
                if (value < 0.0f)
                    value = 0.0f;
                _particleLifetime = value; 
            }
        }

        [ConsoleProperty]
        public float Spread
        {
            get { return _spreadRadians; }
            set { _spreadRadians = value; }
        }

        [ConsoleProperty]
        public float EndScale
        {
            get { return _endScale; }
            set { _endScale = value; }
        }

        [ConsoleProperty]
        public Color EndColor
        {
            get { return _endColor; }
            set { _endColor = value; }
        }

        [ConsoleProperty]
        public Vector2 Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        [ConsoleProperty]
        public int MaxParticles
        {
            get { return _maxParticlesAlive; }
            set
            {
                if (_maxParticlesAlive == value)
                    return;

                if (value <= 0)
                    value = 1;
                
                _maxParticlesAlive = value;
                _particles = new Particle[value];                

                // Make them all available.   Age < 0.0f = free.
                for (int i = 0; i < _maxParticlesAlive; ++i)
                {
                    _particles[i]._age = -1.0f;
                }
            }
        }

        [ConsoleProperty]
        public float MinSpeed
        {
            get { return _minSpeed; }
            set { _minSpeed = value; }
        }

        [ConsoleProperty]
        public float MaxSpeed
        {
            get { return _maxSpeed; }
            set { _maxSpeed = value; }
        }

        public void SetSpeedRange(float minSpeed, float maxSpeed)
        {
            _minSpeed = minSpeed;
            _maxSpeed = maxSpeed;
        }

        public override void Update(GameTime aTime)
        {
            base.Update(aTime);

	        if (_maxParticlesAlive == 0)
		        return;

	        //
	        // Update existing particles.
	        //
	        _numParticlesAlive = 0;
	        for (int i=0; i < _maxParticlesAlive; ++i)
	        {
                if (_particles[i]._age < 0.0f)
			        continue;

                if (_particles[i]._age < _particles[i]._lifetime)
		        {
                    _particles[i]._age += (float)aTime.ElapsedGameTime.TotalSeconds;

                    if (_particles[i]._age < _particles[i]._lifetime)
			        {
				        // Where are we in our lifespan? (0..1)
                        float lifePercent = _particles[i]._age / _particles[i]._lifetime;

				        // Determine current position based on last known position, velocity and
				        // current time delta.
                        _particles[i]._pos = _particles[i]._pos + _particles[i]._vel * (float)aTime.ElapsedGameTime.TotalSeconds;

				        // Update our current velocity, which will be used next update.
                        _particles[i]._vel = _particles[i]._vel + _gravity * (float)aTime.ElapsedGameTime.TotalSeconds;

                        Vector4 startColor = Color.ToVector4();
                        Vector4 endColor = _endColor.ToVector4();
                        Vector4 currentColor = new Vector4();
				        currentColor.X = MathHelper.Lerp(startColor.X, endColor.X, lifePercent);
                        currentColor.Y = MathHelper.Lerp(startColor.Y, endColor.Y, lifePercent);
                        currentColor.Z = MathHelper.Lerp(startColor.Z, endColor.Z, lifePercent);
                        currentColor.W = MathHelper.Lerp(startColor.W, endColor.W, lifePercent);
                        _particles[i]._color = new Color(currentColor);
				        _particles[i]._scale = MathHelper.Lerp(1.0f, _endScale, lifePercent);
        				
				        ++_numParticlesAlive;
			        }
			        else 
			        {
                        _particles[i]._age = -1.0f;
			        }
		        }
	        }

	        // Systems with 0.0f lifetime live forever.
	        if (_systemLifetime > 0.0f)
		        _systemLifetime -= (float)aTime.ElapsedGameTime.TotalSeconds;

	        // We're dead, but we're waiting for our particle to finish.
	        if (_systemLifetime < 0.0f)
	        {
		        if (_numParticlesAlive == 0)
		        {
			        Destroy();
		        }

		        return;
	        }

	        //
	        // Create new particles.
	        //

	        // Add in any residual time from last emission.
            float dt = (float)aTime.ElapsedGameTime.TotalSeconds;
	        dt += _generationResidue;

	        int numParticlesToGenerate = (int)(_particlesPerSecond * dt);
	        _generationResidue = _particlesPerSecond * dt - (float)numParticlesToGenerate;
        	
	        if (numParticlesToGenerate > 0)
	        {		
		        float rot = MathHelper.ToRadians(Rotation);
		        float particleRot;

		        int particlesGenerated = 0;
		        for (int i=0; i<_maxParticlesAlive; ++i)
		        {
			        if (_particles[i]._age < 0.0f)
			        {
                        _particles[i]._age = 0.0f;
                        _particles[i]._lifetime = _particleLifetime;
                        _particles[i]._pos = Position;
        				
				        particleRot = MathUtil.RandomFloatWithError(rot, _spreadRadians);
				        float speed = MathUtil.RandomFloatInRange(_minSpeed, _maxSpeed);
                        _particles[i]._vel = new Vector2(speed * (float)Math.Cos(particleRot), speed * (float)Math.Sin(particleRot));

                        _particles[i]._rotation = MathUtil.RandomFloatInRange(_minRotation, _maxRotation);

				        ++particlesGenerated;

				        // If we've generated enough, break out.
				        if (particlesGenerated == numParticlesToGenerate)
					        break;
			        }
		        }
	        }
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            if (_particles == null)
                return;

            if (s_defaultTexture == null)
                s_defaultTexture = World.Instance.Game.Content.Load<Texture2D>("white");

            Texture2D myTexture = _spriteTextures[_spriteCurrentFrame];
            if (myTexture == null)
                myTexture = s_defaultTexture;

            // Render all of our particles.
            aBatch.Begin();
            for (int i = 0; i < _maxParticlesAlive; ++i)
            {
                if (_particles[i]._age < 0.0f)
                    continue;

                Vector2 screenPos = aCamera.WorldToScreen(_particles[i]._pos);
                Vector2 currentSize = Size * _particles[i]._scale;
                Vector2 screenSize = aCamera.WorldSizeToScreenSize(currentSize);
                Rectangle destRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)screenSize.X, (int)screenSize.Y);

                aBatch.Draw(myTexture, destRect, null, _particles[i]._color, _particles[i]._rotation,
                   new Vector2(myTexture.Width / 2, myTexture.Height / 2), SpriteEffects.None, 0.0f);
            }
            aBatch.End();
        }

        [ConsoleMethod]
        public static new ParticleActor Create()
        {
            return new ParticleActor();
        }
    }
}
