using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace UI
    {
        public class HelpButton : MonoBehaviour
        {
            bool menuOpen = false;
            int thisNumber = 0;
            [SerializeField] private Image panel;
            [SerializeField] private Image infoButton;
            [SerializeField] private Image leftButton;
            [SerializeField] private Image rightButton;
            [SerializeField] private GameObject panelObject;
            [SerializeField] private Sprite[] ruleSprites;
            [SerializeField] private Sprite[] infoButtonSprites = new Sprite[2];

            void Start()
            {

            }

            void Update()
            {
                
            }

            public void ButtonAction()
            {
                if (menuOpen)
                {
                    leftButton.gameObject.SetActive(false);
                    rightButton.gameObject.SetActive(false);
                    panel.gameObject.SetActive(false);
                    // panelObject.SetActive(false);
                    infoButton.sprite = infoButtonSprites[1];
                    menuOpen = false;
                }
                else
                {
                    leftButton.gameObject.SetActive(true);
                    rightButton.gameObject.SetActive(true);
                    // panelObject.SetActive(true);
                    panel.gameObject.SetActive(true);
                    panel.sprite = ruleSprites[0];
                    infoButton.sprite = infoButtonSprites[0];
                    thisNumber = 0;
                    menuOpen = true;
                }
            }

            public void LeftButtonAction()
            {
                if (thisNumber > 0) thisNumber--;
                panel.sprite = ruleSprites[thisNumber];
            }

            public void RightButtonAction()
            {
                if (thisNumber < ruleSprites.Length - 1) thisNumber++;
                panel.sprite = ruleSprites[thisNumber];
            }
        }

    }
}