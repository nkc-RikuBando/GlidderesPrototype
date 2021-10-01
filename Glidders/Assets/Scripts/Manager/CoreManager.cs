using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Command;
using Glidders.Player_namespace;
using Glidders.Buff;
using Glidders.Character;
using System;
using Photon;
using Photon.Pun;
using Glidders.Director;

namespace Glidders
{
    namespace Manager
    { 
        delegate void PhaseMethod(); // フェーズごとの行動を記録した関数を登録するデリゲート
        public class CoreManager : MonoBehaviour, ICharacterDataReceiver, IPhaseInformation, IPlayerInformation
        {
            PhotonView view;

            Action phaseCompleteAction; // デリゲート

            GameObject[] commandDirectorArray = new GameObject[ActiveRule.playerCount];

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
            AutoSignalSelecter autoSignalSelecter;
            CommandFlow[] commandFlows = new CommandFlow[ActiveRule.playerCount];
            CharacterDirection[] characterDirections = new CharacterDirection[ActiveRule.playerCount];

            CharacterData[] characterDataList = new CharacterData[ActiveRule.playerCount]; // データの総量をプレイヤーの総数の分作る



            public int ruleData { get; set; }
            public static bool onlineData { get; set; }
            public int positionSetMenber { get; set; } // 初期位置を選択したメンバー数を把握する
            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            private bool selectStart { get; set; } // 入力可能かどうか
            private bool[] movesignals = new bool[ActiveRule.playerCount];
            private bool[] directionsignals = new bool[ActiveRule.playerCount];
            private bool[] attacksignals = new bool[ActiveRule.playerCount];
            MatchFormat format = new MatchFormat(); // 試合形式を管理するenum

            private Animator[] animators = new Animator[ActiveRule.playerCount]; // アニメーション管理のアニメーター変数
            private Text[] texts = new Text[ActiveRule.playerCount];

            // 消去バフを管理するList<List<int>>
            private List<List<int>> removeNumber_value = new List<List<int>>(0);
            private List<List<int>> removeNumber_view = new List<List<int>>(0);

            [SerializeField] private GameObject serverObject;
            [SerializeField] private UniqueSkillScriptableObject notActionSkill;

            [SerializeField] private int[] playerIndex_row = new int[ActiveRule.playerCount];
            [SerializeField] private int[] playerIndex_colomn = new int[ActiveRule.playerCount];

            int actionCompleateManber = 0;

            #region デバッグ用変数
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,]
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, 1), new FieldIndexOffset(0, -1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            [Header("デバッグ用　スキルデータ")]
            [SerializeField] private Character.UniqueSkillScriptableObject[] UniqueSkillScriptableObject;
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                view = GetComponent<PhotonView>();

                selectStart = true;
                #region リストの初期化
                for (int i = 0; i < ActiveRule.playerCount; i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region デバッグ用　Moveリスト内部の初期化 および　Moveリスト内部の整理
                //for (int i = 0; i < characterDataList.Length; i++)
                //{
                //    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[Rule.maxMoveAmount];
                //    for (int j = 0; j < Rule.maxMoveAmount; j++)
                //    {
                //        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                //    }
                //}

                // デバッグ用を含むキャラクタデータを初期化
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].canAct = true;
                    characterDataList[i].point = 10000;
                    characterDataList[i].energy = 3;
                    characterDataList[i].direcionSignal.direction = FieldIndexOffset.left;
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;

                    characterDataList[i].buffView = new List<BuffViewData>();
                    characterDataList[i].buffValue = new List<List<BuffValueData>>();
                    characterDataList[i].buffTurn = new List<List<int>>();
                    characterDataList[i].buffEffectObject = new List<GameObject>();
                }

                for (int i = 0;i < characterDataList.Length;i++)
                {
                    characterDataList[i].index = new FieldIndex(playerIndex_row[i], playerIndex_colomn[i]);
                }

                #endregion


