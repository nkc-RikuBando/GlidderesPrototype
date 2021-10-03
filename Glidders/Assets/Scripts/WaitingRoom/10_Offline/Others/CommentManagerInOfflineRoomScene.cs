using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.UI;
using UnityEngine.SceneManagement;

namespace Glidders
{
    /// <summary>
    /// コメントを表示するために作成しました by matsumoto
    /// </summary>
    public class CommentManagerInOfflineRoomScene : MonoBehaviour
    {
        // コメント表示スクリプト
        CommentOutput commentOutput;

        // コメントを表示するTextUI
        public Text commentTextUI;

        // Start is called before the first frame update
        void Start()
        {
            commentOutput = GameObject.Find("CommentOutputSystem").GetComponent<CommentOutput>();
            commentOutput.SetTextUI(commentTextUI);
            commentOutput.SetTableActive("ルーム汎用１", true);
            commentOutput.StartComment();
            SceneManager.sceneUnloaded += OnUnloadedScene;
        }

        void OnUnloadedScene(Scene scene)
        {
            commentOutput.SetTableActive("ルーム汎用１", false);
            commentOutput.StopComment();
        }
    }
}
