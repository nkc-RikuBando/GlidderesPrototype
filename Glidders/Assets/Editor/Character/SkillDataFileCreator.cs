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
            AssetDatabase.CreateAsset(skillData, path);
            Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
        }
        else
        {
            // 指定のパスに既に同名のファイルが存在する場合はデータを破棄
            //EditorUtility.CopySerialized(skillData, asset);
            //AssetDatabase.SaveAssets();
            //Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));            
            Debug.Log(string.Format($"\"{skillData.skillName}\" has already been created!\n Please Update On Inspector Window!"));            
        }
        AssetDatabase.Refresh();
    }
}
