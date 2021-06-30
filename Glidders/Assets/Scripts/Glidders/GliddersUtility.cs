using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public struct FieldIndex        // Vector2に代わる二次元配列参照用の構造体
    {
        public int row;        // 行（縦方向）の座標
        public int column;     // 列（横方向）の座標

        public static FieldIndex operator +(FieldIndex a, FieldIndex b)
        {
            FieldIndex ans = a;
            a.row += b.row; a.column += b.column;
            return ans;
        }

        public static FieldIndex operator -(FieldIndex a, FieldIndex b)
        {
            FieldIndex ans = a;
            a.row -= b.row; a.column -= b.column;
            return ans;
        }

        public static FieldIndex operator *(FieldIndex a, float b)
        {
            FieldIndex ans = a;
            a.row = (int)(a.row * b); a.column = (int)(a.column * b);
            return ans;
        }
    }

    public struct FieldIndexOffset        // プレイヤーの座標からみた特定のグリッドの相対座標を格納する構造体
    {
        public int rowOffset { get; set; }     // 行（縦方向）の移動量
        public int columnOffset { get; set; }  // 列（横方向）の移動量

        /// <summary>
        /// 新しくグリッドの相対座標を生成する。
        /// </summary>
        /// <param name="rowOffset">グリッドの行（縦方向）の移動量。</param>
        /// <param name="columnOffset">グリッドの列（横方向）の移動量。</param>
        public FieldIndexOffset(int rowOffset, int columnOffset)
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
        public FieldIndexOffset(int row, int column, int positionRow, int positionColumn)
        {
            rowOffset = row - positionRow;
            columnOffset = column - positionColumn;
        }
    }

    public enum InformationOnGrid  // グリッドごとに記録される情報の種類
    {
        DAMAGE_FIELD,   // ダメージフィールド
        LANDFORM,       // 地形情報
        count,          // このenumの要素数
    }

    public enum CharacterName
    {
        KAITO,
        SEIRA,
    }
}