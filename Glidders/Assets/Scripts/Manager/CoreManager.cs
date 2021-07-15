using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Player_namespace;
using System;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    { 
        delegate void PhaseMethod(); // フェーズごとの行動を記録した関数を登録するデリゲート
        public class CoreManager : MonoBehaviour, ICharacterDataReceiver, IPhaseInformation, IPlayerInformation
        {
            PhotonView view;

            Action phaseCompleteAction;
            const int PLAYER_AMOUNT = 4; // プレイヤーの総数
            const int PLAYER_MOVE_DISTANCE = 5; // 移動の総量

            GameObject[] commandDirectorArray = new GameObject[Rule.maxPlayerCount];

            private enum Phase
            {
                TURN_START, ACTION_SERECT, MOVE, ATTACK, TURN_END
            }

            private enum MatchFormat
            {
                POINT, HITPOINT
            }

            // 各クラス
            CharacterMove characterMove;
            CharacterAttack characterAttack;
            IGetFieldInformation getFieldInformation;
            ISetFieldInformation setFieldInformation;
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // データの総量をプレイヤーの総数の分作る

            [SerializeField] private int lastTurn = 20; // ゲーム終了ターン
            public int thisTurn { get; set; } // 現在のターン
            public int positionSetMenber { get; set; } // 初期位置を選択したメンバー数を把握する
            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            Phase thisPhase = new Phase(); // フェーズを管理するenum
            MatchFormat format = new MatchFormat(); // 試合形式を管理するenum

            event PhaseMethod phaseEvent = delegate () { }; // イベント生成

            private Animator[] animators = new Animator[PLAYER_AMOUNT]; // アニメーション管理のアニメーター変数

            #region デバッグ用変数
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,]
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("デバッグ用　スキルデータ")]
            [SerializeField] private Character.SkillScriptableObject[] skillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Awake()
            {
                #region リストの初期化
                for (int i = 0; i < PLAYER_AMOUNT; i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region デバッグ用　Moveリスト内部の初期化 および　Moveリスト内部の整理
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[PLAYER_MOVE_DISTANCE];
                    for (int j = 0; j < PLAYER_MOVE_DISTANCE; j++)
                    {
                        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                    }
                    MoveDataReceiver(characterDataList[i].moveSignal, i);
                }
                characterDataList[0].index = new FieldIndex(1, 1);
                characterDataList[1].index = new FieldIndex(7, 1);
                characterDataList[2].index = new FieldIndex(1, 7);
                characterDataList[3].index = new FieldIndex(7, 7);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;
                characterDataList[2].canAct = true;
                characterDataList[3].canAct = true;

                characterDataList[0].point = 10000;
                characterDataList[1].point = 10000;
                characterDataList[2].point = 10000;
                characterDataList[3].point = 10000;

                characterDataList[0].energy = 1;
                characterDataList[1].energy = 1;
                characterDataList[2].energy = 1;
                characterDataList[3].energy = 1;
                #endregion


                #region デバッグ用　Attackリストの初期化
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left);
                }
                #endregion
                #endregion

                for (int i = 0; i < characterDirections.Length; i++)
                {
                    AttackDataReceiver(characterDataList[i].attackSignal, i); // 攻撃信号を格納する
                }

                view = GetComponent<PhotonView>();
                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                setFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                characterMove = new CharacterMove(getFieldInformation, setFieldInformation, characterDirections); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                characterAttack = new CharacterAttack(animators); // CharacterAttackの生成

                view.RPC(nameof(FindAndSetCommandObject), RpcTarget.AllBufferedViaServer);
            }

            // Update is called once per frame
            void Update()
            {
                // デバッグ用　初期位置を代入
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        StartPositionSeter(characterDataList[i].index, i);
                    }
                }

                if (PLAYER_AMOUNT > positionSetMenber) return;

                if (thisTurn < lastTurn)
                {
                    // デバッグ用　イベントに登録されている関数を実行
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        phaseEvent();
                    }
                }

                // デバッグ用　キャラクター情報を確認
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        Debug.Log($"CharacterName is {characterDataList[i].thisObject.name}");
                        Debug.Log($"Index ({characterDataList[i].index.row},{characterDataList[i].index.column}) | point ({characterDataList[i].point})");
                    }
                }
            }

            #region 各種ターン処理
            public void TurnStart()
            {
                Debug.Log($"現在{thisPhase}の処理は書かれていません");

                phaseEvent = ActionSelect;

                thisPhase++;

                // phaseCompleteAction();
            }

            public void ActionSelect()
            {
                Debug.Log($"現在{thisPhase}の処理は書かれていません");

                phaseEvent = Move;

                thisPhase++;

                // phaseCompleteAction();
            }

            public void Move()
            {
                moveStart = true;

                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList,phaseCompleteAction)); // 動きを処理するコルーチンを実行

                    phaseEvent = Attack;

                    // Debug.Log(characterDataList[0].thisObject.name);

                    thisPhase++;

                    moveStart = false;
                }

            }

            public void Attack()
            {
                Debug.Log($"{thisPhase}の処理を行います");

                attackStart = true;

                // 攻撃実行フラグがtrueのとき、Attackクラスに攻撃を実行させる
                if (attackStart)
                {
                    StartCoroutine(characterAttack.AttackOrder(characterDataList, phaseCompleteAction));

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false;

                    phaseEvent = TurnEnd;

                    thisPhase++;
                }
            }

            public void TurnEnd()
            {
                Debug.Log($"現在{thisPhase}の処理は書かれていません");
                thisTurn++;
                phaseEvent = TurnStart;
                thisPhase = 0;

                for (int i = 0;i < characterDataList.Length;i++)
                {
                    characterDataList[i].energy++;
                }

                if (thisTurn >= lastTurn)
                {
                    Debug.Log($"最後のターンが終わりました　処理を終了します");
                }
            }
            #endregion

            #region 各種インターフェース
            /// <summary>
            /// 指定されたidの配列番号を持った移動信号に渡された移動信号を格納する
            /// </summary>
            /// <param name="signal">渡す移動信号</param>
            /// <param name="characterID">キャラクタID</param>
            public void MoveDataReceiver(MoveSignal signal, int characterID)
            {
                characterDataList[characterID].moveSignal = signal;
            }

            /// <summary>
            /// 指定されたidの配列番号を持った攻撃信号に渡された攻撃信号を格納する
            /// </summary>
            /// <param name="signal">渡す攻撃信号</param>
            /// <param name="playerID">キャラクタID</param>
            public void AttackDataReceiver(AttackSignal signal, int playerID)
            {
                characterDataList[playerID].attackSignal = signal;
            }

            /// <summary>
            /// 指定された配列番号を持ったFieldIndexに対し、初期位置をもらう
            /// </summary>
            /// <param name="fieldIndex">渡すIndex</param>
            /// <param name="playerID">キャラクタID</param>
            public void StartPositionSeter(FieldIndex fieldIndex, int playerID)
            {
                characterDataList[playerID].index = fieldIndex;
                positionSetMenber++;

                if (PLAYER_AMOUNT == positionSetMenber)
                {
                    thisPhase = Phase.TURN_START;

                    phaseEvent = TurnStart;
                }
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
            /// <param name="playerID">キャラクターID</param>
            public void CharacterDataReceber(GameObject thisObject,string playerName,int playerID,int characterID)
            {
                characterDataList[playerID].thisObject = thisObject;
                characterDataList[playerID].playerName = playerName;
                characterDataList[playerID].characterName = (CharacterName)characterID;

                animators[playerID] = characterDataList[playerID].thisObject.GetComponent<Animator>(); // アニメーター取得
                characterDirections[playerID] = characterDataList[playerID].thisObject.GetComponent<CharacterDirection>(); // 各キャラクターを回転させるクラスを取得する

                // Debug.Log($"CharacterID{characterID}からplayerName{playerName}をうけとりました objectNameは{characterDataList[characterID].thisObject.name}");
            }

            public void SetPhaseCompleteAction(Action phaseCompleteAction)
            {
                this.phaseCompleteAction = phaseCompleteAction;
            }

            public UICharacterDataSeter[] characterDataSeter()
            {
                UICharacterDataSeter[] dataSeters = new UICharacterDataSeter[PLAYER_AMOUNT];

                for (int i = 0;i < dataSeters.Length;i++)
                {
                    dataSeters[i] = new UICharacterDataSeter();
                    dataSeters[i].playerName = characterDataList[i].playerName;
                    dataSeters[i].point = characterDataList[i].point;
                    dataSeters[i].energy = characterDataList[i].energy;
                    dataSeters[i].characterID = characterDataList[i].characterName;
                }

                return dataSeters;
            }
            #endregion

            [PunRPC]
            public void FindAndSetCommandObject()
            {
                GameObject cd = GameObject.Find("CommandDirector");
                PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>();
                commandDirectorArray[playerCore.playerId] = cd;
            }
        }

    }
}