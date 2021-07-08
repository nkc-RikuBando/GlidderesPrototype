using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class PlayerListText : MonoBehaviourPunCallbacks
    {
        [SerializeField] Text[] playerDisp = new Text[4];
        
        PhotonView view;

        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();
            PlayerTextChange();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log(nameof(PlayerTextChange));
            view.RPC(nameof(PlayerTextChange), RpcTarget.All);
        }

        [PunRPC]
        public void PlayerTextChange()
        {
            for (int i = 0; i < Rule.maxPlayerCount - 1; i++)
            {
                if (playerDisp[i].text == "")
                {
                    playerDisp[i].text = PhotonNetwork.PlayerList[i].NickName;
                    break;
                }
            }
        }
    }
}
