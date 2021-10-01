using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Command;
using Glidders.Player_namespace;
using Glidders.Buff;
using Glidders.Character;
using System;
using Photon;
using Photon.Pun;
using Glidders.Director;

namespace Glidders
{
    namespace Manager
    { 
        delegate void PhaseMethod(); // �t�F�[�Y���Ƃ̍s�����L�^�����֐���o�^����f���Q�[�g
        public class CoreManager : MonoBehaviour, ICharacterDataReceiver, IPhaseInformation, IPlayerInformation
        {
            PhotonView view;

            Action phaseCompleteAction; // �f���Q�[�g

            GameObject[] commandDirectorArray = new GameObject[ActiveRule.playerCount];

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
            CommandFlow[] commandFlows = new CommandFlow[ActiveRule.playerCount];
            CharacterDirection[] characterDirections = new CharacterDirection[ActiveRule.playerCount];

            CharacterData[] characterDataList = new CharacterData[ActiveRule.playerCount]; // �f�[�^�̑��ʂ��v���C���[�̑����̕����



            public int ruleData { get; set; }
            public static bool onlineData { get; set; }
            public int positionSetMenber { get; set; } // �����ʒu��I�����������o�[����c������
            public bool moveStart { get; set; } // �ړ����\���ǂ���
            public bool attackStart { get; set; } // �U�����\���ǂ���
            private bool selectStart { get; set; } // ���͉\���ǂ���
            private bool[] movesignals = new bool[ActiveRule.playerCount];
            private bool[] directionsignals = new bool[ActiveRule.playerCount];
            private bool[] attacksignals = new bool[ActiveRule.playerCount];
            MatchFormat format = new MatchFormat(); // �����`�����Ǘ�����enum

            private Animator[] animators = new Animator[ActiveRule.playerCount]; // �A�j���[�V�����Ǘ��̃A�j���[�^�[�ϐ�
            private Text[] texts = new Text[ActiveRule.playerCount];

            // �����o�t���Ǘ�����List<List<int>>
            private List<List<int>> removeNumber_value = new List<List<int>>(0);
            private List<List<int>> removeNumber_view = new List<List<int>>(0);

            [SerializeField] private GameObject serverObject;
            [SerializeField] private UniqueSkillScriptableObject notActionSkill;

            [SerializeField] private int[] playerIndex_row = new int[ActiveRule.playerCount];
            [SerializeField] private int[] playerIndex_colomn = new int[ActiveRule.playerCount];

            int actionCompleateManber = 0;

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
                view = GetComponent<PhotonView>();

                selectStart = true;
                #region ���X�g�̏�����
                for (int i = 0; i < ActiveRule.playerCount; i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region �f�o�b�O�p�@Move���X�g�����̏����� ����с@Move���X�g�����̐���
                //for (int i = 0; i < characterDataList.Length; i++)
                //{
                //    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[Rule.maxMoveAmount];
                //    for (int j = 0; j < Rule.maxMoveAmount; j++)
                //    {
                //        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                //    }
                //}

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
                    characterDataList[i].buffEffectObject = new List<GameObject>();
                }

                for (int i = 0;i < characterDataList.Length;i++)
                {
                    characterDataList[i].index = new FieldIndex(playerIndex_row[i], playerIndex_colomn[i]);
                }

                #endregion


                #region �f�o�b�O�p�@Attack���X�g�̏�����
                //for (int i = 0; i < characterDataList.Length; i++)
                //{
                //    characterDataList[i].attackSignal = new AttackSignal(true, UniqueSkillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left,i);
                //}
                #endregion
                #endregion

                for (int i = 0; i < characterDataList.Length; i++)
                {
                    view.RPC(nameof(StartPositionSeter),RpcTarget.All, i);
                    // commandFlows[0] = GameObject.Find("CommandDirector").GetComponent<CommandFlow>();
                }

                Instantiate(serverObject);

                //for (int i = 0; i < characterDirections.Length; i++)
                //{
                //    AttackDataReceiver(characterDataList[i].attackSignal, i); // �U���M�����i�[����
                //}

