using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            //Debug.Log("Player1.playerID = " + playerDatas[0].playerID);
            //Debug.Log("Player1.playerName = " + playerDatas[0].playerName);
            //Debug.Log("Player1.charcterID = " + playerDatas[0].characterID);

            //Debug.Log("Player2.playerID = " + playerDatas[1].playerID);
            //Debug.Log("Player2.playerName = " + playerDatas[1].playerName);
            //Debug.Log("Player2.charcterID = " + playerDatas[1].characterID);

            //Debug.Log("Player3.playerID = " + playerDatas[2].playerID);
            //Debug.Log("Player3.playerName = " + playerDatas[2].playerName);
            //Debug.Log("Player3.charcterID = " + playerDatas[2].characterID);

            //Debug.Log("Player4.playerID = " + playerDatas[3].playerID);
            //Debug.Log("Player4.playerName = " + playerDatas[3].playerName);
            //Debug.Log("Player4.charcterID = " + playerDatas[3].characterID);
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
            view.RPC(nameof(SetMatchingPlayerData), RpcTarget.AllBufferedViaServer);
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
        public int battleRule;
        public int setTurn;
        public int setLife;
    }
}

