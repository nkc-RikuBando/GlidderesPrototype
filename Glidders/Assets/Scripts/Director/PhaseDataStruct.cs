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
            public PhaseList phaseId;       // ���̃t�F�[�Y��ID�B
            public PhaseList nextPhaseId;   // ���̃t�F�[�Y���ڍs���鎟�̃t�F�[�Y��ID�B
            public Action actionInPhase;    // ���̃t�F�[�Y�Ŏ��s����֐��B

            /// <summary>
            /// �t�F�[�Y�f�[�^��ݒ肵�܂��B
            /// </summary>
            /// <param name="phaseId">���̃t�F�[�Y��ID�B</param>
            /// <param name="nextPhaseId">���̃t�F�[�Y���ڍs���鎟�̃t�F�[�Y��ID�B</param>
            /// <param name="actionInPhase">���̃t�F�[�Y�Ŏ��s����֐��B</param>
            public PhaseDataStruct(PhaseList phaseId, PhaseList nextPhaseId, Action actionInPhase)
            {
                this.phaseId = phaseId;
                this.nextPhaseId = nextPhaseId;
                this.actionInPhase = actionInPhase;
            }
        }
    }
}
