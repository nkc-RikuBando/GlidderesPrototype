using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Buff;
using Glidders.Character;

public class BuffDataCreator : EditorWindow
{
    [MenuItem("Window/BuffDataCreator")]
    static void Open()
    {
        EditorWindow.GetWindow<BuffDataCreator>("BuffDataCreator");
    }

    List<BuffValueData> buffValueDataList;
    BuffViewData buffViewData;

    bool initialize = true;
    int buffValueCount = 0;

    string buffName;
    string buffCaption;
    Sprite buffIcon;

    List<StatusTypeEnum> buffStatusList;
    List<int> buffTypeIndexList;
    List<float> buffScaleList;
    List<int> buffDurationList;

    private string[] signArray = { "+", "*" };

    private void OnGUI()
    {
        if (initialize)
        {
            // スキルデータを保存するScriptableObjectの作成
            buffValueDataList = new List<BuffValueData>();
            buffViewData = ScriptableObject.CreateInstance<BuffViewData>();
            buffViewData.buffValueList = new List<BuffValueData>();

            Reset();

            initialize = false;
        }

        using (new EditorGUILayout.VerticalScope())
        {
            // バフの表示に関する情報を設定
            buffIcon = EditorGUILayout.ObjectField("バフアイコン", buffIcon, typeof(Sprite), true) as Sprite;
            buffViewData.buffIcon = buffIcon;

            buffName = EditorGUILayout.TextField("バフ名称", buffName);
            buffViewData.buffName = buffName;

            buffCaption = EditorGUILayout.TextField("バフ説明文", buffCaption);
            buffViewData.buffCaption = buffCaption;

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                int indexWidth = 20;
                int popupWidth = 40;
                int enumFieldWidth = 100;
                int scaleFieldWidth = 60;
                int durationFieldWidth = 70;

                using (new EditorGUILayout.HorizontalScope())
                {
                    // 実際に増減するステータスの見出しを表示
                    EditorGUILayout.LabelField(" ", GUILayout.Width(indexWidth));

                    EditorGUILayout.LabelField("バフするステータス", GUILayout.Width(enumFieldWidth));

                    EditorGUILayout.LabelField("倍率/加算", GUILayout.Width(popupWidth + scaleFieldWidth));

                    EditorGUILayout.LabelField("継続ターン数", GUILayout.Width(durationFieldWidth));
                }

                for (int i = 0; i < buffValueCount; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // 実際に増減するステータスを設定
                        EditorGUILayout.LabelField(string.Format($"{i + 1}"), GUILayout.Width(indexWidth));

                        buffStatusList[i] = (StatusTypeEnum)EditorGUILayout.EnumPopup("", buffStatusList[i], GUILayout.Width(enumFieldWidth));
                        buffValueDataList[i].buffedStatus = buffStatusList[i];

                        buffTypeIndexList[i] = EditorGUILayout.Popup(buffTypeIndexList[i], signArray, GUILayout.Width(popupWidth));
                        buffValueDataList[i].buffType = (BuffTypeEnum)buffTypeIndexList[i];

                        buffScaleList[i] = EditorGUILayout.FloatField("", buffScaleList[i], GUILayout.Width(scaleFieldWidth));
                        buffValueDataList[i].buffScale = buffScaleList[i];

                        buffDurationList[i] = EditorGUILayout.IntField("", buffDurationList[i], GUILayout.Width(durationFieldWidth));
                        buffValueDataList[i].buffDuration = buffDurationList[i];

                        // バフデータが2件以上あるなら-ボタンを表示
                        if (buffValueCount >= 2)
                        {
                            // -ボタンが押されたとき、このバフを削除する
                            if (GUILayout.Button("-", GUILayout.Width(indexWidth)))
                            {
                                RemoveDataFromAllList(i);
                            }
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(" ", GUILayout.Width(indexWidth));

                    // +ボタンが押されたとき、バフするステータスを追加する
                    if (GUILayout.Button("+", GUILayout.Width(indexWidth)))
                    {
                        AddDataToAllList();
                    }
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("保存"))
            {
                CreateBuffData();
            }
        }
    }

    /// <summary>
    /// 全てのリストに要素を一つ追加する
    /// </summary>
    private void AddDataToAllList()
    {
        buffValueCount++;

        buffValueDataList.Add(ScriptableObject.CreateInstance<BuffValueData>());
        buffStatusList.Add(StatusTypeEnum.DAMAGE);
        buffTypeIndexList.Add(1);
        buffScaleList.Add(1.0f);
        buffDurationList.Add(1);
    }

    /// <summary>
    /// 全てのリストの要素を一つ削除する
    /// </summary>
    private void RemoveDataFromAllList(int removeIndex)
    {
        buffValueCount--;

        buffValueDataList.RemoveAt(removeIndex);
        buffStatusList.RemoveAt(removeIndex);
        buffTypeIndexList.RemoveAt(removeIndex);
        buffScaleList.RemoveAt(removeIndex);
        buffDurationList.RemoveAt(removeIndex);
    }

    private void Reset()
    {
        buffValueCount = 0;

        buffName = "";
        buffCaption = "";
        buffIcon = null;

        buffStatusList = new List<StatusTypeEnum>();
        buffTypeIndexList = new List<int>();
        buffScaleList = new List<float>();
        buffDurationList = new List<int>();

        AddDataToAllList();
    }

    public void CreateBuffData()
    {
        const string PATH = "Assets/ScriptableObjects/Buffs/";

        for (int i = 0; i < buffValueCount; i++)
        {
            string valuePath = PATH + string.Format("{0}_{1:00}", buffViewData.buffName, i) + ".asset";

            // インスタンス化したものをアセットとして保存
            var valueAsset = AssetDatabase.LoadAssetAtPath(valuePath, typeof(BuffValueData));
            if (valueAsset == null)
            {
                // 指定のパスにファイルが存在しない場合は新規作成
                AssetDatabase.CreateAsset(buffValueDataList[i], valuePath);
                //Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
            }
            else
            {
                // 指定のパスに既に同名のファイルが存在する場合はデータを更新
                EditorUtility.CopySerialized(buffValueDataList[i], valueAsset);
                AssetDatabase.SaveAssets();
                //Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));            
                //Debug.Log(string.Format($"\"{skillData.skillName}\" has already been created!\n Please Update On Inspector Window!"));
            }
            EditorUtility.SetDirty(buffValueDataList[i]);
            AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            // BuffViewDataのリストに生成したValueDataを追加
            buffViewData.buffValueList.Add(buffValueDataList[i]);
        }

        string path = PATH + string.Format("{0}", buffViewData.buffName) + ".asset";

        // インスタンス化したものをアセットとして保存
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(BuffViewData));
        if (asset == null)
        {
            // 指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(buffViewData, path);
            //Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
        }
        else
        {
            // 指定のパスに既に同名のファイルが存在する場合はデータを更新
            EditorUtility.CopySerialized(buffViewData, asset);
            AssetDatabase.SaveAssets();
            //Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));            
            //Debug.Log(string.Format($"\"{skillData.skillName}\" has already been created!\n Please Update On Inspector Window!"));
        }
        EditorUtility.SetDirty(buffViewData);
        AssetDatabase.SaveAssets();

        Debug.Log("Create new buff data!");
        //AssetDatabase.Refresh();
    }
}
