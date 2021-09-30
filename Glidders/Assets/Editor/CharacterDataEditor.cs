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

    private UniqueSkillScriptableObject[] skillDatas = new UniqueSkillScriptableObject[Rule.skillCount];    // 3つのスキルを設定する

    bool initializeFlg = true;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        CharacterScriptableObject characterScriptableObject = target as CharacterScriptableObject;

        if (initializeFlg)
        {
            for (int i = 0; i < Rule.skillCount; i++)
            {
                skillDatas[i] = characterScriptableObject.skillDataArray[i];
            }
            initializeFlg = false;
        }

        // 識別ID
        EditorGUILayout.BeginVertical(GUI.skin.box);
        characterScriptableObject.id = EditorGUILayout.TextField("識別ID", characterScriptableObject.id);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

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
            skillDatas[i] = EditorGUILayout.ObjectField(string.Format($"{i + 1}つめのスキル:"), skillDatas[i], typeof(UniqueSkillScriptableObject), true) as UniqueSkillScriptableObject;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // ユニークスキル
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("ユニークスキル");
        characterScriptableObject.uniqueSkillData = EditorGUILayout.ObjectField(string.Format(""), characterScriptableObject.uniqueSkillData, typeof(UniqueSkillScriptableObject), true) as UniqueSkillScriptableObject;
        EditorGUILayout.EndVertical();

        // アニメーター
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("アニメーター");
        characterScriptableObject.characterAnimator = EditorGUILayout.ObjectField(string.Format(""), characterScriptableObject.characterAnimator, typeof(RuntimeAnimatorController), true) as RuntimeAnimatorController;
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("保存"))
        {
            bool unsetFlg = false;
            if (characterScriptableObject.id == "")
            {
                EditorUtility.DisplayDialog("識別ID未設定", "識別IDを設定してください。", "あい。");
                unsetFlg = true;
            }
            foreach (UniqueSkillScriptableObject skillData in skillDatas)
            {
                if (skillData == null)
                {
                    EditorUtility.DisplayDialog("スキル未設定", "スキルが設定されていません。", "あいわかった。");
                    unsetFlg = true;
                    break;
                }
            }
            if (unsetFlg) return;
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < Rule.skillCount; i++)
                {
                    characterScriptableObject.skillDataArray[i] = skillDatas[i];
                }

                var obj = EditorUtility.InstanceIDToObject(target.GetInstanceID());
                ScriptableObjectDatabase.Write(characterScriptableObject.id, AssetDatabase.GetAssetPath(obj));
                Debug.Log("aa=" + AssetDatabase.GetAssetPath(obj));

                //AssetDatabase.Refresh();
                EditorUtility.SetDirty(characterScriptableObject);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                AssetDatabase.SaveAssets();
            }
        }
        EditorGUILayout.HelpBox("ファイル名を変更した場合は必ず保存してください。", MessageType.Warning);
    }
}
