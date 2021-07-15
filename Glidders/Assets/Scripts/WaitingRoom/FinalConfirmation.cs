using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class FinalConfirmation : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CommandInput commandInput;
        private PlayerStartBool playerStartBool;
        PhotonView view;

        [SerializeField] GameObject finalPanel;

        private delegate void FinalInputFunction();
        private FinalInputFunction[] finalInputFunction;

        SingletonData singletonData;

        MatchingPlayerData[] matchingPlayerData = new MatchingPlayerData[Rule.maxPlayerCount];

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

            singletonData = GameObject.Find("Singleton").GetComponent<SingletonData>();
            playerStartBool = GameObject.Find("GameStartFlg").GetComponent<PlayerStartBool>();
            view = GetComponent<PhotonView>();
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


        private void CommandInputYES()
        {
            commandInput.SetInputNumber(0);

            Debug.Log("GO");
            SetPlayerInfo();
            StartFlgChange();
            playerStartBool.CallMethod();
            Debug.Log(playerStartBool.gameStartBool[PlayerStartBool.myPlayerNum]);
        }

        private void CommandInputNO()
        {
            commandInput.SetInputNumber(0);

            finalPanel.SetActive(false);
        }

        public void SetPlayerInfo()
        {
            matchingPlayerData[PlayerStartBool.myPlayerNum ]
            = new MatchingPlayerData { playerID = PlayerStartBool.myPlayerNum, 
                                       playerName = PhotonNetwork.PlayerList[PlayerStartBool.myPlayerNum].NickName,
                                       characterID = CharctorSelect.setCharacter };

            singletonData.GetPlayerData(matchingPlayerData);
        }

        public void StartFlgChange()
        {
            playerStartBool.gameStartBool[PlayerStartBool.myPlayerNum] = true;
        }
    }
}

