using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Director
    {
        public struct ResultDataStruct
        {
            public int ruleType;
            public int playerId;
            public string playerName;
            public CharacterName characterId;
            public int point;
            public int totalDamage;

            public ResultDataStruct(int ruleType,int playerId, string playerName, CharacterName characterId, int point, int totalDamage)
            {
                this.ruleType = ruleType;
                this.playerId = playerId;
                this.playerName = playerName;
                this.characterId = characterId;
                this.point = point;
                this.totalDamage = totalDamage;
            }
        }
    }
}