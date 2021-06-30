using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // �v���W�F�N�g�E�B���h�E�ō쐬�\�ɂ���
        [CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "CreateSkillData")]
        public class SkillScriptableObject : ScriptableObject
        {
            // �v���C���[�����͂���X�L�����
            public string skillName { get; set; }            // �X�L������
            public int energy { get; set; }                  // �G�l���M�[
            public int damage { get; set; }                  // �_���[�W
            public int priority { get; set; }                // �D��x
            public int power { get; set; }                   // �З�(�_���[�W�t�B�[���h)
            //List<Buff> buff;    // �X�L���ɂ��t�^�����o�t���X�g
            public List<SkillRangeGrid> gridList { get; set; }  // �X�L���̍U���͈͂��i�[�����O���b�h���X�g
        }
    }
}
