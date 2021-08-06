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
                        buffViewData.buffValueList.Add(null);
                    }
                }
            }
        }
    }
}
