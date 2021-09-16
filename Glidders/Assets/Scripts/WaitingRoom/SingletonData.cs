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

        public void CallMethod(int playerNum,string playerName,int characterID)
        {
            view.RPC(nameof(GetPlayerData), RpcTarget.AllBufferedViaServer, playerNum, playerName, characterID);
        }

        public MatchingPlayerData GetPlayerData()
        {
            return playerDatas;
        }

        public RuleInfo GetRuleInformation()
        {
            return ruleInfo;
        }

        [PunRPC]
        public void GetPlayerData(MatchingPlayerData playerDatas)
        {
            this.playerDatas = playerDatas;
            view.RPC(nameof(SetMatchingPlayerData), RpcTarget.All);
            //SetMatchingPlayerData();
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

            Debug.Log("Player1.playerID = " + playerDataArray[0].playerID);
            Debug.Log("Player1.playerName = " + playerDataArray[0].playerName);
            Debug.Log("Player1.characterID = " + playerDataArray[0].characterID);
            Debug.Log("Player2.playerID = " + playerDataArray[1].playerID);
            Debug.Log("Player2.playerName = " + playerDataArray[1].playerName);
            Debug.Log("Player2.characterID = " + playerDataArray[1].characterID);
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

