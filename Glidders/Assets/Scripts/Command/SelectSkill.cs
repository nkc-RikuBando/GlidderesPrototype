using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Command
    {
        public class SelectSkill : MonoBehaviour, ICommand
        {
            private GameObject characterObject;

            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            private string[] tabTexts;
            const string WAIT_TEXT = "‘Ò‹@";
            const string BACK_TEXT = "–ß‚é";

            private Sprite[] tabIcons;
            [SerializeField] private Sprite waitIcon = default;
            [SerializeField] private Sprite backIcon = default;

            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private string[] commandInfoTextMessage;
            [SerializeField] private SelectSkillGrid selectSkillGrid;

            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;

            [SerializeField] private CommandManager commandManager;

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
            }

            // Update is called once per frame
            void Update()
            {

            }
            public void SetCharacterObject(GameObject gameObject)
            {
                characterObject = gameObject;
            }


            public void CommandStart()
            {
                SetCommandTab();
            }


            public void CommandUpdate()
            {
                commandInputFunctionTable[commandInput.GetInputNumber()]();
            }

            public void SetCommandTab()
            {
                Character.IGetCharacterCoreData getCharacterCoreData = characterObject.GetComponent<Character.IGetCharacterCoreData>();
                if (commandFlow.uniqueFlg)
                {
                    tabTexts = new string[] {
                        getCharacterCoreData.GetUniqueData().skillName,
                        BACK_TEXT
                    };
                    tabIcons = new Sprite[] {
                        getCharacterCoreData.GetUniqueData().skillIcon,
                        backIcon
                    };
                }
                else
                {
                    tabTexts = new string[] {
                        getCharacterCoreData.GetSkillData(1).skillName,
                        getCharacterCoreData.GetSkillData(2).skillName,
                        getCharacterCoreData.GetSkillData(3).skillName,
                        WAIT_TEXT,
                        BACK_TEXT
                    };
                    tabIcons = new Sprite[] {
                        getCharacterCoreData.GetSkillData(1).skillIcon,
                        getCharacterCoreData.GetSkillData(2).skillIcon,
                        getCharacterCoreData.GetSkillData(3).skillIcon,
                        waitIcon,
                        backIcon
                    };
                }

                setCommandTab.SetTab(commandSprite, tabTexts, tabIcons);
            }

            private void CommandNotInput()
            {
                int selectNumber = commandInput.GetSelectNumber();
                selectNumber = Mathf.Clamp(selectNumber, 0, tabTexts.Length);
                commandInfoText.text = commandInfoTextMessage[selectNumber];
            }

            private void CommandInput1()
            {
                if (commandFlow.uniqueFlg)
                {
                    Character.IGetCharacterCoreData getCharacterCoreData = characterObject.GetComponent<Character.IGetCharacterCoreData>();
                    //getCharacterCoreData.GetUniqueData().
                    //if ()
                }
                else
                {
                    commandInput.SetInputNumber(0);
                    selectSkillGrid.SetSkillNumber(1);
                    commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
                }
            }

            private void CommandInput2()
            {
                if (commandFlow.uniqueFlg)
                {
                    commandInput.SetInputNumber(0);
                    commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_ACTION_OR_UNIQUE);
                }
                else
                {
                    commandInput.SetInputNumber(0);
                    selectSkillGrid.SetSkillNumber(2);
                    commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
                }
            }

            private void CommandInput3()
            {
                commandInput.SetInputNumber(0);
                selectSkillGrid.SetSkillNumber(3);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
            }

            private void CommandInput4()
            {
                commandManager.SetAttackSignal(new Manager.AttackSignal(false));
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_DIRECTION);
            }

            private void CommandInput5()
            {
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_MOVE_GRID);
            }
        }
    }
}

