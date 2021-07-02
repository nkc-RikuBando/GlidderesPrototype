using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Player_namespace
    {
        public class PlayerCore
        {
            // バフリスト
            public enum BuffList
            {
                ATTACKUP, ATTACKDOWN,
                GARDUP, GARDDOWN,
                MOVEUP, MOVEDOWN,
                FIELDUP, FIELDDOWN,
                LENGTH
            }

            [SerializeField] private int playerID = 0; // キャラクターの番号
            [SerializeField] private int characterHP = 100; // キャラクターのHP

            private List<BuffList> buff = new List<BuffList>();

            /// <summary>
            /// バフを増やす
            /// </summary>
            /// <param name="add">追加するバフ</param>
            public void BuffAdd(BuffList add)
            {
                buff.Add(add);
            }

            /// <summary>
            /// バフを減らす
            /// </summary>
            /// <param name="remove">減らすバフ</param>
            public void BuffRemove(BuffList remove)
            {
                buff.Remove(remove);
            }

            /// <summary>
            /// バフを消す
            /// </summary>
            public void BuffNull()
            {
                buff.Clear();
            }
        }

    }
}