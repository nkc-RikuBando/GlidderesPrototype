using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Glidders
{
    namespace Character
    {
        public class CharacterCore : IGetCharacterCoreData
        {
            // �L�����N�^�[�̌Œ�f�[�^���i�[����ScriptableObject
            public CharacterScriptableObject characterScriptableObject;

            public int pointAndHp { get; set; }

            public string GetCharacterName()
            {
                return characterScriptableObject.characterName;
            }

            public int GetMoveAmount()
            {
                return characterScriptableObject.moveAmount;
            }

            public int GetPointAndHp()
            {
                return pointAndHp;
            }

            public SkillScriptableObject GetSkillData(int skillNumber)
            {
                // �P�`�R�̃X�L���ԍ����O�`�Q�̓Y�����ɂ���
                int skillNumberIndex = skillNumber - 1;

                // �X�L���ԍ����z��O���Q�Ƃ��Ă��Ȃ����m�F
                if (skillNumberIndex < 0 || skillNumberIndex >= Rule.skillCount) throw new ArgumentOutOfRangeException("skillNumber", "skillNumber�͂P�`�R�ł���K�v������܂��B");

                // �X�L���ԍ��ɑΉ�����SkillScriptableObject��ԋp
                return characterScriptableObject.skillDatas[skillNumberIndex];
            }
        }
    }
}