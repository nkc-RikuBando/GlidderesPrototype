using Glidders;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateButton : MonoBehaviourPunCallbacks
{
    [SerializeField] Text roomNameText;
    private PhotonCreateOrJoin createjoinSc;

    private void Start()
    {
        createjoinSc = GameObject.Find("PhotonManager").GetComponent<PhotonCreateOrJoin>();
    }

    public void OnClick()
    {
        if(roomNameText.text != "")
        {
            PublicStaticBool.isCreate = true;
            createjoinSc.Act_CreateRoom(roomNameText.text);
        }
        else
        {
            PublicStaticBool.isCreate = false;
        }
    }
}
