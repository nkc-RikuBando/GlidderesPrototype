using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Director
    {
        public struct ResultDataStruct
        {
            public int playerId;
            public string playerName;
            public CharacterName characterId;
            public int point;
            public int rank;

            public ResultDataStruct(int playerId, string playerName, CharacterName characterId, int point, int rank)
            {
                this.playerId = playerId;
                this.playerName = playerName;
                this.characterId = characterId;
                this.point = point;
                this.rank = rank;
            }
        }
    }
}