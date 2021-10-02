using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Director
    {
        public class ResultDataKeeper : MonoBehaviour
        {
            public ResultDataStruct[] resultDataStructs;
            public int playerCount;
            public int turnCount;

            private void Start()
            {
                DontDestroyOnLoad(gameObject);
            }

            public void SetResultData(ResultDataStruct[] resultDataStructs, int playerCount, int turnCount)
            {
                this.resultDataStructs = resultDataStructs;
                this.playerCount = playerCount;
                this.turnCount = turnCount;
            }

            public void DestroyThisObject()
            {
                Destroy(gameObject);
            }
        }
    }
}
