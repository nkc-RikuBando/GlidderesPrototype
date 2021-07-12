using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Character;
using Glidders;

[CustomEditor(typeof(CharacterScriptableObject))]
public class CharacterDataEditor : Editor
{
    const int MOVE_AMOUNT_MIN = 1;      // 移動量の下限
    const int MOVE_AMOUNT_MAX = 5;      // 移動量の上限

    private SkillScriptableObject[] skillDatas = new SkillScriptableObject[Rule.skillCount];    // 3つのスキルを設定する

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        CharacterScriptableObject characterScriptableObject = target as CharacterScriptableObject;
        Undo.RecordObject(characterScriptableObject, "Undo");

        // キャラクターの名前
        EditorGUILayout.BeginVertical(GUI.skin.box);
        //var characterNameSP = serializedObject.FindProperty();
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
            skillDatas[i] = EditorGUILayout.ObjectField(string.Format($"{i + 1}つめのスキル:"), skillDatas[i], typeof(SkillScriptableObject), true) as SkillScriptableObject;
            Debug.Log("i=" + i+"isNull="+(skillDatas[i] == null));
            if (skillDatas[i] != null)
            {
                Debug.Log("yeahi=" + i + "isNull=" + (skillDatas[i] == null));
                characterScriptableObject.skillDatas.skillDataArray[i] = skillDatas[i];
            }
        }
        EditorGUILayout.EndVertical();

        //if (GUILayout.Button("保存"))
        
            if (EditorGUI.EndChangeCheck())
            {
                //AssetDatabase.Refresh();
                EditorUtility.SetDirty(characterScriptableObject);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                AssetDatabase.SaveAssets();
            }
    }
}
