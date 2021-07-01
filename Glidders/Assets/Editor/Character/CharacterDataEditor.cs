using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Character;

[CustomEditor(typeof(CharacterScriptableObject))]
public class CharacterDataEditor : Editor
{
    const int MOVE_AMOUNT_MIN = 1;      // �ړ��ʂ̉���
    const int MOVE_AMOUNT_MAX = 5;      // �ړ��ʂ̏��

    private SkillScriptableObject[] skillDatas = new SkillScriptableObject[3];    // 3�̃X�L����ݒ肷��

    public override void OnInspectorGUI()
    {
        CharacterScriptableObject characterScriptableObject = target as CharacterScriptableObject;

        // �L�����N�^�[�̖��O
        EditorGUILayout.BeginVertical(GUI.skin.box);
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
            skillDatas[i] = EditorGUILayout.ObjectField(string.Format($"{i + 1}�߂̃X�L��:"), skillDatas[i], typeof(SkillScriptableObject)) as SkillScriptableObject;
            if(skillDatas[i] != null)
            {
                characterScriptableObject.skillDatas[i] = skillDatas[i];
            }
        }
        EditorGUILayout.EndVertical();
    }
}
