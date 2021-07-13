using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class SingletonData : MonoBehaviour
    {
        public struct PlayerData
        {
            public int playerID;
            public string playerName;
            public int characterID;
        }

        public struct RuleInfo
        {
            public int battleRule;
        }
    }
}

