using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Glidders;

public class SelectOn_Offline : MonoBehaviour
{
    [SerializeField] private CommandInput commandInput;

    private delegate void CommandInputFunction();
    private CommandInputFunction[] commandInputFunctionTable;

    GameObject singletonObj;

    private enum SelectButton
    {
        BUTTON_NOT_INPUT,
        BUTTON_INPUT_1,
        BUTTON_INPUT_2,

        BUTTON_NUMBER
    }
    // Start is called before the first frame update
    void Start()
    {
        commandInputFunctionTable = new CommandInputFunction[(int)SelectButton.BUTTON_NUMBER];
        commandInputFunctionTable[(int)SelectButton.BUTTON_NOT_INPUT] = ButtonNotInput;
        commandInputFunctionTable[(int)SelectButton.BUTTON_INPUT_1] = OnlineButtonInput;
        commandInputFunctionTable[(int)SelectButton.BUTTON_INPUT_2] = OfflineButtonInput;


        //Menuに戻った時にシングルトンを消す処理
        if(singletonObj = GameObject.Find("MatchDataSingleton"))
        {
            Debug.Log("シングルトン削除");
            Destroy(singletonObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        commandInputFunctionTable[commandInput.GetInputNumber()]();
    }

    private void ButtonNotInput()
    {
        int selectNumber = commandInput.GetSelectNumber();
        selectNumber = Mathf.Clamp(selectNumber, (int)SelectButton.BUTTON_NOT_INPUT, (int)SelectButton.BUTTON_INPUT_2);
    }


    private void OnlineButtonInput()
    {
        commandInput.SetInputNumber(0);
        SceneManager.LoadScene("OnlineRoomScene");
        //FadeManager.Instance.LoadScene("OnlineRoomScene", 1.0f);
    }

    private void OfflineButtonInput()
    {
        commandInput.SetInputNumber(0);

        //SceneManager.LoadScene("OfflineRoomScene");
        FadeManager.Instance.LoadScene("OfflineRoomScene", 0.5f);
    }
}
