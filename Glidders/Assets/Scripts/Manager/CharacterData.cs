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
            public GameObject thisObject;
            public FieldIndex index;
            public MoveSignal moveSignal;
            public DirecionSignal direcionSignal;
            public AttackSignal attackSignal;
            public CharacterName characterName;
            public List<BuffViewData> buffView;
            public List<List<BuffValueData>> buffValue;
            public List<int> buffTurn;
            public int energy { get; set; }
            public bool canAct { get; set; }
            public int point { get; set; }
            public string playerName { get; set; }
        }
    }
}