using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Command;
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
            FieldCore fieldCore;
            DisplayTileMap displayTileMap;
            IGetFieldInformation getFieldInformation;
            ISetFieldInformation setFieldInformation;
            CommandFlow[] commandFlows = new CommandFlow[Rule.maxPlayerCount];
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // データの総量をプレイヤーの総数の分作る

            [SerializeField] private int lastTurn = 20; // ゲーム終了ターン
            public int thisTurn { get; set; } // 現在のターン
            public int positionSetMenber { get; set; } // 初期位置を選択したメンバー数を把握する
            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            private bool[] movesignals = new bool[Rule.maxPlayerCount];
            private bool[] directionsignals = new bool[Rule.maxPlayerCount];
            private bool[] attacksignals = new bool[Rule.maxPlayerCount];
            Phase thisPhase = new Phase(); // フェーズを管理するenum
            MatchFormat format = new MatchFormat(); // 試合形式を管理するenum

            event PhaseMethod phaseEvent = delegate () { }; // イベント生成

            private Animator[] animators = new Animator[PLAYER_AMOUNT]; // アニメーション管理のアニメーター変数

            [SerializeField] private GameObject serverObject;

            #region デバッグ用変数
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,]
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("デバッグ用　スキルデータ")]
            [SerializeField] private Character.SkillScriptableObject[] skillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Start()
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

                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].canAct = true;
                    characterDataList[i].point = 10000;
                    characterDataList[i].energy = 1;
                    characterDataList[i].direcionSignal.direction = FieldIndexOffset.left;
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;
                }

                characterDataList[0].index = new FieldIndex(5, 1);
                characterDataList[1].index = new FieldIndex(7, 1);
                characterDataList[2].index = new FieldIndex(5, 7);
                characterDataList[3].index = new FieldIndex(7, 7);

                #endregion


                #region デバッグ用　Attackリストの初期化
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left,i);
                }
                #endregion
                #endregion

                for (int i = 0; i < characterDataList.Length; i++)
                {
                    StartPositionSeter(characterDataList[i].index, i);
                }

                Instantiate(serverObject);

                for (int i = 0; i < characterDirections.Length; i++)
                {
                    AttackDataReceiver(characterDataList[i].attackSignal, i); // 攻撃信号を格納する
                }

                view = GetComponent<PhotonView>();
                fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>();
                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                setFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                characterMove = new CharacterMove(getFieldInformation, setFieldInformation, characterDirections); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                characterAttack = new CharacterAttack(animators,fieldCore,displayTileMap,characterDirections); // CharacterAttackの生成

                // デバッグ用　前スキルをいったん上方向に撃ったと仮定
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.up, FieldIndexOffset.down, i);
                }

                view.RPC(nameof(FindAndSetCommandObject), RpcTarget.AllBufferedViaServer);
            }

            // Update is called once per frame
            void Update()
            {
                // デバッグ用　初期位置を代入
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    phaseCompleteAction();
                }

                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    for (int i = 0; i < Rule.maxPlayerCount; i++)
                    {
                        movesignals[i] = true;
                        attacksignals[i] = true;
                        directionsignals[i] = true;
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
            [PunRPC]
            public void TurnStart()
            {
                Debug.Log($"現在{thisPhase}の処理は書かれていません");

                phaseEvent = ActionSelect;

                thisPhase++;

                phaseCompleteAction();
            }

            [PunRPC]
            public void ActionSelect()
            {
                Debug.Log($"現在{thisPhase}の処理は書かれていません");

                PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>();

                // commandFlows[playerCore.playerId].StartCommandPhase(playerCore.playerId,characterDataList[playerCore.playerId].thisObject,characterDataList[playerCore.playerId].index);

                StartCoroutine(StaySelectTime());

                phaseEvent = Move;
                
                moveStart = true;
                thisPhase++;

                phaseCompleteAction();
            }

            [PunRPC]
            public void Move()
            {
                Debug.Log($"Moveの処理を行います({thisPhase})");

                // moveStart = true;

                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList, phaseCompleteAction)); // 動きを処理するコルーチンを実行

                    phaseEvent = Attack;

                    // Debug.Log(characterDataList[0].thisObject.name);

                    thisPhase++;

                    attackStart = true;
                    moveStart = false;
                }
                else phaseCompleteAction();

            }

            [PunRPC]
            public void Attack()
            {
                Debug.Log($"Attackの処理を行います({thisPhase})");

                // デバッグ用　スキル向き調整
                switch(thisTurn % 4)
                {
                    case 0: // 上
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.up, FieldIndexOffset.up, i);
                        }
                        break;
                    case 1: // 下
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.down, FieldIndexOffset.down, i);
                        }
                        break;
                    case 2: // 左
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.left, FieldIndexOffset.left, i);
                        }
                        break;
                    case 3: // 右
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.right, FieldIndexOffset.right, i);
                        }
                        break;
                }

                // Debug.Log(attackStart);

                attackStart = true;

                // 攻撃実行フラグがtrueのとき、Attackクラスに攻撃を実行させる
                if (attackStart)
                {
                    Debug.Log("Lets.Attack");
                    StartCoroutine(characterAttack.AttackOrder(characterDataList, phaseCompleteAction));

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false;

                    phaseEvent = TurnEnd;

                    thisPhase++;
                }
                else phaseCompleteAction();
            }

            [PunRPC]
            public void TurnEnd()
            {
                Debug.Log($"現在{thisPhase}の処理は書かれていません");
                displayTileMap.DisplayDamageFieldTilemap(fieldCore.GetFieldData());
                thisTurn++;
                phaseEvent = TurnStart;
                thisPhase = 0;
                phaseCompleteAction();

                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;
                }

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
            /// <param name="playerID">キャラクタID</param>
            public void MoveDataReceiver(MoveSignal signal, int playerID)
            {
                characterDataList[playerID].moveSignal = signal;
                movesignals[playerID] = true;
            }

            /// <summary>
            /// 指定されたidの配列番号を持った方向転換信号に渡された方向転換信号を格納する
            /// </summary>
            /// <param name="signal"></param>
            /// <param name="playerID"></param>
            public void DirectionReceiver(DirecionSignal signal, int playerID)
            {
                characterDataList[playerID].direcionSignal = signal;
                directionsignals[playerID] = true;
            }

            /// <summary>
            /// 指定されたidの配列番号を持った攻撃信号に渡された攻撃信号を格納する
            /// </summary>
            /// <param name="signal">渡す攻撃信号</param>
            /// <param name="playerID">キャラクタID</param>
            public void AttackDataReceiver(AttackSignal signal, int playerID)
            {
                characterDataList[playerID].attackSignal = signal;
                attacksignals[playerID] = true;
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
                // 各種キャラクター情報を取得し、構造体に保存しておく
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
                commandFlows[playerCore.playerId] = cd.GetComponent<CommandFlow>();
                commandFlows[playerCore.playerId].SetCoreManager(this);
            }

            public IEnumerator StaySelectTime()
            {
                while(!movesignals[0] || !movesignals[1] || !movesignals[2] || !movesignals[3] || !attacksignals[0] || !attacksignals[1] || !attacksignals[2] || !attacksignals[3] || !directionsignals[0] || !directionsignals[1] || !directionsignals[2] || !directionsignals[3])
                {
                    yield return null;
                }

                phaseCompleteAction();
            }

        }

    }
}