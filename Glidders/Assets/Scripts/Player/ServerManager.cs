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
            [Header("キャラクター一覧")]
            [SerializeField] private GameObject[] characterList;

            [SerializeField] private GameObject[] players = new GameObject[2];

            [Header("デバッグ用ボタン")]
            [SerializeField] private bool debugData = true;
            string gameObjName;

            MatchingPlayerData[] playerDatas;
            ICharacterDataReceiver dataSeter;  // キャラクターデータをマネージャーに渡すインターフェース
            IGetMatchInformation getMatchInformation; // シングルトンからの仮データ受け取りインターフェース
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
                director = GameObject.Find("GameDirector(Clone)").GetComponent<Director.GameDirector>(); // ディレクター取得
                if (debugData)  getMatchInformation = GameObject.Find("testDataObject").GetComponent<TestData>(); // デバッグ用　インターフェース取得
                else getMatchInformation = GameObject.Find("MatchDataSingleton").GetComponent<SingletonData>(); // わたってきたデータを使用する本来の処理

                dataSeter = GameObject.Find("ManagerCore(Clone)").GetComponent<CoreManager>(); // CoreManagerのインターフェース取得
                playerDatas = new MatchingPlayerData[ActiveRule.playerCount];

                ruleInfo = getMatchInformation.GetRuleInformation(); // ルール受け取りインターフェースからルールデータ取得

                dataSeter.RuleDataReceber(ruleInfo.isOnline, ruleInfo.matchRule);
                director.SetRule(ruleInfo.playerNum, ruleInfo.setTurn);

                playerDatas = getMatchInformation.GetMatchingPlayerData(); // データ受け取りインターフェースからキャラクターデータを取得

                //for(int i = 0;i < Rule.maxPlayerCount;++i)
                //{
                //    Debug.Log($"player[{i}] | playerID = {playerDatas[i].playerID} | chracterID = {playerDatas[i].characterID} | playerName = {playerDatas[i].playerName}");
                //}
                for (int i = 0;i < ActiveRule.playerCount; i++)
                {
                    PlayerInsatnce(playerDatas[i].playerID,playerDatas[i].characterID); // キャラクターIDをもとに使うキャラクターを確定
                    if(ActiveRule.onlineData) players[i] = PhotonNetwork.Instantiate(players[i].name, new Vector3(25,0,0), Quaternion.identity); // キャラクターをインスタンス
                    else players[i] = Instantiate(players[i]);
                    //view.RPC(nameof(ObjectName), RpcTarget.All,i, playerDatas[i].playerID);
                    gameObjName = players[i].name;
                    players[i].AddComponent<Player_namespace.PlayerCore>();
                    players[i].GetComponent<Player_namespace.PlayerCore>().IdSetter(playerDatas[i].playerID,(CharacterName)playerDatas[i].characterID);

                    Debug.Log(gameObjName);
                    Debug.Log(playerDatas[i].playerName);
                    Debug.Log(i);
                    Debug.Log(playerDatas[i].characterID);
                    dataSeter.CallMethod(gameObjName, playerDatas[i].playerName, i, playerDatas[i].characterID); //Photonのメソッドを呼びために中継役を呼ぶ
                    //dataSeter.CharacterDataReceber(players[i],playerDatas[i].playerName, i,playerDatas[i].characterID); // 対象のデータをインターフェースを通してマネージャーへ
                }

            }

            /// <summary>
            /// キャラクターリストを参照し、使用キャラクターを判別する
            /// </summary>
            /// <param name="playerID">プレイヤーID</param>
            /// <param name="characterID">キャラクターID</param>
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