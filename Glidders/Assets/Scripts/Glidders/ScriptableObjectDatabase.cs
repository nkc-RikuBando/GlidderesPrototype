using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;
using Glidders.Buff;
using System;
using System.IO;

namespace Glidders
{
    public class ScriptableObjectDatabase
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
        // .asset���폜���邽�߂̂���
        const string EXTENSION_asset = ".asset";

        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^����Ă���Character����ID�̈ꗗ���X�g�B
        /// </summary>
        public static List<string> characterId { get; private set; }
        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^����Ă���CharacterScriptableObject�̈ꗗ���X�g�B
        /// </summary>
        public static List<CharacterScriptableObject> characterScriptableObject { get; private set; }

        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^����Ă���UniqueSkill����ID�̈ꗗ�z���X�g�B
        /// </summary>
        public static List<string> uniqueSkillId { get; private set; }
        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^����Ă���uniqueSkillScriptableObject�̈ꗗ���X�g�B
        /// </summary>
        public static List<UniqueSkillScriptableObject> uniqueSkillScriptableObject { get; private set; }

        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^����Ă���BuffViewData����ID�̈ꗗ���X�g�B
        /// </summary>
        public static List<string> buffViewDataId { get; private set; }
        /// <summary>
        /// �f�[�^�x�[�X�ɓo�^����Ă���BuffViewData�̈ꗗ���X�g�B
        /// </summary>
        public static List<BuffViewData> buffViewData { get; private set; }

        // ScriptableObject����ނɊւ�炸�܂Ƃ߂ĊǗ����邽�߂̂���
        static ScriptableObjectData[] scriptableObjectDataArray;

        /// <summary>
        /// �e�L�X�g�t�@�C������f�[�^�x�[�X�����擾���A�ݒ肷��
        /// </summary>
        static ScriptableObjectDatabase()
        {
            // �e�탊�X�g������������
            characterId = new List<string>();
            characterScriptableObject = new List<CharacterScriptableObject>();
            uniqueSkillId = new List<string>();
            uniqueSkillScriptableObject = new List<UniqueSkillScriptableObject>();
            buffViewDataId = new List<string>();
            buffViewData = new List<BuffViewData>();

            // �e�L�X�g�t�@�C�����������ScriptableObject���擾����
            string[] idArray, pathArray;
            GetIdList(out idArray, out pathArray);

            // �t�@�C���p�X����ScriptableObject���擾���A�ʔz��Ɋi�[����
            scriptableObjectDataArray = new ScriptableObjectData[idArray.Length];
            for (int i = 0; i < idArray.Length; ++i)
            {
                int resourcesIndexOf = pathArray[i].IndexOf(PATH_Resources);
                resourcesIndexOf += PATH_Resources.Length;
                //Debug.Log("path = " + pathArray[i].Substring(resourcesIndexOf, pathArray[i].Length - resourcesIndexOf - EXTENSION_asset.Length));
                var asset = Resources.Load(pathArray[i].Substring(resourcesIndexOf, pathArray[i].Length - resourcesIndexOf - EXTENSION_asset.Length), typeof(object));
                //Debug.Log("isSkill = " + (asset == null));
                // �ǂ̎�ނ�ScriptableObject���𒲂ׁA�e�z��Ɋi�[����
                if (asset is CharacterScriptableObject)
                {
                    CharacterScriptableObject so = asset as CharacterScriptableObject;
                    scriptableObjectDataArray[i] = new CharacterScriptableObjectData(idArray[i], so);
                    characterId.Add(idArray[i]);
                    characterScriptableObject.Add(so);
                }
                if (asset is UniqueSkillScriptableObject)
                {
                    UniqueSkillScriptableObject so = asset as UniqueSkillScriptableObject;
                    scriptableObjectDataArray[i] = new UniqueSkillScriptableObjectData(idArray[i], so);
                    uniqueSkillId.Add(idArray[i]);
                    uniqueSkillScriptableObject.Add(so);
                }
                if (asset is BuffViewData)
                {
                    BuffViewData so = asset as BuffViewData;
                    scriptableObjectDataArray[i] = new BuffViewDataScriptableObjectData(idArray[i], so);
                    buffViewDataId.Add(idArray[i]);
                    buffViewData.Add(so);
                }
            }

            Debug.Log("charaList = " + characterId.Count + "��, skillList = " + uniqueSkillId.Count + "��, buffList = " + buffViewDataId.Count + "��");
        }

