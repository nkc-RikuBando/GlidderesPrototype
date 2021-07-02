using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Glidders
{
    public class PWaitingRoom : MonoBehaviourPunCallbacks //MonoBehaviourPunCallbacks = Photonのすべてのインタフェースを実装
    {
        [SerializeField] Text roomName;
        private int PlayersNum = 0;
        private bool isCreate;

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); //Photonサーバに接続

            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
        }

        public override void OnConnectedToMaster() //初めに呼ばれる処理
        {
            Debug.Log(PhotonNetwork.IsConnected);
        }

        public void CrateRoom()
        {
            //if (isCreate) return;
            Debug.Log("ok");
            PhotonNetwork.CreateRoom(roomName.text); //ホスト 部屋を作る時の処理
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(roomName.text); // ゲスト 部屋を探す時の処理
        }
 
        public override void OnCreatedRoom() //CreateRoomが成功したら呼ばれる
        {
            isCreate = true;
        }

        public override void OnJoinedRoom() //CreateRoomが成功したら呼ばれる、OnCreatedRoomと同時に処理が実行　//JoinRoomが成功したら呼ばれる
        {
            //ルール選択に移動する処理(ホスト) //キャラクター選択に移動する処理(ゲスト)
            SceneManager.LoadScene("RuleAndCharacterSelectScene");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) //他の人(ゲスト)が入ったら呼ばれる
        {
            PlayersNum = ++PlayersNum;
            Debug.Log(PlayersNum);
        }
    }
}

