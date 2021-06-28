using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    public class CommandFlow : MonoBehaviour
    {
        [SerializeField] private GameObject[] CommandStateObject;
        private int commandStateNumber = 0;

        private delegate void CommandFunction();
        private CommandFunction[] commandFunctionsTable;

        enum CommandState
        {
            SELECT_ACTION_OR_UNIQUE,
            SELECT_MOVE_GLID,
            SELECT_SKILL,
            SELECT_SKILL_GLID,
            SELECT_DIRECTION,
            SELECT_CONILM,

            COMMAND_NUM
        }

        // Start is called before the first frame update
        void Start()
        {
            commandFunctionsTable = new CommandFunction[(int)CommandState.COMMAND_NUM];
            commandFunctionsTable[(int)CommandState.SELECT_ACTION_OR_UNIQUE] = SelectActionOrUnique;
            commandFunctionsTable[(int)CommandState.SELECT_MOVE_GLID] = SelectMoveGlid;
            SetStateNumber(commandStateNumber);
        }

        // Update is called once per frame
        void Update()
        {
            commandFunctionsTable[commandStateNumber]();
        }

        private void SelectActionOrUnique()
        {

        }

        private void SelectMoveGlid()
        {

        }

        private void SelectSkill()
        {

        }

        private void SelectSkillGlid()
        {

        }

        private void SelectDirecton()
        {

        }

        private void SelectConfilm()
        {

        }

        public void SetStateNumber(int setNumber)
        {
            commandStateNumber = setNumber;
            CommandStateObject[setNumber].GetComponent<ICommand>().SetCommandTab();
        }
    }
}
