using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Glidders
{
    public class PCreateRoom : MonoBehaviourPunCallbacks //MonoBehaviourPunCallbacks = Photon�̂��ׂẴC���^�t�F�[�X������
    {
        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); //Photon�T�[�o�ɐڑ�
        }

        public override void OnConnectedToMaster()
        {
            //PhotonNetwork.JoinOrCreateRoom();
        }
    }
}

