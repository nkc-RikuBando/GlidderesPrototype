using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Director
    {
        public class ResultSceneDirector : MonoBehaviour
        {
            // UIオブジェクトたち
            // １位のプレイヤー
            public Image winnerCharacterImage;
            public Image winnerRankImage;
            public Text winnerCharacterName;
            public Text winnerPoint;
            public Text winnerTotalDamage;
            // ２〜４位のプレイヤー
            public Image[] characterFrame;
            public Image[] playerRank;
            public Text[] playerPoint;
            // ルール関連
            public Text ruleText;
            public Text stageText;

            [SerializeField, Tooltip("キャラクター画像")] public Sprite[] characterSprite;
            [SerializeField, Tooltip("順位画像")] public Sprite[] rankSprite;
            [SerializeField, Tooltip("フレーム画像")] public Sprite[] characterFrameSprite;

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

                SortResultData();
            }

            private void SortResultData()
            {
                // 第二キーの総ダメージ量で並び変える
                for (int i = resultDataStructs.Length - 1; i >= 0; --i)
                {
                    for (int j = 0; j < i; ++j)
                    {
                        if (resultDataStructs[j].totalDamage < resultDataStructs[j + 1].totalDamage)
                        {
                            ResultDataStruct work = resultDataStructs[j];
                            resultDataStructs[j] = resultDataStructs[j + 1];
                            resultDataStructs[j + 1] = work;
                        }
                    }
                }
                // 第一キーの所有ポイント（体力）で並び変える
                for (int i = resultDataStructs.Length - 1; i >= 0; --i)
                {
                    for (int j = 0; j < i; ++j)
                    {
                        if (resultDataStructs[j].point < resultDataStructs[j + 1].point)
                        {
                            ResultDataStruct work = resultDataStructs[j];
                            resultDataStructs[j] = resultDataStructs[j + 1];
                            resultDataStructs[j + 1] = work;
                        }
                    }
                }
            }

            private void ResultOutput()
            {
                // １位のプレイヤーの情報を表示
                winnerCharacterImage.sprite = characterSprite[(int)resultDataStructs[0].characterId];
                winnerRankImage.sprite = rankSprite[0];
                winnerCharacterName.text = resultDataStructs[0].playerName;
                if (resultDataStructs[0].ruleType == 0)
                    winnerPoint.text = resultDataStructs[0].point + " pt";
                else
                    winnerPoint.text = "HP:" + resultDataStructs[0].point;
                winnerTotalDamage.text = resultDataStructs[0].totalDamage + " damage";

                // ２〜４位のプレイヤーの情報を表示
                for (int i = 1; i < resultDataStructs.Length; ++i)
                {
                    characterFrame[i - 1].sprite = characterFrameSprite[(int)resultDataStructs[i].characterId];
                    playerRank[i - 1].sprite = rankSprite[i];
                    if (resultDataStructs[i].ruleType == 0)
                        playerPoint[i - 1].text = resultDataStructs[i].point + " pt";
                    else
                        playerPoint[i - 1].text = "HP:" + resultDataStructs[i].point;
                }

                // ルール関連の情報を表示

            }
        }
    }
}
