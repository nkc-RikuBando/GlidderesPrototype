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

    string skillName;
    int energy;
    int damage;
    int priority;
    int power;
    Sprite skillIcon;

    private bool editFlg = false;
    int editCount = 0;
    int savedEditCount = 0;
    private bool beforeEditFlg = false;

    public override void OnInspectorGUI()
    {
        SkillScriptableObject skillData = target as SkillScriptableObject;

        if (!editFlg)
        {
            if (beforeEditFlg)
            {
                beforeEditFlg = false;
            }

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
            if (!beforeEditFlg)
            {
                skillName = skillData.skillName;
                energy = skillData.energy;
                damage = skillData.damage;
                priority = skillData.priority;
                power = skillData.power;
                skillIcon = skillData.skillIcon;
                beforeEditFlg = true;
                editCount = 0;
                savedEditCount = 0;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("スキル名称");
            skillName = EditorGUILayout.TextField("",skillName);
            if (skillName != skillData.skillName)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("消費エネルギー");
            energy = EditorGUILayout.IntSlider(energy, 1, 5);
            if (energy != skillData.energy)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ダメージ");
            damage = EditorGUILayout.IntField(damage);
            if (damage != skillData.damage)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("優先度");
            priority = EditorGUILayout.IntSlider(priority, 1, 10);
            if (priority != skillData.priority)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("威力");
            power = EditorGUILayout.IntSlider(power, 1, 5);
            if (power != skillData.power)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            skillIcon = EditorGUILayout.ObjectField("スキルアイコン", skillIcon, typeof(Sprite), true, GUILayout.Width(224), GUILayout.Height(224)) as Sprite;
            if (skillIcon != skillData.skillIcon)
            {
                editCount++;
            }

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
                skillData.skillName = skillName;
                skillData.energy = energy;
                skillData.damage = damage;
                skillData.priority = priority;
                skillData.power = power;
                skillData.skillIcon = skillIcon;

                savedEditCount = editCount;
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
                if (editCount > savedEditCount)
                {
                    if (EditorUtility.DisplayDialog("保存されていない変更", "保存されていない変更があります。\n変更を破棄して終了しますか？", "OK", "cancel")) editFlg = false;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("編集モード", "編集モードを終了しますか？", "OK", "cancel")) editFlg = false;
                }
            }
        }

        if (editFlg && editCount == savedEditCount)
        {
            EditorGUILayout.HelpBox("データが保存されました。", MessageType.Info);
        }
    }
}
