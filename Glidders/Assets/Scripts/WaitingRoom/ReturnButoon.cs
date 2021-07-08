using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnButoon : MonoBehaviour
{
    [SerializeField] GameObject startRoomPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] GameObject joinRoomPanel;
    [SerializeField] GameObject commonPanel;

    public void OnClick()
    {
        startRoomPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
        commonPanel.SetActive(false);
    }
}
