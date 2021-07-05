using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Glidders
{
    public class PWaitingRoom : MonoBehaviourPunCallbacks //MonoBehaviourPunCallbacks = Photon�̂��ׂẴC���^�t�F�[�X������
    {
        [SerializeField] Text roomName;
        [SerializeField] Text nameText;
        [SerializeField] Text hostDisp;

        [SerializeField] Text[] playerDisp = new Text[4];

        private int playersNum = 0;

        private bool isCreate;

        const int MAX_PLAYERS = 4;

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); //Photon�T�[�o�ɐڑ�

            playerDisp[4].text = SingletonDate.SingletonClass.MemberList[0];
        }

        public override void OnConnectedToMaster() //���߂ɌĂ΂�鏈��
        {
            Debug.Log(PhotonNetwork.IsConnected);
        }

        public void CrateRoom()
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;

            //if (isCreate) return;
            if ((roomName.text == null)) return;
                Debug.Log("ok");
                PhotonNetwork.CreateRoom(roomName.text,roomOptions); //�z�X�g ��������鎞�̏���        
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("���[�����������Ă��������B");
        }
        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(roomName.text); // �Q�X�g ������T�����̏���
        }
 
        public override void OnCreatedRoom() //CreateRoom������������Ă΂��
        {
            isCreate = true;
            Debug.Log(SingletonDate.SingletonClass.MemberList[playersNum]);
        }

        public override void OnJoinedRoom() //CreateRoom������������Ă΂��AOnCreatedRoom�Ɠ����ɏ��������s�@//JoinRoom������������Ă΂��
        {
            //string name = PhotonNetwork.PlayerListOthers[hostNum].NickName;
            //���[���I���Ɉړ����鏈��(�z�X�g) //�L�����N�^�[�I���Ɉړ����鏈��(�Q�X�g)
            SingletonDate.SingletonClass.MemberList.Add(nameText.text);
            SceneManager.LoadScene("RuleAndCharacterSelectScene");
            Debug.Log(SingletonDate.SingletonClass.MemberList[playersNum]);
            NumberOfPlayers();
        }

        public void NameTextSubstitution()
        {
            for (int i = 0;i <= MAX_PLAYERS;i++)
            {
                if(playerDisp[i].text == null)
                {
                    playerDisp[i].text = SingletonDate.SingletonClass.MemberList[playersNum];
                    break;
                }
                
            }
        }

        public void NumberOfPlayers()
        {
            if(playersNum > 3)
            playersNum++;
        }

        public void MyNameInput()
        {
            PhotonNetwork.NickName = nameText.text;
            hostDisp.text = nameText.text;
        }


        public override void OnPlayerEnteredRoom(Player newPlayer) //���̐l(�Q�X�g)����������Ă΂��
        {
           
        }
    }
}

