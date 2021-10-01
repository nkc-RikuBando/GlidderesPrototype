using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Manager;
using Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        public class ServerManager : MonoBehaviour
        {
            [Header("�L�����N�^�[�ꗗ")]
            [SerializeField] private GameObject[] characterList;

            [SerializeField] private GameObject[] players = new GameObject[2];

            [Header("�f�o�b�O�p�{�^��")]
            [SerializeField] private bool debugData = true;
            string gameObjName;

            MatchingPlayerData[] playerDatas;
            ICharacterDataReceiver dataSeter;  // �L�����N�^�[�f�[�^���}�l�[�W���[�ɓn���C���^�[�t�F�[�X
            IGetMatchInformation getMatchInformation; // �V���O���g������̉��f�[�^�󂯎��C���^�[�t�F�[�X
            RuleInfo ruleInfo;
            Director.GameDirector director;
            PhotonView view;

            // Start is called before the first frame update
            void Start()
            {
                if (!debugData)
                {
                    if (!PhotonNetwork.IsMasterClient) return;
                }

                view = GetComponent<PhotonView>();
                Debug.Log("GameObject.Find(GameDirector) " + (GameObject.Find("GameDirector") == null));
                director = GameObject.Find("GameDirector(Clone)").GetComponent<Director.GameDirector>(); // �f�B���N�^�[�擾
                if (debugData)  getMatchInformation = GameObject.Find("testDataObject").GetComponent<TestData>(); // �f�o�b�O�p�@�C���^�[�t�F�[�X�擾
                else getMatchInformation = GameObject.Find("MatchDataSingleton").GetComponent<SingletonData>(); // �킽���Ă����f�[�^���g�p����{���̏���

                dataSeter = GameObject.Find("ManagerCore(Clone)").GetComponent<CoreManager>(); // CoreManager�̃C���^�[�t�F�[�X�擾
                playerDatas = new MatchingPlayerData[ActiveRule.playerCount];

                ruleInfo = getMatchInformation.GetRuleInformation(); // ���[���󂯎��C���^�[�t�F�[�X���烋�[���f�[�^�擾

                dataSeter.RuleDataReceber(ruleInfo.isOnline, ruleInfo.matchRule);
                director.SetRule(ruleInfo.playerNum, ruleInfo.setTurn);

                playerDatas = getMatchInformation.GetMatchingPlayerData(); // �f�[�^�󂯎��C���^�[�t�F�[�X����L�����N�^�[�f�[�^���擾

                //for(int i = 0;i < Rule.maxPlayerCount;++i)
                //{
                //    Debug.Log($"player[{i}] | playerID = {playerDatas[i].playerID} | chracterID = {playerDatas[i].characterID} | playerName = {playerDatas[i].playerName}");
                //}
                for (int i = 0;i < ActiveRule.playerCount; i++)
                {
                    PlayerInsatnce(playerDatas[i].playerID,playerDatas[i].characterID); // �L�����N�^�[ID�����ƂɎg���L�����N�^�[���m��
                    if(ActiveRule.onlineData) players[i] = PhotonNetwork.Instantiate(players[i].name, new Vector3(25,0,0), Quaternion.identity); // �L�����N�^�[���C���X�^���X
                    else players[i] = Instantiate(players[i]);
                    //view.RPC(nameof(ObjectName), RpcTarget.All,i, playerDatas[i].playerID);
                    gameObjName = players[i].name;
                    players[i].AddComponent<Player_namespace.PlayerCore>();
                    players[i].GetComponent<Player_namespace.PlayerCore>().IdSetter(playerDatas[i].playerID,(CharacterName)playerDatas[i].characterID);

                    Debug.Log(gameObjName);
                    Debug.Log(playerDatas[i].playerName);
                    Debug.Log(i);
                    Debug.Log(playerDatas[i].characterID);
                    dataSeter.CallMethod(gameObjName, playerDatas[i].playerName, i, playerDatas[i].characterID); //Photon�̃��\�b�h���Ăт��߂ɒ��p�����Ă�
                    //dataSeter.CharacterDataReceber(players[i],playerDatas[i].playerName, i,playerDatas[i].characterID); // �Ώۂ̃f�[�^���C���^�[�t�F�[�X��ʂ��ă}�l�[�W���[��
                }

            }

            /// <summary>
            /// �L�����N�^�[���X�g���Q�Ƃ��A�g�p�L�����N�^�[�𔻕ʂ���
            /// </summary>
            /// <param name="playerID">�v���C���[ID</param>
            /// <param name="characterID">�L�����N�^�[ID</param>
            public void PlayerInsatnce(int playerID, int characterID)
            {
                players[playerID] = characterList[characterID];
            }

            [PunRPC]
            public void ObjectName(int i,int id)
            {
                players[i].gameObject.name = "Player" + id.ToString();
            }
        }

    }
}