        // �f�[�^�x�[�X����C�ӂ�ScriptableObject���擾���܂��B
        /// <summary>
        /// �f�[�^�x�[�X����CharacterScriptableObject���擾���܂��B���݂��Ȃ��ꍇ�BNullReferenceException���������܂��B
        /// </summary>
        /// <param name="id">ScriptableObject�̎���ID�B</param>
        /// <returns>�擾����ScriptableObject�B</returns>
        public static CharacterScriptableObject GetCharacter(string id)
        {
            // �w�肳�ꂽ����ID���o�^����Ă��邩���ׂ�
            int index = 0;
            while (index < characterId.Count && id != characterId[index]) ++index;

            // ����ID���o�^����Ă��Ȃ������ꍇ�A��O�𓊂���
            if (index >= characterId.Count)
            {
                throw new NullReferenceException(string.Format($"ScriptableObjectDatabase���瑶�݂��Ȃ�����ID��p�����擾�����݂܂����B" +
                    $"\n����ID:{id}, ScriptableObjectType:Character"));
            }

            // ScriptableObject��ԋp����
            return characterScriptableObject[index];
        }

        /// <summary>
        /// �f�[�^�x�[�X����UniqueSkillScriptableObject���擾���܂��B���݂��Ȃ��ꍇ�BNullReferenceException���������܂��B
        /// </summary>
        /// <param name="id">ScriptableObject�̎���ID�B</param>
        /// <returns>�擾����ScriptableObject�B</returns>
        public static UniqueSkillScriptableObject GetSkill(string id)
        {
            // �w�肳�ꂽ����ID���o�^����Ă��邩���ׂ�
            int index = 0;
            while (index < uniqueSkillId.Count && id != uniqueSkillId[index]) ++index;
            //Debug.Log("count=" + uniqueSkillId.Count);
            // ����ID���o�^����Ă��Ȃ������ꍇ�A��O�𓊂���
            if (index >= uniqueSkillId.Count)
            {
                throw new NullReferenceException(string.Format($"ScriptableObjectDatabase���瑶�݂��Ȃ�����ID��p�����擾�����݂܂����B" +
                    $"\n����ID:{id}, ScriptableObjectType:Skill"));
            }

            // ScriptableObject��ԋp����
            return uniqueSkillScriptableObject[index];
        }

        /// <summary>
        /// �f�[�^�x�[�X����BuffViewData���擾���܂��B���݂��Ȃ��ꍇ�BNullReferenceException���������܂��B
        /// </summary>
        /// <param name="id">ScriptableObject�̎���ID�B</param>
        /// <returns>�擾����ScriptableObject�B</returns>
        public static BuffViewData GetBuff(string id)
        {
            // �w�肳�ꂽ����ID���o�^����Ă��邩���ׂ�
            int index = 0;
            while (index < buffViewDataId.Count && id != buffViewDataId[index]) ++index;

            // ����ID���o�^����Ă��Ȃ������ꍇ�A��O�𓊂���
            if (index >= buffViewDataId.Count)
            {
                throw new NullReferenceException(string.Format($"ScriptableObjectDatabase���瑶�݂��Ȃ�����ID��p�����擾�����݂܂����B" +
                    $"\n����ID:{id}, ScriptableObjectType:Buff"));
            }

            // ScriptableObject��ԋp����
            return buffViewData[index];
        }


