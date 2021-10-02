using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Glidders.Manager;
using Glidders.UI;
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
           
            private int turnCount = 0;
            [SerializeField] private GameObject playerInfo;
            public int playerCount { get; private set; }

            // Start is called before the first frame update
            void Awake()
            {
                if (ActiveRule.onlineData)
                {
                    if (!PhotonNetwork.IsMasterClient) return;
                }
                StartCoroutine(WaitManagerIsActive());
            }

            /// <summary>
            /// GameDirector�Ƀ��[�����𑗂�܂��B
            /// </summary>
            /// <param name="playerCount">����̎����̃v���C���[���B</param>
            /// <param name="maxTurn">����̎����̍ő�^�[�����B</param>
            public void SetRule(int playerCount, int maxTurn)
            {
                this.playerCount = playerCount;
                SetActiveRule(this.playerCount, maxTurn);
            }

            /// <summary>
            /// ActiveRule�ɍ���̎����̏���ݒ肵�܂��B
            /// </summary>
            /// <param name="playerCount">����̎����̃v���C���[���B</param>
            /// <param name="maxTurn">����̎����̍ő�^�[�����B</param>
            private void SetActiveRule(int playerCount, int maxTurn)
            {
                ActiveRule.SetPlayerCount(playerCount);
                ActiveRule.SetMaxTurn(maxTurn);
            }

            IEnumerator WaitManagerIsActive()
            {
                // �T�[�o�[�𐶐�
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
                // �����ʒu�I���t�F�[�Y���s��
                phaseIndex = PhaseList.BEGIN_TURN;
                StartNewPhase();
                phaseDataArray[(int)phaseIndex].actionInPhase();
                while (!phaseCompleteFlg) yield return null;
                phaseIndex = phaseDataArray[(int)phaseIndex].nextPhaseId;

                // �Q�[���I���܂Ŋe�t�F�[�Y���J��Ԃ�
                while (!gameOverFlg)
                {
                    StartNewPhase();
                    phaseDataArray[(int)phaseIndex].actionInPhase();
                    while (!phaseCompleteFlg) yield return null;
                    phaseIndex = phaseDataArray[(int)phaseIndex].nextPhaseId;
                }

                // ���U���g�t�F�[�Y���s��
                phaseIndex = PhaseList.RESULT;
                StartNewPhase();
                phaseDataArray[(int)phaseIndex].actionInPhase();
                while (true) yield return null; // ���U���g�ڍs�̃t�F�[�Y�����݂��Ȃ�����
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
            ///  �^�[���J�n���Ɏ��s�B���݃^�[�����𑝉�������B
            /// </summary>
            private void AddTurnCount()
            {
                turnCount++;
            }

            /// <summary>
            ///  �^�[���I�����Ɏ��s����B���݃^�[�����ŏI�^�[�����ǂ������m�F����B
            /// </summary>
            /// <returns></returns>
            private void UpdateGameOverFlg_IsGameOverByTurnLimit()
            {
                // ���݃^�[�����^�[�����~�b�g�Ɣ�r����
                gameOverFlg = (turnCount >= ActiveRule.maxTurn);

            }

            private void GoToResultScene()
            {
                GameObject resultDataKeeper = Instantiate(resultDataKeeperPrefab) as GameObject;
                resultDataKeeper.name = ("ResultDataKeeper");
                ResultDataKeeper script = resultDataKeeper.GetComponent<ResultDataKeeper>();
                script.SetResultData(phaseInformation.GetResultData(), ActiveRule.playerCount);
                FadeManager.Instance.LoadScene("ResultScene", 0.5f);
            }

            /// <summary>
            /// �t�F�[�Y�����Ɋi�[�����z��𐶐�����B
            /// </summary>
            /// <returns>�������ꂽ�z��B</returns>
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
                            returnArray[i].actionInPhase = null;//��
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
                            returnArray[i].actionInPhase = GoToResultScene;//��
                            break;
                    }
                }

                return returnArray;
            }
        }
    }
}
