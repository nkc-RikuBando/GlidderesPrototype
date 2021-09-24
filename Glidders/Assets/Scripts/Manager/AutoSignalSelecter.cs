using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;
using Glidders.Field;
using System;
using System.Linq;

namespace Glidders
{
    namespace Manager
    {
        public class AutoSignalSelecter
        {
            CharacterCore character; // �L�����N�^�̊�b���
            CharacterData charaData; // �L�����N�^�̉Ϗ��
            IGetFieldInformation fieldInformation; // �t�B�[���h��񒲐�
            FieldIndex index; // �ŏI�I�ȓ��B�}�X�����ؗp�ɕۑ�����
            List<FieldIndexOffset> moveIndex = new List<FieldIndexOffset>(); // �ŏI�I�ȓ��B�}�X���܂Ƃ߂����X�g
            List<FieldIndexOffset> wayIndex = new List<FieldIndexOffset>();  // ���B�}�X�ɂ��ǂ蒅���܂ł̓��̂���܂Ƃ߂����X�g
            List<SkillScriptableObject> skillList; // �g�p�\�ȃX�L�����g�p�D��x���ɂ������X�g
            public AutoSignalSelecter(IGetFieldInformation fieldInformation)
            {
                this.fieldInformation = fieldInformation;
            }

            public CharacterData SignalSet(CharacterData signalSetCharaData,CharacterData mainTarget)
            {
                #region ������
                //CharacterScriptableObject character = signalSetCharaData.thisObject.GetComponent<CharacterCore>().characterScriptableObject; // �ύX�_�H

                //if (signalSetCharaData.moveSignal.moveDataArray[0] == FieldIndexOffset.zero)
                //{
                //    for (int i = 0; i < character.moveAmount; i++)
                //    {
                //        switch(Random.Range(0,4))
                //        {
                //            case 0:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.up;
                //                break;
                //            case 1:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.down;
                //                break;
                //            case 2:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.right;
                //                break;
                //            case 3:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.left;
                //                break;
                //        }
                //    }
                //}

                //if (!signalSetCharaData.attackSignal.skillData)
                //{
                //    AttackSignal attackSignal;
                //    FieldIndexOffset fieldIndexOffset = new FieldIndexOffset(1, 1);

                //    int skillRandomNumber = Random.Range(0, 3);

                //    attackSignal.skillData = character.skillDataArray[skillRandomNumber]; ;
                //    attackSignal.skillNumber = skillRandomNumber;
                //    attackSignal.isAttack = true;

                //    switch (Random.Range(0, 4))
                //    {
                //        case 0:
                //            fieldIndexOffset = FieldIndexOffset.up;
                //            break;
                //        case 1:
                //            fieldIndexOffset = FieldIndexOffset.down;
                //            break;
                //        case 2:
                //            fieldIndexOffset = FieldIndexOffset.right;
                //            break;
                //        case 3:
                //            fieldIndexOffset = FieldIndexOffset.left;
                //            break;
                //    }

                //    attackSignal.direction = fieldIndexOffset;
                //    attackSignal.selectedGrid = signalSetCharaData.index + fieldIndexOffset;

                //    signalSetCharaData.attackSignal = attackSignal;
                //}

                // signalSetCharaData.attackSignal.skillData = character.skillDataArray[skillRandomNumber];
                #endregion
                moveIndex = new List<FieldIndexOffset>();

                character = signalSetCharaData.thisObject.GetComponent<CharacterCore>();
                charaData = signalSetCharaData;
                // �łĂȂ��Z�����O�������X�g���쐬
                skillList = new List<SkillScriptableObject>(character.characterScriptableObject.skillDataArray);
                skillList.RemoveAll(x => x.energy > charaData.energy);
                skillList.OrderByDescending(x => x.damage / x.energy);
                int moveAmount = character.GetMoveAmount();

                for (int row = moveAmount * -1; row <= moveAmount;row++)
                {
                    for (int column = moveAmount * -1;column <= moveAmount;column++)
                    {
                        if (Mathf.Abs(row) + Mathf.Abs(column) > moveAmount) continue;
                        moveIndex.Add(new FieldIndexOffset(row, column));
                    }
                }

                for (int i = 0;i < moveIndex.Count;i++)
                {
                    index = moveIndex[i] + charaData.index;
                    if (!fieldInformation.IsPassingGrid(index)) continue; // �ʂ��󋵂ɂȂ��Ȃ珈���X�L�b�v

                    if (!SkillindexCheck(skillList, mainTarget,moveIndex[i])) continue; // �X�L����������Ȃ��Ȃ珈�����X�L�b�v

                    // Debug.Log($"�X�L���ݒ�(charaData.Number({charaData.playerNumber}))");

                    wayIndex = WayIndexChecker(moveIndex[i]); // �ړ��̓��̂���m�F

                    // Debug.Log($"���̂�ݒ�(charaData.Number({charaData.playerNumber}))");

                    wayIndex = GlidChecker(wayIndex); // �ړ��ł��邩�ǂ������m�F

                    if (wayIndex == null) continue;

                    for (int j = wayIndex.Count; j < Rule.maxMoveAmount;j++)
                    {
                        wayIndex.Add(FieldIndexOffset.zero);
                    }

                    charaData.moveSignal.moveDataArray = wayIndex.ToArray();


                    return charaData;
                    Debug.Log($"�ŏI���B�_(charaData.Number({charaData.playerNumber}))");
                    // if (!DamageFieldChecker(moveAmount,signalSetCharaData)) return; // �ړ�����ߒ��Ń_���[�W�t�B�[���h��������Ȃ���Ώ������X�L�b�v
                }

                return signalSetCharaData;
            }

