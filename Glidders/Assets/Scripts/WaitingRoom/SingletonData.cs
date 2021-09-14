using Photon.Pun;
using UnityEngine;
using System;

namespace Glidders
{
    public class SingletonData : MonoBehaviour, IGetMatchInformation
    {
        PhotonView view;

        public static int hostNum;
        public static int playerStorage;

        MatchingPlayerData playerDatas = new MatchingPlayerData();
        public static MatchingPlayerData[] playerDataArray = new MatchingPlayerData[Rule.maxPlayerCount];

        //public static List<MatchingPlayerData> playerDataList = new List<MatchingPlayerData>(Rule.maxPlayerCount);

        RuleInfo ruleInfo = new RuleInfo();

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            view = GetComponent<PhotonView>();
        }

        void Update()
        {
            
        }

        public bool isHost()
        {
            return playerDatas.playerID == hostNum;
        }

        public MatchingPlayerData GetPlayerData()
        {
            return playerDatas;
        }

        public RuleInfo GetRuleInformation()
        {
            return ruleInfo;
        }

        public void GetPlayerData(MatchingPlayerData playerDatas)
        {
            this.playerDatas = playerDatas;
            view.RPC(nameof(SetMatchingPlayerData), RpcTarget.All);
        }

        public void GetRuleData(RuleInfo ruleInfo)
        {
            this.ruleInfo = ruleInfo;
        }

        public MatchingPlayerData[] GetMatchingPlayerData()
        {
            //return playerDataList.ToArray();
            return playerDataArray;
        }

        //RPC
        [PunRPC]
        public void SetMatchingPlayerData()
        {
            //playerDataList[PlayerStartBool.myPlayerNum].Add(playerDatas);
            //playerDataList.Add(playerDatas);

            playerDataArray[PlayerStartBool.myPlayerNum] = playerDatas;

            Debug.Log("player1.playerID = " + playerDataArray[0].playerID);
            Debug.Log("player1.playerName = " + playerDataArray[0].playerName);
            Debug.Log("player1.characterID = " + playerDataArray[0].characterID);
            Debug.Log("player2.playerID = " + playerDataArray[1].playerID);
            Debug.Log("player2.playerName = " + playerDataArray[1].playerName);
            Debug.Log("player2.characterID = " + playerDataArray[1].characterID);
            Debug.Log("player3.playerID = " + playerDataArray[2].playerID);
            Debug.Log("player3.playerName = " + playerDataArray[2].playerName);
            Debug.Log("player3.characterID = " + playerDataArray[2].characterID);
            Debug.Log("player4.playerID = " + playerDataArray[3].playerID);
            Debug.Log("player4.playerName = " + playerDataArray[3].playerName);
            Debug.Log("player4.characterID = " + playerDataArray[3].characterID);
        }

        public static int PlayerStorager()
        {
            playerStorage = playerDataArray[PlayerStartBool.myPlayerNum].playerID;
            Debug.Log("é©ï™ÇÃèÓïÒÇ™äiî[Ç≥ÇÍÇƒÇ¢ÇÈîzóÒÇÕ " + playerStorage + " î‘ñ⁄");
            return playerStorage;
        }
    }

    public struct MatchingPlayerData
    {
        public int playerID;
        public string playerName;
        public int characterID;
    }

    public struct RuleInfo
    {
        public int matchRule;
        public int setTurn;
        public int setLife;
    }
}

