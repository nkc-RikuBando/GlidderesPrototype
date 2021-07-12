using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // プロジェクトウィンドウで作成可能にする
        [CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "CreateCharacterData")]
        public class CharacterScriptableObject : ScriptableObject
        {
            [SerializeField]
            public string characterName;           // キャラクターの名前
            [SerializeField]
            public int moveAmount;                 // 移動量
            [SerializeField]
            public SkillDatasClass skillDatas;

            public CharacterScriptableObject()
            {
                //skillDatas.skillDataArray = new SkillScriptableObject[Rule.skillCount];
            }
        }

        public class SkillDatasClass
        {
            public SkillScriptableObject[] skillDataArray;  // スキルを3つ格納する配列

            public SkillDatasClass()
            {
                skillDataArray = new SkillScriptableObject[Rule.skillCount];
            }
        }
    }
}