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
            // UI�I�u�W�F�N�g����
            // �P�ʂ̃v���C���[
            public Image winnerCharacterImage;
            public Image winnerRankImage;
            public Text winnerCharacterName;
            public Text winnerPoint;
            public Text winnerTotalDamage;
            // �Q�`�S�ʂ̃v���C���[
            public Image[] characterFrame;
            public Image[] playerRank;
            public Image[] playerPointPanel;
            public Text[] playerPoint;
            // ���[���֘A
            public Text ruleText;
            public Text stageText;

            [SerializeField, Tooltip("�L�����N�^�[�摜")] public Sprite[] characterSprite;
            [SerializeField, Tooltip("���ʉ摜")] public Sprite[] rankSprite;
            [SerializeField, Tooltip("�t���[���摜")] public Sprite[] characterFrameSprite;
            [SerializeField, Tooltip("�����摜")] public Sprite invisibleSprite;
            [SerializeField, Tooltip("�Q�`�S�� �|�C���g�p�l��")] public Sprite pointPanel;
            [SerializeField, Tooltip("�R�����g��")] public Text commentTextUI;

            GameObject dataKeeperForResultScene;
            ResultDataKeeper resultDataKeeper;

            ResultDataStruct[] resultDataStructs;
            int playerCount;
            // Start is called before the first frame update
            void Start()
            {
                dataKeeperForResultScene = GameObject.Find("ResultDataKeeper");
                resultDataKeeper = dataKeeperForResultScene.GetComponent<ResultDataKeeper>();

                resultDataStructs = resultDataKeeper.resultDataStructs;
                playerCount = resultDataKeeper.playerCount;

                // �R�����g�̕\��
                CommentOutput commentOutput = GameObject.Find("CommentOutputSystem").GetComponent<CommentOutput>();
                commentOutput.SetTextUI(commentTextUI);
                commentOutput.StartComment();

                SortResultData();
                SetInvisible();
                ResultOutput();
            }

            private void SortResultData()
            {
                // ���L�[�̑��_���[�W�ʂŕ��ѕς���
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
                // ���L�[�̏��L�|�C���g�i�̗́j�ŕ��ѕς���
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
                // �P�ʂ̃v���C���[�̏���\��
                winnerCharacterImage.sprite = characterSprite[(int)resultDataStructs[0].characterId];
                winnerRankImage.sprite = rankSprite[0];
                winnerCharacterName.text = resultDataStructs[0].playerName;
                if (resultDataStructs[0].ruleType == 0)
                    winnerPoint.text = resultDataStructs[0].point + " pt";
                else
                    winnerPoint.text = "HP:" + resultDataStructs[0].point;
                winnerTotalDamage.text = resultDataStructs[0].totalDamage + " damage";

                // �Q�`�S�ʂ̃v���C���[�̏���\��
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

                // ���[���֘A�̏���\��
                if (resultDataStructs[0].ruleType == 0)
                    ruleText.text = string.Format($"�|�C���g�� {resultDataKeeper.turnCount}�^�[��");
                else
                    ruleText.text = string.Format($"�̗͐� HP:{resultDataKeeper.turnCount}k");
                stageText.text = "�X�^���_�[�h���Z��";
            }
        }
    }
}
