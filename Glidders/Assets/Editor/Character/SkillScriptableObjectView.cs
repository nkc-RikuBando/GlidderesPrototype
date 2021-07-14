using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;


[CustomEditor(typeof(SkillScriptableObject))]
public class SkillScriptableObjectView : Editor
{
    // 攻撃範囲の表示に使用
    const string DOT = "■";
    const string NONE = "　";
    const string PLAYER_FALSE = "△";
    const string PLAYER_TRUE = "▲";
    const int DOT_WIDTH = 12;
    const int DOT_HEIGHT = 10;

    private bool editFlg = false;

    public override void OnInspectorGUI()
    {
        SkillScriptableObject skillData = target as SkillScriptableObject;

        if (!editFlg)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("スキル名称");
            EditorGUILayout.LabelField(skillData.skillName, GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("消費エネルギー");
            EditorGUILayout.LabelField(skillData.energy.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ダメージ");
            EditorGUILayout.LabelField(skillData.damage.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("優先度");
            EditorGUILayout.LabelField(skillData.priority.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("威力");
            EditorGUILayout.LabelField(skillData.power.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            FieldIndexOffset[] selectArray = skillData.selectFieldIndexOffsetArray;

            int arrayIndex = 0;
            int rowMin = int.MaxValue, rowMax = int.MinValue;
            int columnMin = int.MaxValue, columnMax = int.MinValue;
            //※int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

            foreach (FieldIndexOffset offset in selectArray)
            {
                if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
                if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
                if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
                if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = rowMin; i <= rowMax; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = columnMin; j <= columnMax; j++)
                {
                    //
                    //if (skillData.selectRangeArray[i, j])
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}
                    //else
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}


                    if (i == selectArray[arrayIndex].rowOffset && j == selectArray[arrayIndex].columnOffset)
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        arrayIndex++;
                        if (arrayIndex >= selectArray.Length) break;
                    }
                    else
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (arrayIndex >= selectArray.Length) break;
            }
            EditorGUILayout.EndVertical();

            FieldIndexOffset[] attackArray = skillData.attackFieldIndexOffsetArray;
            arrayIndex = 0;
            rowMin = int.MaxValue; rowMax = int.MinValue;
            columnMin = int.MaxValue; columnMax = int.MinValue;
            //※int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

            foreach (FieldIndexOffset offset in attackArray)
            {
                if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
                if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
                if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
                if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
            }
            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = rowMin; i <= rowMax; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = columnMin; j <= columnMax; j++)
                {
                    //
                    //if (skillData.attackRangeArray[i, j])
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}
                    //else
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}


                    if (i == attackArray[arrayIndex].rowOffset && j == attackArray[arrayIndex].columnOffset)
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        arrayIndex++;
                        if (arrayIndex >= attackArray.Length) break;
                    }
                    else
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (arrayIndex >= attackArray.Length) break;
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("スキル名称");
            skillData.skillName = EditorGUILayout.TextField("",skillData.skillName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("消費エネルギー");
            skillData.energy = EditorGUILayout.IntSlider(skillData.energy, 1, 5);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ダメージ");
            skillData.damage = EditorGUILayout.IntField(skillData.damage);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("優先度");
            skillData.priority = EditorGUILayout.IntSlider(skillData.priority, 1, 10);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("威力");
            skillData.power = EditorGUILayout.IntSlider(skillData.power, 1, 5);
            EditorGUILayout.EndHorizontal();

            skillData.skillIcon = EditorGUILayout.ObjectField("スキルアイコン", skillData.skillIcon, typeof(Sprite), true, GUILayout.Width(224), GUILayout.Height(224)) as Sprite;

            FieldIndexOffset[] selectArray = skillData.selectFieldIndexOffsetArray;

            int arrayIndex = 0;
            int rowMin = int.MaxValue, rowMax = int.MinValue;
            int columnMin = int.MaxValue, columnMax = int.MinValue;
            //※int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

            foreach (FieldIndexOffset offset in selectArray)
            {
                if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
                if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
                if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
                if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = rowMin; i <= rowMax; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = columnMin; j <= columnMax; j++)
                {
                    //
                    //if (skillData.selectRangeArray[i, j])
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}
                    //else
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}


                    if (i == selectArray[arrayIndex].rowOffset && j == selectArray[arrayIndex].columnOffset)
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        arrayIndex++;
                        if (arrayIndex >= selectArray.Length) break;
                    }
                    else
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (arrayIndex >= selectArray.Length) break;
            }
            EditorGUILayout.EndVertical();

            FieldIndexOffset[] attackArray = skillData.attackFieldIndexOffsetArray;
            arrayIndex = 0;
            rowMin = int.MaxValue; rowMax = int.MinValue;
            columnMin = int.MaxValue; columnMax = int.MinValue;
            //※int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

            foreach (FieldIndexOffset offset in attackArray)
            {
                if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
                if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
                if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
                if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
            }
            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = rowMin; i <= rowMax; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = columnMin; j <= columnMax; j++)
                {
                    //
                    //if (skillData.attackRangeArray[i, j])
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}
                    //else
                    //{
                    //    if (i == 6 && j == 6) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //    else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    //}


                    if (i == attackArray[arrayIndex].rowOffset && j == attackArray[arrayIndex].columnOffset)
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_TRUE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(DOT, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        arrayIndex++;
                        if (arrayIndex >= attackArray.Length) break;
                    }
                    else
                    {
                        if (i == 0 && j == 0) EditorGUILayout.LabelField(PLAYER_FALSE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                        else EditorGUILayout.LabelField(NONE, GUILayout.Width(DOT_WIDTH), GUILayout.Height(DOT_HEIGHT));
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (arrayIndex >= attackArray.Length) break;
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.HelpBox("上向きの場合で表示されています。\n上が選択可能マス、下が攻撃範囲です。\n選択可能マスにおいて、△はキャラクターの位置を表します。\n攻撃範囲において、△は選択されたマスを表します。\n白塗りの△はそのマスを範囲に含まないことを、\n黒塗りの▲はそのマスを範囲に含むことを表します。", MessageType.Info);

        if (editFlg && GUILayout.Button("保存"))
        {
            if (EditorUtility.DisplayDialog("保存確認", "データを保存しますか？", "OK", "cancel"))
            {
                EditorUtility.SetDirty(skillData);
                AssetDatabase.SaveAssets();
            }
        }

        string editButtonCaption = !editFlg ? "編集モード移行" : "編集モード終了";
        if (GUILayout.Button(editButtonCaption))
        {
            if (!editFlg)
            {
                if (EditorUtility.DisplayDialog("編集モード", "編集モードに移行しますか？", "OK", "cancel")) editFlg = true;
            }
            else
            {
                if (EditorUtility.DisplayDialog("編集モード", "編集モードを終了しますか？", "OK", "cancel")) editFlg = false;
            }
        }
    }
}