                #region デバッグ用　Attackリストの初期化
                //for (int i = 0; i < characterDataList.Length; i++)
                //{
                //    characterDataList[i].attackSignal = new AttackSignal(true, UniqueSkillScriptableObject[i], new FieldIndex(3, 3), FieldIndexOffset.left,i);
                //}
                #endregion
                #endregion

                for (int i = 0; i < characterDataList.Length; i++)
                {
                    view.RPC(nameof(StartPositionSeter),RpcTarget.All, i);
                    // commandFlows[0] = GameObject.Find("CommandDirector").GetComponent<CommandFlow>();
                }

                Instantiate(serverObject);

                //for (int i = 0; i < characterDirections.Length; i++)
                //{
                //    AttackDataReceiver(characterDataList[i].attackSignal, i); // 攻撃信号を格納する
                //}

                Debug.Log("GameObject.Find(Vcam) " + (GameObject.Find("Vcam") == null));
                cameraController = GameObject.Find("Vcam").GetComponentInChildren<CameraController>();
                fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // クラス取得
                displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>(); // クラス取得
                characterMove = new CharacterMove(fieldCore, characterDirections,texts,animators); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                characterAttack = new CharacterAttack(animators,fieldCore,displayTileMap,characterDirections,cameraController,texts); // CharacterAttackの生成
                autoSignalSelecter = new AutoSignalSelecter(fieldCore,notActionSkill);

