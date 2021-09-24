using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        public interface IGetCharacterCoreData
        {
            /// <summary>
            /// ���̃C���^�[�t�F�[�X���p������CharacterCore���A�^�b�`����Ă���GameObject���擾����B
            /// </summary>
            /// <returns>GameObject�B</returns>
            GameObject GetMyGameObject();

            /// <summary>
            /// �L�����N�^�[�̖��O���擾����B
            /// </summary>
            /// <returns>�L�����N�^�[�̖��O���i�[���ꂽ������B</returns>
            string GetCharacterName();

            /// <summary>
            /// �L�����N�^�[�̈ړ��ʂ��擾����B
            /// </summary>
            /// <returns>�L�����N�^�[��1�^�[���ňړ��ł���}�X���B</returns>
            int GetMoveAmount();

            /// <summary>
            /// �L�����N�^�[�����X�L���f�[�^���擾����B
            /// </summary>
            /// <param name="skillNumber"�擾�������X�L���̃X�L���ԍ��B</param>
            /// <returns>�X�L�������L�^����ScriptableObject�B</returns>
            SkillScriptableObject GetSkillData(int skillNumber);

            UniqueSkillScriptableObject GetUniqueData();

            /// <summary>
            /// �Q�[�����[���ɉ����āA�L�����N�^�[�̃|�C���g�܂���HP���擾����B
            /// </summary>
            /// <returns>�L�����N�^�[�̃|�C���g�܂���HP���i�[���ꂽ���l�B</returns>
            int GetPointAndHp();
        }
    }
}
