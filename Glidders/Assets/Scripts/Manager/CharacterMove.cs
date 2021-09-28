using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Field;
using Glidders.Graphic;
using DG.Tweening;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        struct CollsionDataBox
        {
            public bool collsionList;
            public FieldIndex defalutIndex;
            public FieldIndex collisionIndex;
            public FieldIndexOffset collisionDirection;
        }

        public delegate void tweenList();
        public class CharacterMove
        {
            // �萔
            const int DAMAGEFIELD_DAMAGE = 2000; // �_���[�W�t�B�[���h�𓥂񂾍ۂɔ�������_���[�W��
            const float TWEEN_MOVETIME = 0.3f; // Dotween�ɂ�鋓���ɂ����鎞��

            private Vector3[] targetPosition = new Vector3[ActiveRule.playerCount]; // �ڕW�n�_��ۑ�����ϐ�
            private FieldIndexOffset[] thisMoveOffset = new FieldIndexOffset[ActiveRule.playerCount]; // �I�u�W�F�N�g�̈ړ���
            private IGetFieldInformation getFieldInformation; // FieldCore�̃C���^�[�t�F�[�X
            private CharacterDirection[] characterDirections; // �e�L�����N�^�̌����ύX�N���X

            private bool[] moveList = new bool[ActiveRule.playerCount]; // �����邩�ǂ�����Character���ƂɊǗ�����
            private Text[] texts = new Text[ActiveRule.playerCount];
            private Animator[] animators;
            private CollsionDataBox[] collisionList = new CollsionDataBox[ActiveRule.playerCount];

            public CharacterMove(IGetFieldInformation getInfo,CharacterDirection[] directions,Text[] texts,Animator[] animators)
            {
                // �R���X�g���N�^��GetComoponent���Ă���I�u�W�F�N�g���擾
                characterDirections = directions;
                getFieldInformation = getInfo;
                this.texts = texts;
                this.animators = animators;

                for (int i = 0;i < moveList.Length;i++)
                {
                    // �����邩�ǂ����̕ϐ���S��true�ɂ���
                    moveList[i] = true;
                }
            }

            [PunRPC]
            public IEnumerator MoveOrder(CharacterData[] characterDatas, Action phaseCompleteAction)
            {
                // �e�v���C���[�̈ړ��������ƂɁA�t�F�[�Y���Ƃ̈ړ������s

                for (int i = 0; i < Rule.maxMoveAmount;i++)
                {
                    for (int j =0;j < characterDatas.Length;j++)
                    {
                        collisionList[j].defalutIndex = characterDatas[j].index; // �L�����̃f�t�H���g�̈ʒu��ݒ�
                        collisionList[j].collsionList = true;

                        moveList[j] = false;�@// �����邩�ǂ����@���@false�@�ɂ���

                        thisMoveOffset[j] = characterDatas[j].moveSignal.moveDataArray[i]; // ���̈ړ��Ɏg��FieldIndexOffset��ۑ�����

                        // Debug.Log($"���݈ʒu({characterDatas[j].index.row} , {characterDatas[j].index.column})  �ړ���({thisMoveOffset.rowOffset.ToString()},{thisMoveOffset.columnOffset.ToString()})");

                        characterDatas[j].index += thisMoveOffset[j]; // �C���f�b�N�X�̈ʒu����������

                        // Debug.Log($"{characterDatas[j].thisObject.name} �� FieldIndex��{characterDatas[j].index.row} , {characterDatas[j].index.column}");

                        // �t�B�[���h���𔻒肵�A�ړ��悪�i�s�s�\�G���A�ł���ꍇ�A�ړ����X�L�b�v����
                        if (!getFieldInformation.IsPassingGrid(characterDatas[j].index))
                        {
                            // Debug.Log($"{characterDatas[j].playerName}��index��{characterDatas[j].index.row},{characterDatas[j].index.column}�̂��ߐi�s���~���܂���");
                            characterDatas[j].index -= thisMoveOffset[j]; // �C���f�b�N�X�ɑ΂��čs�����ύX�����ɖ߂�
                            Stay(j); // ����̈ړ��͂��Ȃ����Ƃ𖽗�
                            continue;
                        }

                        targetPosition[j] = getFieldInformation.GetTilePosition(characterDatas[j].index); // �C���f�b�N�X���W��Vector3�ɏ���������

                        // Debug.Log($"targetPosition��({targetPosition.x},{targetPosition.y} Index��({characterDatas[j].index.row},{characterDatas[j].index.column})");
                    }


                    // �Փ˂��Ă��邩�ǂ����𔻒肷��֐�
                    CollisionObject();

                    for (int j = 0; j < characterDatas.Length; j++)
                    {
                        characterDirections[j].SetDirection(thisMoveOffset[j]); // �����ϊ��̎w�߂��o��

                        // �ړ����W�����ɂ��̈ړ��֐����Ăяo��
                        if (!collisionList[j].collsionList) CollsionMove(characterDatas[j].thisObject,j,collisionList[j]);
                        else if (TeleportChecker(thisMoveOffset[j])) Teleport(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.up) MoveUp(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.down) MoveDown(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.left) MoveLeft(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.right) MoveRight(characterDatas[j].thisObject, j);
                        else Stay(j);
                    }

                    GlidChecker();
                    TextMove();

                    // Tween�ɂ����鎞�ԁ@�������́@Tween�������I�������R���[�`�����~����
                    while (!ListChecker(moveList)) 
                    {
                        yield return new WaitForSeconds(TWEEN_MOVETIME);
                    }

                    for (int j = 0;j < texts.Length;j++)
                    {
                        texts[j].text = "";
                    }

                }

                phaseCompleteAction();

                void TextMove()
                {
                    for (int i = 0;i < texts.Length;i++)
                    {
                        texts[i].rectTransform.localScale = new Vector3(-1, 1, 1);
                    }
                }

                bool ListChecker(bool[] moveList)
                {
                    for (int i = 0;i < moveList.Length;i++)
                    {
                        if (!moveList[i]) return false;
                    }

                    return true;
                }

                #region ���[�J���֐�
                bool TeleportChecker(FieldIndexOffset index)
                {
                    if (index.rowOffset > 1 || index.rowOffset < -1 || index.columnOffset > 1 ||index.columnOffset < -1) return true; // �ړ��ʂ�2�ȏ�ł���Ȃ�΃e���|�[�g�ړ��ɐ؂�ւ���
                    return false;
                }

                void Stay(int j)
                {
                    // �ړ��֘A���s�킸�A�ړ��ϐ���true�ɂ���
                    moveList[j] = true;
                }

                void CollsionMove(GameObject thisObject, int j,CollsionDataBox collsionData)
                {
                    Vector3 defaultPos = getFieldInformation.GetTilePosition(collsionData.defalutIndex);
                    Vector3 collisionPos = getFieldInformation.GetTilePosition(collsionData.collisionIndex);

                    targetPosition[j] = (defaultPos + collisionPos) / 2;

                    Sequence sequence = DOTween.Sequence().
                    Append(thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME / 2).SetEase(Ease.Linear)).
                    AppendCallback(() => characterDirections[j].SetDirection(collisionList[j].collisionDirection)).
                    AppendCallback(() => animators[j].SetTrigger("Damage")).
                    Append(thisObject.transform.DOMove(defaultPos, TWEEN_MOVETIME / 2).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true));
                }

                #region �e�ړ��Ɋ֘A����֐�
                void MoveUp(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveDown(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveLeft(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveRight(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void Teleport(GameObject thisObject,int j)
                {
                    // ���W�𒼐ڏ������A�ړ��ϐ���true�ɂ���
                    thisObject.transform.position = targetPosition[j];
                    moveList[j] = true;
                }
                #endregion

                // �Փ˂��Ă��邩�𔻒肷��֐�
                void CollisionObject()
                {
                    for (int j = 0; j < characterDatas.Length; j++)
                    {
                        if (!characterDatas[j].canAct) continue; // �L�����N�^���s���s�\�ł���Ȃ�Ώ������X�L�b�v

                        for (int I = 0; I < characterDatas.Length; I++)
                        {
                            if (I == j) continue; // �Q�Ƃ���I�u�W�F�N�g�����Ԃ邽�ߏ������X�L�b�v

                            // Debug.Log("j = " + characterDatas[j].index.column.ToString() + characterDatas[j].index.row.ToString() + "; I = " + characterDatas[I].index.column.ToString()  + characterDatas[I].index.row.ToString());
                            if (characterDatas[j].index == characterDatas[I].index)
                            {
                                // Debug.Log(characterDatas[j].thisObject.name + "��" + characterDatas[I].thisObject.name + "�͂Ԃ�����");

                                collisionList[j].collsionList = false;
                                collisionList[I].collsionList = false;
                                collisionList[j].collisionIndex = characterDatas[j].index;
                                collisionList[I].collisionIndex = characterDatas[I].index;
                                collisionList[j].collisionDirection = thisMoveOffset[j];
                                collisionList[I].collisionDirection = thisMoveOffset[I];

                                characterDatas[j].index = collisionList[j].defalutIndex;
                                characterDatas[I].index = collisionList[I].defalutIndex;


                                // �Փ˂��Ă���Ȃ�΁A�Ώۂ̓�̃I�u�W�F�N�g�ɑ΂��āA�ړ��ʂ�S��0�ɏ��������������ōs���s�\�ɂ���
                                for (int J = 0; J < Rule.maxMoveAmount; J++)
                                {
                                    characterDatas[j].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                                for (int J = 0; J < Rule.maxMoveAmount; J++)
                                {
                                    characterDatas[I].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                                characterDatas[j].canAct = false;
                                characterDatas[I].canAct = false;
                            }
                        }
                    }
                }

                // phaseCompleteAction();

                // ���̃O���b�h�̃t�B�[���h�󋵂𔻒肷��֐�(���݂̓_���[�W�t�B�[���h�̂�)
                void GlidChecker()
                {
                    for (int i = 0;i < characterDatas.Length;i++)
                    {
                        int owner = getFieldInformation.GetDamageFieldOwner(characterDatas[i].index); // i�Ԗڂ̃L������index���擾���A���̏ꏊ�̃_���[�W�t�B�[���h�̎���擾
                        if (i != owner && owner >= 0) // �傪�����ł͂Ȃ��A�������Ƀ_���[�W�t�B�[���h������ꍇ�Ƀ_���[�W���󂯂鏈����ǉ�
                        {
                            // Debug.Log($"index({characterDatas[i].index.row},{characterDatas[i].index.column})�̃I�[�i�[��{owner}");
                            characterDatas[i].point -= DAMAGEFIELD_DAMAGE;
                            characterDatas[owner].point += DAMAGEFIELD_DAMAGE;
                            texts[i].text = DAMAGEFIELD_DAMAGE.ToString();
                            // Debug.Log($"{characterDatas[i].playerName}��{characterDatas[owner].playerName}�̃_���[�W�t�B�[���h�𓥂�ł��܂���");
                        }
                    }
                }
                #endregion
                // �ړ���}�X�ڏ󋵂̔���
            }

        }
    }
}