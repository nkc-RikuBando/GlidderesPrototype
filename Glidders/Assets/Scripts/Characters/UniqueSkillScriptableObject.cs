using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Buff;

namespace Glidders
{
    namespace Character
    {
        // �v���W�F�N�g�E�B���h�E�ō쐬�\�ɂ���
        [CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "CreateSkillData")]
        public class UniqueSkillScriptableObject : ScriptableObject
        {
            // ���j�[�N�X�L���̎��ʏ��
            public string skillName;                    // �X�L������
            public int energy;                          // �G�l���M�[
            public int priority;                        // �D��x
            public Sprite skillIcon;                    // �X�L���A�C�R��
            public AnimationClip skillAnimation;        // �A�j���[�V�����N���b�v
            public SkillTypeEnum skillType;             // �X�L���̎�ށi�U���Z���⏕�Z���j

            public UniqueSkillMoveType moveType;        // �ړ��̎��
            public FieldIndexOffset[] MoveSelectRange;  // �ړ���}�X

            public bool isAttack;                       // �U�����邩�ǂ���
            public int damage;                          // �_���[�W
            public int power;                           // �З�(�_���[�W�t�B�[���h)
            public FieldIndexOffset[] AttackSelectRange;// �I���\�}�X
            public FieldIndexOffset[] AttackRange;      // �U���͈̓}�X

            public List<BuffViewData> giveBuff;         // �t�^�����o�t
        }

        public enum UniqueSkillMoveType
        {
            NONE,
            FREE,
            FIXED,
        }
    }
}
