using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // �v���W�F�N�g�E�B���h�E�ō쐬�\�ɂ���
        [CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "CreateCharacterData")]
        public class CharacterScriptableObject : ScriptableObject
        {
            [SerializeField]
            public string characterName;           // �L�����N�^�[�̖��O
            [SerializeField]
            public int moveAmount;                 // �ړ���
            [SerializeField]
            public SkillDatasClass skillDatas;

            public CharacterScriptableObject()
            {
                //skillDatas.skillDataArray = new SkillScriptableObject[Rule.skillCount];
            }
        }

        public class SkillDatasClass
        {
            public SkillScriptableObject[] skillDataArray;  // �X�L����3�i�[����z��

            public SkillDatasClass()
            {
                skillDataArray = new SkillScriptableObject[Rule.skillCount];
            }
        }
    }
}