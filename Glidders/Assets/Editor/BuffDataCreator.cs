using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders.Buff;
using Glidders.Character;

public class BuffDataCreator : EditorWindow
{
    [MenuItem("Window/BuffDataCreator")]
    static void Open()
    {
        EditorWindow.GetWindow<BuffDataCreator>("BuffDataCreator");
    }

    List<BuffValueData> buffValueDataList;
    BuffViewData buffViewData;

    bool initialize = true;
    int buffValueCount = 0;

    string buffName;
    string buffCaption;
    Sprite buffIcon;

    List<StatusTypeEnum> buffStatusList;
    List<int> buffTypeIndexList;
    List<float> buffScaleList;
    List<int> buffDurationList;

    private string[] signArray = { "+", "*" };

    private void OnGUI()
    {
        if (initialize)
        {
            // �X�L���f�[�^��ۑ�����ScriptableObject�̍쐬
            buffValueDataList = new List<BuffValueData>();
            buffViewData = ScriptableObject.CreateInstance<BuffViewData>();
            buffViewData.buffValueList = new List<BuffValueData>();

            Reset();

            initialize = false;
        }

        using (new EditorGUILayout.VerticalScope())
        {
            // �o�t�̕\���Ɋւ������ݒ�
            buffIcon = EditorGUILayout.ObjectField("�o�t�A�C�R��", buffIcon, typeof(Sprite), true) as Sprite;
            buffViewData.buffIcon = buffIcon;

            buffName = EditorGUILayout.TextField("�o�t����", buffName);
            buffViewData.buffName = buffName;

            buffCaption = EditorGUILayout.TextField("�o�t������", buffCaption);
            buffViewData.buffCaption = buffCaption;

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                int indexWidth = 20;
                int popupWidth = 40;
                int enumFieldWidth = 100;
                int scaleFieldWidth = 60;
                int durationFieldWidth = 70;

                using (new EditorGUILayout.HorizontalScope())
                {
                    // ���ۂɑ�������X�e�[�^�X�̌��o����\��
                    EditorGUILayout.LabelField(" ", GUILayout.Width(indexWidth));

                    EditorGUILayout.LabelField("�o�t����X�e�[�^�X", GUILayout.Width(enumFieldWidth));

                    EditorGUILayout.LabelField("�{��/���Z", GUILayout.Width(popupWidth + scaleFieldWidth));

                    EditorGUILayout.LabelField("�p���^�[����", GUILayout.Width(durationFieldWidth));
                }

                for (int i = 0; i < buffValueCount; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // ���ۂɑ�������X�e�[�^�X��ݒ�
                        EditorGUILayout.LabelField(string.Format($"{i + 1}"), GUILayout.Width(indexWidth));

                        buffStatusList[i] = (StatusTypeEnum)EditorGUILayout.EnumPopup("", buffStatusList[i], GUILayout.Width(enumFieldWidth));
                        buffValueDataList[i].buffedStatus = buffStatusList[i];

                        buffTypeIndexList[i] = EditorGUILayout.Popup(buffTypeIndexList[i], signArray, GUILayout.Width(popupWidth));
                        buffValueDataList[i].buffType = (BuffTypeEnum)buffTypeIndexList[i];

                        buffScaleList[i] = EditorGUILayout.FloatField("", buffScaleList[i], GUILayout.Width(scaleFieldWidth));
                        buffValueDataList[i].buffScale = buffScaleList[i];

                        buffDurationList[i] = EditorGUILayout.IntField("", buffDurationList[i], GUILayout.Width(durationFieldWidth));
                        buffValueDataList[i].buffDuration = buffDurationList[i];

                        // �o�t�f�[�^��2���ȏ゠��Ȃ�-�{�^����\��
                        if (buffValueCount >= 2)
                        {
                            // -�{�^���������ꂽ�Ƃ��A���̃o�t���폜����
                            if (GUILayout.Button("-", GUILayout.Width(indexWidth)))
                            {
                                RemoveDataFromAllList(i);
                            }
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(" ", GUILayout.Width(indexWidth));

                    // +�{�^���������ꂽ�Ƃ��A�o�t����X�e�[�^�X��ǉ�����
                    if (GUILayout.Button("+", GUILayout.Width(indexWidth)))
                    {
                        AddDataToAllList();
                    }
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("�ۑ�"))
            {
                CreateBuffData();
            }
        }
    }

    /// <summary>
    /// �S�Ẵ��X�g�ɗv�f����ǉ�����
    /// </summary>
    private void AddDataToAllList()
    {
        buffValueCount++;

        buffValueDataList.Add(ScriptableObject.CreateInstance<BuffValueData>());
        buffStatusList.Add(StatusTypeEnum.DAMAGE);
        buffTypeIndexList.Add(1);
        buffScaleList.Add(1.0f);
        buffDurationList.Add(1);
    }

    /// <summary>
    /// �S�Ẵ��X�g�̗v�f����폜����
    /// </summary>
    private void RemoveDataFromAllList(int removeIndex)
    {
        buffValueCount--;

        buffValueDataList.RemoveAt(removeIndex);
        buffStatusList.RemoveAt(removeIndex);
        buffTypeIndexList.RemoveAt(removeIndex);
        buffScaleList.RemoveAt(removeIndex);
        buffDurationList.RemoveAt(removeIndex);
    }

    private void Reset()
    {
        buffValueCount = 0;

        buffName = "";
        buffCaption = "";
        buffIcon = null;

        buffStatusList = new List<StatusTypeEnum>();
        buffTypeIndexList = new List<int>();
        buffScaleList = new List<float>();
        buffDurationList = new List<int>();

        AddDataToAllList();
    }

    public void CreateBuffData()
    {
        const string PATH = "Assets/ScriptableObjects/Buffs/";

        for (int i = 0; i < buffValueCount; i++)
        {
            string valuePath = PATH + string.Format("{0}_{1:00}", buffViewData.buffName, i) + ".asset";

            // �C���X�^���X���������̂��A�Z�b�g�Ƃ��ĕۑ�
            var valueAsset = AssetDatabase.LoadAssetAtPath(valuePath, typeof(BuffValueData));
            if (valueAsset == null)
            {
                // �w��̃p�X�Ƀt�@�C�������݂��Ȃ��ꍇ�͐V�K�쐬
                AssetDatabase.CreateAsset(buffValueDataList[i], valuePath);
                //Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
            }
            else
            {
                // �w��̃p�X�Ɋ��ɓ����̃t�@�C�������݂���ꍇ�̓f�[�^���X�V
                EditorUtility.CopySerialized(buffValueDataList[i], valueAsset);
                AssetDatabase.SaveAssets();
                //Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));            
                //Debug.Log(string.Format($"\"{skillData.skillName}\" has already been created!\n Please Update On Inspector Window!"));
            }
            EditorUtility.SetDirty(buffValueDataList[i]);
            AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            // BuffViewData�̃��X�g�ɐ�������ValueData��ǉ�
            buffViewData.buffValueList.Add(buffValueDataList[i]);
        }

        string path = PATH + string.Format("{0}", buffViewData.buffName) + ".asset";

        // �C���X�^���X���������̂��A�Z�b�g�Ƃ��ĕۑ�
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(BuffViewData));
        if (asset == null)
        {
            // �w��̃p�X�Ƀt�@�C�������݂��Ȃ��ꍇ�͐V�K�쐬
            AssetDatabase.CreateAsset(buffViewData, path);
            //Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
        }
        else
        {
            // �w��̃p�X�Ɋ��ɓ����̃t�@�C�������݂���ꍇ�̓f�[�^���X�V
            EditorUtility.CopySerialized(buffViewData, asset);
            AssetDatabase.SaveAssets();
            //Debug.Log(string.Format($"Updated \"{skillData.skillName}\"!"));            
            //Debug.Log(string.Format($"\"{skillData.skillName}\" has already been created!\n Please Update On Inspector Window!"));
        }
        EditorUtility.SetDirty(buffViewData);
        AssetDatabase.SaveAssets();

        Debug.Log("Create new buff data!");
        //AssetDatabase.Refresh();
    }
}
