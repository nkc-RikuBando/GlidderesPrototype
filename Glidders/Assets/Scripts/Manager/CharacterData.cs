using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Buff;

namespace Glidders
{
    namespace Manager
    {
        /// <summary>
        /// キャラクタ情報を格納する構造体
        /// </summary>
        public struct CharacterData
        {
            /// <summary>
            /// 自身のオブジェクト
            /// </summary>
            public GameObject thisObject;
            /// <summary>
            /// 自身のグリッド上の座標
            /// </summary>
            public FieldIndex index;
            /// <summary>
            /// 移動情報
            /// </summary>
            public MoveSignal moveSignal;
            /// <summary>
            /// 向き変更情報
            /// </summary>
            public DirecionSignal direcionSignal;
            /// <summary>
            /// 攻撃情報
            /// </summary>
            public AttackSignal attackSignal;
            /// <summary>
            /// キャラクターの名前
            /// </summary>
            public CharacterName characterName;
            /// <summary>
            /// バフの情報
            /// </summary>
            public List<BuffViewData> buffView;
            /// <summary>
            /// バフの内容
            /// </summary>
            public List<List<BuffValueData>> buffValue;
            /// <summary>
            /// バフの経過ターン数
            /// </summary>
            public List<List<int>> buffTurn;
            /// <summary>
            /// 自身のエナジー
            /// </summary>
            public int energy { get; set; }
            /// <summary>
            /// 行動の可否
            /// </summary>
            public bool canAct { get; set; }
            /// <summary>
            /// 自身のポイント量
            /// </summary>
            public int point { get; set; }
            /// <summary>
            /// プレイヤの名前
            /// </summary>
            public string playerName { get; set; }
            /// <summary>
            /// プレイヤ番号
            /// </summary>
            public int playerNumber { get; set; }
        }
    }
}