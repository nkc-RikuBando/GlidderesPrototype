using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Glidders;
using Glidders.Character;

public class SkillCreator : EditorWindow
{
    [MenuItem("Window/SkillCreator")]
    static void Open()
    {
        EditorWindow.GetWindow<SkillCreator>("SkillCreator");
    }

    // アセットファイル作成用のクラス
    SkillDataFileCreator skillDataFileCreator;
    SkillScriptableObject skillData;
    Sprite skillIcon;

    // フィールドサイズの設定
    static int fieldSize = 7;                           // 対戦のフィールドサイズ
    static int rangeSize = fieldSize * 2 - 1;           // フィールドサイズから、攻撃範囲塗りに必要なサイズを計算
    FieldIndex centerIndex = new FieldIndex(rangeSize / 2, rangeSize / 2);   // プレイヤーの位置となる、範囲の中心

    // プレイヤーが入力するスキル情報
    string skillName;            // スキル名称
    int energy;                  // エネルギー
    int damage;                  // ダメージ
    int priority;                // 優先度
    int power;                   // 威力(ダメージフィールド)
    bool[,] selectRange = new bool[rangeSize, rangeSize];           // 選択可能マスを管理する二次元配列
    bool[,] attackRange = new bool[rangeSize, rangeSize];           // 攻撃範囲を管理する二次元配列
    bool[,] selectRangeBefore = new bool[rangeSize, rangeSize];     // 選択可能マスを管理する二次元配列の変更を検知するためのもの
    bool[,] attackRangeBefore = new bool[rangeSize, rangeSize];     // 攻撃範囲を管理する二次元配列の変更を検知するためのもの

    // スライダーの下限・上限を設定する変数
    const int EXTENT_MIN = 0;               // 各種スライダーの下限にアクセスするための添え字
    const int EXTENT_MAX = 1;               // 各種スライダーの上限にアクセスするための添え字
    int[] energyExtent = { 0, 5 };          // 消費エネルギーの設定スライダーの下限・上限
    int[] priorityExtent = { 1, 10 };       // 優先度の設定スライダーの下限・上限
    int[] toggleSizeExtent = { 10, 30 };    // 攻撃範囲のマスサイズの設定スライダーの下限・上限
    int[] fieldSizeExtent = { 5, 15 };      // フィールドサイズ変更の設定スライダーの下限・上限
    int[] powerExtent = { 0, 5 };           // 威力(ダメージフィールド)の設定スライダーの下限・上限

    // 表示・体裁用の変数
    //bool foldout;                                           // バフリストを開閉するための変数
    int beforeFieldSize;                                    // 変更前のフィールドサイズ(フィールドサイズの変更を検知するためのもの)
    static int selectRangeToggleSize = 15;                        // 攻撃範囲１マスの大きさ
    Vector2 scrollPos;                                      // 攻撃範囲のスクロールバー座標
    Vector2 nameWindowSize = new Vector2(400, 20);          // スキル名称のウィンドウサイズ
    Vector2 energySliderSize = new Vector2(300, 20);        // 消費エネルギーのスライダーサイズ
    Vector2 damageWindowSize = new Vector2(200, 20);        // ダメージのウィンドウサイズ
    Vector2 prioritySliderSize = new Vector2(300, 20);      // 優先度のスライダーサイズ
    Vector2 skillIconSize = new Vector2(224, 224);            // スキルアイコンのサイズ
    Vector2 rangeLabelWindowSize = new Vector2(100, 20);    // "攻撃範囲"ラベルのウィンドウサイズ
    Vector2 toggleSizeSliderSize = new Vector2(300, 20);    // 攻撃範囲のマスサイズ変更スライダーのサイズ
    Vector2 fieldSizeSliderSize = new Vector2(300, 20);     // フィールドサイズ変更スライダーのサイズ
    Vector2 powerSliderSize = new Vector2(300, 20);         // 威力(ダメージフィールド)のスライダーサイズ

    bool initialize = true;                                 // 初期化のための変数

