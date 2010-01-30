using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IntroGame.Screens;
using AngelXNA.Infrastructure.Console;
using Microsoft.Xna.Framework.Audio;
using AngelXNA.Messaging;

namespace IntroGame
{
    public class DemoScreen : Renderable
    {
        protected List<Renderable> _objects = new List<Renderable>();

        public virtual void Start()
        {

        }

        public virtual void Stop()
        {
            foreach (Renderable obj in _objects)
            {
                World.Instance.Remove(obj);
            }
            _objects.Clear();
        }
        
        public override void Update(GameTime aTime)
        {
            
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            
        }
    }

    class DemoGameManager : GameManager
    {
        private List<DemoScreen> _screens = new List<DemoScreen>();
        private int _current;
        private SoundEffect _click;

        public DemoGameManager()
        {
            // Allow MoveForards and MoveBackwards to occur both in the console
            // and through input bindings.
            DeveloperConsole.Instance.ItemManager.AddCommand("MoveForwards", new ConsoleCommandHandler(MoveForwards));
            DeveloperConsole.Instance.ItemManager.AddCommand("MoveBackwards", new ConsoleCommandHandler(MoveBackwards));

            Switchboard.Instance["MoveForwards"] += new MessageHandler(x => MoveForwards(null));
            Switchboard.Instance["MoveBackwards"] += new MessageHandler(x => MoveBackwards(null));

            FontCache.Instance.RegisterFont("Fonts\\Inconsolata24", "Console");
            FontCache.Instance.RegisterFont("Fonts\\Inconsolata12", "ConsoleSmall");

            _screens.Add(new DemoScreenStart());                        // 0
            _screens.Add(new DemoScreenInstructions());                 // 1
            _screens.Add(new DemoScreenSimpleActor());                  // 2
            _screens.Add(new DemoScreenRenderLayers());                 // 3
            _screens.Add(new DemoScreenMovingActor());                  // 4
            _screens.Add(new DemoScreenDefFile());                      // 5
            _screens.Add(new DemoScreenLevelFile());                    // 6
            _screens.Add(new DemoScreenBindingInstructions());          // 7
            _screens.Add(new DemoScreenParticleActor());                // 8
            _screens.Add(new DemoScreenPhysicsActor());                 // 9
            _screens.Add(new DemoScreenCollisions());                   // 10
            _screens.Add(new DemoScreenCollisionLevelFile());	        // 11
            _screens.Add(new DemoScreenLayeredCollisionLevelFile());	// 12
            _screens.Add(new DemoScreenMessagePassing());               // 13
            _screens.Add(new DemoScreenIntervals());                    // 14
            _screens.Add(new DemoScreenConsole());                      // 15
            _screens.Add(new DemoScreenLogs());                         // 16
            _screens.Add(new DemoScreenPathfinding());                  // 17
            _screens.Add(new DemoScreenByeBye());                       // 18

            int startingIndex = 0;
            if (_screens.Count > startingIndex)
            {
                World.Instance.Add(_screens[startingIndex]);
                _screens[startingIndex].Start();
                _current = startingIndex;
            }
            else
            {
                _current = -1;
            }

            _click = World.Instance.Game.Content.Load<SoundEffect>("Sounds\\click");
        }

        public object MoveForwards(object[] aParams)
        {
            if (_current >= 0 && (_current < _screens.Count - 1))
            {
                _screens[_current].Stop();
                World.Instance.Remove(_screens[_current]);
                _screens[++_current].Start();
                World.Instance.Add(_screens[_current]);
            }

            _click.Play();

            return null;
        }

        public object MoveBackwards(object[] aParams)
        {
            if (_current > 0)
            {
                _screens[_current].Stop();
                World.Instance.Remove(_screens[_current]);
                _screens[--_current].Start();
                World.Instance.Add(_screens[_current]);
            }

            _click.Play();

            return null;
        }

        public object MoveTo(object[] aParams)
        {
            DeveloperConsole.VerifyArgs(aParams, typeof(float));
            int ipage = (int)aParams[0];

            if (ipage > 0 && ipage < _screens.Count)
            {
                _screens[_current].Stop();
                World.Instance.Remove(_screens[_current]);
                _screens[ipage].Start();
                World.Instance.Add(_screens[ipage]);
                _current = ipage;
            }

            return null;
        }

        public override void Render(GameTime aTime, Camera aCamera, GraphicsDevice aDevice, SpriteBatch aBatch)
        {
            Color textColor = new Color(0.5f, 0.5f, 0.5f);
            SpriteFont font = FontCache.Instance["ConsoleSmall"];

            string infoString = "";
            int xOffset = 0;
            if (_current == 0)
            {
                infoString = "[A]: Next ";
                xOffset = 925;
            }
            else if (_current == _screens.Count - 1)
            {
                infoString = "[Back]: Previous";
                xOffset = 870;
            }
            else
            {
                infoString = "[A]: Next [Back]: Previous";
                xOffset = 785;
            }

            aBatch.Begin();
            aBatch.DrawString(font, infoString, new Vector2(xOffset, 745), textColor);
            aBatch.End();
        }
    }
}
