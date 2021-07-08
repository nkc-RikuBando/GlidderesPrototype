using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // �v���W�F�N�g�E�B���h�E�ō쐬�\�ɂ���
        [CreateAssetMenu(fileName = "SkillScriptableObjectWork", menuName = "CreateSkillWorkData")]
        public class SkillScriptableObjectWork : ScriptableObject
        {
            // �v���C���[�����͂���X�L�����
            public string skillName { get; set; }            // �X�L������
            public int energy { get; set; }                  // �G�l���M�[
            public int damage { get; set; }                  // �_���[�W
            public int priority { get; set; }                // �D��x
            public int power { get; set; }                   // �З�(�_���[�W�t�B�[���h)
            //List<Buff> buff;    // �X�L���ɂ��t�^�����o�t���X�g
            //��public List<FieldIndexOffset> gridList { get; set; }  // �X�L���̍U���͈͂��i�[�����O���b�h���X�g
            public int FIELD_SIZE = 13; // 7 * 2 - 1

            [SerializeField]
            public bool[] array = new bool[13];
            [SerializeField]
            public bool[,] selectRangeArray = new bool[13, 13];
            [SerializeField]
            public bool[,] attackRangeArray = new bool[13, 13];
            public List<FieldIndexOffset> selectGridList { get; set; }
            public List<FieldIndexOffset> attackGridList { get; set; }

            public SkillScriptableObjectWork()
            {
                //��gridList = new List<FieldIndexOffset>();
                selectGridList = new List<FieldIndexOffset>();
                attackGridList = new List<FieldIndexOffset>();
            }
        }
    }
}
