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
        public static string battleStageField;
        private int okPlayerCount = 0;
        bool isStart = false;


        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();

            view.RPC(nameof(PlayerStartBoolCount), RpcTarget.AllBufferedViaServer);

            Debug.Log("myPlayerNum = " + myPlayerNum);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        [PunRPC]
        public void PlayerStartBoolCount()
        {
            gameStartBool[myPlayerNum] = isStart; 
        }

        public void CallMethod(int myPlayerNum)
        {
            view.RPC(nameof(StartConf), RpcTarget.All,(myPlayerNum));
        }

        [PunRPC]
        public void StartConf(int cheakPlayerNum)
        {
            Debug.Log(myPlayerNum);
            gameStartBool[cheakPlayerNum] = true;
            Debug.Log("PhotonNetwork.PlayerList.Length = " + PhotonNetwork.PlayerList.Length);
            for(int i = 0; i <= PhotonNetwork.PlayerList.Length -1;i++)
            {
                Debug.Log("gameStartBool[" + i + "]" + "=" + gameStartBool[i]);
                if (gameStartBool[i] == true)
                {
                    Debug.Log("ƒNƒŠƒA");
                    ++okPlayerCount;
                    Debug.Log(okPlayerCount);
                }
            }
            Debug.Log(PhotonNetwork.PlayerList.Length);

            if (okPlayerCount == PhotonNetwork.PlayerList.Length)
            {
                SceneManager.LoadScene(battleStageField);
            }
            else
            {
                okPlayerCount = 0;
            }
        }
    }
}

