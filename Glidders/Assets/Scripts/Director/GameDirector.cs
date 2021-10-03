using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Glidders.Manager;
using Glidders.UI;
using Glidders.Graphic;
using Photon.Pun;

namespace Glidders
{
    namespace Director
    {
        public class GameDirector : MonoBehaviour
        {
            [SerializeField] GameObject coreManagerPrefab;
            [NonSerialized] public GameObject coreManagerObject;

            PhaseDataStruct[] phaseDataArray = new PhaseDataStruct[(int)PhaseList.count];
            IPhaseInformation phaseInformation;
            PhaseList phaseIndex;
            Action phaseCompleteAction;

            private bool phaseCompleteFlg = false;
            private bool gameOverFlg = false;

            public bool completeStart = false;

            public GameObject resultDataKeeperPrefab;
            private CommentOutput commentOutput;
           
            private int turnCount = 0;
            [SerializeField] private GameObject playerInfo;
            public int playerCount { get; private set; }

            // ターンUIを表示するための変数
            public Text thisTurnCount;          // 現在ターン数
            public Text maxTurnCount;           // 最大ターン数
            private Color red = new Color(224, 48, 0);
            private Color orange = new Color(224, 128, 0);
            private Color yellow = new Color(224, 224, 0);

            // Start is called before the first frame update
            void Awake()
            {
                if (ActiveRule.onlineData)
                {
                    if (!PhotonNetwork.IsMasterClient) return;
                }

                // ターンUIの取得
                Transform turnUIPanel = GameObject.Find("Canvas").transform.Find("TurnTextPanel");
                thisTurnCount = turnUIPanel.Find("ThisTurnCount").GetComponent<Text>();
                maxTurnCount = turnUIPanel.Find("MaxTurnCount").GetComponent<Text>();

                // コメントスクリプトの取得
                commentOutput = GameObject.Find("CommentOutputSystem").GetComponent<CommentOutput>();

                StartCoroutine(WaitManagerIsActive());
            }

            private void UpdateTurnUI()
            {
                thisTurnCount.text = string.Format("{0:##}", turnCount);
                maxTurnCount.text = string.Format("/ {0:##} ターン", ActiveRule.maxTurn);  // 最大ターン数UIを設定
                int turnLeft = ActiveRule.maxTurn - turnCount;  // 残りターン数 
                if (turnLeft < 5)
                    thisTurnCount.color = yellow;
                if (turnLeft < 3)
                    thisTurnCount.color = orange;
                if (turnLeft < 1)
                    thisTurnCount.color = red;
            }

            /// <summary>
            /// GameDirectorにルール情報を送ります。
            /// </summary>
            /// <param name="playerCount">今回の試合のプレイヤー数。</param>
            /// <param name="maxTurn">今回の試合の最大ターン数。</param>
            public void SetRule(int playerCount, int maxTurn)
            {
                this.playerCount = playerCount;
                SetActiveRule(this.playerCount, maxTurn);
            }

            /// <summary>
            /// ActiveRuleに今回の試合の情報を設定します。
            /// </summary>
            /// <param name="playerCount">今回の試合のプレイヤー数。</param>
            /// <param name="maxTurn">今回の試合の最大ターン数。</param>
            private void SetActiveRule(int playerCount, int maxTurn)
            {
                ActiveRule.SetPlayerCount(playerCount);
                ActiveRule.SetMaxTurn(maxTurn);
            }

            IEnumerator WaitManagerIsActive()
            {
                // サーバーを生成
                if (ActiveRule.onlineData) coreManagerObject = PhotonNetwork.Instantiate("ManagerCore", Vector2.zero, Quaternion.identity);
                else coreManagerObject = Instantiate(coreManagerPrefab);
                phaseInformation = coreManagerObject.GetComponent<IPhaseInformation>();
                completeStart = true;

                phaseDataArray = SetPhaseData();
                phaseCompleteAction = PhaseComplete;
                phaseInformation.SetPhaseCompleteAction(phaseCompleteAction);
                StartNewPhase();
                while (!phaseCompleteFlg) yield return null;
                StartCoroutine(CallPhaseAction());
            }

            IEnumerator CallPhaseAction()
            {
                // 初期位置選択フェーズを行う
                phaseIndex = PhaseList.BEGIN_TURN;
                StartNewPhase();
                phaseDataArray[(int)phaseIndex].actionInPhase();
                while (!phaseCompleteFlg) yield return null;
                phaseIndex = phaseDataArray[(int)phaseIndex].nextPhaseId;

                // ゲーム終了まで各フェーズを繰り返す
                while (!gameOverFlg)
                {
                    StartNewPhase();
                    phaseDataArray[(int)phaseIndex].actionInPhase();
                    while (!phaseCompleteFlg) yield return null;
                    phaseIndex = phaseDataArray[(int)phaseIndex].nextPhaseId;
                }

                // リザルトフェーズを行う
                phaseIndex = PhaseList.RESULT;
                StartNewPhase();
                phaseDataArray[(int)phaseIndex].actionInPhase();
                while (true) yield return null; // リザルト移行のフェーズが存在しないため
                //phaseIndex = phaseDataArray[(int)phaseIndex].nextPhaseId;
            }