            bool SkillindexCheck(List<SkillScriptableObject> skill,CharacterData mainTarget,FieldIndexOffset moveOffset)
            {
                FieldIndex targetIndex = mainTarget.index;
                for (int i = 0;i < skill.Count;i++)
                {
                    #region �����m�F
                    FieldIndexOffset indexOffset = new FieldIndexOffset();
                    for (int j = 0; j < 4; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                indexOffset = FieldIndexOffset.up;
                                break;
                            case 1:
                                indexOffset = FieldIndexOffset.down;
                                break;
                            case 2:
                                indexOffset = FieldIndexOffset.left;
                                break;
                            case 3:
                                indexOffset = FieldIndexOffset.right;
                                break;
                        }

                        charaData.attackSignal.direction = indexOffset;

                        for (int I = 0; I < skill[i].attackFieldIndexOffsetArray.Length; I++)
                        {
                            #region index����
                            //FieldIndexOffset directionCheck;
                            //if (charaData.attackSignal.direction == FieldIndexOffset.left)
                            //{
                            //    directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // index�Ɍ��݂�FieldIndexOffset����
                            //    directionCheck = new FieldIndexOffset(directionCheck.columnOffset, directionCheck.rowOffset); // �X�L�������̏c�Ɖ������ւ���
                            //}
                            //else if (charaData.attackSignal.direction == FieldIndexOffset.right)
                            //{
                            //    directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // index�Ɍ��݂�FieldIndexOffset����
                            //    directionCheck = new FieldIndexOffset(directionCheck.columnOffset, directionCheck.rowOffset) * -1; // �X�L�������̏c�Ɖ������ւ����̂��A���̕����ɕϊ�
                            //}
                            //else if (charaData.attackSignal.direction == FieldIndexOffset.up)
                            //{
                            //    directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // index�Ɍ��݂�FieldIndexOffset����
                            //}
                            //else
                            //{
                            //    directionCheck = skill[i].attackFieldIndexOffsetArray[I] * -1; // index�Ɍ��݂�FieldIndexOffset���������̂��A���̕����ɕϊ�
                            //}
                            #endregion

                            targetIndex = mainTarget.index;
                            for (int J = 0; J < mainTarget.moveSignal.moveDataArray.Length; J++)
                            {
                                // Debug.Log($"moveSignal({mainTarget.moveSignal.moveDataArray[i].rowOffset},{mainTarget.moveSignal.moveDataArray[i].columnOffset})");
                                targetIndex += mainTarget.moveSignal.moveDataArray[i];
                            }

                            //Debug.Log($"haraData.index + moveOffset = {charaData.index.row + moveOffset.rowOffset} , {charaData.index.column + moveOffset.columnOffset}");
                            //Debug.Log($"targetIndex = {targetIndex.row} , {targetIndex.column}");

                            if ((charaData.index + moveOffset) /*+ directionCheck*/ == targetIndex)
                            {
                                charaData.attackSignal.selectedGrid = charaData.index + indexOffset;
                                charaData.attackSignal.skillNumber = i;
                                charaData.attackSignal.skillData = skill[i];
                                return true;
                            }
                        }
                    }
                    #endregion

                }
                return false;
            }

