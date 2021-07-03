using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public class CoreManager : MonoBehaviour
        {
            CharacterMove characterMove;
            SignalManager signalManager;

            public bool moveStart { get; set; }
            public bool attackStart { get; set; }
            public List<MoveSignal> MoveSignalList { get; set; }
            public List<AttackSignal> AttackSignalList { get; set; }

            // デバッグ用
            MoveSignal moveSignal = new MoveSignal();

            // Start is called before the first frame update
            void Start()
            {
                MoveSignalList = new List<MoveSignal>(); // リスト初期化
                moveSignal.moveDataArray = new FieldIndexOffset[2]; // 配列数初期化
                moveSignal.moveDataArray[0] = new FieldIndexOffset(0, 2); // デバッグ用　一回目の移動量
                moveSignal.moveDataArray[1] = new FieldIndexOffset(1, 0); // デバッグ用　二回目の移動量
                MoveSignalList.Add(moveSignal); // 移動情報を格納
                characterMove = new CharacterMove();
                signalManager = new SignalManager();

                // デバッグ用Character移動処理
                moveStart = true;

                if (moveStart)
                {
                    foreach (var x in MoveSignalList)
                    {
                        characterMove.MoveOrder(x, 0);
                    }
                }
            }

            // Update is called once per frame
            void Update()
            {
            }

            public void MoveDataReceiver()
            {

            }

            public void AttackDataReceiver()
            {

            }
        }

    }
}