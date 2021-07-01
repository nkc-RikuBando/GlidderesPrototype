using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Glidders
{
    public class PCreateRoom : MonoBehaviourPunCallbacks //MonoBehaviourPunCallbacks = Photonのすべてのインタフェースを実装
    {
        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); //Photonサーバに接続
        }

        public override void OnConnectedToMaster()
        {
            //PhotonNetwork.JoinOrCreateRoom();
        }
    }
}

