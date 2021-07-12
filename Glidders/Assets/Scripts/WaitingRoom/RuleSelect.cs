using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class RuleSelect : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CommandInput commandInput;
        PhotonView view;

        [SerializeField] GameObject rulePanel;
        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;
        [SerializeField] Text dispRule;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        int setTurn = 0;
        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_1,
            COMMAND_INPUT_2,
            COMMAND_INPUT_3,
            COMMAND_INPUT_4,
            COMMAND_INPUT_5,

            COMMAND_NUMBER
        }

        private enum PointGameRule
        {
            TURN_10_GAME = 10,
            TURN_20_GAME = 20,
            TURN_30_GAME = 30,
            TURN_40_GAME = 40,
            TURN_50_GAME = 50
        }

        // Start is called before the first frame update
        void Start()
        {
            commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_3] = CommandInput3;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_4] = CommandInput4;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_5] = CommandInput5;

            view = GetComponent<PhotonView>();

            stagePanel.SetActive(false);

            if (PublicStaticBool.isCreate)
            {
                rulePanel.SetActive(true);
                charctorPanel.SetActive(false);
            }
            else
            {
                charctorPanel.SetActive(true);
                rulePanel.SetActive(false);
            }
        }

        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_5);
        }

        private void CommandInput1() //�^�[��10
        {
            commandInput.SetInputNumber(0);
 
            setTurn = (int)PointGameRule.TURN_10_GAME;
            RuleAnnouncement(); //Photon�g�p���R�����g�A�E�g 
            view.RPC(nameof(RuleAnnouncement), RpcTarget.All); //Photon�q�����Ɩ����Ȃ�A�֐��̓����i�\�����[�h�̓R�����g�A�E�g�j
            ChangeSelectMenu();
            //coreManager��LastTurnSeter��int�̈����œn��
        }

        private void CommandInput2() //�^�[��20
        {
            commandInput.SetInputNumber(0);

            setTurn = (int)PointGameRule.TURN_20_GAME;
            RuleAnnouncement(); //Photon�g�p���R�����g�A�E�g 
            view.RPC(nameof(RuleAnnouncement), RpcTarget.All); //Photon�q�����Ɩ����Ȃ�A�֐��̓����i�\�����[�h�̓R�����g�A�E�g�j
            ChangeSelectMenu();
        }

        private void CommandInput3() //�^�[��30
        {
            commandInput.SetInputNumber(0);

            setTurn = (int)PointGameRule.TURN_30_GAME;
            RuleAnnouncement(); //Photon�g�p���R�����g�A�E�g 
            view.RPC(nameof(RuleAnnouncement), RpcTarget.All); //Photon�q�����Ɩ����Ȃ�A�֐��̓����i�\�����[�h�̓R�����g�A�E�g�j
            ChangeSelectMenu();
        }

        private void CommandInput4() //�^�[��40
        {
            commandInput.SetInputNumber(0);

            setTurn = (int)PointGameRule.TURN_40_GAME;
            RuleAnnouncement(); //Photon�g�p���R�����g�A�E�g 
            view.RPC(nameof(RuleAnnouncement), RpcTarget.All); //Photon�q�����Ɩ����Ȃ�A�֐��̓����i�\�����[�h�̓R�����g�A�E�g�j
            ChangeSelectMenu();
        }

        private void CommandInput5() //�^�[��50
        {
            commandInput.SetInputNumber(0);

            setTurn = (int)PointGameRule.TURN_50_GAME;
            RuleAnnouncement(); //Photon�g�p���R�����g�A�E�g   
            view.RPC(nameof(RuleAnnouncement), RpcTarget.All); //Photon�q�����Ɩ����Ȃ�A�֐��̓����i�\�����[�h�̓R�����g�A�E�g�j
            ChangeSelectMenu();
        }

        [PunRPC]
        public void RuleAnnouncement() //���[�����\
        {
            dispRule.text = "�|�C���g�� \n" +
                "" + setTurn + "�^�[��";
            //coreManager��LastTurnSeter��int�̈����œn��
        }

        public void ChangeSelectMenu()
        {
            Debug.Log(setTurn);
            stagePanel.SetActive(true);
            rulePanel.SetActive(false);
        }
    }

}