using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // プロジェクトウィンドウで作成可能にする
        [CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "CreateSkillData")]
        public class SkillScriptableObject : ScriptableObject
        {
            // プレイヤーが入力するスキル情報
            public string skillName { get; set; }            // スキル名称
            public int energy { get; set; }                  // エネルギー
            public int damage { get; set; }                  // ダメージ
            public int priority { get; set; }                // 優先度
            public int power { get; set; }                   // 威力(ダメージフィールド)
            //List<Buff> buff;    // スキルにより付与されるバフリスト
            //※public List<FieldIndexOffset> gridList { get; set; }  // スキルの攻撃範囲を格納したグリッドリスト
            public bool[,] selectRangeArray { get; set; } = new bool[13, 13];
            public bool[,] attackRangeArray { get; set; } = new bool[13, 13];


            public SkillScriptableObject()
            {
                //※gridList = new List<FieldIndexOffset>();
            }
        }
    }
}
