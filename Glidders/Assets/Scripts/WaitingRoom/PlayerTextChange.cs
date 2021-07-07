using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class PlayerTextChange : MonoBehaviour
    {
        [SerializeField] Text[] playerDisp = new Text[4];

        private int playersNum = 0;

        // Start is called before the first frame update
        void Start()
        {
            NameTextSubstitution();
            NumberOfPlayers();
        }

        public void NameTextSubstitution()
        {
            for (int i = 0; i < Rule.maxPlayerCount; i++)
            {
                Debug.Log("PlayerDisp = " + playerDisp[i].text);
                if (playerDisp[i].text == "")
                {
                    playerDisp[i].text = SingletonDate.SingletonClass.MemberList[playersNum];
                    Debug.Log("–¼‘O‚Í: " + SingletonDate.SingletonClass.MemberList[playersNum]);
                    break;
                }
            }
        }

        public void NumberOfPlayers()
        {
            if (playersNum < Rule.maxPlayerCount - 1)
            {
                ++playersNum;
            }
            Debug.Log(playersNum);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

