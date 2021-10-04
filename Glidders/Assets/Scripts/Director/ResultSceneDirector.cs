using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.UI;

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
            public Image winnerCharacterNameBack;
            public Text winnerPoint;
            public Text winnerTotalDamage;
            // ２～４位のプレイヤー
            public Image[] characterFrame;
            public Image[] playerRank;
            public Image[] playerPointPanel;
            public Text[] playerPoint;
            // ルール関連
            public Text ruleText;
            public Text stageText;

            [SerializeField, Tooltip("キャラクター画像")] public Sprite[] characterSprite;
            [SerializeField, Tooltip("順位画像")] public Sprite[] rankSprite;
            [SerializeField, Tooltip("フレーム画像")] public Sprite[] characterFrameSprite;
            [SerializeField, Tooltip("キャラ名の背景色")] public Sprite[] characterBackColor;
            [SerializeField, Tooltip("透明画像")] public Sprite invisibleSprite;
            [SerializeField, Tooltip("２～４位 ポイントパネル")] public Sprite pointPanel;
            [SerializeField, Tooltip("コメント欄")] public Text commentTextUI;

            GameObject dataKeeperForResultScene;
            ResultDataKeeper resultDataKeeper;
            CommentOutput commentOutput;

            ResultDataStruct[] resultDataStructs;
            int playerCount;
            // Start is called before the first frame update
            void Start()
            {
                dataKeeperForResultScene = GameObject.Find("ResultDataKeeper");
                resultDataKeeper = dataKeeperForResultScene.GetComponent<ResultDataKeeper>();
                commentOutput = GameObject.Find("CommentOutputSystem").GetComponent<CommentOutput>();

                resultDataStructs = resultDataKeeper.resultDataStructs;
                playerCount = resultDataKeeper.playerCount;

                // コメントの表示とテーブルの有効化
                commentOutput.SetTextUI(commentTextUI);
                commentOutput.StartComment();
                commentOutput.SetTableActive("リザルト汎用１", true);
                commentOutput.SetInterval(Comment.interval_short);

                StartCoroutine(GoToTitleScene());
                SortResultData();
                SetInvisible();
                ResultOutput();

                resultDataKeeper.DestroyThisObject();
            }

            IEnumerator GoToTitleScene()
            {
                // 5秒間はTitleSceneへ遷移しないようにする
                yield return new WaitForSeconds(5.0f);

                while(true)
                {
                    if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
                    {
                        commentOutput.SetTableActive("リザルト汎用１", false);
                        commentOutput.StopComment();
                        //commentOutput.DestroyThisObject();
                        FadeManager.Instance.LoadScene("MenuScene", 0.5f);
                        yield return new WaitForSeconds(0.5f);
                        StopCoroutine(GoToTitleScene());
                    }
                    yield return null;
                }
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

            private void SetInvisible()
            {
                for (int i = 1; i < Rule.maxPlayerCount; ++i)
                {
                    characterFrame[i - 1].sprite = invisibleSprite;
                    playerRank[i - 1].sprite = invisibleSprite;
                    playerPointPanel[i - 1].sprite = invisibleSprite;
                    playerPoint[i - 1].text = "";
                }
            }

            private void ResultOutput()
            {
                // １位のプレイヤーの情報を表示
                winnerCharacterImage.sprite = characterSprite[(int)resultDataStructs[0].characterId];
                winnerRankImage.sprite = rankSprite[0];
                winnerCharacterName.text = resultDataStructs[0].characterId.ToString();
                winnerCharacterNameBack.sprite = characterBackColor[(int)resultDataStructs[0].characterId];
                if (resultDataStructs[0].ruleType == 0)
                    winnerPoint.text = resultDataStructs[0].point + " pt";
                else
                    winnerPoint.text = "HP:" + resultDataStructs[0].point;
                winnerTotalDamage.text = resultDataStructs[0].totalDamage + " damage";

                // ２～４位のプレイヤーの情報を表示
                for (int i = 1; i < resultDataStructs.Length; ++i)
                {
                    characterFrame[i - 1].sprite = characterFrameSprite[(int)resultDataStructs[i].characterId];
                    playerRank[i - 1].sprite = rankSprite[i];
                    playerPointPanel[i - 1].sprite = pointPanel;
                    if (resultDataStructs[i].ruleType == 0)
                        playerPoint[i - 1].text = resultDataStructs[i].point + " pt";
                    else
                        playerPoint[i - 1].text = "HP:" + resultDataStructs[i].point;
                }

                // ルール関連の情報を表示
                if (resultDataStructs[0].ruleType == 0)
                    ruleText.text = string.Format($"ポイント制 {resultDataKeeper.turnCount}ターン");
                else
                    ruleText.text = string.Format($"体力制 HP:{ActiveRule.startPoint}k");
                stageText.text = "スタンダード闘技場";
            }
        }
    }
}
