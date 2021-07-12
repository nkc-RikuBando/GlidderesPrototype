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

    // �A�Z�b�g�t�@�C���쐬�p�̃N���X
    SkillDataFileCreator skillDataFileCreator;
    SkillScriptableObject skillData;
    Sprite skillIcon;

    // �t�B�[���h�T�C�Y�̐ݒ�
    static int fieldSize = 7;                           // �ΐ�̃t�B�[���h�T�C�Y
    static int rangeSize = fieldSize * 2 - 1;           // �t�B�[���h�T�C�Y����A�U���͈͓h��ɕK�v�ȃT�C�Y���v�Z
    FieldIndex centerIndex = new FieldIndex(rangeSize / 2, rangeSize / 2);   // �v���C���[�̈ʒu�ƂȂ�A�͈͂̒��S

    // �v���C���[�����͂���X�L�����
    string skillName;            // �X�L������
    int energy;                  // �G�l���M�[
    int damage;                  // �_���[�W
    int priority;                // �D��x
    int power;                   // �З�(�_���[�W�t�B�[���h)
    bool[,] selectRange = new bool[rangeSize, rangeSize];           // �I���\�}�X���Ǘ�����񎟌��z��
    bool[,] attackRange = new bool[rangeSize, rangeSize];           // �U���͈͂��Ǘ�����񎟌��z��
    bool[,] selectRangeBefore = new bool[rangeSize, rangeSize];     // �I���\�}�X���Ǘ�����񎟌��z��̕ύX�����m���邽�߂̂���
    bool[,] attackRangeBefore = new bool[rangeSize, rangeSize];     // �U���͈͂��Ǘ�����񎟌��z��̕ύX�����m���邽�߂̂���

    // �X���C�_�[�̉����E�����ݒ肷��ϐ�
    const int EXTENT_MIN = 0;               // �e��X���C�_�[�̉����ɃA�N�Z�X���邽�߂̓Y����
    const int EXTENT_MAX = 1;               // �e��X���C�_�[�̏���ɃA�N�Z�X���邽�߂̓Y����
    int[] energyExtent = { 0, 5 };          // ����G�l���M�[�̐ݒ�X���C�_�[�̉����E���
    int[] priorityExtent = { 1, 10 };       // �D��x�̐ݒ�X���C�_�[�̉����E���
    int[] toggleSizeExtent = { 10, 30 };    // �U���͈͂̃}�X�T�C�Y�̐ݒ�X���C�_�[�̉����E���
    int[] fieldSizeExtent = { 5, 15 };      // �t�B�[���h�T�C�Y�ύX�̐ݒ�X���C�_�[�̉����E���
    int[] powerExtent = { 0, 5 };           // �З�(�_���[�W�t�B�[���h)�̐ݒ�X���C�_�[�̉����E���

    // �\���E�̍ٗp�̕ϐ�
    //bool foldout;                                           // �o�t���X�g���J���邽�߂̕ϐ�
    int beforeFieldSize;                                    // �ύX�O�̃t�B�[���h�T�C�Y(�t�B�[���h�T�C�Y�̕ύX�����m���邽�߂̂���)
    static int selectRangeToggleSize = 15;                        // �U���͈͂P�}�X�̑傫��
    Vector2 scrollPos;                                      // �U���͈͂̃X�N���[���o�[���W
    Vector2 nameWindowSize = new Vector2(400, 20);          // �X�L�����̂̃E�B���h�E�T�C�Y
    Vector2 energySliderSize = new Vector2(300, 20);        // ����G�l���M�[�̃X���C�_�[�T�C�Y
    Vector2 damageWindowSize = new Vector2(200, 20);        // �_���[�W�̃E�B���h�E�T�C�Y
    Vector2 prioritySliderSize = new Vector2(300, 20);      // �D��x�̃X���C�_�[�T�C�Y
    Vector2 skillIconSize = new Vector2(224, 224);            // �X�L���A�C�R���̃T�C�Y
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

            //��skillData.gridList = new List<FieldIndexOffset>();
            initialize = false;
        }

        // �X�L���A�C�R���ƕ���\�����邽�߂̂���
        EditorGUILayout.BeginHorizontal();

        // �X�L���A�C�R���ȊO�𒼗�\��
        EditorGUILayout.BeginVertical();
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
        // ����\���̏I��
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        skillIcon = EditorGUILayout.ObjectField("�X�L���A�C�R��", skillIcon, typeof(Sprite), true, GUILayout.Width(skillIconSize.x), GUILayout.Height(skillIconSize.y)) as Sprite;
        EditorGUILayout.EndVertical();
        // ����\���̏I��
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();                            // �U���͈̓{�b�N�X�Ƃ̗]�����쐬

        EditorGUILayout.BeginHorizontal();                  // �I���\�}�X�ƍU���͈͂����\�����邽�߂̂���
        EditorGUILayout.BeginVertical(GUI.skin.box);        // �I���\�}�X�Ɋւ�����𒼗�\�����邽�߂̂���

        EditorGUILayout.LabelField("�I���\�}�X", GUILayout.Width(rangeLabelWindowSize.x), GUILayout.Height(rangeLabelWindowSize.y));  // �I���\�}�X���x����\��
        // �\���T�C�Y�ύX�X���C�_�[��\��
        selectRangeToggleSize = EditorGUILayout.IntSlider("�}�X�̑傫��", selectRangeToggleSize, toggleSizeExtent[EXTENT_MIN], toggleSizeExtent[EXTENT_MAX], GUILayout.Width(toggleSizeSliderSize.x), GUILayout.Height(toggleSizeSliderSize.y));

        // �З�(�_���[�W�t�B�[���h)�̓���
        power = EditorGUILayout.IntSlider("�З�", power, powerExtent[EXTENT_MIN], powerExtent[EXTENT_MAX], GUILayout.Width(powerSliderSize.x), GUILayout.Height(powerSliderSize.y));
        skillData.power = power;
        
        // �t�B�[���h�T�C�Y�̕ύX
        fieldSize = EditorGUILayout.IntSlider("�t�B�[���h�T�C�Y�̕ύX", fieldSize, fieldSizeExtent[EXTENT_MIN], fieldSizeExtent[EXTENT_MAX], GUILayout.Width(fieldSizeSliderSize.x), GUILayout.Height(fieldSizeSliderSize.y));
        if (fieldSize != beforeFieldSize)               // �t�B�[���h�T�C�Y�̕ύX���������Ȃ�
        {
            beforeFieldSize = fieldSize;                // �t�B�[���h�T�C�Y�̍X�V
            rangeSize = fieldSize * 2 - 1;              // �U���͈͐ݒ��ʂ̃T�C�Y���X�V
            selectRange = new bool[rangeSize, rangeSize];     // �e�z��̃T�C�Y���Đݒ肷��
            attackRange = new bool[rangeSize, rangeSize];
            selectRangeBefore = new bool[rangeSize, rangeSize];
            attackRangeBefore = new bool[rangeSize, rangeSize];
            skillData.selectGridArray = new bool[rangeSize * rangeSize];
            skillData.attackGridArray = new bool[rangeSize * rangeSize];
            centerIndex.row = rangeSize / 2; centerIndex.column = rangeSize / 2;  // ���S�ʒu���Đݒ�
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);     // �I���\�}�X�̃X�N���[���o�[�̕\��
        // �I���\�}�X����͂���g�O����\��
        for (int i = 0; i < rangeSize; ++i)
        {
            EditorGUILayout.BeginHorizontal();      // �����\�������邽�߁A����\���̐ݒ������
            for (int j = 0; j < rangeSize; ++j)
            {
                // �͈͂̒����Ȃ�A�v���C���[��\������
                if (i == centerIndex.row && j == centerIndex.column)
                {
                    selectRange[i, j] = EditorGUILayout.Toggle("", selectRange[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                    // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                    if (selectRange[i, j] != selectRangeBefore[i, j])
                    {
                        SetArrayData(new FieldIndex(i, j), selectRange[i, j], "Select");
                    }
                    selectRangeBefore[i, j] = selectRange[i, j];
                    continue;
                }
                // �{�^���̌����ڂɕύX�����g�O����\��
                selectRange[i, j] = EditorGUILayout.Toggle("", selectRange[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                if (selectRange[i, j] != selectRangeBefore[i, j])
                {
                    SetArrayData(new FieldIndex(i, j), selectRange[i, j], "Select");
                }
                selectRangeBefore[i, j] = selectRange[i, j];
            }
            EditorGUILayout.EndHorizontal();        // �����̕\���������������߁A����\���̏I��
        }
        EditorGUILayout.EndScrollView();                            // �I���\�}�X�̃X�N���[���o�[�͈̔͌���
        EditorGUILayout.HelpBox("������̏ꍇ�Őݒ肵�Ă��������B\n�`�F�b�N�{�b�N�X�����S�i�L�����N�^�[�̈ʒu�j�ł��B\n���S�}�X���܂߂�ꍇ�̓`�F�b�N�����Ă��������B", MessageType.None);
        EditorGUILayout.EndVertical();       // �I���\�}�X�Ɋւ��钼��\���̏I��

        EditorGUILayout.BeginVertical(GUI.skin.box);    // �U���͈͂Ɋւ�����𒼗�\�����邽�߂̂���
        EditorGUILayout.LabelField("�U���͈�", GUILayout.Width(rangeLabelWindowSize.x), GUILayout.Height(rangeLabelWindowSize.y));  // �U���͈̓��x����\��
        // �\���T�C�Y�ύX�X���C�_�[��\��
        selectRangeToggleSize = EditorGUILayout.IntSlider("�}�X�̑傫��", selectRangeToggleSize, toggleSizeExtent[EXTENT_MIN], toggleSizeExtent[EXTENT_MAX], GUILayout.Width(toggleSizeSliderSize.x), GUILayout.Height(toggleSizeSliderSize.y));

        // �З�(�_���[�W�t�B�[���h)�̓���
        power = EditorGUILayout.IntSlider("�З�", power, powerExtent[EXTENT_MIN], powerExtent[EXTENT_MAX], GUILayout.Width(powerSliderSize.x), GUILayout.Height(powerSliderSize.y));
        skillData.power = power;
        
        // �t�B�[���h�T�C�Y�̕ύX
        fieldSize = EditorGUILayout.IntSlider("�t�B�[���h�T�C�Y�̕ύX", fieldSize, fieldSizeExtent[EXTENT_MIN], fieldSizeExtent[EXTENT_MAX], GUILayout.Width(fieldSizeSliderSize.x), GUILayout.Height(fieldSizeSliderSize.y));
        if (fieldSize != beforeFieldSize)               // �t�B�[���h�T�C�Y�̕ύX���������Ȃ�
        {
            beforeFieldSize = fieldSize;                // �t�B�[���h�T�C�Y�̍X�V
            rangeSize = fieldSize * 2 - 1;              // �U���͈͐ݒ��ʂ̃T�C�Y���X�V
            selectRange = new bool[rangeSize, rangeSize];     // �e�z��̃T�C�Y���Đݒ肷��
            attackRange = new bool[rangeSize, rangeSize];
            selectRangeBefore = new bool[rangeSize, rangeSize];
            attackRangeBefore = new bool[rangeSize, rangeSize];
            skillData.selectGridArray = new bool[rangeSize * rangeSize];
            skillData.attackGridArray = new bool[rangeSize * rangeSize];
            centerIndex.row = rangeSize / 2; centerIndex.column = rangeSize / 2;  // ���S�ʒu���Đݒ�
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);     // �U���͈͂̃X�N���[���o�[�̕\��
        // �U���͈͂���͂���g�O����\��
        for (int i = 0; i < rangeSize; ++i)
        {
            EditorGUILayout.BeginHorizontal();      // �����\�������邽�߁A����\���̐ݒ������
            for (int j = 0; j < rangeSize; ++j)
            {
                // �͈͂̒����Ȃ�A�v���C���[��\������
                if (i == centerIndex.row && j == centerIndex.column)
                {
                    attackRange[i, j] = EditorGUILayout.Toggle("", attackRange[i, j], GUI.skin.toggle, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                    // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                    if (attackRange[i, j] != attackRangeBefore[i, j])
                    {
                        SetArrayData(new FieldIndex(i, j), attackRange[i, j], "Attack");
                    }
                    attackRangeBefore[i, j] = attackRange[i, j];
                    continue;
                }
                // �{�^���̌����ڂɕύX�����g�O����\��
                attackRange[i, j] = EditorGUILayout.Toggle("", attackRange[i, j], GUI.skin.button, GUILayout.Width(selectRangeToggleSize), GUILayout.Height(selectRangeToggleSize));
                // �g�O���̒l���X�V���ꂽ�ꍇ�A���X�g�̓��e���X�V����
                if (attackRange[i, j] != attackRangeBefore[i, j])
                {
                    SetArrayData(new FieldIndex(i, j), attackRange[i, j], "Attack");
                }
                attackRangeBefore[i, j] = attackRange[i, j];
            }
            EditorGUILayout.EndHorizontal();        // �����̕\���������������߁A����\���̏I��
        }
        EditorGUILayout.EndScrollView();            // �U���͈͂̃X�N���[���o�[�͈̔͌���]
        EditorGUILayout.HelpBox("������̏ꍇ�Őݒ肵�Ă��������B\n�`�F�b�N�{�b�N�X�����S�i�v���C���[���I�������ʒu�j�ł��B\n���S�}�X���܂߂�ꍇ�̓`�F�b�N�����Ă��������B", MessageType.None);
        EditorGUILayout.EndVertical();              // �U���͈͂Ɋւ��钼��\���̏I��

        EditorGUILayout.EndHorizontal();            // �I���\�}�X�ƍU���͈͂̕���\���̏I��

        // ���ӏ����̕\��
        EditorGUILayout.HelpBox("�t�B�[���h�T�C�Y��ύX����ƁA�I���\�}�X����эU���͈͂̓��Z�b�g����܂��B", MessageType.Warning);

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
            foreach (bool flg in selectRange)
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

            /*��// �U���͈͂̃O���b�h���X�g�̍쐬
            int centerIndex = rangeSize / 2;
            for (int i = 0; i < rangeSize; ++i)
            {
                for (int j = 0; j < rangeSize; ++j)
                {
                    if (selectRange[i, j]) skillData.gridList.Add(new FieldIndexOffset(i, j, centerIndex, centerIndex));
                }
            }*/

            //��skillData.selectRangeArray = selectRange;
            //��skillData.attackRangeArray = attackRange;
            
            // �f�[�^�ۑ����\�b�h�̌Ăяo��
            CreateSkillScriptableObject();

            // �E�B���h�E�����
            Close();
        }

        // ���Z�b�g�{�^���̔z�u
        if (GUILayout.Button("���Z�b�g"))
        {
            if (EditorUtility.DisplayDialog("���Z�b�g�m�F", string.Format("���͂����f�[�^�����Z�b�g���܂����H"), "OK", "CANCEL")) Reset();
        }
        EditorUtility.SetDirty(skillData);
        AssetDatabase.SaveAssets();
    }

    public void CreateSkillScriptableObject()
    {
        const string PATH = "Assets/ScriptableObjects/Skills/";
        string path = PATH + skillData.skillName + ".asset";

        // �C���X�^���X���������̂��A�Z�b�g�Ƃ��ĕۑ�
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(SkillScriptableObject));
        if (asset == null)
        {
            // �w��̃p�X�Ƀt�@�C�������݂��Ȃ��ꍇ�͐V�K�쐬
            AssetDatabase.CreateAsset(skillData, path);
            Debug.Log(string.Format($"Created new skill, \"{skillData.skillName}\"!"));
        }
        else
        {
            // �w��̃p�X�Ɋ��ɓ����̃t�@�C�������݂���ꍇ�̓f�[�^��j��
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
        // �e��ϐ��̏�����
        skillName = "";
        energy = 0;
        damage = 0;
        priority = 1;
        power = 0;
        skillData.selectGridArray = new bool[rangeSize * rangeSize];
        skillData.attackGridArray = new bool[rangeSize * rangeSize];
        skillData.rangeSize = rangeSize;
        skillData.center = rangeSize / 2;

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