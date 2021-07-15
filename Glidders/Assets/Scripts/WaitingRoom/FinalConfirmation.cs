using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class FinalConfirmation : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CommandInput commandInput;

        [SerializeField] GameObject characterPanel;
        [SerializeField] GameObject finalConfirm;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        MatchingPlayerData[] matchingPlayerData = new MatchingPlayerData[Rule.maxPlayerCount];

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
            commandInputFunctionTable[commandInput.GetInputNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_2);
        }

        private void CommandInput1()
        {
            
        }

        private void CommandInput2()
        {

        }

        public void SetPlayerInfo()
        {
            matchingPlayerData[PlayerStartBool.myPlayerNum ]
            = new MatchingPlayerData { playerID = PlayerStartBool.myPlayerNum, 
                                       playerName = PhotonNetwork.PlayerList[PlayerStartBool.myPlayerNum].NickName,
                                       characterID = CharctorSelect.setCharacter };
        }
    }
}

