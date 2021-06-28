using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Command
{
    public class SelectActionOrUnique : MonoBehaviour, ICommand
    {
        [SerializeField] private CommandInput commandInput;

        [SerializeField] private Sprite commandSprite;
        [SerializeField] private string[] tabTexts;
        [SerializeField] private Sprite[] tabIcons;
        [SerializeField] private SetCommandTab setCommandTab;

        private delegate void CommandInput();
        private CommandInput[] commandInputs;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SetCommandTab()
        {
            setCommandTab.SetTab(commandSprite, tabTexts, tabIcons);
        }

        public void CommandUpdate()
        {
            
        }

        private void CommandNotInput()
        {

        }

        private void CommandInput1()
        {

        }
        private void CommandInput2()
        {

        }
    }
}
