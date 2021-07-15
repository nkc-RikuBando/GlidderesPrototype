using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Manager;

namespace Glidders
{
    namespace Manager
    {
        public class ServerManager : MonoBehaviour
        {
            [Header("�L�����N�^�[�ꗗ")]
            [SerializeField] private GameObject[] characterList;

            [Header("�f�o�b�O�p�@���L�����N�^�[����")]
            [SerializeField] private GameObject[] players = new GameObject[Rule.maxPlayerCount];

            MatchingPlayerData[] playerDatas = new MatchingPlayerData[Rule.maxPlayerCount];
            ICharacterDataReceiver dataSeter;  // �L�����N�^�[�f�[�^���}�l�[�W���[�ɓn���C���^�[�t�F�[�X
            IGetMatchInformation getMatchInformation; // �V���O���g������̉��f�[�^�󂯎��C���^�[�t�F�[�X
            // Start is called before the first frame update
            void Start()
            {
                getMatchInformation = GameObject.Find("IGetMatchInformation_testObject").GetComponent<TestData>(); // �f�o�b�O�p�@�C���^�[�t�F�[�X�擾
                dataSeter = GameObject.Find("ManagerCore(Clone)").GetComponent<CoreManager>(); // CoreManager�̃C���^�[�t�F�[�X�擾
                playerDatas = getMatchInformation.GetMatchingPlayerData(); // �f�[�^�󂯎��C���^�[�t�F�[�X����L�����N�^�[�f�[�^���擾

                for (int i = 0;i < players.Length;i++)
                {
                    PlayerInsatnce(playerDatas[i].playerID,playerDatas[i].characterID); // �L�����N�^�[ID�����ƂɎg���L�����N�^�[���m��
                    players[i] = Instantiate(players[i]); // �L�����N�^�[���C���X�^���X
                    players[i].AddComponent<Player_namespace.PlayerCore>();
                    players[i].GetComponent<Player_namespace.PlayerCore>().IdSetter(playerDatas[i].playerID,(CharacterName)playerDatas[i].characterID);
                    dataSeter.CharacterDataReceber(players[i],playerDatas[i].playerName, i,playerDatas[i].characterID); // �Ώۂ̃f�[�^���C���^�[�t�F�[�X��ʂ��ă}�l�[�W���[��
                }
            }

            /// <summary>
            /// �L�����N�^�[���X�g���Q�Ƃ��A�g�p�L�����N�^�[�𔻕ʂ���
            /// </summary>
            /// <param name="playerID">�v���C���[ID</param>
            /// <param name="characterID">�L�����N�^�[ID</param>
            public void PlayerInsatnce(int playerID,int characterID)
            {
                players[playerID] = characterList[characterID];
            }
        }

    }
}