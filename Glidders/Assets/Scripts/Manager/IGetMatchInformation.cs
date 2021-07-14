using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public interface IGetMatchInformation
    {
        /// <summary>
        /// 各プレイヤーがマッチング画面で設定された情報を取得する。
        /// </summary>
        /// <returns>プレイヤー情報を格納した構造体配列。</returns>
        MatchingPlayerData[] GetMatchingPlayerData();

        /// <summary>
        /// マッチング画面で設定されたルールを取得する。
        /// </summary>
        /// <returns>取得するルールが格納された構造体。</returns>
        RuleInfo GetRuleInformation();
    }
}
