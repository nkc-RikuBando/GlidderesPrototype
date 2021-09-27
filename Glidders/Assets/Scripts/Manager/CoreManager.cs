using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Command;
using Glidders.Player_namespace;
using Glidders.Buff;
using System;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    { 
        delegate void PhaseMethod(); // �t�F�[�Y���Ƃ̍s�����L�^�����֐���o�^����f���Q�[�g
        public class CoreManager : MonoBehaviour, ICharacterDataReceiver, IPhaseInformation, IPlayerInformation
        {
            PhotonView view;

            Action phaseCompleteAction; // �f���Q�[�g

            GameObject[] commandDirectorArray = new GameObject[Rule.maxPlayerCount];

            private enum Phase
            {
                TURN_START, ACTION_SERECT, MOVE, ATTACK, TURN_END
            }

            private enum MatchFormat
            {
                POINT, HITPOINT
            }

            // �e�N���X
            CharacterMove characterMove;
            CharacterAttack characterAttack;
            FieldCore fieldCore;
            DisplayTileMap displayTileMap;
            CameraController cameraController;
            AutoSignalSelecter autoSignalSelecter;
            CommandFlow[] commandFlows = new CommandFlow[Rule.maxPlayerCount];
            CharacterDirection[] characterDirections = new CharacterDirection[Rule.maxPlayerCount];

            CharacterData[] characterDataList = new CharacterData[Rule.maxPlayerCount]; // �f�[�^�̑��ʂ��v���C���[�̑����̕����

            public int thisTurn { get; set; } // ���݂̃^�[��
            public int positionSetMenber { get; set; } // �����ʒu��I�����������o�[����c������
            public bool moveStart { get; set; } // �ړ����\���ǂ���
            public bool attackStart { get; set; } // �U�����\���ǂ���
            private bool selectStart { get; set; } // ���͉\���ǂ���
            private bool[] movesignals = new bool[Rule.maxPlayerCount];
            private bool[] directionsignals = new bool[Rule.maxPlayerCount];
            private bool[] attacksignals = new bool[Rule.maxPlayerCount];
            MatchFormat format = new MatchFormat(); // �����`�����Ǘ�����enum

            private Animator[] animators = new Animator[Rule.maxPlayerCount]; // �A�j���[�V�����Ǘ��̃A�j���[�^�[�ϐ�
            private Text[] texts = new Text[Rule.maxPlayerCount];

            // �����o�t���Ǘ�����List<List<int>>
            private List<List<int>> removeNumber_value = new List<List<int>>(0);
            private List<List<int>> removeNumber_view = new List<List<int>>(0);

            [SerializeField] private GameObject serverObject;

            [Header("�f�o�b�O�p�@�g�p�o�t")]
            [SerializeField] private BuffViewData[] buffViewData = new BuffViewData[4];


            #region �f�o�b�O�p�ϐ�
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,]
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("�f�o�b�O�p�@�X�L���f�[�^")]
            [SerializeField] private Character.UniqueSkillScriptableObject[] UniqueSkillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                selectStart = true;
                #region ���X�g�̏�����
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region �f�o�b�O�p�@Move���X�g�����̏����� ����с@Move���X�g�����̐���
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[Rule.maxMoveAmount];
                    for (int j = 0; j < Rule.maxMoveAmount; j++)
                    {
                        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                    }
                    MoveDataReceiver(characterDataList[i].moveSignal, i);
                }

                // �f�o�b�O�p���܂ރL�����N�^�f�[�^��������
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].canAct = true;
                    characterDataList[i].point = 10000;
                    characterDataList[i].energy = 3;
                    characterDataList[i].direcionSignal.direction = FieldIndexOffset.left;
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;

                    characterDataList[i].buffView = new List<BuffViewData>();
                    characterDataList[i].buffValue = new List<List<BuffValueData>>();
                    characterDataList[i].buffTurn = new List<List<int>>();
                }

                characterDataList[0].index = new FieldIndex(4, 4);
                characterDataList[1].index = new FieldIndex(5, 3);
                //characterDataList[2].index = new FieldIndex(5, 4);
                //characterDataList[3].index = new FieldIndex(5, 5);

                #endregion


                #region �f�o�b�O�p�@Attack���X�g�̏�����
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, UniqueSkillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left,i);
                }
                #endregion
                #endregion

                for (int i = 0; i < characterDataList.Length; i++)
                {
                    StartPositionSeter(characterDataList[i].index, i);
                    // commandFlows[0] = GameObject.Find("CommandDirector").GetComponent<CommandFlow>();
                }

                Instantiate(serverObject);

                for (int i = 0; i < characterDirections.Length; i++)
                {
                    AttackDataReceiver(characterDataList[i].attackSignal, i); // �U���M�����i�[����
                }

                view = GetComponent<PhotonView>();
                cameraController = GameObject.Find("Vcam").GetComponentInChildren<CameraController>();
                fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �N���X�擾
                displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>(); // �N���X�擾
                characterMove = new CharacterMove(fieldCore, characterDirections,texts); // CharacterMove�̐����@�擾�����C���^�[�t�F�[�X�̏���n��
                characterAttack = new CharacterAttack(animators,fieldCore,displayTileMap,characterDirections,cameraController,texts); // CharacterAttack�̐���
                autoSignalSelecter = new AutoSignalSelecter(fieldCore);

                FindAndSetCommandObject();
                // view.RPC(nameof(FindAndSetCommandObject), RpcTarget.AllBufferedViaServer);
            }

            // Update is called once per frame
            void Update()
            {
                // �f�o�b�O�p�@�����ʒu����
                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                //    phaseCompleteAction();
                //}

                // �f�o�b�O�p �S�M����true����
                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    for (int i = 1; i < Rule.maxPlayerCount; i++)
                    {
                        characterDataList[i] = autoSignalSelecter.SignalSet(characterDataList[i],characterDataList[0]);
                        movesignals[i] = true;
                        attacksignals[i] = true;
                        directionsignals[i] = true;
                    }
                }

                if (Rule.maxPlayerCount > positionSetMenber) return;

                // �f�o�b�O�p�@�L�����N�^�[�����m�F
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        Debug.Log($"CharacterName is {characterDataList[i].thisObject.name}({characterDataList[i].playerName})");
                        Debug.Log($"Index ({characterDataList[i].index.row},{characterDataList[i].index.column}) | point ({characterDataList[i].point})");
                    }
                }
            }

            #region �e��^�[������
            // PhaseCompleateAction ��GameDirector ���ɏ��������ׂďI��������Ƃ�m�点��f���Q�[�g

            [PunRPC]
            public void TurnStart()
            {
                // �L�����N�^�̈ʒu�𔽉f(�����̈ʒu���𔽉f���邽��)
                for (int i = 0;i < Rule.maxPlayerCount;i++)
                {
                    characterDataList[i].thisObject.transform.position = fieldCore.GetTilePosition(characterDataList[i].index);
                }

                phaseCompleteAction();
            }

            [PunRPC]
            public void ActionSelect()
            {
                if (!selectStart) return; 
                PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>(); // ���̃V�[���ɑ��݂���PlayerCore���擾����

                // commandFlows[playerCore.playerId].StartCommandPhase(playerCore.playerId,characterDataList[playerCore.playerId].thisObject,characterDataList[playerCore.playerId].index);
                // Debug.Log(characterDataList[0].index.column + " : " + characterDataList[0].index.row);
                float movebuff = 0;
                for (int i = 0;i < characterDataList[0].buffView.Count;i++)
                {
                    for (int j = 0;j < characterDataList[0].buffValue[i].Count;j++)
                    {
                        if (characterDataList[0].buffValue[i][j].buffedStatus == Character.StatusTypeEnum.MOVE)
                            movebuff += characterDataList[0].buffValue[i][j].buffScale;
                    }
                }
                // �f�o�b�O�p�@�ŏ��̃L�����݈̂ړ��������s��
                commandFlows[0].StartCommandPhase(0,characterDataList[0].thisObject,characterDataList[0].index,(int)movebuff,characterDataList[0].energy);

                StartCoroutine(StaySelectTime()); // �S�L�����̃R�}���h����������܂őҋ@����

                moveStart = true; // �ړ����\�ɂ���

                // phaseCompleteAction(); // �f�o�b�O�p�@�t���O�Ǘ��𖳎����Ď��̃t�F�[�Y��

                // ��L�̏������O���ꍇ�A�E�V�t�g�������ƃt���O��true�ɂȂ�

            }

            [PunRPC]
            public void Move()
            {
                selectStart = true;
                // �ړ����s�t���O��true�̂Ƃ��AMove�N���X�Ɉړ������s������
                if (moveStart)
                {
                    cameraController.ClearTarget(); // �S�ẴJ�����Ǐ]�Ώۂ���������
                    for (int i = 0;i < characterDataList.Length;i++)
                    {
                        cameraController.AddTarget(characterDataList[i].thisObject.transform);
                    }

                    StartCoroutine(characterMove.MoveOrder(characterDataList, phaseCompleteAction)); // ��������������R���[�`�������s

                    attackStart = true; // �U�����\�ɂ���
                    moveStart = false; // �ړ���s�\�ɂ���
                }
                // else phaseCompleteAction();

            }

            [PunRPC]
            public void Attack()
            {
                for (int debug = 0;debug < characterDataList.Length;debug++)
                {
                    Debug.Log($"({debug}) {characterDataList[debug].attackSignal}");
                }

                #region �f�o�b�O�p�@�X�L����������
                switch (thisTurn % 4)
                {
                    case 0: // ��
                        for (int i = 1; i < characterDataList.Length; i++)
                        {
                            if (characterDataList[i].attackSignal.skillData != null) break;
                            characterDataList[i].attackSignal = new AttackSignal(true, UniqueSkillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.up, FieldIndexOffset.up, Mathf.Max(i, 1));
                        }
                        break;
                    case 1: // ��
                        for (int i = 1; i < characterDataList.Length; i++)
                        {
                            if (characterDataList[i].attackSignal.skillData != null) break;
                            characterDataList[i].attackSignal = new AttackSignal(true, UniqueSkillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.down, FieldIndexOffset.down, Mathf.Max(i, 1));
                        }
                        break;
                    case 2: // ��
                        for (int i = 1; i < characterDataList.Length; i++)
                        {
                            if (characterDataList[i].attackSignal.skillData != null) break;
                            characterDataList[i].attackSignal = new AttackSignal(true, UniqueSkillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.left, FieldIndexOffset.left, Mathf.Max(i, 1));
                        }
                        break;
                    case 3: // �E
                        for (int i = 1; i < characterDataList.Length; i++)
                        {
                            if (characterDataList[i].attackSignal.skillData != null) break;
                            characterDataList[i].attackSignal = new AttackSignal(true, UniqueSkillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.right, FieldIndexOffset.right, Mathf.Max(i, 1));
                        }
                        break;
                }
                #endregion

                // Debug.Log(attackStart);

                // �U�����s�t���O��true�̂Ƃ��AAttack�N���X�ɍU�������s������
                if (attackStart)
                {
                    // Debug.Log("Lets.Attack");
                    StartCoroutine(characterAttack.AttackOrder(characterDataList,phaseCompleteAction)); // �U������������R���[�`�������s

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false; // �U����s�\�ɂ���
                }
                // else phaseCompleteAction();
            }

            [PunRPC]
            public void TurnEnd()
            {
                thisTurn++; // �f�o�b�O�p�̌����ύX�����p�@�^�[���Ǘ�

                // �^�[���I�����̃_���[�W�t�B�[���h��������
                fieldCore.UpdateFieldData(); 
                displayTileMap.ClearDamageFieldTilemap();
                displayTileMap.DisplayDamageFieldTilemap(fieldCore.GetFieldData());

                // �e�R�}���h���͏���������
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;

                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[5];
                    characterDataList[i].attackSignal.skillData = null;
                }

                // �e�L�����N�^�̃G�i�W�[��ǉ��A�s���s�\��Ԃ����� �܂��@�o�t�̃^�[���ɂ����ŏ���
                for (int i = 0;i < characterDataList.Length;i++)
                {
                    characterDataList[i].energy++;
                    characterDataList[i].canAct = true;

                    for (int j = 0; j < characterDataList[i].buffValue.Count; j++) // �o�t�̂��Ă��鐔����
                    {
                        for (int I = 0; I < characterDataList[i].buffValue[j].Count; I++) // �o�t���e����
                        {
                            characterDataList[i].buffTurn[j][I]++; // �o�t�̌o�߃^�[���𑝂₷
                            if (characterDataList[i].buffValue[j][I].buffDuration <= characterDataList[i].buffTurn[j][0]) // �o�߃^�[�����o�t�����^�[�����������ꍇ�A�o�t���e��������
                            {
                                removeNumber_value.Add(new List<int>());
                                removeNumber_value[removeNumber_value.Count - 1].Add(i);
                                removeNumber_value[removeNumber_value.Count - 1].Add(j);
                                removeNumber_value[removeNumber_value.Count - 1].Add(I);

                                // Debug.Log($"removeNumber_value[{removeNumber_value.Count -1}].Count {removeNumber_value[removeNumber_value.Count-1].Count}(i = {removeNumber_value[removeNumber_value.Count - 1][0]} | j = {removeNumber_value[removeNumber_value.Count - 1][1]} | I = {removeNumber_value[removeNumber_value.Count - 1][2]})");
                            }
                        }
                    }
                }

                // ���Ń^�[���̃o�t���e������
                if (removeNumber_value.Count > 0)
                {
                    int number = removeNumber_value.Count-1; // ���Ń^�[���̃o�t����
                    for (int i = number; i >= 0;i--)
                    {
                        // ���X�g����o�^���Ă������o�t������
                        ListRemover(removeNumber_value[i][0], removeNumber_value[i][1], removeNumber_value[i][2]);
                    }

                    removeNumber_value = new List<List<int>>(0); // �o�^�����폜����
                }

                // �o�t���e�����ׂď������o�t��������
                if (removeNumber_view.Count > 0)
                {
                    int number = removeNumber_view.Count -1; // ���ł���o�t��񑍐�
                    for (int i = number; i >= 0; i--)
                    {
                        // ���X�g����o�^���Ă������o�t������
                        ListRemover(removeNumber_view[i][0], removeNumber_view[i][1]);
                    }

                    removeNumber_view = new List<List<int>>(0); // �o�^�����폜����
                }

                phaseCompleteAction();
            }
            #endregion

            /// <summary>
            /// �o�t���e�����֐�
            /// </summary>
            /// <param name="i">�v���C���ԍ�</param>
            /// <param name="j">�o�t����</param>
            /// <param name="I">�o�t���e����</param>
            private void ListRemover(int i,int j,int I)
            {
                int count = characterDataList[i].buffValue[j].Count;

                characterDataList[i].buffTurn[j].RemoveAt(I);
                if (characterDataList[i].buffTurn[j] == null) characterDataList[i].buffTurn.RemoveAt(j);
                characterDataList[i].buffValue[j].RemoveAt(I);

                if (count == 1) // �o�t�����Ă��镪���Ȃ��Ȃ����Ƃ��A�o�t�֘A�������X�g�����������
                {
                    removeNumber_view.Add(new List<int>());
                    removeNumber_view[removeNumber_view.Count - 1].Add(i);
                    removeNumber_view[removeNumber_view.Count - 1].Add(j);
                }
            }

            /// <summary>
            /// �o�t�������֐�
            /// </summary>
            /// <param name="i">�v���C������</param>
            /// <param name="j">�o�t���e����</param>
            private void ListRemover(int i,int j)
            {
                characterDataList[i].buffValue.RemoveAt(j);
                characterDataList[i].buffView.RemoveAt(j);
            }

            #region �e��C���^�[�t�F�[�X
            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ����������ړ��M���ɓn���ꂽ�ړ��M�����i�[����
            /// </summary>
            /// <param name="signal">�n���ړ��M��</param>
            /// <param name="playerID">�L�����N�^ID</param>
            public void MoveDataReceiver(MoveSignal signal, int playerID)
            {
                characterDataList[playerID].moveSignal = signal;
                movesignals[playerID] = true;
            }

            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ��������������]���M���ɓn���ꂽ�����]���M�����i�[����
            /// </summary>
            /// <param name="signal"></param>
            /// <param name="playerID"></param>
            public void DirectionReceiver(DirecionSignal signal, int playerID)
            {
                characterDataList[playerID].direcionSignal = signal;
                directionsignals[playerID] = true;
            }

            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ����������U���M���ɓn���ꂽ�U���M�����i�[����
            /// </summary>
            /// <param name="signal">�n���U���M��</param>
            /// <param name="playerID">�L�����N�^ID</param>
            public void AttackDataReceiver(AttackSignal signal, int playerID)
            {
                characterDataList[playerID].attackSignal = signal;
                attacksignals[playerID] = true;
            }

            /// <summary>
            /// �w�肳�ꂽ�z��ԍ���������FieldIndex�ɑ΂��A�����ʒu�����炤
            /// </summary>
            /// <param name="fieldIndex">�n��Index</param>
            /// <param name="playerID">�L�����N�^ID</param>
            public void StartPositionSeter(FieldIndex fieldIndex, int playerID)
            {
                characterDataList[playerID].index = fieldIndex;
                positionSetMenber++;

                if (Rule.maxPlayerCount == positionSetMenber)
                {
                    phaseCompleteAction();
                }
            }

            /// <summary>
            /// �����`����ݒ肵�Ă��炤
            /// </summary>
            /// <param name="formatNumber">�`���̐ݒ�ɑΉ���������</param>
            public void MatchFormatSeter(int formatNumber)
            {
                format = (MatchFormat)formatNumber;
            }

            /// <summary>
            /// �I�u�W�F�N�g���������Ă��炤�֐�
            /// </summary>
            /// <param name="thisObject">�Ώۂ̃I�u�W�F�N�g</param>
            /// <param name="playerID">�L�����N�^�[ID</param>
            public void CharacterDataReceber(GameObject thisObject,string playerName,int playerID,int characterID)
            {
                // �e��L�����N�^�[�����擾���A�\���̂ɕۑ����Ă���
                characterDataList[playerID].thisObject = thisObject;
                characterDataList[playerID].playerName = playerName;
                characterDataList[playerID].playerNumber = playerID;
                characterDataList[playerID].characterName = (CharacterName)characterID;

                animators[playerID] = characterDataList[playerID].thisObject.GetComponent<Animator>(); // �A�j���[�^�[�擾
                texts[playerID] = characterDataList[playerID].thisObject.GetComponentInChildren<Text>(); // �e�L�X�g�擾
                characterDirections[playerID] = characterDataList[playerID].thisObject.GetComponent<CharacterDirection>(); // �e�L�����N�^�[����]������N���X���擾����

                // Debug.Log($"CharacterID{characterID}����playerName{playerName}�������Ƃ�܂��� objectName��{characterDataList[characterID].thisObject.name}");
            }

            /// <summary>
            /// ���������̃f���Q�[�g��ݒ肵�Ă��炤
            /// </summary>
            /// <param name="phaseCompleteAction"></param>
            public void SetPhaseCompleteAction(Action phaseCompleteAction)
            {
                this.phaseCompleteAction = phaseCompleteAction;
            }

            /// <summary>
            /// UI���ɃL�����N�^�f�[�^��n���֐�
            /// </summary>
            /// <returns>�ԋp����\����</returns>
            public UICharacterDataSeter[] characterDataSeter()
            {
                UICharacterDataSeter[] dataSeters = new UICharacterDataSeter[Rule.maxPlayerCount];

                for (int i = 0;i < dataSeters.Length;i++)
                {
                    dataSeters[i] = new UICharacterDataSeter();
                    dataSeters[i].playerName = characterDataList[i].playerName;
                    dataSeters[i].point = characterDataList[i].point;
                    dataSeters[i].energy = characterDataList[i].energy;
                    dataSeters[i].characterID = characterDataList[i].characterName;
                    dataSeters[i].buffSpriteList = new List<Sprite>();

                    for (int j = 0;j < characterDataList[i].buffView.Count;j++)
                    {
                        dataSeters[i].buffSpriteList.Add(characterDataList[i].buffView[j].buffIcon);
                    }
                }

                return dataSeters;
            }
            #endregion

            [PunRPC]
            public void FindAndSetCommandObject()
            {
                GameObject cd = GameObject.Find("CommandDirector");
                //PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>();
                //commandDirectorArray[playerCore.playerId] = cd;
                //commandFlows[playerCore.playerId] = cd.GetComponent<CommandFlow>();
                //commandFlows[playerCore.playerId].SetCoreManager(this);

                commandDirectorArray[0] = cd;
                commandFlows[0] = cd.GetComponent<CommandFlow>();
                commandFlows[0].SetCoreManager(this);
            }

            public IEnumerator StaySelectTime()
            {
                selectStart = false;
                // �S�t���O��true�ɂȂ�܂őҋ@
                while(!ListChecker(movesignals) || !ListChecker(attacksignals) || !ListChecker(directionsignals))
                {
                    yield return null;
                }

                phaseCompleteAction();
            }

            bool ListChecker(bool[] lists)
            {
                for (int i = 0;i < lists.Length;i++)
                {
                    if (!lists[i]) return false;
                }

                return true;
            }
        }

    }
}