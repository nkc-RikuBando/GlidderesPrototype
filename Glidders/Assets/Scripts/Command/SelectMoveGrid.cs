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

            [SerializeField] private Field.IGetFieldInformation iGetFieldInformation;

            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;

            bool[,] selectableGridTable = new bool[9, 9];

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
                //DisplaySelectableGrid();
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
                // SetSelectableGrid(iGetFieldInformation.GetPlayerPosition(0),2);
                SetSelectableGrid(new FieldIndex(3, 2), 2);
                DisplayTile();
            }

            private void SetSelectableGrid(FieldIndex playerPosition, int move)
            {
                for(int i = 0; i < selectableGridTable.GetLength(0); i++)
                {
                    for(int j = 0; j < selectableGridTable.GetLength(1); j++)
                    {
                        int distance = Mathf.Abs(i - playerPosition.row) + Mathf.Abs(j - playerPosition.column);
                        selectableGridTable[i, j] = (distance <= move) && iGetFieldInformation.IsPassingGrid(new FieldIndex(i, j));
                    }
                }
            }

            private void DisplayTile()
            {

            }
        }
    }
}

