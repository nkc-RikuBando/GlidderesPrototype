using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class SingletonData : MonoBehaviour, IGetMatchInformation
    {
        public static SingletonData instance;
        MatchingPlayerData[] playerDatas = new MatchingPlayerData[Rule.maxPlayerCount];
        RuleInfo ruleInfo = new RuleInfo();

        public MatchingPlayerData[] GetMatchingPlayerData()
        {
            return playerDatas;
        }

        public RuleInfo GetRuleInformation()
        {
            return ruleInfo;
        }

        public void GetPlayerData(MatchingPlayerData[]playerDatas)
        {
            this.playerDatas = playerDatas;
        }

        public void GetRuleData(RuleInfo ruleInfo)
        {
            this.ruleInfo = ruleInfo;
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

