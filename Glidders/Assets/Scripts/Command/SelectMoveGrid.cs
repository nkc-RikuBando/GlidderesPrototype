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
            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite commandSprite;
            [SerializeField] private string[] tabTexts;
            [SerializeField] private Sprite[] tabIcons;
            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private string[] commandInfoTextMessage;

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

            private bool[,] selectableGridTable = new bool[9, 9];

            private bool startFlag = true;

            private int move = 2;
            private FieldIndex playerPosition = new FieldIndex(3, 2);

            private bool isDrag = false;
            private FieldIndex[] movePositionTable = new FieldIndex[6];
            private FieldIndexOffset[] moveOffsetTable = new FieldIndexOffset[5];

            private FieldIndex previousPosition;

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
                input = inputObject.GetComponent<Inputer.IInput>();
                getCursorPosition = cursorObject.GetComponent<IGetCursorPosition>();
                getFieldInformation = iGetFieldInformationObject.GetComponent<Field.IGetFieldInformation>();
                commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
                commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void CommandStart()
            {
                if (!startFlag) return;
                Debug.Log("‚ ‚ ‚ ‚ ");
                startFlag = false;
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
                CommandStart();

                int selectNumber = commandInput.GetSelectNumber();
                selectNumber = Mathf.Clamp(selectNumber, 0, tabTexts.Length);
                commandInfoText.text = commandInfoTextMessage[selectNumber];

                SelectStartGrid();
                SelectGridPath();
                SelectEndGrid();
            }

            private void CommandInput1()
            {
                startFlag = true;
                cameraController.RemoveCarsor();
                commandInput.SetInputNumber(0);
                displayTileMap.ClearSelectableTileMap();
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_ACTION_OR_UNIQUE);
                hologramController.DeleteHologram();
            }

            public void SetCommandTab()
            {
                setCommandTab.SetTab(commandSprite, tabTexts, tabIcons);
            }

            private void DisplaySelectableGrid()
            {
                SetSelectableGrid(playerPosition, move);
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
            }

            private void SelectGridPath()
            {
                if (!isDrag) return;
                for(int i = 0; i < Mathf.Min(move + 1,movePositionTable.Length); i++)
                {
                    if (getCursorPosition.GetCursorIndex() == movePositionTable[i]) return;
                    if (movePositionTable[i] != FieldIndex.zero) continue;
                    int distace = Mathf.Abs(getCursorPosition.GetCursorIndex().row - previousPosition.row)
                        + Mathf.Abs(getCursorPosition.GetCursorIndex().column - previousPosition.column);
                    if (distace > 1) continue;
                    movePositionTable[i] = getCursorPosition.GetCursorIndex();
                    SetOffsetTable();
                    previousPosition = movePositionTable[i];
                    FieldIndexOffset direction = new FieldIndexOffset(movePositionTable[i].row - movePositionTable[i - 1].row,
                        movePositionTable[i].column - movePositionTable[i - 1].column);
                    hologramController.MoveHologram(movePositionTable[i], direction);
                    return;
                }
            }

            private void SelectEndGrid()
            {
                if (!isDrag) return;
                if (!(input.IsClickUp())) return;
                isDrag = false;
                startFlag = true;
                cameraController.RemoveCarsor();
                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL);
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

