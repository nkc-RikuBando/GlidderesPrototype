using Glidders.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Glidders
{
    public class SelectHostOrGest : MonoBehaviourPunCallbacks
    {
        [SerializeField] GameObject createButton;
        [SerializeField] GameObject lookForButton;
        [SerializeField] GameObject createRoom;
        [SerializeField] GameObject lookForRoom;
        [SerializeField] GameObject canCreate1;
        [SerializeField] GameObject canCreate2;
        [SerializeField] GameObject returnButton;


        [SerializeField] GameObject createRoomText;
        [SerializeField] GameObject lookForRoomText;

        [SerializeField] Text playreName;

        [SerializeField] private CommandInput commandInput;

        private bool isCreate = false;
        private bool isRoomIn = false;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private PWaitingRoom photonCS;
        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_1,
            COMMAND_INPUT_2,
            COMMAND_INPUT_3,
            COMMAND_INPUT_4,
            COMMAND_INPUT_5,
            COMMAND_INPUT_6,

            COMMAND_NUMBER
        }

        // Start is called before the first frame update
        void Start()
        {
            photonCS = GameObject.Find("PhotonManager").GetComponent<PWaitingRoom>();

            commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_3] = CommandInput3;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_4] = CommandInput4;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_5] = CommandInput5;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_6] = CommandInput6;

            createButton.SetActive(true);
            lookForButton.SetActive(true);
            createRoom.SetActive(false);
            canCreate1.SetActive(false);
            canCreate2.SetActive(false);
            returnButton.SetActive(false);

            createRoomText.SetActive(false);
            lookForRoomText.SetActive(false);
        }

            // Update is called once per frame
        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
        }

        public void SetCommandTab()
        {
            
        }

        public void CommandUpdate()
        {
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, 0, 7);
        }

        private void CommandInput1()
        {
            Debug.Log(playreName);
            if ((playreName.text == null)) return;
            commandInput.SetInputNumber(0);

            createButton.SetActive(false);
            lookForButton.SetActive(false);
            createRoom.SetActive(true);
            canCreate1.SetActive(true);
            canCreate2.SetActive(false);
            returnButton.SetActive(true);

            createRoomText.SetActive(true);
            
        }

        private void CommandInput2()
        {
            if ((playreName.text == null)) return;
            commandInput.SetInputNumber(0);

            createButton.SetActive(false);
            lookForButton.SetActive(false);
            createRoom.SetActive(true);
            canCreate1.SetActive(false);
            canCreate2.SetActive(true);
            returnButton.SetActive(true);

            lookForRoomText.SetActive(true);
        }

        private void CommandInput3()
        {
            commandInput.SetInputNumber(0);

            if (isCreate) return;
            photonCS.CrateRoom();
            isCreate = true;
        }

        private void CommandInput4()
        {
            commandInput.SetInputNumber(0);

            if (isRoomIn) return;
            isRoomIn = true;
            photonCS.JoinRoom();
        }

        private void CommandInput5()
        {
            commandInput.SetInputNumber(0);

            createButton.SetActive(true);
            lookForButton.SetActive(true);
            createRoom.SetActive(false);
            canCreate1.SetActive(false);
            canCreate2.SetActive(false);

            createRoomText.SetActive(false);
            lookForRoomText.SetActive(false);
        }

        private void CommandInput6()
        {
            commandInput.SetInputNumber(0);

            photonCS.MyNameInput();
        }
    }
}
