using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Glidders.Field;
using Glidders.Graphic;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const int PLAYER_AMOUNT = 4; // player�̑���

            private int defalutNumber = 0; // Linq�ɂ���ē���ւ�����v�f�ԍ����A���̔ԍ������m���ۑ�����ϐ�
            private int targetObjectLength; // �֐�TargetSetting�Ɏg����ϐ��@�ύX�O��setTargetObject�̑��ʂ�ۑ�����ϐ�

            public List<CharacterData> sampleSignals; // �󂯎�����z������X�g�Ƃ��Ĉ������߂̃��X�g
            public int[] addPoint = new int[PLAYER_AMOUNT]; // �ǉ�����|�C���g��

            // �e�N���X
            private Animator[] animators = new Animator[Rule.maxPlayerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private CameraController cameraController;
            private FieldIndexOffset index;
            private FieldIndex attackPosition;

            private List<GameObject> setTargetObject = new List<GameObject>();
            private CharacterDirection[] characterDirections;
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap,CharacterDirection[] directions, CameraController cameraController)
            {
                // GetComponent�ς݂̊e�N���X�����̂܂ܓ����
                displayTile = displayTileMap;
                characterDirections = directions;
                fieldCore = core;
                this.cameraController = cameraController;
                this.animators = animators;
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas, AnimationClip clip,Action phaseCompleteAction)
            {
                sampleSignals = new List<CharacterData>(); // ���X�g����������
                cameraController.ClearTarget(); // �S�Ă̒Ǐ]�Ώۂ���������

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
                    // Debug.Log($"{x.playerName}��{x.thisObject.name}��{x.attackSignal.skillData.skillName}��{x.attackSignal.skillData.damage}�̃_���[�W�l�������Ă��܂�");

                    if (!x.canAct) continue; // ���g���U���ł��Ȃ��󋵂ɂ���ꍇ�A�������X�L�b�v����
                    if (!x.attackSignal.isAttack) continue; // �U�������Ȃ��Ƃ�����񂪓����Ă���Ƃ��A�������X�L�b�v����

                    AnimationPlaying(x.thisObject); // �A�j���[�V�����̍Đ����s���֐����Ăяo��

                    cameraController.ClearTarget(); // �S�ẴJ�����Ǐ]�Ώۂ���������
                    setTargetObject.Clear(); // �J�����Ǐ]�Ώۂɐݒ肳��Ă���I�u�W�F�N�g������������

                    // �U���}�X������������
                    for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                    {
                        for (int j = 0; j < Rule.maxPlayerCount; j++)
                        {
                            if (sampleSignals[j].thisObject == x.thisObject) defalutNumber = j;
                        }

                        #region �X�L���̌����Ɋ�Â����ʂɂȂ�悤��FieldIndex�𒲐����鏈��
                        if (x.attackSignal.direction == FieldIndexOffset.left)
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // index�Ɍ��݂�FieldIndexOffset����
                            index = new FieldIndexOffset(index.columnOffset, index.rowOffset); // �X�L�������̏c�Ɖ������ւ���
                        }
                        else if (x.attackSignal.direction == FieldIndexOffset.right)
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // index�Ɍ��݂�FieldIndexOffset����
                            index = new FieldIndexOffset(index.columnOffset, index.rowOffset) * -1; // �X�L�������̏c�Ɖ������ւ����̂��A���̕����ɕϊ�
                        }
                        else if (x.attackSignal.direction == FieldIndexOffset.up)
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // index�Ɍ��݂�FieldIndexOffset����
                        }
                        else
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i] * -1; // index�Ɍ��݂�FieldIndexOffset���������̂��A���̕����ɕϊ�
                        }
                        #endregion

                        // Debug.Log($"�U�����W ({x.attackSignal.selectedGrid.row},{x.attackSignal.selectedGrid.column})");
                        // Debug.Log($"index({index.rowOffset},{index.columnOffset})");

                        attackPosition = x.attackSignal.selectedGrid + index; // �U���w��ʒu�ɁA�U���͈͂𑫂����ʂ��U���ʒu�Ƃ��ĕۑ�

                        if (attackPosition.row > 0 && attackPosition.row < 8 && attackPosition.column > 0 && attackPosition.column < 8)
                        {
                            fieldCore.SetDamageField(defalutNumber, sampleSignals[defalutNumber].attackSignal.skillData.power, attackPosition);
                            displayTile.DisplayDamageFieldTilemap(attackPosition, fieldCore.GetDamageFieldOwner(attackPosition));
                        }
                        AttackDamage(x, attackPosition); // �U���̃_���[�W�𔭐�����֐�

                        if (i == x.attackSignal.skillData.attackFieldIndexOffsetArray.Length - 1 && setTargetObject.Count == 0) setTargetObject.Add(x.thisObject); 
                        // Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                    }

                    CameraPositionSeter(setTargetObject); // �J���������֐�
                    // yield return new WaitForSeconds(YIELD_TIME); // �w��b����~
                    yield return new WaitForSeconds(clip.length); // ���Ŏ󂯎�����A�j���[�V�����N���b�v�̍Đ����ԕ��̃R���[�`�������s
                }

                // �����Ă���|�C���g���e�L�����ɒǉ�
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                    characterDirections[i].SetDirection(characterDatas[i].direcionSignal.direction);
                }

                phaseCompleteAction(); // ����������ʒm

                // Debug.Log("�����I��");
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
                        for (int j = 0; j < sampleSignals.Count; j++) // �����̃L�����f�[�^���擾���邽�ߍēxfor��
                        {
                            // �����̃L�����f�[�^�������ꍇ�A�ǉ��|�C���g�𑝂₷
                            if (sampleSignals[j].thisObject == character.thisObject)
                            {
                                addPoint[i] -= sampleSignals[j].attackSignal.skillData.damage;
                                addPoint[j] += sampleSignals[j].attackSignal.skillData.damage;

                                TargetSeting(sampleSignals[i].thisObject, sampleSignals[j].thisObject);
                            }
                        }
                        animators[i].SetTrigger("Damage");

                        // Debug.Log($"{character.thisObject.name}��{character.attackSignal.skillData.name}��{sampleSignals[i].thisObject.name}�Ƀq�b�g���A{character.attackSignal.skillData.damage}�̃|�C���g�𓾂�");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            /// <summary>
            /// �J�����̒Ǐ]�Ώۂ�ݒ肷��
            /// </summary>
            /// <param name="Diffence">�U�����󂯂����̃I�u�W�F�N�g</param>
            /// <param name="Attack">�U���������������̃I�u�W�F�N�g</param>
            private void TargetSeting(GameObject Diffence,GameObject Attack)
            {
                targetObjectLength = setTargetObject.Count; // SetTargetObject�̕ύX�O�̑���
                if (targetObjectLength == 0)
                {
                    // �������ʂ�0�ł���Ȃ�A�o����Ǐ]�Ώۂɂ���
                    setTargetObject.Add(Diffence); 
                    setTargetObject.Add(Attack);
                }
                else
                {
                    // ���ʂ�1�𒴂��Ă��āA�ݒ肳��Ă���Ȃ�Ǐ]�Ώۂ����Ԃ��Ă��Ȃ��������m���A���Ȃ���Βǉ�����
                    for (int i = 0; i < targetObjectLength; i++)
                    {
                        if (setTargetObject[i] != Diffence)
                        {
                            setTargetObject.Add(Diffence);
                        }

                        if (setTargetObject[i] != Attack)
                        {
                            setTargetObject.Add(Attack);
                        }
                    }
                }

            }

            /// <summary>
            /// �J�����̒Ǐ]�Ώۂ𔽉f����֐�
            /// </summary>
            /// <param name="gameObjects">�Ǐ]������Ώۂ̓������z��</param>
            private void CameraPositionSeter(List<GameObject> gameObjects)
            {
                for (int i = 0;i < gameObjects.Count;i++)
                {
                    cameraController.AddTarget(gameObjects[i].transform); // �J�����Ǐ]�Ώۂɐݒ肳��Ă���I�u�W�F�N�g�����ɒǉ�
                }
                // Debug.Log("�J���������֐����퓮��");
            }

            private void AnimationPlaying(GameObject animationObject)
            {
                // Debug.Log("�A�j���[�V�����Đ��֐����퓮��");
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    if (sampleSignals[i].thisObject == animationObject)
                    {
                        animators[i].SetTrigger("Act" + sampleSignals[i].attackSignal.skillNumber);
                    }
                }
            }
        }

    }
}
