using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class SetCommandTab : MonoBehaviour
    {
        [SerializeField] GameObject commandImageObject;
        [SerializeField] GameObject[] commandTabs = new GameObject[4];

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetTab(Sprite commandSprite, string[] tabTexts, Sprite[] tabIcons)
        {
            commandImageObject.GetComponent<Image>().sprite = commandSprite;
            for (int i = 0; i < commandTabs.Length; i++)
            {
                if (i < tabTexts.Length)
                {
                    commandTabs[i].SetActive(true);
                    Text tabText = commandTabs[i].GetComponentInChildren<Text>();
                    tabText.text = tabTexts[i];
                    GameObject childObject = commandTabs[i].transform.Find("Command_Icon").gameObject;
                    Image tabImage = childObject.GetComponent<Image>();
                    tabImage.sprite = tabIcons[i];
                }
                else
                {
                    commandTabs[i].SetActive(false);
                }
            }
        }

        public GameObject[] GetCommandTabs()
        {
            return commandTabs;
        }
    }
}

