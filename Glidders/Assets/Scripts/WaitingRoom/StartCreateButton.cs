using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCreateButton : MonoBehaviour
{
    [SerializeField] GameObject startRoomPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] GameObject commonPanel;
    
    public void OnClick()
    {
        if (!(PublicStaticBool.isName)) return;
        createRoomPanel.SetActive(true);
        commonPanel.SetActive(true);
        startRoomPanel.SetActive(false);
    }
}
