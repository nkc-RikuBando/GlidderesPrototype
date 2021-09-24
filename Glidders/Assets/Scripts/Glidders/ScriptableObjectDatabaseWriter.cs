using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Glidders
{
    public static class ScriptableObjectDatabaseWriter
    {
        // ����ID��ScriptableObject�̑Ή��\���i�[����Ă���t�@�C���p�X�i���g�p���Ă��Ȃ��j
        //const string ID_PATH = "Assets/Resources/ScriptableObjectDatabase/";
        // �Ή��\�̃t�@�C���Ƃ��Ă̖��O�i.csv���j
        const string TXT_NAME_CSV = "IdList.csv";
        // �Ή��\�̃t�@�C���Ƃ��Ă̖��O�i.csv�Ȃ��j
        const string TXT_NAME = "IdList";
        // Resources�t�@�C�����ł̑Ή��\�܂ł̃p�X
        const string PATH_ScriptableObjectDatabase = "ScriptableObjectDatabase/";
        // �Ή��\�܂ł̃p�X
        const string PATH_Resources = "/Resources/";

        /// <summary>
        /// ID�Ή��\�ɐV�����f�[�^��ǉ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�́A���̃f�[�^�����������܂��B
        /// </summary>
        /// <returns>�f�[�^��ǉ������ꍇ��true�A�����������ꍇ��false�B</returns>
        public static bool Write(string id, string path)
        {
            // ���Ƀf�[�^�����݂��Ă��邩�ǂ������m�F����
            string[] idArray, pathArray;
            GetIdList(out idArray, out pathArray);
            int index = 0;
            while (index < idArray.Length && id != idArray[index]) ++index;

            bool flg = (index == idArray.Length);
            // �f�[�^�����݂��Ă����ꍇ�A�p�X���قȂ�Ȃ珑��������
            if (!flg)
            {
                if (path != pathArray[index]) pathArray[index] = path;
                Debug.LogWarning($"����ID:{id} �̃t�@�C���p�X�����ɐݒ肳��Ă������ߏ��������܂����B����ScriptableObject�Ǝ���ID���d�����Ă����ꍇ�A�㏑������Ă���̂Œ��ӂ��Ă��������B");
            }

            // �f�[�^�����݂��Ă��Ȃ������ꍇ�A�f�[�^��ǉ�����
            else
            {
                string[] workIdArray = new string[idArray.Length + 1];
                string[] workPathArray = new string[idArray.Length + 1];
                for(int i = 0; i < idArray.Length; ++i)
                {
                    workIdArray[i] = idArray[i];
                    workPathArray[i] = pathArray[i];
                }
                workIdArray[idArray.Length] = id;
                workPathArray[idArray.Length] = path;
                idArray = workIdArray;
                pathArray = workPathArray;
            }

            // �f�[�^����������
            StreamWriter writer;
            FileInfo fileInfo;
            // Aplication.dataPath �� �v���W�F�N�g�t�@�C���������΃p�X����荞�߂�
            fileInfo = new FileInfo(Application.dataPath + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
            Debug.Log("path = " + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
            writer = fileInfo.CreateText();
            for (int i = 0; i < idArray.Length; ++i)
            {
                // �J���}��؂�Ńf�[�^����������
                writer.Write(idArray[i]);
                writer.Write(",");
                writer.Write(pathArray[i]);
                if (i == idArray.Length - 1) continue;  // �Ō�̃J���}���������܂Ȃ��悤��
                writer.Write(",");
            }
            writer.Flush();
            writer.Close();

            return flg;
        }

        /// <summary>
        /// ID�Ή��\����ID�ꗗ�z��ƃp�X�ꗗ�z����擾���܂��B�����̔z��͓Y�����őΉ����Ă��܂��B
        /// </summary>
        /// <param name="idArray">���݂�ID�ꗗ�z��B</param>
        /// <param name="passArray">���݂̃p�X�ꗗ�z��B</param>
        public static void GetIdList(out string[] idArray, out string[] pathArray)
        {
            // id�ꗗ�ƃp�X�ꗗ���쐬���邽�߂̃��X�g
            List<string> idList = new List<string>();
            List<string> pathList = new List<string>();

            // �t�@�C������Q�Ƃ���csv�t�@�C����ۑ�����
            TextAsset csvText = Resources.Load(PATH_ScriptableObjectDatabase + TXT_NAME_CSV) as TextAsset;
            // csv�t�@�C�����f�[�^�ɕ������Ă������߂̂���
            StringReader reader = new StringReader(csvText.text);
            // csv�t�@�C���̃f�[�^��1�����i�[����z��
            string[] csvArray;

            // StringReader����f�[�^��ǂݎ��A�J���}�ŋ�؂�
            string line = reader.ReadToEnd();
            csvArray = line.Split(',');

            // �ǂݎ�����l��ID���ǂ���
            bool isId = true;
            for(int i = 0; i < csvArray.Length; ++i)
            {
                // ID�ƃp�X�̓K�؂ȕ��Ƀf�[�^��ǉ�����
                if (isId) idList.Add(csvArray[i]);
                else pathList.Add(csvArray[i]);

                // ID�ƃp�X�͌��݂Ɋi�[����Ă��邽�߁A�ǉ�������݂ɐ؂�ւ���
                isId = !isId;
            }

            // idArray��pathArray�Ƀ��X�g�̒l���ڂ�
            int count = idList.Count;
            idArray = new string[count];
            pathArray = new string[count];
            for(int i = 0; i < count; ++i)
            {
                idArray[i] = idList[i];
                pathArray[i] = pathList[i];
            }
        }
    }
}
