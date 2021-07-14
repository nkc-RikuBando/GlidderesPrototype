using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        // �v���W�F�N�g�E�B���h�E�ō쐬�\�ɂ���
        [System.Serializable, CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "CreateCharacterData")]
        public class CharacterScriptableObject : ScriptableObject
        {
            [SerializeField]
            public string characterName;           // �L�����N�^�[�̖��O
            [SerializeField]
            public int moveAmount;                 // �ړ���
            [SerializeField]
            public SkillScriptableObject[] skillDataArray = new SkillScriptableObject[Rule.skillCount];  // �X�L����3�i�[����z��
        }
    }
}