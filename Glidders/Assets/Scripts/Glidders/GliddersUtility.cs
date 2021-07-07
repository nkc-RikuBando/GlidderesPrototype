using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    /// <summary>
    /// #Glidders!!で大前提とされるルールをまとめたクラス。マジックナンバー対策。
    /// </summary>
    public static class Rule
    {
        public static readonly int skillCount = 3;     // キャラクターが持つスキルの数
        public static readonly int maxMoveAmount = 5;  // キャラクターの最大移動量
    }

    public struct FieldIndex        // Vector2に代わる二次元配列参照用の構造体
    {
        public int row;        // 行（縦方向）の座標
        public int column;     // 列（横方向）の座標

        /// <summary>
        /// = new FieldIndex(0, 0)
        /// </summary>
        public static FieldIndex zero { get => new FieldIndex(0, 0); }
        /// <summary>
        /// = new FieldIndex(-1, -1)
        /// </summary>
        public static FieldIndex minus { get => new FieldIndex(-1, -1); }

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

        public static FieldIndex operator +(FieldIndex a, FieldIndexOffset b)
        {
            FieldIndex ans = a;
            a.row += b.rowOffset; a.column += b.columnOffset;
            return ans;
        }

        public static FieldIndex operator -(FieldIndex a, FieldIndexOffset b)
        {
            FieldIndex ans = a;
            a.row -= b.rowOffset; a.column -= b.columnOffset;
            return ans;
        }

        public static FieldIndex operator +(FieldIndexOffset a, FieldIndex b)
        {
            FieldIndex ans = b;
            b.row += a.rowOffset; b.column += a.columnOffset;
            return ans;
        }

        public static FieldIndex operator -(FieldIndexOffset a, FieldIndex b)
        {
            FieldIndex ans = b;
            b.row -= a.rowOffset; b.column -= a.columnOffset;
            return ans;
        }

        public static FieldIndex operator *(FieldIndex a, float b)
        {
            FieldIndex ans = a;
            a.row = (int)(a.row * b); a.column = (int)(a.column * b);
            return ans;
        }

        public static FieldIndex operator /(FieldIndex a, float b)
        {
            FieldIndex ans = a;
            a.row = (int)(a.row / b); a.column = (int)(a.column / b);
            return ans;
        }

        public static bool operator ==(FieldIndex l, FieldIndex r)
        {
            bool ans = true;
            ans &= l.row == r.row;
            ans &= l.column == r.column;
            return ans;
        }

        public static bool operator !=(FieldIndex l, FieldIndex r)
        {
            bool ans = true;
            ans &= l.row == r.row;
            ans &= l.column == r.column;
            return !ans;
        }

        /// <summary>
        /// 新しくグリッドの絶対座標を生成する。
        /// </summary>
        /// <param name="row">行（縦方向）の絶対座標。</param>
        /// <param name="column">列（横方向）の絶対座標。</param>
        public FieldIndex(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public override bool Equals(object obj)
        {
            return obj is FieldIndex index &&
                   row == index.row &&
                   column == index.column;
        }

        public override int GetHashCode()
        {
            var hashCode = -1663278630;
            hashCode = hashCode * -1521134295 + row.GetHashCode();
            hashCode = hashCode * -1521134295 + column.GetHashCode();
            return hashCode;
        }
    }

    public struct FieldIndexOffset        // プレイヤーの座標からみた特定のグリッドの相対座標を格納する構造体
    {
        public int rowOffset;     // 行（縦方向）の移動量
        public int columnOffset;  // 列（横方向）の移動量

        /// <summary>
        /// = new FieldIndexOffset(0, 1)
        /// </summary>
        public static FieldIndexOffset right { get => new FieldIndexOffset(0, 1); }
        /// <summary>
        /// = new FieldIndexOffset(0, -1)
        /// </summary>
        public static FieldIndexOffset left { get => new FieldIndexOffset(0, -1); }
        /// <summary>
        /// = new FieldIndexOffset(1, 0)
        /// </summary>
        public static FieldIndexOffset down { get => new FieldIndexOffset(1, 0); }
        /// <summary>
        /// = new FieldIndexOffset(-1, 0)
        /// </summary>
        public static FieldIndexOffset up { get => new FieldIndexOffset(-1, 0); }
        /// <summary>
        /// = new FieldIndexOffset(0, 0)
        /// </summary>
        public static FieldIndexOffset zero { get => new FieldIndexOffset(0, 0); }

        public static FieldIndexOffset operator +(FieldIndexOffset a, FieldIndexOffset b)
        {
            FieldIndexOffset ans = a;
            a.rowOffset += b.rowOffset; a.columnOffset += b.columnOffset;
            return ans;
        }

        public static FieldIndexOffset operator -(FieldIndexOffset a, FieldIndexOffset b)
        {
            FieldIndexOffset ans = a;
            a.rowOffset -= b.rowOffset; a.columnOffset -= b.columnOffset;
            return ans;
        }

        public static FieldIndexOffset operator *(FieldIndexOffset a, float b)
        {
            FieldIndexOffset ans = a;
            a.rowOffset = (int)(a.rowOffset * b); a.columnOffset = (int)(a.columnOffset * b);
            return ans;
        }

        public static FieldIndexOffset operator /(FieldIndexOffset a, float b)
        {
            FieldIndexOffset ans = a;
            a.rowOffset = (int)(a.rowOffset / b); a.columnOffset = (int)(a.columnOffset / b);
            return ans;
        }

        public static implicit operator FieldIndex(FieldIndexOffset fi)
        {
            FieldIndex ans = FieldIndex.zero;
            ans.row = fi.rowOffset; ans.column = fi.columnOffset;
            return ans;
        }

        public static implicit operator FieldIndexOffset(FieldIndex fi)
        {
            FieldIndexOffset ans = FieldIndexOffset.zero;
            ans.rowOffset = fi.row; ans.columnOffset = fi.column;
            return ans;
        }

        public static bool operator ==(FieldIndexOffset l, FieldIndexOffset r)
        {
            bool ans = true;
            ans &= l.rowOffset == r.rowOffset;
            ans &= l.columnOffset == r.columnOffset;
            return ans;
        }

        public static bool operator !=(FieldIndexOffset l, FieldIndexOffset r)
        {
            bool ans = true;
            ans &= l.rowOffset == r.rowOffset;
            ans &= l.columnOffset == r.columnOffset;
            return !ans;
        }

        /// <summary>
        /// 新しくグリッドの相対座標を生成する。
        /// </summary>
        /// <param name="fieldIndex">相対座標に変換する絶対座標。</param>
        public FieldIndexOffset(FieldIndex fieldIndex)
        {
            rowOffset = fieldIndex.row;
            columnOffset = fieldIndex.column;
        }

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
        /// <param name="ToFieldIndex">移動先の絶対座標。</param>
        /// <param name="FromFieldIndex">移動元の絶対座標。</param>
        public FieldIndexOffset(FieldIndex ToFieldIndex, FieldIndex FromFieldIndex)
        {
            FieldIndexOffset temp = ToFieldIndex - FromFieldIndex;
            rowOffset = temp.rowOffset; columnOffset = temp.columnOffset;
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

        public override bool Equals(object obj)
        {
            return obj is FieldIndexOffset offset &&
                   rowOffset == offset.rowOffset &&
                   columnOffset == offset.columnOffset;
        }

        public override int GetHashCode()
        {
            var hashCode = 492947604;
            hashCode = hashCode * -1521134295 + rowOffset.GetHashCode();
            hashCode = hashCode * -1521134295 + columnOffset.GetHashCode();
            return hashCode;
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