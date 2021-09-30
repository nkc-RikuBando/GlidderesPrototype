using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace UI
    {
        public class CommentOutput : MonoBehaviour
        {
            // コメントテーブルの一覧を格納するリスト。
            public List<string[]> commentTable;

            // コメントテーブルごとのコメント採用率
            public List<float> commentRate;

            public void Start()
            {
                
            }

            /*IEnumerator Output()
            {
                float totalRate = 0;
                foreach ()

                Random.Range(0, 1);
            }*/
        }
    }
}
