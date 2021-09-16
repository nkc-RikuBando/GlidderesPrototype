using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Glidders
{
    public class PlayerStartBool : MonoBehaviourPunCallbacks
    {
        PhotonView view;
        public bool[] gameStartBool = new bool[Rule.maxPlayerCount];

        public static int myPlayerNum;
        public static string battleStageField;
        private int okPlayerCount = 0;
        bool isStart = false;

        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();

            view.RPC(nameof(PlayerStartBoolCount), RpcTarget.AllBufferedViaServer);//RPCメソッドを呼ぶ　PlayerStartBoolCount

            Debug.Log("myPlayerNum = " + myPlayerNum);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        [PunRPC]
        public void PlayerStartBoolCount()
        {
            gameStartBool[myPlayerNum] = isStart; //入ってきたら自分の番号の配列の場所にfalseをいれる
        }

        public void CallMethod(int myPlayerNum) //他のスクリプトからRPCメソッドを呼ぶためのメソッド
        {
            view.RPC(nameof(StartConf), RpcTarget.All,(myPlayerNum)); //RPCメソッドを呼ぶ　StartConf
        }

        [PunRPC]
        public void StartConf(int cheakPlayerNum) //試合開始できるか判断するメソッド
        {
            gameStartBool[cheakPlayerNum] = true; //自分の番号の配列をtrueにする
            //Debug.Log("PhotonNetwork.PlayerList.Length = " + PhotonNetwork.PlayerList.Length);
            for(int i = 0; i <= PhotonNetwork.PlayerList.Length -1;i++) //for文で全員trueか判断する
            {
                //Debug.Log("gameStartBool[" + i + "]" + "=" + gameStartBool[i]);
                if (gameStartBool[i] == true) //判断した人がtrueだったら
                {
                    //Debug.Log("クリア");
                    ++okPlayerCount; //countが増加する
                    //Debug.Log(okPlayerCount);
                }
            }
            Debug.Log(PhotonNetwork.PlayerList.Length);

            if (PhotonNetwork.PlayerList.Length >= 2) //メンバーが2人以上いる必要があるから1人ならシーン移動できない
            {
                if (okPlayerCount == PhotonNetwork.PlayerList.Length) //countがプレイ人数と一致したら
                {
                    //SingletonData.SetMatchingPlayerData();
                    SceneManager.LoadScene(battleStageField); //シーン移動
                }
                else okPlayerCount = 0; //しなかったらカウントが0になる
            }
            else okPlayerCount = 0;
            

        }
    }
}

