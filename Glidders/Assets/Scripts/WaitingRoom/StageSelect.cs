using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class StageSelect : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CommandInput commandInput;
        private CharctorSelect charctorSelect;
        PhotonView view;

        [SerializeField] GameObject rulePanel;
        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;
        [SerializeField] Text dispStage;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        string[] strStage = {"","スタンダード闘技場" };
        string[] strScene = {"", "Stage1Scene" };

        private string stageName;
        private string sceneName;

        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_1,
            COMMAND_INPUT_2,

            COMMAND_NUMBER
        }

        private enum StageNum
        {
            SELECT_NOT_STAGE,
            SELECT_STAGE_1,
        }

        // Start is called before the first frame update
        void Start()
        {
            charctorSelect = GameObject.Find("CharctorMenu").GetComponent<CharctorSelect>();
            view = GetComponent<PhotonView>();

            commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
        }

        // Update is called once per frame
        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_2);
        }

        private void CommandInput1() //ルール設定に戻る
        {
            commandInput.SetInputNumber(0);

            rulePanel.SetActive(true);
            stagePanel.SetActive(false);
        }

        private void CommandInput2() //ステージ1を選択してキャラクター選択に移動
        {
            commandInput.SetInputNumber(0);

            stageName = strStage[(int)StageNum.SELECT_STAGE_1];
            sceneName = strScene[(int)StageNum.SELECT_STAGE_1];

            VenueAnnouncement(); //Photon使用時コメントアウト 
            view.RPC(nameof(VenueAnnouncement), RpcTarget.All); //Photon繋がらんと無理なやつ、関数の同期（ソロモードはコメントアウト）
            ChangeCharctorSelect();
        }

        [PunRPC]
        public void VenueAnnouncement() //会場発表
        {
            dispStage.text = "ステージ名 \n" + stageName;
            charctorSelect.EnterTheVenue(sceneName);
        }

        public void ChangeCharctorSelect()
        {
            charctorPanel.SetActive(true);
            stagePanel.SetActive(false);
        }
    }
}

