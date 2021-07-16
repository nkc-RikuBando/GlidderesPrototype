using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Director
    {
        public class ResultSceneDirector : MonoBehaviour
        {
            GameObject dataKeeperForResultScene;
            ResultDataKeeper resultDataKeeper;

            ResultDataStruct[] resultDataStructs;
            int playerCount;
            // Start is called before the first frame update
            void Start()
            {
                dataKeeperForResultScene = GameObject.Find("DataKeeperForResultScene");
                resultDataKeeper = dataKeeperForResultScene.GetComponent<ResultDataKeeper>();

                resultDataStructs = resultDataKeeper.resultDataStructs;
                playerCount = resultDataKeeper.playerCount;
                // çÏÇËÇ©ÇØ
            }

            private void ResultOutput()
            {

            }
        }
    }
}
