using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Character;

public class SkillDataFileCreator : MonoBehaviour
{
    public void CreateSkillScriptableObject(SkillScriptableObject skillData)
    {
        const string PATH = "Assets/ScriptableObjects/Skills";

        // �C���X�^���X���������̂��A�Z�b�g�Ƃ��ĕۑ�
        var asset = (SkillScriptableObject)AssetDatabase.LoadAssetAtPath(PATH, typeof(SkillScriptableObject));
        if (asset == null)
        {
            // �w��̃p�X�Ƀt�@�C�������݂��Ȃ��ꍇ�͐V�K�쐬
            AssetDatabase.CreateAsset(skillData, PATH);
        }
        else
        {
            // �w��̃p�X�Ɋ��ɓ����̃t�@�C�������݂���ꍇ�͍X�V
            EditorUtility.CopySerialized(skillData, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
}
