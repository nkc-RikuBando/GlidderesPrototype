using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Glidders
{
    public static class ScriptableObjectDatabaseWriter
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
                if (path != pathArray[index]) pathArray[index] = path;
                Debug.LogWarning($"識別ID:{id} のファイルパスが既に設定されていたため書き換えました。他のScriptableObjectと識別IDが重複していた場合、上書きされているので注意してください。");
            }

            // データが存在していなかった場合、データを追加する
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

            // データを書き込む
            StreamWriter writer;
            FileInfo fileInfo;
            // Aplication.dataPath で プロジェクトファイルがある絶対パスが取り込める
            fileInfo = new FileInfo(Application.dataPath + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
            Debug.Log("path = " + PATH_Resources + PATH_ScriptableObjectDatabase + TXT_NAME_CSV);
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

            return flg;
        }

        /// <summary>
        /// ID対応表からID一覧配列とパス一覧配列を取得します。これらの配列は添え字で対応しています。
        /// </summary>
        /// <param name="idArray">現在のID一覧配列。</param>
        /// <param name="passArray">現在のパス一覧配列。</param>
        public static void GetIdList(out string[] idArray, out string[] pathArray)
        {
            // id一覧とパス一覧を作成するためのリスト
            List<string> idList = new List<string>();
            List<string> pathList = new List<string>();

            // ファイルから参照したcsvファイルを保存する
            TextAsset csvText = Resources.Load(PATH_ScriptableObjectDatabase + TXT_NAME_CSV) as TextAsset;
            // csvファイルをデータに分解していくためのもの
            StringReader reader = new StringReader(csvText.text);
            // csvファイルのデータを1件ずつ格納する配列
            string[] csvArray;

            // StringReaderからデータを読み取り、カンマで区切る
            string line = reader.ReadToEnd();
            csvArray = line.Split(',');

            // 読み取った値がIDかどうか
            bool isId = true;
            for(int i = 0; i < csvArray.Length; ++i)
            {
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
            for(int i = 0; i < count; ++i)
            {
                idArray[i] = idList[i];
                pathArray[i] = pathList[i];
            }
        }
    }
}
