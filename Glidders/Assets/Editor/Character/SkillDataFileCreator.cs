using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Character;

public class SkillDataFileCreator : MonoBehaviour
{
    const string PATH = "Assets/ScriptableObjects/Skills/";

    public void CreateSkillScriptableObject(SkillScriptableObject skillData)
    {
        string path = PATH + skillData.skillName + ".asset";

        // �C���X�^���X���������̂��A�Z�b�g�Ƃ��ĕۑ�
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(SkillScriptableObject));
        if (asset == null)
        {
            // �w��̃p�X�Ƀt�@�C�������݂��Ȃ��ꍇ�͐V�K�쐬
            AssetDatabase.CreateAsset(skillData as SkillScriptableObject, path);
            Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
        }
        else
        {
            // �w��̃p�X�Ɋ��ɓ����̃t�@�C�������݂���ꍇ�͍X�V(�폜���V�K�쐬)
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(skillData, path);
            Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
}
