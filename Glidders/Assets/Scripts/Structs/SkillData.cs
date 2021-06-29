using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        public struct SkillData
        {
            string name;
            int energy;
            int damage;
            int priority;
            string caption;
            List<SkillRange> rangeList;
        }

        public struct SkillRange
        {
            int rowOffset;
            int columnOffset;
        }
    }
}
