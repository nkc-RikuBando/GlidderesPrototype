using Glidders.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class SelectHostOrGest : MonoBehaviour
    {
        [SerializeField] private CommandInput commandInput;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_1,
            COMMAND_INPUT_2,

            COMMAND_NUMBER
        }

        // Start is called before the first frame update
        void Start()
        {
            commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
        }

            // Update is called once per frame
        void Update()
        {

        }

        public void SetCommandTab()
        {
            
        }

        public void CommandUpdate()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, 0, 2);
        }

        private void CommandInput1()
        {
            commandInput.SetInputNumber(0);
            
        }

        private void CommandInput2()
        {
            commandInput.SetInputNumber(0);

        }
    }
}
