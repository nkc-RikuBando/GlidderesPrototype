using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    bool isClick;
    [SerializeField] string sceneName; 
    // Start is called before the first frame update
    void Start()
    {
        isClick = true;
    }

    // Update is called once per frame
    void Update()
    {
        StartGameMenu();
    }

    public void StartGameMenu()
    {
        if (!isClick) return;

        if(Input.anyKeyDown)
        {
            isClick = false;
            FadeManager.Instance.LoadScene(sceneName, 1.0f);
        }

        if(Input.GetMouseButtonDown(0))
        {
            isClick = false;
            FadeManager.Instance.LoadScene(sceneName, 1.0f);
        }
    }
}
