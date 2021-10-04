using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Glidders
{
    public class Offline_RuleSelect : MonoBehaviour
    {
        [SerializeField] private CommandInput commandInput;
        [SerializeField] GameObject rulePanel;
        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;
        [SerializeField] GameObject enemyCharacterPenel;
        [SerializeField] GameObject finalPanel;
        [SerializeField] Text dispRule;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private SingletonData singletonData;

        int battleRule;
        int battleTurn = 0;
        int battleHp = 0;
        int playerNum = 2;

        bool isOnline = false;
        bool isBackScene = false;


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
            COMMAND_INPUT_11,

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
            TURN_50_GAME = 50,
            TURN_999_GAME = 999
        }

        private enum HpGameRule
        {
            HP_0 = 0,
            HP_30000_GAME = 30000,
            HP_60000_GAME = 60000,
            HP_90000_GAME = 90000,
            HP_120000_GAME = 120000,
            HP_150000_GAME = 150000,

            HP_100000_TURNGAME = 100000
        }

        // Start is called before the first frame update
        void Start()
        {
            singletonData = GameObject.Find("MatchDataSingleton").GetComponent<SingletonData>();

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
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_11] = MenuBack;

            stagePanel.SetActive(false);
            charctorPanel.SetActive(false);
            enemyCharacterPenel.SetActive(false);
            finalPanel.SetActive(false);

            isBackScene = false;
        }

        // Update is called once per frame
        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_5);
        }

        private void CommandInput1() //ポイント制　10
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.POINT_BATTLE;
            battleHp = (int)HpGameRule.HP_100000_TURNGAME;
            battleTurn = (int)PointGameRule.TURN_10_GAME;
            PointRuleAnnouncement(); //Photon使用時コメントアウト 
            SetRuleInfo();
            ChangeSelectMenu();
        }
        private void CommandInput2() //ポイント制 20
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.POINT_BATTLE;
            battleHp = (int)HpGameRule.HP_100000_TURNGAME;
            battleTurn = (int)PointGameRule.TURN_20_GAME;
            PointRuleAnnouncement(); //Photon使用時コメントアウト 
            SetRuleInfo();
            ChangeSelectMenu();
        }

        private void CommandInput3() //ポイント制 30
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.POINT_BATTLE;
            battleHp = (int)HpGameRule.HP_100000_TURNGAME;
            battleTurn = (int)PointGameRule.TURN_30_GAME;
            PointRuleAnnouncement(); //Photon使用時コメントアウト 
            SetRuleInfo();
            ChangeSelectMenu();
        }

        private void CommandInput4() //ポイント制 40
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.POINT_BATTLE;
            battleHp = (int)HpGameRule.HP_100000_TURNGAME;
            battleTurn = (int)PointGameRule.TURN_40_GAME;
            PointRuleAnnouncement(); //Photon使用時コメントアウト 
            SetRuleInfo();
            ChangeSelectMenu();
        }

        private void CommandInput5() //ポイント制 50
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.POINT_BATTLE;
            battleHp = (int)HpGameRule.HP_100000_TURNGAME;
            battleTurn = (int)PointGameRule.TURN_50_GAME;
            PointRuleAnnouncement(); //Photon使用時コメントアウト 
            SetRuleInfo();
            ChangeSelectMenu();
        }

        public void PointRuleAnnouncement() //ルール発表
        {
            dispRule.text = "ポイント制 \n" +
                "" + battleTurn + "ターン";
        }

        public void HpInput1() //体力制 30000
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.HP_BATTLE;
            battleTurn = (int)PointGameRule.TURN_999_GAME;
            battleHp = (int)HpGameRule.HP_30000_GAME;
            HpRuleAnnouncement();
            SetRuleInfo();
            ChangeSelectMenu();
        }

        public void HpInput2() //体力制 60000
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.HP_BATTLE;
            battleTurn = (int)PointGameRule.TURN_999_GAME;
            battleHp = (int)HpGameRule.HP_60000_GAME;
            HpRuleAnnouncement();
            SetRuleInfo();
            ChangeSelectMenu();
        }

        public void HpInput3() //体力制 90000
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.HP_BATTLE;
            battleTurn = (int)PointGameRule.TURN_999_GAME;
            battleHp = (int)HpGameRule.HP_90000_GAME;
            HpRuleAnnouncement();
            SetRuleInfo();
            ChangeSelectMenu();
        }

        public void HpInput4() //体力制120000
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.HP_BATTLE;
            battleTurn = (int)PointGameRule.TURN_999_GAME;
            battleHp = (int)HpGameRule.HP_120000_GAME;
            HpRuleAnnouncement();
            SetRuleInfo();
            ChangeSelectMenu();
        }

        public void HpInput5() //体力制150000
        {
            commandInput.SetInputNumber(0);

            battleRule = (int)BattleRule.HP_BATTLE;
            battleTurn = (int)PointGameRule.TURN_999_GAME;
            battleHp = (int)HpGameRule.HP_150000_GAME;
            HpRuleAnnouncement();
            SetRuleInfo();
            ChangeSelectMenu();
        }

        public void HpRuleAnnouncement()
        {
            dispRule.text = "体力制 \n" +
               "" + "HP" + battleHp ;
        }

        public void MenuBack()
        {
            dispRule.text = "";
            if (isBackScene) return;
            FadeManager.Instance.LoadScene("MenuScene",0.5f);
            isBackScene = true;
        }


        private void SetRuleInfo()
        {
            singletonData.GetRuleData(battleRule, battleTurn, battleHp, isOnline);
            singletonData.GetPlayerNum(playerNum);
        }

        public void ChangeSelectMenu()
        {
            stagePanel.SetActive(true);
            rulePanel.SetActive(false);
        }
    }

}
