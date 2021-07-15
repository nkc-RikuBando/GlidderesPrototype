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

        public static int myPlayerNum = 0;
        public static string battleStageFeild;
        private int okPlayerCount = 0;
        bool isStart = false;


        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();

            myPlayerNum = PhotonNetwork.PlayerList.Length - 1;
            view.RPC(nameof(PlayerStartBoolCount), RpcTarget.All);

            Debug.Log(PhotonNetwork.PlayerList.Length -1 + "=" + gameStartBool[PhotonNetwork.PlayerList.Length - 1]);
            Debug.Log("myPlayerNum = " + myPlayerNum);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        [PunRPC]
        public void PlayerStartBoolCount()
        {
            gameStartBool[PhotonNetwork.PlayerList.Length - 1] = isStart;  
        }

        public void CallMethod()
        {
            view.RPC(nameof(StartConf), RpcTarget.All);
        }

        [PunRPC]
        public void StartConf()
        {
            for(int i = 0; i < PhotonNetwork.PlayerList.Length -1;i++)
            {
                if (!(gameStartBool[i] = true)) return;
                okPlayerCount++;
                Debug.Log(gameStartBool[i]);
            }
            Debug.Log(okPlayerCount);
            Debug.Log(PhotonNetwork.PlayerList.Length - 1);

            if (okPlayerCount == PhotonNetwork.PlayerList.Length - 1)
            {
                SceneManager.LoadScene(battleStageFeild);
            }
            else
            {
                okPlayerCount = 0;
            }
        }
    }
}

