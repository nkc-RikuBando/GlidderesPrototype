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
        public MatchingPlayerData[] matchingPlayerData = new MatchingPlayerData[Rule.maxPlayerCount];
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
            GetPlayerData();
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
        public void GetPlayerData(int playerNum, string playerName, int characterID)
        {
            Debug.Log("1回目playerNum = " + playerNum);
            Debug.Log("1回目characterID = " + characterID);
            matchingPlayerData[playerNum] = new MatchingPlayerData
            {
                //ここですべてが0になってる！！！！(player2以降の人)
                playerID = playerNum, //playerID
                playerName = playerName, //playerName
                characterID = characterID //characterID
            };

            playerDatas = matchingPlayerData[playerNum];
            Debug.Log("2回目playerID = " + matchingPlayerData[playerNum].playerID);
            Debug.Log("2回目characterID = " + matchingPlayerData[playerNum].characterID);
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
            
            playerDataArray[PlayerStartBool.myPlayerNum] = playerDatas;

            Debug.Log("値　" + PlayerStartBool.myPlayerNum);
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
            Debug.Log("自分の情報が格納されている配列は " + playerStorage + " 番目");
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

