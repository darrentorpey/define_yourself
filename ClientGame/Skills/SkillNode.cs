using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefineYourself.Skills
{
    class SkillNode
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Location { get; set; }
        public string PointsNeeded { get; set; }

        SkillPointsModifier skillPointsModifier = new SkillPointsModifier();

        public List<SkillNode> parents = new List<SkillNode>();

        public SkillNode(string id)
        {
            this.ID = id;
            //this.skillPointsModifier.setType
        }

        public void AddParent(SkillNode skillNode)
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

        public String ToString()
        {
            return this.ID.ToString();
        }

        public void UpdateValues(SkillPointSet skillPointSet) {
            skillPointsModifier.UpdateValues(skillPointSet);
        }
    }
}
