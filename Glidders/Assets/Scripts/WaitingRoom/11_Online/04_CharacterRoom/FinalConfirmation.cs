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

        private int playerID;
        private string playerName;
        private int characterID;

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

            playerStartBool = GameObject.Find("GameStartFlg").GetComponent<PlayerStartBool>();
            singletonData = GameObject.Find("MatchDataSingleton").GetComponent<SingletonData>();
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


        private void CommandInputYES() //準備できた
        {
            commandInput.SetInputNumber(0);
            //view.RPC(nameof(SetPlayerInfo),RpcTarget.AllBufferedViaServer);
            SetPlayerInfo();
            playerStartBool.CallMethod(PlayerStartBool.myPlayerNum); //PlayerStartBoolのCallMethodを呼ぶ
            //view.RPC(nameof())
            finalPanel.SetActive(false);
        }

        private void CommandInputNO() //準備できてない
        {
            commandInput.SetInputNumber(0);

            characterPanel.SetActive(true);
            finalPanel.SetActive(false);
        }

        [PunRPC]
        public void SetPlayerInfo() //プレイヤー情報をシングルトンに送るメソッド
        {
            Debug.Log("myPlayerNum = " + PlayerStartBool.myPlayerNum);
            //matchingPlayerData[PlayerStartBool.myPlayerNum] = new MatchingPlayerData
            //{
            //    playerID = PlayerStartBool.myPlayerNum, //playerID
            //    playerName = PhotonNetwork.PlayerList[PlayerStartBool.myPlayerNum].NickName, //playerName
            //    characterID = CharacterSelect.setCharacter //characterID
            //};

            playerID = PlayerStartBool.myPlayerNum; //playerID
            playerName = PhotonNetwork.PlayerList[PlayerStartBool.myPlayerNum].NickName; //playerName
            characterID = characterID = CharacterSelect.setCharacter; //characterID

            singletonData.CallMethod(playerID, playerName, characterID);
            //singletonData.GetPlayerData(matchingPlayerData[PlayerStartBool.myPlayerNum]); //配列をシングルトンに送る
        }

        public void CharcterFlg()
        {

        }

        [PunRPC]
        public void StartFlgChange()
        {
            playerStartBool.gameStartBool[PlayerStartBool.myPlayerNum] = true;
            //Debug.Log("gameStartBool[" + PlayerStartBool.myPlayerNum + "]" + playerStartBool.gameStartBool[PlayerStartBool.myPlayerNum]);
        }
    }
}

