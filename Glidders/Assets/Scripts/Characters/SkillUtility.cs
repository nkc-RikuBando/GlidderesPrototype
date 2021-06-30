using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        public struct SkillRangeGrid        // �v���C���[�̍��W����݂�����̃O���b�h�̑��΍��W���i�[����\����
        {
            public int rowOffset { get; set; }     // �s�i�c�����j�̈ړ���
            public int columnOffset { get; set; }  // ��i�������j�̈ړ���

            /// <summary>
            /// �V�����O���b�h�̑��΍��W�𐶐�����B
            /// </summary>
            /// <param name="rowOffset">�O���b�h�̍s�i�c�����j�̈ړ��ʁB</param>
            /// <param name="columnOffset">�O���b�h�̗�i�������j�̈ړ��ʁB</param>
            public SkillRangeGrid(int rowOffset, int columnOffset)
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
            public SkillRangeGrid(int row, int column, int positionRow, int positionColumn)
            {
                rowOffset = row - positionRow;
                columnOffset = column - positionColumn;
            }
        }
    }
}
