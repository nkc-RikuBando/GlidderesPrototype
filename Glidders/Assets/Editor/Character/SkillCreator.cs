using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Glidders.Character;

public class SkillCreator : EditorWindow
{
    [MenuItem("Window/SkillCreator")]
    static void Open()
    {
        EditorWindow.GetWindow<SkillCreator>("SkillCreator");
    }

    // アセットファイル作成用のクラス
    SkillDataFileCreator skillDataFileCreator = new SkillDataFileCreator();
    SkillScriptableObject skillData;

    // フィールドサイズの設定
    static int fieldSize = 7;                           // 対戦のフィールドサイズ
    static int rangeSize = fieldSize * 2 - 1;           // フィールドサイズから、攻撃範囲塗りに必要なサイズを計算

    // プレイヤーが入力するスキル情報
    string skillName;            // スキル名称
    int energy;                  // エネルギー
    int damage;                  // ダメージ
    int priority;                // 優先度
    int power;                   // 威力(ダメージフィールド)
    bool[,] range = new bool[rangeSize, rangeSize];     // 攻撃範囲を管理する二次元配列

    // スライダーの下限・上限を設定する変数
    const int EXTENT_MIN = 0;               // 各種スライダーの下限にアクセスするための添え字
    const int EXTENT_MAX = 1;               // 各種スライダーの上限にアクセスするための添え字
    int[] energyExtent = { 0, 5 };          // 消費エネルギーの設定スライダーの下限・上限
    int[] priorityExtent = { 1, 10 };       // 優先度の設定スライダーの下限・上限
    int[] toggleSizeExtent = { 10, 30 };    // 攻撃範囲のマスサイズの設定スライダーの下限・上限
    int[] fieldSizeExtent = { 5, 15 };      // フィールドサイズ変更の設定スライダーの下限・上限
    int[] powerExtent = { 0, 5 };           // 威力(ダメージフィールド)の設定スライダーの下限・上限

    // 表示・体裁用の変数
    bool foldout;                                           // バフリストを開閉するための変数
    int beforeFieldSize;                                    // 変更前のフィールドサイズ(フィールドサイズの変更を検知するためのもの)
    static int rangeToggleSize = 15;                        // 攻撃範囲１マスの大きさ
    Vector2 scrollPos;                                      // 攻撃範囲のスクロールバー座標
    Vector2 nameWindowSize = new Vector2(400, 20);          // スキル名称のウィンドウサイズ
    Vector2 energySliderSize = new Vector2(300, 20);        // 消費エネルギーのスライダーサイズ
    Vector2 damageWindowSize = new Vector2(200, 20);        // ダメージのウィンドウサイズ
    Vector2 prioritySliderSize = new Vector2(300, 20);      // 優先度のスライダーサイズ
    Vector2 rangeLabelWindowSize = new Vector2(100, 20);    // "攻撃範囲"ラベルのウィンドウサイズ
    Vector2 toggleSizeSliderSize = new Vector2(300, 20);    // 攻撃範囲のマスサイズ変更スライダーのサイズ
    Vector2 fieldSizeSliderSize = new Vector2(300, 20);     // フィールドサイズ変更スライダーのサイズ
    Vector2 powerSliderSize = new Vector2(300, 20);         // 威力(ダメージフィールド)のスライダーサイズ

    public SkillCreator()
    {
        // スキルデータを保存するScriptableObjectの作成
        skillData = ScriptableObject.CreateInstance<SkillScriptableObject>();

        // 攻撃範囲の二次元配列の初期化
        for (int i = 0; i < rangeSize; ++i)
        {
            for (int j = 0; j < rangeSize; ++j)
            {
                range[i, j] = false;
            }
        }
    }

