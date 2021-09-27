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

            [Header("デバッグ用　仮キャラクター生成")]
            [SerializeField] private GameObject[] players = new GameObject[Rule.maxPlayerCount];

            MatchingPlayerData[] playerDatas = new MatchingPlayerData[Rule.maxPlayerCount];
            ICharacterDataReceiver dataSeter;  // キャラクターデータをマネージャーに渡すインターフェース
            IGetMatchInformation getMatchInformation; // シングルトンからの仮データ受け取りインターフェース
            // Start is called before the first frame update
            void Start()
            {
                getMatchInformation = GameObject.Find("IGetMatchInformation_testObject").GetComponent<TestData>(); // デバッグ用　インターフェース取得
                // getMatchInformation = GameObject.Find("MatchDataSingleton").GetComponent<SingletonData>(); // わたってきたデータを使用する本来の処理

                getMatchInformation = GameObject.Find("IGetMatchInformation_testObject").GetComponent<TestData>(); // デバッグ用　インターフェース取得
                // getMatchInformation = GameObject.Find("MatchDataSingleton(Clone)").GetComponent<SingletonData>(); // わたってきたデータを使用する本来の処理

                dataSeter = GameObject.Find("ManagerCore(Clone)").GetComponent<CoreManager>(); // CoreManagerのインターフェース取得
                playerDatas = getMatchInformation.GetMatchingPlayerData(); // データ受け取りインターフェースからキャラクターデータを取得

                //for(int i = 0;i < Rule.maxPlayerCount;++i)
                //{
                //    Debug.Log($"player[{i}] | playerID = {playerDatas[i].playerID} | chracterID = {playerDatas[i].characterID} | playerName = {playerDatas[i].playerName}");
                //}
                for (int i = 0;i < Rule.maxPlayerCount; i++)
                {
                    PlayerInsatnce(playerDatas[i].playerID,playerDatas[i].characterID); // キャラクターIDをもとに使うキャラクターを確定
                    // players[i] = PhotonNetwork.Instantiate(players[i].name, new Vector3(25,0,0), Quaternion.identity); // キャラクターをインスタンス
                    players[i] = Instantiate(players[i]);
                    players[i].AddComponent<Player_namespace.PlayerCore>();
                    players[i].GetComponent<Player_namespace.PlayerCore>().IdSetter(playerDatas[i].playerID,(CharacterName)playerDatas[i].characterID);
                    dataSeter.CharacterDataReceber(players[i],playerDatas[i].playerName, i,playerDatas[i].characterID); // 対象のデータをインターフェースを通してマネージャーへ
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
        }

    }
}