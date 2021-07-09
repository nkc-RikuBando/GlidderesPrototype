using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class RuleSelect : MonoBehaviour
    {
        [SerializeField] private CommandInput commandInput;

        [SerializeField] GameObject rulePanel;
        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;

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
            selectNumber = Mathf.Clamp(selectNumber, 0, 5);
        }

        private void CommandInput1() //ターン10
        {
            commandInput.SetInputNumber(0);

            setTurn = 10;
            ChangeSelectMenu();
            //coreManagerのLastTurnSeterにintの引数で渡す
        }

        private void CommandInput2() //ターン20
        {
            commandInput.SetInputNumber(0);

            setTurn = 20;
            ChangeSelectMenu();
        }

        private void CommandInput3() //ターン30
        {
            commandInput.SetInputNumber(0);

            setTurn = 30;
            ChangeSelectMenu();
        }

        private void CommandInput4() //ターン40
        {
            commandInput.SetInputNumber(0);

            setTurn = 40;
            ChangeSelectMenu();
        }

        private void CommandInput5() //ターン50
        {
            commandInput.SetInputNumber(0);

            setTurn = 50;
            ChangeSelectMenu();
        }

        public void ChangeSelectMenu()
        {
            Debug.Log(setTurn);
            stagePanel.SetActive(true);
            rulePanel.SetActive(false);
        }
    }

}
