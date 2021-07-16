using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Glidders.Field;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const int PLAYER_AMOUNT = 4; // player�̑���
            const int YIELD_TIME = 1;

            public List<CharacterData> sampleSignals; // �󂯎�����z������X�g�Ƃ��Ĉ������߂̃��X�g
            public int[] addPoint = new int[PLAYER_AMOUNT]; // �ǉ�����|�C���g��

            private Animator[] animators = new Animator[Rule.maxPlayerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private int count = 0;
            private int defalutNumber = 0;
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap)
            {
                displayTile = displayTileMap;
                fieldCore = core;
                this.animators = animators; // GetComponent�ς݂̃A�j���[�^�[�z������̂܂ܓ����
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas, Action phaseCompleteAction)
            {
                sampleSignals = new List<CharacterData>(); // ���X�g����������
                count = 0;

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
                var signalList = sampleSignals.OrderByDescending(x => x.attackSignal.skillData.priority); // �U�����Ƀ��X�g�����ւ���  

                foreach (var x in signalList)
                {
                    Debug.Log($"{x.playerName}��{x.thisObject.name}��{x.attackSignal.skillData.skillName}��{x.attackSignal.skillData.damage}�̃_���[�W�l�������Ă��܂�");

                    if (!x.canAct) continue; // ���g���U���ł��Ȃ��󋵂ɂ���ꍇ�A�������X�L�b�v����
                    if (!x.attackSignal.isAttack) continue; // �U�������Ȃ��Ƃ�����񂪓����Ă���Ƃ��A�������X�L�b�v����

                    // �U���}�X������������
                    for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                    {
                        for (int j = 0; j < Rule.maxPlayerCount;j++)
                        {
                            if (sampleSignals[j].thisObject == x.thisObject) defalutNumber = j;
                        }

                        FieldIndex attackPosition = x.attackSignal.selectedGrid + x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // �U���w��ʒu�ɁA�U���͈͂𑫂����ʂ��U���ʒu�Ƃ��ĕۑ�

                        if (attackPosition.row > 0 && attackPosition.row < 8 && attackPosition.column > 0 && attackPosition.column < 8)
                        {
                            fieldCore.SetDamageField(defalutNumber, sampleSignals[defalutNumber].attackSignal.skillData.power, attackPosition);
                            displayTile.DisplayDamageFieldTilemap(attackPosition,defalutNumber);
                        }
                        AttackDamage(x, attackPosition); // �U���̃_���[�W�𔭐�����֐�

                        Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                    }

                    CameraPositionSeter();

                    AnimationPlaying(x.thisObject);

                    yield return new WaitForSeconds(YIELD_TIME); // �w��b����~

                    count++;
                }

                // �����Ă���|�C���g���e�L�����ɒǉ�
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                }

                phaseCompleteAction();

                Debug.Log("�����I��");
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
                        animators[i].SetTrigger("Damage");

                        Debug.Log($"{character.thisObject.name}��{character.attackSignal.skillData.name}��{sampleSignals[i].thisObject.name}�Ƀq�b�g���A{character.attackSignal.skillData.damage}�̃|�C���g�𓾂�");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            private void CameraPositionSeter()
            {
                // Debug.Log("�J���������֐����퓮��");
            }

            private void AnimationPlaying(GameObject animationObject)
            {
                // Debug.Log("�A�j���[�V�����Đ��֐����퓮��");
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    if (sampleSignals[i].thisObject == animationObject)
                    {
                        animators[i].SetTrigger("Act3");
                    }
                }
            }
        }

    }
}
