using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Manager;
using Glidders.Director;
using Glidders;

namespace Glidders
{
    namespace Graphic
    {
        public class PlayerInformationUIOutput
        {
            Image[] playerInfoObjectArray = new Image[Rule.maxPlayerCount];
            /*[SerializeField, Tooltip("CoreManager")]*/ GameObject coreManagerObject;
            Sprite[] playerInfoSprite = new Sprite[(int)CharacterName.count];
            Sprite[] playerInfoColorSprite = new Sprite[Rule.maxPlayerCount];
            Sprite[] playerRankSprite = new Sprite[Rule.maxPlayerCount];
            Sprite playerPointSprite;
            Sprite playerEnergySprite;
            Sprite playerInfoIconNoneSprite;
            Sprite playerInfoNoneSprite;
            Image[] player1BuffImage;
            Image[] player2BuffImage;
            Image[] player3BuffImage;
            Image[] player4BuffImage;
            List<Image[]> buffImageArray = new List<Image[]>();
            PlayerInformationUIObject[] playerInformationUIArray;
            IPlayerInformation playerInformation;
            GameDirector gameDirector;

            public void SetCore(GameDirector gameDirector, IPlayerInformation playerInformation)
            {
                this.gameDirector = gameDirector;
                this.playerInformation = playerInformation;
            }

            public void SetObject(Image[] playerInfoObjectArray, Sprite[] playerInfoSprite,
                Sprite[] playerInfoColorSprite, Sprite[] playerRankSprite, Sprite playerPointSprite, Sprite playerEnergySprite,
                Sprite playerInfoIconNoneSprite, Sprite playerInfoNoneSprite, Image[] player1BuffImage, Image[] player2BuffImage,
                Image[] player3BuffImage, Image[] player4BuffImage)
            {

                this.playerInfoObjectArray = playerInfoObjectArray;
                this.playerInfoSprite = playerInfoSprite;
                this.playerInfoColorSprite = playerInfoColorSprite;
                this.playerRankSprite = playerRankSprite;
                this.playerPointSprite = playerPointSprite;
                this.playerEnergySprite = playerEnergySprite;
                this.playerInfoIconNoneSprite = playerInfoIconNoneSprite;
                this.playerInfoNoneSprite = playerInfoNoneSprite;
                this.player1BuffImage = player1BuffImage;
                buffImageArray.Add(this.player1BuffImage);
                this.player2BuffImage = player2BuffImage;
                buffImageArray.Add(this.player2BuffImage);
                this.player3BuffImage = player3BuffImage;
                buffImageArray.Add(this.player3BuffImage);
                this.player4BuffImage = player4BuffImage;
                buffImageArray.Add(this.player4BuffImage);

                SetPlayerInformationUI(out playerInformationUIArray);
            }

            public UICharacterDataSeter[] DataSeter()
            {
                return playerInformation.characterDataSeter();
            }

            public void Output(UICharacterDataSeter[] characterData)
            {
                PlayerInformationUIValueSetter(characterData);
            }

            private void PlayerInformationUIValueSetter(UICharacterDataSeter[] characterData)
            {
                // èáà ÇãÅÇﬂÇÈîzóÒÇíËã`ÇµÅAèâä˙âªÇ∑ÇÈ
                int[] pointRank = new int[ActiveRule.playerCount];
                for (int i = 0; i < pointRank.Length; i++) pointRank[i] = 1;

                // åªç›èáà ÇãÅÇﬂÇÈ
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
                    if (i < ActiveRule.playerCount)
                    {
                        // UI
                        //Debug.Log("playerInformationUIArray " + playerInformationUIArray.Length);
                        //Debug.Log("characterData " + characterData.Length);
                        //Debug.Log("playerInfoSprite " + playerInfoSprite.Length);
                        //Debug.Log("i " + i);
                        playerInformationUIArray[i].player_Info.sprite = playerInfoSprite[(int)characterData[i].characterID];
                        playerInformationUIArray[i].player_Info_Color.sprite = playerInfoColorSprite[characterData[i].playerID];
                        playerInformationUIArray[i].player_Rank.sprite = playerRankSprite[pointRank[i] - 1];
                        playerInformationUIArray[i].player_Name.text = characterData[i].playerName;
                        playerInformationUIArray[i].player_Point_Icon.sprite = playerPointSprite;
                        playerInformationUIArray[i].player_Point.text = string.Format("{0:#######} pt", characterData[i].point);
                        playerInformationUIArray[i].player_Energy_Icon.sprite = playerEnergySprite;
                        playerInformationUIArray[i].player_Energy.text = characterData[i].energy.ToString();
                        for(int j = 0; j < 4; ++j)
                        {
                            buffImageArray[i][j].sprite = playerInfoNoneSprite;
                            if (j >= characterData[i].buffSpriteList.Count) continue;
                            if (characterData[i].buffSpriteList[j] == "") continue;
                            buffImageArray[i][j].sprite = ScriptableObjectDatabase.GetBuff(characterData[i].buffSpriteList[j]).buffIcon;
                        }
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