using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public interface IGetOfflineInformation
    {
        Offline_SingletonData.OfflinePlayerData GetOfflinePlayerData();

        Offline_SingletonData.OfflineRuleInfo GetOfflineRuleInformation();
    }
    
}