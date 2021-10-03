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

            // �^�[��UI��\�����邽�߂̕ϐ�
            public Text thisTurnCount;          // ���݃^�[����
            public Text maxTurnCount;           // �ő�^�[����
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

                // �^�[��UI�̎擾
                Transform turnUIPanel = GameObject.Find("Canvas").transform.Find("TurnTextPanel");
                thisTurnCount = turnUIPanel.Find("ThisTurnCount").GetComponent<Text>();
                maxTurnCount = turnUIPanel.Find("MaxTurnCount").GetComponent<Text>();

                // �R�����g�X�N���v�g�̎擾
                commentOutput = GameObject.Find("CommentOutputSystem").GetComponent<CommentOutput>();

                StartCoroutine(WaitManagerIsActive());
            }

            private void UpdateTurnUI()
            {
                thisTurnCount.text = string.Format("{0:##}", turnCount);
                maxTurnCount.text = string.Format("/ {0:##} �^�[��", ActiveRule.maxTurn);  // �ő�^�[����UI��ݒ�
                int turnLeft = ActiveRule.maxTurn - turnCount;  // �c��^�[���� 
                if (turnLeft < 5)
                    thisTurnCount.color = yellow;
                if (turnLeft < 3)
                    thisTurnCount.color = orange;
                if (turnLeft < 1)
                    thisTurnCount.color = red;
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
                // �Q�[���I���̃J�b�g�C�����Ă�
                DisplayPhaseCutIn cutInScript = GameObject.Find("Canvas").transform.Find("PhaseCutIn").Find("CutInImage").GetComponent<DisplayPhaseCutIn>();
                cutInScript.StartGameSetCutIn();
                yield return new WaitForSeconds(1.5f);

                // �R�����g���~�߂Ă���
                commentOutput.StopComment();

                GameObject resultDataKeeper = Instantiate(resultDataKeeperPrefab) as GameObject;
                resultDataKeeper.name = ("ResultDataKeeper");
                ResultDataKeeper script = resultDataKeeper.GetComponent<ResultDataKeeper>();
                script.SetResultData(phaseInformation.GetResultData(), ActiveRule.playerCount, ActiveRule.maxTurn);
                FadeManager.Instance.LoadScene("ResultScene", 0.5f);
                StopCoroutine(WaitCutInAndGoToResultScene());
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
                            returnArray[i].actionInPhase = GoToResultScene;//��
                            break;
                    }
                }

                return returnArray;
            }
        }
    }
}
