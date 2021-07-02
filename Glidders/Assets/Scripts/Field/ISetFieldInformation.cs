using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Field
    {
        public interface ISetFieldInformation
        {
            /// <summary>
            /// �w�肵���v���C���[�̈ʒu�����X�V����B
            /// </summary>
            /// <param name="playerNumber">�ʒu�����X�V����v���C���[�̃v���C���[�ԍ��B</param>
            /// <param name="position">�V�����ʒu���B</param>
            void SetPlayerPosition(int playerNumber, FieldIndex position);
        }
    }
}
