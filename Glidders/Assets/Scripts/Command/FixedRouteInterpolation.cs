using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    /// <summary>
    /// 開始地点と終着点から、移動情報を生成するためのクラス
    /// </summary>
    public class FixedRouteInterpolation
    {
        /// <summary>
        /// 開始地点（現在のキャラクターの位置）と終着点（移動完了後の位置）から、移動情報を生成します。
        /// </summary>
        /// <param name="characterPosition">開始地点（現在のキャラクターの位置）。</param>
        /// <param name="endPosition">終着点（移動完了後の位置）</param>
        /// <returns>生成した移動情報。</returns>
        public static FieldIndexOffset[] Make(FieldIndex characterPosition, FieldIndexOffset endPosition)
        {
            // 実際の移動情報を格納した配列
            FieldIndexOffset[] moveArray = new FieldIndexOffset[Rule.maxMoveAmount];

            // endPositionの値から大きいほうの長さ（移動量が格納されているほう）を抽出する
            int length = Mathf.Abs(Mathf.Max(endPosition.rowOffset, endPosition.columnOffset));

            // 移動方向を抽出したもの
            FieldIndexOffset duration = endPosition / length;


            for (int i = 0; i <Rule.maxMoveAmount; ++i)
            {
                // iがlength未満（求められた移動量を満たしていない）なら、移動情報を格納する
                if (i < length) moveArray[i] = duration;
                else moveArray[i] = FieldIndexOffset.zero;
            }

            return moveArray;
        }
    }
}
