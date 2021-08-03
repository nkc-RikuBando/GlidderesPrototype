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

        private void CommandInput1() //����������͂��Č��肷��{�^��
        {
            commandInput.SetInputNumber(0);

            if (!(nameText.text != "")) return; //�������O���󔒂�������Return
            PhotonNetwork.NickName = nameText.text; //NickName�ɖ��O��ۑ�����
            if (roomNameText.text != "") //���[�����������Ă��邩�ǂ���
            {
                PublicStaticBool.isCreate = true; //true�ɂ��ď���
                createjoinSc.Act_CreateRoom(roomNameText.text); //���[������n�� PhotonCreateOrJoin.cs��
            }
            else
            {
                PublicStaticBool.isCreate = false; //false�Ő�֐i�߂Ȃ�
            }    
        }

        private void CommandInput2()//��������T���Č��肷��{�^��
        {
            commandInput.SetInputNumber(0);

            if (!(nameText.text != "")) return; //�������O���󔒂�������Return
            PhotonNetwork.NickName = nameText.text; //NickName�ɖ��O��ۑ�����
            if (roomNameText.text != "") //���[�����������Ă��邩�ǂ���
            {
                PublicStaticBool.isJoin = true; //true�ɂ��ď���
                createjoinSc.Act_JoinRoom(roomNameText.text); //���[������n�� PhotonCreateOrJoin.cs��
            }
            else
            {
                PublicStaticBool.isJoin = false; //false�Ő�֐i�߂Ȃ�
            }
        }

        private void CommandInput3() //1�O�ɖ߂�{�^��
        {
            commandInput.SetInputNumber(0);

            startRoomPanel.SetActive(true);
        }
    }
}

