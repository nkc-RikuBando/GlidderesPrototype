using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class StartSelect : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CommandInput commandInput;
        
        [SerializeField] GameObject startRoomPanel;

        [SerializeField] Text nameText;
        [SerializeField] Text roomNameText;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private PhotonCreateOrJoin createjoinSc;

        private enum SelectCommand
        {
            COMMAND_NOT_INPUT,
            COMMAND_INPUT_1,
            COMMAND_INPUT_2,
            COMMAND_INPUT_3,
          
            COMMAND_NUMBER
        }

        // Start is called before the first frame update
        void Start()
        {
            commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
            commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
            commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_3] = CommandInput3;

            createjoinSc = GameObject.Find("PhotonManager").GetComponent<PhotonCreateOrJoin>();

            startRoomPanel.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_3);
        }

        private void CommandInput1() //部屋名を入力して決定するボタン
        {
            commandInput.SetInputNumber(0);

            if (!(nameText.text != "")) return;
            PhotonNetwork.NickName = nameText.text;
            if (roomNameText.text != "")
            {
                PublicStaticBool.isCreate = true;
                createjoinSc.Act_CreateRoom(roomNameText.text);
            }
            else
            {
                PublicStaticBool.isCreate = false;
            }    
        }

        private void CommandInput2()//部屋名を探して決定するボタン
        {
            commandInput.SetInputNumber(0);

            if (!(nameText.text != "")) return;
            PhotonNetwork.NickName = nameText.text;
            if (roomNameText.text != "")
            {
                PublicStaticBool.isJoin = true;
                createjoinSc.Act_JoinRoom(roomNameText.text);
            }
            else
            {
                PublicStaticBool.isJoin = false;
            }
        }

        private void CommandInput3() //1つ前に戻るボタン
        {
            commandInput.SetInputNumber(0);

            startRoomPanel.SetActive(true);
        }
    }
}

