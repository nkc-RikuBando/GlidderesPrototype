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
            /// 指定したプレイヤーの位置情報を更新する。
            /// </summary>
            /// <param name="playerNumber">位置情報を更新するプレイヤーのプレイヤー番号。</param>
            /// <param name="position">新しい位置情報。</param>
            void SetPlayerPosition(int playerNumber, FieldIndex position);
        }
    }
}
