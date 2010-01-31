using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using AngelXNA.Actors;
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Infrastructure;

namespace DefineYourself
{
    public class MapSpot : Actor
    {
        public static List<Actor> BuildingList { get; set; }

        public MapSpot()
        {
            Size = new Vector2(40, 40);
            Color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Log.Instance.Log("Added a map spot");
        }

        [ConsoleMethod]
        public static new MapSpot Create()
        {
            MapSpot newSpot = new MapSpot();
            BuildingList.Add(newSpot);
            World.Instance.Add(newSpot);
            return newSpot;
        }
    }
}
