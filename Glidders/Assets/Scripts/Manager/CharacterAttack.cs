using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const int CHARACTER_AMOUNT = 2;

            // public AttackSignal[] signals = new AttackSignal[CHARACTER_AMOUNT];
            public List<AttackSignal> sampleSignals = new List<AttackSignal>();

            public List<AttackSignal> signals = new List<AttackSignal>();
            public CharacterAttack()
            {

            }

            public void AttackOrder(CharacterData[] characterDatas)
            {
                for (int i = 0; i < characterDatas.Length;i++)
                {
                    signals.Add(characterDatas[i].attackSignal);
                }

                // signals = sampleSignals.OrderByDescending(x => x.skillData.priority) as List<AttackSignal>;
            }
        }

    }
}
