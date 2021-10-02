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

            // コメント欄に表示する文字列を管理する配列
            string[] commentArray;
            // commentArryayの文字列がひとつのコメント内で何行目に該当するかを管理する配列
            int[] lineAtComment;

            private void Start()
            {
                // 配列の要素数を設定し初期化する
                commentArray = new string[lineCount];
                lineAtComment = new int[lineCount];
                for (int i = 0; i < lineCount; ++i)
                {
                    commentArray[i] = "";
                    lineAtComment[i] = 0;
                }
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

            IEnumerator Output()
            {
                while (true)
                {
                    // 今回表示するコメントのテーブルを決定する
                    int index = ChoiceCommentTable();

                    // 今回表示するコメントを取得する
                    string comment = ChoiceComment(index);

                    // 配列を更新する
                    ArrayUpdate(index, comment, out commentArray, out lineAtComment);

                    yield return null;
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
                for (int i = 0; i < commentRate.Count; ++i)
                {
                    if (tableActive[i]) totalRate += commentRate[i];
                }

                // 全体の割合をもとに今回のrand値を求める
                float rand = Random.Range(0.0f, 1.0f);
                float choiceRate = totalRate * rand;

                // rand値をもとに今回選択されるテーブルを決定する
                totalRate = 0;
                int index = 0;
                for (; index < commentRate.Count; ++index)
                {
                    totalRate += commentRate[index];
                    if (choiceRate < totalRate) break;
                }

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
                int rand = Random.Range(startIndex, endIndex);
                return commentTable[rand];
            }

            private void ArrayUpdate(int index, string comment, out string[] commentArray, out int[] lineAtComment)
            {
                commentArray = this.commentArray;
                lineAtComment = this.lineAtComment;

                // 今回のコメントが必要とする行数を求める
                int line = (comment.Length / charCountInLine) + 1;

                // 空きがあった場合はその分流れる行数を減らす
                for (int i = lineCount; i > 0; --i)
                {
                    if (lineAtComment[i] != 0) break;
                    --line;
                }

                // コメントが見切れないように流れる行数を求める
                while (lineAtComment[line] != 1) ++line;

                // 既にあったコメントを求めた行数分流す
                int lineIndex = line;
                for (; lineIndex < lineCount; ++lineIndex)
                {
                    // LineAtCommentが0のときはその行は使われていないので処理を終了する
                    if (lineAtComment[lineIndex] == 0) break;

                    commentArray[lineIndex - line] = commentArray[lineIndex];
                    lineAtComment[lineIndex - line] = lineAtComment[lineIndex];
                }

                // 空いたスペースに新しいコメントを代入する
                lineIndex -= line;
                for (int i = 0; i < line; ++i)
                {
                    commentArray[lineIndex + i] = comment.Substring(0, charCountInLine);
                    lineAtComment[lineIndex + i] = i + 1;
                    comment = comment.Substring(charCountInLine);
                }

                // 使用しなかった部分があればlineAtCommentに0を代入する
                for(; lineIndex < lineCount; ++lineIndex)
                {
                    commentArray[lineIndex] = "";
                    lineAtComment[lineIndex] = 0;
                }
            }
        }
    }
}
