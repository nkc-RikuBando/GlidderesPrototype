using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Buff
    {
        public class BuffViewData : ScriptableObject
        {
            [SerializeField]
            public Sprite buffIcon;                     // バフのアイコン

            [SerializeField]
            public string buffName;                     // バフの名称

            [SerializeField]
            public string buffCaption;                  // バフの説明文

            [SerializeField]
            public List<BuffValueData> buffValueList;   // 実際のバフ情報
        }
    }
}
