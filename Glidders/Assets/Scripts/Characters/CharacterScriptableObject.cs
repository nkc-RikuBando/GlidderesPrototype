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
            public string characterName { get; set; }           // キャラクターの名前
            public int moveAmount { get; set; }                 // 移動量

            public SkillScriptableObject[] skillDatas { get; set; }  // スキルを3つ格納する配列
        }
    }
}