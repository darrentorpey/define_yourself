using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AngelXNA.Infrastructure.Logging;

namespace DefineYourself.Skills
{
    public class SkillPointSet
    {
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
                Log.Instance.Log(type.Type.ToString() + ": " + type.Value.ToString());
            }
        }
    }
}
