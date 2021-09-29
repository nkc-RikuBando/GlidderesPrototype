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
        public static readonly int maxPlayerCount = 4; // 参加できる最大人数
    }

    /// <summary>
    /// #Glidders!で一試合のなかで用いられるルールをまとめたクラス。
    /// </summary>
    public static class ActiveRule
    {
        public static int playerCount { get; private set; } = 4; // 今回の試合のプレイヤー数
        public static int maxTurn { get; private set; }    // 今回の試合のターン数

        /// <summary>
        /// ゲーム開始時に今回のプレイヤー数を設定するもの。安易に呼び出すでないわ！
        /// </summary>
        /// <param name="playerCount">今回の試合のプレイヤー数。</param>
        public static void SetPlayerCount(int newPlayerCount)
        {
            playerCount = newPlayerCount;
        }

        /// <summary>
        /// ゲーム開始時に今回のプレイヤー数を設定するもの。呼び出すつもりなら#Glidders!!運営事務局に申請するのじゃ！
        /// </summary>
        /// <param name="newMaxTurn">今回の試合のプレイヤー数。</param>
        public static void SetMaxTurn(int newMaxTurn)
        {
            maxTurn = newMaxTurn;
        }
    }

    public enum PhaseList
    {
        SET_STARTING_POSITION,  // 初期位置選択
        BEGIN_TURN,             // ターン開始時
        INPUT_COMMAND,          // 行動選択
        CHARACTER_MOVE,         // キャラクター移動
        CHARACTER_ATTACK,       // キャラクター攻撃
        END_TURN,               // ターン終了時
        RESULT,                 // リザルト
        count,  // 要素数
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
            ans.row += b.row; ans.column += b.column;
            return ans;
        }

        public static FieldIndex operator -(FieldIndex a, FieldIndex b)
        {
            FieldIndex ans = a;
            ans.row -= b.row; ans.column -= b.column;
            return ans;
        }

        public static FieldIndex operator +(FieldIndex a, FieldIndexOffset b)
        {
            FieldIndex ans = a;
            ans.row += b.rowOffset; ans.column += b.columnOffset;
            return ans;
        }

        public static FieldIndex operator -(FieldIndex a, FieldIndexOffset b)
        {
            FieldIndex ans = a;
            ans.row -= b.rowOffset; ans.column -= b.columnOffset;
            return ans;
        }

        public static FieldIndex operator +(FieldIndexOffset a, FieldIndex b)
        {
            FieldIndex ans = b;
            ans.row += a.rowOffset; ans.column += a.columnOffset;
            return ans;
        }

        public static FieldIndex operator -(FieldIndexOffset a, FieldIndex b)
        {
            FieldIndex ans = b;
            ans.row -= a.rowOffset; ans.column -= a.columnOffset;
            return ans;
        }

        public static FieldIndex operator *(FieldIndex a, float b)
        {
            FieldIndex ans = a;
            ans.row = (int)(a.row * b); ans.column = (int)(a.column * b);
            return ans;
        }

        public static FieldIndex operator /(FieldIndex a, float b)
        {
            FieldIndex ans = a;
            ans.row = (int)(a.row / b); ans.column = (int)(a.column / b);
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
            ans.rowOffset += b.rowOffset; ans.columnOffset += b.columnOffset;
            return ans;
        }

        public static FieldIndexOffset operator -(FieldIndexOffset a, FieldIndexOffset b)
        {
            FieldIndexOffset ans = a;
            ans.rowOffset -= b.rowOffset; ans.columnOffset -= b.columnOffset;
            return ans;
        }

        public static FieldIndexOffset operator *(FieldIndexOffset a, float b)
        {
            FieldIndexOffset ans = a;
            ans.rowOffset = (int)(a.rowOffset * b); ans.columnOffset = (int)(a.columnOffset * b);
            return ans;
        }

        public static FieldIndexOffset operator /(FieldIndexOffset a, float b)
        {
            FieldIndexOffset ans = a;
            ans.rowOffset = (int)(a.rowOffset / b); ans.columnOffset = (int)(a.columnOffset / b);
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
        YU,
        MITSUHA,
        count,  // 要素数
    }
}