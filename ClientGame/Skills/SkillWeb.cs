using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefineYourself.Skills
{
    class SkillWeb
    {
        public List<SkillNode> Nodes { get; set; }// = new List<SkillNode>();

        public SkillWeb()
        {
            Nodes = new List<SkillNode>();
        }

        public void AddNode(SkillNode node)
        {
            Nodes.Add(node);
        }

        public void Report()
        {
            Console.WriteLine("NodeZ:");
            foreach (SkillNode node in Nodes)
            {
                node.Report();
            }
        }
    }
}
