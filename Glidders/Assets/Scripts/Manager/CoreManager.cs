using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;
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
            IGetFieldInformation getFieldInformation;
            ISetFieldInformation setFieldInformation;
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // �f�[�^�̑��ʂ��v���C���[�̑����̕����

            [SerializeField] private int lastTurn = 20; // �Q�[���I���^�[��
            public int thisTurn { get; set; } // ���݂̃^�[��
            public int positionSetMenber { get; set; } // �����ʒu��I�����������o�[����c������
            public bool moveStart { get; set; } // �ړ����\���ǂ���
            public bool attackStart { get; set; } // �U�����\���ǂ���
            Phase thisPhase = new Phase(); // �t�F�[�Y���Ǘ�����enum
            MatchFormat format = new MatchFormat(); // �����`�����Ǘ�����enum

            event PhaseMethod phaseEvent = delegate () { }; // �C�x���g����

            private Animator[] animators = new Animator[PLAYER_AMOUNT]; // �A�j���[�V�����Ǘ��̃A�j���[�^�[�ϐ�

            #region �f�o�b�O�p�ϐ�
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,]
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("�f�o�b�O�p�@�X�L���f�[�^")]
            [SerializeField] private Character.SkillScriptableObject[] skillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Awake()
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
                characterDataList[0].index = new FieldIndex(1, 1);
                characterDataList[1].index = new FieldIndex(7, 1);
                characterDataList[2].index = new FieldIndex(1, 7);
                characterDataList[3].index = new FieldIndex(7, 7);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;
                characterDataList[2].canAct = true;
                characterDataList[3].canAct = true;

                characterDataList[0].point = 10000;
                characterDataList[1].point = 10000;
                characterDataList[2].point = 10000;
                characterDataList[3].point = 10000;

                characterDataList[0].energy = 1;
                characterDataList[1].energy = 1;
                characterDataList[2].energy = 1;
                characterDataList[3].energy = 1;
                #endregion


                #region �f�o�b�O�p�@Attack���X�g�̏�����
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left);
                }
                #endregion
                #endregion

                for (int i = 0; i < characterDirections.Length; i++)
                {
                    AttackDataReceiver(characterDataList[i].attackSignal, i); // �U���M�����i�[����
                }

                view = GetComponent<PhotonView>();
                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                setFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                characterMove = new CharacterMove(getFieldInformation, setFieldInformation, characterDirections); // CharacterMove�̐����@�擾�����C���^�[�t�F�[�X�̏���n��
                characterAttack = new CharacterAttack(animators); // CharacterAttack�̐���

                view.RPC(nameof(FindAndSetCommandObject), RpcTarget.AllBufferedViaServer);
            }

            // Update is called once per frame
            void Update()
            {
                // �f�o�b�O�p�@�����ʒu����
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        StartPositionSeter(characterDataList[i].index, i);
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
            public void TurnStart()
            {
                Debug.Log($"����{thisPhase}�̏����͏�����Ă��܂���");

                phaseEvent = ActionSelect;

                thisPhase++;

                // phaseCompleteAction();
            }

            public void ActionSelect()
            {
                Debug.Log($"����{thisPhase}�̏����͏�����Ă��܂���");

                phaseEvent = Move;

                thisPhase++;

                // phaseCompleteAction();
            }

            public void Move()
            {
                moveStart = true;

                // �ړ����s�t���O��true�̂Ƃ��AMove�N���X�Ɉړ������s������
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList,phaseCompleteAction)); // ��������������R���[�`�������s

                    phaseEvent = Attack;

                    // Debug.Log(characterDataList[0].thisObject.name);

                    thisPhase++;

                    moveStart = false;
                }

            }

            public void Attack()
            {
                Debug.Log($"{thisPhase}�̏������s���܂�");

                attackStart = true;

                // �U�����s�t���O��true�̂Ƃ��AAttack�N���X�ɍU�������s������
                if (attackStart)
                {
                    StartCoroutine(characterAttack.AttackOrder(characterDataList, phaseCompleteAction));

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false;

                    phaseEvent = TurnEnd;

                    thisPhase++;
                }
            }

            public void TurnEnd()
            {
                Debug.Log($"����{thisPhase}�̏����͏�����Ă��܂���");
                thisTurn++;
                phaseEvent = TurnStart;
                thisPhase = 0;

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
            /// <param name="characterID">�L�����N�^ID</param>
            public void MoveDataReceiver(MoveSignal signal, int characterID)
            {
                characterDataList[characterID].moveSignal = signal;
            }

            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ����������U���M���ɓn���ꂽ�U���M�����i�[����
            /// </summary>
            /// <param name="signal">�n���U���M��</param>
            /// <param name="playerID">�L�����N�^ID</param>
            public void AttackDataReceiver(AttackSignal signal, int playerID)
            {
                characterDataList[playerID].attackSignal = signal;
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
            }
        }

    }
}