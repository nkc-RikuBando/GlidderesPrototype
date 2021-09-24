using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Glidders
{
    namespace Character
    {
        public class CharacterCore : MonoBehaviour, IGetCharacterCoreData
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

            public GameObject GetMyGameObject()
            {
                return gameObject;
            }

            public int GetPointAndHp()
            {
                return pointAndHp;
            }

            /// <summary>
            /// このキャラクターに付与されたスキルデータを参照する。
            /// </summary>
            /// <param name="skillNumber">1～3のスキル番号。</param>
            /// <returns></returns>
            public SkillScriptableObject GetSkillData(int skillNumber)
            {
                // 1～3のスキル番号を0～2の添え字にする
                int skillNumberIndex = skillNumber - 1;

                // スキル番号が配列外を参照していないか確認
                if (skillNumberIndex < 0 || skillNumberIndex >= Rule.skillCount) throw new ArgumentOutOfRangeException("skillNumber", "skillNumberは１～３である必要があります。");

                // スキル番号に対応したSkillScriptableObjectを返却
                return characterScriptableObject.skillDataArray[skillNumberIndex];
            }

            public UniqueSkillScriptableObject GetUniqueData()
            {
                return characterScriptableObject.uniqueSkillData;
            }
        }
    }
}