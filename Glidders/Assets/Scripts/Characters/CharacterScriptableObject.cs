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
            public string characterName { get; set; }           // �L�����N�^�[�̖��O
            public int moveAmount { get; set; }                 // �ړ���

            public SkillScriptableObject[] skillDatas { get; set; }  // �X�L����3�i�[����z��
        }
    }
}