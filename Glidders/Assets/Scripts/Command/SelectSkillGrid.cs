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
            [SerializeField] private Character.CharacterCore characterCore = default;
            private Character.SkillScriptableObject skillScriptableObject = default;

            private FieldIndex playerPosition = new FieldIndex(3, 2);

            [SerializeField] private GameObject inputObject;
            private Inputer.IInput input;

            [SerializeField] private GameObject cursorObject;
            private IGetCursorPosition getCursorPosition;

            private bool startFlag = true;

            private bool[,] selectableGridTable;

            private FieldIndexOffset[] testSelectArray;
            private FieldIndexOffset[] testAttackArray;

            private FieldIndex beforeCursorIndex = default;

            [SerializeField] private Graphic.HologramController hologramController;

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

            public void CommandUpdate()
            {
                commandInputFunctionTable[commandInput.GetInputNumber()]();
            }

            private void CommandNotInput()
            {
                CommandStart();
                int selectNumber = commandInput.GetSelectNumber();
                selectNumber = Mathf.Clamp(selectNumber, 0, tabTexts.Length);
                commandInfoText.text = commandInfoTextMessage[selectNumber];

                DisplayAttackGrid(SelectMoveGrid.testSelectGrid);
                SelectGrid();
            }

            private void CommandInput1()
            {
                commandInput.SetInputNumber(0);
                startFlag = true;
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
                if (!startFlag) return;
                startFlag = false;
                switch (selectSkillNumber)
                {
                    case 1:
                        testSelectArray = new FieldIndexOffset[]{
                                    new FieldIndexOffset(0, -1),
                                    new FieldIndexOffset(-1, 0),
                                    new FieldIndexOffset(0, 1),
                                    new FieldIndexOffset(1, 0)};
                        testAttackArray = new FieldIndexOffset[]{
                                    new FieldIndexOffset(0,0),
                                    new FieldIndexOffset(-1,0)};
                        break;
                    case 2:
                        testSelectArray = new FieldIndexOffset[]{
                                    new FieldIndexOffset(0, -1),
                                    new FieldIndexOffset(-1, 0),
                                    new FieldIndexOffset(0, 1),
                                    new FieldIndexOffset(1, 0)};
                        testAttackArray = new FieldIndexOffset[]{
                                    new FieldIndexOffset(0,0),
                                    new FieldIndexOffset(0,1),
                                    new FieldIndexOffset(0,-1)};
                        break;
                    case 3:
                        testSelectArray = new FieldIndexOffset[]{
                                    new FieldIndexOffset(0, -1),
                                    new FieldIndexOffset(-1, 0),
                                    new FieldIndexOffset(0, 1),
                                    new FieldIndexOffset(1, 0)};
                        testAttackArray = new FieldIndexOffset[]{
                                    new FieldIndexOffset(-1,0),
                                    new FieldIndexOffset(-2,0)};
                        break;
                }
                //skillScriptableObject = characterCore.GetSkillData(selectSkillNumber);
                DisplaySelectableGrid();
                cameraController.AddCarsor();
            }

            public void SetSkillNumber(int skillNumber)
            {
                selectSkillNumber = skillNumber;
            }

            private void DisplaySelectableGrid()
            {
                SetSelectableGrid(SelectMoveGrid.testSelectGrid);
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
                foreach (var selectGridOffset in testSelectArray /*skillScriptableObject.selectFieldIndexOffsetArray*/)
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
                commandInput.SetInputNumber(0);
                commandInput.SetSelectNumber(0);
                displayTileMap.ClearAttackTilemap();
                displayTileMap.ClearSelectableTileMap();
                cameraController.RemoveCarsor();
                startFlag = true;
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
                foreach (var attackGridOffset in testAttackArray)
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
