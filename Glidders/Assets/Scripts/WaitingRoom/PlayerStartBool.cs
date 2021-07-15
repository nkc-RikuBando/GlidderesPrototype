using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class PlayerStartBool : MonoBehaviourPunCallbacks
    {
        PhotonView view;
        bool[] playerStartBool = new bool[Rule.maxPlayerCount];

        public static int myPlayerNum = 0;
        bool isStart = false;


        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();

            myPlayerNum = PhotonNetwork.PlayerList.Length - 1;
            view.RPC(nameof(PlayerStartBoolCount), RpcTarget.All);

            Debug.Log(PhotonNetwork.PlayerList.Length -1 + "=" + playerStartBool[PhotonNetwork.PlayerList.Length - 1]);
            Debug.Log("myPlayerNum = " + myPlayerNum);
        }

        // Update is called once per frame
        void Update()
        {

        }

        [PunRPC]
        public void PlayerStartBoolCount()
        {
            playerStartBool[PhotonNetwork.PlayerList.Length - 1] = isStart;
            
        }

    }
}

