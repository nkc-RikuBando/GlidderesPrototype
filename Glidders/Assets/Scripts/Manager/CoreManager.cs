using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Command;
using Glidders.Player_namespace;
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

            Action phaseCompleteAction;
            const int PLAYER_AMOUNT = 4; // �v���C���[�̑���
            const int PLAYER_MOVE_DISTANCE = 5; // �ړ��̑���

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
            IGetFieldInformation getFieldInformation;
            ISetFieldInformation setFieldInformation;
            CommandFlow[] commandFlows = new CommandFlow[Rule.maxPlayerCount];
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // �f�[�^�̑��ʂ��v���C���[�̑����̕����

            [SerializeField] private int lastTurn = 20; // �Q�[���I���^�[��
            public int thisTurn { get; set; } // ���݂̃^�[��
            public int positionSetMenber { get; set; } // �����ʒu��I�����������o�[����c������
            public bool moveStart { get; set; } // �ړ����\���ǂ���
            public bool attackStart { get; set; } // �U�����\���ǂ���
            private bool[] movesignals = new bool[Rule.maxPlayerCount];
            private bool[] directionsignals = new bool[Rule.maxPlayerCount];
            private bool[] attacksignals = new bool[Rule.maxPlayerCount];
            Phase thisPhase = new Phase(); // �t�F�[�Y���Ǘ�����enum
            MatchFormat format = new MatchFormat(); // �����`�����Ǘ�����enum

            event PhaseMethod phaseEvent = delegate () { }; // �C�x���g����

            private Animator[] animators = new Animator[PLAYER_AMOUNT]; // �A�j���[�V�����Ǘ��̃A�j���[�^�[�ϐ�

            [SerializeField] private GameObject serverObject;

            #region �f�o�b�O�p�ϐ�
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,]
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("�f�o�b�O�p�@�X�L���f�[�^")]
            [SerializeField] private Character.SkillScriptableObject[] skillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                #region ���X�g�̏�����
                for (int i = 0; i < PLAYER_AMOUNT; i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region �f�o�b�O�p�@Move���X�g�����̏����� ����с@Move���X�g�����̐���
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[PLAYER_MOVE_DISTANCE];
                    for (int j = 0; j < PLAYER_MOVE_DISTANCE; j++)
                    {
                        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                    }
                    MoveDataReceiver(characterDataList[i].moveSignal, i);
                }

                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].canAct = true;
                    characterDataList[i].point = 10000;
                    characterDataList[i].energy = 1;
                    characterDataList[i].direcionSignal.direction = FieldIndexOffset.left;
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;
                }

                characterDataList[0].index = new FieldIndex(5, 1);
                characterDataList[1].index = new FieldIndex(7, 1);
                characterDataList[2].index = new FieldIndex(5, 7);
                characterDataList[3].index = new FieldIndex(7, 7);

                #endregion


                #region �f�o�b�O�p�@Attack���X�g�̏�����
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left,i);
                }
                #endregion
                #endregion

                for (int i = 0; i < characterDataList.Length; i++)
                {
                    StartPositionSeter(characterDataList[i].index, i);
                }

                Instantiate(serverObject);

                for (int i = 0; i < characterDirections.Length; i++)
                {
                    AttackDataReceiver(characterDataList[i].attackSignal, i); // �U���M�����i�[����
                }

                view = GetComponent<PhotonView>();
                fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>();
                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                setFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                characterMove = new CharacterMove(getFieldInformation, setFieldInformation, characterDirections); // CharacterMove�̐����@�擾�����C���^�[�t�F�[�X�̏���n��
                characterAttack = new CharacterAttack(animators,fieldCore,displayTileMap,characterDirections); // CharacterAttack�̐���

                // �f�o�b�O�p�@�O�X�L�����������������Ɍ������Ɖ���
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.up, FieldIndexOffset.down, i);
                }

                view.RPC(nameof(FindAndSetCommandObject), RpcTarget.AllBufferedViaServer);
            }

            // Update is called once per frame
            void Update()
            {
                // �f�o�b�O�p�@�����ʒu����
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    phaseCompleteAction();
                }

                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    for (int i = 0; i < Rule.maxPlayerCount; i++)
                    {
                        movesignals[i] = true;
                        attacksignals[i] = true;
                        directionsignals[i] = true;
                    }
                }

                if (PLAYER_AMOUNT > positionSetMenber) return;

                if (thisTurn < lastTurn)
                {
                    // �f�o�b�O�p�@�C�x���g�ɓo�^����Ă���֐������s
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        phaseEvent();
                    }
                }

                // �f�o�b�O�p�@�L�����N�^�[�����m�F
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        Debug.Log($"CharacterName is {characterDataList[i].thisObject.name}");
                        Debug.Log($"Index ({characterDataList[i].index.row},{characterDataList[i].index.column}) | point ({characterDataList[i].point})");
                    }
                }
            }

            #region �e��^�[������
            [PunRPC]
            public void TurnStart()
            {
                Debug.Log($"����{thisPhase}�̏����͏�����Ă��܂���");

                phaseEvent = ActionSelect;

                thisPhase++;

                phaseCompleteAction();
            }

            [PunRPC]
            public void ActionSelect()
            {
                Debug.Log($"����{thisPhase}�̏����͏�����Ă��܂���");

                PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>();

                // commandFlows[playerCore.playerId].StartCommandPhase(playerCore.playerId,characterDataList[playerCore.playerId].thisObject,characterDataList[playerCore.playerId].index);

                StartCoroutine(StaySelectTime());

                phaseEvent = Move;
                
                moveStart = true;
                thisPhase++;

                phaseCompleteAction();
            }

            [PunRPC]
            public void Move()
            {
                Debug.Log($"Move�̏������s���܂�({thisPhase})");

                // moveStart = true;

                // �ړ����s�t���O��true�̂Ƃ��AMove�N���X�Ɉړ������s������
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList, phaseCompleteAction)); // ��������������R���[�`�������s

                    phaseEvent = Attack;

                    // Debug.Log(characterDataList[0].thisObject.name);

                    thisPhase++;

                    attackStart = true;
                    moveStart = false;
                }
                else phaseCompleteAction();

            }

            [PunRPC]
            public void Attack()
            {
                Debug.Log($"Attack�̏������s���܂�({thisPhase})");

                // �f�o�b�O�p�@�X�L����������
                switch(thisTurn % 4)
                {
                    case 0: // ��
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.up, FieldIndexOffset.up, i);
                        }
                        break;
                    case 1: // ��
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.down, FieldIndexOffset.down, i);
                        }
                        break;
                    case 2: // ��
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.left, FieldIndexOffset.left, i);
                        }
                        break;
                    case 3: // �E
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.right, FieldIndexOffset.right, i);
                        }
                        break;
                }

                // Debug.Log(attackStart);

                attackStart = true;

                // �U�����s�t���O��true�̂Ƃ��AAttack�N���X�ɍU�������s������
                if (attackStart)
                {
                    Debug.Log("Lets.Attack");
                    StartCoroutine(characterAttack.AttackOrder(characterDataList, phaseCompleteAction));

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false;

                    phaseEvent = TurnEnd;

                    thisPhase++;
                }
                else phaseCompleteAction();
            }

            [PunRPC]
            public void TurnEnd()
            {
                Debug.Log($"����{thisPhase}�̏����͏�����Ă��܂���");
                displayTileMap.DisplayDamageFieldTilemap(fieldCore.GetFieldData());
                thisTurn++;
                phaseEvent = TurnStart;
                thisPhase = 0;
                phaseCompleteAction();

                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;
                }

                for (int i = 0;i < characterDataList.Length;i++)
                {
                    characterDataList[i].energy++;
                }

                if (thisTurn >= lastTurn)
                {
                    Debug.Log($"�Ō�̃^�[�����I���܂����@�������I�����܂�");
                }
            }
            #endregion

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

                if (PLAYER_AMOUNT == positionSetMenber)
                {
                    thisPhase = Phase.TURN_START;

                    phaseEvent = TurnStart;
                }
            }

            /// <summary>
            /// �Q�[�����I������^�[����ݒ肵�Ă��炤
            /// </summary>
            /// <param name="turn">�n���^�[��</param>
            public void LastTurnSeter(int turn)
            {
                lastTurn = turn;
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
                characterDataList[playerID].characterName = (CharacterName)characterID;

                animators[playerID] = characterDataList[playerID].thisObject.GetComponent<Animator>(); // �A�j���[�^�[�擾
                characterDirections[playerID] = characterDataList[playerID].thisObject.GetComponent<CharacterDirection>(); // �e�L�����N�^�[����]������N���X���擾����

                // Debug.Log($"CharacterID{characterID}����playerName{playerName}�������Ƃ�܂��� objectName��{characterDataList[characterID].thisObject.name}");
            }

            public void SetPhaseCompleteAction(Action phaseCompleteAction)
            {
                this.phaseCompleteAction = phaseCompleteAction;
            }

            public UICharacterDataSeter[] characterDataSeter()
            {
                UICharacterDataSeter[] dataSeters = new UICharacterDataSeter[PLAYER_AMOUNT];

                for (int i = 0;i < dataSeters.Length;i++)
                {
                    dataSeters[i] = new UICharacterDataSeter();
                    dataSeters[i].playerName = characterDataList[i].playerName;
                    dataSeters[i].point = characterDataList[i].point;
                    dataSeters[i].energy = characterDataList[i].energy;
                    dataSeters[i].characterID = characterDataList[i].characterName;
                }

                return dataSeters;
            }
            #endregion

            [PunRPC]
            public void FindAndSetCommandObject()
            {
                GameObject cd = GameObject.Find("CommandDirector");
                PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>();
                commandDirectorArray[playerCore.playerId] = cd;
                commandFlows[playerCore.playerId] = cd.GetComponent<CommandFlow>();
                commandFlows[playerCore.playerId].SetCoreManager(this);
            }

            public IEnumerator StaySelectTime()
            {
                while(!movesignals[0] || !movesignals[1] || !movesignals[2] || !movesignals[3] || !attacksignals[0] || !attacksignals[1] || !attacksignals[2] || !attacksignals[3] || !directionsignals[0] || !directionsignals[1] || !directionsignals[2] || !directionsignals[3])
                {
                    yield return null;
                }

                phaseCompleteAction();
            }

        }

    }
}