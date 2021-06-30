using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Glidders
{
    public class PWaitingRoom : MonoBehaviourPunCallbacks //MonoBehaviourPunCallbacks = Photon�̂��ׂẴC���^�t�F�[�X������
    {
        private int PlayersNum = 0;
        private string roomName;

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); //Photon�T�[�o�ɐڑ�
        }

        public override void OnConnectedToMaster() //���߂ɌĂ΂�鏈��
        {
            PhotonNetwork.CreateRoom(roomName); //�z�X�g ��������鎞�̏���
            PhotonNetwork.JoinRoom(roomName); // �Q�X�g ������T�����̏���
        }

        public override void OnCreatedRoom() //CreateRoom������������Ă΂��
        {
            //���[���I���Ɉړ����鏈��
        }

        public override void OnJoinedRoom() //CreateRoom������������Ă΂��AOnCreatedRoom�Ɠ����ɏ��������s�@JoinRoom������������Ă΂��
        {
            
        }

        public override void OnJoinRoomFailed(short returnCode, string message) //������������Ȃ��Ƃ�
        {
            base.OnJoinRoomFailed(returnCode, message);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) //���̐l(�Q�X�g)����������Ă΂��
        {
            PlayersNum = ++PlayersNum;
        }
    }
}