                Debug.Log("GameObject.Find(Vcam) " + (GameObject.Find("Vcam") == null));
                cameraController = GameObject.Find("Vcam").GetComponentInChildren<CameraController>();
                fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �N���X�擾
                displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>(); // �N���X�擾
                characterMove = new CharacterMove(fieldCore, characterDirections,texts,animators); // CharacterMove�̐����@�擾�����C���^�[�t�F�[�X�̏���n��
                characterAttack = new CharacterAttack(animators,fieldCore,displayTileMap,characterDirections,cameraController,texts); // CharacterAttack�̐���
                autoSignalSelecter = new AutoSignalSelecter(fieldCore,notActionSkill);

                // FindAndSetCommandObject();
                view.RPC(nameof(FindAndSetCommandObject), RpcTarget.AllBufferedViaServer);
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
                    for (int i = 1; i < ActiveRule.playerCount; i++)
                    {
                        characterDataList[i] = autoSignalSelecter.SignalSet(characterDataList[i],characterDataList[0]);
                        movesignals[i] = true;
                        attacksignals[i] = true;
                        directionsignals[i] = true;
                    }
                }

                // if (ActiveRule.playerCount > positionSetMenber) return;

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

            
            public void TurnStart()
            {
                Debug.Log("�R�[���Ăׂ�");
                view.RPC(nameof(CallTurnStart), RpcTarget.All);
            }

            [PunRPC]
            public void CallTurnStart()
            {
                Debug.Log("�R�[���Ă΂���");
                if (onlineData)
                {
                    cameraController = GameObject.Find("Vcam").GetComponentInChildren<CameraController>();
                    fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �N���X�擾
                    displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>(); // �N���X�擾
                    characterMove = new CharacterMove(fieldCore, characterDirections, texts, animators); // CharacterMove�̐����@�擾�����C���^�[�t�F�[�X�̏���n��
                    characterAttack = new CharacterAttack(animators, fieldCore, displayTileMap, characterDirections, cameraController, texts); // CharacterAttack�̐���
                    autoSignalSelecter = new AutoSignalSelecter(fieldCore, notActionSkill);
                }

                // �L�����N�^�̈ʒu�𔽉f(�����̈ʒu���𔽉f���邽��)
                for (int i = 0; i < ActiveRule.playerCount; i++)
                {
                    Debug.Log("�Ă΂ꂽ2");
                    Debug.Log($"thisObject({i}) = {characterDataList[i].thisObject}");
                    Debug.Log($"index({i}) = ({characterDataList[i].index.row},{characterDataList[i].index.column})");
                    Debug.Log(fieldCore == null);
                    characterDataList[i].thisObject.transform.position = fieldCore.GetTilePosition(characterDataList[i].index);
                }
                view.RPC(nameof(CallPhaseCompleteAction), RpcTarget.All); //phaseCompleteAction();
            }

            public void ActionSelect()
            {
                view.RPC(nameof(CallActionSelect), RpcTarget.All);
            }

