using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders;

namespace Glidders
{
    public class GameStartObject : MonoBehaviour
    {
        [SerializeField] private GameObject Director;
        private void Awake()
        {
            if (GameObject.Find("MatchDataSingleton") != null)
            {
                ActiveRule.onlineData = GameObject.Find("MatchDataSingleton").GetComponent<IGetMatchInformation>().GetRuleInformation().isOnline;
                ActiveRule.SetMaxTurn(GameObject.Find("MatchDataSingleton").GetComponent<IGetMatchInformation>().GetRuleInformation().playerNum);
                ActiveRule.gameRule = GameObject.Find("MatchDataSingleton").GetComponent<IGetMatchInformation>().GetRuleInformation().matchRule;
                ActiveRule.startPoint = GameObject.Find("MatchDataSingleton").GetComponent<IGetMatchInformation>().GetRuleInformation().setLife;
            }
            else if (GameObject.Find("testDataObject") != null)
            {
                ActiveRule.onlineData = GameObject.Find("testDataObject").GetComponent<TestData>().GetRuleInformation().isOnline;
                ActiveRule.SetMaxTurn(GameObject.Find("testDataObject").GetComponent<TestData>().GetRuleInformation().playerNum);
                ActiveRule.gameRule = GameObject.Find("testDataObject").GetComponent<IGetMatchInformation>().GetRuleInformation().matchRule;
                ActiveRule.startPoint = GameObject.Find("testDataObject").GetComponent<IGetMatchInformation>().GetRuleInformation().setLife;
                //Debug.Log(ActiveRule.onlineData);
                //Debug.Log(ActiveRule.playerCount);
            }

            Instantiate(Director);
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