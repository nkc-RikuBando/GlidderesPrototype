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

            // �R�����g���ɕ\�����镶������Ǘ�����z��
            string[] commentArray;
            // commentArryay�̕����񂪂ЂƂ̃R�����g���ŉ��s�ڂɊY�����邩���Ǘ�����z��
            int[] lineAtComment;

            private void Start()
            {
                // �z��̗v�f����ݒ肵����������
                commentArray = new string[lineCount];
                lineAtComment = new int[lineCount];
                for (int i = 0; i < lineCount; ++i)
                {
                    commentArray[i] = "";
                    lineAtComment[i] = 0;
                }
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

            IEnumerator Output()
            {
                while (true)
                {
                    // ����\������R�����g�̃e�[�u�������肷��
                    int index = ChoiceCommentTable();

                    // ����\������R�����g���擾����
                    string comment = ChoiceComment(index);

                    // �z����X�V����
                    ArrayUpdate(index, comment, out commentArray, out lineAtComment);

                    yield return null;
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
                for (int i = 0; i < commentRate.Count; ++i)
                {
                    if (tableActive[i]) totalRate += commentRate[i];
                }

                // �S�̂̊��������Ƃɍ����rand�l�����߂�
                float rand = Random.Range(0.0f, 1.0f);
                float choiceRate = totalRate * rand;

                // rand�l�����Ƃɍ���I�������e�[�u�������肷��
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
                int rand = Random.Range(startIndex, endIndex);
                return commentTable[rand];
            }

            private void ArrayUpdate(int index, string comment, out string[] commentArray, out int[] lineAtComment)
            {
                commentArray = this.commentArray;
                lineAtComment = this.lineAtComment;

                // ����̃R�����g���K�v�Ƃ���s�������߂�
                int line = (comment.Length / charCountInLine) + 1;

                // �󂫂��������ꍇ�͂��̕������s�������炷
                for (int i = lineCount; i > 0; --i)
                {
                    if (lineAtComment[i] != 0) break;
                    --line;
                }

                // �R�����g�����؂�Ȃ��悤�ɗ����s�������߂�
                while (lineAtComment[line] != 1) ++line;

                // ���ɂ������R�����g�����߂��s��������
                int lineIndex = line;
                for (; lineIndex < lineCount; ++lineIndex)
                {
                    // LineAtComment��0�̂Ƃ��͂��̍s�͎g���Ă��Ȃ��̂ŏ������I������
                    if (lineAtComment[lineIndex] == 0) break;

                    commentArray[lineIndex - line] = commentArray[lineIndex];
                    lineAtComment[lineIndex - line] = lineAtComment[lineIndex];
                }

                // �󂢂��X�y�[�X�ɐV�����R�����g��������
                lineIndex -= line;
                for (int i = 0; i < line; ++i)
                {
                    commentArray[lineIndex + i] = comment.Substring(0, charCountInLine);
                    lineAtComment[lineIndex + i] = i + 1;
                    comment = comment.Substring(charCountInLine);
                }

                // �g�p���Ȃ����������������lineAtComment��0��������
                for(; lineIndex < lineCount; ++lineIndex)
                {
                    commentArray[lineIndex] = "";
                    lineAtComment[lineIndex] = 0;
                }
            }
        }
    }
}