            [PunRPC]
            public void CallActionSelect()
            {
                if (!selectStart) return;
                // Debug.Log(characterDataList[0].index.column + " : " + characterDataList[0].index.row);
                float movebuff = 0;

                Debug.Log("PhotonNetwork.PlayerList.Length " + PhotonNetwork.PlayerList.Length);
                if (onlineData)
                {
                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                        {
                            Debug.Log("i " + i);
                            for (int j = 0; j < characterDataList[i].buffView.Count; j++)
                            {
                                for (int I = 0; I < characterDataList[i].buffValue[j].Count; I++)
                                {
                                    if (characterDataList[i].buffValue[j][I].buffedStatus == Character.StatusTypeEnum.MOVE)
                                    {
                                        movebuff += characterDataList[i].buffValue[j][I].buffScale;
                                    }
                                }
                            }

                            Debug.Log($"characterDataList[{i}].thisObject = {characterDataList[i].thisObject.name}");
                            Debug.Log($"characterDataList[{i}].index = ({characterDataList[i].index.row},{characterDataList[i].index.column})");
                            Debug.Log($"characterDataList[{i}].energy = {characterDataList[i].energy}");
                            commandFlows[i].StartCommandPhase(i, characterDataList[i].thisObject, characterDataList[i].index, (int)movebuff, characterDataList[i].energy);

                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < characterDataList[0].buffView.Count; i++)
                    {
                        for (int j = 0; j < characterDataList[0].buffValue[i].Count; j++)
                        {
                            if (characterDataList[0].buffValue[i][j].buffedStatus == Character.StatusTypeEnum.MOVE)
                                movebuff += characterDataList[0].buffValue[i][j].buffScale;
                        }
                    }

                    // �f�o�b�O�p�@�ŏ��̃L�����݈̂ړ��������s��
                    commandFlows[0].StartCommandPhase(0, characterDataList[0].thisObject, characterDataList[0].index, (int)movebuff, characterDataList[0].energy);
                }


                view.RPC(nameof(StaySelectTime),RpcTarget.All) ; // �S�L�����̃R�}���h����������܂őҋ@����

                moveStart = true; // �ړ����\�ɂ���

                // phaseCompleteAction(); // �f�o�b�O�p�@�t���O�Ǘ��𖳎����Ď��̃t�F�[�Y��

                // ��L�̏������O���ꍇ�A�E�V�t�g�������ƃt���O��true�ɂȂ�

            }

            public void Move()
            {
                view.RPC(nameof(CallActionSelect), RpcTarget.All);
            }

            [PunRPC]
            public void CallMove()
            {
                selectStart = true;
                // �ړ����s�t���O��true�̂Ƃ��AMove�N���X�Ɉړ������s������
                if (moveStart)
                {
                    cameraController.ClearTarget(); // �S�ẴJ�����Ǐ]�Ώۂ���������
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        cameraController.AddTarget(characterDataList[i].thisObject.transform);
                    }

                    StartCoroutine(characterMove.MoveOrder(characterDataList,  phaseCompleteAction,onlineData)); // ��������������R���[�`�������s

                    attackStart = true; // �U�����\�ɂ���
                    moveStart = false; // �ړ���s�\�ɂ���
                }
            }

            [PunRPC]
            public void Attack()
            {
                view.RPC(nameof(CallAttack),RpcTarget.All);
            }

            [PunRPC]
            public void CallAttack()
            {
                // �U�����s�t���O��true�̂Ƃ��AAttack�N���X�ɍU�������s������
                if (attackStart)
                {
                    // Debug.Log("Lets.Attack");
                    StartCoroutine(characterAttack.AttackOrder(characterDataList, phaseCompleteAction,onlineData)); // �U������������R���[�`�������s

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false; // �U����s�\�ɂ���
                }
            }

            [PunRPC]
            public void TurnEnd()
            {
                view.RPC(nameof(CallTurnEnd), RpcTarget.All);
            }
            #endregion

            [PunRPC]
            public void CallTurnEnd()
            {
                // �^�[���I�����̃_���[�W�t�B�[���h��������
                fieldCore.UpdateFieldData();
                displayTileMap.ClearDamageFieldTilemap();
                displayTileMap.DisplayDamageFieldTilemap(fieldCore.GetFieldData());

                // �e�R�}���h���͏���������
                for (int i = 0; i < ActiveRule.playerCount; i++)
                {
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;

                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[5];
                    characterDataList[i].attackSignal.skillData = null;
                }

                // �e�L�����N�^�̃G�i�W�[��ǉ��A�s���s�\��Ԃ����� �܂��@�o�t�̃^�[���ɂ����ŏ���
                for (int i = 0; i < characterDataList.Length; i++)
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
                    int number = removeNumber_value.Count - 1; // ���Ń^�[���̃o�t����
                    for (int i = number; i >= 0; i--)
                    {
                        // ���X�g����o�^���Ă������o�t������
                        ListRemover(removeNumber_value[i][0], removeNumber_value[i][1], removeNumber_value[i][2]);
                    }

                    removeNumber_value = new List<List<int>>(0); // �o�^�����폜����
                }

