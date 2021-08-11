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
        [SerializeField] GameObject finalPanel;
        [SerializeField] Text dispRule;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        SingletonData singletonData;

        RuleInfo ruleInfo = new RuleInfo();

        int battleTurn = 0;
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

        private enum BattleRule
        {
            POINT_BATTLE = 0,
            HP_BATTLE = 1
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
            singletonData = GameObject.Find("MatchDataSingleton(Clone)").GetComponent<SingletonData>();

            stagePanel.SetActive(false);
            finalPanel.SetActive(false);

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

        private void CommandInput1() //ターン10
        {
            commandInput.SetInputNumber(0);
 
            //setTurn = (int)PointGameRule.TURN_10_GAME;
            //RuleAnnouncement(); //Photon使用時コメントアウト 
            view.RPC(nameof(TurnSetting10), RpcTarget.AllBuffered); //Photon繋がらんと無理なやつ、関数の同期（ソロモードはコメントアウト）
            ChangeSelectMenu();
            //coreManagerのLastTurnSeterにintの引数で渡す
        }

        private void CommandInput2() //ターン20
        {
            commandInput.SetInputNumber(0);

            //setTurn = (int)PointGameRule.TURN_20_GAME;
            //RuleAnnouncement(); //Photon使用時コメントアウト 
            view.RPC(nameof(TurnSetting20), RpcTarget.AllBuffered); //Photon繋がらんと無理なやつ、関数の同期（ソロモードはコメントアウト）
            ChangeSelectMenu();
        }

        private void CommandInput3() //ターン30
        {
            commandInput.SetInputNumber(0);

            //setTurn = (int)PointGameRule.TURN_30_GAME;
            //RuleAnnouncement(); //Photon使用時コメントアウト 
            view.RPC(nameof(TurnSetting30), RpcTarget.AllBuffered); //Photon繋がらんと無理なやつ、関数の同期（ソロモードはコメントアウト）
            ChangeSelectMenu();
        }

        private void CommandInput4() //ターン40
        {
            commandInput.SetInputNumber(0);

            //setTurn = (int)PointGameRule.TURN_40_GAME;
            //RuleAnnouncement(); //Photon使用時コメントアウト 
            view.RPC(nameof(TurnSetting40), RpcTarget.AllBuffered); //Photon繋がらんと無理なやつ、関数の同期（ソロモードはコメントアウト）
            ChangeSelectMenu();
        }

        private void CommandInput5() //ターン50
        {
            commandInput.SetInputNumber(0);

            //setTurn = (int)PointGameRule.TURN_50_GAME;
            //RuleAnnouncement(); //Photon使用時コメントアウト   
            view.RPC(nameof(TurnSetting50), RpcTarget.AllBuffered); //Photon繋がらんと無理なやつ、関数の同期（ソロモードはコメントアウト）
            ChangeSelectMenu();
        }

        [PunRPC]
        public void TurnSetting10()
        {
            battleTurn = (int)PointGameRule.TURN_10_GAME;
            RuleAnnouncement();
        }

        [PunRPC]
        public void TurnSetting20()
        {
            battleTurn = (int)PointGameRule.TURN_20_GAME;
            RuleAnnouncement();
        }

        [PunRPC]
        public void TurnSetting30()
        {
            battleTurn = (int)PointGameRule.TURN_30_GAME;
            RuleAnnouncement();
        }

        [PunRPC]
        public void TurnSetting40()
        {
            battleTurn = (int)PointGameRule.TURN_40_GAME;
            RuleAnnouncement();
        }

        [PunRPC]
        public void TurnSetting50()
        {
            battleTurn = (int)PointGameRule.TURN_50_GAME;
            RuleAnnouncement();
        }

        [PunRPC]
        public void RuleAnnouncement() //ルール発表
        {
            dispRule.text = "ポイント制 \n" +
                "" + battleTurn + "ターン";

            ruleInfo.matchRule = (int)BattleRule.POINT_BATTLE;
            ruleInfo.setTurn = battleTurn;

            singletonData.GetRuleData(ruleInfo);
        }

        public void ChangeSelectMenu()
        {
            Debug.Log(battleTurn);
            stagePanel.SetActive(true);
            rulePanel.SetActive(false);
        }
    }

}
