using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glidders;
using Glidders.Character;
using Glidders.Buff;

public class UniqueSkillCreator : EditorWindow
{
    // �A�Z�b�g�t�@�C���쐬�p�̃N���X
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
            // �X�L���f�[�^��ۑ�����ScriptableObject�̍쐬
            uniqueSkillData = ScriptableObject.CreateInstance<UniqueSkillScriptableObject>();

            Reset();

            //��skillData.gridList = new List<FieldIndexOffset>();
            initialize = false;
        }

        // ����
        uniqueSkillData.skillName = EditorGUILayout.TextField("����", uniqueSkillData.skillName);

        // �G�l���M�[
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("�G�l���M�[");
            uniqueSkillData.energy = EditorGUILayout.IntSlider(uniqueSkillData.energy, 1, 5);
        }

        // �D��x
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("�D��x");
            uniqueSkillData.priority = EditorGUILayout.IntSlider(uniqueSkillData.priority, 1, 10);
        }

        EditorGUILayout.Space();

        // �ړ��̎��
        uniqueSkillData.moveType = (UniqueSkillMoveType)EditorGUILayout.EnumPopup("�ړ��̎��", uniqueSkillData.moveType);
        EditorGUILayout.LabelField("NONE �c�ړ����Ȃ�");
        EditorGUILayout.LabelField("FREE �c�ʏ�ړ�");
        EditorGUILayout.LabelField("FIXED�c�Œ�ړ�");

        // �ړ���}�X
        EditorGUILayout.LabelField("�撣���č��܂�");

        EditorGUILayout.Space();

        // �U�����邩�ǂ���
        //uniqueSkillData.isAttack

        // �_���[�W
        uniqueSkillData.damage = EditorGUILayout.IntField("�_���[�W", uniqueSkillData.damage);

        // �З�
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("�З�(�_���[�W�t�B�[���h)");
            uniqueSkillData.power = EditorGUILayout.IntSlider(uniqueSkillData.power, 1, 5);
        }
    }

    private void Reset()
    {/*
        // �e��ϐ��̏�����
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

        // �U���͈͂̓񎟌��z��̏�����
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
