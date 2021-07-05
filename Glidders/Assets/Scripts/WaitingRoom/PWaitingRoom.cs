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
        [SerializeField] Text nameText;
        [SerializeField] Text hostDisp;

        [SerializeField] Text[] playerDisp = new Text[4];

        private int playersNum = 0;

        private bool isCreate;

        const int MAX_PLAYERS = 4;

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); //Photonサーバに接続

            playerDisp[4].text = SingletonDate.SingletonClass.MemberList[0];
        }

        public override void OnConnectedToMaster() //初めに呼ばれる処理
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
                PhotonNetwork.CreateRoom(roomName.text,roomOptions); //ホスト 部屋を作る時の処理        
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("ルーム名を書いてください。");
        }
        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(roomName.text); // ゲスト 部屋を探す時の処理
        }
 
        public override void OnCreatedRoom() //CreateRoomが成功したら呼ばれる
        {
            isCreate = true;
            Debug.Log(SingletonDate.SingletonClass.MemberList[playersNum]);
        }

        public override void OnJoinedRoom() //CreateRoomが成功したら呼ばれる、OnCreatedRoomと同時に処理が実行　//JoinRoomが成功したら呼ばれる
        {
            //string name = PhotonNetwork.PlayerListOthers[hostNum].NickName;
            //ルール選択に移動する処理(ホスト) //キャラクター選択に移動する処理(ゲスト)
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


        public override void OnPlayerEnteredRoom(Player newPlayer) //他の人(ゲスト)が入ったら呼ばれる
        {
           
        }
    }
}

