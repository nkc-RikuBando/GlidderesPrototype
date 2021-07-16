using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class SingletonData : MonoBehaviour, IGetMatchInformation
    {
        public static SingletonData instance;

        public static int hostNum;
        
        MatchingPlayerData playerDatas = new MatchingPlayerData();

        MatchingPlayerData[] playerDataArray = new MatchingPlayerData[Rule.maxPlayerCount];

        RuleInfo ruleInfo = new RuleInfo();

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
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
        }

        public void GetRuleData(RuleInfo ruleInfo)
        {
            this.ruleInfo = ruleInfo;
        }

        public MatchingPlayerData[] GetMatchingPlayerData()
        {
            return playerDataArray;
        }

        public void SetMatchingPlayerData(MatchingPlayerData[] array)
        {
            playerDataArray = array;
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

