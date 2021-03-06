using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Glidders.Buff;

namespace Glidders
{
    namespace Character
    {
        // プロジェクトウィンドウで作成可能にする
        [CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "CreateSkillData")]
        public class SkillScriptableObject : ScriptableObject
        {
            // プレイヤーが入力するスキル情報
            public string id;                     // 識別ID
            public string skillName;              // スキル名称
            public string skillCaption;           // スキル説明文
            public int energy;                    // エネルギー
            public int damage;                    // ダメージ
            public int priority;                  // 優先度
            public int power;                     // 威力(ダメージフィールド)
            public Sprite skillIcon;              // スキルアイコン
            public SkillTypeEnum skillType;       // スキルの種類（攻撃技か補助技か）
            public List<BuffViewData> giveBuff;   // 付与されるバフ
            public AnimationClip skillAnimation;  // アニメーションクリップ

            #region 範囲に関する実データを格納。外部からの参照非推奨。
            /// <summary> 参照非推奨。 </summary>
            public bool[] selectGridArray;                 // 選択可能マスを実際に格納しておく一次元配列
            /// <summary> 参照非推奨。 </summary>
            public bool[] attackGridArray;                 // 攻撃範囲を実際に格納しておく一次元配列
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
            public int rangeSize;
            /// <summary> 計算用変数。参照非推奨。 </summary>
            public int center;
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

        public enum SkillTypeEnum
        {
            ATTACK,
            SUPPORT,
        }
    }
}
