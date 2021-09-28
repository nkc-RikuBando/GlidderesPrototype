using Glidders;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinButton : MonoBehaviourPunCallbacks
{
    [SerializeField] Text roomNameText;
    private PhotonCreateOrJoin createorjoinSc;

    private void Start()
    {
        createorjoinSc = GameObject.Find("PhotonManager").GetComponent<PhotonCreateOrJoin>();
    }

    public void OnClick()
    {
        if (roomNameText.text != "")
        {
            PublicStaticBool.isJoin = true;
            createorjoinSc.Act_JoinRoom(roomNameText.text);
        }
        else
        {
            PublicStaticBool.isJoin = false;
        }
    }
}