    private void OnGUI()
    {
        // スキル名称の入力
        skillName = EditorGUILayout.TextField("スキル名称", skillName, GUILayout.Width(nameWindowSize.x), GUILayout.Height(nameWindowSize.y));
        skillData.skillName = skillName;

        // 消費エネルギーの入力
        energy = EditorGUILayout.IntSlider("消費エネルギー", energy, energyExtent[EXTENT_MIN], energyExtent[EXTENT_MAX], GUILayout.Width(energySliderSize.x), GUILayout.Height(energySliderSize.y));
        skillData.energy = energy;

        // ダメージの入力
        damage = EditorGUILayout.IntField("ダメージ", damage, GUILayout.Width(damageWindowSize.x), GUILayout.Height(damageWindowSize.y));
        skillData.damage = damage;

        // 優先度の入力
        priority = EditorGUILayout.IntSlider("優先度", priority, priorityExtent[EXTENT_MIN], priorityExtent[EXTENT_MAX], GUILayout.Width(prioritySliderSize.x), GUILayout.Height(prioritySliderSize.y));
        skillData.priority = priority;
        // 優先度補足説明ラベルの表示
        EditorGUILayout.LabelField(" 1<-------優先度------->10");
        EditorGUILayout.LabelField("速<-------行動順------->遅");

        EditorGUILayout.Space();                            // 攻撃範囲ボックスとの余白を作成
        EditorGUILayout.BeginVertical(GUI.skin.box);        // 攻撃範囲に関する設定をボックスで囲う

        EditorGUILayout.LabelField("攻撃範囲", GUILayout.Width(rangeLabelWindowSize.x), GUILayout.Height(rangeLabelWindowSize.y));  // 攻撃範囲ラベルを表示
        // 表示サイズ変更スライダーを表示
        rangeToggleSize = EditorGUILayout.IntSlider("マスの大きさ", rangeToggleSize, toggleSizeExtent[EXTENT_MIN], toggleSizeExtent[EXTENT_MAX], GUILayout.Width(toggleSizeSliderSize.x), GUILayout.Height(toggleSizeSliderSize.y));

        EditorGUILayout.BeginHorizontal();      // 注意書きを右に表示するため、並列表示
        EditorGUILayout.BeginVertical();        // フィールドサイズと威力を直列表示
        // 威力(ダメージフィールド)の入力
        power = EditorGUILayout.IntSlider("威力", skillData.power, powerExtent[EXTENT_MIN], powerExtent[EXTENT_MAX], GUILayout.Width(powerSliderSize.x), GUILayout.Height(powerSliderSize.y));
        skillData.power = power;
        
        // フィールドサイズの変更
        fieldSize = EditorGUILayout.IntSlider("フィールドサイズの変更", fieldSize, fieldSizeExtent[EXTENT_MIN], fieldSizeExtent[EXTENT_MAX], GUILayout.Width(fieldSizeSliderSize.x), GUILayout.Height(fieldSizeSliderSize.y));
        if (fieldSize != beforeFieldSize)               // フィールドサイズの変更があったなら
        {
            beforeFieldSize = fieldSize;                // フィールドサイズの更新
            rangeSize = fieldSize * 2 - 1;              // 攻撃範囲設定画面のサイズを更新
            range = new bool[rangeSize, rangeSize];     // 攻撃範囲を管理する配列を再設定
        }
        EditorGUILayout.EndVertical();          // 直列表示の終了
        // 注意書きの表示
        EditorGUILayout.HelpBox("フィールドサイズを変更すると、攻撃範囲はリセットされます。", MessageType.Warning);
        EditorGUILayout.EndHorizontal();        // 並列表示の終了

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);     // 攻撃範囲のスクロールバーの表示
        // 攻撃範囲を入力するトグルを表示
        for (int i = 0; i < rangeSize; ++i)
        {
            EditorGUILayout.BeginHorizontal();      // 横一列表示をするため、並列表示の設定をする
            for (int j = 0; j < rangeSize; ++j)
            {
                // 範囲の中央なら、プレイヤーを表示する
                if (i == rangeSize / 2 && j == rangeSize / 2)
                {
                    EditorGUILayout.LabelField("↑", GUILayout.Width(rangeToggleSize), GUILayout.Height(rangeToggleSize));
                    continue;
                }
                // ボタンの見た目に変更したトグルを表示
                range[i, j] = EditorGUILayout.Toggle("", range[i, j], GUI.skin.button, GUILayout.Width(rangeToggleSize), GUILayout.Height(rangeToggleSize));
            }
            EditorGUILayout.EndHorizontal();        // 横一列の表示が完了したため、並列表示の終了
        }
        EditorGUILayout.EndScrollView();                            // 攻撃範囲のスクロールバーの範囲決定

        // デバッグ用
        if (GUILayout.Button("スキルデータ保存"))
        {
            // 攻撃範囲のグリッドリストの作成
            int centerIndex = rangeSize / 2;
            for (int i = 0; i < rangeSize; ++i)
            {
                for (int j = 0; j < rangeSize; ++j)
                {
                    Debug.Log("grid[i, j] = " + range[i, j]);
                    if (range[i, j]) skillData.gridList.Add(new SkillRangeGrid(i, j, centerIndex, centerIndex));
                }
            }

            // データ保存メソッドの呼び出し
            skillDataFileCreator.CreateSkillScriptableObject(skillData);
        }

        EditorGUILayout.EndVertical();      // 攻撃範囲のボックスを閉じる
    }
}