using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;
using Glidders.Buff;

public class UniqueSkillCreator : EditorWindow
{
    // アセットファイル作成用のクラス
    UniqueSkillScriptableObject uniqueSkillData;

    bool initialize = true;

    [MenuItem("Window/UniqueSkillCreator")]
    static void Open()
    {
        EditorWindow.GetWindow<SkillCreator>("UniqueSkillCreator");
    }

    private void OnGUI()
    {
        if (initialize)
        {
            // スキルデータを保存するScriptableObjectの作成
            uniqueSkillData = ScriptableObject.CreateInstance<UniqueSkillScriptableObject>();

            Reset();

            //※skillData.gridList = new List<FieldIndexOffset>();
            initialize = false;
        }

        // 名称
        uniqueSkillData.skillName = EditorGUILayout.TextField("名称", uniqueSkillData.skillName);

        // エネルギー
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("エネルギー");
            uniqueSkillData.energy = EditorGUILayout.IntSlider(uniqueSkillData.energy, 1, 5);
        }

        // 優先度
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("優先度");
            uniqueSkillData.priority = EditorGUILayout.IntSlider(uniqueSkillData.priority, 1, 10);
        }

        EditorGUILayout.Space();

        // 移動の種類
        uniqueSkillData.moveType = (UniqueSkillMoveType)EditorGUILayout.EnumPopup("移動の種類", uniqueSkillData.moveType);
        EditorGUILayout.LabelField("NONE …移動しない");
        EditorGUILayout.LabelField("FREE …通常移動");
        EditorGUILayout.LabelField("FIXED…固定移動");

        // 移動先マス
        EditorGUILayout.LabelField("頑張って作ります");

        EditorGUILayout.Space();

        // 攻撃するかどうか
        //uniqueSkillData.isAttack

        // ダメージ
        uniqueSkillData.damage = EditorGUILayout.IntField("ダメージ", uniqueSkillData.damage);

        // 威力
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("威力(ダメージフィールド)");
            uniqueSkillData.power = EditorGUILayout.IntSlider(uniqueSkillData.power, 1, 5);
        }
    }

    private void Reset()
    {/*
        // 各種変数の初期化
        skillName = "";
        energy = 0;
        damage = 0;
        priority = 1;
        power = 0;
        skillIcon = null;
        skillType = SkillTypeEnum.ATTACK;
        giveBuff = new List<BuffViewData>();
        uniqueSkillData.selectGridArray = new bool[rangeSize * rangeSize];
        uniqueSkillData.attackGridArray = new bool[rangeSize * rangeSize];
        uniqueSkillData.rangeSize = rangeSize;
        uniqueSkillData.center = rangeSize / 2;

        // 攻撃範囲の二次元配列の初期化
        for (int i = 0; i < rangeSize; ++i)
        {
            for (int j = 0; j < rangeSize; ++j)
            {
                selectRange[i, j] = false;
                attackRange[i, j] = false;
                selectRangeBefore[i, j] = false;
                attackRangeBefore[i, j] = false;
                skillData.selectGridArray[i * rangeSize + j] = false;
                skillData.attackGridArray[i * rangeSize + j] = false;
            }
        }*/
    }
}
