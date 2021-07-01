using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        /// <summary>
        /// プレイヤーがサーバーに移動情報を送る際に用いられる構造体
        /// </summary>
        public struct MoveSignal
        {
            public FieldIndexOffset[] moveDataArray;    // 移動情報を格納した配列

            public MoveSignal(FieldIndexOffset[] moveDataArray)
            {
                this.moveDataArray = new FieldIndexOffset[Rule.maxMoveAmount];
                for (int i = 0; i < this.moveDataArray.Length; i++)
                {
                    this.moveDataArray[i] = FieldIndexOffset.zero;                    
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
