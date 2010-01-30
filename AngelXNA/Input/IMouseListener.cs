using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.Input
{
    public interface IMouseListener
    {
        void MouseDownEvent(Vector2 screenCoordinates, InputManager.MouseButton button);

        void MouseUpEvent(Vector2 screenCoordinates, InputManager.MouseButton button);

        void MouseMotionEvent(int screenPosX, int screenPosY);
    }
}
