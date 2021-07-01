using UnityEngine;
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

    // �A�Z�b�g�t�@�C���쐬�p�̃N���X
    SkillDataFileCreator skillDataFileCreator;
    SkillScriptableObject skillData;

    // �t�B�[���h�T�C�Y�̐ݒ�
    static int fieldSize = 7;                           // �ΐ�̃t�B�[���h�T�C�Y
    static int rangeSize = fieldSize * 2 - 1;           // �t�B�[���h�T�C�Y����A�U���͈͓h��ɕK�v�ȃT�C�Y���v�Z

    // �v���C���[�����͂���X�L�����
    string skillName;            // �X�L������
    int energy;                  // �G�l���M�[
    int damage;                  // �_���[�W
    int priority;                // �D��x
    int power;                   // �З�(�_���[�W�t�B�[���h)
    bool[,] range = new bool[rangeSize, rangeSize];     // �U���͈͂��Ǘ�����񎟌��z��

    // �X���C�_�[�̉����E�����ݒ肷��ϐ�
    const int EXTENT_MIN = 0;               // �e��X���C�_�[�̉����ɃA�N�Z�X���邽�߂̓Y����
    const int EXTENT_MAX = 1;               // �e��X���C�_�[�̏���ɃA�N�Z�X���邽�߂̓Y����
    int[] energyExtent = { 0, 5 };          // ����G�l���M�[�̐ݒ�X���C�_�[�̉����E���
    int[] priorityExtent = { 1, 10 };       // �D��x�̐ݒ�X���C�_�[�̉����E���
    int[] toggleSizeExtent = { 10, 30 };    // �U���͈͂̃}�X�T�C�Y�̐ݒ�X���C�_�[�̉����E���
    int[] fieldSizeExtent = { 5, 15 };      // �t�B�[���h�T�C�Y�ύX�̐ݒ�X���C�_�[�̉����E���
    int[] powerExtent = { 0, 5 };           // �З�(�_���[�W�t�B�[���h)�̐ݒ�X���C�_�[�̉����E���

    // �\���E�̍ٗp�̕ϐ�
    bool foldout;                                           // �o�t���X�g���J���邽�߂̕ϐ�
    int beforeFieldSize;                                    // �ύX�O�̃t�B�[���h�T�C�Y(�t�B�[���h�T�C�Y�̕ύX�����m���邽�߂̂���)
    static int rangeToggleSize = 15;                        // �U���͈͂P�}�X�̑傫��
    Vector2 scrollPos;                                      // �U���͈͂̃X�N���[���o�[���W
    Vector2 nameWindowSize = new Vector2(400, 20);          // �X�L�����̂̃E�B���h�E�T�C�Y
    Vector2 energySliderSize = new Vector2(300, 20);        // ����G�l���M�[�̃X���C�_�[�T�C�Y
    Vector2 damageWindowSize = new Vector2(200, 20);        // �_���[�W�̃E�B���h�E�T�C�Y
    Vector2 prioritySliderSize = new Vector2(300, 20);      // �D��x�̃X���C�_�[�T�C�Y
    Vector2 rangeLabelWindowSize = new Vector2(100, 20);    // "�U���͈�"���x���̃E�B���h�E�T�C�Y
    Vector2 toggleSizeSliderSize = new Vector2(300, 20);    // �U���͈͂̃}�X�T�C�Y�ύX�X���C�_�[�̃T�C�Y
    Vector2 fieldSizeSliderSize = new Vector2(300, 20);     // �t�B�[���h�T�C�Y�ύX�X���C�_�[�̃T�C�Y
    Vector2 powerSliderSize = new Vector2(300, 20);         // �З�(�_���[�W�t�B�[���h)�̃X���C�_�[�T�C�Y

    bool initialize = true;                                 // �������̂��߂̕ϐ�

    private void OnGUI()
    {
        if (initialize)
        {
            // �X�L���f�[�^��ۑ�����ScriptableObject�̍쐬
            skillData = ScriptableObject.CreateInstance<SkillScriptableObject>();
            skillDataFileCreator = new SkillDataFileCreator();

            Reset();

            skillData.gridList = new List<FieldIndexOffset>();
            initialize = false;
        }

        // �X�L�����̂̓���
        skillName = EditorGUILayout.TextField("�X�L������", skillName, GUILayout.Width(nameWindowSize.x), GUILayout.Height(nameWindowSize.y));
        skillData.skillName = skillName;

        // ����G�l���M�[�̓���
        energy = EditorGUILayout.IntSlider("����G�l���M�[", energy, energyExtent[EXTENT_MIN], energyExtent[EXTENT_MAX], GUILayout.Width(energySliderSize.x), GUILayout.Height(energySliderSize.y));
        skillData.energy = energy;

        // �_���[�W�̓���
        damage = EditorGUILayout.IntField("�_���[�W", damage, GUILayout.Width(damageWindowSize.x), GUILayout.Height(damageWindowSize.y));
        skillData.damage = damage;

        // �D��x�̓���
        priority = EditorGUILayout.IntSlider("�D��x", priority, priorityExtent[EXTENT_MIN], priorityExtent[EXTENT_MAX], GUILayout.Width(prioritySliderSize.x), GUILayout.Height(prioritySliderSize.y));
        skillData.priority = priority;
        // �D��x�⑫�������x���̕\��
        EditorGUILayout.LabelField(" 1<-------�D��x------->10");
        EditorGUILayout.LabelField("��<-------�s����------->�x");

        EditorGUILayout.Space();                            // �U���͈̓{�b�N�X�Ƃ̗]�����쐬
        EditorGUILayout.BeginVertical(GUI.skin.box);        // �U���͈͂Ɋւ���ݒ���{�b�N�X�ň͂�

        EditorGUILayout.BeginHorizontal();                  // �I���\�}�X�ƍU���͈͂����\�����邽�߂̂���
        EditorGUILayout.BeginVertical(GUI.skin.box);        // �I���\�}�X�Ɋւ�����𒼗�\�����邽�߂̂���

        EditorGUILayout.LabelField("�I���\�}�X", GUILayout.Width(rangeLabelWindowSize.x), GUILayout.Height(rangeLabelWindowSize.y));  // �I���\�}�X���x����\��
        // �\���T�C�Y�ύX�X���C�_�[��\��
        rangeToggleSize = EditorGUILayout.IntSlider("�}�X�̑傫��", rangeToggleSize, toggleSizeExtent[EXTENT_MIN], toggleSizeExtent[EXTENT_MAX], GUILayout.Width(toggleSizeSliderSize.x), GUILayout.Height(toggleSizeSliderSize.y));

        // �З�(�_���[�W�t�B�[���h)�̓���
        power = EditorGUILayout.IntSlider("�З�", power, powerExtent[EXTENT_MIN], powerExtent[EXTENT_MAX], GUILayout.Width(powerSliderSize.x), GUILayout.Height(powerSliderSize.y));
        skillData.power = power;
        
        // �t�B�[���h�T�C�Y�̕ύX
        fieldSize = EditorGUILayout.IntSlider("�t�B�[���h�T�C�Y�̕ύX", fieldSize, fieldSizeExtent[EXTENT_MIN], fieldSizeExtent[EXTENT_MAX], GUILayout.Width(fieldSizeSliderSize.x), GUILayout.Height(fieldSizeSliderSize.y));
        if (fieldSize != beforeFieldSize)               // �t�B�[���h�T�C�Y�̕ύX���������Ȃ�
        {
            beforeFieldSize = fieldSize;                // �t�B�[���h�T�C�Y�̍X�V
            rangeSize = fieldSize * 2 - 1;              // �I���\�}�X�ݒ��ʂ̃T�C�Y���X�V
            range = new bool[rangeSize, rangeSize];     // �I���\�}�X���Ǘ�����z����Đݒ�
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);     // �I���\�}�X�̃X�N���[���o�[�̕\��
        // �I���\�}�X����͂���g�O����\��
        for (int i = 0; i < rangeSize; ++i)
        {
            EditorGUILayout.BeginHorizontal();      // �����\�������邽�߁A����\���̐ݒ������
            for (int j = 0; j < rangeSize; ++j)
            {
                // �͈͂̒����Ȃ�A�v���C���[��\������
                if (i == rangeSize / 2 && j == rangeSize / 2)
                {
                    EditorGUILayout.LabelField("��", GUILayout.Width(rangeToggleSize), GUILayout.Height(rangeToggleSize));
                    continue;
                }
                // �{�^���̌����ڂɕύX�����g�O����\��
                range[i, j] = EditorGUILayout.Toggle("", range[i, j], GUI.skin.button, GUILayout.Width(rangeToggleSize), GUILayout.Height(rangeToggleSize));
            }
            EditorGUILayout.EndHorizontal();        // �����̕\���������������߁A����\���̏I��
        }
        EditorGUILayout.EndScrollView();                            // �U���͈͂̃X�N���[���o�[�͈̔͌���
        EditorGUILayout.EndVertical();       // �I���\�}�X�Ɋւ��钼��\���̏I��
        /*
        EditorGUILayout.BeginVertical(GUI.skin.box);    // �U���͈͂Ɋւ�����𒼗�\�����邽�߂̂���
        EditorGUILayout.LabelField("�U���͈�", GUILayout.Width(rangeLabelWindowSize.x), GUILayout.Height(rangeLabelWindowSize.y));  // �U���͈̓��x����\��
        // �\���T�C�Y�ύX�X���C�_�[��\��
        rangeToggleSize = EditorGUILayout.IntSlider("�}�X�̑傫��", rangeToggleSize, toggleSizeExtent[EXTENT_MIN], toggleSizeExtent[EXTENT_MAX], GUILayout.Width(toggleSizeSliderSize.x), GUILayout.Height(toggleSizeSliderSize.y));

        // �З�(�_���[�W�t�B�[���h)�̓���
        power = EditorGUILayout.IntSlider("�З�", power, powerExtent[EXTENT_MIN], powerExtent[EXTENT_MAX], GUILayout.Width(powerSliderSize.x), GUILayout.Height(powerSliderSize.y));
        skillData.power = power;
        
        // �t�B�[���h�T�C�Y�̕ύX
        fieldSize = EditorGUILayout.IntSlider("�t�B�[���h�T�C�Y�̕ύX", fieldSize, fieldSizeExtent[EXTENT_MIN], fieldSizeExtent[EXTENT_MAX], GUILayout.Width(fieldSizeSliderSize.x), GUILayout.Height(fieldSizeSliderSize.y));
        if (fieldSize != beforeFieldSize)               // �t�B�[���h�T�C�Y�̕ύX���������Ȃ�
        {
            beforeFieldSize = fieldSize;                // �t�B�[���h�T�C�Y�̍X�V
            rangeSize = fieldSize * 2 - 1;              // �U���͈͐ݒ��ʂ̃T�C�Y���X�V
            range = new bool[rangeSize, rangeSize];     // �U���͈͂��Ǘ�����z����Đݒ�
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);     // �U���͈͂̃X�N���[���o�[�̕\��
        // �U���͈͂���͂���g�O����\��
        for (int i = 0; i < rangeSize; ++i)
        {
            EditorGUILayout.BeginHorizontal();      // �����\�������邽�߁A����\���̐ݒ������
            for (int j = 0; j < rangeSize; ++j)
            {
                // �͈͂̒����Ȃ�A�v���C���[��\������
                if (i == rangeSize / 2 && j == rangeSize / 2)
                {
                    EditorGUILayout.LabelField("��", GUILayout.Width(rangeToggleSize), GUILayout.Height(rangeToggleSize));
                    continue;
                }
                // �{�^���̌����ڂɕύX�����g�O����\��
                range[i, j] = EditorGUILayout.Toggle("", range[i, j], GUI.skin.button, GUILayout.Width(rangeToggleSize), GUILayout.Height(rangeToggleSize));
            }
            EditorGUILayout.EndHorizontal();        // �����̕\���������������߁A����\���̏I��
        }
        EditorGUILayout.EndScrollView();            // �U���͈͂̃X�N���[���o�[�͈̔͌���
        EditorGUILayout.EndVertical();       // �I���\�}�X�Ɋւ��钼��\���̏I��

        EditorGUILayout.EndVertical();      // �U���͈͂̃{�b�N�X�����
        */
        // �X�L���f�[�^�ۑ��{�^��
        if (GUILayout.Button("�X�L���f�[�^�ۑ�"))
        {
            // �X�L�����̂����͂���Ă��Ȃ��ꍇ
            if (skillName == "")
            {
                EditorUtility.DisplayDialog("Error!", string.Format("�X�L�����̂����͂���Ă��܂���B"), "OK");
                return;
            }
            // �U���͈͂�1�}�X�����݂��Ȃ��ꍇ
            bool rangeError = false;
            foreach (bool flg in range)
            {
                rangeError = flg;
                if (rangeError) break;
            }
            if (!rangeError)
            {
                EditorUtility.DisplayDialog("Error!", string.Format("�U���͈͂��ݒ肳��Ă��܂���B"), "OK");
                return;
            }

            // �ۑ��m�F
            if (!EditorUtility.DisplayDialog("�X�L���f�[�^�ۑ��m�F", string.Format("�X�L���f�[�^��ۑ����܂����H"), "OK", "CANCEL")) return;

            // �U���͈͂̃O���b�h���X�g�̍쐬
            int centerIndex = rangeSize / 2;
            for (int i = 0; i < rangeSize; ++i)
            {
                for (int j = 0; j < rangeSize; ++j)
                {
                    if (range[i, j]) skillData.gridList.Add(new FieldIndexOffset(i, j, centerIndex, centerIndex));
                }
            }
            
            // �f�[�^�ۑ����\�b�h�̌Ăяo��
            skillDataFileCreator.CreateSkillScriptableObject(skillData);

            // �E�B���h�E�����
            Close();
        }

        // ���ӏ����̕\��
        EditorGUILayout.HelpBox("�t�B�[���h�T�C�Y��ύX����ƁA�I���\�}�X����эU���͈͂̓��Z�b�g����܂��B", MessageType.Warning);

        // ���Z�b�g�{�^���̔z�u
        if (GUILayout.Button("���Z�b�g"))
        {
            if (EditorUtility.DisplayDialog("���Z�b�g�m�F", string.Format("���͂����f�[�^�����Z�b�g���܂����H"), "OK", "CANCEL")) Reset();
        }
    }

    private void Reset()
    {
        // �e��ϐ��̏�����
        skillName = "";
        energy = 0;
        damage = 0;
        priority = 1;
        power = 0;

        // �U���͈͂̓񎟌��z��̏�����
        for (int i = 0; i < rangeSize; ++i)
        {
            for (int j = 0; j < rangeSize; ++j)
            {
                range[i, j] = false;
            }
        }
    }
}