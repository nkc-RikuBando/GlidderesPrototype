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
    bool isAttack = false;

    // フィールドサイズの設定
    static int fieldSize = 11;                           // 対戦のフィールドサイズ
    static int rangeSize = fieldSize * 2 - 1;           // フィールドサイズから、攻撃範囲塗りに必要なサイズを計算
    FieldIndex centerIndex = new FieldIndex(rangeSize / 2, rangeSize / 2);   // プレイヤーの位置となる、範囲の中心

    int selectRangeToggleSize = 15;
    int width = 300;
    Vector2 skillIconSize = new Vector2(224, 224);            // スキルアイコンのサイズ

    bool[,] moveArray = new bool[rangeSize, rangeSize]; 
    bool[,] selectArray = new bool[rangeSize, rangeSize]; 
    bool[,] attackArray = new bool[rangeSize, rangeSize];
    bool[,] moveArrayBefore = new bool[rangeSize, rangeSize];
    bool[,] selectArrayBefore = new bool[rangeSize, rangeSize];
    bool[,] attackArrayBefore = new bool[rangeSize, rangeSize];
    List<FieldIndexOffset> moveRange = new List<FieldIndexOffset>();             // 移動先を管理する二次元配列
    List<FieldIndexOffset> selectRange = new List<FieldIndexOffset>();           // 選択可能マスを管理する二次元配列
    List<FieldIndexOffset> attackRange = new List<FieldIndexOffset>();           // 攻撃範囲を管理する二次元配列

    [MenuItem("Window/UniqueSkillCreator")]
    static void Open()
    {
        EditorWindow.GetWindow<UniqueSkillCreator>("UniqueSkillCreator");
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

        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope())
            {
                // 名称
                uniqueSkillData.skillName = EditorGUILayout.TextField("名称", uniqueSkillData.skillName, GUILayout.Width(width));

                // エネルギー
                using (new GUILayout.HorizontalScope())
                {
                    uniqueSkillData.energy = EditorGUILayout.IntSlider("エネルギー", uniqueSkillData.energy, 1, 5, GUILayout.Width(width));
                }

                // 優先度
                using (new GUILayout.HorizontalScope())
                {
                    uniqueSkillData.priority = EditorGUILayout.IntSlider("優先度", uniqueSkillData.priority, 1, 10, GUILayout.Width(width));
                }

                EditorGUILayout.Space();

                // 移動の種類
                uniqueSkillData.moveType = (UniqueSkillMoveType)EditorGUILayout.EnumPopup("移動の種類", uniqueSkillData.moveType, GUILayout.Width(width));
                EditorGUILayout.LabelField("　　　　　　　NONE …移動しない");
                EditorGUILayout.LabelField("　　　　　　　FREE …通常移動");
                EditorGUILayout.LabelField("　　　　　　　FIXED…固定移動");

                // 攻撃するかどうか
                isAttack = EditorGUILayout.Toggle("攻撃を行う", isAttack);
                uniqueSkillData.isAttack = isAttack;

                // ダメージ
                uniqueSkillData.damage = EditorGUILayout.IntField("ダメージ", uniqueSkillData.damage, GUILayout.Width(width));

                // 威力
                using (new GUILayout.HorizontalScope())
                {
                    uniqueSkillData.power = EditorGUILayout.IntSlider("威力(ダメージフィールド)", uniqueSkillData.power, 1, 5, GUILayout.Width(width));
                }
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                uniqueSkillData.skillIcon = EditorGUILayout.ObjectField("スキルアイコン", uniqueSkillData.skillIcon, typeof(Sprite), true, GUILayout.Width(skillIconSize.x), GUILayout.Height(skillIconSize.y)) as Sprite;
                uniqueSkillData.skillAnimation = EditorGUILayout.ObjectField("アニメーションクリップ", uniqueSkillData.skillAnimation, typeof(AnimationClip), true) as AnimationClip;
            }
        }

        EditorGUILayout.Space();
        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("移動先マス");
                    EditorGUILayout.Space();

                    // 移動先マス
                    // 選択可能マスを入力するトグルを表示
                    for (int i = 0; i < rangeSize; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();      // 横一列表示をするため、並列表示の設定をする
                        for (int j = 0; j < rangeSize; ++j)
                        {
                            // 範囲の中央なら、プレイヤーを表示する
                            if (i == centerIndex.row && j == centerIndex.column)
                            {
                                moveArray[i, j] = EditorGUILayout.Toggle("", moveArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                                // トグルの値が更新された場合、リストの内容を更新する
                                if (moveArray[i, j] != moveArrayBefore[i, j])
                                {
                                    SetArrayData(new FieldIndexOffset(i, j), "Move");
                                }
                                moveArrayBefore[i, j] = moveArray[i, j];
                                continue;
                            }
                            // ボタンの見た目に変更したトグルを表示
                            moveArray[i, j] = EditorGUILayout.Toggle("", moveArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // トグルの値が更新された場合、リストの内容を更新する
                            if (moveArray[i, j] != moveArrayBefore[i, j])
                            {
                                SetArrayData(new FieldIndexOffset(i, j), "Move");
                            }
                            moveArrayBefore[i, j] = moveArray[i, j];
                        }
                        EditorGUILayout.EndHorizontal();        // 横一列の表示が完了したため、並列表示の終了
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("固定移動の場合は移動の終点を設定してください。\n自身のマスが選択可能なら中心のチェックを入れてください。", MessageType.Warning);
                }
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("攻撃選択可能マス");
                    EditorGUILayout.Space();

                    // 選択可能マスを入力するトグルを表示
                    for (int i = 0; i < rangeSize; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();      // 横一列表示をするため、並列表示の設定をする
                        for (int j = 0; j < rangeSize; ++j)
                        {
                            // 範囲の中央なら、プレイヤーを表示する
                            if (i == centerIndex.row && j == centerIndex.column)
                            {
                                selectArray[i, j] = EditorGUILayout.Toggle("", selectArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                                // トグルの値が更新された場合、リストの内容を更新する
                                if (selectArray[i, j] != selectArrayBefore[i, j])
                                {
                                    SetArrayData(new FieldIndexOffset(i, j), "Select");
                                }
                                selectArrayBefore[i, j] = selectArray[i, j];
                                continue;
                            }
                            // ボタンの見た目に変更したトグルを表示
                            selectArray[i, j] = EditorGUILayout.Toggle("", selectArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // トグルの値が更新された場合、リストの内容を更新する
                            if (selectArray[i, j] != selectArrayBefore[i, j])
                            {
                                SetArrayData(new FieldIndexOffset(i, j), "Select");
                            }
                            selectArrayBefore[i, j] = selectArray[i, j];
                        }
                        EditorGUILayout.EndHorizontal();        // 横一列の表示が完了したため、並列表示の終了
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("上向きの場合で入力してください。\n自身のマスが選択可能なら中心のチェックを入れてください。", MessageType.Warning);
                }
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("攻撃範囲マス");
                    EditorGUILayout.Space();

                    // 攻撃マスを入力するトグルを表示
                    for (int i = 0; i < rangeSize; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();      // 横一列表示をするため、並列表示の設定をする
                        for (int j = 0; j < rangeSize; ++j)
                        {
                            // 範囲の中央なら、プレイヤーを表示する
                            if (i == centerIndex.row && j == centerIndex.column)
                            {
                                attackArray[i, j] = EditorGUILayout.Toggle("", attackArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                                // トグルの値が更新された場合、リストの内容を更新する
                                if (attackArray[i, j] != attackArrayBefore[i, j])
                                {
                                    SetArrayData(new FieldIndexOffset(i, j), "Attack");
                                }
                                attackArrayBefore[i, j] = attackArray[i, j];
                                continue;
                            }
                            // ボタンの見た目に変更したトグルを表示
                            attackArray[i, j] = EditorGUILayout.Toggle("", attackArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // トグルの値が更新された場合、リストの内容を更新する
                            if (attackArray[i, j] != attackArrayBefore[i, j])
                            {
                                SetArrayData(new FieldIndexOffset(i, j), "Attack");
                            }
                            attackArrayBefore[i, j] = attackArray[i, j];
                        }
                        EditorGUILayout.EndHorizontal();        // 横一列の表示が完了したため、並列表示の終了
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("上向きの場合で入力してください。\n中心のチェックマスは選択可能マスで選ばれたマスと対応します。\n自身のマスが攻撃範囲なら中心のチェックを入れてください。", MessageType.Warning);
                }
            }
        }
    }

    private void Reset()
    {
        /*
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
        */

        // 攻撃範囲の二次元配列の初期化
        for (int i = 0; i < rangeSize; ++i)
        {
            for (int j = 0; j < rangeSize; ++j)
            {
                moveArray[i, j] = false;
                selectArray[i, j] = false;
                attackArray[i, j] = false;
                moveArrayBefore[i, j] = false;
                selectArrayBefore[i, j] = false;
                attackArrayBefore[i, j] = false;
                moveRange = new List<FieldIndexOffset>();
                selectRange = new List<FieldIndexOffset>();
                attackRange = new List<FieldIndexOffset>();
            }
        }

        moveArray[centerIndex.row, centerIndex.column] = true;
        selectArray[centerIndex.row, centerIndex.column] = true;
        attackArray[centerIndex.row, centerIndex.column] = true;
        moveArrayBefore[centerIndex.row, centerIndex.column] = true;
        selectArrayBefore[centerIndex.row, centerIndex.column] = true;
        attackArrayBefore[centerIndex.row, centerIndex.column] = true;
    }

    private void SetArrayData(FieldIndexOffset fieldIndexOffset, string arrayName)
    {
        switch(arrayName)
        {
            case "move":
                if (!moveRange.Remove(fieldIndexOffset))
                    moveRange.Add(fieldIndexOffset);
                break;
            case "select":
                if (!selectRange.Remove(fieldIndexOffset))
                    selectRange.Add(fieldIndexOffset);
                break;
            case "attack":
                if (!attackRange.Remove(fieldIndexOffset))
                    attackRange.Add(fieldIndexOffset);
                break;
        }
    }
}
