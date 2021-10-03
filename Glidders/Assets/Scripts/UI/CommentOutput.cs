using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace UI
    {
        public class CommentOutput : MonoBehaviour
        {
            // コメントを表示するTextUI
            private Text commentUI;
            // コメントの一覧を格納するリスト
            public List<string> commentTable = new List<string>();
            // コメントリストの中でテーブルの切り替えを表すリスト
            public List<int> tableSize = new List<int>();

            // コメントテーブルの名称
            public List<string> tableName = new List<string>();
            // コメントテーブルごとのコメント採用率
            public List<float> commentRate = new List<float>();
            // コメントテーブルが有効かどうか
            public List<bool> tableActive = new List<bool>();
            // エディタ作業用のfoldOut
            public List<bool> foldOut = new List<bool>();

            // コメント欄の行数
            public int lineCount;
            // コメント欄の一行あたりの文字数
            public int charCountInLine;
            // コメントの間隔
            private float interval = 0.3f;

            // コメント欄に表示する文字列を管理する配列
            string[] commentArray;
            // commentArryayの文字列がひとつのコメント内で何行目に該当するかを管理する配列
            int[] lineAtComment;

            // ひとつ前のコメントを格納しておく
            private string beforeComment = "null";

            private void Start()
            {
                // このオブジェクトを破壊不可に
                DontDestroyOnLoad(gameObject);

                // 配列の要素数を設定し初期化する
                commentArray = new string[lineCount];
                lineAtComment = new int[lineCount];
                for (int i = 0; i < lineCount; ++i)
                {
                    commentArray[i] = "";
                    lineAtComment[i] = 0;
                }
                /*
                for (int i = 0; i < tableSize.Count; ++i)
                {
                    Debug.Log(tableName[i] + ".size=" + tableSize[i]);
                }
                for (int j = 0; j < commentTable.Count; ++j)
                {
                    Debug.Log("comment=" + commentTable[j]);
                }*/
            }

            public void DestroyThisObject()
            {
                Destroy(gameObject);
            }

            /// <summary>
            ///  コメントの生成間隔を変更します。
            /// </summary>
            /// <param name="interval">コメントの生成間隔。</param>
            public void SetInverval(float interval)
            {
                this.interval = interval;
            }

            /// <summary>
            /// コメントの出力先となるTextUIを設定します。
            /// </summary>
            /// <param name="textUI"></param>
            public void SetTextUI(Text textUI)
            {
                commentUI = textUI;
            }

            /// <summary>
            ///  コメントを一時的に停止します。
            /// </summary>
            public void StopComment()
            {
                StopCoroutine(Output());
            }

            /// <summary>
            /// 停止してあるコメントを再生します。
            /// </summary>
            public void StartComment()
            {
                StartCoroutine(Output());
            }

            /// <summary>
            /// 指定した名前のコメントテーブルを有効または無効にします。
            /// </summary>
            /// <param name="tableName">指定するコメントテーブルの名称。</param>
            /// <param name="active">有効または無効。</param>
            public void SetTableActive(string tableName, bool active)
            {
                for(int i = 0; i < this.tableName.Count; ++i)
                {
                    if (this.tableName[i] == tableName)
                    {
                        tableActive[i] = active;
                        return;
                    }
                }
            }

            /// <summary>
            /// 指定した名前のコメントテーブルを一定時間後に有効または無効にします。
            /// </summary>
            /// <param name="tableName">指定するコメントテーブルの名称。</param>
            /// <param name="active">有効または無効。</param>
            public void SetTableActive(string tableName, bool active, float waitSecond)
            {
                for (int i = 0; i < this.tableName.Count; ++i)
                {
                    if (this.tableName[i] == tableName)
                    {
                        tableActive[i] = active;
                        return;
                    }
                }
            }

            IEnumerator wait()
            {

            }

            IEnumerator Output()
            {
                while (true)
                {
                    // 今回表示するコメントのテーブルを決定する
                    int index = ChoiceCommentTable();

                    // 今回表示するコメントを取得する
                    string comment = ChoiceComment(index);

                        // 配列を更新する
                        bool repeatFlg = ArrayUpdate(index, comment, out commentArray, out lineAtComment);

                    // コメントを表示する
                    OutputToTextUI();

                    // コメント欄が流れない場合不自然なので、間を置かずにもう一度コメントを抽出する
                    if (repeatFlg)
                        yield return null;
                    else
                        yield return new WaitForSeconds(interval);
                }
            }

            /// <summary>
            /// 現在有効なテーブルから一つ選び、その添え字を返却します。
            /// </summary>
            /// <returns>選択されたテーブルの添え字。</returns>
            private int ChoiceCommentTable()
            {
                // 有効なコメントテーブルの出現率の合計を求める
                float totalRate = 0;
                //Debug.Log("commentRate.count = " + commentRate.Count);
                for (int i = 0; i < commentRate.Count; ++i)
                {
                    //Debug.Log("Table = " + tableName[i] + ", isAcitive = " + tableActive[i]);
                    if (tableActive[i]) totalRate += commentRate[i];
                }

                // 全体の割合をもとに今回のrand値を求める
                float rand = Random.Range(0.0f, 1.0f);
                float choiceRate = totalRate * rand;
                //Debug.Log("totalRate=" + totalRate);

                // rand値をもとに今回選択されるテーブルを決定する
                totalRate = 0;
                int index = 0;
                for (; index < commentRate.Count; ++index)
                {
                    if (!tableActive[index]) continue;
                    totalRate += commentRate[index];
                    if (choiceRate < totalRate) break;
                }
                //Debug.Log("index = " + index);

                return index;
            }

            /// <summary>
            /// 指定したコメントテーブルからランダムにひとつコメントを取得します。
            /// </summary>
            /// <param name="index">指定したテーブルの添え字。</param>
            /// <returns>取得したコメント。</returns>
            private string ChoiceComment(int index)
            {
                int startIndex = tableSize[index];
                int endIndex;
                if (index < tableSize.Count - 1)
                    endIndex = tableSize[index + 1];
                else
                    endIndex = commentTable.Count;
                
                string returnComment;
                do
                {
                    int rand = Random.Range(startIndex, endIndex);
                    returnComment = commentTable[rand];
                    //Debug.Log("si=" + startIndex + ", ei=" + endIndex + ", reco=" + returnComment + ", beco=" + beforeComment);
                }
                while (startIndex < endIndex - 1 && returnComment == beforeComment);
                //Debug.Log("comment=" + returnComment + ", startindex=" + startIndex + ", endindex=" + endIndex);
                beforeComment = returnComment;

                return returnComment;
            }

            private bool ArrayUpdate(int index, string comment, out string[] commentArray, out int[] lineAtComment)
            {
                // コメントが流れないと不自然なので、流れなかったことを検知する
                bool returnFlg = false;

                commentArray = this.commentArray;
                lineAtComment = this.lineAtComment;
                //Debug.Log("come = " + comment);
                // 今回のコメントが必要とする行数を求める
                int line = (comment.Length / charCountInLine) + 1;
                int lineWork = line;
                //Debug.Log("line = " + line);

                // コメントが見切れないように流れる行数を求める
                while (line < lineAtComment.Length && lineAtComment[line] > 1) ++line;

                // 空きがあった場合はその分流れる行数を減らす
                for (int i = lineCount - 1; i > 0; --i)
                {
                    //Debug.Log("lineAtComment[i] = " + lineAtComment[i]);
                    if (lineAtComment[i] != 0) break;
                    --line;
                }
                line = Mathf.Max(line, 0);  // マイナスにならないように
                //Debug.Log("line = " + line);

                // 既にあったコメントを求めた行数分流す
                int lineIndex = line;
                returnFlg = (line == 0);
                for (; lineIndex < lineCount; ++lineIndex)
                {
                    // LineAtCommentが0のときはその行は使われていないので処理を終了する
                    if (lineAtComment[lineIndex] == 0) break;

                    commentArray[lineIndex - line] = commentArray[lineIndex];
                    lineAtComment[lineIndex - line] = lineAtComment[lineIndex];
                }

                // 空いたスペースに新しいコメントを代入する
                lineIndex -= line;
                line = lineWork;
                for (int i = 0; i < line; ++i)
                {
                    //Debug.Log("lineIndex = " + lineIndex + ", line = " + line + ", i = " + i);
                    //Debug.Log("lineCount = " + commentArray.Length + ", comment = " + commentArray[lineIndex - 1]);
                    //Debug.Log("charcount = " + charCountInLine);
                    commentArray[lineIndex] = comment.Substring(0, Mathf.Min(charCountInLine, comment.Length));
                    lineAtComment[lineIndex] = i + 1;
                    ++lineIndex;
                    if (comment.Length > charCountInLine)
                        comment = comment.Substring(charCountInLine);
                    else
                        break;
                }

                // 使用しなかった部分があればlineAtCommentに0を代入する
                for(; lineIndex < lineCount; ++lineIndex)
                {
                    commentArray[lineIndex] = "";
                    lineAtComment[lineIndex] = 0;
                }

                return returnFlg;
            }

            private void OutputToTextUI()
            {
                if (commentUI == null) return;
                commentUI.text = commentArray[0];
                for (int i = 1; i < lineCount; ++i)
                {
                    commentUI.text += "\n" + commentArray[i];
                }
            }
        }
    }
}
