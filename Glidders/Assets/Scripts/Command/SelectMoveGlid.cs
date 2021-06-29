using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Command
    {
        public class SelectMoveGlid : MonoBehaviour, ICommand
        {
            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            [SerializeField] private string[] tabTexts;
            [SerializeField] private Sprite[] tabIcons;
            [SerializeField] private SetCommandTab setCommandTab;

            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
            public void CommandUpdate()
            {
                if (!Input.GetKeyDown(KeyCode.Return)) return;
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL);

            }

            public void SetCommandTab()
            {
                setCommandTab.SetTab(commandSprite, tabTexts, tabIcons);
            }


        }
    }
}

