using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class PhotonCreateOrJoin : MonoBehaviourPunCallbacks
    {
        private Player[] punPlayer = new Player[4];

        public void Act_CreateRoom(string RoomName)
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;

            if (!(PublicStaticBool.isCreate)) return;
            PhotonNetwork.CreateRoom(RoomName,roomOptions); //�z�X�g ��������鎞
        }

        public void Act_JoinRoom(string RoomName)
        {
            punPlayer = PhotonNetwork.PlayerList;

            if (!(PublicStaticBool.isJoin)) return;
            PhotonNetwork.JoinRoom(RoomName); //�Q�X�g ������T����
        }

        public override void OnConnected()
        {
            
        }

        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene("RuleAndCharacterSelectScene");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            
        }
    }
}

