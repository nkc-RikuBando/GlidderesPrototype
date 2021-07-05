using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;

namespace Glidders
{
    namespace Manager
    {
        /// <summary>
        /// �v���C���[���T�[�o�[�Ɉړ����𑗂�ۂɗp������\����
        /// </summary>
        public struct MoveSignal
        {
            public FieldIndexOffset[] moveDataArray;    // �ړ������i�[�����z��

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
        /// �v���C���[���T�[�o�[�ɍU�����𑗂�ۂɗp������\���́B
        /// </summary>
        public struct AttackSignal
        {
            bool isAttack;
            SkillScriptableObject skillData;    // �g�p����X�L����񂪊i�[���ꂽSkillScriptableObject�B
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

        // �^�[���I�����ɓ���������e�i�܂��p�����Ă��Ȃ��j
        public struct EndTurnSignal
        {
            int audience;
        }
    }
}
