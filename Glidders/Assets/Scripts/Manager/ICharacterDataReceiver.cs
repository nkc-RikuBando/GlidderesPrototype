using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public interface ICharacterDataReceiver
        {
            void MoveDataReceiver(int signal, int playerID);
            void AttackDataReceiver(int signal, int playerID);
            void DirectionReceiver(int signal, int playerID);
            void StartPositionSeter(int characterID);
            void CharacterDataReceber(string thisObject,string playerName, int playerID, int characterID);
            void RuleDataReceber(bool onlineChecker,int macthingRule,int pointState);
            void CallMethod(string thisObject, string playerName, int playerID, int characterID);

        }
    }
}