    private void OnGUI()
    {
        if (initialize)
        {
            // スキルデータを保存するScriptableObjectの作成
            skillData = ScriptableObject.CreateInstance<SkillScriptableObject>();
            skillDataFileCreator = new SkillDataFileCreator();

            Reset();

            //※skillData.gridList = new List<FieldIndexOffset>();
            initialize = false;
        }

        // スキルアイコンと並列表示するためのもの
        EditorGUILayout.BeginHorizontal();

        // スキルアイコン以外を直列表示
        EditorGUILayout.BeginVertical();
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
        // 直列表示の終了
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        skillIcon = EditorGUILayout.ObjectField("スキルアイコン", skillIcon, typeof(Sprite), true, GUILayout.Width(skillIconSize.x), GUILayout.Height(skillIconSize.y)) as Sprite;
        EditorGUILayout.EndVertical();
        // 並列表示の終了
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();                            // 攻撃範囲ボックスとの余白を作成

        EditorGUILayout.BeginHorizontal();                  // 選択可能マスと攻撃範囲を並列表示するためのもの
        EditorGUILayout.BeginVertical(GUI.skin.box);        // 選択可能マスに関する情報を直列表示するためのもの

        EditorGUILayout.LabelField("選択可能マス", GUILayout.Width(rangeLabelWindowSize.x), GUILayout.Height(rangeLabelWindowSize.y));  // 選択可能マスラベルを表示
        // 表示サイズ変更スライダーを表示
        selectRangeToggleSize = EditorGUILayout.IntSlider("マスの大きさ", selectRangeToggleSize, toggleSizeExtent[EXTENT_MIN], toggleSizeExtent[EXTENT_MAX], GUILayout.Width(toggleSizeSliderSize.x), GUILayout.Height(toggleSizeSliderSize.y));

        // 威力(ダメージフィールド)の入力
        power = EditorGUILayout.IntSlider("威力", power, powerExtent[EXTENT_MIN], powerExtent[EXTENT_MAX], GUILayout.Width(powerSliderSize.x), GUILayout.Height(powerSliderSize.y));
        skillData.power = power;
        
        // フィールドサイズの変更
        fieldSize = EditorGUILayout.IntSlider("フィールドサイズの変更", fieldSize, fieldSizeExtent[EXTENT_MIN], fieldSizeExtent[EXTENT_MAX], GUILayout.Width(fieldSizeSliderSize.x), GUILayout.Height(fieldSizeSliderSize.y));
        if (fieldSize != beforeFieldSize)               // フィールドサイズの変更があったなら
        {
            beforeFieldSize = fieldSize;                // フィールドサイズの更新
            rangeSize = fieldSize * 2 - 1;              // 攻撃範囲設定画面のサイズを更新
            selectRange = new bool[rangeSize, rangeSize];     // 各配列のサイズを再設定する
            attackRange = new bool[rangeSize, rangeSize];
            selectRangeBefore = new bool[rangeSize, rangeSize];
            attackRangeBefore = new bool[rangeSize, rangeSize];
            skillData.selectGridArray = new bool[rangeSize * rangeSize];
            skillData.attackGridArray = new bool[rangeSize * rangeSize];
            centerIndex.row = rangeSize / 2; centerIndex.column = rangeSize / 2;  // 中心位置を再設定
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);     // 選択可能マスのスクロールバーの表示
        // 選択可能マスを入力するトグルを表示
        for (int i = 0; i < rangeSize; ++i)
        {
            EditorGUILayout.BeginHorizontal();      // 横一列表示をするため、並列表示の設定をする
            for (int j = 0; j < rangeSize; ++j)
            {
                // 範囲の中央なら、プレイヤーを表示する
                if (i == centerIndex.row && j == centerIndex.column)
                {
                    selectRange[i, j] = EditorGUILayout.Toggle("", selectRange[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                    // トグルの値が更新された場合、リストの内容を更新する
                    if (selectRange[i, j] != selectRangeBefore[i, j])
                    {
                        SetArrayData(new FieldIndex(i, j), selectRange[i, j], "Select");
                    }
                    selectRangeBefore[i, j] = selectRange[i, j];
                    continue;
                }
                // ボタンの見た目に変更したトグルを表示
                selectRange[i, j] = EditorGUILayout.Toggle("", selectRange[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                // トグルの値が更新された場合、リストの内容を更新する
                if (selectRange[i, j] != selectRangeBefore[i, j])
                {
                    SetArrayData(new FieldIndex(i, j), selectRange[i, j], "Select");
                }
                selectRangeBefore[i, j] = selectRange[i, j];
            }
            EditorGUILayout.EndHorizontal();        // 横一列の表示が完了したため、並列表示の終了
        }
        EditorGUILayout.EndScrollView();                            // 選択可能マスのスクロールバーの範囲決定
        EditorGUILayout.HelpBox("上向きの場合で設定してください。\nチェックボックスが中心（キャラクターの位置）です。\n中心マスを含める場合はチェックを入れてください。", MessageType.None);
        EditorGUILayout.EndVertical();       // 選択可能マスに関する直列表示の終了

        EditorGUILayout.BeginVertical(GUI.skin.box);    // 攻撃範囲に関する情報を直列表示するためのもの
        EditorGUILayout.LabelField("攻撃範囲", GUILayout.Width(rangeLabelWindowSize.x), GUILayout.Height(rangeLabelWindowSize.y));  // 攻撃範囲ラベルを表示
        // 表示サイズ変更スライダーを表示
        selectRangeToggleSize = EditorGUILayout.IntSlider("マスの大きさ", selectRangeToggleSize, toggleSizeExtent[EXTENT_MIN], toggleSizeExtent[EXTENT_MAX], GUILayout.Width(toggleSizeSliderSize.x), GUILayout.Height(toggleSizeSliderSize.y));

        // 威力(ダメージフィールド)の入力
        power = EditorGUILayout.IntSlider("威力", power, powerExtent[EXTENT_MIN], powerExtent[EXTENT_MAX], GUILayout.Width(powerSliderSize.x), GUILayout.Height(powerSliderSize.y));
        skillData.power = power;
        
        // フィールドサイズの変更
        fieldSize = EditorGUILayout.IntSlider("フィールドサイズの変更", fieldSize, fieldSizeExtent[EXTENT_MIN], fieldSizeExtent[EXTENT_MAX], GUILayout.Width(fieldSizeSliderSize.x), GUILayout.Height(fieldSizeSliderSize.y));
        if (fieldSize != beforeFieldSize)               // フィールドサイズの変更があったなら
        {
            beforeFieldSize = fieldSize;                // フィールドサイズの更新
            rangeSize = fieldSize * 2 - 1;              // 攻撃範囲設定画面のサイズを更新
            selectRange = new bool[rangeSize, rangeSize];     // 各配列のサイズを再設定する
            attackRange = new bool[rangeSize, rangeSize];
            selectRangeBefore = new bool[rangeSize, rangeSize];
            attackRangeBefore = new bool[rangeSize, rangeSize];
            skillData.selectGridArray = new bool[rangeSize * rangeSize];
            skillData.attackGridArray = new bool[rangeSize * rangeSize];
            centerIndex.row = rangeSize / 2; centerIndex.column = rangeSize / 2;  // 中心位置を再設定
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);     // 攻撃範囲のスクロールバーの表示
        // 攻撃範囲を入力するトグルを表示
        for (int i = 0; i < rangeSize; ++i)
        {
            EditorGUILayout.BeginHorizontal();      // 横一列表示をするため、並列表示の設定をする
            for (int j = 0; j < rangeSize; ++j)
            {
                // 範囲の中央なら、プレイヤーを表示する
                if (i == centerIndex.row && j == centerIndex.column)
                {
                    attackRange[i, j] = EditorGUILayout.Toggle("", attackRange[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                    // トグルの値が更新された場合、リストの内容を更新する
                    if (attackRange[i, j] != attackRangeBefore[i, j])
                    {
                        SetArrayData(new FieldIndex(i, j), attackRange[i, j], "Attack");
                    }
                    attackRangeBefore[i, j] = attackRange[i, j];
                    continue;
                }
                // ボタンの見た目に変更したトグルを表示
                attackRange[i, j] = EditorGUILayout.Toggle("", attackRange[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                // トグルの値が更新された場合、リストの内容を更新する
                if (attackRange[i, j] != attackRangeBefore[i, j])
                {
                    SetArrayData(new FieldIndex(i, j), attackRange[i, j], "Attack");
                }
                attackRangeBefore[i, j] = attackRange[i, j];
            }
            EditorGUILayout.EndHorizontal();        // 横一列の表示が完了したため、並列表示の終了
        }
        EditorGUILayout.EndScrollView();            // 攻撃範囲のスクロールバーの範囲決定]
        EditorGUILayout.HelpBox("上向きの場合で設定してください。\nチェックボックスが中心（プレイヤーが選択した位置）です。\n中心マスを含める場合はチェックを入れてください。", MessageType.None);
        EditorGUILayout.EndVertical();              // 攻撃範囲に関する直列表示の終了

        EditorGUILayout.EndHorizontal();            // 選択可能マスと攻撃範囲の並列表示の終了

        // 注意書きの表示
        EditorGUILayout.HelpBox("フィールドサイズを変更すると、選択可能マスおよび攻撃範囲はリセットされます。", MessageType.Warning);

        // スキルデータ保存ボタン
        if (GUILayout.Button("スキルデータ保存"))
        {
            // スキル名称が入力されていない場合
            if (skillName == "")
            {
                EditorUtility.DisplayDialog("Error!", string.Format("スキル名称が入力されていません。"), "OK");
                return;
            }
            // 攻撃範囲が1マスも存在しない場合
            bool rangeError = false;
            foreach (bool flg in selectRange)
            {
                rangeError = flg;
                if (rangeError) break;
            }
            if (!rangeError)
            {
                EditorUtility.DisplayDialog("Error!", string.Format("攻撃範囲が設定されていません。"), "OK");
                return;
            }

            // 保存確認
            if (!EditorUtility.DisplayDialog("スキルデータ保存確認", string.Format("スキルデータを保存しますか？"), "OK", "CANCEL")) return;

            /*※// 攻撃範囲のグリッドリストの作成
            int centerIndex = rangeSize / 2;
            for (int i = 0; i < rangeSize; ++i)
            {
                for (int j = 0; j < rangeSize; ++j)
                {
                    if (selectRange[i, j]) skillData.gridList.Add(new FieldIndexOffset(i, j, centerIndex, centerIndex));
                }
            }*/

            //※skillData.selectRangeArray = selectRange;
            //※skillData.attackRangeArray = attackRange;
            
            // データ保存メソッドの呼び出し
            CreateSkillScriptableObject();

            // ウィンドウを閉じる
            Close();
        }

        // リセットボタンの配置
        if (GUILayout.Button("リセット"))
        {
            if (EditorUtility.DisplayDialog("リセット確認", string.Format("入力したデータをリセットしますか？"), "OK", "CANCEL")) Reset();
        }
        EditorUtility.SetDirty(skillData);
        AssetDatabase.SaveAssets();
    }

    public void CreateSkillScriptableObject()
    {
        const string PATH = "Assets/ScriptableObjects/Skills/";
        string path = PATH + skillData.skillName + ".asset";

        // インスタンス化したものをアセットとして保存
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(SkillScriptableObject));
        if (asset == null)
        {
            // 指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(skillData, path);
            Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
        }
        else
        {
            // 指定のパスに既に同名のファイルが存在する場合はデータを破棄
            //EditorUtility.CopySerialized(skillData, asset);
            //AssetDatabase.SaveAssets();
            //Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));            
            Debug.Log(string.Format($"\"{skillData.skillName}\" has already been created!\n Please Update On Inspector Window!"));
        }
        EditorUtility.SetDirty(skillData);
        AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    private void Reset()
    {
        // 各種変数の初期化
        skillName = "";
        energy = 0;
        damage = 0;
        priority = 1;
        power = 0;
        skillData.selectGridArray = new bool[rangeSize * rangeSize];
        skillData.attackGridArray = new bool[rangeSize * rangeSize];
        skillData.rangeSize = rangeSize;
        skillData.center = rangeSize / 2;

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
        }
    }

    private void SetArrayData(FieldIndex gridData, bool active, string listType)
    {
        int index = gridData.row * rangeSize + gridData.column;
        if (listType == "Select")
        {
            skillData.selectGridArray[index] = active;
        }
        else if (listType == "Attack")
        {
            skillData.attackGridArray[index] = active;
        }
    }
}