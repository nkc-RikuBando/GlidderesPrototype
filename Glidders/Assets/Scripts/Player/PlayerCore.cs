using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Player_namespace
    {
//<<<<<<< HEAD
//        public class PlayerCore:MonoBehaviour
//        {
//            // バフリスト
//            public enum BuffList
//            {
//                ATTACKUP, ATTACKDOWN,
//                GARDUP, GARDDOWN,
//                MOVEUP, MOVEDOWN,
//                FIELDUP, FIELDDOWN,
//                LENGTH
//            }

//            [SerializeField] private int playerID = 0; // プレイヤーの番号
//            [SerializeField] private CharacterName characterID; // キャラクターの番号

//            public IEnumerator nemusugi()
//            {
//                bool Sleep = true;

//                yield return new WaitForSeconds(10000);

//                Sleep = false;
//            }
//=======
        public class PlayerCore : MonoBehaviour
        {
            public int playerId { get; private set; } = 0; // キャラクターの番号
            public CharacterName characterId { get; private set; } = 0; // キャラクターのID
// >>>>>>> 8a68d5333eefeee530b648c6b6cb493ed240cf7d

            public void IdSetter(int playerId, CharacterName characterId)
            {
                this.playerId = playerId;
                this.characterId = characterId;
            }

            //public void ID_Receiver(int playerID,CharacterName character)
            //{
            //    this.playerID = playerID;
            //    characterID = character;
            //}
        }
    }
}