using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefineYourself.Skills
{
    class SkillPointsModifier
    {

        List<Delegate> modifierDelegates = new List<Delegate>();

        delegate int SomeDelegate(int x);
        Dictionary<string, SomeDelegate> modifierDels = new Dictionary<string, SomeDelegate>();

        public SkillPointsModifier()
        {
            foreach (SkillPointType.Types type in Enum.GetValues(typeof(SkillPointType.Types)))
            {
                //modifierDelegates.Add(((SomeDelegate)(delegate(int x) { return x; }));
                modifierDels.Add(type.ToString(), delegate(int x) { return x + 2; });
            }
            //foreach(Delegate del in modifierDelegates) {
            //    del.DynamicInvoke();
            //}
        }

        public void UpdateValues(SkillPointSet pointSet)
        {
            foreach (SkillPointType pointType in pointSet.PointTypes)
            {
                SomeDelegate del = this.modifierDels[pointType.ToString()];
                pointType.Value = (int)del.DynamicInvoke(pointType.Value);
            }
        }
    }
}
