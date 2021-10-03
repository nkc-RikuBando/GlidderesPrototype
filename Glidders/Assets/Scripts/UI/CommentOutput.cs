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
            // �R�����g��\������TextUI
            private Text commentUI;
            // �R�����g�̈ꗗ���i�[���郊�X�g
            public List<string> commentTable = new List<string>();
            // �R�����g���X�g�̒��Ńe�[�u���̐؂�ւ���\�����X�g
            public List<int> tableSize = new List<int>();

            // �R�����g�e�[�u���̖���
            public List<string> tableName = new List<string>();
            // �R�����g�e�[�u�����Ƃ̃R�����g�̗p��
            public List<float> commentRate = new List<float>();
            // �R�����g�e�[�u�����L�����ǂ���
            public List<bool> tableActive = new List<bool>();
            // �G�f�B�^��Ɨp��foldOut
            public List<bool> foldOut = new List<bool>();

            // �R�����g���̍s��
            public int lineCount;
            // �R�����g���̈�s������̕�����
            public int charCountInLine;
            // �R�����g�̊Ԋu
            private float interval = 0.3f;

            // �R�����g���ɕ\�����镶������Ǘ�����z��
            string[] commentArray;
            // commentArryay�̕����񂪂ЂƂ̃R�����g���ŉ��s�ڂɊY�����邩���Ǘ�����z��
            int[] lineAtComment;

            // �ЂƂO�̃R�����g���i�[���Ă���
            private string beforeComment = "null";

            private void Start()
            {
                // ���̃I�u�W�F�N�g��j��s��
                DontDestroyOnLoad(gameObject);

                // �z��̗v�f����ݒ肵����������
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
            ///  �R�����g�̐����Ԋu��ύX���܂��B
            /// </summary>
            /// <param name="interval">�R�����g�̐����Ԋu�B</param>
            public void SetInverval(float interval)
            {
                this.interval = interval;
            }

            /// <summary>
            /// �R�����g�̏o�͐�ƂȂ�TextUI��ݒ肵�܂��B
            /// </summary>
            /// <param name="textUI"></param>
            public void SetTextUI(Text textUI)
            {
                commentUI = textUI;
            }

            /// <summary>
            ///  �R�����g���ꎞ�I�ɒ�~���܂��B
            /// </summary>
            public void StopComment()
            {
                StopCoroutine(Output());
            }

            /// <summary>
            /// ��~���Ă���R�����g���Đ����܂��B
            /// </summary>
            public void StartComment()
            {
                StartCoroutine(Output());
            }

            /// <summary>
            /// �w�肵�����O�̃R�����g�e�[�u����L���܂��͖����ɂ��܂��B
            /// </summary>
            /// <param name="tableName">�w�肷��R�����g�e�[�u���̖��́B</param>
            /// <param name="active">�L���܂��͖����B</param>
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
            /// �w�肵�����O�̃R�����g�e�[�u������莞�Ԍ�ɗL���܂��͖����ɂ��܂��B
            /// </summary>
            /// <param name="tableName">�w�肷��R�����g�e�[�u���̖��́B</param>
            /// <param name="active">�L���܂��͖����B</param>
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
                    // ����\������R�����g�̃e�[�u�������肷��
                    int index = ChoiceCommentTable();

                    // ����\������R�����g���擾����
                    string comment = ChoiceComment(index);

                        // �z����X�V����
                        bool repeatFlg = ArrayUpdate(index, comment, out commentArray, out lineAtComment);

                    // �R�����g��\������
                    OutputToTextUI();

                    // �R�����g��������Ȃ��ꍇ�s���R�Ȃ̂ŁA�Ԃ�u�����ɂ�����x�R�����g�𒊏o����
                    if (repeatFlg)
                        yield return null;
                    else
                        yield return new WaitForSeconds(interval);
                }
            }

            /// <summary>
            /// ���ݗL���ȃe�[�u�������I�сA���̓Y������ԋp���܂��B
            /// </summary>
            /// <returns>�I�����ꂽ�e�[�u���̓Y�����B</returns>
            private int ChoiceCommentTable()
            {
                // �L���ȃR�����g�e�[�u���̏o�����̍��v�����߂�
                float totalRate = 0;
                //Debug.Log("commentRate.count = " + commentRate.Count);
                for (int i = 0; i < commentRate.Count; ++i)
                {
                    //Debug.Log("Table = " + tableName[i] + ", isAcitive = " + tableActive[i]);
                    if (tableActive[i]) totalRate += commentRate[i];
                }

                // �S�̂̊��������Ƃɍ����rand�l�����߂�
                float rand = Random.Range(0.0f, 1.0f);
                float choiceRate = totalRate * rand;
                //Debug.Log("totalRate=" + totalRate);

                // rand�l�����Ƃɍ���I�������e�[�u�������肷��
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
            /// �w�肵���R�����g�e�[�u�����烉���_���ɂЂƂR�����g���擾���܂��B
            /// </summary>
            /// <param name="index">�w�肵���e�[�u���̓Y�����B</param>
            /// <returns>�擾�����R�����g�B</returns>
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
                // �R�����g������Ȃ��ƕs���R�Ȃ̂ŁA����Ȃ��������Ƃ����m����
                bool returnFlg = false;

                commentArray = this.commentArray;
                lineAtComment = this.lineAtComment;
                //Debug.Log("come = " + comment);
                // ����̃R�����g���K�v�Ƃ���s�������߂�
                int line = (comment.Length / charCountInLine) + 1;
                int lineWork = line;
                //Debug.Log("line = " + line);

                // �R�����g�����؂�Ȃ��悤�ɗ����s�������߂�
                while (line < lineAtComment.Length && lineAtComment[line] > 1) ++line;

                // �󂫂��������ꍇ�͂��̕������s�������炷
                for (int i = lineCount - 1; i > 0; --i)
                {
                    //Debug.Log("lineAtComment[i] = " + lineAtComment[i]);
                    if (lineAtComment[i] != 0) break;
                    --line;
                }
                line = Mathf.Max(line, 0);  // �}�C�i�X�ɂȂ�Ȃ��悤��
                //Debug.Log("line = " + line);

                // ���ɂ������R�����g�����߂��s��������
                int lineIndex = line;
                returnFlg = (line == 0);
                for (; lineIndex < lineCount; ++lineIndex)
                {
                    // LineAtComment��0�̂Ƃ��͂��̍s�͎g���Ă��Ȃ��̂ŏ������I������
                    if (lineAtComment[lineIndex] == 0) break;

                    commentArray[lineIndex - line] = commentArray[lineIndex];
                    lineAtComment[lineIndex - line] = lineAtComment[lineIndex];
                }

                // �󂢂��X�y�[�X�ɐV�����R�����g��������
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

                // �g�p���Ȃ����������������lineAtComment��0��������
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
