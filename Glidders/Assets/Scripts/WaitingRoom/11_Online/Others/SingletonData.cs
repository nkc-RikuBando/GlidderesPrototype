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

        RuleInfo ruleInfo = new RuleInfo();

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            view = GetComponent<PhotonView>();
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

        [PunRPC]
        public void GetPlayerData(int playerNum, string playerName, int characterID)
        {
            matchingPlayerData[playerNum] = new MatchingPlayerData
            {
                playerID = playerNum, //playerID
                playerName = playerName, //playerName
                characterID = characterID //characterID
            };

            playerDatas = matchingPlayerData[playerNum];
            view.RPC(nameof(SetMatchingPlayerData), RpcTarget.All);
            //SetMatchingPlayerData();
        }

        [PunRPC]
        public void SetMatchingPlayerData()
        {

            playerDataArray[playerDatas.playerID] = playerDatas;

            Debug.Log("値　" + PlayerStartBool.myPlayerNum);
            Debug.Log("Player1.playerID = " + playerDataArray[0].playerID);
            Debug.Log("Player1.playerName = " + playerDataArray[0].playerName);
            Debug.Log("Player1.characterID = " + playerDataArray[0].characterID);
            Debug.Log("Player2.playerID = " + playerDataArray[1].playerID);
            Debug.Log("Player2.playerName = " + playerDataArray[1].playerName);
            Debug.Log("Player2.characterID = " + playerDataArray[1].characterID);
            Debug.Log("Player3.playerID = " + playerDataArray[2].playerID);
            Debug.Log("Player3.playerName = " + playerDataArray[2].playerName);
            Debug.Log("Player3.characterID = " + playerDataArray[2].characterID);
            Debug.Log("Player4.playerID = " + playerDataArray[3].playerID);
            Debug.Log("Player4.playerName = " + playerDataArray[3].playerName);
            Debug.Log("Player4.characterID = " + playerDataArray[3].characterID);

            Debug.Log("RuleInfo ルール　" + ruleInfo.matchRule);
            Debug.Log("RuleInfo ターン　" + ruleInfo.setTurn);
            Debug.Log("RuleInfo 体力　" + ruleInfo.setLife);
            Debug.Log("RuleInfo 人数　" + ruleInfo.playerNum);
            Debug.Log("RuleInfo Bool　" + ruleInfo.isOnline);

        }

        public MatchingPlayerData GetPlayerData()
        {
            return playerDatas;
        }

        public RuleInfo GetRuleInformation()
        {
            return ruleInfo;
        }

        public void GetRuleData(int battleRule, int battleTurn ,int battleHp, bool isOnOff)
        {
            ruleInfo.matchRule = battleRule;
            ruleInfo.setTurn = battleTurn;
            ruleInfo.setLife = battleHp;
            ruleInfo.isOnline = isOnOff;
        }

        public void GetPlayerNum(int playerCount)
        {
            ruleInfo.playerNum = playerCount;
        }

        public MatchingPlayerData[] GetMatchingPlayerData()
        {
            //return playerDataList.ToArray();
            return playerDataArray;
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

        public int playerNum;
        public bool isOnline;
    }
}

