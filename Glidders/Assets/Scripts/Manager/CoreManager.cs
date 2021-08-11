using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Command;
using Glidders.Player_namespace;
using Glidders.Buff;
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

            Action phaseCompleteAction; // デリゲート

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
            CameraController cameraController;
            CommandFlow[] commandFlows = new CommandFlow[Rule.maxPlayerCount];
            CharacterDirection[] characterDirections = new CharacterDirection[Rule.maxPlayerCount];

            CharacterData[] characterDataList = new CharacterData[Rule.maxPlayerCount]; // データの総量をプレイヤーの総数の分作る

            [SerializeField] private int lastTurn = 20; // ゲーム終了ターン
            public int thisTurn { get; set; } // 現在のターン
            public int positionSetMenber { get; set; } // 初期位置を選択したメンバー数を把握する
            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            private bool[] movesignals = new bool[Rule.maxPlayerCount];
            private bool[] directionsignals = new bool[Rule.maxPlayerCount];
            private bool[] attacksignals = new bool[Rule.maxPlayerCount];
            MatchFormat format = new MatchFormat(); // 試合形式を管理するenum

            private Animator[] animators = new Animator[Rule.maxPlayerCount]; // アニメーション管理のアニメーター変数

            [SerializeField] private GameObject serverObject;

            [Header("デバッグ用　アニメーションクリップ")]
            [SerializeField] private AnimationClip[] Clips = new AnimationClip[4];

            [Header("デバッグ用　使用バフ")]
            [SerializeField] private BuffViewData[] buffViewData = new BuffViewData[4];


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
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region デバッグ用　Moveリスト内部の初期化 および　Moveリスト内部の整理
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[Rule.maxMoveAmount];
                    characterDataList[i].buffTurn = new List<int>();
                    for (int j = 0; j < Rule.maxMoveAmount; j++)
                    {
                        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                    }
                    MoveDataReceiver(characterDataList[i].moveSignal, i);
                }

                // デバッグ用を含むキャラクタデータを初期化
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].canAct = true;
                    characterDataList[i].point = 10000;
                    characterDataList[i].energy = 1;
                    characterDataList[i].direcionSignal.direction = FieldIndexOffset.left;
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;

                    characterDataList[i].buffView = new List<BuffViewData>();
                    characterDataList[i].buffValue = new List<List<BuffValueData>>();
                    if (buffViewData[i] != null)
                    {
                        // characterDataList[i].buffView[0] = buffViewData[i];
                        characterDataList[i].buffView.Add(buffViewData[i]);
                        // characterDataList[i].buffValue[0] = characterDataList[i].buffView[0].buffValueList;
                        List<BuffValueData> sampleValue = new List<BuffValueData>(characterDataList[i].buffView[0].buffValueList);
                        characterDataList[i].buffValue.Add(sampleValue);
                        characterDataList[i].buffTurn = new List<int>();
                        characterDataList[i].buffTurn.Add(0);
                    }
                }

                characterDataList[0].index = new FieldIndex(4, 4);
                characterDataList[1].index = new FieldIndex(5, 3);
                characterDataList[2].index = new FieldIndex(5, 4);
                characterDataList[3].index = new FieldIndex(5, 5);

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
                cameraController = GameObject.Find("Vcam").GetComponentInChildren<CameraController>();
                fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // クラス取得
                displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>(); // クラス取得
                characterMove = new CharacterMove(fieldCore, characterDirections); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                characterAttack = new CharacterAttack(animators,fieldCore,displayTileMap,characterDirections,cameraController); // CharacterAttackの生成

                view.RPC(nameof(FindAndSetCommandObject), RpcTarget.AllBufferedViaServer);
            }

            // Update is called once per frame
            void Update()
            {
                // デバッグ用　初期位置を代入
                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                //    phaseCompleteAction();
                //}

                // デバッグ用 全信号にtrueを代入
                if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    for (int i = 0; i < Rule.maxPlayerCount; i++)
                    {
                        movesignals[i] = true;
                        attacksignals[i] = true;
                        directionsignals[i] = true;
                    }
                }

                if (Rule.maxPlayerCount > positionSetMenber) return;

                // デバッグ用　キャラクター情報を確認
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        Debug.Log($"CharacterName is {characterDataList[i].thisObject.name}({characterDataList[i].playerName})");
                        Debug.Log($"Index ({characterDataList[i].index.row},{characterDataList[i].index.column}) | point ({characterDataList[i].point})");
                    }
                }
            }

            #region 各種ターン処理
            // PhaseCompleateAction はGameDirector 側に処理がすべて終わったことを知らせるデリゲート

            [PunRPC]
            public void TurnStart()
            {
                // 特筆すべき処理はいったんなし
                phaseCompleteAction();
            }

            [PunRPC]
            public void ActionSelect()
            {
                PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>(); // そのシーンに存在するPlayerCoreを取得する

                // commandFlows[playerCore.playerId].StartCommandPhase(playerCore.playerId,characterDataList[playerCore.playerId].thisObject,characterDataList[playerCore.playerId].index);

                StartCoroutine(StaySelectTime()); // 全キャラのコマンドが完了するまで待機する

                moveStart = true; // 移動を可能にする

                //phaseCompleteAction(); // デバッグ用　フラグ管理を無視して次のフェーズへ

                // 上記の処理を外す場合、右シフトを押すとフラグがtrueになる

            }

            [PunRPC]
            public void Move()
            {
                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
                    cameraController.ClearTarget(); // 全てのカメラ追従対象を消去する
                    for (int i = 0;i < characterDataList.Length;i++)
                    {
                        cameraController.AddTarget(characterDataList[i].thisObject.transform);
                    }

                    StartCoroutine(characterMove.MoveOrder(characterDataList, phaseCompleteAction)); // 動きを処理するコルーチンを実行

                    attackStart = true; // 攻撃を可能にする
                    moveStart = false; // 移動を不可能にする
                }
                // else phaseCompleteAction();

            }

            [PunRPC]
            public void Attack()
            {
                #region デバッグ用　スキル向き調整
                switch (thisTurn % 4)
                {
                    case 0: // 上
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.up, FieldIndexOffset.up, Mathf.Max(i, 1));
                        }
                        break;
                    case 1: // 下
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.down, FieldIndexOffset.down, Mathf.Max(i, 1));
                        }
                        break;
                    case 2: // 左
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.left, FieldIndexOffset.left, Mathf.Max(i, 1));
                        }
                        break;
                    case 3: // 右
                        for (int i = 0; i < characterDataList.Length; i++)
                        {
                            characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObject[i], characterDataList[i].index + FieldIndexOffset.right, FieldIndexOffset.right, Mathf.Max(i, 1));
                        }
                        break;
                }
                #endregion

                // Debug.Log(attackStart);

                // 攻撃実行フラグがtrueのとき、Attackクラスに攻撃を実行させる
                if (attackStart)
                {
                    // Debug.Log("Lets.Attack");
                    StartCoroutine(characterAttack.AttackOrder(characterDataList, Clips[0],phaseCompleteAction)); // 攻撃を処理するコルーチンを実行

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false; // 攻撃を不可能にする
                }
                // else phaseCompleteAction();
            }

            [PunRPC]
            public void TurnEnd()
            {
                thisTurn++; // デバッグ用の向き変更処理用　ターン管理

                // ターン終了時のダメージフィールド減衰処理
                fieldCore.UpdateFieldData(); 
                displayTileMap.ClearDamageFieldTilemap();
                displayTileMap.DisplayDamageFieldTilemap(fieldCore.GetFieldData());

                // 各コマンド入力情報を初期化
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;
                }

                // 各キャラクタのエナジーを追加、行動不能状態を解除 また　バフのターンによる消滅処理
                for (int i = 0;i < characterDataList.Length;i++)
                {
                    characterDataList[i].energy++;
                    characterDataList[i].canAct = true;

                    for (int j = 0; j < characterDataList[i].buffValue.Count; j++) // バフのついている数分回す
                    {
                        // Debug.Log(characterDataList[i].buffTurn[j]);
                        characterDataList[i].buffTurn[j]++;
                        for (int I = 0; I < characterDataList[i].buffValue[j].Count; I++) // バフ内容分回す
                        {
                            if (characterDataList[i].buffValue[j][I].buffDuration <= characterDataList[i].buffTurn[j]) // 経過ターンをバフ持続ターンを上回った場合、バフ内容を初期化
                            {
                                characterDataList[i].buffValue[j].RemoveAt(I);
                            }

                            if (characterDataList[i].buffValue[j].Count <= 0) // バフがついている分がなくなったとき、バフ関連情報をリストから消去する
                            {
                                characterDataList[i].buffTurn.RemoveAt(j);
                                characterDataList[i].buffValue.RemoveAt(j);
                                characterDataList[i].buffView.RemoveAt(j);

                                break;
                            }
                        }
                    }
                }
                
                phaseCompleteAction();
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

                if (Rule.maxPlayerCount == positionSetMenber)
                {
                    phaseCompleteAction();
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

            /// <summary>
            /// 処理完了のデリゲートを設定してもらう
            /// </summary>
            /// <param name="phaseCompleteAction"></param>
            public void SetPhaseCompleteAction(Action phaseCompleteAction)
            {
                this.phaseCompleteAction = phaseCompleteAction;
            }

            /// <summary>
            /// U側にキャラクタデータを渡す関数
            /// </summary>
            /// <returns>返却する構造体</returns>
            public UICharacterDataSeter[] characterDataSeter()
            {
                UICharacterDataSeter[] dataSeters = new UICharacterDataSeter[Rule.maxPlayerCount];

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
                // 全フラグがtrueになるまで待機
                while(!movesignals[0] || !movesignals[1] || !movesignals[2] || !movesignals[3] || !attacksignals[0] || !attacksignals[1] || !attacksignals[2] || !attacksignals[3] || !directionsignals[0] || !directionsignals[1] || !directionsignals[2] || !directionsignals[3])
                {
                    yield return null;
                }

                phaseCompleteAction();
            }

        }

    }
}