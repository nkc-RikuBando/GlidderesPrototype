using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;

namespace Glidders
{
    namespace Manager
    { 
        /// <summary>
        /// キャラクタ情報を格納するストラクト
        /// </summary>
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
            const int PLAYER_AMOUNT = 2; // プレイヤーの総数
            const int PLAYER_MOVE_DISTANCE = 2; // 移動の総量

            GameObject[] Characters = new GameObject[PLAYER_AMOUNT]; // いったんCoreManager内に記述
            
            // 各クラス
            CharacterMove characterMove; 
            SignalManager signalManager;
            IGetFieldInformation getFieldInformation;

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // データの総量をプレイヤーの総数の分作る

            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            public List<MoveSignal> MoveSignalList { get; set; } // MoveSignalのリスト
            public List<AttackSignal> AttackSignalList { get; set; } // AttackSignalのリスト

            #region デバッグ用変数
            MoveSignal[] moveSignal = new MoveSignal[PLAYER_AMOUNT]; 

            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,] 
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset(1, 0) }, { new FieldIndexOffset(1,0),new FieldIndexOffset(0,1)} };
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                #region リストの初期化
                MoveSignalList = new List<MoveSignal>(); // リスト初期化

                for (int i = 0; i < PLAYER_AMOUNT;i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region デバッグ用　リスト内部の初期化 および　リスト内部の整理
                for (int i = 0; i < characterDataList.Length;i++)
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

                characterDataList[0].index = new FieldIndex(3, 2);
                characterDataList[1].index = new FieldIndex(5, 2);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;
                #endregion
                #endregion

                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                characterMove = new CharacterMove(getFieldInformation); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                signalManager = new SignalManager(); // SignalManagerの生成

            }

            // Update is called once per frame
            void Update()
            {
                // デバッグ用　インデックスの位置をVector3に変換し、移動させる
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    characterDataList[0].thisObject.transform.position = getFieldInformation.GetTilePosition(new FieldIndex(3, 2));
                }

                // デバッグ用　Enterキーで実行フラグをtrueに
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    moveStart = true;
                }

                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
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
                characterDataList[id].moveSignal = signal;
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