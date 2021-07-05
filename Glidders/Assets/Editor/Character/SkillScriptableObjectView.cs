using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;

[CustomEditor(typeof(SkillScriptableObject))]
public class SkillScriptableObjectView : Editor
{
    // 攻撃範囲の表示に使用
    const string DOT = "■";
    const string NONE = "　";
    const string PLAYER_FALSE = "△";
    const string PLAYER_TRUE = "▲";
    const int DOT_WIDTH = 12;
    const int DOT_HEIGHT = 10;

    public override void OnInspectorGUI()
    {
        SkillScriptableObject skillData = target as SkillScriptableObject;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("スキル名称");
        EditorGUILayout.LabelField(skillData.skillName, GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("消費エネルギー");
        EditorGUILayout.LabelField(skillData.energy.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ダメージ");
        EditorGUILayout.LabelField(skillData.damage.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("優先度");
        EditorGUILayout.LabelField(skillData.priority.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("威力");
        EditorGUILayout.LabelField(skillData.power.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        int listIndex = 0;
        /*※
        int rowMin = int.MaxValue, rowMax = int.MinValue;
        int columnMin = int.MaxValue, columnMax = int.MinValue;*/
        int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;
        /*※
        foreach (FieldIndexOffset offset in skillData.gridList)
        {
            if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
            if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
            if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
            if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
        }*/
        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = rowMin; i <= rowMax; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = columnMin; j <= columnMax; j++)
            {
                if (skillData.selectRangeArray[i, j])
                {
                    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }
                else
                {
                    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }

                /*※  
                if (i == skillData.gridList[listIndex].rowOffset && j == skillData.gridList[listIndex].columnOffset)
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    listIndex++;
                }
                else
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }*/
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = rowMin; i <= rowMax; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = columnMin; j <= columnMax; j++)
            {
                if (skillData.attackRangeArray[i, j])
                {
                    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }
                else
                {
                    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }

                /*※  
                if (i == skillData.gridList[listIndex].rowOffset && j == skillData.gridList[listIndex].columnOffset)
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    listIndex++;
                }
                else
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }*/
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}
