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
        private int PlayersNum = 0;
        private bool isCreate;

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); //Photon�T�[�o�ɐڑ�

            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
        }

        public override void OnConnectedToMaster() //���߂ɌĂ΂�鏈��
        {
            Debug.Log(PhotonNetwork.IsConnected);
        }

        public void CrateRoom()
        {
            //if (isCreate) return;
            Debug.Log("ok");
            PhotonNetwork.CreateRoom(roomName.text); //�z�X�g ��������鎞�̏���
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(roomName.text); // �Q�X�g ������T�����̏���
        }
 
        public override void OnCreatedRoom() //CreateRoom������������Ă΂��
        {
            isCreate = true;
        }

        public override void OnJoinedRoom() //CreateRoom������������Ă΂��AOnCreatedRoom�Ɠ����ɏ��������s�@//JoinRoom������������Ă΂��
        {
            //���[���I���Ɉړ����鏈��(�z�X�g) //�L�����N�^�[�I���Ɉړ����鏈��(�Q�X�g)
            SceneManager.LoadScene("RuleAndCharacterSelectScene");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) //���̐l(�Q�X�g)����������Ă΂��
        {
            PlayersNum = ++PlayersNum;
            Debug.Log(PlayersNum);
        }
    }
}

