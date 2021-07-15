using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Manager;
using Glidders.Director;

namespace Glidders
{
    namespace Graphic
    {
        public class PlayerInformationUIOutput : MonoBehaviour
        {
            [SerializeField, Tooltip("Player_Info")] Image[] playerInfoObjectArray = new Image[Rule.maxPlayerCount];
            /*[SerializeField, Tooltip("CoreManager")]*/ GameObject coreManagerObject;
            [SerializeField, Tooltip("Player_Info_Icon_Sprite")] Sprite[] playerInfoSprite = new Sprite[(int)CharacterName.count];
            [SerializeField, Tooltip("Player_Info_Color_Sprite")] Sprite[] playerInfoColorSprite = new Sprite[Rule.maxPlayerCount];
            [SerializeField, Tooltip("Player_Rank_Sprite")] Sprite[] playerRankSprite = new Sprite[Rule.maxPlayerCount];
            [SerializeField, Tooltip("Player_Point_Sprite")] Sprite playerPointSprite;
            [SerializeField, Tooltip("Player_Energy_Sprite")] Sprite playerEnergySprite;
            [SerializeField, Tooltip("Player_Info_Icon_None_Sprite")] Sprite playerInfoIconNoneSprite;
            [SerializeField, Tooltip("Player_Info_None_Sprite")] Sprite playerInfoNoneSprite;
            UICharacterDataSeter[] characterData;
            PlayerInformationUIObject[] playerInformationUIArray;
            IPlayerInformation playerInformation;
            GameDirector gameDirector;
            private void Start()
            {
                gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
                coreManagerObject = gameDirector.coreManagerObject;
                playerInformation = coreManagerObject.GetComponent<IPlayerInformation>();

                characterData = playerInformation.characterDataSeter();
                SetPlayerInformationUI(out playerInformationUIArray);
            }

            private void Update()
            {
                PlayerInformationUIValueSetter();
            }

            private void PlayerInformationUIValueSetter()
            {
                // ���ʂ����߂�z����`���A����������
                int[] pointRank = new int[Rule.maxPlayerCount];
                for (int i = 0; i < pointRank.Length; i++) pointRank[i] = 1;

                // ���ݏ��ʂ����߂�
                for ( int i = 0; i < pointRank.Length - 1; i++)
                {
                    for (int j = i + 1; j < pointRank.Length; j++)
                    {
                        if (characterData[i].point < characterData[j].point) pointRank[i]++;
                        if (characterData[i].point > characterData[j].point) pointRank[j]++;
                    }
                }

                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    if (i < gameDirector.playerCount)
                    {
                        // UI
                        playerInformationUIArray[i].player_Info.sprite = playerInfoSprite[(int)characterData[i].characterID];
                        playerInformationUIArray[i].player_Info_Color.sprite = playerInfoColorSprite[characterData[i].playerID];
                        playerInformationUIArray[i].player_Rank.sprite = playerRankSprite[pointRank[i] - 1];
                        playerInformationUIArray[i].player_Name.text = characterData[i].playerName;
                        playerInformationUIArray[i].player_Point_Icon.sprite = playerPointSprite;
                        playerInformationUIArray[i].player_Point.text = string.Format("{0:#######} pt", characterData[i].point);
                        playerInformationUIArray[i].player_Energy_Icon.sprite = playerEnergySprite;
                        playerInformationUIArray[i].player_Energy.text = characterData[i].energy.ToString();
                    }
                    else
                    {
                        playerInformationUIArray[i].player_Info.sprite = playerInfoIconNoneSprite;
                        playerInformationUIArray[i].player_Info_Color.sprite = playerInfoNoneSprite;
                        playerInformationUIArray[i].player_Rank.sprite = playerInfoNoneSprite;
                        playerInformationUIArray[i].player_Name.text = "";
                        playerInformationUIArray[i].player_Point_Icon.sprite = playerInfoNoneSprite;
                        playerInformationUIArray[i].player_Point.text = "";
                        playerInformationUIArray[i].player_Energy_Icon.sprite = playerInfoNoneSprite;
                        playerInformationUIArray[i].player_Energy.text = "";
                    }
                }
            }

            private void SetPlayerInformationUI(out PlayerInformationUIObject[] setArray)
            {
                PlayerInformationUIObject[] workArray = new PlayerInformationUIObject[Rule.maxPlayerCount];
                for (int i = 0; i < workArray.Length; i++)
                {
                    workArray[i].player_Info = playerInfoObjectArray[i];
                    workArray[i].player_Info_Color = playerInfoObjectArray[i].transform.Find("Player_Info_Color").gameObject.GetComponent<Image>();
                    workArray[i].player_Rank = playerInfoObjectArray[i].transform.Find("Player_Rank").gameObject.GetComponent<Image>();
                    workArray[i].player_Name = playerInfoObjectArray[i].transform.Find("Player_Name").gameObject.GetComponent<Text>();
                    workArray[i].player_Point_Icon = playerInfoObjectArray[i].transform.Find("Player_Pt_Icon").gameObject.GetComponent<Image>();
                    workArray[i].player_Point = playerInfoObjectArray[i].transform.Find("Player_Point").gameObject.GetComponent<Text>();
                    workArray[i].player_Energy_Icon = playerInfoObjectArray[i].transform.Find("Player_Energy_Icon").gameObject.GetComponent<Image>();
                    workArray[i].player_Energy = playerInfoObjectArray[i].transform.Find("Player_Energy").gameObject.GetComponent<Text>();
                }

                setArray = workArray;
            }
        }

        public struct PlayerInformationUIObject
        {
            public Image player_Info;
            public Image player_Info_Color;
            public Image player_Rank;
            public Text player_Name;
            public Image player_Point_Icon;
            public Text player_Point;
            public Image player_Energy_Icon;
            public Text player_Energy;
        }
    }
}