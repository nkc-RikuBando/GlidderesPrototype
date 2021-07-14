using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;


[CustomEditor(typeof(SkillScriptableObject))]
public class SkillScriptableObjectView : Editor
{
    // �U���͈͂̕\���Ɏg�p
    const string DOT = "��";
    const string NONE = "�@";
    const string PLAYER_FALSE = "��";
    const string PLAYER_TRUE = "��";
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
            EditorGUILayout.LabelField("�X�L������");
            EditorGUILayout.LabelField(skillData.skillName, GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("����G�l���M�[");
            EditorGUILayout.LabelField(skillData.energy.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�_���[�W");
            EditorGUILayout.LabelField(skillData.damage.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�D��x");
            EditorGUILayout.LabelField(skillData.priority.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�З�");
            EditorGUILayout.LabelField(skillData.power.ToString(), GUI.skin.textField);
            EditorGUILayout.EndHorizontal();

            FieldIndexOffset[] selectArray = skillData.selectFieldIndexOffsetArray;

            int arrayIndex = 0;
            int rowMin = int.MaxValue, rowMax = int.MinValue;
            int columnMin = int.MaxValue, columnMax = int.MinValue;
            //��int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

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
            //��int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

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
            EditorGUILayout.LabelField("�X�L������");
            skillName = EditorGUILayout.TextField("",skillName);
            if (skillName != skillData.skillName)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("����G�l���M�[");
            energy = EditorGUILayout.IntSlider(energy, 1, 5);
            if (energy != skillData.energy)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�_���[�W");
            damage = EditorGUILayout.IntField(damage);
            if (damage != skillData.damage)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�D��x");
            priority = EditorGUILayout.IntSlider(priority, 1, 10);
            if (priority != skillData.priority)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�З�");
            power = EditorGUILayout.IntSlider(power, 1, 5);
            if (power != skillData.power)
            {
                editCount++;
            }
            EditorGUILayout.EndHorizontal();

            skillIcon = EditorGUILayout.ObjectField("�X�L���A�C�R��", skillIcon, typeof(Sprite), true, GUILayout.Width(224), GUILayout.Height(224)) as Sprite;
            if (skillIcon != skillData.skillIcon)
            {
                editCount++;
            }

            FieldIndexOffset[] selectArray = skillData.selectFieldIndexOffsetArray;

            int arrayIndex = 0;
            int rowMin = int.MaxValue, rowMax = int.MinValue;
            int columnMin = int.MaxValue, columnMax = int.MinValue;
            //��int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

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
            //��int rowMin = 0, rowMax = 12, columnMin = 0, columnMax = 12;

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

        EditorGUILayout.HelpBox("������̏ꍇ�ŕ\������Ă��܂��B\n�オ�I���\�}�X�A�����U���͈͂ł��B\n�I���\�}�X�ɂ����āA���̓L�����N�^�[�̈ʒu��\���܂��B\n�U���͈͂ɂ����āA���͑I�����ꂽ�}�X��\���܂��B\n���h��́��͂��̃}�X��͈͂Ɋ܂܂Ȃ����Ƃ��A\n���h��́��͂��̃}�X��͈͂Ɋ܂ނ��Ƃ�\���܂��B", MessageType.Info);

        if (editFlg && GUILayout.Button("�ۑ�"))
        {
            if (EditorUtility.DisplayDialog("�ۑ��m�F", "�f�[�^��ۑ����܂����H", "OK", "cancel"))
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

        string editButtonCaption = !editFlg ? "�ҏW���[�h�ڍs" : "�ҏW���[�h�I��";
        if (GUILayout.Button(editButtonCaption))
        {
            if (!editFlg)
            {
                if (EditorUtility.DisplayDialog("�ҏW���[�h", "�ҏW���[�h�Ɉڍs���܂����H", "OK", "cancel")) editFlg = true;
            }
            else
            {
                if (editCount > savedEditCount)
                {
                    if (EditorUtility.DisplayDialog("�ۑ�����Ă��Ȃ��ύX", "�ۑ�����Ă��Ȃ��ύX������܂��B\n�ύX��j�����ďI�����܂����H", "OK", "cancel")) editFlg = false;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("�ҏW���[�h", "�ҏW���[�h���I�����܂����H", "OK", "cancel")) editFlg = false;
                }
            }
        }

        if (editFlg && editCount == savedEditCount)
        {
            EditorGUILayout.HelpBox("�f�[�^���ۑ�����܂����B", MessageType.Info);
        }
    }
}
