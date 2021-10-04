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
                    if (ruleSprites.Length != 1) leftButton.gameObject.SetActive(false);
                    if (ruleSprites.Length != 1) rightButton.gameObject.SetActive(false);
                    panel.gameObject.SetActive(false);
                    infoButton.sprite = infoButtonSprites[1];
                    menuOpen = false;
                }
                else
                {
                    if (ruleSprites.Length != 1) leftButton.gameObject.SetActive(true);
                    if (ruleSprites.Length != 1) rightButton.gameObject.SetActive(true);
                    panel.gameObject.SetActive(true);
                    panel.sprite = ruleSprites[0];
                    infoButton.sprite = infoButtonSprites[0];
                    thisNumber = 0;
                    menuOpen = true;
                }
            }

            public void LeftButtonAction()
            {
                thisNumber--;
                if (thisNumber < 0) thisNumber = ruleSprites.Length -1;
                panel.sprite = ruleSprites[thisNumber];
            }

            public void RightButtonAction()
            {
                thisNumber++;
                if (thisNumber >= ruleSprites.Length) thisNumber = 0;
                panel.sprite = ruleSprites[thisNumber];
            }
        }

    }
}