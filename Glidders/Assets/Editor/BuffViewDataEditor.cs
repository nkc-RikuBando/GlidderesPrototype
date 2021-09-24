using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Buff;
using Glidders.Character;

[CustomEditor(typeof(BuffViewData))]
public class BuffViewDataEditor : Editor
{
    bool foldout;

    public override void OnInspectorGUI()
    {
        BuffViewData buffViewData = target as BuffViewData;

        buffViewData.id = EditorGUILayout.TextField("識別ID", buffViewData.id);

        buffViewData.buffIcon = EditorGUILayout.ObjectField("アイコン", buffViewData.buffIcon, typeof(Sprite), true) as Sprite;

        buffViewData.buffName = EditorGUILayout.TextField("名称", buffViewData.buffName);

        buffViewData.buffCaption = EditorGUILayout.TextField("説明文", buffViewData.buffCaption);

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            int indexWidth = 20;
            int listCount = buffViewData.buffValueList.Count;

            foldout = EditorGUILayout.Foldout(foldout, "設定されているバフ一覧");
            if (foldout)
            {
                for (int i = 0; i < listCount; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(string.Format("{0:00}", i), GUILayout.Width(indexWidth));

                        buffViewData.buffValueList[i] = EditorGUILayout.ObjectField("", buffViewData.buffValueList[i], typeof(BuffValueData), true) as BuffValueData;

                        if (listCount >= 2)
                        {
                            if (GUILayout.Button("-", GUILayout.Width(indexWidth)))
                            {
                                // valueDataの名称を取得
                                string valueDataName = buffViewData.buffValueList[i].ToString();
                                int nameIndex = valueDataName.IndexOf(' ');
                                valueDataName = valueDataName.Substring(0, nameIndex);

                                // valueDataのパスを設定
                                const string PATH = "Assets/ScriptableObjects/Buffs/";
                                string valuePath = PATH + valueDataName + ".asset";
                                Debug.Log(valuePath);

                                //valueDataをファイルから削除
                                AssetDatabase.DeleteAsset(valuePath);

                                // ViewDataとの関連付けを削除
                                buffViewData.buffValueList.RemoveAt(i);
                            }
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(" ", GUILayout.Width(indexWidth));
                    if (GUILayout.Button("+", GUILayout.Width(indexWidth)))
                    {
                        // valueDataのパスを設定
                        const string PATH = "Assets/ScriptableObjects/Buffs/";
                        string valuePath = PATH + string.Format("{0}_{1:00}", buffViewData.buffName, listCount) + ".asset";

                        // valueDataを生成
                        BuffValueData newValueData = ScriptableObject.CreateInstance<BuffValueData>();
                        AssetDatabase.CreateAsset(newValueData, valuePath);
                        buffViewData.buffValueList.Add(newValueData);
                    }
                }
            }
        }

        if (GUILayout.Button("保存"))
        {
            //AssetDatabase.Refresh();
            EditorUtility.SetDirty(buffViewData);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }
    }
}
