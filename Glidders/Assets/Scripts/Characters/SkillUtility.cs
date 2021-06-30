using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        public struct SkillRangeGrid        // プレイヤーの座標からみた特定のグリッドの相対座標を格納する構造体
        {
            public int rowOffset { get; set; }     // 行（縦方向）の移動量
            public int columnOffset { get; set; }  // 列（横方向）の移動量

            /// <summary>
            /// 新しくグリッドの相対座標を生成する。
            /// </summary>
            /// <param name="rowOffset">グリッドの行（縦方向）の移動量。</param>
            /// <param name="columnOffset">グリッドの列（横方向）の移動量。</param>
            public SkillRangeGrid(int rowOffset, int columnOffset)
            {
                this.rowOffset = rowOffset;
                this.columnOffset = columnOffset;
            }

            /// <summary>
            /// 新しくグリッドの相対座標を生成する。
            /// </summary>
            /// <param name="row">グリッドの行（縦方向）の絶対座標。</param>
            /// <param name="column">グリッドの列（横方向）の絶対座標。</param>
            /// <param name="positionRow">基準点の行（縦方向）の絶対座標。</param>
            /// <param name="positionColumn">基準点の列（横方向）の絶対座標。</param>
            public SkillRangeGrid(int row, int column, int positionRow, int positionColumn)
            {
                rowOffset = row - positionRow;
                columnOffset = column - positionColumn;
            }
        }
    }
}
