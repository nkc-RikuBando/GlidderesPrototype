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
    List<List<string>> commentList = new List<List<string>>();
    //List<string> commentList;

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
        commentOutput.lineCount = EditorGUILayout.IntField("行数", commentOutput.lineCount);
        commentOutput.charCountInLine = EditorGUILayout.IntField("文字数/行", commentOutput.charCountInLine);

        commentList = new List<List<string>>();
        for (int i = 0; i < commentOutput.tableSize.Count; ++i)
            commentList.Add(new List<string>());
        for (int i = 0; i < commentOutput.tableSize.Count; ++i)
        {
            commentList[i] = new List<string>();
            int startIndex = commentOutput.tableSize[i];
            int endIndex;
            if (i < commentOutput.tableSize.Count - 1)
                endIndex = commentOutput.tableSize[i + 1];
            else
                endIndex = commentOutput.commentTable.Count;
            for (int x = startIndex; x < endIndex; ++x)
                commentList[i].Add(commentOutput.commentTable[x]);
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
                        for (int j = 0; j < commentList[i].Count; ++j)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                commentList[i][j] = EditorGUILayout.TextField("", commentList[i][j]);
                                if ((endIndex - startIndex) > 0 && GUILayout.Button("-"))
                                {
                                    commentList[i].RemoveAt(j);
                                    commentOutput.commentTable.RemoveAt(j + startIndex);
                                    --endIndex;
                                    for (int k = i + 1; k < commentOutput.tableSize.Count; ++k)
                                        --commentOutput.tableSize[k];
                                }
                            }
                        }
                        if (GUILayout.Button("+"))
                        {
                            commentList[i].Add("");
                            commentOutput.commentTable.Insert(endIndex, "");
                            //++endIndex;
                            for (int k = i + 1; k < commentOutput.tableSize.Count; ++k)
                                ++commentOutput.tableSize[k];
                            Debug.Log("endindex = " + endIndex);
                        }
                        for(int j = startIndex; j < endIndex; ++j)
                        {
                            Debug.Log("j = " + j);
                            Debug.Log("same = " + (j == j - startIndex));
                            Debug.Log("startIndex = " + startIndex + ", commenTable.Count = " + commentOutput.commentTable.Count + ", list.count = " + commentList[i].Count);
                            commentOutput.commentTable[j] = commentList[i][j - startIndex];
                        }
                    }

                }
                if (commentOutput.tableSize.Count > 1 && GUILayout.Button("-"))
                {
                    commentOutput.tableName.RemoveAt(i);
                    //commentOutput.commentTable.RemoveAt(i);
                    commentOutput.commentRate.RemoveAt(i);
                    commentOutput.tableActive.RemoveAt(i);
                    commentOutput.foldOut.RemoveAt(i);
                    for (int j = startIndex; j < endIndex; ++j)
                    {
                        commentOutput.commentTable.RemoveAt(j);
                    }
                    int removeCommentCount = endIndex - startIndex;
                    for (int j = i + 1; j < commentOutput.tableSize.Count; ++j)
                    {
                        commentOutput.tableSize[j] -= removeCommentCount;
                    }
                    commentOutput.tableSize.RemoveAt(i);
                }
            }
        }
        if (GUILayout.Button("+"))
        {
            commentOutput.tableName.Add("");
            //commentOutput.commentTable.Add();
            commentOutput.tableSize.Add(commentOutput.commentTable.Count);
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
