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
        [SerializeField] GameObject characterPanel;

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

            singletonData = GameObject.Find("MatchDataSingleton(Clone)").GetComponent<SingletonData>();
            playerStartBool = GameObject.Find("GameStartFlg").GetComponent<PlayerStartBool>();
            view = GetComponent<PhotonView>();

            characterPanel.SetActive(false);
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


        private void CommandInputYES() //�����ł���
        {
            commandInput.SetInputNumber(0);

            Debug.Log("GO");
            SetPlayerInfo();
            playerStartBool.CallMethod(PlayerStartBool.myPlayerNum); //PlayerStartBool��CallMethod���Ă�
            finalPanel.SetActive(false);
        }

        private void CommandInputNO() //�����ł��ĂȂ�
        {
            commandInput.SetInputNumber(0);

            characterPanel.SetActive(true);
            finalPanel.SetActive(false);
        }

        public void SetPlayerInfo() //�v���C���[�����V���O���g���ɑ��郁�\�b�h
        {
            matchingPlayerData[PlayerStartBool.myPlayerNum] //�z��ɂ����
            = new MatchingPlayerData { playerID = PlayerStartBool.myPlayerNum, //playerID
                                       playerName = PhotonNetwork.PlayerList[PlayerStartBool.myPlayerNum].NickName, //playerName
                                       characterID = CharacterSelect.setCharacter}; //characterID

            singletonData.GetPlayerData(matchingPlayerData[PlayerStartBool.myPlayerNum]); //�z����V���O���g���ɑ���
        }






        [PunRPC]
        public void StartFlgChange()
        {
            playerStartBool.gameStartBool[PlayerStartBool.myPlayerNum] = true;
            Debug.Log("gameStartBool[" + PlayerStartBool.myPlayerNum + "]" + playerStartBool.gameStartBool[PlayerStartBool.myPlayerNum]);
        }
    }
}

