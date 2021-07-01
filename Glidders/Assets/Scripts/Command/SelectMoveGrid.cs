using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Command
    {
        public class SelectMoveGrid : MonoBehaviour, ICommand
        {
            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            [SerializeField] private string[] tabTexts;
            [SerializeField] private Sprite[] tabIcons;
            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private string[] commandInfoTextMessage;


            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;

            private enum SelectCommand
            {
                COMMAND_NOT_INPUT,
                COMMAND_INPUT_1,

                COMMAND_NUMBER
            }

            // Start is called before the first frame update
            void Start()
            {
                commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
                commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void CommandUpdate()
            {
                commandInputFunctionTable[commandInput.GetInputNumber()]();
                if (!Input.GetKeyDown(KeyCode.Return)) return;
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL);

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
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_ACTION_OR_UNIQUE);
            }

            public void SetCommandTab()
            {
                setCommandTab.SetTab(commandSprite, tabTexts, tabIcons);
            }

            private void DisplaySelectableGrid()
            {
                int move = 2;
                int x = 3;
                int y = 4;


            }

            private void SetSelectableGrid(int x, int y, int move)
            {
                bool[,] selectableGridTable = new bool[9, 9];
                for(int i = 0; i < selectableGridTable.GetLength(0); i++)
                {
                    for(int j = 0; j < selectableGridTable.GetLength(0); j++)
                    {
                        int distance = Mathf.Abs(i - y) + Mathf.Abs(j - x);
                        if(distance > move)
                        {

                        }
                    }
                }
            }
        }
    }
}