        // �����艺�́AScriptableObject�Ɏ���ID��ݒ肵���ۂɂ����o�^���郁�\�b�h
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
                if (path != pathArray[index])
                {
                    pathArray[index] = path;
                    Debug.LogWarning($"����ID:{id} �̃t�@�C���p�X�����ɐݒ肳��Ă������ߏ��������܂����B����ScriptableObject�Ǝ���ID���d�����Ă����ꍇ�A�㏑������Ă���̂Œ��ӂ��Ă��������B");
                }
            }

            // �f�[�^�����݂��Ă��Ȃ������ꍇ�A�f�[�^��ǉ�����
            else
            {
                string[] workIdArray = new string[idArray.Length + 1];
                string[] workPathArray = new string[idArray.Length + 1];
                for (int i = 0; i < idArray.Length; ++i)
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
            //Debug.Log("path = " + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
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
            //AssetDatabase.ImportAsset(Application.dataPath + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
            return flg;
        }

        /// <summary>
        /// ID�Ή��\����ID�ꗗ�z��ƃp�X�ꗗ�z����擾���܂��B�����̔z��͓Y�����őΉ����Ă��܂��B
        /// </summary>
        /// <param name="idArray">���݂�ID�ꗗ�z��B</param>
        /// <param name="passArray">���݂̃p�X�ꗗ�z��B</param>
        public static void GetIdList(out string[] idArray, out string[] pathArray)
        {
            //idArray = new string[0]; pathArray = new string[0]; return;
            // id�ꗗ�ƃp�X�ꗗ���쐬���邽�߂̃��X�g
            List<string> idList = new List<string>();
            List<string> pathList = new List<string>();

            // �t�@�C������Q�Ƃ���csv�t�@�C����ۑ�����
            TextAsset csvText = Resources.Load(PATH_ScriptableObjectDatabase + TXT_NAME) as TextAsset;
            //Debug.Log("log = " + csvText.name);
            // csv�t�@�C�����f�[�^�ɕ������Ă������߂̂���
            StringReader reader = new StringReader(csvText.text);
            // csv�t�@�C���̃f�[�^��1�����i�[����z��
            string[] csvArray;

            // StringReader����f�[�^��ǂݎ��A�J���}�ŋ�؂�
            string line = reader.ReadToEnd();
            csvArray = line.Split(',');

            // �ǂݎ�����l��ID���ǂ���
            bool isId = true;
            for (int i = 0; i < csvArray.Length; ++i)
            {
                //Debug.Log("csvArray[i] = " + csvArray[i]);
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
            for (int i = 0; i < count; ++i)
            {
                idArray[i] = idList[i];
                pathArray[i] = pathList[i];
            }
            //Debug.Log("idArray.length = " + idArray.Length);
        }

        /// <summary>
        /// ��΂̎���ID��ScriptableObject�̃Z�b�g���i�[���邽�߂̐e�N���X�ł��B
        /// </summary>
        public abstract class ScriptableObjectData
        {
            // �Ή�����ScriptableObject�����ʂ���ID
            string id;

            /// <summary>
            /// �Ǘ����鎯��ID��ScriptableObject�̎�ނ�ݒ肵�܂��B
            /// </summary>
            /// <param name="id">�ݒ肷�鎯��ID�B</param>
            /// <param name="id2ScriptableObjectType">�ݒ肷��ScriptableObject�̑Ή��\���́B</param>
            public ScriptableObjectData(string id)
            {
                this.id = id;
            }

            /// <summary>
            /// �Ǘ�����Ă���ScriptableObject�̎�ނ�ԋp���܂��B
            /// </summary>
            /// <returns>ScriptableObject�̎�ށB</returns>
            public abstract ScriptableObjectType GetScriptableObjectType();
        }

        /// <summary>
        /// ��΂̎���ID��CharacterScriptableObject�̃Z�b�g���i�[���܂��B
        /// </summary>
        public class CharacterScriptableObjectData : ScriptableObjectData
        {
            // ID�Ŏ��ʂ����CharacterScriptableObject
            CharacterScriptableObject scriptableObject;

            /// <summary>
            /// �Ǘ����鎯��ID��ScriptableObject�����ScriptableObject�̎�ނ�ݒ肵�܂��B
            /// </summary>
            /// <param name="id">�ݒ肷�鎯��ID�B</param>
            /// <param name="scriptableObject">�ݒ肷��ScriptableObject�B</param>
            /// <param name="id2ScriptableObjectType">�ݒ肷��ScriptableObject�̑Ή��\���́B</param>
            public CharacterScriptableObjectData
                (string id, CharacterScriptableObject scriptableObject)
                : base(id)
            {
                this.scriptableObject = scriptableObject;
            }

            /// <summary>
            /// ���̃f�[�^��CharacterScriptableObject�ł��邱�Ƃ�ԋp���܂��B
            /// </summary>
            /// <returns>ScriptableObjectType.CHARACTER�B</returns>
            public override ScriptableObjectType GetScriptableObjectType()
            {
                return ScriptableObjectType.CHARACTER;
            }
        }

        /// <summary>
        /// ��΂̎���ID��UniqueSkillScriptableObject�̃Z�b�g���i�[���܂��B
        /// </summary>
        public class UniqueSkillScriptableObjectData : ScriptableObjectData
        {
            // ID�Ŏ��ʂ����CharacterScriptableObject
            UniqueSkillScriptableObject scriptableObject;

            /// <summary>
            /// �Ǘ����鎯��ID��ScriptableObject�����ScriptableObject�̎�ނ�ݒ肵�܂��B
            /// </summary>
            /// <param name="id">�ݒ肷�鎯��ID�B</param>
            /// <param name="scriptableObject">�ݒ肷��ScriptableObject�B</param>
            /// <param name="id2ScriptableObjectType">�ݒ肷��ScriptableObject�̑Ή��\���́B</param>
            public UniqueSkillScriptableObjectData
                (string id, UniqueSkillScriptableObject scriptableObject)
                : base(id)
            {
                this.scriptableObject = scriptableObject;
            }

            /// <summary>
            /// ���̃f�[�^��UniqueSkillScriptableObject�ł��邱�Ƃ�ԋp���܂��B
            /// </summary>
            /// <returns>ScriptableObjectType.SKILL�B</returns>
            public override ScriptableObjectType GetScriptableObjectType()
            {
                return ScriptableObjectType.SKILL;
            }
        }

        /// <summary>
        /// ��΂̎���ID��BuffViewData�̃Z�b�g���i�[���܂��B
        /// </summary>
        public class BuffViewDataScriptableObjectData : ScriptableObjectData
        {
            // ID�Ŏ��ʂ����CharacterScriptableObject
            BuffViewData scriptableObject;

            /// <summary>
            /// �Ǘ����鎯��ID��ScriptableObject�����ScriptableObject�̎�ނ�ݒ肵�܂��B
            /// </summary>
            /// <param name="id">�ݒ肷�鎯��ID�B</param>
            /// <param name="scriptableObject">�ݒ肷��ScriptableObject�B</param>
            /// <param name="id2ScriptableObjectType">�ݒ肷��ScriptableObject�̑Ή��\���́B</param>
            public BuffViewDataScriptableObjectData
                (string id, BuffViewData scriptableObject)
                : base(id)
            {
                this.scriptableObject = scriptableObject;
            }

            /// <summary>
            /// ���̃f�[�^��BuffViewData�ł��邱�Ƃ�ԋp���܂��B
            /// </summary>
            /// <returns>ScriptableObjectType.BUFF�B</returns>
            public override ScriptableObjectType GetScriptableObjectType()
            {
                return ScriptableObjectType.BUFF;
            }
        }

        public enum ScriptableObjectType
        {
            CHARACTER,
            SKILL,
            BUFF,
        }
    }
}