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
            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            [SerializeField] private string[] tabTexts;
            [SerializeField] private Sprite[] tabIcons;
            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private string[] commandInfoTextMessage;
            [SerializeField] private SelectSkillGrid selectSkillGrid;

            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;

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

            public void CommandUpdate()
            {
                commandInputFunctionTable[commandInput.GetInputNumber()]();
            }

            public void SetCommandTab()
            {
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
                commandInput.SetInputNumber(0);
                selectSkillGrid.SetSkillNumber(1);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
            }

            private void CommandInput2()
            {
                commandInput.SetInputNumber(0);
                selectSkillGrid.SetSkillNumber(2);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
            }

            private void CommandInput3()
            {
                commandInput.SetInputNumber(0);
                selectSkillGrid.SetSkillNumber(3);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
            }

            private void CommandInput4()
            {
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_DIRECTION);
            }

            private void CommandInput5()
            {
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_MOVE_GRID);
            }

            public void CommandStart()
            {

            }
        }
    }
}

