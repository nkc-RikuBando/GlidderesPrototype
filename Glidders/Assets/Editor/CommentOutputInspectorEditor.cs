using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.UI;

public class CommentOutputInspectorEditor : Editor
{
    bool initializeFlg = true;
    int index = 0;

    public override void OnInspectorGUI()
    {
        CommentOutput commentOutput = target as CommentOutput;

        if (initializeFlg)
        {
            commentOutput.tableName = new List<string>();
            commentOutput.commentTable = new List<string[]>();
            commentOutput.commentRate = new List<float>();
            commentOutput.tableActive = new List<bool>();
        }

        for (int i = 0; i < index; ++i)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                commentOutput.tableName[i] = EditorGUILayout.TextField("テーブル名", commentOutput.tableName[i]);
                using (new EditorGUILayout.HorizontalScope())
                {
                    commentOutput.commentRate[i] = EditorGUILayout.FloatField("出現比率", commentOutput.commentRate[i]);
                    commentOutput.tableActive[i] = EditorGUILayout.Toggle("有効化", commentOutput.tableActive[i]);
                }
                // 作業途中
            }
        }
    }
}
