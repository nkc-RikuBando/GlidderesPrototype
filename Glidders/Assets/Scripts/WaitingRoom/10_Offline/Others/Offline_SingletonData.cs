using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class Offline_SingletonData : MonoBehaviour,IGetOfflineInformation
    {
        OfflinePlayerData offlinePlayerData = new OfflinePlayerData();
        OfflineRuleInfo offlineRuleInfo = new OfflineRuleInfo();

        public struct OfflinePlayerData
        {
            public int offlineCharacterID;
            public int offlineCpuCharacterID;
        }

        public struct OfflineRuleInfo
        {
            public int offlineMatchRule;
            public int offlineSetTurn;
            public int offlineSetLife;
        }
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /*OfflinePlayerData‚É’l‚ð“ü‚ê‚È‚ª‚çreturn‚·‚é*/
        public void SetOfflinePlayerData(int characterID)
        {
            offlinePlayerData.offlineCharacterID = characterID;
        }

        public void SetOfflineCpuData(int cpuID)
        {
            offlinePlayerData.offlineCpuCharacterID = cpuID;
        }

        public OfflinePlayerData GetOfflinePlayerData()
        {
            return offlinePlayerData;
        }

        /*OfflineRuleInfo‚É’l‚ð“ü‚ê‚È‚ª‚çreturn‚·‚é*/

        public void SetOfflineRuleInfo(int matchRule, int setTurn, int setLife)
        {

        }

        public OfflineRuleInfo GetOfflineRuleInformation()
        {
            return offlineRuleInfo;
        }



    }
}

