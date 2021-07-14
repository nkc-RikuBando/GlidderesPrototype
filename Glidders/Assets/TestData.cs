using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class TestData : MonoBehaviour,IGetMatchInformation
    {
        public MatchingPlayerData[] GetMatchingPlayerData()
        {
            MatchingPlayerData[] datas = new MatchingPlayerData[4];

            for (int i = 0;i < datas.Length;i++)
            {
                datas[i] = new MatchingPlayerData();
                datas[i].characterID = i;
                datas[i].playerID = i;
            }

            datas[0].playerName = "‚¾‚¾‚¾‚¾‚¾‚¾‚¾‚¾‚¾‚¾!!!!!";
            datas[1].playerName = "‚À‚À‚À‚À‚À‚À‚À‚À‚À‚À!!!!!";
            datas[2].playerName = "‚Ã‚Ã‚Ã‚Ã‚Ã‚Ã‚Ã‚Ã‚Ã‚Ã!!!!!";
            datas[3].playerName = "‚Å‚Å‚Å‚Å‚Å‚Å‚Å‚Å‚Å‚Å!!!!!";

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