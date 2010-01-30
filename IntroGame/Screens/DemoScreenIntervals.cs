using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AngelXNA.Actors;
using AngelXNA.Input;
using AngelXNA.Messaging;
using AngelXNA.Infrastructure;
using AngelXNA.Infrastructure.Logging;

namespace IntroGame.Screens
{
    class DemoScreenIntervals : DemoScreen
    {
        private Actor shiftingActor;
        private TextActor text1;
        private TextActor text2;

        public override void Start()
        {
            // Create the particle actor via the Actor Definition system (.adf files)
            shiftingActor = new Actor();
            shiftingActor.Size = new Vector2(4.0f, 4.0f);
            shiftingActor.Position = new Vector2(-5.0f, 0.0f);
            shiftingActor.Color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
            World.Instance.Add(shiftingActor);

	        Switchboard.Instance.Broadcast(new Message("IntervalScreenStarted"));

            // Register message (delegates) that will handle transitions between what morphs are happening
            Switchboard.Instance["IntervalScreenStarted"] += MoveRight;
            Switchboard.Instance["LeftMoveDone"] += MoveRight;
            Switchboard.Instance["RightMoveDone"] += MoveLeft;

            text1 = new TextActor("Console", "This Actor is using Intervals to change its properties. ");
            text1.Position = new Vector2(0.0f, 3.5f);
            text1.TextAlignment = TextActor.Alignment.Center;
            String explanation = "Intervals are kind of \"fire and forget\" functions that let you";
            explanation += "\ngive a goal state and a duration, then the Actor itself";
            explanation += "\ndoes the interpolation for you.";
            text2 = new TextActor("Console", explanation);
            text2.Position = new Vector2(0.0f, -4.0f);
            text2.TextAlignment = TextActor.Alignment.Center;
            World.Instance.Add(text1);
            World.Instance.Add(text2);

            #region Demo housekeeping
            TextActor fileLoc = new TextActor("ConsoleSmall", "DemoScreenIntervals.cs");
            fileLoc.Position = World.Instance.Camera.ScreenToWorld(5, 755);
            fileLoc.Color = new Color(.3f, .3f, .3f);
            World.Instance.Add(fileLoc);
            _objects.Add(shiftingActor);
            _objects.Add(fileLoc);
            _objects.Add(text1);
            _objects.Add(text2);
            #endregion
        }

        public void MoveRight(Message message)
        {
            shiftingActor.MoveTo(   // Change an Actor's position over an interval
                  new Vector2(5.0f, 0.0f),  //the new position
                  3.0f,					    //how long it should take to get there
                  true,					    //whether or not the interval should use MathUtil::SmoothStep
                  "RightMoveDone"		    //the (optional) message to send when the transition is done
            );
            shiftingActor.RotateTo(45.0f, 3.0f, true);
            shiftingActor.ChangeColorTo(new Color(1.0f, 0.0f, 1.0f, 1.0f), 3.0f, true);
            shiftingActor.ChangeSizeTo(1.0f, 3.0f, true);
        }

        public void MoveLeft(Message message)
        {
            shiftingActor.MoveTo(   // Change an Actor's position over an interval
                  new Vector2(-5.0f, 0.0f),  //the new position
                  3.0f,					    //how long it should take to get there
                  true,					    //whether or not the interval should use MathUtil::SmoothStep
                  "LeftMoveDone"		    //the (optional) message to send when the transition is done
            );
            shiftingActor.RotateTo(0.0f, 3.0f, true);
            shiftingActor.ChangeColorTo(new Color(1.0f, 1.0f, 0.0f, 0.5f), 3.0f, false);
            shiftingActor.ChangeSizeTo(3.0f, 3.0f, true);
        }
    }
}
