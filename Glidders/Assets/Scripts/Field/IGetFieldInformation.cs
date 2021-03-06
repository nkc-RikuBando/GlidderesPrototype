using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Field
    {
        /// <summary>
        /// フィールドから情報を取得したいときに用いるインターフェース
        /// </summary>
        public interface IGetFieldInformation
        {
            /// <summary>
            /// 指定したプレイヤーがどのグリッドにいるかを取得する。
            /// </summary>
            /// <param name="playerNumber">位置を知りたいプレイヤーのプレイヤー番号。</param>
            /// <returns></returns>
            FieldIndex GetPlayerPosition(int playerNumber);

            /// <summary>
            /// 指定したグリッドにあるダメージフィールドの所有者を取得する。
            /// </summary>
            /// <param name="fieldIndex">指定したグリッドの座標。</param>
            /// <returns>ダメージフィールド所有者のプレイヤー番号。</returns>
            int GetDamageFieldOwner(FieldIndex fieldIndex);

            /// <summary>
            /// 指定したグリッドが通行可能かを取得する。
            /// </summary>
            /// <param name="fieldIndex">指定したグリッドの座標。</param>
            /// <returns>グリッドが通行可能かを表すbool値。trueなら通行可能。</returns>
            bool IsPassingGrid(FieldIndex fieldIndex);

            /// <summary>
            /// 指定したグリッドのscene上でのtransform.positionを取得する。
            /// </summary>
            /// <param name="fieldIndex">指定したグリッドの座標。</param>
            /// <returns></returns>
            Vector3 GetTilePosition(FieldIndex fieldIndex);

            int GetGridCode(FieldIndex fieldIndex);

            FieldIndex GetFieldSize();
        }
    }
}