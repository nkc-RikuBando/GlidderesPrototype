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
            public StatusTypeEnum buffedStatus;     // バフされるステータス

            [SerializeField]
            public BuffTypeEnum buffType;           // バフのスケール

            [SerializeField]
            public float buffScale;                 // バフの倍率/加算値

            [SerializeField]
            public int buffDuration;                // バフの継続ターン数
        }

        public enum BuffTypeEnum
        {
            PLUS,
            MULTIPLIED,
        }
    }
}
