using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public struct CharacterData
        {
            public GameObject thisObject;
            public FieldIndex index;
            public MoveSignal moveSignal;
            public AttackSignal attackSignal;
            public bool canAct{ get; set; }
        }

        public class CoreManager : MonoBehaviour
        {
            const int PLAYER_AMOUNT = 2;
            const int PLAYER_MOVE_DISTANCE = 2;

            GameObject[] Characters = new GameObject[PLAYER_AMOUNT]; // いったんCoreManager内に記述
            CharacterMove characterMove;
            SignalManager signalManager;

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT];

            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            public List<MoveSignal> MoveSignalList { get; set; } // MoveSignalのリスト
            public List<AttackSignal> AttackSignalList { get; set; } // AttackSignalのリスト

            #region デバッグ用変数
            MoveSignal[] moveSignal = new MoveSignal[PLAYER_AMOUNT];

            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,] 
            { { new FieldIndexOffset(0, 1), new FieldIndexOffset(1, 0) }, { new FieldIndexOffset(1,0),new FieldIndexOffset(0,1)} };
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                MoveSignalList = new List<MoveSignal>(); // リスト初期化

                for (int i = 0; i < PLAYER_AMOUNT;i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                // デバッグ用　リスト内部の初期化 および　リスト内部の整理
                for (int i = 0; i < moveSignal.Length;i++)
                {
                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[PLAYER_MOVE_DISTANCE];
                    for (int j = 0;j < PLAYER_MOVE_DISTANCE;j++)
                    {
                        
                        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                    }
                    MoveDataReceiver(characterDataList[i].moveSignal, i);
                }
                characterDataList[0].thisObject = GameObject.Find("Kaito");
                characterDataList[1].thisObject = GameObject.Find("Seira");

                characterDataList[0].index = new FieldIndex(2, 3);
                characterDataList[1].index = new FieldIndex(3, 3);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;

                // デバッグ用　キャラクタを取得
                Characters[0] = GameObject.Find("Kaito");
                Characters[1] = GameObject.Find("Seira");

                // MoveSignalList.Add(moveSignal); // 移動情報を格納
                characterMove = new CharacterMove();
                signalManager = new SignalManager();

            }

            // Update is called once per frame
            void Update()
            {

                // デバッグ用　Enterキーで実行フラグをtrueに
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    moveStart = true;
                }

                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
                    //for (int i = 0; i < moveSignal.Length; i++)
                    //{
                    //    characterMove.MoveOrder(moveSignal[i], i);
                    //}

                    // characterMove.MoveOrder(characterDataList);
                    StartCoroutine(characterMove.MoveOrder(characterDataList));

                    moveStart = false;
                }
            }

            /// <summary>
            /// 指定されたidの配列番号を持った移動信号に渡された移動信号を格納する
            /// </summary>
            /// <param name="signal">渡す移動信号</param>
            /// <param name="id">キャラクタID</param>
            public void MoveDataReceiver(MoveSignal signal,int id)
            {
                moveSignal[id] = signal;


            }

            public void AttackDataReceiver()
            {

            }

            /// <summary>
            /// オブジェクト情報を代入してもらう関数
            /// </summary>
            /// <param name="thisObject">対象のオブジェクト</param>
            /// <param name="characterID">キャラクターID</param>
            public void CharacterDataReceber(GameObject thisObject,int characterID)
            {
                Characters[characterID] = thisObject;
            }
        }

    }
}