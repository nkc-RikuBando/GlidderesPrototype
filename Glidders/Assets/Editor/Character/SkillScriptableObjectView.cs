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
    const string PLAYER_FALSE = "��";
    const string PLAYER_TRUE = "��";
    const int DOT_WIDTH = 12;
    const int DOT_HEIGHT = 10;

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

        FieldIndexOffset[] selectArray = skillData.selectFieldIndexOffsetArray;
        
        int arrayIndex = 0;
        int rowMin = int.MaxValue, rowMax = int.MinValue;
        int columnMin = int.MaxValue, columnMax = int.MinValue;
        //��int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

        foreach (FieldIndexOffset offset in selectArray)
        {
            if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
            if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
            if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
            if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
        }
        Debug.Log("skillName=" + skillData.skillName + ", columnMin=" + columnMin + ", columnMax=" + columnMax);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = rowMin; i <= rowMax; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = columnMin; j <= columnMax; j++)
            {
                //
                //if (skillData.selectRangeArray[i, j])
                //{
                //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //}
                //else
                //{
                //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //}

                
                if (i == selectArray[arrayIndex].rowOffset && j == selectArray[arrayIndex].columnOffset)
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    arrayIndex++;
                    if (arrayIndex >= selectArray.Length) break;
                }
                else
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }
            }
            EditorGUILayout.EndHorizontal();
            if (arrayIndex >= selectArray.Length) break;
        }
        EditorGUILayout.EndVertical();

        FieldIndexOffset[] attackArray = skillData.attackFieldIndexOffsetArray;
        arrayIndex = 0;
        rowMin = int.MaxValue; rowMax = int.MinValue;
        columnMin = int.MaxValue; columnMax = int.MinValue;
        //��int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

        foreach (FieldIndexOffset offset in attackArray)
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
                //
                //if (skillData.attackRangeArray[i, j])
                //{
                //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //}
                //else
                //{
                //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                //}

                
                if (i == attackArray[arrayIndex].rowOffset && j == attackArray[arrayIndex].columnOffset)
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    arrayIndex++;
                    if (arrayIndex >= attackArray.Length) break;
                }
                else
                {
                    if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                }
            }
            EditorGUILayout.EndHorizontal();
            if (arrayIndex >= attackArray.Length) break;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.HelpBox("������̏ꍇ�ŕ\������Ă��܂��B\n�オ�I���\�}�X�A�����U���͈͂ł��B\n�I���\�}�X�ɂ����āA���̓L�����N�^�[�̈ʒu��\���܂��B\n�U���͈͂ɂ����āA���͑I�����ꂽ�}�X��\���܂��B\n���h��́��͂��̃}�X��͈͂Ɋ܂܂Ȃ����Ƃ��A\n���h��́��͂��̃}�X��͈͂Ɋ܂ނ��Ƃ�\���܂��B", MessageType.Info);
    }
}
