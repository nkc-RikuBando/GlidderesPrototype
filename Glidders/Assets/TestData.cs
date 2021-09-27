using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Manager;

namespace Glidders
{
    public class TestData : MonoBehaviour,IGetMatchInformation
    {
        public MatchingPlayerData[] GetMatchingPlayerData()
        {
            MatchingPlayerData[] datas = new MatchingPlayerData[Rule.maxPlayerCount];

            for (int i = 0;i < datas.Length;i++)
            {
                datas[i] = new MatchingPlayerData();
                datas[i].characterID = i;
                datas[i].playerID = i;
                datas[i].playerName = "player" + i.ToString();
            }


            return datas;
        }

        public RuleInfo GetRuleInformation()
        {
            throw new System.NotImplementedException();
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