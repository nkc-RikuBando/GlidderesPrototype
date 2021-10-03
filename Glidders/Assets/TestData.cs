using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Manager;

namespace Glidders
{
    public class TestData : MonoBehaviour,IGetMatchInformation
    {
        [SerializeField] private int[] debugData_charaID = new int[4];
        public MatchingPlayerData[] GetMatchingPlayerData()
        {
            MatchingPlayerData[] datas = new MatchingPlayerData[ActiveRule.playerCount];

            for (int i = 0;i < datas.Length;i++)
            {
                datas[i] = new MatchingPlayerData();
                datas[i].characterID = debugData_charaID[i];
                datas[i].playerID = i;
                datas[i].playerName = "player" + i.ToString();
            }


            return datas;
        }

        public RuleInfo GetRuleInformation()
        {
            RuleInfo rule = new RuleInfo();
            rule.matchRule = 1;
            rule.setTurn = 30;
            rule.setLife = 0;
            rule.playerNum = 2;
            rule.isOnline = false;
            return rule;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}