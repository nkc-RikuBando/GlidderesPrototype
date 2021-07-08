using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

            public bool[] selectGridArray { get; set; }                 // �I���\�}�X�����ۂɊi�[���Ă����ꎟ���z��
            public bool[] attackGridArray { get; set; }                 // �U���͈͂����ۂɊi�[���Ă����ꎟ���z��

            // �X�L���̑I���\�}�X���i�[�����O���b�h���X�g
            public FieldIndexOffset[] selectFieldIndexOffsetArray
            {
                get => GetSelectFieldIndexOffsetArray();
            }

            // �X�L���̍U���͈͂��i�[�����O���b�h���X�g
            public FieldIndexOffset[] attackFieldIndexOffsetArray
            {
                get => GetAttackFieldIndexOffsetArray();
            }

            public int rangeSize { get; set; }
            public int center { get; set; }

            private FieldIndexOffset[] GetSelectFieldIndexOffsetArray()
            {
                List<FieldIndexOffset> selectGridTrueList = new List<FieldIndexOffset>();
                int index = 0;
                foreach (bool active in selectGridArray)
                {
                    if (active)
                    {
                        int rowOffset = index / rangeSize - center;
                        int columnOffset = index % rangeSize - center;
                        selectGridTrueList.Add(new FieldIndexOffset(rowOffset, columnOffset));
                        index++;
                    }
                }
                FieldIndexOffset[] returnArray = new FieldIndexOffset[selectGridTrueList.Count];
                for (int i = 0; i < selectGridTrueList.Count; i++)
                {
                    returnArray[i] = selectGridTrueList[i];
                }
                return returnArray;
            }

            private FieldIndexOffset[] GetAttackFieldIndexOffsetArray()
            {
                List<FieldIndexOffset> attackGridTrueList = new List<FieldIndexOffset>();
                int index = 0;
                foreach (bool active in attackGridArray)
                {
                    if (active)
                    {
                        int rowOffset = index / rangeSize - center;
                        int columnOffset = index % rangeSize - center;
                        attackGridTrueList.Add(new FieldIndexOffset(rowOffset, columnOffset));
                        index++;
                    }
                }
                FieldIndexOffset[] returnArray = new FieldIndexOffset[attackGridTrueList.Count];
                for (int i = 0; i < attackGridTrueList.Count; i++)
                {
                    returnArray[i] = attackGridTrueList[i];
                }
                return returnArray;
            }
        }
    }
}
