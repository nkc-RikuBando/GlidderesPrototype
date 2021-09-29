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

        public void Act_CreateRoom(string RoomName) //部屋を作るメソッド　ルーム名が送られてくる
        {
            var roomOptions = new RoomOptions(); //ルームにルールを付ける準備
            roomOptions.MaxPlayers = 4; //部屋の最大人数は4人

            if (!(PublicStaticBool.isCreate)) return; //trueじゃなければReturn
            PhotonNetwork.CreateRoom(RoomName, roomOptions); //ホスト 部屋を作ります
        }

        public void Act_JoinRoom(string RoomName) //部屋を探すメソッド　ルーム名が送られてくる
        {
            punPlayer = PhotonNetwork.PlayerList;

            Debug.Log(RoomName + "1");
            if (!(PublicStaticBool.isJoin)) return; //trueじゃなければReturn
            Debug.Log(RoomName);
            PhotonNetwork.JoinRoom(RoomName); //ゲスト 部屋を探す時
        }

        public override void OnConnected()
        {
            
        }

        public override void OnConnectedToMaster() //Photonに接続したときに呼ばれる
        {
            Debug.Log("サーバーへ接続しました");
        }

        public override void OnCreatedRoom() //部屋を作ったときに呼ばれる
        {
            SingletonData.hostNum = PhotonNetwork.CurrentRoom.PlayerCount - 1; //作った人はホストだから0を渡す
            ruleAndCharacterSelectSceneObj = PhotonNetwork.Instantiate("RuleAndCharacterSelectSceneObj", Vector3.zero, Quaternion.identity);
        }

        public override void OnJoinedRoom() //部屋に入室したときに呼ばれる
        {
            PlayerStartBool.myPlayerNum = PhotonNetwork.CurrentRoom.PlayerCount - 1; //自分が何番目に入ったかを渡す(0から)
            Debug.Log("部屋人数 = " + PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.IsMessageQueueRunning = false;
            uiCanvasObj.SetActive(false);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("現在のプレイヤーの番号　" + PlayerStartBool.myPlayerNum);
            Debug.Log("現在の部屋人数　" + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        private void Update()
        {
            if ( PhotonNetwork.NetworkClientState.ToString() == "ConnectingToMasterserver" ) 
            {
                Debug.Log("サーバー接続中");
            }
            if (PhotonNetwork.NetworkClientState.ToString() == "Authenticating")
            {
                Debug.Log("認証中");
            }
            if (PhotonNetwork.NetworkClientState.ToString() == "Joining")
            {
                Debug.Log("参加中");
            }
        }

    }
}

