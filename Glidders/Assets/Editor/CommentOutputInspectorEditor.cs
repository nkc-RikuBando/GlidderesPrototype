using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.UI;

public class CommentOutputInspectorEditor : Editor
{
    bool initializeFlg = true;
    int index = 0;
    List<bool> foldOuts;

    public override void OnInspectorGUI()
    {
        CommentOutput commentOutput = target as CommentOutput;

        if (initializeFlg)
        {
            commentOutput.tableName = new List<string>();
            commentOutput.commentTable = new List<string[]>();
            commentOutput.commentRate = new List<float>();
            commentOutput.tableActive = new List<bool>();
            foldOuts = new List<bool>();
            initializeFlg = false;
        }

        for (int i = 0; i < index; ++i)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                commentOutput.tableName[i] = EditorGUILayout.TextField("�e�[�u����", commentOutput.tableName[i]);
                using (new EditorGUILayout.HorizontalScope())
                {
                    commentOutput.commentRate[i] = EditorGUILayout.FloatField("�o���䗦", commentOutput.commentRate[i]);
                    commentOutput.tableActive[i] = EditorGUILayout.Toggle("�L����", commentOutput.tableActive[i]);
                }
                foldOuts[i] = EditorGUILayout.Foldout(foldOuts[i], "���e");
                if (foldOuts[i])
                {
                    for (int j = 0; j < commentOutput.commentTable.Count; ++j)
                        commentOutput.commentTable[i][j] = EditorGUILayout.TextField("", commentOutput.commentTable[i][j]);
                }
                
            }
        }
    }
}