                // �o�t���e�����ׂď������o�t��������
                if (removeNumber_view.Count > 0)
                {
                    int number = removeNumber_view.Count - 1; // ���ł���o�t��񑍐�
                    for (int i = number; i >= 0; i--)
                    {
                        // ���X�g����o�^���Ă������o�t������
                        ListRemover(removeNumber_view[i][0], removeNumber_view[i][1]);
                    }

                    removeNumber_view = new List<List<int>>(0); // �o�^�����폜����
                }

                phaseCompleteAction();
            }

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
                if (characterDataList[i].buffEffectObject[j] != null) Destroy(characterDataList[i].buffEffectObject[j]);
                characterDataList[i].buffEffectObject.RemoveAt(j);

                characterDataList[i].buffValue.RemoveAt(j);
                characterDataList[i].buffView.RemoveAt(j);
            }

            #region �e��C���^�[�t�F�[�X
            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ����������ړ��M���ɓn���ꂽ�ړ��M�����i�[����
            /// </summary>
            /// <param name="moveID">�n���ړ��M��</param>
            /// <param name="playerID">�L�����N�^ID</param>
            public void MoveDataReceiver(int moveID, int playerID)
            {
                characterDataList[playerID].moveSignalNumber = moveID;
                movesignals[playerID] = true;
            }

            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ��������������]���M���ɓn���ꂽ�����]���M�����i�[����
            /// </summary>
            /// <param name="directionID"></param>
            /// <param name="playerID"></param>
            public void DirectionReceiver(int directionID, int playerID)
            {
                characterDataList[playerID].directionSignalNumber = directionID;
                directionsignals[playerID] = true;
            }

            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ����������U���M���ɓn���ꂽ�U���M�����i�[����
            /// </summary>
            /// <param name="attackID">�n���U���M��</param>
            /// <param name="playerID">�L�����N�^ID</param>
            public void AttackDataReceiver(int attackID, int playerID)
            {
                characterDataList[playerID].attackSignalNumber = attackID;
                attacksignals[playerID] = true;
            }

            /// <summary>
            /// �w�肳�ꂽ�z��ԍ���������FieldIndex�ɑ΂��A�����ʒu�����炤
            /// </summary>
            /// <param name="fieldIndex">�n��Index</param>
            /// <param name="playerID">�L�����N�^ID</param>
            [PunRPC]
            public void StartPositionSeter(int playerID)
            {
                characterDataList[playerID].index = new FieldIndex(playerIndex_row[playerID], playerIndex_colomn[playerID]);

                if (ActiveRule.playerCount == positionSetMenber)
                {
                    CallPhaseCompleteAction();
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
            /// 
            [PunRPC]
            public void CharacterDataReceber(string thisObject,string playerName,int playerID,int characterID)
            {
                // �e��L�����N�^�[�����擾���A�\���̂ɕۑ����Ă���
                characterDataList[playerID].thisObject = GameObject.Find(thisObject);
                characterDataList[playerID].playerName = playerName;
                characterDataList[playerID].playerNumber = playerID;
                characterDataList[playerID].characterName = (CharacterName)characterID;

                Debug.Log($"CharacterID = {characterID}");
                Debug.Log($"playerName = {playerName}");
                Debug.Log(thisObject);
                Debug.Log($"objectName = {characterDataList[characterID].thisObject.name}");

                animators[playerID] = characterDataList[playerID].thisObject.GetComponent<Animator>(); // �A�j���[�^�[�擾
                texts[playerID] = characterDataList[playerID].thisObject.GetComponentInChildren<Text>(); // �e�L�X�g�擾
                characterDirections[playerID] = characterDataList[playerID].thisObject.GetComponent<CharacterDirection>(); // �e�L�����N�^�[����]������N���X���擾����

                
            }

            /// <summary>
            /// ���������̃f���Q�[�g��ݒ肵�Ă��炤
            /// </summary>
            /// <param name="phaseCompleteAction"></param> 
            [PunRPC]
            public void SetPhaseCompleteAction(Action phaseCompleteAction)
            {
                
            }

            public void CallSetPhaseCompleteAction(Action phaseCompleteAction)
            {
                this.phaseCompleteAction = phaseCompleteAction;
            }
            /// <summary>
            /// UI���ɃL�����N�^�f�[�^��n���֐�
            /// </summary>
            /// <returns>�ԋp����\����</returns>
            public UICharacterDataSeter[] characterDataSeter()
            {
                UICharacterDataSeter[] dataSeters = new UICharacterDataSeter[ActiveRule.playerCount];

                for (int i = 0;i < dataSeters.Length;i++)
                {
                    dataSeters[i] = new UICharacterDataSeter();
                    dataSeters[i].playerName = characterDataList[i].playerName;
                    dataSeters[i].point = characterDataList[i].point;
                    dataSeters[i].energy = characterDataList[i].energy;
                    dataSeters[i].characterID = characterDataList[i].characterName;
                    dataSeters[i].buffSpriteList = new List<string>();

                    for (int j = 0;j < characterDataList[i].buffView.Count;j++)
                    {
                        dataSeters[i].buffSpriteList.Add(characterDataList[i].buffView[j].id);
                    }
                }

                return dataSeters;
            }

            public void RuleDataReceber(bool onlineCheck,int matchingData)
            {
                onlineData = onlineCheck;
                ruleData = matchingData;
            }

            #endregion

            [PunRPC]
            public void FindAndSetCommandObject()
            {
                GameObject cd = GameObject.Find("CommandDirector");

                if (onlineData)
                {
                    PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>();
                    commandDirectorArray[playerCore.playerId] = cd;
                    commandFlows[playerCore.playerId] = cd.GetComponent<CommandFlow>();
                    commandFlows[playerCore.playerId].SetCoreManager(this);
                }
                else
                {
                    commandDirectorArray[0] = cd;
                    commandFlows[0] = cd.GetComponent<CommandFlow>();
                    commandFlows[0].SetCoreManager(this);
                }
            }

            [PunRPC]
            public IEnumerator StaySelectTime()
            {
                selectStart = false;
                // �S�t���O��true�ɂȂ�܂őҋ@
                while(!ListChecker(movesignals) || !ListChecker(attacksignals) || !ListChecker(directionsignals))
                {
                    yield return null;
                }

                CallPhaseCompleteAction();
            }

            bool ListChecker(bool[] lists)
            {
                for (int i = 0;i < lists.Length;i++)
                {
                    if (!lists[i]) return false;
                }

                return true;
            }

            public ResultDataStruct[] GetResultData(ResultDataStruct[] resultDataStruct)
            {
                for (int i = 0;i < resultDataStruct.Length;i++)
                {
                    resultDataStruct[i].characterId = characterDataList[i].characterName;
                    resultDataStruct[i].playerId = characterDataList[i].playerNumber;
                    resultDataStruct[i].playerName = characterDataList[i].playerName;
                    resultDataStruct[i].point = characterDataList[i].point;
                    resultDataStruct[i].ruleType = ruleData;
                    resultDataStruct[i].totalDamage = characterDataList[i].totalDamage;
                }

                return resultDataStruct;
            }

            public void CallMethod(string thisObject, string playerName, int playerID, int characterID)
            {
                view.RPC(nameof(CharacterDataReceber), RpcTarget.All, thisObject, playerName, playerID, characterID);
            }

            [PunRPC]
            public void CallPhaseCompleteAction()
            {
                if (onlineData)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        actionCompleateManber++;
                        Debug.Log("actionCompleateManber" + actionCompleateManber);
                        Debug.Log("ActiveRule.playerCount " + ActiveRule.playerCount);
                        if (actionCompleateManber >= ActiveRule.playerCount)
                        {
                            phaseCompleteAction();
                            actionCompleateManber = 0;
                        }
                    }
                }
                else
                {
                    phaseCompleteAction();
                }
            }
            public void offlineSignalSeter(AttackSignal attackSignal, MoveSignal moveSignal, DirecionSignal direcionSignal)
            {
                characterDataList[0].attackSignal = attackSignal;
                characterDataList[0].moveSignal = moveSignal;
                characterDataList[0].direcionSignal = direcionSignal;
                characterDataList[1] = autoSignalSelecter.SignalSet(characterDataList[1], characterDataList[0]);

                phaseCompleteAction();
            }
        }

    }
}