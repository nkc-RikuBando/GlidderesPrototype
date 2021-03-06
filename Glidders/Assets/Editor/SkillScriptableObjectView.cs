using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;
using Glidders.Buff;


[CustomEditor(typeof(SkillScriptableObject))]
public class SkillScriptableObjectView : Editor
{
    // UÍÍÌ\¦Égp
    const string DOT = "¡";
    const string NONE = "@";
    const string PLAYER_FALSE = "¢";
    const string PLAYER_TRUE = "£";
    const int DOT_WIDTH = 12;
    const int DOT_HEIGHT = 10;

    //string skillName;
    //int energy;
    //int damage;
    //int priority;
    //int power;
    //Sprite skillIcon;
    //AnimationClip animationClip;

    public override void OnInspectorGUI()
    {
        SkillScriptableObject skillData = target as SkillScriptableObject;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("¯ÊID");
        skillData.id = EditorGUILayout.TextField("", skillData.id);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("XL¼Ì");
        skillData.skillName = EditorGUILayout.TextField("", skillData.skillName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("XLà¾¶");
        skillData.skillCaption = EditorGUILayout.TextArea(skillData.skillCaption);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ÁïGlM[");
        skillData.energy = EditorGUILayout.IntSlider(skillData.energy, 1, 5);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("_[W");
        skillData.damage = EditorGUILayout.IntField(skillData.damage);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dæx");
        skillData.priority = EditorGUILayout.IntSlider(skillData.priority, 1, 10);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ÐÍ");
        skillData.power = EditorGUILayout.IntSlider(skillData.power, 1, 5);
        EditorGUILayout.EndHorizontal();

        skillData.skillType = (SkillTypeEnum)EditorGUILayout.EnumPopup("XLÌíÞ", skillData.skillType);

        // ±ÌXLÅt^³êéotðÝè
        int buffButtonWidth = 20;
        int buffObjectWidth = 160;
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("t^³êéot");
            using (new GUILayout.VerticalScope())
            {
                for (int i = 0; i < skillData.giveBuff.Count; i++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        skillData.giveBuff[i] = EditorGUILayout.ObjectField("", skillData.giveBuff[i], typeof(BuffViewData), true, GUILayout.Width(buffObjectWidth)) as BuffViewData;
                        if (GUILayout.Button("-", GUILayout.Width(buffButtonWidth)))
                        {
                            skillData.giveBuff.RemoveAt(i);
                        }
                    }
                }
                if (GUILayout.Button("+", GUILayout.Width(buffButtonWidth)))
                {
                    skillData.giveBuff.Add(null);
                }
            }
        }

        skillData.skillAnimation = EditorGUILayout.ObjectField("Aj[VNbv", skillData.skillAnimation, typeof(AnimationClip), true) as AnimationClip;

        skillData.skillIcon = EditorGUILayout.ObjectField("XLACR", skillData.skillIcon, typeof(Sprite), true, GUILayout.Width(224), GUILayout.Height(224)) as Sprite;

        FieldIndexOffset[] selectArray = skillData.selectFieldIndexOffsetArray;

        int arrayIndex = 0;
        int rowMin = int.MaxValue, rowMax = int.MinValue;
        int columnMin = int.MaxValue, columnMax = int.MinValue;
        //¦int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

        // UÍÍð`æ·éÛÌÅã,Åº,Å¶,ÅEðßé
        foreach (FieldIndexOffset offset in selectArray)
        {
            if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
            if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
            if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
            if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
        }
        // SÀWð`æ·éæ¤É·é
        if (rowMin > 0) rowMin = 0;
        if (rowMax < 0) rowMax = 0;
        if (columnMin > 0) columnMin = 0;
        if (columnMax < 0) columnMax = 0;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = rowMin; i <= rowMax; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = columnMin; j <= columnMax; j++)
            {
                if (!(arrayIndex >= selectArray.Length) && i == selectArray[arrayIndex].rowOffset && j == selectArray[arrayIndex].columnOffset)
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
        }
        EditorGUILayout.EndVertical();

        FieldIndexOffset[] attackArray = skillData.attackFieldIndexOffsetArray;
        arrayIndex = 0;
        rowMin = int.MaxValue; rowMax = int.MinValue;
        columnMin = int.MaxValue; columnMax = int.MinValue;
        //¦int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

        // UÍÍð`æ·éÛÌÅã,Åº,Å¶,ÅEðßé
        foreach (FieldIndexOffset offset in attackArray)
        {
            if (offset.rowOffset < rowMin) rowMin = offset.rowOffset;
            if (offset.rowOffset > rowMax) rowMax = offset.rowOffset;
            if (offset.columnOffset < columnMin) columnMin = offset.columnOffset;
            if (offset.columnOffset > columnMax) columnMax = offset.columnOffset;
        }
        // SÀWð`æ·éæ¤É·é
        if (rowMin > 0) rowMin = 0;
        if (rowMax < 0) rowMax = 0;
        if (columnMin > 0) columnMin = 0;
        if (columnMax < 0) columnMax = 0;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        for (int i = rowMin; i <= rowMax; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = columnMin; j <= columnMax; j++)
            {
                if (!(arrayIndex >= attackArray.Length) && i == attackArray[arrayIndex].rowOffset && j == attackArray[arrayIndex].columnOffset)
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
        }
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Û¶"))
        {
            //AssetDatabase.Refresh();
            EditorUtility.SetDirty(skillData);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.HelpBox("ãü«ÌêÅ\¦³êÄ¢Ü·B\nãªIðÂ\}XAºªUÍÍÅ·B\nIðÂ\}XÉ¨¢ÄA¢ÍLN^[ÌÊuð\µÜ·B\nUÍÍÉ¨¢ÄA¢ÍIð³ê½}Xð\µÜ·B\nhèÌ¢Í»Ì}XðÍÍÉÜÜÈ¢±ÆðA\nhèÌ£Í»Ì}XðÍÍÉÜÞ±Æð\µÜ·B", MessageType.Info);

        AssetDatabase.SaveAssets();
    }
}