            public void PhaseComplete()
            {
                phaseCompleteFlg = true;
            }

            private void StartNewPhase()
            {
                phaseCompleteFlg = false;
            }

            /// <summary>
            ///  ターン開始時に実行。現在ターン数を増加させる。
            /// </summary>
            private void AddTurnCount()
            {
                turnCount++;
            }

            /// <summary>
            ///  ターン終了時に実行する。現在ターンが最終ターンかどうかを確認する。
            /// </summary>
            /// <returns></returns>
            private void UpdateGameOverFlg_IsGameOverByTurnLimit()
            {
                // 現在ターンをターンリミットと比較する
                gameOverFlg = (turnCount >= ActiveRule.maxTurn);

            }

            private void IsGameOverByHP()
            {
                gameOverFlg |= phaseInformation.HitPointChecker();
            }

            private void GoToResultScene()
            {
                StartCoroutine(WaitCutInAndGoToResultScene());
            }

            IEnumerator WaitCutInAndGoToResultScene()
            {
                // ゲーム終了のカットインを呼ぶ
                DisplayPhaseCutIn cutInScript = GameObject.Find("Canvas").transform.Find("PhaseCutIn").Find("CutInImage").GetComponent<DisplayPhaseCutIn>();
                cutInScript.StartGameSetCutIn();
                yield return new WaitForSeconds(1.5f);

                // コメントを止めておく
                commentOutput.StopComment();

                GameObject resultDataKeeper = Instantiate(resultDataKeeperPrefab) as GameObject;
                resultDataKeeper.name = ("ResultDataKeeper");
                ResultDataKeeper script = resultDataKeeper.GetComponent<ResultDataKeeper>();
                script.SetResultData(phaseInformation.GetResultData(), ActiveRule.playerCount, ActiveRule.maxTurn);
                FadeManager.Instance.LoadScene("ResultScene", 0.5f);
                StopCoroutine(WaitCutInAndGoToResultScene());
            }

            /// <summary>
            /// フェーズを順に格納した配列を生成する。
            /// </summary>
            /// <returns>生成された配列。</returns>
            private PhaseDataStruct[] SetPhaseData()
            {
                PhaseDataStruct[] returnArray = new PhaseDataStruct[(int)PhaseList.count];

                for (int i = 0; i < (int)PhaseList.count; i++)
                {
                    returnArray[i].phaseId = (PhaseList)i;

                    switch (returnArray[i].phaseId)
                    {
                        case PhaseList.SET_STARTING_POSITION:
                            returnArray[i].nextPhaseId = PhaseList.BEGIN_TURN;
                            returnArray[i].actionInPhase = null;//※
                            break;
                        case PhaseList.BEGIN_TURN:
                            returnArray[i].nextPhaseId = PhaseList.INPUT_COMMAND;
                            returnArray[i].actionInPhase = AddTurnCount;
                            returnArray[i].actionInPhase += UpdateTurnUI;
                            returnArray[i].actionInPhase += phaseInformation.TurnStart;
                            break;
                        case PhaseList.INPUT_COMMAND:
                            returnArray[i].nextPhaseId = PhaseList.CHARACTER_MOVE;
                            returnArray[i].actionInPhase = phaseInformation.ActionSelect;
                            break;
                        case PhaseList.CHARACTER_MOVE:
                            returnArray[i].nextPhaseId = PhaseList.CHARACTER_ATTACK;
                            returnArray[i].actionInPhase = phaseInformation.Move;
                            break;
                        case PhaseList.CHARACTER_ATTACK:
                            returnArray[i].nextPhaseId = PhaseList.END_TURN;
                            returnArray[i].actionInPhase = phaseInformation.Attack;
                            break;
                        case PhaseList.END_TURN:
                            returnArray[i].nextPhaseId = PhaseList.BEGIN_TURN;
                            returnArray[i].actionInPhase = phaseInformation.TurnEnd;
                            returnArray[i].actionInPhase += UpdateGameOverFlg_IsGameOverByTurnLimit;
                            returnArray[i].actionInPhase += IsGameOverByHP;
                            break;
                        case PhaseList.RESULT:
                            returnArray[i].nextPhaseId = PhaseList.SET_STARTING_POSITION;
                            returnArray[i].actionInPhase = GoToResultScene;//※
                            break;
                    }
                }

                return returnArray;
            }
        }
    }
}
