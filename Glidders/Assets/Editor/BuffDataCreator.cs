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

    string id;
    string buffName;
    string buffCaption;
    Sprite buffIcon;

    List<StatusTypeEnum> buffStatusList;
    List<int> buffTypeIndexList;
    List<float> buffScaleList;
    List<int> buffDurationList;

    private string[] signArray = { "+", "*" };

    // おふざけ要素。消してもいいです。
    int jokeInt;

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

            GUILayout.Box(EditorGUIUtility.Load("BuildSettings.Switch") as Texture, GUI.skin.label);

            // おふざけ要素。消してもいいです。
            jokeInt = Joke();
        }

        using (new EditorGUILayout.VerticalScope())
        {
            // バフの表示に関する情報を設定
            id = EditorGUILayout.TextField("識別ID", id);
            buffViewData.id = id;

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
                        EditorGUILayout.LabelField(string.Format("{0:00}", i), GUILayout.Width(indexWidth));

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

            // 保存ボタンが押されたときに保存処理をする
            if (GUILayout.Button("保存"))
            {
                if (EditorUtility.DisplayDialog("保存確認", "バフデータを保存しますか？", "OK", "cancel"))
                {
                    CreateBuffData();
                    initialize = true;
                }
            }

            // おふざけ要素。消してもいいです。
            switch (jokeInt)
            {
                case 0:
                    for (int i = 0; i < 6; i++) EditorGUILayout.Space();
                    GUILayout.Box(EditorGUIUtility.Load("BuildSettings.Switch") as Texture, GUI.skin.label);
                    break;
                case 1:
                    for (int i = 0; i < 6; i++) EditorGUILayout.Space();
                    GUILayout.Box(EditorGUIUtility.Load("BuildSettings.PS4") as Texture, GUI.skin.label);
                    break;
                case 2:
                    for (int i = 0; i < 6; i++) EditorGUILayout.Space();
                    GUILayout.Box(EditorGUIUtility.Load("BuildSettings.XboxOne") as Texture, GUI.skin.label);
                    break;
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

        id = "";
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
        bool updateFlg = false;

        // バフデータの保存先を設定
        const string PATH = "Assets/ScriptableObjects/Buffs/";
        string path = PATH + string.Format("{0}", buffViewData.buffName) + ".asset";
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(BuffViewData));

        // 同名のバフデータが既に存在するか確認し、存在した場合は削除する
        if (asset != null)
        {
            updateFlg = true;
            AssetDatabase.DeleteAsset(path);
            int i = 0;
            while (true)
            {
                string valuePath = PATH + string.Format("{0}_{1:00}", buffViewData.buffName, i) + ".asset";
                var valueAsset = AssetDatabase.LoadAssetAtPath(valuePath, typeof(BuffValueData));
                if (valueAsset == null)
                {
                    break;
                }
                else
                {
                    AssetDatabase.DeleteAsset(valuePath);
                }
                i++;
            }
        }

        for (int i = 0; i < buffValueCount; i++)
        {
            string valuePath = PATH + string.Format("{0}_{1:00}", buffViewData.buffName, i) + ".asset";

            // インスタンス化したものをアセットとして保存
            // valueDataを生成
            AssetDatabase.CreateAsset(buffValueDataList[i], valuePath);

            EditorUtility.SetDirty(buffValueDataList[i]);
            AssetDatabase.SaveAssets();

            // BuffViewDataのリストに生成したValueDataを追加
            buffViewData.buffValueList.Add(buffValueDataList[i]);
        }

        // インスタンス化したものをアセットとして保存
        // viewDataを生成
        AssetDatabase.CreateAsset(buffViewData, path);

        EditorUtility.SetDirty(buffViewData);
        AssetDatabase.SaveAssets();

        if (updateFlg)
        {
            Debug.Log(string.Format($"Updated \"{buffViewData.buffName}\""));
        }
        else
        {
            Debug.Log(string.Format($"Created new buff data, \"{buffViewData.buffName}\"!"));
        }
    }

    /// <summary>
    /// おふざけ関数。消してもいいやつです。
    /// </summary>
    /// <returns></returns>
    private int Joke()
    {
        int random = (int)(Random.value * 50);
        return random;
    }
}
