using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // プロジェクトウィンドウで作成可能にする
        [CreateAssetMenu(fileName = "SkillScriptableObjectWork", menuName = "CreateSkillWorkData")]
        public class SkillScriptableObjectWork : ScriptableObject
        {
            // プレイヤーが入力するスキル情報
            public string skillName { get; set; }            // スキル名称
            public int energy { get; set; }                  // エネルギー
            public int damage { get; set; }                  // ダメージ
            public int priority { get; set; }                // 優先度
            public int power { get; set; }                   // 威力(ダメージフィールド)
            //List<Buff> buff;    // スキルにより付与されるバフリスト
            //※public List<FieldIndexOffset> gridList { get; set; }  // スキルの攻撃範囲を格納したグリッドリスト
            public int FIELD_SIZE = 13; // 7 * 2 - 1

            [SerializeField]
            public bool[] array = new bool[13];
            [SerializeField]
            public bool[,] selectRangeArray = new bool[13, 13];
            [SerializeField]
            public bool[,] attackRangeArray = new bool[13, 13];
            public List<FieldIndexOffset> selectGridList { get; set; }
            public List<FieldIndexOffset> attackGridList { get; set; }

            public SkillScriptableObjectWork()
            {
                //※gridList = new List<FieldIndexOffset>();
                selectGridList = new List<FieldIndexOffset>();
                attackGridList = new List<FieldIndexOffset>();
            }
        }
    }
}
