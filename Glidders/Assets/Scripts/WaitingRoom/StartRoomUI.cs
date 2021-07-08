using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoomUI : MonoBehaviour
{
    [SerializeField] GameObject startRoomPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] GameObject joinRoomPanel;
    [SerializeField] GameObject commonPanel;

    // Start is called before the first frame update
    void Start()
    {
        startRoomPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
        commonPanel.SetActive(false);
    }

    
}
