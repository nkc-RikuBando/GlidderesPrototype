using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartJoinButton : MonoBehaviour
{
    [SerializeField] GameObject startRoomPanel;
    [SerializeField] GameObject joinRoomPanel;
    [SerializeField] GameObject commonPanel;

    public void OnClick()
    {
        if (!(PublicStaticBool.isName)) return;
        joinRoomPanel.SetActive(true);
        commonPanel.SetActive(true);
        startRoomPanel.SetActive(false);
    }
}
