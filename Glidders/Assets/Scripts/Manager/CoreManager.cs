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
        /// �L�����N�^�����i�[����X�g���N�g
        /// </summary>
        public struct CharacterData
        {
            public GameObject thisObject;
            public FieldIndex index;
            public MoveSignal moveSignal;
            public AttackSignal attackSignal;
            public bool canAct{ get; set; }
        }

        public class CoreManager : MonoBehaviour,ICharacterDataReceiver
        {
            const int PLAYER_AMOUNT = 2; // �v���C���[�̑���
            const int PLAYER_MOVE_DISTANCE = 5; // �ړ��̑���

            GameObject[] Characters = new GameObject[PLAYER_AMOUNT]; // ��������CoreManager���ɋL�q
            
            // �e�N���X
            CharacterMove characterMove; 
            IGetFieldInformation getFieldInformation;
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // �f�[�^�̑��ʂ��v���C���[�̑����̕����

            public bool moveStart { get; set; } // �ړ����\���ǂ���
            public bool attackStart { get; set; } // �U�����\���ǂ���
            public List<MoveSignal> MoveSignalList { get; set; } // MoveSignal�̃��X�g
            public List<AttackSignal> AttackSignalList { get; set; } // AttackSignal�̃��X�g

            #region �f�o�b�O�p�ϐ�
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,] 
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset(1, 0), new FieldIndexOffset(0, 0), new FieldIndexOffset(0, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset(0, 1), new FieldIndexOffset(0, 0), new FieldIndexOffset(0, 0), new FieldIndexOffset(0, 0)} };
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                #region ���X�g�̏�����
                MoveSignalList = new List<MoveSignal>(); // ���X�g������

                for (int i = 0; i < PLAYER_AMOUNT;i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region �f�o�b�O�p�@���X�g�����̏����� ����с@���X�g�����̐���
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

                characterDataList[0].index = new FieldIndex(3, 2);
                characterDataList[1].index = new FieldIndex(5, 2);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;
                #endregion
                #endregion

                for (int i = 0;i < characterDirections.Length;i++)
                {
                    characterDirections[i] = characterDataList[i].thisObject.GetComponent<CharacterDirection>();
                }

                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X���擾����
                characterMove = new CharacterMove(getFieldInformation); // CharacterMove�̐����@�擾�����C���^�[�t�F�[�X�̏���n��
            }

            // Update is called once per frame
            void Update()
            {
                // �f�o�b�O�p�@�C���f�b�N�X�̈ʒu��Vector3�ɕϊ����A�ړ�������
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    characterDataList[0].thisObject.transform.position = getFieldInformation.GetTilePosition(new FieldIndex(3, 2));
                }

                // �f�o�b�O�p�@Enter�L�[�Ŏ��s�t���O��true��
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    moveStart = true;
                }

                // �ړ����s�t���O��true�̂Ƃ��AMove�N���X�Ɉړ������s������
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList));

                    moveStart = false;
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