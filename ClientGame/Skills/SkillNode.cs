using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using AngelXNA.Actors;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Infrastructure.Logging;

namespace DefineYourself.Skills
{
    public class SkillNode
    {        
        [ConsoleProperty]
        public string Name { get; set; }
        
        [ConsoleProperty]
        public string IconName { set { Actor.SetSprite(value); } }
        
        [ConsoleProperty]
        public Vector2 Location { get { return Actor.Position; } set { Actor.Position = new Vector2(value.X, 384 - value.Y); } }
        
        [ConsoleProperty]
        public Color Color { get { return Actor.Color; } set { Actor.Color = value; } }

        public int P1_Points { get; set; }
        public Actor P1_ProgBar { get; set; }
        public int P2_Points { get; set; }
        public Actor P2_ProgBar { get; set; }

        public int PointsNeeded { get; set; }

        public void SetLocation(Vector2 loc)
        {
            Actor.Position = loc;
        }
        public SkillPointsModifier SkillPointsModifier { get; set; }

        public List<SkillNode> prereqs = new List<SkillNode>();

        public static int ProgressBarHeight = 40;

        public Actor Actor { get; set; }

        private static List<SkillNode> s_Nodes = new List<SkillNode>();
        public static List<SkillNode> Nodes { get { return s_Nodes; } set { s_Nodes = value; } }

        public SkillNode()
        {
            Actor = new Actor();
            Actor.Color = new Color(1.0f, 1.0f, 1.0f);
            Actor.DrawShape = Actor.ActorDrawShape.Square;
            Actor.Size = new Vector2(40, 40);
            Actor.Tag("skill_node");

            PointsNeeded = 50;

            SkillPointsModifier = new SkillPointsModifier();
            SkillNode.Nodes.Add(this);
            SkillWeb.Instance.AddNode(this);
        }

        public bool Completed(string playerName)
        {
            if (playerName == "Player 1") {
                return P1_Points >= PointsNeeded;
            } else {
                return P2_Points >= PointsNeeded;
            }
        }

        public void Draw()
        {
            int height;

            if (P1_Points == 0)
            {
                height = 5;
            }
            else
            {
                height = Math.Min(ProgressBarHeight, (int)(ProgressBarHeight * ((P1_Points + 0.0f) / (PointsNeeded + 0.0f))));
            }
            P1_ProgBar.Size = new Vector2(P1_ProgBar.Size.X, height);

            if (P2_Points == 0)
            {
                height = 5;
            }
            else
            {
                height = Math.Min(ProgressBarHeight, (int)(ProgressBarHeight * ((P2_Points + 0.0f) / (PointsNeeded + 0.0f))));
            }
            P2_ProgBar.Size = new Vector2(P2_ProgBar.Size.X, height);
        }

        public SkillNode(string name)
        {
            Actor = new Actor();
            Name = name;
            Actor.Size = new Vector2(40, 40);
            Actor.Position = new Vector2(-256.0f, 0.0f);
            Actor.Color = new Color(1.0f, 0.3f, 0.0f);
            Actor.DrawShape = Actor.ActorDrawShape.Square;

            SkillPointsModifier = new SkillPointsModifier();
        }

        public bool ActiveForPlayer(string playerName)
        {
            prereqs.RemoveAll(x => { return x == null; });
            return prereqs.All(prereq => { return prereq.Completed(playerName); });
        }

        [ConsoleMethod]
        public void Prereq(string name)
        {
            SkillNode skillNode = SkillWeb.Instance.FindNodeByName(name);
            prereqs.Add(skillNode);
        }

        public void Report()
        {
            //Console.WriteLine("Parents:");
            //foreach (SkillNode prereq in prereqs)
            //{
            //    Console.WriteLine(prereq.ToString());
            //}

            Log.Instance.Log("P1: " + P1_Points);
            Log.Instance.Log("----");
            Log.Instance.Log("P2: " + P2_Points);
        }

        public override String ToString()
        {
            return Name.ToString();
        }

        public void UpdateValues(SkillPointSet skillPointSet) {
            SkillPointsModifier.UpdateValues(skillPointSet);
        }

        [ConsoleMethod]
        public static SkillNode Create()
        {
            return new SkillNode();
        }
    }
}
