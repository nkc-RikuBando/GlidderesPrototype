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

        // インスタンス化したものをアセットとして保存
        var asset = (SkillScriptableObject)AssetDatabase.LoadAssetAtPath(PATH, typeof(SkillScriptableObject));
        if (asset == null)
        {
            // 指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(skillData, PATH);
        }
        else
        {
            // 指定のパスに既に同名のファイルが存在する場合は更新
            EditorUtility.CopySerialized(skillData, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
}
