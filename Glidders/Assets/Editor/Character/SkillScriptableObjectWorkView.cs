using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;

[CustomEditor(typeof(SkillScriptableObjectWork))]
public class SkillScriptableObjectWorkView : Editor
{
    
    static int selectRangeToggleSize = 15;                        // 攻撃範囲１マスの大きさ


    public override void OnInspectorGUI()
    {
        SkillScriptableObjectWork skillScriptableObject = target as SkillScriptableObjectWork;
        //SerializedObject skillScriptableObjectSO = new SerializedObject(skillScriptableObject);

        int center = skillScriptableObject.FIELD_SIZE / 2;

        //SerializedProperty skillNameSP = skillScriptableObjectSO.FindProperty("skillName");
        skillScriptableObject.skillName = EditorGUILayout.TextField("スキル名称", skillScriptableObject.skillName);

        EditorGUILayout.LabelField("消費エネルギー");
        skillScriptableObject.energy = EditorGUILayout.IntSlider(skillScriptableObject.energy, 1, 5);

        skillScriptableObject.damage = EditorGUILayout.IntField("ダメージ", skillScriptableObject.damage);

        EditorGUILayout.LabelField("優先度");
        skillScriptableObject.priority = EditorGUILayout.IntSlider(skillScriptableObject.priority, 1, 10);

        EditorGUILayout.LabelField("威力");
        skillScriptableObject.power = EditorGUILayout.IntSlider(skillScriptableObject.power, 1, 5);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = 0; i < skillScriptableObject.FIELD_SIZE; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < skillScriptableObject.FIELD_SIZE; j++)
            {
                // 範囲の中央なら、プレイヤーを表示する
                if (i == center && j == center)
                {
                    skillScriptableObject.selectRangeArray[i, j] = EditorGUILayout.Toggle("", skillScriptableObject.selectRangeArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                    continue;
                }
                skillScriptableObject.selectRangeArray[i, j] = EditorGUILayout.Toggle("", skillScriptableObject.selectRangeArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = 0; i < skillScriptableObject.FIELD_SIZE; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < skillScriptableObject.FIELD_SIZE; j++)
            {
                // 範囲の中央なら、プレイヤーを表示する
                if (i == center && j == center)
                {
                    skillScriptableObject.attackRangeArray[i, j] = EditorGUILayout.Toggle("", skillScriptableObject.attackRangeArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                    continue;
                }
                skillScriptableObject.attackRangeArray[i, j] = EditorGUILayout.Toggle("", skillScriptableObject.attackRangeArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        for(int i = 0; i < 13; i++)
        {
            skillScriptableObject.array[i] = EditorGUILayout.Toggle("", skillScriptableObject.array[i]);
        }

        EditorGUILayout.HelpBox(string.Format("キャラクターが上向きの場合で作成してください。\n中心にあるチェックボックスがキャラクターのいる位置です。\nキャラクターの位置を範囲に含めたい場合はチェックを入れてください。"), MessageType.Warning);


        for (int i = 0; i < skillScriptableObject.FIELD_SIZE; i++)
        {
            for ( int j = 0; j < skillScriptableObject.FIELD_SIZE; j++)
            {
                if (skillScriptableObject.selectRangeArray[i, j]) skillScriptableObject.selectGridList.Add(new FieldIndexOffset(i - center, j - center));
                if (skillScriptableObject.attackRangeArray[i, j]) skillScriptableObject.attackGridList.Add(new FieldIndexOffset(i - center, j - center));
            }
        }
    }
}
