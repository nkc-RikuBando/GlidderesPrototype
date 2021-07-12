using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace  Manager
    {
        public interface IGameDataSeter
        {
            void MatchFormatSeter(int formatNumber);
            void LastTurnSeter(int turn);
        }
    }
}