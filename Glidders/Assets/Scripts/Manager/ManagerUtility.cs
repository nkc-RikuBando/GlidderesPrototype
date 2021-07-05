using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;

namespace Glidders
{
    namespace Manager
    {
        /// <summary>
        /// プレイヤーがサーバーに移動情報を送る際に用いられる構造体
        /// </summary>
        public struct MoveSignal
        {
            public FieldIndexOffset[] moveDataArray;    // 移動情報を格納した配列

            public MoveSignal(FieldIndexOffset[] moveDataArray)
            {
                this.moveDataArray = new FieldIndexOffset[Rule.maxMoveAmount];
                for (int i = 0; i < this.moveDataArray.Length; i++)
                {
                    this.moveDataArray[i] = FieldIndexOffset.zero;                    
                }

                for (int i = 0; i < moveDataArray.Length; i++)
                {
                    if (i >= this.moveDataArray.Length) break;
                    this.moveDataArray[i] = moveDataArray[i];
                }
            }
        }

        public struct DirecionSignal
        {
            public FieldIndexOffset direction;

            public DirecionSignal(FieldIndexOffset direction)
            {
                this.direction = direction;
            }
        }

        /// <summary>
        /// プレイヤーがサーバーに攻撃情報を送る際に用いられる構造体。
        /// </summary>
        public struct AttackSignal
        {
            bool isAttack;
            SkillScriptableObject skillData;    // 使用するスキル情報が格納されたSkillScriptableObject。
            FieldIndex selectedGrid;            // 
            FieldIndexOffset direction;

            public AttackSignal(SkillScriptableObject skillData, FieldIndex selectedGrid, FieldIndexOffset direction)
            {
                isAttack = true;
                this.skillData = skillData;
                this.selectedGrid = selectedGrid;
                this.direction = direction;
            }

            public AttackSignal(bool flg, SkillScriptableObject skillData, FieldIndex selectedGrid, FieldIndexOffset direction)
            {
                isAttack = flg;
                this.skillData = skillData;
                this.selectedGrid = selectedGrid;
                this.direction = direction;
            }

            public AttackSignal(bool setFalseToFlgAutomatically)
            {
                isAttack = false;
                skillData = null;
                selectedGrid = FieldIndex.minus;
                direction = FieldIndexOffset.zero;
            }
        }

        // ターン終了時に同期する内容（まだ継承していない）
        public struct EndTurnSignal
        {
            int audience;
        }
    }
}
