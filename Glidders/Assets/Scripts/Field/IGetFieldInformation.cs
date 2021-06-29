using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Field
    {
        /// <summary>
        /// �t�B�[���h��������擾�������Ƃ��ɗp����C���^�[�t�F�[�X
        /// </summary>
        public interface IGetFieldInformation
        {
            /// <summary>
            /// �w�肵���O���b�h�ɂ���_���[�W�t�B�[���h�̏��L�҂��擾����B
            /// </summary>
            /// <param name="fieldIndex">�w�肵���O���b�h�̍��W�B</param>
            /// <returns>�_���[�W�t�B�[���h���L�҂̃v���C���[�ԍ��B</returns>
            int GetDamageFieldOwner(FieldIndex fieldIndex);

            /// <summary>
            /// �w�肵���O���b�h���ʍs�\�����擾����B
            /// </summary>
            /// <param name="fieldIndex">�w�肵���O���b�h�̍��W�B</param>
            /// <returns>�O���b�h���ʍs�\����\��bool�l�Btrue�Ȃ�ʍs�\�B</returns>
            bool IsPassingGrid(FieldIndex fieldIndex);
        }
    }
}