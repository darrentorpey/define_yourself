using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Physics;
using Microsoft.Xna.Framework.Audio;
using AngelXNA.Infrastructure;
using AngelXNA.Infrastructure.Console;

namespace AngelXNA.Collisions
{
    public class PlaySoundCollisionResponse : ICollisionResponse
    {
        private SoundEffect _sound;
        private float _volume;

        public PlaySoundCollisionResponse(SoundEffect effect, float volume)
        {
            _sound = effect;
            _volume = volume;
        }

        #region ICollisionResponse Members

        public void Execute(PhysicsEventActor struck, PhysicsEventActor striker)
        {
            _sound.Play(_volume);
        }

        #endregion

        [ConsoleMethod]
        public static PlaySoundCollisionResponse Create(string sound, float volume)
        {
            SoundEffect soundEffect = World.Instance.Game.Content.Load<SoundEffect>(sound);
            return new PlaySoundCollisionResponse(soundEffect, volume);
        }

        public static ICollisionResponse FactoryMethod(string[] input)
        {
            ICollisionResponse retVal = null;
            if (input.Length > 0)
            {
                SoundEffect sound = World.Instance.Game.Content.Load<SoundEffect>(input[0]);
                float volume = 1.0f;
                if (input.Length > 1)
                {
                    volume = float.Parse(input[1]);
                }

                retVal = new PlaySoundCollisionResponse(sound, volume);
            }

            return retVal;
        }
    }
}
