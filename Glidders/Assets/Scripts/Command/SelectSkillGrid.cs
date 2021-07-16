using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Command
    {
        public class SelectSkillGrid : MonoBehaviour, ICommand
        {
            private GameObject characterObject;

            [SerializeField] private CameraController cameraController;

            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            [SerializeField] private string[] tabTexts;
            [SerializeField] private Sprite[] tabIcons;
            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private string[] commandInfoTextMessage;
            [SerializeField] private Field.FieldCore fieldCore;
            private Field.IGetFieldInformation getFieldInformation;
            private FieldIndex fieldSize;

            [SerializeField] private Field.DisplayTileMap displayTileMap;

            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;

            private int selectSkillNumber = default;
            private Character.SkillScriptableObject skillScriptableObject = default;

            private FieldIndex playerPosition = new FieldIndex(3, 2);

            [SerializeField] private GameObject inputObject;
            private Inputer.IInput input;

            [SerializeField] private GameObject cursorObject;
            private IGetCursorPosition getCursorPosition;

            private bool[,] selectableGridTable;

            private FieldIndexOffset[] testSelectArray;
            private FieldIndexOffset[] testAttackArray;

            private FieldIndex beforeCursorIndex = default;

            [SerializeField] private Graphic.HologramController hologramController;

            public FieldIndex selectIndex = default;

            [SerializeField] private CommandManager commandManager;

            private enum SelectCommand
            {
                COMMAND_NOT_INPUT,
                COMMAND_INPUT_1,

                COMMAND_NUMBER
            }

            // Start is called before the first frame update
            void Start()
            {
                getFieldInformation = fieldCore.GetComponent<Field.IGetFieldInformation>();
                getCursorPosition = cursorObject.GetComponent<IGetCursorPosition>();
                input = inputObject.GetComponent<Inputer.IInput>();
                commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
                commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
                fieldSize = getFieldInformation.GetFieldSize();
                selectableGridTable = new bool[fieldSize.row, fieldSize.column];
            }

            // Update is called once per frame
            void Update()
            {

            }

            public void SetCharacterObject(GameObject gameObject)
            {
                characterObject = gameObject;
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

                DisplayAttackGrid(selectIndex);
                SelectGrid();
            }

            private void CommandInput1()
            {
                commandInput.SetInputNumber(0);
                displayTileMap.ClearSelectableTileMap();
                cameraController.RemoveCarsor();
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL);

            }

            public void SetCommandTab()
            {
                setCommandTab.SetTab(commandSprite, tabTexts, tabIcons);
            }

            public void CommandStart()
            {
                SetCommandTab();

                Character.IGetCharacterCoreData getCharacterCoreData = characterObject.GetComponent<Character.IGetCharacterCoreData>();
                skillScriptableObject = getCharacterCoreData.GetSkillData(selectSkillNumber);
                DisplaySelectableGrid();
                cameraController.AddCarsor();
            }

            public void SetSkillNumber(int skillNumber)
            {
                selectSkillNumber = skillNumber;
            }

            private void DisplaySelectableGrid()
            {
                SetSelectableGrid(selectIndex);
                //displayTileMap.DisplaySelectableTileMap(selectableGridTable);
            }

            private void SetSelectableGrid(FieldIndex playerPosition)
            {
                for(int i = 0; i < fieldSize.row; i++)
                {
                    for(int j = 0; j < fieldSize.column; j++)
                    {
                        selectableGridTable[i, j] = false;
                    }
                }
                displayTileMap.ClearSelectableTileMap();
                foreach (var selectGridOffset in skillScriptableObject.selectFieldIndexOffsetArray)
                {
                    FieldIndex fieldIndex =  playerPosition + selectGridOffset;
                    if (!CheckInside(fieldIndex)) continue;
                    selectableGridTable[fieldIndex.row, fieldIndex.column] = true;
                    displayTileMap.DisplaySelectableTile(fieldIndex);
                }
            }

            private bool CheckInside(FieldIndex fieldIndex)
            {
                if (fieldIndex.column >= fieldCore.GetFieldSize().column) return false;
                if (fieldIndex.column <= 0) return false;
                if (fieldIndex.row >= fieldCore.GetFieldSize().row) return false;
                if (fieldIndex.row <= 0) return false;
                return true;
            }

            private void SelectGrid()
            {
                if (!(input.IsClickDown())) return;
                FieldIndex cursorIndex = getCursorPosition.GetCursorIndex();
                if (!CheckInside(cursorIndex)) return;
                if (!selectableGridTable[cursorIndex.row, cursorIndex.column]) return;
                FieldIndexOffset direction = cursorIndex - playerPosition;
                FieldIndexOffset direction01 = new FieldIndexOffset(
                    Mathf.Clamp(direction.rowOffset, -1, 1), Mathf.Clamp(direction.columnOffset, -1, 1));
                commandManager.SetAttackSignal(new Manager.AttackSignal(skillScriptableObject, cursorIndex, direction01, selectSkillNumber));
                commandInput.SetInputNumber(0);
                commandInput.SetSelectNumber(0);
                displayTileMap.ClearAttackTilemap();
                displayTileMap.ClearSelectableTileMap();
                cameraController.RemoveCarsor();
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_DIRECTION);
            }

            private void DisplayAttackGrid(FieldIndex playerPosition)
            {
                FieldIndex cursorIndex = getCursorPosition.GetCursorIndex();
                if (beforeCursorIndex == cursorIndex) return;
                displayTileMap.ClearAttackTilemap();
                beforeCursorIndex = cursorIndex;
                if (!CheckInside(cursorIndex)) return;
                if (!selectableGridTable[cursorIndex.row, cursorIndex.column]) return;
                FieldIndexOffset direction = cursorIndex - playerPosition;
                FieldIndexOffset direction01 = new FieldIndexOffset(
                    Mathf.Clamp(direction.rowOffset, -1, 1), Mathf.Clamp(direction.columnOffset, -1, 1));
                hologramController.SetHologramDirection(direction);
                foreach (var attackGridOffset in skillScriptableObject.attackFieldIndexOffsetArray)
                {
                    FieldIndexOffset offset = Character.SkillDirection.ChangeSkillDirection(attackGridOffset, direction);
                    FieldIndex fieldIndex = cursorIndex + offset;
                    if (!CheckInside(fieldIndex)) continue;
                    displayTileMap.DisplayAttackTilemap(fieldIndex);
                }
            }
        }
    }
}
