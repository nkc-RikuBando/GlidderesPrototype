using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Buff;
using Glidders.Character;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const float RETURNTIME_STATE = 0.25f;
            const float RETURNTIME_END = 0.5f;

            private int defalutNumber = 0; // Linq�ɂ���ē���ւ�����v�f�ԍ����A���̔ԍ������m���ۑ�����ϐ�
            private int targetObjectLength; // �֐�TargetSetting�Ɏg����ϐ��@�ύX�O��setTargetObject�̑��ʂ�ۑ�����ϐ�
            private float damage; // ���_���[�W�ʂ�ۊǂ���ϐ�

            public List<CharacterData> sampleSignals; // �󂯎�����z������X�g�Ƃ��Ĉ������߂̃��X�g
            public int[] addPoint = new int[ActiveRule.playerCount]; // �ǉ�����|�C���g��

            // �e�N���X
            private Animator[] animators = new Animator[ActiveRule.playerCount];
            private Text[] texts = new Text[ActiveRule.playerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private CameraController cameraController;
            private FieldIndexOffset index;
            private FieldIndex attackPosition;
            DisplaySkillCutIn skillCutIn;

            private List<GameObject> setTargetObject = new List<GameObject>();
            private CharacterDirection[] characterDirections;

            private List<Vector3> textStatus = new List<Vector3>();
            private List<Vector2> buffStatus = new List<Vector2>();
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap,CharacterDirection[] directions, CameraController cameraController,Text[] texts,DisplaySkillCutIn skillCutIn)
            {
                // GetComponent�ς݂̊e�N���X�����̂܂ܓ����
                displayTile = displayTileMap;
                characterDirections = directions;
                fieldCore = core;
                this.cameraController = cameraController;
                this.animators = animators;
                this.texts = texts;
                this.skillCutIn = skillCutIn;
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas,Action phaseCompleteAction,bool onlineData)
            {
                if (onlineData)
                {
                    SignalConverter signal = new SignalConverter();

                    for (int i = 0; i < characterDatas.Length; i++)
                    {
                        characterDatas[i].attackSignal = signal.GetAttackSignalData(characterDatas[i].attackSignalNumber, characterDatas[i].playerNumber);
                        characterDatas[i].direcionSignal = signal.GetDirectionSignalData(characterDatas[i].directionSignalNumber, characterDatas[i].playerNumber);
                    }
                }

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
                    if (characterDatas[i].canAct && characterDatas[i].attackSignal.isAttack) characterDatas[i].energy -= characterDatas[i].attackSignal.skillData.energy;
                    sampleSignals.Add(characterDatas[i]);
                }

                // sampleSignals.RemoveAll(x => x.attackSignal.skillData == null);
                sampleSignals.OrderBy(x => x.attackSignal.skillData.priority); // �U�����Ƀ��X�g�����ւ���  


                foreach (var x in sampleSignals)
                {
                    // Debug.Log($"{x.playerName}��{x.thisObject.name}��{x.attackSignal.skillData.skillName}��{x.attackSignal.skillData.damage}�̃_���[�W�l�������Ă��܂�");

                    if (!x.canAct) continue; // ���g���U���ł��Ȃ��󋵂ɂ���ꍇ�A�������X�L�b�v����
                    if (!x.attackSignal.isAttack) continue; // �U�������Ȃ��Ƃ�����񂪓����Ă���Ƃ��A�������X�L�b�v����

                    // �����̈ʒu�ɍU���̎�ނŏ�������(�U���A�ړ��A�o�t)
                    // �U���͏]���̏����@�ړ��̓��[�J���֐� SkillMove �o�t�͊֐� BuffSeter �Ƃ�����������
                    if (x.attackSignal.skillData.skillType == SkillTypeEnum.SUPPORT)
                    {
                        AnimationPlaying(x.thisObject);

                        setTargetObject.Add(x.thisObject);
                        skillCutIn.StartSkillCutIn((int)x.characterName, x.attackSignal.skillData.skillName);
                    }
                    else
                    {
                        AnimationPlaying(x.thisObject); // �A�j���[�V�����̍Đ����s���֐����Ăяo��

                        cameraController.ClearTarget(); // �S�ẴJ�����Ǐ]�Ώۂ���������
                        setTargetObject.Clear(); // �J�����Ǐ]�Ώۂɐݒ肳��Ă���I�u�W�F�N�g������������

                        // �U���}�X������������
                        for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                        {
                            for (int j = 0; j < ActiveRule.playerCount; j++)
                            {
                                if (sampleSignals[j].thisObject == x.thisObject) defalutNumber = j; // ����ւ�����f�[�^���{���ǂ��ɂ������̂����擾
                            }

                            #region �X�L���̌����Ɋ�Â����ʂɂȂ�悤��FieldIndex�𒲐����鏈��

                            // �f�o�b�O�p�����@�L�����N�^1�̕����ڍ�
                            //if (x.playerName == "��������������������!!!!!")
                            //{
                            //    Debug.Log($"attackDirection({x.attackSignal.direction.rowOffset},{x.attackSignal.direction.columnOffset})");
                            //    Debug.Log($"DirectionSignal({x.direcionSignal.direction.rowOffset},{x.direcionSignal.direction.columnOffset})");
                            //}

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
                                // Debug.Log($"�o�t�v�Z�O�@�F {sampleSignals[defalutNumber].attackSignal.skillData.power} | PlayerName {characterDatas[defalutNumber].playerName}");
                                int fieldPower = (int)BuffFieldCheck(sampleSignals[defalutNumber].attackSignal.skillData.power, sampleSignals[defalutNumber]);
                                // Debug.Log($"�o�t�v�Z��@�F {fieldPower}");
                                fieldCore.SetDamageField(defalutNumber, fieldPower, attackPosition);
                                displayTile.DisplayDamageFieldTilemap(attackPosition, fieldCore.GetDamageFieldOwner(attackPosition));
                                // Debug.Log($"index({i}) = ({attackPosition.row},{attackPosition.column})�̓_���[�W�t�B�[���h���������Ƃ��Đ���ɍ쓮���܂���");
                            }
                            skillCutIn.StartSkillCutIn((int)x.characterName,x.attackSignal.skillData.skillName);
                            AttackDamage(x, attackPosition); // �U���̃_���[�W�𔭐�����֐�


                            // �U���̏������I������Ƃ��ɑΏۂ��܂��ݒ肳��Ă��Ȃ��Ȃ玩�g�݂̂�ݒ�
                            if (i == x.attackSignal.skillData.attackFieldIndexOffsetArray.Length - 1 && setTargetObject.Count == 0) setTargetObject.Add(x.thisObject);
                            // Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                        }
                    }

                    CameraPositionSeter(setTargetObject); // �J���������֐�
                    // yield return new WaitForSeconds(YIELD_TIME); // �w��b����~
                    yield return new WaitForSeconds(x.attackSignal.skillData.skillAnimation.length + RETURNTIME_STATE); // �X�L���f�[�^�ɂ��Ă���N���b�v�̍Đ����ԕ�������~

                    if (x.attackSignal.skillData.loseBuff != null) // �����X�L����ł����ۂɏ�������������Ȃ�
                    {
                        for (int j = 0; j < x.attackSignal.skillData.loseBuff.Count; j++) // �����o�t�̐�������������
                        {
                            for (int I = 0; I < x.buffView.Count; I++) // ���g�������Ă���o�t�̐�������������
                            {
                                if (x.buffView[I] == x.attackSignal.skillData.loseBuff[j])
                                {
                                    for (int J = 0; J < x.buffValue[I].Count; J++) // �o�t���e�������񂵁A�^�[���Ɠ��e������
                                    {
                                        x.buffValue[I].RemoveAt(J);
                                        x.buffTurn[I].RemoveAt(J);
                                    }

                                    if (x.buffEffectObject[I] != null) UnityEngine.Object.Destroy(x.buffEffectObject[I]);
                                    x.buffEffectObject.RemoveAt(I);

                                    // �������o�t�̓��e���`�ԕω��𔺂��o�t�ł������ꍇ�A�`�Ԃ����Ƃɖ߂�
                                    if (x.buffView[I].lowerTransform != null)
                                    {
                                        // �L�����N�^�[��ScriptableObject��Animator��ݒ肷��
                                        x.thisObject.GetComponent<CharacterCore>().characterScriptableObject = x.buffView[I].lowerTransform;
                                        x.thisObject.GetComponent<Animator>().runtimeAnimatorController = x.buffView[I].lowerTransform.characterAnimator;
                                    }

                                    // �S�Ẵo�t�֘A������
                                    x.buffView.RemoveAt(I);
                                    x.buffValue.RemoveAt(I);
                                    x.buffTurn.RemoveAt(I);
                                }
                            }
                        }
                    }

                    if (x.attackSignal.skillData.skillType == SkillTypeEnum.SUPPORT)
                    {
                        BuffChecker(x);
                    }

                    for (int i = 0;i < textStatus.Count;i++)
                    {
                        TextMove((int)textStatus[i].x, (int)textStatus[i].y, textStatus[i].z);
                        characterDatas[(int)textStatus[i].x].totalDamage += (int)textStatus[i].y;
                    }


                    for (int i = 0;i < buffStatus.Count;i++)
                    {
                        BuffSeter((int)buffStatus[i].x, (int)buffStatus[i].y);
                    }

                    yield return new WaitForSeconds(RETURNTIME_END);

                    textStatus = new List<Vector3>();
                    buffStatus = new List<Vector2>();
                    TextReseter(ref texts);
                }

                setTargetObject = new List<GameObject>();
                // �����Ă���|�C���g���e�L�����ɒǉ� �ŏI�������𔽉f
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    setTargetObject.Add(characterDatas[i].thisObject);
                    characterDatas[i].point += addPoint[i];
                    characterDirections[i].SetDirection(characterDatas[i].direcionSignal.direction);
                }
                CameraPositionSeter(setTargetObject);

                phaseCompleteAction(); // ����������ʒm

                // Debug.Log("�����I��");

                #region ���[�J���֐�

                void AnimationPlaying(GameObject animationObject)
                {
                    // Debug.Log("�A�j���[�V�����Đ��֐����퓮��");
                    for (int i = 0; i < ActiveRule.playerCount; i++)
                    {
                        if (sampleSignals[i].thisObject == animationObject)
                        {
                            characterDirections[i].SetDirection(characterDatas[i].attackSignal.direction);
                            animators[i].SetTrigger("Act" + sampleSignals[i].attackSignal.skillNumber);
                        }
                    }
                }

                void BuffSeter(int characterNumber,int i)
                {
                    for (int j = 0;j < characterDatas[characterNumber].attackSignal.skillData.giveBuff.Count;j++)
                    {
                        characterDatas[characterNumber].buffView.Add(characterDatas[characterNumber].attackSignal.skillData.giveBuff[j]); // �o�t����ǉ�
                        characterDatas[characterNumber].buffTurn.Add(new List<int>()); // �o�t�o�߃^�[����List���쐬
                        if (characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].effectObjectPrefab != null) // �����Ή�����o�t�ɃI�u�W�F�N�g������Ȃ�
                        {
                            characterDatas[characterNumber].buffEffectObject.Add(UnityEngine.Object.Instantiate(characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].effectObjectPrefab, characterDatas[characterNumber].thisObject.transform));
                        }
                        else characterDatas[characterNumber].buffEffectObject.Add(null);

                        // ���`�ԕω�����o�t�ł������ꍇ
                        if (characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].upperTransform != null)
                        {
                            // �L�����N�^�[��ScriptableObject��Animator��ݒ肷��
                            characterDatas[characterNumber].thisObject.GetComponent<CharacterCore>().characterScriptableObject = characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].upperTransform;
                            characterDatas[characterNumber].thisObject.GetComponent<Animator>().runtimeAnimatorController = characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].upperTransform.characterAnimator;
                        }

                        List<BuffValueData> sampleData = new List<BuffValueData>(characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].buffValueList);
                        characterDatas[characterNumber].buffValue.Add(sampleData); // ����Ă�����List�Ƀo�t���e���L�q

                        // �o�t���ɓ����Ă���o�t���e��������������
                        for (int I = 0; I < characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].buffValueList.Count; I++)
                        {
                            characterDatas[characterNumber].buffTurn[i].Add(0);
                        }
                    }
                }
                #endregion
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
                                damage = BuffDamageCheck(sampleSignals[j].attackSignal.skillData.damage, sampleSignals[i], sampleSignals[j]);// �o�t�_���[�W���v�Z����֐����o�R���A�ŏI�_���[�W���o��

                                textStatus.Add(new Vector3(i, (int)Mathf.Round(damage), sampleSignals[i].thisObject.transform.localScale.x));

                                // �ŏI�_���[�W�̉����Z���U�����A������ɔ��f����
                                addPoint[i] -= (int)Mathf.Round(damage);
                                if (ActiveRule.gameRule == 0) addPoint[j] += (int)Mathf.Round(damage);
                                
                                TargetSeting(sampleSignals[i].thisObject, sampleSignals[j].thisObject); // �J�����̒Ǐ]�Ώۂ�ݒ肷��֐����Ă�
                            }
                        }

                        // Debug.Log($"{character.thisObject.name}��{character.attackSignal.skillData.name}��{sampleSignals[i].thisObject.name}�Ƀq�b�g���A{damage}�̃|�C���g�𓾂�");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            /// <summary>
            /// �X�L���ɂ��o�t������֐�
            /// </summary>
            private void BuffChecker(CharacterData characterData)
            {
                bool returnFlg = false;

                int count = characterData.buffView.Count; // �����������s���O�̃o�t����ۑ�

                for (int i = 0; i < characterData.buffView.Count; i++)
                {
                    for (int j = 0; j < characterData.attackSignal.skillData.giveBuff.Count; j++)
                    {
                        if (characterData.buffView[i] == characterData.attackSignal.skillData.giveBuff[j])
                        {
                            for (int I = 0; I < characterData.buffValue[i].Count; I++)
                            {
                                for (int J = 0; J < characterData.attackSignal.skillData.giveBuff[j].buffValueList.Count; J++)
                                {
                                    int plusTurn = characterData.attackSignal.skillData.giveBuff[j].buffValueList[J].buffDuration - 1;
                                    characterData.buffTurn[i][I] += plusTurn;
                                }
                            }
                            returnFlg = true;
                            break;
                        }
                    }
                }

                if (returnFlg) return;

                // �ǉ�����o�t�̐�������������
                for (int i = count; i < characterData.attackSignal.skillData.giveBuff.Count + count;++i)
                {
                    buffStatus.Add(new Vector2(characterData.playerNumber, i));

                    #region �O�̂��ߕۑ��@����
                    //characterData.buffView.Add(characterData.attackSignal.skillData.giveBuff[i]); // �o�t����ǉ�
                    //characterData.buffTurn.Add(new List<int>()); // �o�t�o�߃^�[����List���쐬
                    //if (characterData.attackSignal.skillData.giveBuff[i].effectObjectPrefab != null) // �����Ή�����o�t�ɃI�u�W�F�N�g������Ȃ�
                    //{
                    //    characterData.buffEffectObject.Add(UnityEngine.Object.Instantiate(characterData.attackSignal.skillData.giveBuff[i].effectObjectPrefab, characterData.thisObject.transform));
                    //}
                    //else characterData.buffEffectObject.Add(null);

                    //// ���`�ԕω�����o�t�ł������ꍇ
                    //if (characterData.attackSignal.skillData.giveBuff[i].upperTransform != null)
                    //{
                    //    // �L�����N�^�[��ScriptableObject��Animator��ݒ肷��
                    //    characterData.thisObject.GetComponent<CharacterCore>().characterScriptableObject = characterData.attackSignal.skillData.giveBuff[i].upperTransform;
                    //    characterData.thisObject.GetComponent<Animator>().runtimeAnimatorController = characterData.attackSignal.skillData.giveBuff[i].upperTransform.characterAnimator;
                    //}

                    //List<BuffValueData> sampleData = new List<BuffValueData>(characterData.attackSignal.skillData.giveBuff[i].buffValueList);
                    //characterData.buffValue.Add(sampleData); // ����Ă�����List�Ƀo�t���e���L�q

                    //// �o�t���ɓ����Ă���o�t���e��������������
                    //for (int j = 0; j < characterData.attackSignal.skillData.giveBuff[i].buffValueList.Count; j++)
                    //{
                    //    characterData.buffTurn[i].Add(0);
                    //}
                    #endregion
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
            /// �_���[�W�t�B�[���h�̃^�[�����̃o�t�̉����Z���v�Z����֐�
            /// </summary>
            /// <param name="defaultPower">���З�</param>
            /// <param name="attackSideData">�U���T�C�h�̃L�����N�^�f�[�^</param>
            /// <returns>�o�t�����������З�</returns>
            private float BuffFieldCheck(int defaultPower,CharacterData attackSideData)
            {
               float totalFieldPower = defaultPower; // �ŏI�З͂�ۊǂ��郍�[�J���֐�

                for (int i = 0;i < attackSideData.buffView.Count; i++) // ���Ă���o�t�̐�������������
                {
                    for (int j = 0;j < attackSideData.buffValue[i].Count;j++) // ���ݏ���Ă���o�t�̐�������������
                    {
                        if (attackSideData.buffValue[i][j].buffedStatus == StatusTypeEnum.POWER) // �o�t�̓��e���З͂Ȃ珈�������s
                        {
                            switch(attackSideData.buffValue[i][j].buffType)
                            {
                                case BuffTypeEnum.PLUS:
                                    totalFieldPower += attackSideData.buffValue[i][j].buffScale;
                                    break;
                                case BuffTypeEnum.MULTIPLIED:
                                    totalFieldPower *= attackSideData.buffValue[i][j].buffScale;
                                    break;
                            }
                        }
                    }
                }

                return Mathf.Round(totalFieldPower);
            }

            /// <summary>
            /// �o�t�ɂ��_���[�W�̉����Z����������֐�
            /// </summary>
            /// <param name="defaultDamage">�����������_���[�W��</param>
            /// <param name="deffenceSideData">����T�C�h���̃L�����N�^�f�[�^</param>
            /// <param name="attackSideData">�U���T�C�h���̃L�����N�^�f�[�^</param>
            /// <returns>�o�t�����������ŏI�_���[�W��</returns>
            private float BuffDamageCheck(int defaultDamage,CharacterData deffenceSideData,CharacterData attackSideData)
            {
                float totalDamage = defaultDamage; // ���_���[�W�ʂ�ۊǂ��郍�[�J���ϐ�

                // �U���T�C�h�̏����@�_���[�W����������o�t�̏ꍇ�̂ݏ���
                for (int i = 0;i < attackSideData.buffView.Count;i++) // ���ݏ���Ă���o�t�̗ʕ���for��
                {
                    for (int j = 0;j < attackSideData.buffValue[i].Count;j++) // ���ݏ���Ă���o�t���ʂ̑��ʕ���for��
                    {
                        if (attackSideData.buffValue[i][j].buffedStatus == StatusTypeEnum.DAMAGE) // ���ݏ������̃o�t���ʂ��_���[�W�㏸�֘A�������ꍇ�̂ݏ���
                        {
                            switch (attackSideData.buffValue[i][j].buffType)
                            {
                                case BuffTypeEnum.PLUS:
                                    totalDamage += attackSideData.buffValue[i][j].buffScale;
                                    break;
                                case BuffTypeEnum.MULTIPLIED:
                                    totalDamage *= attackSideData.buffValue[i][j].buffScale;
                                    break;
                            }
                        }
                    }
                }

                // ����T�C�h�̏����@�_���[�W�����Z����o�t�̎��̂ݏ���
                for (int i = 0;i < deffenceSideData.buffView.Count;i++)// ���ݏ���Ă���o�t�̗ʕ���for��
                {
                    for (int j = 0;j < deffenceSideData.buffValue[i].Count;j++) // ���ݏ���Ă���o�t���ʂ̑��ʕ���for��
                    {
                        if (deffenceSideData.buffValue[i][j].buffedStatus == StatusTypeEnum.DEFENSE) // ���ݏ������̃o�t���ʂ��_���[�W���Z�֘A�������ꍇ�̂ݏ���
                        {
                            switch (deffenceSideData.buffValue[i][j].buffType)
                            {
                                case BuffTypeEnum.PLUS:
                                    totalDamage -= deffenceSideData.buffValue[i][j].buffScale;
                                    break;
                                case BuffTypeEnum.MULTIPLIED:
                                    totalDamage *= deffenceSideData.buffValue[i][j].buffScale;
                                    break;
                            }
                        }
                    }
                }
                return totalDamage;
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


            private void TextMove(int targetNumber,int damagePoint,float x_localScale)
            {
                texts[targetNumber].text = damagePoint.ToString();
                texts[targetNumber].rectTransform.localScale = new Vector3(x_localScale,1,1);
                animators[targetNumber].SetTrigger("Damage");
            }

            private void TextReseter(ref Text[] texts)
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = "";
                }
            }
        }

    }
}
