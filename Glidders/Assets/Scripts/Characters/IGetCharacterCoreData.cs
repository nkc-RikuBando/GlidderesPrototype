using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        public interface IGetCharacterCoreData
        {
            /// <summary>
            /// このインターフェースを継承したCharacterCoreがアタッチされているGameObjectを取得する。
            /// </summary>
            /// <returns>GameObject。</returns>
            GameObject GetMyGameObject();

            /// <summary>
            /// キャラクターの名前を取得する。
            /// </summary>
            /// <returns>キャラクターの名前が格納された文字列。</returns>
            string GetCharacterName();

            /// <summary>
            /// キャラクターの移動量を取得する。
            /// </summary>
            /// <returns>キャラクターが1ターンで移動できるマス数。</returns>
            int GetMoveAmount();

            /// <summary>
            /// キャラクターが持つスキルデータを取得する。
            /// </summary>
            /// <param name="skillNumber"取得したいスキルのスキル番号。</param>
            /// <returns>スキル情報を記録するScriptableObject。</returns>
            SkillScriptableObject GetSkillData(int skillNumber);

            UniqueSkillScriptableObject GetUniqueData();

            /// <summary>
            /// ゲームルールに応じて、キャラクターのポイントまたはHPを取得する。
            /// </summary>
            /// <returns>キャラクターのポイントまたはHPが格納された数値。</returns>
            int GetPointAndHp();
        }
    }
}