            List<FieldIndexOffset> WayIndexChecker (FieldIndexOffset sample)
            {
                // Debug.Log($"WayIndexChecker(player{charaData.playerNumber})");
                List<FieldIndexOffset> fieldIndexOffsets = new List<FieldIndexOffset>();

                for (int i = sample.rowOffset;i != 0;)
                {
                    if (i < 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.up);
                        i++;
                    }
                    else if (i > 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.down);
                        i--;
                    }
                    else i = 0;
                }
                
                for (int i = sample.columnOffset;i != 0;)
                {
                    if (i > 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.right);
                        i--;
                    }
                    else if (i < 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.left);
                        i++;
                    }
                    else i = 0;
                }

                return fieldIndexOffsets;
            }

            List<FieldIndexOffset> GlidChecker(List<FieldIndexOffset> wayIndex)
            {
                // Debug.Log($"GlidChecker(player{charaData.playerNumber})");
                FieldIndex testIndex = FieldIndex.zero; // �ړ��}�X�����������ړ����W��ۑ�����
                List<FieldIndexOffset> fieldIndexOffsets = new List<FieldIndexOffset>(); // �l��Ԃ��p�̃��X�g
                for (int i = 0;i < wayIndex.Count;i++)
                {
                    testIndex = wayIndex[i] + charaData.index; // �L�����f�[�^�̍��W�ƍ���̒ǉ����W�������������W�ɐݒ�
                    if (!CanProceed(testIndex)) // �ʂ�邩�ǂ����̊֐��ɑΉ�
                    {
                        fieldIndexOffsets = new List<FieldIndexOffset>(); // ���X�g��������
                        continue; // �ʂ�Ȃ��̂ō���̒l���X�L�b�v����
                    }
                    else fieldIndexOffsets.Add(wayIndex[i]); // �ʂ��̂ł���Βl��Ԃ��p�̃��X�g�Ɋi�[����

                    if (wayIndex.Count <= 1) return fieldIndexOffsets; // ����ւ��鐔�������܂łȂ�l��Ԃ�
                    
                    for (int j = 0;j < wayIndex.Count;j++)
                    {
                        if (j == i) continue; // �O��Q�Ƃ����l�Ɠ����Ȃ珈�����X�L�b�v

                        testIndex += wayIndex[j]; // �ړ����W�ɍ���̈ړ��������Z

                        // �ȉ���̓����Ȃ̂Ŋ���

                        if (!CanProceed(testIndex))
                        {
                            fieldIndexOffsets = new List<FieldIndexOffset>();
                            continue;
                        }
                        else fieldIndexOffsets.Add(wayIndex[j]);

                        if (wayIndex.Count <= 2) return fieldIndexOffsets;

                        for (int I = 0;I < wayIndex.Count;I++)
                        {
                            if (I == i || I == j) continue;

                            testIndex += wayIndex[I];
                            if (!CanProceed(testIndex))
                            {
                                fieldIndexOffsets = new List<FieldIndexOffset>();
                                continue;
                            }
                            else fieldIndexOffsets.Add(wayIndex[I]);

                            if (wayIndex.Count <= 3) return fieldIndexOffsets;

                            for (int J = 0;J < wayIndex.Count;J++)
                            {
                                if (J == i || J == j || J == I) continue;

                                testIndex += wayIndex[J];
                                if (!CanProceed(testIndex))
                                {
                                    fieldIndexOffsets = new List<FieldIndexOffset>();
                                    continue;
                                }
                                else fieldIndexOffsets.Add(wayIndex[J]);

                                return fieldIndexOffsets;
                            }
                        }
                    }
                }

                return wayIndex;

                bool CanProceed(FieldIndex testIndex)
                {
                    // Debug.Log("�i�s�s��");
                    if (!fieldInformation.IsPassingGrid(testIndex)) return false; // �i�����Ƃ��Ă���悪�i�s�s�Ȃ��false��Ԃ�
                    // Debug.Log("�i�s��/�ʍs�s��");
                    if (fieldInformation.GetDamageFieldOwner(testIndex) != charaData.playerNumber && fieldInformation.GetDamageFieldOwner(testIndex) != 0) return false; // �i�����Ƃ��Ă����Ɏ����ȊO�̃_���[�W�t�B�[���h�����݂���Ȃ�false��Ԃ�
                    
                    return true; // ��L�̏����ɊY�����Ȃ��ꍇ�i�s�\�Ƃ݂Ȃ�true��Ԃ�
                }
            }
        }
    }
}