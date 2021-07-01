using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;

[CustomEditor(typeof(SkillScriptableObject))]
public class SkillScriptableObjectView : Editor
{
    // �U���͈͂̕\���Ɏg�p
    const string DOT = "��";
    const string NONE = "�@";
    const string PLAYER = "��";
    const int DOT_SIZE = 8;

    public override void OnInspectorGUI()
    {
        SkillScriptableObject skillData = target as SkillScriptableObject;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("�X�L������");
        EditorGUILayout.LabelField(skillData.skillName, GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("����G�l���M�[");
        EditorGUILayout.LabelField(skillData.energy.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("�_���[�W");
        EditorGUILayout.LabelField(skillData.damage.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("�D��x");
        EditorGUILayout.LabelField(skillData.priority.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("�З�");
        EditorGUILayout.LabelField(skillData.power.ToString(), GUI.skin.textField);
        EditorGUILayout.EndHorizontal();

        int listIndex = 0;
        int rowMin = int.MaxValue, rowMax = int.MinValue;
        int columnMin = int.MaxValue, columnMax = int.MinValue;
        foreach (FieldIndexOffset offset in skillData.gridList)
        {
            if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
            if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
            if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
            if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
        }
        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = rowMin; i <= rowMax; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = columnMin; j <= columnMax; j++)
            {
                if (i == skillData.gridList[listIndex].rowOffset && j == skillData.gridList[listIndex].columnOffset)
                {
                    EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_SIZE), GUILayout.Height(DOT_SIZE));
                    listIndex++;
                }
                else
                {
                    EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_SIZE), GUILayout.Height(DOT_SIZE));
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}
