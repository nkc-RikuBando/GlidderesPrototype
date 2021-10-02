using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Command
    {
        public class SelectMoveGrid : MonoBehaviour, ICommand
        {
            private GameObject characterObject = default;

            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            [SerializeField] private Sprite infoSprite;
            [SerializeField] private string[] tabTexts;
            [SerializeField] private Sprite[] tabIcons;
            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private string[] commandInfoTextMessage;
            [SerializeField] private Field.FieldCore fieldCore;

            [SerializeField] private GameObject iGetFieldInformationObject;
            [SerializeField] private Field.DisplayTileMap displayTileMap;

            [SerializeField] CameraController cameraController;

            [SerializeField] private GameObject cursorObject;
            private IGetCursorPosition getCursorPosition;

            [SerializeField] private GameObject inputObject;
            private Inputer.IInput input;

            private delegate void CommandInputFunction();
            private CommandInputFunction[] commandInputFunctionTable;
            private Field.IGetFieldInformation getFieldInformation;

            private bool[,] selectableGridTable;

            private int move = 2;
            private FieldIndex playerPosition = new FieldIndex(3, 2);

            private bool isDrag = false;
            private FieldIndex[] movePositionTable = new FieldIndex[6];
            private FieldIndexOffset[] moveOffsetTable = new FieldIndexOffset[5];

            private FieldIndex previousPosition;

            private FieldIndex fieldSize;
            private FieldIndex selectGlid;

            private bool isFixed = false;

            private Character.UniqueSkillScriptableObject UniqueSkillScriptableObject = default;

            [SerializeField] private Graphic.HologramController hologramController;

            [SerializeField] private SelectSkillGrid skillGrid;

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
                input = inputObject.GetComponent<Inputer.IInput>();
                getCursorPosition = cursorObject.GetComponent<IGetCursorPosition>();
                getFieldInformation = iGetFieldInformationObject.GetComponent<Field.IGetFieldInformation>();
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

            public void CommandStart()
            {
                SetCommandTab();
                playerPosition = commandFlow.GetCharacterPosition();
                move = characterObject.GetComponent<Character.IGetCharacterCoreData>().GetMoveAmount() + commandFlow.plMoveBuff;
                isFixed = (characterObject.GetComponent<Character.IGetCharacterCoreData>().GetUniqueData().moveType == Character.UniqueSkillMoveType.FIXED) && commandFlow.uniqueFlg;
                if (isFixed) UniqueSkillScriptableObject = characterObject.GetComponent<Character.IGetCharacterCoreData>().GetUniqueData();
                cameraController.AddCarsor();
                DisplaySelectableGrid();
                hologramController.DeleteHologram();
            }

            public void CommandUpdate()
            {
                commandInputFunctionTable[commandInput.GetInputNumber()]();
                if (!Input.GetKeyDown(KeyCode.Return)) return;
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL);
            }

            private void CommandNotInput()
            {
                int selectNumber = commandInput.GetSelectNumber();
                selectNumber = Mathf.Clamp(selectNumber, 0, tabTexts.Length);
                commandInfoText.text = commandInfoTextMessage[selectNumber];

                SelectStartGrid();
                if (isFixed)
                {
                    SelectGrid();
                    return;
                }
                SelectGridPath();
                SelectEndGrid();
            }

            private void CommandInput1()
            {
                cameraController.RemoveCarsor();
                commandInput.SetInputNumber(0);
                displayTileMap.ClearSelectableTileMap();
                commandFlow.SetStateNumber(commandFlow.GetBeforeState());
                hologramController.DeleteHologram();
            }

            public void SetCommandTab()
            {
                setCommandTab.SetTab(commandSprite, infoSprite, tabTexts, tabIcons);
            }

            private void DisplaySelectableGrid()
            {
                if (isFixed) SetSelectableGrid();
                else SetSelectableGrid(playerPosition, move);
                displayTileMap.DisplaySelectableTileMap(selectableGridTable);
            }

            private void SetSelectableGrid(FieldIndex playerPosition, int move)
            {
                for(int i = 0; i < selectableGridTable.GetLength(0); i++)
                {
                    for(int j = 0; j < selectableGridTable.GetLength(1); j++)
                    {
                        int distance = Mathf.Abs(i - playerPosition.row) + Mathf.Abs(j - playerPosition.column);

                        FieldIndex pos = new FieldIndex(i, j);                       
                        selectableGridTable[i, j] = (distance <= move) && getFieldInformation.IsPassingGrid(pos);
                    }
                }
            }

            private void SetSelectableGrid()
            {
                for (int i = 0; i < fieldSize.row; i++)
                {
                    for (int j = 0; j < fieldSize.column; j++)
                    {
                        selectableGridTable[i, j] = false;
                    }
                }
                displayTileMap.ClearSelectableTileMap();
                foreach (var selectGridOffset in UniqueSkillScriptableObject.moveFieldIndexOffsetArray)
                {
                    FieldIndex fieldIndex = playerPosition + selectGridOffset;
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

            private void SelectStartGrid()
            {
                if (isDrag) return;
                if (!(input.IsClickDown())) return;
                if (getCursorPosition.GetCursorIndex() != playerPosition) return;
                isDrag = true;
                for(int i = 0; i < movePositionTable.Length; i++)
                {
                    movePositionTable[i] = FieldIndex.zero;
                }
                movePositionTable[0] = playerPosition;
                previousPosition = playerPosition;
                hologramController.DeleteHologram();
                hologramController.DisplayHologram(playerPosition, FieldIndexOffset.left);
                selectGlid = playerPosition;
                SetOffsetTable();
            }

            private void SelectGridPath()
            {
                if (!isDrag) return;
                FieldIndex cursorIndex = getCursorPosition.GetCursorIndex();
                if (cursorIndex.row < 0) return;
                if (cursorIndex.column < 0) return;
                if (cursorIndex.row > fieldSize.row - 1) return;
                if (cursorIndex.column > fieldSize.column - 1) return;
                if (!selectableGridTable[cursorIndex.row, cursorIndex.column]) return;
                for(int i = 0; i < Mathf.Min(move + 1,movePositionTable.Length); i++)
                {
                    if (cursorIndex == movePositionTable[i]) return;
                    if (movePositionTable[i] != FieldIndex.zero) continue;
                    int distace = Mathf.Abs(cursorIndex.row - previousPosition.row)
                        + Mathf.Abs(cursorIndex.column - previousPosition.column);
                    if (distace > 1) continue;
                    movePositionTable[i] = cursorIndex;
                    skillGrid.selectIndex = movePositionTable[i];
                    SetOffsetTable();
                    previousPosition = movePositionTable[i];
                    FieldIndexOffset direction = new FieldIndexOffset(movePositionTable[i].row - movePositionTable[i - 1].row,
                        movePositionTable[i].column - movePositionTable[i - 1].column);
                    hologramController.MoveHologram(movePositionTable[i], direction);
                    selectGlid = movePositionTable[i];
                    return;
                }
            }

            private void SelectEndGrid()
            {
                if (!isDrag) return;
                if (!(input.IsClickUp())) return;
                skillGrid.selectIndex = selectGlid;
                isDrag = false;
                cameraController.RemoveCarsor();
                commandInput.SetInputNumber(0);
                displayTileMap.ClearSelectableTileMap();
                commandManager.SetMoveSignal(new Manager.MoveSignal(moveOffsetTable));
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_MOVE_GRID);
                if (commandFlow.uniqueFlg)
                {
                    commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
                }
                else
                {
                    commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL);
                }
            }

            private void SelectGrid()
            {
                if (!(input.IsClickDown())) return;
                FieldIndex cursorIndex = getCursorPosition.GetCursorIndex();
                if (!CheckInside(cursorIndex)) return;
                if (!selectableGridTable[cursorIndex.row, cursorIndex.column]) return;
                hologramController.DeleteHologram();
                hologramController.DisplayHologram(cursorIndex, FieldIndexOffset.left);
                skillGrid.selectIndex = cursorIndex;               
                FieldIndexOffset[] moveOffsetTableFixed = FixedRouteInterpolation.Make(playerPosition, cursorIndex - playerPosition);
                commandManager.SetMoveSignal(new Manager.MoveSignal(moveOffsetTableFixed));
                commandInput.SetInputNumber(0);
                commandInput.SetSelectNumber(0);
                displayTileMap.ClearSelectableTileMap();
                cameraController.RemoveCarsor();
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_MOVE_GRID);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
            }

            private void SetOffsetTable()
            {
                for(int i = 0; i < moveOffsetTable.Length; i++)
                {
                    moveOffsetTable[i] = FieldIndexOffset.zero;
                }
                for(int i = 0; i < moveOffsetTable.Length; i++)
                {
                    if (movePositionTable[i + 1] == FieldIndex.zero) return;
                    moveOffsetTable[i] = new FieldIndexOffset(movePositionTable[i + 1].row - movePositionTable[i].row,
                        movePositionTable[i + 1].column - movePositionTable[i].column);
                    Debug.Log(moveOffsetTable[i].rowOffset + ":" + moveOffsetTable[i].columnOffset);
                }
            }
        }
    }
}

