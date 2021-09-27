using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders;
using Glidders.Manager;
using Glidders.Graphic;
using Glidders.Director;

namespace Glidders
{
    namespace UI
    {
        public class SerializedPlayerInformation : MonoBehaviour
        {
            PlayerInformationUIOutput playerInformationUIOutput;
            IPlayerInformation playerInformation;

            [SerializeField, Tooltip("Player_Info")] Image[] playerInfoObjectArray = new Image[Rule.maxPlayerCount];
            /*[SerializeField, Tooltip("CoreManager")]*/
            GameObject coreManagerObject;
            [SerializeField, Tooltip("Player_Info_Icon_Sprite")] Sprite[] playerInfoSprite = new Sprite[(int)CharacterName.count];
            [SerializeField, Tooltip("Player_Info_Color_Sprite")] Sprite[] playerInfoColorSprite = new Sprite[Rule.maxPlayerCount];
            [SerializeField, Tooltip("Player_Rank_Sprite")] Sprite[] playerRankSprite = new Sprite[Rule.maxPlayerCount];
            [SerializeField, Tooltip("Player_Point_Sprite")] Sprite playerPointSprite;
            [SerializeField, Tooltip("Player_Energy_Sprite")] Sprite playerEnergySprite;
            [SerializeField, Tooltip("Player_Info_Icon_None_Sprite")] Sprite playerInfoIconNoneSprite;
            [SerializeField, Tooltip("Player_Info_None_Sprite")] Sprite playerInfoNoneSprite;
            [SerializeField] Image[] player1BuffImage;
            [SerializeField] Image[] player2BuffImage;
            [SerializeField] Image[] player3BuffImage;
            [SerializeField] Image[] player4BuffImage;

            public  void Start()
            {
                GameDirector gameDirector = GetComponent<GameDirector>();
                playerInformation = gameDirector.coreManagerObject.GetComponent<IPlayerInformation>();
                playerInformationUIOutput = new PlayerInformationUIOutput(gameDirector, playerInformation, playerInfoObjectArray,
                    playerInfoSprite, playerInfoColorSprite, playerRankSprite, playerPointSprite, playerEnergySprite, playerInfoIconNoneSprite, playerInfoNoneSprite,
                    player1BuffImage, player2BuffImage, player3BuffImage, player4BuffImage);
            }

            private void Update()
            {
                playerInformationUIOutput.Update();
            }
        }
    }
}
