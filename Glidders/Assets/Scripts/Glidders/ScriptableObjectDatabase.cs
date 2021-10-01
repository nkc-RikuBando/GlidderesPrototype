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
        // 識別IDとScriptableObjectの対応表が格納されているファイルパス（※使用していない）
        //const string ID_PATH = "Assets/Resources/ScriptableObjectDatabase/";
        // 対応表のファイルとしての名前（.csvつき）
        const string TXT_NAME_CSV = "IdList.csv";
        // 対応表のファイルとしての名前（.csvなし）
        const string TXT_NAME = "IdList";
        // Resourcesファイル内での対応表までのパス
        const string PATH_ScriptableObjectDatabase = "ScriptableObjectDatabase/";
        // 対応表までのパス
        const string PATH_Resources = "/Resources/";
        // .assetを削除するためのもの
        const string EXTENSION_asset = ".asset";

        /// <summary>
        /// データベースに登録されているCharacter識別IDの一覧リスト。
        /// </summary>
        public static List<string> characterId { get; private set; }
        /// <summary>
        /// データベースに登録されているCharacterScriptableObjectの一覧リスト。
        /// </summary>
        public static List<CharacterScriptableObject> characterScriptableObject { get; private set; }

        /// <summary>
        /// データベースに登録されているUniqueSkill識別IDの一覧配リスト。
        /// </summary>
        public static List<string> uniqueSkillId { get; private set; }
        /// <summary>
        /// データベースに登録されているuniqueSkillScriptableObjectの一覧リスト。
        /// </summary>
        public static List<UniqueSkillScriptableObject> uniqueSkillScriptableObject { get; private set; }

        /// <summary>
        /// データベースに登録されているBuffViewData識別IDの一覧リスト。
        /// </summary>
        public static List<string> buffViewDataId { get; private set; }
        /// <summary>
        /// データベースに登録されているBuffViewDataの一覧リスト。
        /// </summary>
        public static List<BuffViewData> buffViewData { get; private set; }

        // ScriptableObjectを種類に関わらずまとめて管理するためのもの
        static ScriptableObjectData[] scriptableObjectDataArray;

        /// <summary>
        /// テキストファイルからデータベース情報を取得し、設定する
        /// </summary>
        static ScriptableObjectDatabase()
        {
            // 各種リストを初期化する
            characterId = new List<string>();
            characterScriptableObject = new List<CharacterScriptableObject>();
            uniqueSkillId = new List<string>();
            uniqueSkillScriptableObject = new List<UniqueSkillScriptableObject>();
            buffViewDataId = new List<string>();
            buffViewData = new List<BuffViewData>();

            // テキストファイルから既存のScriptableObjectを取得する
            string[] idArray, pathArray;
            GetIdList(out idArray, out pathArray);

            // ファイルパスからScriptableObjectを取得し、別配列に格納する
            scriptableObjectDataArray = new ScriptableObjectData[idArray.Length];
            for (int i = 0; i < idArray.Length; ++i)
            {
                int resourcesIndexOf = pathArray[i].IndexOf(PATH_Resources);
                resourcesIndexOf += PATH_Resources.Length;
                //Debug.Log("path = " + pathArray[i].Substring(resourcesIndexOf, pathArray[i].Length - resourcesIndexOf - EXTENSION_asset.Length));
                var asset = Resources.Load(pathArray[i].Substring(resourcesIndexOf, pathArray[i].Length - resourcesIndexOf - EXTENSION_asset.Length), typeof(object));
                //Debug.Log("isSkill = " + (asset == null));
                // どの種類のScriptableObjectかを調べ、各配列に格納する
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

            Debug.Log("charaList = " + characterId.Count + "件, skillList = " + uniqueSkillId.Count + "件, buffList = " + buffViewDataId.Count + "件");
        }

        // データベースから任意のScriptableObjectを取得します。
        /// <summary>
        /// データベースからCharacterScriptableObjectを取得します。存在しない場合。NullReferenceExceptionが投げられます。
        /// </summary>
        /// <param name="id">ScriptableObjectの識別ID。</param>
        /// <returns>取得したScriptableObject。</returns>
        public static CharacterScriptableObject GetCharacter(string id)
        {
            // 指定された識別IDが登録されているか調べる
            int index = 0;
            while (index < characterId.Count && id != characterId[index]) ++index;

            // 識別IDが登録されていなかった場合、例外を投げる
            if (index >= characterId.Count)
            {
                throw new NullReferenceException(string.Format($"ScriptableObjectDatabaseから存在しない識別IDを用いた取得を試みました。" +
                    $"\n識別ID:{id}, ScriptableObjectType:Character"));
            }

            // ScriptableObjectを返却する
            return characterScriptableObject[index];
        }

        /// <summary>
        /// データベースからUniqueSkillScriptableObjectを取得します。存在しない場合。NullReferenceExceptionが投げられます。
        /// </summary>
        /// <param name="id">ScriptableObjectの識別ID。</param>
        /// <returns>取得したScriptableObject。</returns>
        public static UniqueSkillScriptableObject GetSkill(string id)
        {
            // 指定された識別IDが登録されているか調べる
            int index = 0;
            while (index < uniqueSkillId.Count && id != uniqueSkillId[index]) ++index;
            //Debug.Log("count=" + uniqueSkillId.Count);
            // 識別IDが登録されていなかった場合、例外を投げる
            if (index >= uniqueSkillId.Count)
            {
                throw new NullReferenceException(string.Format($"ScriptableObjectDatabaseから存在しない識別IDを用いた取得を試みました。" +
                    $"\n識別ID:{id}, ScriptableObjectType:Skill"));
            }

            // ScriptableObjectを返却する
            return uniqueSkillScriptableObject[index];
        }

        /// <summary>
        /// データベースからBuffViewDataを取得します。存在しない場合。NullReferenceExceptionが投げられます。
        /// </summary>
        /// <param name="id">ScriptableObjectの識別ID。</param>
        /// <returns>取得したScriptableObject。</returns>
        public static BuffViewData GetBuff(string id)
        {
            // 指定された識別IDが登録されているか調べる
            int index = 0;
            while (index < buffViewDataId.Count && id != buffViewDataId[index]) ++index;

            // 識別IDが登録されていなかった場合、例外を投げる
            if (index >= buffViewDataId.Count)
            {
                throw new NullReferenceException(string.Format($"ScriptableObjectDatabaseから存在しない識別IDを用いた取得を試みました。" +
                    $"\n識別ID:{id}, ScriptableObjectType:Buff"));
            }

            // ScriptableObjectを返却する
            return buffViewData[index];
        }


        // これより下は、ScriptableObjectに識別IDを設定した際にそれを登録するメソッド
        /// <summary>
        /// ID対応表に新しいデータを追加します。既にデータが存在していた場合は、そのデータを書き換えます。
        /// </summary>
        /// <returns>データを追加した場合はtrue、書き換えた場合はfalse。</returns>
        public static bool Write(string id, string path)
        {
            // 既にデータが存在しているかどうかを確認する
            string[] idArray, pathArray;
            GetIdList(out idArray, out pathArray);
            int index = 0;
            while (index < idArray.Length && id != idArray[index]) ++index;

            bool flg = (index == idArray.Length);
            // データが存在していた場合、パスが異なるなら書き換える
            if (!flg)
            {
                if (path != pathArray[index])
                {
                    pathArray[index] = path;
                    Debug.LogWarning($"識別ID:{id} のファイルパスが既に設定されていたため書き換えました。他のScriptableObjectと識別IDが重複していた場合、上書きされているので注意してください。");
                }
            }

            // データが存在していなかった場合、データを追加する
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

            // データを書き込む
            StreamWriter writer;
            FileInfo fileInfo;
            // Aplication.dataPath で プロジェクトファイルがある絶対パスが取り込める
            fileInfo = new FileInfo(Application.dataPath + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
            //Debug.Log("path = " + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
            writer = fileInfo.CreateText();
            for (int i = 0; i < idArray.Length; ++i)
            {
                // カンマ区切りでデータを書き込む
                writer.Write(idArray[i]);
                writer.Write(",");
                writer.Write(pathArray[i]);
                if (i == idArray.Length - 1) continue;  // 最後のカンマを書き込まないように
                writer.Write(",");
            }
            writer.Flush();
            writer.Close();
            //AssetDatabase.ImportAsset(Application.dataPath + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
            return flg;
        }

        /// <summary>
        /// ID対応表からID一覧配列とパス一覧配列を取得します。これらの配列は添え字で対応しています。
        /// </summary>
        /// <param name="idArray">現在のID一覧配列。</param>
        /// <param name="passArray">現在のパス一覧配列。</param>
        public static void GetIdList(out string[] idArray, out string[] pathArray)
        {
            //idArray = new string[0]; pathArray = new string[0]; return;
            // id一覧とパス一覧を作成するためのリスト
            List<string> idList = new List<string>();
            List<string> pathList = new List<string>();

            // ファイルから参照したcsvファイルを保存する
            TextAsset csvText = Resources.Load(PATH_ScriptableObjectDatabase + TXT_NAME) as TextAsset;
            //Debug.Log("log = " + csvText.name);
            // csvファイルをデータに分解していくためのもの
            StringReader reader = new StringReader(csvText.text);
            // csvファイルのデータを1件ずつ格納する配列
            string[] csvArray;

            // StringReaderからデータを読み取り、カンマで区切る
            string line = reader.ReadToEnd();
            csvArray = line.Split(',');

            // 読み取った値がIDかどうか
            bool isId = true;
            for (int i = 0; i < csvArray.Length; ++i)
            {
                //Debug.Log("csvArray[i] = " + csvArray[i]);
                // IDとパスの適切な方にデータを追加する
                if (isId) idList.Add(csvArray[i]);
                else pathList.Add(csvArray[i]);

                // IDとパスは交互に格納されているため、追加先を交互に切り替える
                isId = !isId;
            }

            // idArrayとpathArrayにリストの値を移す
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
        /// 一対の識別IDとScriptableObjectのセットを格納するための親クラスです。
        /// </summary>
        public abstract class ScriptableObjectData
        {
            // 対応するScriptableObjectを識別するID
            string id;

            /// <summary>
            /// 管理する識別IDとScriptableObjectの種類を設定します。
            /// </summary>
            /// <param name="id">設定する識別ID。</param>
            /// <param name="id2ScriptableObjectType">設定するScriptableObjectの対応構造体。</param>
            public ScriptableObjectData(string id)
            {
                this.id = id;
            }

            /// <summary>
            /// 管理されているScriptableObjectの種類を返却します。
            /// </summary>
            /// <returns>ScriptableObjectの種類。</returns>
            public abstract ScriptableObjectType GetScriptableObjectType();
        }

        /// <summary>
        /// 一対の識別IDとCharacterScriptableObjectのセットを格納します。
        /// </summary>
        public class CharacterScriptableObjectData : ScriptableObjectData
        {
            // IDで識別されるCharacterScriptableObject
            CharacterScriptableObject scriptableObject;

            /// <summary>
            /// 管理する識別IDとScriptableObjectおよびScriptableObjectの種類を設定します。
            /// </summary>
            /// <param name="id">設定する識別ID。</param>
            /// <param name="scriptableObject">設定するScriptableObject。</param>
            /// <param name="id2ScriptableObjectType">設定するScriptableObjectの対応構造体。</param>
            public CharacterScriptableObjectData
                (string id, CharacterScriptableObject scriptableObject)
                : base(id)
            {
                this.scriptableObject = scriptableObject;
            }

            /// <summary>
            /// このデータがCharacterScriptableObjectであることを返却します。
            /// </summary>
            /// <returns>ScriptableObjectType.CHARACTER。</returns>
            public override ScriptableObjectType GetScriptableObjectType()
            {
                return ScriptableObjectType.CHARACTER;
            }
        }

        /// <summary>
        /// 一対の識別IDとUniqueSkillScriptableObjectのセットを格納します。
        /// </summary>
        public class UniqueSkillScriptableObjectData : ScriptableObjectData
        {
            // IDで識別されるCharacterScriptableObject
            UniqueSkillScriptableObject scriptableObject;

            /// <summary>
            /// 管理する識別IDとScriptableObjectおよびScriptableObjectの種類を設定します。
            /// </summary>
            /// <param name="id">設定する識別ID。</param>
            /// <param name="scriptableObject">設定するScriptableObject。</param>
            /// <param name="id2ScriptableObjectType">設定するScriptableObjectの対応構造体。</param>
            public UniqueSkillScriptableObjectData
                (string id, UniqueSkillScriptableObject scriptableObject)
                : base(id)
            {
                this.scriptableObject = scriptableObject;
            }

            /// <summary>
            /// このデータがUniqueSkillScriptableObjectであることを返却します。
            /// </summary>
            /// <returns>ScriptableObjectType.SKILL。</returns>
            public override ScriptableObjectType GetScriptableObjectType()
            {
                return ScriptableObjectType.SKILL;
            }
        }

        /// <summary>
        /// 一対の識別IDとBuffViewDataのセットを格納します。
        /// </summary>
        public class BuffViewDataScriptableObjectData : ScriptableObjectData
        {
            // IDで識別されるCharacterScriptableObject
            BuffViewData scriptableObject;

            /// <summary>
            /// 管理する識別IDとScriptableObjectおよびScriptableObjectの種類を設定します。
            /// </summary>
            /// <param name="id">設定する識別ID。</param>
            /// <param name="scriptableObject">設定するScriptableObject。</param>
            /// <param name="id2ScriptableObjectType">設定するScriptableObjectの対応構造体。</param>
            public BuffViewDataScriptableObjectData
                (string id, BuffViewData scriptableObject)
                : base(id)
            {
                this.scriptableObject = scriptableObject;
            }

            /// <summary>
            /// このデータがBuffViewDataであることを返却します。
            /// </summary>
            /// <returns>ScriptableObjectType.BUFF。</returns>
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