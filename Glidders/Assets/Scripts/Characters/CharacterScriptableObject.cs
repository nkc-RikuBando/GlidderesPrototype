using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // プロジェクトウィンドウで作成可能にする
        [System.Serializable, CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "CreateCharacterData")]
        public class CharacterScriptableObject : ScriptableObject
        {
            [SerializeField]
            public string id;                      // 識別ID
            [SerializeField]
            public string characterName;           // キャラクターの名前
            [SerializeField]
            public int moveAmount;                 // 移動量
            [SerializeField]
            public UniqueSkillScriptableObject[] skillDataArray = new UniqueSkillScriptableObject[Rule.skillCount];  // スキルを3つ格納する配列
            [SerializeField]
            public UniqueSkillScriptableObject uniqueSkillData; // ユニークスキル
            [SerializeField]
            public Animator characterAnimator;     // キャラクターのアニメーター
        }
    }
}