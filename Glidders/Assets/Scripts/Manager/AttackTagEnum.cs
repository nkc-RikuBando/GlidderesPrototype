using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public enum AttackTagEnum
    {
        ANY,            // 全て（ワイルドカードのような存在）
        PLAYER,         // 他プレイヤーによる攻撃
        MYSELF,         // 自分自身による攻撃
        DAMAGE_FIELD,   // ダメージフィールドによる攻撃
        STAGE_GIMMICK   // ステージギミックによる攻撃
    }
}
