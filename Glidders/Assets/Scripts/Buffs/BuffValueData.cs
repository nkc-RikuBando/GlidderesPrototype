using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;

namespace Glidders
{
    namespace Buff
    {
        public class BuffValueData : ScriptableObject
        {
            [SerializeField]
            public StatusTypeEnum buffedStatus;     // �o�t�����X�e�[�^�X

            [SerializeField]
            public BuffTypeEnum buffType;           // �o�t�̃X�P�[��

            [SerializeField]
            public float buffScale;                 // �o�t�̔{��/���Z�l

            [SerializeField]
            public int buffDuration;                // �o�t�̌p���^�[����
        }

        public enum BuffTypeEnum
        {
            PLUS,
            MULTIPLIED,
        }
    }
}
