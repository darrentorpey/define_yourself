using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using AngelXNA.Actors;
using AngelXNA.Infrastructure.Console;

namespace DefineYourself.Skills
{
    class SkillNode
    {        
        [ConsoleProperty]
        public string Name { get; set; }
        
        [ConsoleProperty]
        public string IconName { set { Actor.SetSprite(value); } }
        
        [ConsoleProperty]
        public Vector2 Location { get { return Actor.Position; } set { Actor.Position = value; } }
        
        [ConsoleProperty]
        public Color Color { get { return Actor.Color; } set { Actor.Color = value; } }
        
        [ConsoleProperty]
        public int PointsNeeded { get; set; }

        public void SetLocation(Vector2 loc)
        {
            Actor.Position = loc;
        }
        public SkillPointsModifier SkillPointsModifier { get; set; }

        public List<SkillNode> parents = new List<SkillNode>();

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

            SkillPointsModifier = new SkillPointsModifier();
            SkillNode.Nodes.Add(this);
            SkillWeb.Instance.AddNode(this);
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

        public void AddPrereq(SkillNode skillNode)
        {
            parents.Add(skillNode);
        }

        public void Report()
        {
            Console.WriteLine("Parents:");
            foreach(SkillNode parent in parents) {
                Console.WriteLine(parent.ToString());
            }
        }

        public override String ToString()
        {
            return Name.ToString();
        }

        public void UpdateValues(SkillPointSet skillPointSet) {
            SkillPointsModifier.UpdateValues(skillPointSet);
        }

        [ConsoleMethod]
        public static new SkillNode Create()
        {
            return new SkillNode();
        }
    }
}
