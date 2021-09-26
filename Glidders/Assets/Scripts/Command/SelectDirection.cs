using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Command
    {
        public class SelectDirection : MonoBehaviour, ICommand
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
            
            [SerializeField] private Graphic.HologramController hologramController = default;

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

            private enum Directions
            {
                LEFT = 1,
                FRONT,
                RIGHT,
                BACK
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
            public void CommandStart()
            {
                SetCommandTab();
                commandInput.SetSelectNumber(0);
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
                switch (selectNumber)
                {
                    case (int)Directions.LEFT:
                        hologramController.SetHologramDirection(FieldIndexOffset.left);
                        break;
                    case (int)Directions.FRONT:
                        hologramController.SetHologramDirection(FieldIndexOffset.down);
                        break;
                    case (int)Directions.RIGHT:
                        hologramController.SetHologramDirection(FieldIndexOffset.right);
                        break;
                    case (int)Directions.BACK:
                        hologramController.SetHologramDirection(FieldIndexOffset.up);
                        break;
                }
            }

            private void CommandInput1()
            {
                commandManager.SetDirectionSignal(new Manager.DirecionSignal(FieldIndexOffset.left));
                commandInput.SetInputNumber(0);
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_DIRECTION);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_CONFILM);
            }

            private void CommandInput2()
            {
                commandManager.SetDirectionSignal(new Manager.DirecionSignal(FieldIndexOffset.down));
                commandInput.SetInputNumber(0);
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_DIRECTION);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_CONFILM);
            }

            private void CommandInput3()
            {
                commandManager.SetDirectionSignal(new Manager.DirecionSignal(FieldIndexOffset.right));
                commandInput.SetInputNumber(0);
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_DIRECTION);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_CONFILM);
            }

            private void CommandInput4()
            {
                commandManager.SetDirectionSignal(new Manager.DirecionSignal(FieldIndexOffset.up));
                commandInput.SetInputNumber(0);
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_DIRECTION);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_CONFILM);
            }

            private void CommandInput5()
            {
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber(commandFlow.GetBeforeState());
            }

            public void SetCharacterObject(GameObject gameObject)
            {
                
            }
        }
    }
}
