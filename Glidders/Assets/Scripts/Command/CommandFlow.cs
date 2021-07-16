using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Command
    {
        public class CommandFlow : MonoBehaviour
        {
            [SerializeField] private GameObject testCharacterObject;
            private GameObject characterObject;

            [SerializeField] private GameObject[] CommandStateObject;
            private int commandStateNumber = 0;

            private delegate void CommandFunction();
            private CommandFunction[] commandFunctionsTable;

            [SerializeField] private GameObject commandUI;

            private bool commandFlag = false;

            private int playerID = 0;

            private FieldIndex characterPosition = default;

            public enum CommandState
            {
                SELECT_ACTION_OR_UNIQUE,
                SELECT_MOVE_GRID,
                SELECT_SKILL,
                SELECT_SKILL_GRID,
                SELECT_DIRECTION,
                SELECT_CONFILM,

                COMMAND_NUM
            }


            // Start is called before the first frame update
            void Start()
            {
                commandFunctionsTable = new CommandFunction[(int)CommandState.COMMAND_NUM];
                commandFunctionsTable[(int)CommandState.SELECT_ACTION_OR_UNIQUE] = SelectActionOrUnique;
                commandFunctionsTable[(int)CommandState.SELECT_MOVE_GRID] = SelectMoveGrid;
                commandFunctionsTable[(int)CommandState.SELECT_SKILL] = SelectSkill;
                commandFunctionsTable[(int)CommandState.SELECT_SKILL_GRID] = SelectSkillGrid;
                commandFunctionsTable[(int)CommandState.SELECT_DIRECTION] = SelectDirecton;
                commandFunctionsTable[(int)CommandState.SELECT_CONFILM] = SelectConfilm;
                commandUI.SetActive(false);
            }

            // Update is called once per frame
            void Update()
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) StartCommandPhase(0, testCharacterObject, new FieldIndex(3, 2));
                if (!commandFlag) return;
                commandFunctionsTable[commandStateNumber]();
            }

            private void SelectActionOrUnique()
            {
                CommandStateObject[(int)CommandState.SELECT_ACTION_OR_UNIQUE].GetComponent<ICommand>().CommandUpdate();
            }

            private void SelectMoveGrid()
            {
                CommandStateObject[(int)CommandState.SELECT_MOVE_GRID].GetComponent<ICommand>().CommandUpdate();
            }

            private void SelectSkill()
            {
                CommandStateObject[(int)CommandState.SELECT_SKILL].GetComponent<ICommand>().CommandUpdate();
            }

            private void SelectSkillGrid()
            {
                CommandStateObject[(int)CommandState.SELECT_SKILL_GRID].GetComponent<ICommand>().CommandUpdate();
            }

            private void SelectDirecton()
            {
                CommandStateObject[(int)CommandState.SELECT_DIRECTION].GetComponent<ICommand>().CommandUpdate();
            }

            private void SelectConfilm()
            {
                CommandStateObject[(int)CommandState.SELECT_CONFILM].GetComponent<ICommand>().CommandUpdate();
            }

            public void SetStateNumber(int setNumber)
            {
                commandStateNumber = setNumber;
                CommandStateObject[commandStateNumber].GetComponent<ICommand>().SetCharacterObject(characterObject);
                CommandStateObject[commandStateNumber].GetComponent<ICommand>().CommandStart();

                //CommandStateObject[setNumber].GetComponent<ICommand>().SetCommandTab();
            }

            public void StartCommandPhase(int id, GameObject charaObject,FieldIndex position)
            {
                playerID = id;
                characterObject = charaObject;
                characterPosition = position;
                commandUI.SetActive(true);
                SetStateNumber((int)CommandState.SELECT_ACTION_OR_UNIQUE);
                commandFlag = true;
            }

            public FieldIndex GetCharacterPosition()
            {
                return characterPosition;
            }
        }
    }
}
