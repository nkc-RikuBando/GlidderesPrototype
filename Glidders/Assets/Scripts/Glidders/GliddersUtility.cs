using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public struct FieldIndex        // Vector2�ɑ���񎟌��z��Q�Ɨp�̍\����
    {
        public int row;        // �s�i�c�����j�̍��W
        public int column;     // ��i�������j�̍��W

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

    public struct FieldIndexOffset        // �v���C���[�̍��W����݂�����̃O���b�h�̑��΍��W���i�[����\����
    {
        public int rowOffset { get; set; }     // �s�i�c�����j�̈ړ���
        public int columnOffset { get; set; }  // ��i�������j�̈ړ���

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
        /// <param name="row">�O���b�h�̍s�i�c�����j�̐�΍��W�B</param>
        /// <param name="column">�O���b�h�̗�i�������j�̐�΍��W�B</param>
        /// <param name="positionRow">��_�̍s�i�c�����j�̐�΍��W�B</param>
        /// <param name="positionColumn">��_�̗�i�������j�̐�΍��W�B</param>
        public FieldIndexOffset(int row, int column, int positionRow, int positionColumn)
        {
            rowOffset = row - positionRow;
            columnOffset = column - positionColumn;
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