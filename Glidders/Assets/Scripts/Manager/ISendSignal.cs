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
            /// �T�[�o�[�Ɉړ����𑗂�B
            /// </summary>
            /// <param name="moveSignal">�ړ������i�[����MoveSignal�\���́B</param>
            void SendMoveSignal(MoveSignal moveSignal);

            void SendAttackSignal(AttackSignal attackSignal);
        }
    }
}