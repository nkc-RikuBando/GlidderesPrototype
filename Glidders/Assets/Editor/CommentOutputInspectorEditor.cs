using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Glidders.UI;

[CustomEditor(typeof(CommentOutput))]
public class CommentOutputInspectorEditor : Editor
{
    bool initializeFlg = true;
    List<string> commentList;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        CommentOutput commentOutput = target as CommentOutput;

        //if (initializeFlg)
        //{
        //    commentOutput.tableName = new List<string>();
        //    commentOutput.commentTable = new List<string[]>();
        //    commentOutput.commentRate = new List<float>();
        //    commentOutput.tableActive = new List<bool>();
        //    foldOuts = new List<bool>();
        //    initializeFlg = false;
        //}
        
        EditorGUILayout.LabelField("a");
        for (int i = 0; i < commentOutput.commentTable.Count; ++i)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    commentOutput.tableName[i] = EditorGUILayout.TextField("テーブル名", commentOutput.tableName[i]);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        commentOutput.commentRate[i] = EditorGUILayout.FloatField("出現比率", commentOutput.commentRate[i]);
                        commentOutput.tableActive[i] = EditorGUILayout.Toggle("有効化", commentOutput.tableActive[i]);
                    }
                    commentOutput.foldOut[i] = EditorGUILayout.Foldout(commentOutput.foldOut[i], "内容");
                    if (commentOutput.foldOut[i])
                    {
                        commentList = commentOutput.commentTable[i];
                        for (int j = 0; j < commentOutput.commentTable[i].Count; ++j)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                commentList[j] = EditorGUILayout.TextField("", commentList[j]);
                                if (GUILayout.Button("-"))
                                    commentList.RemoveAt(i);
                            }
                        }
                        if (GUILayout.Button("+"))
                            commentList.Add("");
                        commentOutput.commentTable[i] = commentList;
                    }

                }
                if (GUILayout.Button("-"))
                {
                    commentOutput.tableName.RemoveAt(i);
                    commentOutput.commentTable.RemoveAt(i);
                    commentOutput.commentRate.RemoveAt(i);
                    commentOutput.tableActive.RemoveAt(i);
                    commentOutput.foldOut.RemoveAt(i);
                }
            }
        }
        if (GUILayout.Button("+"))
        {
            commentOutput.tableName.Add("");
            commentOutput.commentTable.Add(new List<string>());
            commentOutput.commentRate.Add(1);
            commentOutput.tableActive.Add(true);
            commentOutput.foldOut.Add(false);
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
