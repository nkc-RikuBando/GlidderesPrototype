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

        int battleRule;
        int battleTurn = 0;
        int battleHp = 0;

        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_1,
            COMMAND_INPUT_2,
            COMMAND_INPUT_3,
            COMMAND_INPUT_4,
            COMMAND_INPUT_5,
            COMMAND_INPUT_6,
            COMMAND_INPUT_7,
            COMMAND_INPUT_8,
            COMMAND_INPUT_9,
            COMMAND_INPUT_10,

            COMMAND_NUMBER
        }

        private enum BattleRule
        {
            POINT_BATTLE = 0,
            HP_BATTLE = 1
        }

        private enum PointGameRule
        {
            TURN_0 = 0,
            TURN_10_GAME = 10,
            TURN_20_GAME = 20,
            TURN_30_GAME = 30,
            TURN_40_GAME = 40,
            TURN_50_GAME = 50
        }

        private enum HpGameRule
        {
            HP_0 = 0,
            HP_30000_GAME = 30000,
            HP_60000_GAME = 60000,
            HP_90000_GAME = 90000,
            HP_120000_GAME = 120000,
            HP_150000_GAME = 150000
        }

        private void Awake()
        {
            PhotonNetwork.IsMessageQueueRunning = true;
            
        }

        // Start is called before the first frame update
        void Start()
        {
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

            commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_3] = CommandInput3;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_4] = CommandInput4;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_5] = CommandInput5;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_6] = HpInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_7] = HpInput2;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_8] = HpInput3;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_9] = HpInput4;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_10] = HpInput5;

            view = GetComponent<PhotonView>();
            singletonData = GameObject.Find("MatchDataSingleton").GetComponent<SingletonData>();

            stagePanel.SetActive(false);
            finalPanel.SetActive(false);
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

        /*�^�[�����̑I���A�R�[�h---------------------------------------------------------------------------------------------------------------*/
        private void CommandInput1() //�^�[��10
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(TurnSetting10), RpcTarget.AllBuffered); //Photon�q�����Ɩ����Ȃ�A�֐��̓���
            ChangeSelectMenu();
            //coreManager��LastTurnSeter��int�̈����œn��
        }

        private void CommandInput2() //�^�[��20
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(TurnSetting20), RpcTarget.AllBuffered); //Photon�q�����Ɩ����Ȃ�A�֐��̓���
            ChangeSelectMenu();
        }

        private void CommandInput3() //�^�[��30
        {
            commandInput.SetInputNumber(0);
 
            view.RPC(nameof(TurnSetting30), RpcTarget.AllBuffered); //Photon�q�����Ɩ����Ȃ�A�֐��̓���
            ChangeSelectMenu();
        }

        private void CommandInput4() //�^�[��40
        {
            commandInput.SetInputNumber(0);
 
            view.RPC(nameof(TurnSetting40), RpcTarget.AllBuffered); //Photon�q�����Ɩ����Ȃ�A�֐��̓���
            ChangeSelectMenu();
        }

        private void CommandInput5() //�^�[��50
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(TurnSetting50), RpcTarget.AllBuffered); //Photon�q�����Ɩ����Ȃ�A�֐��̓���
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
            Debug.Log("3333333333");
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

        public void RuleAnnouncement() //���[�����\
        {
            dispRule.text = "�|�C���g�� \n" +
                "" + battleTurn + "�^�[��";

            battleRule = (int)BattleRule.POINT_BATTLE;
            battleHp = (int)HpGameRule.HP_0;
            SetRuleInfo();
        }

        /*�̗͐��̑I���A�R�[�h---------------------------------------------------------------------------------------------------------------------------*/
        public void HpInput1() //�̗͐� 30000
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(HpSetting30000), RpcTarget.AllBufferedViaServer);
            ChangeSelectMenu();
        }

        public void HpInput2() //�̗͐� 60000
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(HpSetting60000), RpcTarget.AllBufferedViaServer);
            ChangeSelectMenu();
        }

        public void HpInput3() //�̗͐� 90000
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(HpSetting90000), RpcTarget.AllBufferedViaServer);
            ChangeSelectMenu();
        }

        public void HpInput4() //�̗͐�120000
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(HpSetting120000), RpcTarget.AllBufferedViaServer);
            ChangeSelectMenu();
        }

        public void HpInput5() //�̗͐�150000
        {
            commandInput.SetInputNumber(0);

            view.RPC(nameof(HpSetting150000), RpcTarget.AllBufferedViaServer);
            ChangeSelectMenu();
        }

        [PunRPC]
        public void HpSetting30000()
        {
            battleHp = (int)HpGameRule.HP_30000_GAME;
            HpRuleAnnouncement();
        }

        [PunRPC]
        public void HpSetting60000()
        {
            battleHp = (int)HpGameRule.HP_60000_GAME;
            HpRuleAnnouncement();
        }

        [PunRPC]
        public void HpSetting90000()
        {
            battleHp = (int)HpGameRule.HP_90000_GAME;
            HpRuleAnnouncement();
        }

        [PunRPC]
        public void HpSetting120000()
        {
            battleHp = (int)HpGameRule.HP_120000_GAME;
            HpRuleAnnouncement();
        }

        [PunRPC]
        public void HpSetting150000()
        {
            battleHp = (int)HpGameRule.HP_150000_GAME;
            HpRuleAnnouncement();
        }

        public void HpRuleAnnouncement()
        {
            dispRule.text = "�̗͐� \n" +
               "" + "HP" + battleHp;

            battleRule = (int)BattleRule.HP_BATTLE;
            battleTurn = (int)PointGameRule.TURN_0;
            SetRuleInfo();
        }

        private void SetRuleInfo()
        {
            singletonData.GetRuleData(battleRule, battleTurn, battleHp);
        }

        public void ChangeSelectMenu()
        {
            stagePanel.SetActive(true);
            rulePanel.SetActive(false);
        }
    }

}