                // FindAndSetCommandObject();
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
                    for (int i = 1; i < ActiveRule.playerCount; i++)
                    {
                        characterDataList[i] = autoSignalSelecter.SignalSet(characterDataList[i],characterDataList[0]);
                        movesignals[i] = true;
                        attacksignals[i] = true;
                        directionsignals[i] = true;
                    }
                }

                // if (ActiveRule.playerCount > positionSetMenber) return;

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

            
            public void TurnStart()
            {
                Debug.Log("コール呼べよ");
                view.RPC(nameof(CallTurnStart), RpcTarget.All);
            }

            [PunRPC]
            public void CallTurnStart()
            {
                Debug.Log("コール呼ばれろよ");
                if (onlineData)
                {
                    cameraController = GameObject.Find("Vcam").GetComponentInChildren<CameraController>();
                    fieldCore = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // クラス取得
                    displayTileMap = GameObject.Find("FieldCore").GetComponent<DisplayTileMap>(); // クラス取得
                    characterMove = new CharacterMove(fieldCore, characterDirections, texts, animators); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                    characterAttack = new CharacterAttack(animators, fieldCore, displayTileMap, characterDirections, cameraController, texts); // CharacterAttackの生成
                    autoSignalSelecter = new AutoSignalSelecter(fieldCore, notActionSkill);
                }

                // キャラクタの位置を反映(初期の位置情報を反映するため)
                for (int i = 0; i < ActiveRule.playerCount; i++)
                {
                    Debug.Log("呼ばれた2");
                    Debug.Log($"thisObject({i}) = {characterDataList[i].thisObject}");
                    Debug.Log($"index({i}) = ({characterDataList[i].index.row},{characterDataList[i].index.column})");
                    Debug.Log(fieldCore == null);
                    characterDataList[i].thisObject.transform.position = fieldCore.GetTilePosition(characterDataList[i].index);
                }
                view.RPC(nameof(CallPhaseCompleteAction), RpcTarget.All); //phaseCompleteAction();
            }

            public void ActionSelect()
            {
                view.RPC(nameof(CallActionSelect), RpcTarget.All);
            }

            [PunRPC]
            public void CallActionSelect()
            {
                if (!selectStart) return;
                // Debug.Log(characterDataList[0].index.column + " : " + characterDataList[0].index.row);
                float movebuff = 0;

                Debug.Log("PhotonNetwork.PlayerList.Length " + PhotonNetwork.PlayerList.Length);
                if (onlineData)
                {
                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                        {
                            Debug.Log("i " + i);
                            for (int j = 0; j < characterDataList[i].buffView.Count; j++)
                            {
                                for (int I = 0; I < characterDataList[i].buffValue[j].Count; I++)
                                {
                                    if (characterDataList[i].buffValue[j][I].buffedStatus == Character.StatusTypeEnum.MOVE)
                                    {
                                        movebuff += characterDataList[i].buffValue[j][I].buffScale;
                                    }
                                }
                            }

                            Debug.Log($"characterDataList[{i}].thisObject = {characterDataList[i].thisObject.name}");
                            Debug.Log($"characterDataList[{i}].index = ({characterDataList[i].index.row},{characterDataList[i].index.column})");
                            Debug.Log($"characterDataList[{i}].energy = {characterDataList[i].energy}");
                            commandFlows[i].StartCommandPhase(i, characterDataList[i].thisObject, characterDataList[i].index, (int)movebuff, characterDataList[i].energy);

                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < characterDataList[0].buffView.Count; i++)
                    {
                        for (int j = 0; j < characterDataList[0].buffValue[i].Count; j++)
                        {
                            if (characterDataList[0].buffValue[i][j].buffedStatus == Character.StatusTypeEnum.MOVE)
                                movebuff += characterDataList[0].buffValue[i][j].buffScale;
                        }
                    }

                    // デバッグ用　最初のキャラのみ移動処理を行う
                    commandFlows[0].StartCommandPhase(0, characterDataList[0].thisObject, characterDataList[0].index, (int)movebuff, characterDataList[0].energy);
                }


                view.RPC(nameof(StaySelectTime),RpcTarget.All) ; // 全キャラのコマンドが完了するまで待機する

                moveStart = true; // 移動を可能にする

                // phaseCompleteAction(); // デバッグ用　フラグ管理を無視して次のフェーズへ

                // 上記の処理を外す場合、右シフトを押すとフラグがtrueになる

            }

            public void Move()
            {
                view.RPC(nameof(CallActionSelect), RpcTarget.All);
            }

            [PunRPC]
            public void CallMove()
            {
                selectStart = true;
                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
                    cameraController.ClearTarget(); // 全てのカメラ追従対象を消去する
                    for (int i = 0; i < characterDataList.Length; i++)
                    {
                        cameraController.AddTarget(characterDataList[i].thisObject.transform);
                    }

                    StartCoroutine(characterMove.MoveOrder(characterDataList,  phaseCompleteAction,onlineData)); // 動きを処理するコルーチンを実行

                    attackStart = true; // 攻撃を可能にする
                    moveStart = false; // 移動を不可能にする
                }
            }

            [PunRPC]
            public void Attack()
            {
                view.RPC(nameof(CallAttack),RpcTarget.All);
            }

            [PunRPC]
            public void CallAttack()
            {
                // 攻撃実行フラグがtrueのとき、Attackクラスに攻撃を実行させる
                if (attackStart)
                {
                    // Debug.Log("Lets.Attack");
                    StartCoroutine(characterAttack.AttackOrder(characterDataList, phaseCompleteAction,onlineData)); // 攻撃を処理するコルーチンを実行

                    // characterAttack.AttackOrder(characterDataList,phaseCompleteAction);

                    attackStart = false; // 攻撃を不可能にする
                }
            }

            [PunRPC]
            public void TurnEnd()
            {
                view.RPC(nameof(CallTurnEnd), RpcTarget.All);
            }
            #endregion

            [PunRPC]
            public void CallTurnEnd()
            {
                // ターン終了時のダメージフィールド減衰処理
                fieldCore.UpdateFieldData();
                displayTileMap.ClearDamageFieldTilemap();
                displayTileMap.DisplayDamageFieldTilemap(fieldCore.GetFieldData());

                // 各コマンド入力情報を初期化
                for (int i = 0; i < ActiveRule.playerCount; i++)
                {
                    movesignals[i] = false;
                    attacksignals[i] = false;
                    directionsignals[i] = false;

                    characterDataList[i].moveSignal.moveDataArray = new FieldIndexOffset[5];
                    characterDataList[i].attackSignal.skillData = null;
                }

                // 各キャラクタのエナジーを追加、行動不能状態を解除 また　バフのターンによる消滅処理
                for (int i = 0; i < characterDataList.Length; i++)
                {
                    characterDataList[i].energy++;
                    characterDataList[i].canAct = true;

                    for (int j = 0; j < characterDataList[i].buffValue.Count; j++) // バフのついている数分回す
                    {
                        for (int I = 0; I < characterDataList[i].buffValue[j].Count; I++) // バフ内容分回す
                        {
                            characterDataList[i].buffTurn[j][I]++; // バフの経過ターンを増やす
                            if (characterDataList[i].buffValue[j][I].buffDuration <= characterDataList[i].buffTurn[j][0]) // 経過ターンをバフ持続ターンを上回った場合、バフ内容を初期化
                            {
                                removeNumber_value.Add(new List<int>());
                                removeNumber_value[removeNumber_value.Count - 1].Add(i);
                                removeNumber_value[removeNumber_value.Count - 1].Add(j);
                                removeNumber_value[removeNumber_value.Count - 1].Add(I);

                                // Debug.Log($"removeNumber_value[{removeNumber_value.Count -1}].Count {removeNumber_value[removeNumber_value.Count-1].Count}(i = {removeNumber_value[removeNumber_value.Count - 1][0]} | j = {removeNumber_value[removeNumber_value.Count - 1][1]} | I = {removeNumber_value[removeNumber_value.Count - 1][2]})");
                            }
                        }
                    }
                }

                // 消滅ターンのバフ内容を消す
                if (removeNumber_value.Count > 0)
                {
                    int number = removeNumber_value.Count - 1; // 消滅ターンのバフ総数
                    for (int i = number; i >= 0; i--)
                    {
                        // リストから登録しておいたバフを消す
                        ListRemover(removeNumber_value[i][0], removeNumber_value[i][1], removeNumber_value[i][2]);
                    }

                    removeNumber_value = new List<List<int>>(0); // 登録情報を削除する
                }

                // バフ内容がすべて消えたバフ情報を消す
                if (removeNumber_view.Count > 0)
                {
                    int number = removeNumber_view.Count - 1; // 消滅するバフ情報総数
                    for (int i = number; i >= 0; i--)
                    {
                        // リストから登録しておいたバフを消す
                        ListRemover(removeNumber_view[i][0], removeNumber_view[i][1]);
                    }

                    removeNumber_view = new List<List<int>>(0); // 登録情報を削除する
                }

                phaseCompleteAction();
            }

            /// <summary>
            /// バフ内容消去関数
            /// </summary>
            /// <param name="i">プレイヤ番号</param>
            /// <param name="j">バフ総数</param>
            /// <param name="I">バフ内容総数</param>
            private void ListRemover(int i,int j,int I)
            {
                int count = characterDataList[i].buffValue[j].Count;

                characterDataList[i].buffTurn[j].RemoveAt(I);
                if (characterDataList[i].buffTurn[j] == null) characterDataList[i].buffTurn.RemoveAt(j);
                characterDataList[i].buffValue[j].RemoveAt(I);

                if (count == 1) // バフがついている分がなくなったとき、バフ関連情報をリストから消去する
                {
                    removeNumber_view.Add(new List<int>());
                    removeNumber_view[removeNumber_view.Count - 1].Add(i);
                    removeNumber_view[removeNumber_view.Count - 1].Add(j);
                }
            }

            /// <summary>
            /// バフ情報消去関数
            /// </summary>
            /// <param name="i">プレイヤ総数</param>
            /// <param name="j">バフ内容総数</param>
            private void ListRemover(int i,int j)
            {
                if (characterDataList[i].buffEffectObject[j] != null) Destroy(characterDataList[i].buffEffectObject[j]);
                characterDataList[i].buffEffectObject.RemoveAt(j);

                characterDataList[i].buffValue.RemoveAt(j);
                characterDataList[i].buffView.RemoveAt(j);
            }

            #region 各種インターフェース
            /// <summary>
            /// 指定されたidの配列番号を持った移動信号に渡された移動信号を格納する
            /// </summary>
            /// <param name="moveID">渡す移動信号</param>
            /// <param name="playerID">キャラクタID</param>
            public void MoveDataReceiver(int moveID, int playerID)
            {
                characterDataList[playerID].moveSignalNumber = moveID;
                movesignals[playerID] = true;
            }

            /// <summary>
            /// 指定されたidの配列番号を持った方向転換信号に渡された方向転換信号を格納する
            /// </summary>
            /// <param name="directionID"></param>
            /// <param name="playerID"></param>
            public void DirectionReceiver(int directionID, int playerID)
            {
                characterDataList[playerID].directionSignalNumber = directionID;
                directionsignals[playerID] = true;
            }

            /// <summary>
            /// 指定されたidの配列番号を持った攻撃信号に渡された攻撃信号を格納する
            /// </summary>
            /// <param name="attackID">渡す攻撃信号</param>
            /// <param name="playerID">キャラクタID</param>
            public void AttackDataReceiver(int attackID, int playerID)
            {
                characterDataList[playerID].attackSignalNumber = attackID;
                attacksignals[playerID] = true;
            }

            /// <summary>
            /// 指定された配列番号を持ったFieldIndexに対し、初期位置をもらう
            /// </summary>
            /// <param name="fieldIndex">渡すIndex</param>
            /// <param name="playerID">キャラクタID</param>
            [PunRPC]
            public void StartPositionSeter(int playerID)
            {
                characterDataList[playerID].index = new FieldIndex(playerIndex_row[playerID], playerIndex_colomn[playerID]);

                if (ActiveRule.playerCount == positionSetMenber)
                {
                    CallPhaseCompleteAction();
                }
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
            /// 
            [PunRPC]
            public void CharacterDataReceber(string thisObject,string playerName,int playerID,int characterID)
            {
                // 各種キャラクター情報を取得し、構造体に保存しておく
                characterDataList[playerID].thisObject = GameObject.Find(thisObject);
                characterDataList[playerID].playerName = playerName;
                characterDataList[playerID].playerNumber = playerID;
                characterDataList[playerID].characterName = (CharacterName)characterID;

                Debug.Log($"CharacterID = {characterID}");
                Debug.Log($"playerName = {playerName}");
                Debug.Log(thisObject);
                Debug.Log($"objectName = {characterDataList[characterID].thisObject.name}");

                animators[playerID] = characterDataList[playerID].thisObject.GetComponent<Animator>(); // アニメーター取得
                texts[playerID] = characterDataList[playerID].thisObject.GetComponentInChildren<Text>(); // テキスト取得
                characterDirections[playerID] = characterDataList[playerID].thisObject.GetComponent<CharacterDirection>(); // 各キャラクターを回転させるクラスを取得する

                
            }

            /// <summary>
            /// 処理完了のデリゲートを設定してもらう
            /// </summary>
            /// <param name="phaseCompleteAction"></param> 
            [PunRPC]
            public void SetPhaseCompleteAction(Action phaseCompleteAction)
            {
                
            }

            public void CallSetPhaseCompleteAction(Action phaseCompleteAction)
            {
                this.phaseCompleteAction = phaseCompleteAction;
            }
            /// <summary>
            /// UI側にキャラクタデータを渡す関数
            /// </summary>
            /// <returns>返却する構造体</returns>
            public UICharacterDataSeter[] characterDataSeter()
            {
                UICharacterDataSeter[] dataSeters = new UICharacterDataSeter[ActiveRule.playerCount];

                for (int i = 0;i < dataSeters.Length;i++)
                {
                    dataSeters[i] = new UICharacterDataSeter();
                    dataSeters[i].playerName = characterDataList[i].playerName;
                    dataSeters[i].point = characterDataList[i].point;
                    dataSeters[i].energy = characterDataList[i].energy;
                    dataSeters[i].characterID = characterDataList[i].characterName;
                    dataSeters[i].buffSpriteList = new List<string>();

                    for (int j = 0;j < characterDataList[i].buffView.Count;j++)
                    {
                        dataSeters[i].buffSpriteList.Add(characterDataList[i].buffView[j].id);
                    }
                }

                return dataSeters;
            }

            public void RuleDataReceber(bool onlineCheck,int matchingData)
            {
                onlineData = onlineCheck;
                ruleData = matchingData;
            }

            #endregion

            [PunRPC]
            public void FindAndSetCommandObject()
            {
                GameObject cd = GameObject.Find("CommandDirector");

                if (onlineData)
                {
                    PlayerCore playerCore = GameObject.Find("PlayerCore").GetComponent<PlayerCore>();
                    commandDirectorArray[playerCore.playerId] = cd;
                    commandFlows[playerCore.playerId] = cd.GetComponent<CommandFlow>();
                    commandFlows[playerCore.playerId].SetCoreManager(this);
                }
                else
                {
                    commandDirectorArray[0] = cd;
                    commandFlows[0] = cd.GetComponent<CommandFlow>();
                    commandFlows[0].SetCoreManager(this);
                }
            }

            [PunRPC]
            public IEnumerator StaySelectTime()
            {
                selectStart = false;
                // 全フラグがtrueになるまで待機
                while(!ListChecker(movesignals) || !ListChecker(attacksignals) || !ListChecker(directionsignals))
                {
                    yield return null;
                }

                CallPhaseCompleteAction();
            }

            bool ListChecker(bool[] lists)
            {
                for (int i = 0;i < lists.Length;i++)
                {
                    if (!lists[i]) return false;
                }

                return true;
            }

            public ResultDataStruct[] GetResultData(ResultDataStruct[] resultDataStruct)
            {
                for (int i = 0;i < resultDataStruct.Length;i++)
                {
                    resultDataStruct[i].characterId = characterDataList[i].characterName;
                    resultDataStruct[i].playerId = characterDataList[i].playerNumber;
                    resultDataStruct[i].playerName = characterDataList[i].playerName;
                    resultDataStruct[i].point = characterDataList[i].point;
                    resultDataStruct[i].ruleType = ruleData;
                    resultDataStruct[i].totalDamage = characterDataList[i].totalDamage;
                }

                return resultDataStruct;
            }

            public void CallMethod(string thisObject, string playerName, int playerID, int characterID)
            {
                view.RPC(nameof(CharacterDataReceber), RpcTarget.All, thisObject, playerName, playerID, characterID);
            }

            [PunRPC]
            public void CallPhaseCompleteAction()
            {
                if (onlineData)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        actionCompleateManber++;
                        Debug.Log("actionCompleateManber" + actionCompleateManber);
                        Debug.Log("ActiveRule.playerCount " + ActiveRule.playerCount);
                        if (actionCompleateManber >= ActiveRule.playerCount)
                        {
                            phaseCompleteAction();
                            actionCompleateManber = 0;
                        }
                    }
                }
                else
                {
                    phaseCompleteAction();
                }
            }
            public void offlineSignalSeter(AttackSignal attackSignal, MoveSignal moveSignal, DirecionSignal direcionSignal)
            {
                characterDataList[0].attackSignal = attackSignal;
                characterDataList[0].moveSignal = moveSignal;
                characterDataList[0].direcionSignal = direcionSignal;
                characterDataList[1] = autoSignalSelecter.SignalSet(characterDataList[1], characterDataList[0]);

                phaseCompleteAction();
            }
        }

    }
}