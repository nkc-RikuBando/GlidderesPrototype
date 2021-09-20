using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Buff;

namespace Glidders
{
    namespace Character
    {
        // プロジェクトウィンドウで作成可能にする
        [CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "CreateSkillData")]
        public class UniqueSkillScriptableObject : ScriptableObject
        {
            // ユニークスキルの識別情報
            public string skillName;                    // スキル名称
            public string skillCaption;                 // スキル説明文
            public int energy;                          // エネルギー
            public int priority;                        // 優先度
            public Sprite skillIcon;                    // スキルアイコン
            public AnimationClip skillAnimation;        // アニメーションクリップ
            public SkillTypeEnum skillType;             // スキルの種類（攻撃技か補助技か）

            public UniqueSkillMoveType moveType;        // 移動の種類
            public FieldIndexOffset[] moveSelectRange   // 移動先マス
            {
                get => GetRangeArray(moveSelectArray);
            }

            public int damage;                          // ダメージ
            public int power;                           // 威力(ダメージフィールド)
            public FieldIndexOffset[] attackSelectRange // 選択可能マス
            {
                get => GetRangeArray(attackSelectArray);
            }
            public FieldIndexOffset[] attackRange       // 攻撃範囲マス
            {
                get => GetRangeArray(attackArray);
            }
            public List<BuffViewData> giveBuff;         // 付与されるバフ

            #region 範囲に関する実データを格納。外部からの参照非推奨。
            public bool[] moveSelectArray;       // 移動先マスの原型。11*11の121マス。
            public bool[] attackSelectArray;     // 選択可能マスの原型。11*11の121マス。
            public bool[] attackArray;           // 攻撃範囲マスの原型。11*11の121マス。
            #endregion

            #region 計算用変数。参照非推奨。
            /// <summary> 計算用のため参照非推奨。 </summary> 
            private static int rangeSize = 11 * 2 - 1;
            /// <summary> 計算用変数。参照非推奨。 </summary>
            private static int center = rangeSize / 2;
            #endregion

            /// <summary>
            /// 範囲に関する全てのデータが入った配列から、選択されているマスだけを抽出した配列を取得する。
            /// </summary>
            /// <param name="beforeArray">抽出前の全てのデータが入った配列。</param>
            /// <returns>選択されているマスだけを抽出した配列。</returns>
            private FieldIndexOffset[] GetRangeArray(bool[] beforeArray)
            {
                // 配列から選択されているマスだけを抽出する
                List<FieldIndexOffset> workList = new List<FieldIndexOffset>();
                int index = 0;
                foreach (bool active in beforeArray)
                {
                    if (active)
                    {
                        int rowOffset = index / rangeSize - center;
                        int columnOffset = index % rangeSize - center;
                        workList.Add(new FieldIndexOffset(rowOffset, columnOffset));
                    }
                    ++index;
                }

                // 選択されているマスだけを格納した配列を返却する
                FieldIndexOffset[] returnArray = new FieldIndexOffset[workList.Count];
                for (int i = 0; i < returnArray.Length; ++i)
                {
                    returnArray[i] = workList[i];
                }

                return returnArray;
            }
        }

        public enum UniqueSkillMoveType
        {
            NONE,
            FREE,
            FIXED,
        }
    }
}
