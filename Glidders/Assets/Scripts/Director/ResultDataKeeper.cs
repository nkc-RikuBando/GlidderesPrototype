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

            private void Start()
            {
                DontDestroyOnLoad(gameObject);
            }

            public void SetResultData(ResultDataStruct[] resultDataStructs, int playerCount)
            {
                this.resultDataStructs = resultDataStructs;
                this.playerCount = playerCount;
            }

            public void DestroyThisObject()
            {
                Destroy(gameObject);
            }
        }
    }
}
