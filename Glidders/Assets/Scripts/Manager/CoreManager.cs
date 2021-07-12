using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;

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
            public AttackSignal attackSignal;
            public bool canAct{ get; set; }
            public int point { get; set; }
        }

        public class CoreManager : MonoBehaviour,ICharacterDataReceiver,IGameDataSeter
        {
            const int PLAYER_AMOUNT = 2; // プレイヤーの総数
            const int PLAYER_MOVE_DISTANCE = 5; // 移動の総量

            private enum Phase
            {
                TURNSTART,ACTIONSERECT,MOVE,ATTACK,TURNEND
            }

            private enum MatchFormat
            {
                POINT,HITPOINT
            }

            GameObject[] Characters = new GameObject[PLAYER_AMOUNT]; // いったんCoreManager内に記述
            
            // 各クラス
            CharacterMove characterMove;
            CharacterAttack characterAttack;
            IGetFieldInformation getFieldInformation;
            ISetFieldInformation setFieldInformation;
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // データの総量をプレイヤーの総数の分作る

            public int lastTurn= 20; // ゲーム終了ターン
            public int thisTurn = 0; // 現在のターン
            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            Phase thisPhase = new Phase();
            MatchFormat format = new MatchFormat();

            #region デバッグ用変数
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,] 
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("デバッグ用　スキルデータ")]
            [SerializeField] private Character.SkillScriptableObject[] skillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                #region リストの初期化
                for (int i = 0; i < PLAYER_AMOUNT;i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region デバッグ用　Moveリスト内部の初期化 および　Moveリスト内部の整理
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

                characterDataList[0].index = new FieldIndex(3, 1);
                characterDataList[1].index = new FieldIndex(5, 2);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;

                characterDataList[0].point = 10000;
                characterDataList[1].point = 10000; 
                #endregion


                #region デバッグ用　Attackリストの初期化
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left);
                }
                #endregion
                #endregion

                for (int i = 0;i < characterDirections.Length;i++)
                {
                    characterDirections[i] = characterDataList[i].thisObject.GetComponent<CharacterDirection>();

                    AttackDataReceiver(characterDataList[i].attackSignal, i);
                }

                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                setFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                characterMove = new CharacterMove(getFieldInformation,setFieldInformation,characterDirections); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                characterAttack = new CharacterAttack(); // CharacterAttackの生成

                thisPhase = Phase.TURNSTART;
            }

            // Update is called once per frame
            void Update()
            {
                // フェーズ管理用
                switch (thisPhase)
                {
                    case Phase.TURNSTART:
                        break;
                    case Phase.ACTIONSERECT:
                        break;
                    case Phase.MOVE:
                        break;
                    case Phase.ATTACK:
                        break;
                    case Phase.TURNEND:
                        break;
                }

                // デバッグ用　Enterキーで実行フラグをtrueに
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    moveStart = true;
                }

                // デバッグ用　Spaceキーで実行フラグをtrueに
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    attackStart = true;
                }

                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList));

                    moveStart = false;
                }

                // 攻撃実行フラグがtrueのとき、Attackクラスに攻撃を実行させる
                if (attackStart)
                {
                    characterAttack.AttackOrder(ref characterDataList);

                    attackStart = false;
                }

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    for (int i = 0;i < characterDataList.Length;i++)
                    {
                        Debug.Log($"CharacterName is {characterDataList[i].thisObject.name}");
                        Debug.Log($"Index ({characterDataList[i].index.row},{characterDataList[i].index.column}) | point ({characterDataList[i].point})");
                    }
                }
            }

            /// <summary>
            /// 指定されたidの配列番号を持った移動信号に渡された移動信号を格納する
            /// </summary>
            /// <param name="signal">渡す移動信号</param>
            /// <param name="characterID">キャラクタID</param>
            public void MoveDataReceiver(MoveSignal signal,int characterID)
            {
                characterDataList[characterID].moveSignal = signal;
            }

            /// <summary>
            /// 指定されたidの配列番号を持った攻撃信号に渡された攻撃信号を格納する
            /// </summary>
            /// <param name="signal">渡す攻撃信号</param>
            /// <param name="characterID">キャラクタID</param>
            public void AttackDataReceiver(AttackSignal signal,int characterID)
            {
                characterDataList[characterID].attackSignal = signal;
            }

            /// <summary>
            /// 指定された配列番号を持ったFieldIndexに対し、初期位置を渡す
            /// </summary>
            /// <param name="fieldIndex">渡すIndex</param>
            /// <param name="characterID">キャラクタID</param>
            public void StartPositionSeter(FieldIndex fieldIndex,int characterID)
            {
                characterDataList[characterID].index = fieldIndex;
            }

            /// <summary>
            /// ゲームを終了するターンを設定してもらう
            /// </summary>
            /// <param name="turn">渡すターン</param>
            public void LastTurnSeter(int turn)
            {
                lastTurn = turn;
            }

            /// <summary>
            /// 試合形式を設定してもらう
            /// </summary>
            /// <param name="formatNumber">形式の設定に対応した数字</param>
            public void MatchFormatSeter(int formatNumber)
            {
                format = (MatchFormat)formatNumber;
            }

            /// <summary>
            /// オブジェクト情報を代入してもらう関数
            /// </summary>
            /// <param name="thisObject">対象のオブジェクト</param>
            /// <param name="characterID">キャラクターID</param>
            public void CharacterDataReceber(GameObject thisObject,int characterID)
            {
                characterDataList[characterID].thisObject = thisObject;

                Characters[characterID] = thisObject;
            }
        }

    }
}