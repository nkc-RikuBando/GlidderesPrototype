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
            public BuffTypeEnum buffType;           // バフが乗算か加算か

            [SerializeField]
            public float buffScale;                 // バフの倍率/加算値

            [SerializeField]
            public int buffDuration;                // バフの継続ターン数
        }

        /// <summary>
        /// バフが乗算か加算かを識別するためのもの
        /// </summary>
        public enum BuffTypeEnum
        {
            PLUS,
            MULTIPLIED,
        }
    }
}
