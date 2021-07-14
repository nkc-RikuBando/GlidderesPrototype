using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Glidders
{
    namespace Director
    {
        public struct PhaseDataStruct
        {
            public PhaseList phaseId;       // このフェーズのID。
            public PhaseList nextPhaseId;   // このフェーズが移行する次のフェーズのID。
            public Action actionInPhase;    // このフェーズで実行する関数。

            /// <summary>
            /// フェーズデータを設定します。
            /// </summary>
            /// <param name="phaseId">このフェーズのID。</param>
            /// <param name="nextPhaseId">このフェーズが移行する次のフェーズのID。</param>
            /// <param name="actionInPhase">このフェーズで実行する関数。</param>
            public PhaseDataStruct(PhaseList phaseId, PhaseList nextPhaseId, Action actionInPhase)
            {
                this.phaseId = phaseId;
                this.nextPhaseId = nextPhaseId;
                this.actionInPhase = actionInPhase;
            }
        }
    }
}
