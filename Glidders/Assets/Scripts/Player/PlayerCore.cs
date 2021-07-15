using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Player_namespace
    {
        public class PlayerCore : MonoBehaviour
        {
            public int playerId { get; private set; } = 0; // プレイヤー番号
            public CharacterName characterId { get; private set; } = 0; // キャラクターID

            public void IdSetter(int playerId, CharacterName characterId)
            {
                this.playerId = playerId;
                this.characterId = characterId;
            }
        }
    }
}