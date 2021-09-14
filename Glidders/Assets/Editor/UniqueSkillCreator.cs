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
    bool isAttack = false;

    // �t�B�[���h�T�C�Y�̐ݒ�
    static int fieldSize = 11;                           // �ΐ�̃t�B�[���h�T�C�Y
    static int rangeSize = fieldSize * 2 - 1;           // �t�B�[���h�T�C�Y����A�U���͈͓h��ɕK�v�ȃT�C�Y���v�Z
    FieldIndex centerIndex = new FieldIndex(rangeSize / 2, rangeSize / 2);   // �v���C���[�̈ʒu�ƂȂ�A�͈͂̒��S

    int selectRangeToggleSize = 15;
    int width = 300;
    Vector2 skillIconSize = new Vector2(224, 224);            // �X�L���A�C�R���̃T�C�Y

    bool[,] moveArray = new bool[rangeSize, rangeSize]; 
    bool[,] selectArray = new bool[rangeSize, rangeSize]; 
    bool[,] attackArray = new bool[rangeSize, rangeSize];
    bool[,] moveArrayBefore = new bool[rangeSize, rangeSize];
    bool[,] selectArrayBefore = new bool[rangeSize, rangeSize];
    bool[,] attackArrayBefore = new bool[rangeSize, rangeSize];
    List<FieldIndexOffset> moveRange = new List<FieldIndexOffset>();             // �ړ�����Ǘ�����񎟌��z��
    List<FieldIndexOffset> selectRange = new List<FieldIndexOffset>();           // �I���\�}�X���Ǘ�����񎟌��z��
    List<FieldIndexOffset> attackRange = new List<FieldIndexOffset>();           // �U���͈͂��Ǘ�����񎟌��z��

    [MenuItem("Window/UniqueSkillCreator")]
    static void Open()
    {
        EditorWindow.GetWindow<UniqueSkillCreator>("UniqueSkillCreator");
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

        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope())
            {
                // ����
                uniqueSkillData.skillName = EditorGUILayout.TextField("����", uniqueSkillData.skillName, GUILayout.Width(width));

                // �G�l���M�[
                using (new GUILayout.HorizontalScope())
                {
                    uniqueSkillData.energy = EditorGUILayout.IntSlider("�G�l���M�[", uniqueSkillData.energy, 1, 5, GUILayout.Width(width));
                }

                // �D��x
                using (new GUILayout.HorizontalScope())
                {
                    uniqueSkillData.priority = EditorGUILayout.IntSlider("�D��x", uniqueSkillData.priority, 1, 10, GUILayout.Width(width));
                }

                EditorGUILayout.Space();

                // �ړ��̎��
                uniqueSkillData.moveType = (UniqueSkillMoveType)EditorGUILayout.EnumPopup("�ړ��̎��", uniqueSkillData.moveType, GUILayout.Width(width));
                EditorGUILayout.LabelField("�@�@�@�@�@�@�@NONE �c�ړ����Ȃ�");
                EditorGUILayout.LabelField("�@�@�@�@�@�@�@FREE �c�ʏ�ړ�");
                EditorGUILayout.LabelField("�@�@�@�@�@�@�@FIXED�c�Œ�ړ�");

                // �U�����邩�ǂ���
                isAttack = EditorGUILayout.Toggle("�U�����s��", isAttack);
                uniqueSkillData.isAttack = isAttack;

                // �_���[�W
                uniqueSkillData.damage = EditorGUILayout.IntField("�_���[�W", uniqueSkillData.damage, GUILayout.Width(width));

                // �З�
                using (new GUILayout.HorizontalScope())
                {
                    uniqueSkillData.power = EditorGUILayout.IntSlider("�З�(�_���[�W�t�B�[���h)", uniqueSkillData.power, 1, 5, GUILayout.Width(width));
                }
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                uniqueSkillData.skillIcon = EditorGUILayout.ObjectField("�X�L���A�C�R��", uniqueSkillData.skillIcon, typeof(Sprite), true, GUILayout.Width(skillIconSize.x), GUILayout.Height(skillIconSize.y)) as Sprite;
                uniqueSkillData.skillAnimation = EditorGUILayout.ObjectField("�A�j���[�V�����N���b�v", uniqueSkillData.skillAnimation, typeof(AnimationClip), true) as AnimationClip;
            }
        }

        EditorGUILayout.Space();
        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("�ړ���}�X");
                    EditorGUILayout.Space();

                    // �ړ���}�X
                    // �I���\�}�X����͂���g�O����\��
                    for (int i = 0; i < rangeSize; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();      // �����\�������邽�߁A����\���̐ݒ������
                        for (int j = 0; j < rangeSize; ++j)
                        {
                            // �͈͂̒����Ȃ�A�v���C���[��\������
                            if (i == centerIndex.row && j == centerIndex.column)
                            {
                                moveArray[i, j] = EditorGUILayout.Toggle("", moveArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                                // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                                if (moveArray[i, j] != moveArrayBefore[i, j])
                                {
                                    SetArrayData(new FieldIndexOffset(i, j), "Move");
                                }
                                moveArrayBefore[i, j] = moveArray[i, j];
                                continue;
                            }
                            // �{�^���̌����ڂɕύX�����g�O����\��
                            moveArray[i, j] = EditorGUILayout.Toggle("", moveArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                            if (moveArray[i, j] != moveArrayBefore[i, j])
                            {
                                SetArrayData(new FieldIndexOffset(i, j), "Move");
                            }
                            moveArrayBefore[i, j] = moveArray[i, j];
                        }
                        EditorGUILayout.EndHorizontal();        // �����̕\���������������߁A����\���̏I��
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("�Œ�ړ��̏ꍇ�͈ړ��̏I�_��ݒ肵�Ă��������B\n���g�̃}�X���I���\�Ȃ璆�S�̃`�F�b�N�����Ă��������B", MessageType.Warning);
                }
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("�U���I���\�}�X");
                    EditorGUILayout.Space();

                    // �I���\�}�X����͂���g�O����\��
                    for (int i = 0; i < rangeSize; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();      // �����\�������邽�߁A����\���̐ݒ������
                        for (int j = 0; j < rangeSize; ++j)
                        {
                            // �͈͂̒����Ȃ�A�v���C���[��\������
                            if (i == centerIndex.row && j == centerIndex.column)
                            {
                                selectArray[i, j] = EditorGUILayout.Toggle("", selectArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                                // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                                if (selectArray[i, j] != selectArrayBefore[i, j])
                                {
                                    SetArrayData(new FieldIndexOffset(i, j), "Select");
                                }
                                selectArrayBefore[i, j] = selectArray[i, j];
                                continue;
                            }
                            // �{�^���̌����ڂɕύX�����g�O����\��
                            selectArray[i, j] = EditorGUILayout.Toggle("", selectArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                            if (selectArray[i, j] != selectArrayBefore[i, j])
                            {
                                SetArrayData(new FieldIndexOffset(i, j), "Select");
                            }
                            selectArrayBefore[i, j] = selectArray[i, j];
                        }
                        EditorGUILayout.EndHorizontal();        // �����̕\���������������߁A����\���̏I��
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("������̏ꍇ�œ��͂��Ă��������B\n���g�̃}�X���I���\�Ȃ璆�S�̃`�F�b�N�����Ă��������B", MessageType.Warning);
                }
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("�U���͈̓}�X");
                    EditorGUILayout.Space();

                    // �U���}�X����͂���g�O����\��
                    for (int i = 0; i < rangeSize; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();      // �����\�������邽�߁A����\���̐ݒ������
                        for (int j = 0; j < rangeSize; ++j)
                        {
                            // �͈͂̒����Ȃ�A�v���C���[��\������
                            if (i == centerIndex.row && j == centerIndex.column)
                            {
                                attackArray[i, j] = EditorGUILayout.Toggle("", attackArray[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                                // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                                if (attackArray[i, j] != attackArrayBefore[i, j])
                                {
                                    SetArrayData(new FieldIndexOffset(i, j), "Attack");
                                }
                                attackArrayBefore[i, j] = attackArray[i, j];
                                continue;
                            }
                            // �{�^���̌����ڂɕύX�����g�O����\��
                            attackArray[i, j] = EditorGUILayout.Toggle("", attackArray[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                            // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                            if (attackArray[i, j] != attackArrayBefore[i, j])
                            {
                                SetArrayData(new FieldIndexOffset(i, j), "Attack");
                            }
                            attackArrayBefore[i, j] = attackArray[i, j];
                        }
                        EditorGUILayout.EndHorizontal();        // �����̕\���������������߁A����\���̏I��
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("������̏ꍇ�œ��͂��Ă��������B\n���S�̃`�F�b�N�}�X�͑I���\�}�X�őI�΂ꂽ�}�X�ƑΉ����܂��B\n���g�̃}�X���U���͈͂Ȃ璆�S�̃`�F�b�N�����Ă��������B", MessageType.Warning);
                }
            }
        }
    }

    private void Reset()
    {
        /*
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
        */

        // �U���͈͂̓񎟌��z��̏�����
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
