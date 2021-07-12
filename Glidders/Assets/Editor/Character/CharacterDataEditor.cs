using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Character;
using Glidders;

[CustomEditor(typeof(CharacterScriptableObject))]
public class CharacterDataEditor : Editor
{
    const int MOVE_AMOUNT_MIN = 1;      // �ړ��ʂ̉���
    const int MOVE_AMOUNT_MAX = 5;      // �ړ��ʂ̏��

    private SkillScriptableObject[] skillDatas = new SkillScriptableObject[Rule.skillCount];    // 3�̃X�L����ݒ肷��

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

        // �L�����N�^�[�̖��O
        EditorGUILayout.BeginVertical(GUI.skin.box);
        //var characterNameSP = serializedObject.FindProperty();
        characterScriptableObject.characterName = EditorGUILayout.TextField("���O", characterScriptableObject.characterName);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // �L�����N�^�[�̈ړ���
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("�L�����N�^�[�̈ړ���");
        characterScriptableObject.moveAmount = EditorGUILayout.IntSlider(characterScriptableObject.moveAmount, MOVE_AMOUNT_MIN, MOVE_AMOUNT_MAX);        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // �L�����N�^�[�̃X�L��
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("�L�����N�^�[�̏��L�X�L��");
        for (int i = 0; i < skillDatas.Length; i++)
        {
            skillDatas[i] = EditorGUILayout.ObjectField(string.Format($"{i + 1}�߂̃X�L��:"), skillDatas[i], typeof(SkillScriptableObject), true) as SkillScriptableObject;
        }
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("�ۑ�"))
        {
            bool unsetFlg = false;
            foreach (SkillScriptableObject skillData in skillDatas)
            {
                if (skillData == null)
                {
                    EditorUtility.DisplayDialog("�X�L�����ݒ�", "�X�L�����ݒ肳��Ă��܂���B", "�����킩�����B");
                    break;
                }
            }
            if (unsetFlg) return;
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("���炷����ł������ۑ�����ׁI");
                for (int i = 0; i < Rule.skillCount; i++)
                {
                    characterScriptableObject.skillDataArray[i] = skillDatas[i];
                }
                //AssetDatabase.Refresh();
                EditorUtility.SetDirty(characterScriptableObject);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                AssetDatabase.SaveAssets();
            }
        }
    }
}
