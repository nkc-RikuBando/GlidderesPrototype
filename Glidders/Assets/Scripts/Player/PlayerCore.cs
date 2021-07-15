using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Player_namespace
    {
        public class PlayerCore : MonoBehaviour
        {
            public int playerId { get; private set; } = 0; // �v���C���[�ԍ�
            public CharacterName characterId { get; private set; } = 0; // �L�����N�^�[ID

            public void IdSetter(int playerId, CharacterName characterId)
            {
                this.playerId = playerId;
                this.characterId = characterId;
            }
        }
    }
}