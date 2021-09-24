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
    List<BuffViewData> giveBuff; // 付与されるバフ
    List<BuffViewData> loseBuff; // 失うバフ

    // フィールドサイズの設定
    static int fieldSize = 11;                           // 対戦のフィールドサイズ
    static int rangeSize = fieldSize * 2 - 1;           // フィールドサイズから、攻撃範囲塗りに必要なサイズを計算
    FieldIndex centerIndex = new FieldIndex(rangeSize / 2, rangeSize / 2);   // プレイヤーの位置となる、範囲の中心

    int selectRangeToggleSize = 15;
    int width = 300;
    Vector2 skillIconSize = new Vector2(224, 224);            // スキルアイコンのサイズ

    bool[,] moveArray = new bool[rangeSize, rangeSize];         // 移動先マスを格納する二次元配列
    bool[,] selectArray = new bool[rangeSize, rangeSize];       // 選択可能マスを格納する二次元配列
    bool[,] attackArray = new bool[rangeSize, rangeSize];       // 攻撃範囲マスを格納する二次元配列
    //bool[,] moveArrayBefore = new bool[rangeSize, rangeSize];
    //bool[,] selectArrayBefore = new bool[rangeSize, rangeSize];
    //bool[,] attackArrayBefore = new bool[rangeSize, rangeSize];

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
                // 識別ID
                uniqueSkillData.id = EditorGUILayout.TextField("識別ID", uniqueSkillData.id, GUILayout.Width(width));

                // ユニークスキルかどうか
                uniqueSkillData.isUniqueSkill = EditorGUILayout.Toggle("ユニークスキルかどうか", uniqueSkillData.isUniqueSkill);

                // 名称
                uniqueSkillData.skillName = EditorGUILayout.TextField("名称", uniqueSkillData.skillName, GUILayout.Width(width));

                //スキル説明文uniqueSkillData.skillCaption = EditorGUILayout.TextField("説明文", uniqueSkillData.skillCaption);
                EditorGUILayout.LabelField("説明文");
                uniqueSkillData.skillCaption = EditorGUILayout.TextArea(uniqueSkillData.skillCaption);

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
                uniqueSkillData.skillType = (SkillTypeEnum)EditorGUILayout.EnumPopup("技の種類", uniqueSkillData.skillType);

                // ダメージ
                uniqueSkillData.damage = EditorGUILayout.IntField("ダメージ", uniqueSkillData.damage, GUILayout.Width(width));

                // 威力
                using (new GUILayout.HorizontalScope())
                {
                    uniqueSkillData.power = EditorGUILayout.IntSlider("威力(ダメージフィールド)", uniqueSkillData.power, 1, 5, GUILayout.Width(width));
                }

                // このスキルで付与される、失うバフを設定
                int buffButtonWidth = 20;
                int buffObjectWidth = 260;
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("付与されるバフ", GUILayout.Width(width));
                    using (new GUILayout.VerticalScope())
                    {
                        for (int i = 0; i < giveBuff.Count; i++)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                giveBuff[i] = EditorGUILayout.ObjectField("", giveBuff[i], typeof(BuffViewData), true, GUILayout.Width(buffObjectWidth)) as BuffViewData;
                                if (GUILayout.Button("-", GUILayout.Width(buffButtonWidth)))
                                {
                                    giveBuff.RemoveAt(i);
                                }
                            }
                        }
                        if (GUILayout.Button("+", GUILayout.Width(buffButtonWidth)))
                        {
                            giveBuff.Add(null);
                        }
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("失うバフ", GUILayout.Width(width));
                    using (new GUILayout.VerticalScope())
                    {
                        for (int i = 0; i < loseBuff.Count; i++)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                loseBuff[i] = EditorGUILayout.ObjectField("", loseBuff[i], typeof(BuffViewData), true, GUILayout.Width(buffObjectWidth)) as BuffViewData;
                                if (GUILayout.Button("-", GUILayout.Width(buffButtonWidth)))
                                {
                                    loseBuff.RemoveAt(i);
                                }
                            }
                        }
                        if (GUILayout.Button("+", GUILayout.Width(buffButtonWidth)))
                        {
                            loseBuff.Add(null);
                        }
                    }
                }
                uniqueSkillData.giveBuff = giveBuff;
                uniqueSkillData.loseBuff = loseBuff;
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
                                /*// トグルの値が更新された場合、リストの内容を更新する ☆現在不要、☆印は同様の文が記載されていた場所
                                if (moveArray[i, j] != moveArrayBefore[i, j])
                                {
                                    SetArrayData(new FieldIndex(i, j) - centerIndex, "Move");
                                }
                                moveArrayBefore[i, j] = moveArray[i, j];*/
                                continue;
                            }
                            // ボタンの見た目に変更したトグルを表示
                            moveArray[i, j] = EditorGUILayout.Toggle("", moveArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // ☆
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
                                // ☆
                                continue;
                            }
                            // ボタンの見た目に変更したトグルを表示
                            selectArray[i, j] = EditorGUILayout.Toggle("", selectArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // ☆
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
                                // ☆
                                continue;
                            }
                            // ボタンの見た目に変更したトグルを表示
                            attackArray[i, j] = EditorGUILayout.Toggle("", attackArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // ☆
                        }
                        EditorGUILayout.EndHorizontal();        // 横一列の表示が完了したため、並列表示の終了
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("上向きの場合で入力してください。\n中心のチェックマスは選択可能マスで選ばれたマスと対応します。\n自身のマスが攻撃範囲なら中心のチェックを入れてください。", MessageType.Warning);
                }
            }
        }

        // スキルデータ保存ボタン
        if (GUILayout.Button("スキルデータ保存"))
        {
            // スキル名称が入力されていない場合
            if (uniqueSkillData.skillName == "")
            {
                EditorUtility.DisplayDialog("Error!", string.Format("スキル名称が入力されていません。"), "OK");
                return;
            }

            // 保存確認
            if (!EditorUtility.DisplayDialog("スキルデータ保存確認", string.Format("スキルデータを保存しますか？"), "OK", "CANCEL")) return;

            Debug.Log("rangeSize = " + rangeSize);
            for (int i = 0; i < rangeSize; i++)
            {
                for (int j = 0; j <rangeSize; j++)
                {
                    Debug.Log("movearray = " + moveArray[i, j]);
                }
            }
            // 二次元配列を一次元配列に変換して保存する
            for (int i = 0; i < rangeSize; ++i)
            {
                for (int j = 0; j < rangeSize; ++j)
                {
                    uniqueSkillData.moveSelectArray[i * rangeSize + j] = moveArray[i, j];
                    uniqueSkillData.attackSelectArray[i * rangeSize + j] = selectArray[i, j];
                    uniqueSkillData.attackArray[i * rangeSize + j] = attackArray[i, j];
                }
            }

            SaveUniqueSkillData(uniqueSkillData);
            Close();
        }

        // リセットボタンの配置
        if (GUILayout.Button("リセット"))
        {
            if (EditorUtility.DisplayDialog("リセット確認", string.Format("入力したデータをリセットしますか？"), "OK", "cancel")) Reset();
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
        uniqueSkillData.skillName = "";
        uniqueSkillData.skillCaption = "";
        uniqueSkillData.energy = 1;
        uniqueSkillData.priority = 1;
        uniqueSkillData.skillIcon = null;
        uniqueSkillData.skillAnimation = null;
        uniqueSkillData.moveType = UniqueSkillMoveType.NONE;
        uniqueSkillData.damage = 0;
        uniqueSkillData.power = 0;
        uniqueSkillData.moveSelectArray = new bool[rangeSize * rangeSize];
        uniqueSkillData.attackSelectArray = new bool[rangeSize * rangeSize];
        uniqueSkillData.attackArray = new bool[rangeSize * rangeSize];
        giveBuff = new List<BuffViewData>();
        loseBuff = new List<BuffViewData>();

        // 攻撃範囲の二次元配列の初期化
        for (int i = 0; i < rangeSize; ++i)
        {
            for (int j = 0; j < rangeSize; ++j)
            {
                moveArray[i, j] = false;
                selectArray[i, j] = false;
                attackArray[i, j] = false;
            }
        }

        moveArray[centerIndex.row, centerIndex.column] = true;
        selectArray[centerIndex.row, centerIndex.column] = true;
        attackArray[centerIndex.row, centerIndex.column] = true;
    }

    private void SaveUniqueSkillData(UniqueSkillScriptableObject UniqueSkillScriptableObject)
    {
        const string PATH = "Assets/ScriptableObjects/Skills/";
        string path = PATH + uniqueSkillData.skillName + ".asset";

        // インスタンス化したものをアセットとして保存
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(UniqueSkillScriptableObject));
        if (asset == null)
        {
            // 指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(uniqueSkillData, path);
            Debug.Log(string.Format($"Created new skill, \"{uniqueSkillData.skillName}\"!"));
        }
        else
        {
            // 指定のパスに既に同名のファイルが存在する場合はデータを破棄
            //EditorUtility.CopySerialized(skillData, asset);
            //AssetDatabase.SaveAssets();
            //Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));            
            Debug.Log(string.Format($"\"{uniqueSkillData.skillName}\" has already been created!\n Please Update On Inspector Window!"));
        }
        EditorUtility.SetDirty(uniqueSkillData);
        AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh()
    }
}
