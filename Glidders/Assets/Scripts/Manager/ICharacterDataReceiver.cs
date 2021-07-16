using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public interface ICharacterDataReceiver
        {
            void MoveDataReceiver(MoveSignal signal, int playerID);
            void AttackDataReceiver(AttackSignal signal, int playerID);
            void DirectionReceiver(DirecionSignal signal, int playerID);
            void StartPositionSeter(FieldIndex fieldIndex, int characterID);
            void CharacterDataReceber(GameObject thisObject,string playerName, int playerID, int characterID);
        }
    }
}