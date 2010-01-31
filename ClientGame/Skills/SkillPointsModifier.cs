using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefineYourself.Skills
{
    public delegate int PointModDelegate(int x);
    public class SkillPointsModifier
    {
        private Dictionary<SkillPointType.Types, PointModDelegate> _modifierDels;

        public Dictionary<SkillPointType.Types, PointModDelegate> GetModDels()
        {
            return _modifierDels;
        }

        public SkillPointsModifier()
        {
            _modifierDels = new Dictionary<SkillPointType.Types, PointModDelegate>();
            foreach (SkillPointType.Types type in Enum.GetValues(typeof(SkillPointType.Types)))
            {
                //modifierDelegates.Add(((SomeDelegate)(delegate(int x) { return x; }));
                _modifierDels.Add(type, delegate(int x) { return x; });
            }
            //foreach(Delegate del in modifierDelegates) {
            //    del.DynamicInvoke();
            //}
        }

        public void UpdateValues(SkillPointSet pointSet)
        {
            foreach (SkillPointType pointType in pointSet.PointTypes)
            {
                PointModDelegate del = _modifierDels[pointType.Type];
                pointType.Value = (int)del.DynamicInvoke(pointType.Value);
            }
        }
    }
}
