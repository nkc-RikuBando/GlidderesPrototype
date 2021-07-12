using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;

namespace Glidders
{
    namespace Manager
    { 
        /// <summary>
        /// �L�����N�^�����i�[����\����
        /// </summary>
        public struct CharacterData
        {
            public GameObject thisObject;
            public FieldIndex index;
            public MoveSignal moveSignal;
            public AttackSignal attackSignal;
            public bool canAct{ get; set; }
            public int point { get; set; }
        }

        public class CoreManager : MonoBehaviour,ICharacterDataReceiver,IGameDataSeter
        {
            const int PLAYER_AMOUNT = 2; // �v���C���[�̑���
            const int PLAYER_MOVE_DISTANCE = 5; // �ړ��̑���

            private enum Phase
            {
                TURNSTART,ACTIONSERECT,MOVE,ATTACK,TURNEND
            }

            private enum MatchFormat
            {
                POINT,HITPOINT
            }

            GameObject[] Characters = new GameObject[PLAYER_AMOUNT]; // ��������CoreManager���ɋL�q
            
            // �e�N���X
            CharacterMove characterMove;
            CharacterAttack characterAttack;
            IGetFieldInformation getFieldInformation;
            ISetFieldInformation setFieldInformation;
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // �f�[�^�̑��ʂ��v���C���[�̑����̕����

            public int lastTurn= 20; // �Q�[���I���^�[��
            public int thisTurn = 0; // ���݂̃^�[��
            public bool moveStart { get; set; } // �ړ����\���ǂ���
            public bool attackStart { get; set; } // �U�����\���ǂ���
            Phase thisPhase = new Phase();
            MatchFormat format = new MatchFormat();

            #region �f�o�b�O�p�ϐ�
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,] 
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("�f�o�b�O�p�@�X�L���f�[�^")]
            [SerializeField] private Character.SkillScriptableObject[] skillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                #region ���X�g�̏�����
                for (int i = 0; i < PLAYER_AMOUNT;i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region �f�o�b�O�p�@Move���X�g�����̏����� ����с@Move���X�g�����̐���
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[PLAYER_MOVE_DISTANCE];
                    for (int j = 0;j < PLAYER_MOVE_DISTANCE;j++)
                    {
                        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                    }
                    MoveDataReceiver(characterDataList[i].moveSignal, i);
                }
                characterDataList[0].thisObject = GameObject.Find("Kaito");
                characterDataList[1].thisObject = GameObject.Find("Seira");

                characterDataList[0].index = new FieldIndex(3, 1);
                characterDataList[1].index = new FieldIndex(5, 2);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;

                characterDataList[0].point = 10000;
                characterDataList[1].point = 10000; 
                #endregion


                #region �f�o�b�O�p�@Attack���X�g�̏�����
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left);
                }
                #endregion
                #endregion

                for (int i = 0;i < characterDirections.Length;i++)
                {
                    characterDirections[i] = characterDataList[i].thisObject.GetComponent<CharacterDirection>();

                    AttackDataReceiver(characterDataList[i].attackSignal, i);
                }

                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                setFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                characterMove = new CharacterMove(getFieldInformation,setFieldInformation,characterDirections); // CharacterMove�̐����@�擾�����C���^�[�t�F�[�X�̏���n��
                characterAttack = new CharacterAttack(); // CharacterAttack�̐���

                thisPhase = Phase.TURNSTART;
            }

            // Update is called once per frame
            void Update()
            {
                // �t�F�[�Y�Ǘ��p
                switch (thisPhase)
                {
                    case Phase.TURNSTART:
                        break;
                    case Phase.ACTIONSERECT:
                        break;
                    case Phase.MOVE:
                        break;
                    case Phase.ATTACK:
                        break;
                    case Phase.TURNEND:
                        break;
                }

                // �f�o�b�O�p�@Enter�L�[�Ŏ��s�t���O��true��
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    moveStart = true;
                }

                // �f�o�b�O�p�@Space�L�[�Ŏ��s�t���O��true��
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    attackStart = true;
                }

                // �ړ����s�t���O��true�̂Ƃ��AMove�N���X�Ɉړ������s������
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList));

                    moveStart = false;
                }

                // �U�����s�t���O��true�̂Ƃ��AAttack�N���X�ɍU�������s������
                if (attackStart)
                {
                    characterAttack.AttackOrder(ref characterDataList);

                    attackStart = false;
                }

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    for (int i = 0;i < characterDataList.Length;i++)
                    {
                        Debug.Log($"CharacterName is {characterDataList[i].thisObject.name}");
                        Debug.Log($"Index ({characterDataList[i].index.row},{characterDataList[i].index.column}) | point ({characterDataList[i].point})");
                    }
                }
            }

            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ����������ړ��M���ɓn���ꂽ�ړ��M�����i�[����
            /// </summary>
            /// <param name="signal">�n���ړ��M��</param>
            /// <param name="characterID">�L�����N�^ID</param>
            public void MoveDataReceiver(MoveSignal signal,int characterID)
            {
                characterDataList[characterID].moveSignal = signal;
            }

            /// <summary>
            /// �w�肳�ꂽid�̔z��ԍ����������U���M���ɓn���ꂽ�U���M�����i�[����
            /// </summary>
            /// <param name="signal">�n���U���M��</param>
            /// <param name="characterID">�L�����N�^ID</param>
            public void AttackDataReceiver(AttackSignal signal,int characterID)
            {
                characterDataList[characterID].attackSignal = signal;
            }

            /// <summary>
            /// �w�肳�ꂽ�z��ԍ���������FieldIndex�ɑ΂��A�����ʒu��n��
            /// </summary>
            /// <param name="fieldIndex">�n��Index</param>
            /// <param name="characterID">�L�����N�^ID</param>
            public void StartPositionSeter(FieldIndex fieldIndex,int characterID)
            {
                characterDataList[characterID].index = fieldIndex;
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
            /// <param name="characterID">�L�����N�^�[ID</param>
            public void CharacterDataReceber(GameObject thisObject,int characterID)
            {
                characterDataList[characterID].thisObject = thisObject;

                Characters[characterID] = thisObject;
            }
        }

    }
}