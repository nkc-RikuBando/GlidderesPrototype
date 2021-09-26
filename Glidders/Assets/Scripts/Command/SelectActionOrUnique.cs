using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Command
    {
        public class SelectActionOrUnique : MonoBehaviour, ICommand
        {
            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            [SerializeField] private string[] tabTexts;
            [SerializeField] private Sprite[] tabIcons;
            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private string[] commandInfoTextMessage;

            private GameObject characterObject;
            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;

            private enum SelectCommand
            {
                COMMAND_NOT_INPUT,
                COMMAND_INPUT_1,
                COMMAND_INPUT_2,

                COMMAND_NUMBER
            }

            // Start is called before the first frame update
            void Start()
            {
                commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
                commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
            }

            // Update is called once per frame
            void Update()
            {

            }

            public void SetCharacterObject(GameObject gameObject)
            {
                characterObject = gameObject;
            }

            public void SetCommandTab()
            {
                Character.IGetCharacterCoreData getCharacterCoreData = characterObject.GetComponent<Character.IGetCharacterCoreData>();
                tabTexts = new string[] {
                    "通常スキル",
                    "ユニークスキル"
                };
                tabIcons = new Sprite[] {
                    getCharacterCoreData.GetSkillData(1).skillIcon,
                    getCharacterCoreData.GetUniqueData().skillIcon
                };
                setCommandTab.SetTab(commandSprite, tabTexts, tabIcons);
            }

            public void CommandStart()
            {
                SetCommandTab();

            }

            public void CommandUpdate()
            {
                commandInputFunctionTable[commandInput.GetInputNumber()]();
            }

            private void CommandNotInput()
            {
                int selectNumber = commandInput.GetSelectNumber();
                selectNumber = Mathf.Clamp(selectNumber, 0, tabTexts.Length);
                commandInfoText.text = commandInfoTextMessage[selectNumber];
            }

            private void CommandInput1()
            {
                commandInput.SetInputNumber(0);
                commandFlow.uniqueFlg = false;
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_ACTION_OR_UNIQUE);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_MOVE_GRID);
            }
            private void CommandInput2()
            {
                commandInput.SetInputNumber(0);
                commandFlow.uniqueFlg = true;
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_ACTION_OR_UNIQUE);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL);
            }
        }
    }
}
