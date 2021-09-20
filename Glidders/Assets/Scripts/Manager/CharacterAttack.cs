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
            private int defalutNumber = 0; // Linq�ɂ���ē���ւ�����v�f�ԍ����A���̔ԍ������m���ۑ�����ϐ�
            private int targetObjectLength; // �֐�TargetSetting�Ɏg����ϐ��@�ύX�O��setTargetObject�̑��ʂ�ۑ�����ϐ�
            private float damage; // ���_���[�W�ʂ�ۊǂ���ϐ�

            public List<CharacterData> sampleSignals; // �󂯎�����z������X�g�Ƃ��Ĉ������߂̃��X�g
            public int[] addPoint = new int[Rule.maxPlayerCount]; // �ǉ�����|�C���g��

            // �e�N���X
            private Animator[] animators = new Animator[Rule.maxPlayerCount];
            private Text[] texts = new Text[Rule.maxPlayerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private CameraController cameraController;
            private FieldIndexOffset index;
            private FieldIndex attackPosition;

            private List<GameObject> setTargetObject = new List<GameObject>();
            private CharacterDirection[] characterDirections;
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap,CharacterDirection[] directions, CameraController cameraController,Text[] texts)
            {
                // GetComponent�ς݂̊e�N���X�����̂܂ܓ����
                displayTile = displayTileMap;
                characterDirections = directions;
                fieldCore = core;
                this.cameraController = cameraController;
                this.animators = animators;
                this.texts = texts;
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas,Action phaseCompleteAction)
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

                // var sampleList = sampleSignals.RemoveAll(x => x.attackSignal);
                var signalList = sampleSignals.OrderByDescending(x => x.attackSignal.skillData.priority); // �U�����Ƀ��X�g�����ւ���  

                foreach (var x in signalList)
                {
                    // Debug.Log($"{x.playerName}��{x.thisObject.name}��{x.attackSignal.skillData.skillName}��{x.attackSignal.skillData.damage}�̃_���[�W�l�������Ă��܂�");

                    if (!x.canAct) continue; // ���g���U���ł��Ȃ��󋵂ɂ���ꍇ�A�������X�L�b�v����
                    if (!x.attackSignal.isAttack) continue; // �U�������Ȃ��Ƃ�����񂪓����Ă���Ƃ��A�������X�L�b�v����

                    // �����̈ʒu�ɍU���̎�ނŏ�������(�U���A�ړ��A�o�t)
                    // �U���͏]���̏����@�ړ��̓��[�J���֐� SkillMove �o�t�͊֐� BuffSeter �Ƃ�����������
                    if (x.attackSignal.skillData.skillType == SkillTypeEnum.SUPPORT)
                    {
                        BuffSeter(x);
                        setTargetObject.Add(x.thisObject);
                    }
                    else
                    {
                        AnimationPlaying(x.thisObject); // �A�j���[�V�����̍Đ����s���֐����Ăяo��

                        cameraController.ClearTarget(); // �S�ẴJ�����Ǐ]�Ώۂ���������
                        setTargetObject.Clear(); // �J�����Ǐ]�Ώۂɐݒ肳��Ă���I�u�W�F�N�g������������

                        // �U���}�X������������
                        for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                        {
                            for (int j = 0; j < Rule.maxPlayerCount; j++)
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
                            AttackDamage(x, attackPosition); // �U���̃_���[�W�𔭐�����֐�

                            // �U���̏������I������Ƃ��ɑΏۂ��܂��ݒ肳��Ă��Ȃ��Ȃ玩�g�݂̂�ݒ�
                            if (i == x.attackSignal.skillData.attackFieldIndexOffsetArray.Length - 1 && setTargetObject.Count == 0) setTargetObject.Add(x.thisObject);
                            // Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                        }
                    }

                    CameraPositionSeter(setTargetObject); // �J���������֐�
                    // yield return new WaitForSeconds(YIELD_TIME); // �w��b����~
                    yield return new WaitForSeconds(x.attackSignal.skillData.skillAnimation.length); // �X�L���f�[�^�ɂ��Ă���N���b�v�̍Đ����ԕ�������~

                    TextReseter(ref texts);
                }

                // �����Ă���|�C���g���e�L�����ɒǉ� �ŏI�������𔽉f
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                    characterDirections[i].SetDirection(characterDatas[i].direcionSignal.direction);
                }

                phaseCompleteAction(); // ����������ʒm

                // Debug.Log("�����I��");

                #region ���[�J���֐�
                void SkillMove(ref CharacterData characterData)
                {
                    // ��肽������
                    // 1.�X�L���f�[�^����ړ��ʒu�𔲂��o��
                    // 2.���̈ʒu�ւ�index�̏��������@transform��̈ړ����Y�ꂸ��
                    // 3.���̈ʒu�����̃L�����N�^�Ƃ��Ԃ��Ă��Ȃ��������m�@����Ă���Ȃ玩�g���s���s�\�ɂ���
                    // 4.�_���[�W�t�B�[���h�̌��m�@����Ȃ�΃_���[�W�������s��
                    // 5.�K�v�ɉ����ăX�L���A�j���[�V�������Đ�

                    FieldIndex debugIndex = new FieldIndex(0, 0); // �K���Ȓl�@�{���͐ݒ肳�ꂽindex���g������

                    characterData.index = debugIndex;
                    characterData.thisObject.transform.position = fieldCore.GetTilePosition(characterData.index);

                    for (int i = 0; i < Rule.maxPlayerCount; i++)
                    {
                        if (characterDatas[i].thisObject == characterData.thisObject) return;

                        if (characterDatas[i].index == characterData.index)
                        {
                            characterData.canAct = false;
                            break;
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

                                TextMove(i, (int)Mathf.Round(damage));

                                // �ŏI�_���[�W�̉����Z���U�����A������ɔ��f����
                                addPoint[i] -= (int)Mathf.Round(damage);
                                addPoint[j] += (int)Mathf.Round(damage);
                                
                                TargetSeting(sampleSignals[i].thisObject, sampleSignals[j].thisObject); // �J�����̒Ǐ]�Ώۂ�ݒ肷��֐����Ă�
                            }
                        }
                        animators[i].SetTrigger("Damage");

                        // Debug.Log($"{character.thisObject.name}��{character.attackSignal.skillData.name}��{sampleSignals[i].thisObject.name}�Ƀq�b�g���A{damage}�̃|�C���g�𓾂�");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            /// <summary>
            /// �X�L���ɂ��o�t������֐�
            /// </summary>
            private void BuffSeter(CharacterData characterData)
            {
                // ��肽�����Ɓ@
                // 1.�X�L���f�[�^����ǉ�����o�t�𔲂��o��
                // 2.���łɂ��̃o�t�����݂��邩�ǂ��������m�@����Ȃ牄���@�Ȃ��Ȃ�ǉ�
                // 3.�X�L���A�j���[�V�����̍Đ����s��

                int count = characterData.buffView.Count + 1;

                for (int i = count; i < characterData.attackSignal.skillData.giveBuff.Count + count;i++)
                {
                    characterData.buffView.Add(characterData.attackSignal.skillData.giveBuff[i]);
                    characterData.buffValue.Add(new List<BuffValueData>());
                    characterData.buffTurn.Add(new List<int>());

                    for (int j = 0;j < characterData.attackSignal.skillData.giveBuff[i].buffValueList.Count;j++)
                    { 
                        characterData.buffValue[i].Add(characterData.attackSignal.skillData.giveBuff[i].buffValueList[j]);
                        characterData.buffTurn[i].Add(characterData.attackSignal.skillData.giveBuff[i].buffValueList[j].buffDuration);
                    }
                }

                //for (int i = 0; i < characterData.attackSignal.skillData.giveBuff.buffValueList.Count; i++)
                //{
                //    characterData.buffTurn.Add(characterData.attackSignal.skillData.giveBuff.buffValueList[i].buffDuration);
                //}

                // characterData.attackSignal.skillData.
                // characterData.buffTurn.Add(0);
                // characterData.buffView.Add();
                // characterData.buffValue.Add();
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

            private void TextMove(int targetNumber,int damagePoint)
            {
                texts[targetNumber].text = damagePoint.ToString();
                texts[targetNumber].rectTransform.localScale = new Vector3(-1,1,1);
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
