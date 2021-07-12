using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const int PLAYER_AMOUNT = 2; // player�̑���

            // public AttackSignal[] signals = new AttackSignal[CHARACTER_AMOUNT];
            public List<CharacterData> sampleSignals; // �󂯎�����z������X�g�Ƃ��Ĉ������߂̃��X�g
            public int[] addPoint = new int[PLAYER_AMOUNT]; // �ǉ�����|�C���g��
            public CharacterAttack()
            {

            }

            public void AttackOrder(ref CharacterData[] characterDatas)
            {
                sampleSignals = new List<CharacterData>(); // ���X�g����������

                // �ǉ��|�C���g�ʏ�����
                for (int i = 0;i < addPoint.Length;i++)
                {
                    addPoint[i] = 0;
                }

                // ���X�g�Ɏ󂯎�����z����i�[
                for (int i = 0; i < characterDatas.Length;i++)
                {
                    sampleSignals.Add(characterDatas[i]);
                }
                
                AttackIndexCheck(); // �U���̏ꏊ���m�肷��֐�

                // �����Ă���|�C���g���e�L�����ɒǉ�
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                }
            }
            /// <summary>
            /// �U���̏ꏊ���m�肵�A�_���[�W�̔����֐����Ă�
            /// </summary>
            private void AttackIndexCheck()
            {
                var signalList = sampleSignals.OrderByDescending(x => x.attackSignal.skillData.priority); // �U�����Ƀ��X�g�����ւ���  

                foreach (var x in signalList)
                { 
                    // Debug.Log($"sampleList��{x.attackSignal.skillData.skillName}��{x.attackSignal.skillData.damage}�̃_���[�W���������邺");

                    if (!x.canAct) return; // ���g���U���ł��Ȃ��󋵂ɂ���ꍇ�A�������X�L�b�v����
                    if (!x.attackSignal.isAttack) return; // �U�������Ȃ��Ƃ�����񂪓����Ă���Ƃ��A�������X�L�b�v����

                    // �U���}�X������������
                    for (int j = 0;j < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length;j++)
                    {
                        FieldIndex attackPosition = x.attackSignal.selectedGrid + x.attackSignal.skillData.attackFieldIndexOffsetArray[j]; // �U���w��ʒu�ɁA�U���͈͂𑫂����ʂ��U���ʒu�Ƃ��ĕۑ�

                        AttackDamage(x, attackPosition); // �U���̃_���[�W�𔭐�����֐�

                        Debug.Log($"attackPosition.index({j}) = ({attackPosition.row},{attackPosition.column})");
                    }
                }
            }

            /// <summary>
            /// �_���[�W�@����с@�|�C���g�̑������s���֐�
            /// </summary>
            /// <param name="character">�L�����N�^�[���</param>
            /// <param name="attackPosition">�󂯎��U���ʒu</param>
            private void AttackDamage(CharacterData character,FieldIndex attackPosition)
            {
                for (int i = 0;i < sampleSignals.Count;i++)
                {
                    if (sampleSignals[i].thisObject == character.thisObject) continue; // �����̈ʒu�ɍU�����肪�����Ă��������Ȃ�

                    if (sampleSignals[i].index == attackPosition) // �U������̈ʒu�ƑΏۂ̈ʒu���������ꍇ
                    {
                        for (int j = 0; j < sampleSignals.Count;j++) // �����̃L�����f�[�^���擾���邽�ߍēxfor��
                        {
                            // �����̃L�����f�[�^�������ꍇ�A�ǉ��|�C���g�𑝂₷
                            if (sampleSignals[j].thisObject == character.thisObject) addPoint[j] += sampleSignals[j].attackSignal.skillData.damage;
                        }
                        Debug.Log($"{character.thisObject.name}��{character.attackSignal.skillData.name}��{sampleSignals[i].thisObject.name}�Ƀq�b�g���A{character.attackSignal.skillData.damage}�̃|�C���g�𓾂�");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }
        }

    }
}
