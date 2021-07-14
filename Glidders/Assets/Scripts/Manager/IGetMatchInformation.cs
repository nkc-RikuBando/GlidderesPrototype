using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public interface IGetMatchInformation
    {
        /// <summary>
        /// �e�v���C���[���}�b�`���O��ʂŐݒ肳�ꂽ�����擾����B
        /// </summary>
        /// <returns>�v���C���[�����i�[�����\���̔z��B</returns>
        MatchingPlayerData[] GetMatchingPlayerData();

        /// <summary>
        /// �}�b�`���O��ʂŐݒ肳�ꂽ���[�����擾����B
        /// </summary>
        /// <returns>�擾���郋�[�����i�[���ꂽ�\���́B</returns>
        RuleInfo GetRuleInformation();
    }
}
