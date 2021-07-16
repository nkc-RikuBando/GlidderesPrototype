using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;
using DG.Tweening;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        public delegate void tweenList();
        public class CharacterMove
        {
            // �萔
            const int PLAYER_AMOUNT = 4; // �v���C���[�̑���
            const int PLAYER_MOVEAMOUNT_MAX = 5; // �e��L�����N�^�[�����̈ړ���
            const float TWEEN_MOVETIME = 0.5f; // Dotween�ɂ�鋓���ɂ����鎞��

            private static Vector3 targetPosition; // �ڕW�n�_��ۑ�����ϐ�
            private FieldIndexOffset thisMoveOffset; // �I�u�W�F�N�g�̈ړ���
            private IGetFieldInformation getFieldInformation; // FieldCore�̃C���^�[�t�F�[�X
            private ISetFieldInformation setFieldInformation; // FieldCore�̃C���^�[�t�F�[�X
            private CharacterDirection[] characterDirections; // �e�L�����N�^�̌����ύX�N���X

            private bool[] moveList = new bool[PLAYER_AMOUNT]; // �����邩�ǂ�����Character���ƂɊǗ�����

            public CharacterMove(IGetFieldInformation getInfo,ISetFieldInformation setInfo,CharacterDirection[] directions)
            {
                // �R���X�g���N�^��GetComoponent���Ă���I�u�W�F�N�g���擾
                characterDirections = directions;
                getFieldInformation = getInfo; 
                setFieldInformation = setInfo; 

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

                for (int i = 0; i < PLAYER_MOVEAMOUNT_MAX;i++)
                {
                    for (int j =0;j < characterDatas.Length;j++)
                    {
                        moveList[j] = false;�@// �����邩�ǂ����@���@false�@�ɂ���

                        thisMoveOffset = characterDatas[j].moveSignal.moveDataArray[i]; // ���̈ړ��Ɏg��FieldIndexOffset��ۑ�����

                        // Debug.Log($"���݈ʒu({characterDatas[j].index.row} , {characterDatas[j].index.column})  �ړ���({thisMoveOffset.rowOffset.ToString()},{thisMoveOffset.columnOffset.ToString()})");

                        characterDatas[j].index += thisMoveOffset; // �C���f�b�N�X�̈ʒu����������

                        Debug.Log($"{characterDatas[j].thisObject.name} �� FieldIndex��{characterDatas[j].index.row} , {characterDatas[j].index.column}");

                        // �t�B�[���h���𔻒肵�A�ړ��悪�i�s�s�\�G���A�ł���ꍇ�A�ړ����X�L�b�v����
                        if (!getFieldInformation.IsPassingGrid(characterDatas[j].index))
                        {
                            Debug.Log($"{characterDatas[j].playerName}��index��{characterDatas[j].index.row},{characterDatas[j].index.column}�̂��ߐi�s���~���܂���");
                            characterDatas[j].index -= thisMoveOffset; // �C���f�b�N�X�ɑ΂��čs�����ύX�����ɖ߂�
                            Stay(j); // ����̈ړ��͂��Ȃ����Ƃ𖽗�
                            continue;
                        }

                        targetPosition = getFieldInformation.GetTilePosition(characterDatas[j].index); // �C���f�b�N�X���W��Vector3�ɏ���������

                        characterDirections[j].SetDirection(thisMoveOffset); // �����ϊ��̎w�߂��o��

                        // Debug.Log($"targetPosition��({targetPosition.x},{targetPosition.y} Index��({characterDatas[j].index.row},{characterDatas[j].index.column})");

                        // �ړ����W�����ɂ��̈ړ��֐����Ăяo��
                        if (TeleportChecker(thisMoveOffset)) Teleport(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.up) MoveUp(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.down) MoveDown(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.left) MoveLeft(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.right) MoveRight(characterDatas[j].thisObject, j);
                        else Stay(j);
                    }

                    // Tween�ɂ����鎞�ԁ@�������́@Tween�������I�������R���[�`�����~����
                    while (!moveList[0] || !moveList[1] || !moveList[2] || !moveList[3])
                    {
                        yield return new WaitForSeconds(TWEEN_MOVETIME);
                    }

                    // �Փ˂��Ă��邩�ǂ����𔻒肷��֐�
                    CollisionObject();
                }

                for (int i = 0; i < characterDatas.Length; i++)
                {
                    GlidChecker();
                    // Debug.Log($"�ŏI�ړ��n�_({characterDatas[i].index.row}{characterDatas[i].index.column})");
                    // Field�ɑ΂��ăC���f�b�N�X��Ԃ�
                    // setFieldInformation.SetPlayerPosition(i, characterDatas[i].index); // �ŏI���W��Field�ɕԋp����
                }
                
                phaseCompleteAction();

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

                void MoveUp(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveDown(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveLeft(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveRight(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void Teleport(GameObject thisObject,int j)
                {
                    // ���W�𒼐ڏ������A�ړ��ϐ���true�ɂ���
                    thisObject.transform.position = targetPosition;
                    moveList[j] = true;
                }

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

                                // �Փ˂��Ă���Ȃ�΁A�Ώۂ̓�̃I�u�W�F�N�g�ɑ΂��āA�ړ��ʂ�S��0�ɏ��������������ōs���s�\�ɂ���
                                for (int J = 0; J < PLAYER_MOVEAMOUNT_MAX; J++)
                                {
                                    characterDatas[j].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                                for (int J = 0; J < PLAYER_MOVEAMOUNT_MAX; J++)
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

                void GlidChecker()
                {
                    // �t�B�[���h���𔻒肷��֐��ł�
                }
                #endregion
                // �ړ���}�X�ڏ󋵂̔���
            }

        }
    }
}