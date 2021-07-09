using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameButton : MonoBehaviourPunCallbacks
{
    [SerializeField] Text nameText;
    // Start is called before the first frame update
    
    public void OnClick()
    {
        if(nameText.text != "")
        {
            PublicStaticBool.isName = true;
            PhotonNetwork.NickName = nameText.text;
        }
        else
        {
            PublicStaticBool.isName = false;
        }
    }
}
