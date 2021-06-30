using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Field
    {
        public struct FieldIndex        // Vector2に代わる二次元配列参照用の構造体
        {
            int row;        // 行（縦方向）の座標
            int column;     // 列（横方向）の座標
        }

        public enum InformationOnGrid  // グリッドごとに記録される情報の種類
        {
            DAMAGE_FIELD,   // ダメージフィールド
            LANDFORM,       // 地形情報
            count,          // このenumの要素数
        }
    }
}
