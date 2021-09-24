using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Buff;
using Glidders.Character;

[CustomEditor(typeof(BuffValueData))]
public class BuffValueDataEditor : Editor
{
    List<string> captions;
    public override void OnInspectorGUI()
    {
        BuffValueData buffValueData = target as BuffValueData;

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            buffValueData.buffedStatus = (StatusTypeEnum)EditorGUILayout.EnumPopup("バフするステータス", buffValueData.buffedStatus);
        }
        captions = new List<string>();
        captions.Add("DAMAGE …与えるダメージ");
        captions.Add("DEFENSE…受けるダメージ");
        captions.Add("POWER   …威力(ダメージフィールドの継続ターン数)");
        captions.Add("MOVE     …移動量");
        captions.Add("SPECIAL…キャラクター能力固有の数値");
        WriteInformation(captions);
        EditorGUILayout.Space();

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            buffValueData.buffType = (BuffTypeEnum)EditorGUILayout.EnumPopup("バフの種類", buffValueData.buffType);
        }
        captions = new List<string>();
        captions.Add("PLUS            …加算");
        captions.Add("MULTIPLIED…乗算");
        WriteInformation(captions);
        EditorGUILayout.Space();

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            buffValueData.buffScale = EditorGUILayout.FloatField("ステータスの増減量", buffValueData.buffScale);
        }
        captions = new List<string>();
        captions.Add("\"PLUS\"は増加する値をそのまま設定");
        captions.Add("\"MULTIPLIED\"は元の値への倍率を設定");
        WriteInformation(captions);
        EditorGUILayout.Space();

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            buffValueData.buffDuration = EditorGUILayout.IntField("継続ターン数", buffValueData.buffDuration);
        }
        captions = new List<string>();
        captions.Add("残りターン数はターン終了時に減少する。");
        captions.Add("バフが発動したターンも減少するため、");
        captions.Add("発動ターンを含めた継続ターン数を設定する。");
        WriteInformation(captions);
        EditorGUILayout.Space();

        if (GUILayout.Button("保存"))
        {
            //AssetDatabase.Refresh();
            EditorUtility.SetDirty(buffValueData);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// 説明書きを表示する関数
    /// </summary>
    /// <param name="captions"></param>
    private void WriteInformation(List<string> captions)
    {
        int iconWidth = 20;

        using (new EditorGUILayout.HorizontalScope(GUI.skin.label))
        {
            GUILayout.Box(EditorGUIUtility.Load("console.erroricon.inactive.sml") as Texture, GUI.skin.label, GUILayout.Width(iconWidth));
            using (new EditorGUILayout.VerticalScope())
            {
                foreach (string caption in captions)
                    EditorGUILayout.LabelField(caption);
            }
        }
    }
}
