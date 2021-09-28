using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class Offline_FinalConfirmation : MonoBehaviour
    {
        [SerializeField] private CommandInput commandInput;

        [SerializeField] GameObject finalPanel;
        [SerializeField] GameObject cpuPanel;

        private delegate void FinalInputFunction();
        private FinalInputFunction[] finalInputFunction;

        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_YES,
            COMMAND_INPUT_NO,

            COMMAND_NUMBER
        }

        // Start is called before the first frame update
        void Start()
        {
            finalInputFunction = new FinalInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            finalInputFunction[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            finalInputFunction[(int)SelectCommand.COMMAND_INPUT_YES] = CommandInputYES;
            finalInputFunction[(int)SelectCommand.COMMAND_INPUT_NO] = CommandInputNO;
        }

        // Update is called once per frame
        void Update()
        {
            finalInputFunction[commandInput.GetInputNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_NO);
        }

        private void CommandInputYES() //èÄîıÇ≈Ç´ÇΩ
        {
            commandInput.SetInputNumber(0);
            finalPanel.SetActive(false);
        }

        private void CommandInputNO() //èÄîıÇ≈Ç´ÇƒÇ»Ç¢
        {
            commandInput.SetInputNumber(0);

            cpuPanel.SetActive(true);
            finalPanel.SetActive(false);
        }
    }

}
