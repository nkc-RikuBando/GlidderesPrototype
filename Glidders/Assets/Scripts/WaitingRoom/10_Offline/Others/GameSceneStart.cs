using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneStart : MonoBehaviour
{
    public bool isStart { get; set; } = false;

    string stageSceneName;

    private void Update()
    {
        StartGame();
    }

    public void StartGame()
    {
        if (!isStart) return;
        FadeManager.Instance.LoadScene(stageSceneName,0.5f);
        isStart = false;
    }

    public void GetStageName(string SceneName)
    {
        stageSceneName = SceneName;
    }
}
