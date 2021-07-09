using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

            #region 範囲に関する実データを格納。外部からの参照非推奨。
            /// <summary> 参照非推奨。 </summary>
            public bool[] selectGridArray { get; set; }                 // 選択可能マスを実際に格納しておく一次元配列
            /// <summary> 参照非推奨。 </summary>
            public bool[] attackGridArray { get; set; }                 // 攻撃範囲を実際に格納しておく一次元配列
            #endregion

            // スキルの選択可能マスを格納したグリッドリスト
            public FieldIndexOffset[] selectFieldIndexOffsetArray
            {
                get => GetSelectFieldIndexOffsetArray();
            }

            // スキルの攻撃範囲を格納したグリッドリスト
            public FieldIndexOffset[] attackFieldIndexOffsetArray
            {
                get => GetAttackFieldIndexOffsetArray();
            }

            #region 計算用変数。参照非推奨。
            /// <summary> 計算用のため参照非推奨。 </summary> 
            public int rangeSize { get; set; }
            /// <summary> 計算用変数。参照非推奨。 </summary>
            public int center { get; set; }
            #endregion

            private FieldIndexOffset[] GetSelectFieldIndexOffsetArray()
            {
                List<FieldIndexOffset> selectGridTrueList = new List<FieldIndexOffset>();
                int index = 0;
                foreach (bool active in selectGridArray)
                {
                    if (active)
                    {
                        int rowOffset = index / rangeSize - center;
                        int columnOffset = index % rangeSize - center;
                        selectGridTrueList.Add(new FieldIndexOffset(rowOffset, columnOffset));
                    }
                    index++;
                }
                FieldIndexOffset[] returnArray = new FieldIndexOffset[selectGridTrueList.Count];
                for (int i = 0; i < selectGridTrueList.Count; i++)
                {
                    returnArray[i] = selectGridTrueList[i];
                }
                return returnArray;
            }

            private FieldIndexOffset[] GetAttackFieldIndexOffsetArray()
            {
                List<FieldIndexOffset> attackGridTrueList = new List<FieldIndexOffset>();
                int index = 0;
                foreach (bool active in attackGridArray)
                {
                    if (active)
                    {
                        int rowOffset = index / rangeSize - center;
                        int columnOffset = index % rangeSize - center;
                        attackGridTrueList.Add(new FieldIndexOffset(rowOffset, columnOffset));
                    }
                    index++;
                }
                FieldIndexOffset[] returnArray = new FieldIndexOffset[attackGridTrueList.Count];
                for (int i = 0; i < attackGridTrueList.Count; i++)
                {
                    returnArray[i] = attackGridTrueList[i];
                }
                return returnArray;
            }
        }
    }
}
