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
        [SerializeField] GameObject selectSceneObj;
        [SerializeField] GameObject uiCanvasObj;
        [SerializeField] GameObject ruleObj;
        GameObject ruleAndCharacterSelectSceneObj;
        
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public void Act_CreateRoom(string RoomName) //��������郁�\�b�h�@���[�����������Ă���
        {
            var roomOptions = new RoomOptions(); //���[���Ƀ��[����t���鏀��
            roomOptions.MaxPlayers = 4; //�����̍ő�l����4�l

            if (!(PublicStaticBool.isCreate)) return; //true����Ȃ����Return
            PhotonNetwork.CreateRoom(RoomName, roomOptions); //�z�X�g ���������܂�
        }

        public void Act_JoinRoom(string RoomName) //������T�����\�b�h�@���[�����������Ă���
        {
            punPlayer = PhotonNetwork.PlayerList;

            Debug.Log(RoomName + "1");
            if (!(PublicStaticBool.isJoin)) return; //true����Ȃ����Return
            Debug.Log(RoomName);
            PhotonNetwork.JoinRoom(RoomName); //�Q�X�g ������T����
        }

        public override void OnConnected()
        {
            
        }

        public override void OnConnectedToMaster() //Photon�ɐڑ������Ƃ��ɌĂ΂��
        {
            Debug.Log("�T�[�o�[�֐ڑ����܂���");
        }

        public override void OnCreatedRoom() //������������Ƃ��ɌĂ΂��
        {
            SingletonData.hostNum = PhotonNetwork.CurrentRoom.PlayerCount - 1; //������l�̓z�X�g������0��n��
            //PhotonNetwork.Instantiate("MatchDataSingleton", Vector3.zero, Quaternion.identity); //�V���O���g������
            ruleAndCharacterSelectSceneObj = PhotonNetwork.Instantiate("RuleAndCharacterSelectSceneObj", Vector3.zero, Quaternion.identity);
            //matchDataSingleton.name = "MatchDataSingleton";
        }

        public override void OnJoinedRoom() //�����ɓ��������Ƃ��ɌĂ΂��
        {
            PlayerStartBool.myPlayerNum = PhotonNetwork.CurrentRoom.PlayerCount - 1; //���������Ԗڂɓ���������n��(0����)
            Debug.Log("�����l�� = " + PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.IsMessageQueueRunning = false;
            uiCanvasObj.SetActive(false);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("���݂̃v���C���[�̔ԍ��@" + PlayerStartBool.myPlayerNum);
            Debug.Log("���݂̕����l���@" + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        private void Update()
        {
            if ( PhotonNetwork.NetworkClientState.ToString() == "ConnectingToMasterserver" ) 
            {
                Debug.Log("�T�[�o�[�ڑ���");
            }
            if (PhotonNetwork.NetworkClientState.ToString() == "Authenticating")
            {
                Debug.Log("�F�ؒ�");
            }
            if (PhotonNetwork.NetworkClientState.ToString() == "Joining")
            {
                Debug.Log("�Q����");
            }
        }

    }
}
