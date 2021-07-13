using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Glidders
{
    public class CharctorSelect : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CommandInput commandInput;
        PhotonView view;

        [SerializeField] GameObject rulePanel;
        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;
        [SerializeField] Text dispCharctor;

        [SerializeField] GameObject kaitoDescription;
        [SerializeField] GameObject seiraDescription;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private delegate void CharctorTouchFunction();
        private CharctorTouchFunction[] charctorTouchFunctionTable;

        private bool isPlayerWait = false;

        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_1,
            COMMAND_INPUT_2,
            COMMAND_INPUT_3,
            COMMAND_INPUT_4,

            COMMAND_NUMBER
        }

        private enum SelectCharctor
        {
            SELECT_NOT_CHARCTOR,
            SELECT_CHARCTOR_KAITO,
            SELECT_CHARCTOR_SEIRA,
            SELECT_CHARCTOR_MITUHA,

            CHARCTOR_NUMBER
        }

        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();

            commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_3] = CommandInput3;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_3] = CommandInput4;

            charctorTouchFunctionTable = new CharctorTouchFunction[(int)SelectCharctor.CHARCTOR_NUMBER];
            charctorTouchFunctionTable[(int)SelectCharctor.SELECT_NOT_CHARCTOR] = CharctorNotTouch;
            charctorTouchFunctionTable[(int)SelectCharctor.SELECT_CHARCTOR_KAITO] = CharctorTouch1;
            charctorTouchFunctionTable[(int)SelectCharctor.SELECT_CHARCTOR_SEIRA] = CharctorTouch2;

            kaitoDescription.SetActive(false);
            seiraDescription.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
            charctorTouchFunctionTable[commandInput.GetSelectNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_4);
        }

        private void CommandInput1()
        {
            commandInput.SetInputNumber(0);
            //view.RPC(nameof(), RpcTarget.All);
        }

        private void CommandInput2()
        {
            commandInput.SetInputNumber(0);
        }

        private void CommandInput3()
        {
            commandInput.SetInputNumber(0);
        }

        private void CommandInput4() //ステージ選択に戻る
        {
            commandInput.SetInputNumber(0);

            stagePanel.SetActive(true);
            charctorPanel.SetActive(false);
        }

        public void CharctorAnnouncement() //選手発表
        {
            //dispCharctor.text = "キャラクター名 \n" + ;
        }

        private void CharctorNotTouch()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCharctor.SELECT_NOT_CHARCTOR, (int)SelectCharctor.SELECT_CHARCTOR_SEIRA);
        }

        private void CharctorTouch1()
        {
            kaitoDescription.SetActive(true);
        }

        private void CharctorTouch2()
        {
            seiraDescription.SetActive(true);
        }

        public void EnterTheVenue(string sceneName) //会場入り
        {
            //全員がキャラクター選択できたら
            if (!isPlayerWait == true) return;
            Debug.Log(sceneName);
        }

    }
}

