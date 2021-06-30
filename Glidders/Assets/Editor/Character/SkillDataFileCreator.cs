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

        // インスタンス化したものをアセットとして保存
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(SkillScriptableObject));
        if (asset == null)
        {
            // 指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(skillData as SkillScriptableObject, path);
            Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
        }
        else
        {
            // 指定のパスに既に同名のファイルが存在する場合は更新(削除→新規作成)
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(skillData, path);
            Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
}
