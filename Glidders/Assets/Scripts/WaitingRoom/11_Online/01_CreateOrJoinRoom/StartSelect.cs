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

            if (!(nameText.text != "")) return; //もし名前が空白だったらReturn
            PhotonNetwork.NickName = nameText.text; //NickNameに名前を保存する
            if (roomNameText.text != "") //ルーム名が書いてあるかどうか
            {
                PublicStaticBool.isCreate = true; //trueにして準備
                createjoinSc.Act_CreateRoom(roomNameText.text); //ルーム名を渡す PhotonCreateOrJoin.csへ
            }
            else
            {
                PublicStaticBool.isCreate = false; //falseで先へ進めない
            }    
        }

        private void CommandInput2()//部屋名を探して決定するボタン
        {
            commandInput.SetInputNumber(0);

            if (!(nameText.text != "")) return; //もし名前が空白だったらReturn
            PhotonNetwork.NickName = nameText.text; //NickNameに名前を保存する
            if (roomNameText.text != "") //ルーム名が書いてあるかどうか
            {
                PublicStaticBool.isJoin = true; //trueにして準備
                createjoinSc.Act_JoinRoom(roomNameText.text); //ルーム名を渡す PhotonCreateOrJoin.csへ
            }
            else
            {
                PublicStaticBool.isJoin = false; //falseで先へ進めない
            }
        }

        private void CommandInput3() //1つ前に戻るボタン
        {
            commandInput.SetInputNumber(0);

            startRoomPanel.SetActive(true);
        }
    }
}

