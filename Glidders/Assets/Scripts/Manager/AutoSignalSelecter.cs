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
            bool cantMove;
            IGetFieldInformation fieldInformation; // �t�B�[���h��񒲐�
            UniqueSkillScriptableObject nonSkill;
            FieldIndex index; // �ŏI�I�ȓ��B�}�X�����ؗp�ɕۑ�����
            List<FieldIndexOffset> moveIndex = new List<FieldIndexOffset>(); // �ŏI�I�ȓ��B�}�X���܂Ƃ߂����X�g
            List<FieldIndexOffset> wayIndex = new List<FieldIndexOffset>();  // ���B�}�X�ɂ��ǂ蒅���܂ł̓��̂���܂Ƃ߂����X�g
            List<UniqueSkillScriptableObject> skillList; // �g�p�\�ȃX�L�����g�p�D��x���ɂ������X�g
            List<UniqueSkillScriptableObject> nonAttackSkillList;
            FieldIndexOffset randomIndex;
            const int KAITO_RANDOM = 10;
            const int SEIRA_RANDOM = 10;
            const int YU_RANDOM = 7;
            const int MITSUHA_RANDOM = 3;
            const int INDEX_RANDOM = 2;
            public AutoSignalSelecter(IGetFieldInformation fieldInformation,UniqueSkillScriptableObject nonSkill)
            {
                this.fieldInformation = fieldInformation;
                this.nonSkill = nonSkill;
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
                moveIndex = new List<FieldIndexOffset>(0);
                wayIndex = new List<FieldIndexOffset>(0);

                character = signalSetCharaData.thisObject.GetComponent<CharacterCore>();
                charaData = signalSetCharaData;
                // �łĂȂ��Z�����O�������X�g���쐬
                skillList = new List<UniqueSkillScriptableObject>(character.characterScriptableObject.skillDataArray);
                skillList.Add(character.characterScriptableObject.uniqueSkillData);
                skillList.RemoveAll(x => x.energy > charaData.energy);
                //skillList.RemoveAll(x => x.skillType != SkillTypeEnum.ATTACK);
                skillList.OrderByDescending(x => x.damage * (1 + (x.power * x.power) / 10) / (1 + (x.energy * x.energy) / 10));
                nonAttackSkillList = new List<UniqueSkillScriptableObject>(character.characterScriptableObject.skillDataArray);
                nonAttackSkillList.Add(character.characterScriptableObject.uniqueSkillData);
                nonAttackSkillList.RemoveAll(x => x.skillType == SkillTypeEnum.ATTACK);

                //for (int i = 0; i < skillList.Count;i++)
                //{
                //    Debug.Log($"skill({i}) is ({skillList[i].skillName})");
                //}

                FieldIndex targetIndex = mainTarget.index;

                for (int i = 0; i < mainTarget.moveSignal.moveDataArray.Length; i++)
                {
                    targetIndex += mainTarget.moveSignal.moveDataArray[i];
                }

                int moveAmount = character.GetMoveAmount();

                int random_skillSkip = UnityEngine.Random.Range(1,10);
                int random_skillArray = UnityEngine.Random.Range(1, 5);

                for (int i = 0; i < charaData.buffView.Count; i++)
                {
                    for (int j = 0; j < charaData.buffValue[i].Count; j++)
                    {
                        if (charaData.buffValue[i][j].buffedStatus == StatusTypeEnum.MOVE)
                        {
                            if (charaData.buffValue[i][j].buffType == Buff.BuffTypeEnum.PLUS) moveAmount += (int)charaData.buffValue[i][j].buffScale;
                            else if (charaData.buffValue[i][j].buffType == Buff.BuffTypeEnum.MULTIPLIED) moveAmount *= (int)charaData.buffValue[i][j].buffScale;
                        }
                    }
                }

                if (random_skillArray != 1)
                {
                    for (int row = moveAmount * -1; row <= moveAmount; row++)
                    {
                        for (int column = moveAmount * -1; column <= moveAmount; column++)
                        {
                            if (Mathf.Abs(row) + Mathf.Abs(column) > moveAmount) continue;
                            moveIndex.Add(new FieldIndexOffset(row, column));
                        }
                    }
                }
                else
                {
                    Debug.Log("�s�����ύX");
                    for (int column = moveAmount * -1;column <= moveAmount;column++)
                    {
                        for (int row = moveAmount * -1; row <= moveAmount;row++)
                        {
                            if (Mathf.Abs(row) + Mathf.Abs(column) > moveAmount) continue;
                            moveIndex.Add(new FieldIndexOffset(row, column));
                        }
                    }
                }

                int rabdom_randomMove = UnityEngine.Random.Range(1, 20);

                if (rabdom_randomMove < 4)
                {
                    Debug.Log("�����_���s��");
                    List<FieldIndexOffset> addIndex = new List<FieldIndexOffset>();
                    FieldIndex testIndex = charaData.index;
                    charaData.moveSignal.moveDataArray = new FieldIndexOffset[Rule.maxMoveAmount];
                    for (int i = 0; i < moveAmount;i++)
                    {
                        FieldIndexOffset indexOffset = new FieldIndexOffset();
                        switch (UnityEngine.Random.Range(0,4))
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

                        addIndex.Add(indexOffset);
                        if (i != 0)
                        {
                            if (indexOffset.rowOffset * -1 == addIndex[i - 1].rowOffset || indexOffset.columnOffset * -1 == addIndex[i - 1].columnOffset)
                            {
                                addIndex = new List<FieldIndexOffset>();
                                continue;
                            }
                        }

                        for (int j = 0;j < addIndex.Count;j++)
                        {
                            testIndex += addIndex[i];
                        }

                        if (testIndex == mainTarget.index)
                        {
                            addIndex = new List<FieldIndexOffset>();
                            testIndex = charaData.index;
                            i = 0;
                            continue;
                        }
                    }

                    for (int i = addIndex.Count;i < Rule.maxMoveAmount;i++)
                    {
                        addIndex.Add(FieldIndexOffset.zero);
                    }

                    charaData.moveSignal.moveDataArray = addIndex.ToArray();
                    charaData.attackSignal.skillData = nonSkill;
                    charaData.attackSignal.isAttack = false;

                    return charaData;
                }

                for (int i = 0;i < moveIndex.Count;i++)
                {
                    if (random_skillSkip < 5 - charaData.energy) break;

                    index = moveIndex[i] + charaData.index;
                    if (index.row < 0 || index.row > 8 || index.column < 0 || index.column > 8) continue; // �ړ��悪��ʊO�ł���Ȃ珈�����X�L�b�v
                    if (!fieldInformation.IsPassingGrid(index)) continue; // �ʂ��󋵂ɂȂ��Ȃ珈���X�L�b�v

                    if (!SkillindexCheck(skillList, mainTarget,moveIndex[i])) continue; // �X�L����������Ȃ��Ȃ珈�����X�L�b�v

                    // Debug.Log($"�X�L���ݒ�(charaData.Number({charaData.playerNumber}))");

                    wayIndex = WayIndexChecker(moveIndex[i]); // �ړ��̓��̂���m�F

                    // Debug.Log($"���̂�ݒ�(charaData.Number({charaData.playerNumber}))");

                    wayIndex = GlidChecker(wayIndex); // �ړ��ł��邩�ǂ������m�F

                    if (cantMove) continue;

                    for (int j = wayIndex.Count; j < Rule.maxMoveAmount;j++)
                    {
                        wayIndex.Add(FieldIndexOffset.zero);
                    }

                    if (charaData.attackSignal.skillData.moveType == UniqueSkillMoveType.NONE)
                    {
                        for (int I = 0; I < charaData.moveSignal.moveDataArray.Length; I++)
                        {
                            wayIndex[I] = FieldIndexOffset.zero;
                        }

                    }

                    if (charaData.characterName == CharacterName.YU)
                    {
                        if (UnityEngine.Random.Range(1, 10) <= YU_RANDOM && charaData.energy > character.characterScriptableObject.skillDataArray[0].energy)
                        {
                            charaData.attackSignal.skillData = character.characterScriptableObject.skillDataArray[0];
                            charaData.attackSignal.direction = FieldIndexOffset.down;
                            charaData.attackSignal.isAttack = true;
                            charaData.attackSignal.selectedGrid = charaData.index;
                            charaData.attackSignal.skillNumber = 1;
                        }

                    }
                    else if (charaData.characterName == CharacterName.MITSUHA)
                    {
                        if (UnityEngine.Random.Range(1, 10) <= MITSUHA_RANDOM)
                        {
                            List<FieldIndexOffset> offsetList = new List<FieldIndexOffset>();
                            if (charaData.energy >= character.characterScriptableObject.uniqueSkillData.energy)
                            {
                                FieldIndexOffset offset = targetIndex - charaData.index;
                                if (offset == FieldIndexOffset.down || offset == FieldIndexOffset.left || offset == FieldIndexOffset.right || offset == FieldIndexOffset.up)
                                {
                                    charaData.attackSignal.skillData = character.characterScriptableObject.uniqueSkillData;
                                    charaData.attackSignal.direction = FieldIndexOffset.down;
                                    charaData.attackSignal.isAttack = true;
                                    charaData.attackSignal.skillNumber = 4;
                                    if (charaData.index.row - 2 > 0) offsetList.Add(FieldIndexOffset.up);
                                    if (charaData.index.row + 2 < 8) offsetList.Add(FieldIndexOffset.down);
                                    if (charaData.index.column - 2 > 0) offsetList.Add(FieldIndexOffset.left);
                                    if (charaData.index.column + 2 < 8) offsetList.Add(FieldIndexOffset.right);

                                    for (int I = 0; I < Rule.maxMoveAmount; I++)
                                    {
                                        if (I < 2) wayIndex[I] = offsetList[0];
                                        else wayIndex[I] = FieldIndexOffset.zero;
                                    }
                                }
                            }
                        }
                    }

                    if (randomIndex != FieldIndexOffset.zero) Debug.Log($"�U�����W��({randomIndex.rowOffset},{randomIndex.columnOffset})���ꂽ");

                    charaData.moveSignal.moveDataArray = wayIndex.ToArray();
                    return charaData;
                }

                for (int i = 0;i < moveAmount;i++)
                {
                    if (targetIndex.column > signalSetCharaData.index.column) wayIndex.Add(FieldIndexOffset.right);
                    else if (targetIndex.column < signalSetCharaData.index.column) wayIndex.Add(FieldIndexOffset.left);
                    else if (targetIndex.row > signalSetCharaData.index.row) wayIndex.Add(FieldIndexOffset.down);
                    else if (targetIndex.row < signalSetCharaData.index.row) wayIndex.Add(FieldIndexOffset.up);
                }
                for (int i = moveAmount; i <= Rule.maxMoveAmount;i++)
                {
                    wayIndex.Add(FieldIndexOffset.zero);
                }

                if (charaData.characterName == CharacterName.YU)
                {
                    for (int i = 0;i < charaData.buffView.Count;i++)
                    {
                        if (charaData.buffView[i] == character.characterScriptableObject.skillDataArray[0].giveBuff[0] || charaData.energy < character.characterScriptableObject.skillDataArray[0].energy)
                        {
                            charaData.attackSignal.skillData = nonSkill;
                            charaData.attackSignal.isAttack = false;
                            if (i == charaData.buffView.Count - 1) break;
                        }

                        if (i == charaData.buffView.Count -1)
                        {
                            charaData.attackSignal.skillData = character.characterScriptableObject.skillDataArray[0];
                            charaData.attackSignal.direction = FieldIndexOffset.down;
                            charaData.attackSignal.isAttack = true;
                            charaData.attackSignal.selectedGrid = charaData.index;
                            charaData.attackSignal.skillNumber = 1;
                        }
                    }

                    if (charaData.buffView.Count == 0 && charaData.energy > character.characterScriptableObject.skillDataArray[0].energy)
                    {
                        charaData.attackSignal.skillData = character.characterScriptableObject.skillDataArray[0];
                        charaData.attackSignal.direction = FieldIndexOffset.down;
                        charaData.attackSignal.isAttack = true;
                        charaData.attackSignal.selectedGrid = charaData.index;
                        charaData.attackSignal.skillNumber = 1;
                    }
                }
                else if (charaData.characterName == CharacterName.KAITO)
                {
                    if (UnityEngine.Random.Range(1, 10) <= KAITO_RANDOM && charaData.energy > character.characterScriptableObject.uniqueSkillData.energy)
                    {
                        charaData.attackSignal.skillData = character.characterScriptableObject.uniqueSkillData;
                        charaData.attackSignal.direction = FieldIndexOffset.down;
                        charaData.attackSignal.isAttack = true;
                        charaData.attackSignal.selectedGrid = charaData.index;
                        charaData.attackSignal.skillNumber = 4;
                    }

                    for (int j = 0; j < charaData.buffView.Count; j++)
                    {
                        if (charaData.buffView[j] == character.characterScriptableObject.uniqueSkillData.giveBuff[0])
                        {
                            charaData.attackSignal.skillData = nonSkill;
                            charaData.attackSignal.isAttack = false;
                        }
                    }
                }
                else if (charaData.characterName == CharacterName.SEIRA)
                {
                    if (UnityEngine.Random.Range(1, 10) <= SEIRA_RANDOM && charaData.energy > character.characterScriptableObject.skillDataArray[2].energy)
                    {
                        charaData.attackSignal.skillData = character.characterScriptableObject.skillDataArray[2];
                        charaData.attackSignal.direction = FieldIndexOffset.down;
                        charaData.attackSignal.isAttack = true;
                        charaData.attackSignal.selectedGrid = charaData.index;
                        charaData.attackSignal.skillNumber = 3;
                    }

                    for (int j = 0; j < charaData.buffView.Count; j++)
                    {
                        if (charaData.buffView[j] == character.characterScriptableObject.skillDataArray[2].giveBuff[0])
                        {
                            charaData.attackSignal.skillData = nonSkill;
                            charaData.attackSignal.isAttack = false;
                        }
                    }

                }

                if (charaData.attackSignal.skillData == null)
                {
                    charaData.attackSignal.skillData = nonSkill;
                    charaData.attackSignal.isAttack = false;
                }

                charaData.moveSignal.moveDataArray = wayIndex.ToArray();
                return charaData;
            }

            bool SkillindexCheck(List<UniqueSkillScriptableObject> skill,CharacterData mainTarget,FieldIndexOffset moveOffset)
            {
                FieldIndex targetIndex = mainTarget.index;
                FieldIndexOffset testIndexOffset = moveOffset;
                int fixedMoveAmount = 0;
                for (int i = 0;i < skill.Count;i++)
                {
                    if (skill[i].skillType == SkillTypeEnum.SUPPORT) continue;
                    targetIndex = mainTarget.index;
                    testIndexOffset = moveOffset;
                    if (UnityEngine.Random.Range(1, 10) <= INDEX_RANDOM)
                    {
                        randomIndex = FieldIndexOffset.zero;
                        switch (UnityEngine.Random.Range(0,4))
                        {
                            case 0:
                                randomIndex = FieldIndexOffset.down;
                                break;
                            case 1:
                                randomIndex = FieldIndexOffset.up;
                                break;
                            case 2:
                                randomIndex = FieldIndexOffset.left;
                                break;
                            case 3:
                                randomIndex = FieldIndexOffset.right;
                                break;
                        }
                        targetIndex += randomIndex;
                    }

                    for (int j = 0; j < mainTarget.moveSignal.moveDataArray.Length; j++)
                    {
                        // Debug.Log($"moveSignal({mainTarget.moveSignal.moveDataArray[i].rowOffset},{mainTarget.moveSignal.moveDataArray[i].columnOffset})");
                        targetIndex += mainTarget.moveSignal.moveDataArray[j];
                    }

                    //if (skill[i].moveType == UniqueSkillMoveType.FIXED) fixedMoveAmount = skill[i].moveFieldIndexOffsetArray[0].rowOffset + skill[i].moveFieldIndexOffsetArray[0].columnOffset;

                    #region �����m�F
                    FieldIndexOffset indexOffset = new FieldIndexOffset();
                    for (int j = 0; j < skill[i].selectFieldIndexOffsetArray.Length; j++)
                    {
                        indexOffset = skill[i].selectFieldIndexOffsetArray[j];

                        charaData.attackSignal.direction = indexOffset;

                        for (int I = 0; I < skill[i].attackFieldIndexOffsetArray.Length; I++)
                        {
                            #region index����
                            FieldIndexOffset directionCheck;
                            if (charaData.attackSignal.direction == FieldIndexOffset.left)
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // index�Ɍ��݂�FieldIndexOffset����
                                directionCheck = new FieldIndexOffset(directionCheck.columnOffset, directionCheck.rowOffset); // �X�L�������̏c�Ɖ������ւ���
                            }
                            else if (charaData.attackSignal.direction == FieldIndexOffset.right)
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // index�Ɍ��݂�FieldIndexOffset����
                                directionCheck = new FieldIndexOffset(directionCheck.columnOffset, directionCheck.rowOffset) * -1; // �X�L�������̏c�Ɖ������ւ����̂��A���̕����ɕϊ�
                            }
                            else if (charaData.attackSignal.direction == FieldIndexOffset.up)
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // index�Ɍ��݂�FieldIndexOffset����
                            }
                            else
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I] * -1; // index�Ɍ��݂�FieldIndexOffset���������̂��A���̕����ɕϊ�
                            }
                            #endregion


                            if (skill[i].moveType == UniqueSkillMoveType.NONE)
                            {
                                testIndexOffset = FieldIndexOffset.zero;
                            }
                            else if (skill[i].moveType == UniqueSkillMoveType.FIXED)
                            {
                                testIndexOffset = indexOffset * fixedMoveAmount;
                                if (!fieldInformation.IsPassingGrid(charaData.index + testIndexOffset)) continue;
                            }

                            //Debug.Log($"haraData.index + moveOffset = {charaData.index.row + moveOffset.rowOffset} , {charaData.index.column + moveOffset.columnOffset} ({directionCheck.rowOffset},{directionCheck.columnOffset})");
                            //Debug.Log($"targetIndex = {targetIndex.row} , {targetIndex.column}");

                            if (skill[i].skillType == SkillTypeEnum.ATTACK)
                            {
                                if ((charaData.index + testIndexOffset) + directionCheck + indexOffset == targetIndex)
                                {
                                    charaData.attackSignal.selectedGrid = charaData.index + indexOffset + testIndexOffset;
                                    charaData.attackSignal.skillNumber = i + 1; 
                                    charaData.attackSignal.skillData = skill[i];
                                    charaData.attackSignal.isAttack = true;
                                    return true;
                                }
                            }
                            //else if (skill[i].skillType == SkillTypeEnum.SUPPORT)
                            //{
                            //    charaData.attackSignal.selectedGrid = charaData.index + indexOffset;
                            //    charaData.attackSignal.skillNumber = i;
                            //    charaData.attackSignal.skillData = skill[i];
                            //    charaData.attackSignal.isAttack = true;
                            //    return true;
                            //}
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
                cantMove = false;
                FieldIndex testIndex = FieldIndex.zero; // �ړ��}�X�����������ړ����W��ۑ�����
                List<FieldIndexOffset> fieldIndexOffsets = new List<FieldIndexOffset>(); // �l��Ԃ��p�̃��X�gList<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
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

                cantMove = true;
                return wayIndex;

                bool CanProceed(FieldIndex testIndex_)
                {
                    // Debug.Log("�i�s�s��");
                    if (!fieldInformation.IsPassingGrid(testIndex_)) return false; // �i�����Ƃ��Ă���悪�i�s�s�Ȃ��false��Ԃ�
                    // Debug.Log("�i�s��/�ʍs�s��");
                    if (fieldInformation.GetDamageFieldOwner(testIndex_) != charaData.playerNumber && fieldInformation.GetDamageFieldOwner(testIndex_) >= 0) return false; // �i�����Ƃ��Ă����Ɏ����ȊO�̃_���[�W�t�B�[���h�����݂���Ȃ�false��Ԃ�
                    
                    return true; // ��L�̏����ɊY�����Ȃ��ꍇ�i�s�\�Ƃ݂Ȃ�true��Ԃ�
                }
            }
        }
    }
}