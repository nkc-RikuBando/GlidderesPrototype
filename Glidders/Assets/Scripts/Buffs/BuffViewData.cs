using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;

namespace Glidders
{
    namespace Buff
    {
        public class BuffViewData : ScriptableObject
        {
            [SerializeField]
            public string id;                           // 識別ID

            [SerializeField]
            public Sprite buffIcon;                     // バフのアイコン

            [SerializeField]
            public string buffName;                     // バフの名称

            [SerializeField]
            public string buffCaption;                  // バフの説明文

            [SerializeField]
            public GameObject effectObjectPrefab;       // バフの演出を行うGameObjectのPrefabを格納

            [SerializeField]
            public CharacterScriptableObject upperTransform;    // 変身後のキャラクター

            [SerializeField]
            public CharacterScriptableObject lowerTransform;    // 変身前のキャラクター

            [SerializeField]
            public List<BuffValueData> buffValueList;   // 実際のバフ情報
        }
    }
}
