using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders;
using Glidders.Manager;
using Glidders.Graphic;
using Glidders.Director;
using Photon.Pun;

namespace Glidders
{
    namespace UI
    {
        public class SerializedPlayerInformation : MonoBehaviour
        {
            PlayerInformationUIOutput playerInformationUIOutput;
            IPlayerInformation playerInformation;

            PhotonView view;

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
                StartCoroutine(StartAfterGameDirector());
            }

            IEnumerator StartAfterGameDirector()
            {
                view = GetComponent<PhotonView>();
                GameDirector gameDirector = GetComponent<GameDirector>();
                while (!gameDirector.completeStart) yield return null;
                playerInformation = gameDirector.coreManagerObject.GetComponent<IPlayerInformation>();
                playerInformationUIOutput = new PlayerInformationUIOutput();
                playerInformationUIOutput.SetCore(gameDirector, playerInformation);
                playerInformationUIOutput.SetObject(playerInfoObjectArray,
                    playerInfoSprite, playerInfoColorSprite, playerRankSprite, playerPointSprite, playerEnergySprite, playerInfoIconNoneSprite, playerInfoNoneSprite,
                    player1BuffImage, player2BuffImage, player3BuffImage, player4BuffImage);
            }

            private void Update()
            {
                if (!PhotonNetwork.IsMasterClient) return;
                UICharacterDataSeter[] characterData = playerInformationUIOutput.DataSeter();

                string[] playerName = new string[characterData.Length];
                int[] characterName = new int[characterData.Length];
                int[] point = new int[characterData.Length];
                int[] enegy = new int[characterData.Length];
                List<string>[] buffDataID = new List<string>[characterData.Length];

                for (int i = 0;i < characterData.Length;i++)
                {
                    playerName[i] = characterData[i].playerName;
                    characterName[i] = (int)characterData[i].characterID;
                    point[i] = characterData[i].point;
                    enegy[i] = characterData[i].energy;
                    buffDataID[i] = characterData[i].buffSpriteList;
                }

                view.RPC(nameof(UpdateWithRPC), RpcTarget.All, playerName[0],playerName[1],playerName[2],playerName[3],
                    characterName[0],characterData[1],characterData[2],characterData[3],
                    point[0],point[1],point[2],point[3],
                    enegy[0],enegy[1],enegy[2],enegy[3],
                    buffDataID[0][0],buffDataID[0][1],buffDataID[0][2],buffDataID[0][3],
                    buffDataID[1][0],buffDataID[1][1],buffDataID[1][2],buffDataID[1][3],
                    buffDataID[2][0],buffDataID[2][1],buffDataID[2][2],buffDataID[2][3],
                    buffDataID[3][0],buffDataID[3][1],buffDataID[3][2],buffDataID[3][3]);
            }

            [PunRPC]
            private void UpdateWithRPC(string playerName_zero,string playerName_one,string playerName_two,string playerName_three,
                int characterName_zero,int characterName_one,int characterName_two,int characterName_three,
                int point_zero,int point_one,int point_two,int point_three,
                int energy_zero,int energy_one,int energy_two,int energy_three,
                string buff_zero_zero,string buff_zero_one,string buff_zero_two,string buff_zero_three,
                string buff_one_zero,string buff_one_one,string buff_one_two,string buff_one_three,
                string buff_two_zero,string buff_two_one,string buff_two_two,string buff_two_three,
                string buff_three_zero,string buff_three_one,string buff_three_two,string buff_three_three)
            {

                UICharacterDataSeter[] uICharacterData = new UICharacterDataSeter[4];
                for (int i = 0;i < uICharacterData.Length;i++)
                {
                    uICharacterData[i].buffSpriteList = new List<string>();
                }

                uICharacterData[0].playerName = playerName_zero;
                uICharacterData[1].playerName = playerName_one;
                uICharacterData[2].playerName = playerName_two;
                uICharacterData[3].playerName = playerName_three;
                uICharacterData[0].characterID = (CharacterName)characterName_zero;
                uICharacterData[1].characterID = (CharacterName)characterName_one;
                uICharacterData[2].characterID = (CharacterName)characterName_two;
                uICharacterData[3].characterID = (CharacterName)characterName_three;
                uICharacterData[0].point = point_zero;
                uICharacterData[1].point = point_one;
                uICharacterData[2].point = point_two;
                uICharacterData[3].point = point_three;
                uICharacterData[0].energy = energy_zero;
                uICharacterData[1].energy = energy_one;
                uICharacterData[2].energy = energy_two;
                uICharacterData[3].energy = energy_three;
                uICharacterData[0].buffSpriteList[0] = buff_zero_zero;
                uICharacterData[0].buffSpriteList[1] = buff_zero_one;
                uICharacterData[0].buffSpriteList[2] = buff_zero_two;
                uICharacterData[0].buffSpriteList[3] = buff_zero_three;
                uICharacterData[1].buffSpriteList[0] = buff_one_zero;
                uICharacterData[1].buffSpriteList[1] = buff_one_one;
                uICharacterData[1].buffSpriteList[2] = buff_one_two;
                uICharacterData[1].buffSpriteList[3] = buff_one_three;
                uICharacterData[2].buffSpriteList[0] = buff_two_zero;
                uICharacterData[2].buffSpriteList[1] = buff_two_one;
                uICharacterData[2].buffSpriteList[2] = buff_two_two;
                uICharacterData[2].buffSpriteList[2] = buff_two_three;
                uICharacterData[3].buffSpriteList[0] = buff_three_zero;
                uICharacterData[3].buffSpriteList[1] = buff_three_one;
                uICharacterData[3].buffSpriteList[2] = buff_three_two;
                uICharacterData[3].buffSpriteList[3] = buff_three_three;


                playerInformationUIOutput.Output(uICharacterData);
            }
        }
    }
}
