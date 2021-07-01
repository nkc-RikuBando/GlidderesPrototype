using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Character;

[CustomEditor(typeof(CharacterScriptableObject))]
public class CharacterDataEditor : Editor
{
    const int MOVE_AMOUNT_MIN = 1;      // 移動量の下限
    const int MOVE_AMOUNT_MAX = 5;      // 移動量の上限

    private SkillScriptableObject[] skillDatas = new SkillScriptableObject[3];    // 3つのスキルを設定する

    public override void OnInspectorGUI()
    {
        CharacterScriptableObject characterScriptableObject = target as CharacterScriptableObject;

        // キャラクターの名前
        EditorGUILayout.BeginVertical(GUI.skin.box);
        characterScriptableObject.characterName = EditorGUILayout.TextField("名前", characterScriptableObject.characterName);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // キャラクターの移動量
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("キャラクターの移動量");
        characterScriptableObject.moveAmount = EditorGUILayout.IntSlider(characterScriptableObject.moveAmount, MOVE_AMOUNT_MIN, MOVE_AMOUNT_MAX);        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // キャラクターのスキル
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("キャラクターの所有スキル");
        for (int i = 0; i < skillDatas.Length; i++)
        {
            skillDatas[i] = EditorGUILayout.ObjectField(string.Format($"{i + 1}つめのスキル:"), skillDatas[i], typeof(SkillScriptableObject)) as SkillScriptableObject;
            if(skillDatas[i] != null)
            {
                characterScriptableObject.skillDatas[i] = skillDatas[i];
            }
        }
        EditorGUILayout.EndVertical();
    }
}
