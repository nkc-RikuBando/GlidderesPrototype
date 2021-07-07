using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    /// <summary>
    /// #Glidders!!�ő�O��Ƃ���郋�[�����܂Ƃ߂��N���X�B�}�W�b�N�i���o�[�΍�B
    /// </summary>
    public static class Rule
    {
        public static readonly int skillCount = 3;     // �L�����N�^�[�����X�L���̐�
        public static readonly int maxMoveAmount = 5;  // �L�����N�^�[�̍ő�ړ���
    }

    public struct FieldIndex        // Vector2�ɑ���񎟌��z��Q�Ɨp�̍\����
    {
        public int row;        // �s�i�c�����j�̍��W
        public int column;     // ��i�������j�̍��W

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
        /// �V�����O���b�h�̐�΍��W�𐶐�����B
        /// </summary>
        /// <param name="row">�s�i�c�����j�̐�΍��W�B</param>
        /// <param name="column">��i�������j�̐�΍��W�B</param>
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

    public struct FieldIndexOffset        // �v���C���[�̍��W����݂�����̃O���b�h�̑��΍��W���i�[����\����
    {
        public int rowOffset;     // �s�i�c�����j�̈ړ���
        public int columnOffset;  // ��i�������j�̈ړ���

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
        /// �V�����O���b�h�̑��΍��W�𐶐�����B
        /// </summary>
        /// <param name="fieldIndex">���΍��W�ɕϊ������΍��W�B</param>
        public FieldIndexOffset(FieldIndex fieldIndex)
        {
            rowOffset = fieldIndex.row;
            columnOffset = fieldIndex.column;
        }

        /// <summary>
        /// �V�����O���b�h�̑��΍��W�𐶐�����B
        /// </summary>
        /// <param name="rowOffset">�O���b�h�̍s�i�c�����j�̈ړ��ʁB</param>
        /// <param name="columnOffset">�O���b�h�̗�i�������j�̈ړ��ʁB</param>
        public FieldIndexOffset(int rowOffset, int columnOffset)
        {
            this.rowOffset = rowOffset;
            this.columnOffset = columnOffset;
        }

        /// <summary>
        /// �V�����O���b�h�̑��΍��W�𐶐�����B
        /// </summary>
        /// <param name="ToFieldIndex">�ړ���̐�΍��W�B</param>
        /// <param name="FromFieldIndex">�ړ����̐�΍��W�B</param>
        public FieldIndexOffset(FieldIndex ToFieldIndex, FieldIndex FromFieldIndex)
        {
            FieldIndexOffset temp = ToFieldIndex - FromFieldIndex;
            rowOffset = temp.rowOffset; columnOffset = temp.columnOffset;
        }

        /// <summary>
        /// �V�����O���b�h�̑��΍��W�𐶐�����B
        /// </summary>
        /// <param name="row">�O���b�h�̍s�i�c�����j�̐�΍��W�B</param>
        /// <param name="column">�O���b�h�̗�i�������j�̐�΍��W�B</param>
        /// <param name="positionRow">��_�̍s�i�c�����j�̐�΍��W�B</param>
        /// <param name="positionColumn">��_�̗�i�������j�̐�΍��W�B</param>
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



    public enum InformationOnGrid  // �O���b�h���ƂɋL�^�������̎��
    {
        DAMAGE_FIELD,   // �_���[�W�t�B�[���h
        LANDFORM,       // �n�`���
        count,          // ����enum�̗v�f��
    }

    public enum CharacterName
    {
        KAITO,
        SEIRA,
    }
}