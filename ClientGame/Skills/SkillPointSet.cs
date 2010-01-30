using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefineYourself.Skills
{
    class SkillPointSet
    {
        //List<SkillPointType> PointTypes = new List<SkillPointType>();

        public List<SkillPointType> PointTypes { get; set; }

        public SkillPointSet(int red, int blue)
        {
            PointTypes = new List<SkillPointType>();
            PointTypes.Add(new SkillPointType(SkillPointType.Types.Red, red));
            PointTypes.Add(new SkillPointType(SkillPointType.Types.Blue, blue));
        }

        public void Report()
        {
            foreach (SkillPointType type in PointTypes)
            {
                Console.WriteLine(type.Type.ToString() + ": " + type.Value.ToString());
            }
        }
    }
}
