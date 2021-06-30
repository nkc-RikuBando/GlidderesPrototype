using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public static class Gamerule
        {
            public const int MAX_MOVE_AMOUNT = 5;

        }

        public struct MoveData
        {
            public int row;
            public int column;

            public static MoveData right { get; }
            public static MoveData left { get; }
            public static MoveData down { get; }
            public static MoveData up { get; }
            public static MoveData wait { get; }

            public MoveData(int row, int column)
            {
                this.row = row;
                this.column = column;
            }
        }

        /// <summary>
        /// プレイヤーがサーバーに移動情報を送る際に用いられる構造体
        /// </summary>
        public struct MoveSignal
        {
            public MoveData[] moveDataArray;    // 移動情報を格納した配列

            public MoveSignal(MoveData[] moveDataArray)
            {
                this.moveDataArray = new MoveData[Gamerule.MAX_MOVE_AMOUNT];
                for (int i = 0; i < this.moveDataArray.Length; i++)
                {
                    this.moveDataArray[i] = MoveData.wait;                    
                }

                for (int i = 0; i < moveDataArray.Length; i++)
                {
                    if (i >= this.moveDataArray.Length) break;
                    this.moveDataArray[i] = moveDataArray[i];
                }
            }
        }
    }
}
