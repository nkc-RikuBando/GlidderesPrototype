using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.UI;
using UnityEngine.SceneManagement;

namespace Glidders
{
    /// <summary>
    /// �R�����g��\�����邽�߂ɍ쐬���܂��� by matsumoto
    /// </summary>
    public class CommentManagerInOfflineRoomScene : MonoBehaviour
    {
        // �R�����g�\���X�N���v�g
        CommentOutput commentOutput;

        // �R�����g��\������TextUI
        public Text commentTextUI;

        // Start is called before the first frame update
        void Start()
        {
            commentOutput = GameObject.Find("CommentOutputSystem").GetComponent<CommentOutput>();
            commentOutput.SetTextUI(commentTextUI);
            commentOutput.SetTableActive("���[���ėp�P", true);
            commentOutput.StartComment();
            SceneManager.sceneUnloaded += OnUnloadedScene;
        }

        void OnUnloadedScene(Scene scene)
        {
            commentOutput.SetTableActive("���[���ėp�P", false);
            commentOutput.StopComment();
        }
    }
}
