using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Glidders.Manager;
using Glidders.UI;

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
           
            private int turnCount = 0;
            public int playerCount { get; private set; } = 3;

            // Start is called before the first frame update
            void Awake()
            {
                StartCoroutine(WaitManagerIsActive());
            }

            IEnumerator WaitManagerIsActive()
            {
                // サーバーを生成
                coreManagerObject = Instantiate(coreManagerPrefab);
                phaseInformation = coreManagerObject.GetComponent<IPhaseInformation>();

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
                while (!phaseCompleteFlg) yield return null;
                phaseIndex = phaseDataArray[(int)phaseIndex].nextPhaseId;
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
                gameOverFlg = (turnCount >= 30);

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
                            break;
                        case PhaseList.RESULT:
                            returnArray[i].nextPhaseId = PhaseList.SET_STARTING_POSITION;
                            returnArray[i].actionInPhase = null;//※
                            break;
                    }
                }

                return returnArray;
            }
        }
    }
}
