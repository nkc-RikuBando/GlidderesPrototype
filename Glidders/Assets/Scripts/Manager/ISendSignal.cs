using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public interface ISendSignal
        {
            /// <summary>
            /// サーバーに移動情報を送る。
            /// </summary>
            /// <param name="moveSignal">移動情報を格納したMoveSignal構造体。</param>
            void SendMoveSignal(MoveSignal moveSignal);

            void SendAttackSignal(AttackSignal attackSignal);
        }
    }
}