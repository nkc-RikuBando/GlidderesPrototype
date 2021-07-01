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
            // キャラクターの固定データを格納するScriptableObject
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
                // １～３のスキル番号を０～２の添え字にする
                int skillNumberIndex = skillNumber - 1;

                // スキル番号が配列外を参照していないか確認
                if (skillNumberIndex < 0 || skillNumberIndex >= Rule.skillCount) throw new ArgumentOutOfRangeException("skillNumber", "skillNumberは１～３である必要があります。");

                // スキル番号に対応したSkillScriptableObjectを返却
                return characterScriptableObject.skillDatas[skillNumberIndex];
            }
        }
    }
}