using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public interface ICharacterDataReceiver
        {
            void MoveDataReceiver(MoveSignal signal, int characterID);
            void AttackDataReceiver(AttackSignal signal, int characterID);
            void StartPositionSeter(FieldIndex fieldIndex, int characterID);
            void CharacterDataReceber(GameObject thisObject, int characterID);
        }
    }
}