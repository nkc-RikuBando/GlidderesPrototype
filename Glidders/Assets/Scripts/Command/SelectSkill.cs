using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    namespace Command
    {
        public class SelectSkill : MonoBehaviour, ICommand
        {
            private GameObject characterObject;

            [SerializeField] private CommandInput commandInput;
            [SerializeField] private CommandFlow commandFlow;

            [SerializeField] private Sprite infoSprite;
            [SerializeField] private Sprite commandSprite;
            private string[] tabTexts;
            const string WAIT_TEXT = "待機";
            const string BACK_TEXT = "戻る";
            const string WAIT_INFO = "スキルを使わず、待機する";
            const string BACK_INFO = "前に戻る";

            private int beforeSelectNumber;

            private Sprite[] tabIcons;
            [SerializeField] private Sprite waitIcon = default;
            [SerializeField] private Sprite backIcon = default;

            [SerializeField] private SetCommandTab setCommandTab;
            [SerializeField] private Text commandInfoText;
            [SerializeField] private GameObject skillInfoObject;
            [SerializeField] private Image moveTypeImage;
            [SerializeField] private Sprite[] moveTypeSprite;
            [SerializeField] private Text energyText;
            [SerializeField] private Text damageText;
            [SerializeField] private Text powerText;
            [SerializeField] private string[] commandInfoTextMessage;
            [SerializeField] private SelectSkillGrid selectSkillGrid;


            private delegate void CommandInputFunction();
            private int beforeState = 0;

            private Character.IGetCharacterCoreData getCharacterCoreData;

            private CommandInputFunction[] commandInputFunctionTable;

            [SerializeField] private Graphic.HologramController hologramController;

            [SerializeField] private CommandManager commandManager;

            [SerializeField] private Character.UniqueSkillScriptableObject noneSkill = default;

            private enum SelectCommand
            {
                COMMAND_NOT_INPUT,
                COMMAND_INPUT_1,
                COMMAND_INPUT_2,
                COMMAND_INPUT_3,
                COMMAND_INPUT_4,
                COMMAND_INPUT_5,

                COMMAND_NUMBER
            }

            // Start is called before the first frame update
            void Start()
            {
                commandInputFunctionTable = new CommandInputFunction[(int)SelectCommand.COMMAND_NUMBER];
                commandInputFunctionTable[(int)SelectCommand.COMMAND_NOT_INPUT] = CommandNotInput;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_1] = CommandInput1;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_2] = CommandInput2;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_3] = CommandInput3;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_4] = CommandInput4;
                commandInputFunctionTable[(int)SelectCommand.COMMAND_INPUT_5] = CommandInput5;
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
                skillInfoObject.SetActive(true);
                SetCommandTab();
                if(commandFlow.uniqueFlg) hologramController.DeleteHologram();
            }


            public void CommandUpdate()
            {
                commandInputFunctionTable[commandInput.GetInputNumber()]();
            }

            public void SetCommandTab()
            {
                getCharacterCoreData = characterObject.GetComponent<Character.IGetCharacterCoreData>();
                if (commandFlow.uniqueFlg)
                {
                    tabTexts = new string[] {
                        getCharacterCoreData.GetUniqueData().skillName,
                        BACK_TEXT
                    };
                    tabIcons = new Sprite[] {
                        getCharacterCoreData.GetUniqueData().skillIcon,
                        backIcon
                    };
                    commandInfoTextMessage = new string[]
                    {
                        "",
                        getCharacterCoreData.GetUniqueData().skillCaption,
                        BACK_INFO
                    };
                }
                else
                {
                    tabTexts = new string[] {
                        getCharacterCoreData.GetSkillData(1).skillName,
                        getCharacterCoreData.GetSkillData(2).skillName,
                        getCharacterCoreData.GetSkillData(3).skillName,
                        WAIT_TEXT,
                        BACK_TEXT
                    };
                    tabIcons = new Sprite[] {
                        getCharacterCoreData.GetSkillData(1).skillIcon,
                        getCharacterCoreData.GetSkillData(2).skillIcon,
                        getCharacterCoreData.GetSkillData(3).skillIcon,
                        waitIcon,
                        backIcon
                    };
                    commandInfoTextMessage = new string[]
                    {
                        "",
                        getCharacterCoreData.GetSkillData(1).skillCaption,
                        getCharacterCoreData.GetSkillData(2).skillCaption,
                        getCharacterCoreData.GetSkillData(3).skillCaption,
                        WAIT_INFO,
                        BACK_INFO
                    };
                }

                setCommandTab.SetTab(commandSprite, infoSprite, tabTexts, tabIcons);
            }

            private void CommandNotInput()
            {
                int selectNumber = commandInput.GetSelectNumber();
                selectNumber = Mathf.Clamp(selectNumber, 0, tabTexts.Length);
                if (beforeSelectNumber == selectNumber) return;
                beforeSelectNumber = selectNumber;
                commandInfoText.text = commandInfoTextMessage[selectNumber];
                if (commandFlow.uniqueFlg)
                {
                    if (selectNumber > 0 && selectNumber < 2)
                    {
                        skillInfoObject.SetActive(true);
                        moveTypeImage.sprite = moveTypeSprite[(int)getCharacterCoreData.GetUniqueData().moveType];
                        energyText.text = getCharacterCoreData.GetUniqueData().energy.ToString();
                        if (getCharacterCoreData.GetUniqueData().energy <= commandFlow.plEnergy) energyText.color = Color.white;
                        else energyText.color = new Color(1, 0.2f, 0.6f);
                        damageText.text = getCharacterCoreData.GetUniqueData().damage > 0 ? 
                            getCharacterCoreData.GetUniqueData().damage.ToString() : "-";
                        powerText.text = getCharacterCoreData.GetUniqueData().power > 0 ?
                            getCharacterCoreData.GetUniqueData().power.ToString() : "-";
                    }
                    else skillInfoObject.SetActive(false);
                }
                else
                {
                    if (selectNumber > 0 && selectNumber < 4)
                    {
                        skillInfoObject.SetActive(true);
                        moveTypeImage.sprite = moveTypeSprite[(int)getCharacterCoreData.GetSkillData(selectNumber).moveType];
                        energyText.text = getCharacterCoreData.GetSkillData(selectNumber).energy.ToString();
                        if (getCharacterCoreData.GetSkillData(selectNumber).energy <= commandFlow.plEnergy) energyText.color = Color.white; 
                        else energyText.color = new Color(1, 0.2f, 0.6f);
                        damageText.text = getCharacterCoreData.GetSkillData(selectNumber).damage > 0 ?
                            getCharacterCoreData.GetSkillData(selectNumber).damage.ToString() : "-";
                        powerText.text = getCharacterCoreData.GetSkillData(selectNumber).power > 0 ?
                            getCharacterCoreData.GetSkillData(selectNumber).power.ToString() : "-";
                    }
                    else skillInfoObject.SetActive(false);
                }
            }

            private void CommandInput1()
            {
                skillInfoObject.SetActive(false);

                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_SKILL);
                if (commandFlow.uniqueFlg)
                {
                    if (getCharacterCoreData.GetUniqueData().moveType == Character.UniqueSkillMoveType.NONE)
                    {
                        hologramController.DisplayHologram(commandFlow.GetCharacterPosition(), FieldIndexOffset.left);
                        commandInput.SetInputNumber(0);
                        commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
                    }
                    else
                    {
                        commandInput.SetInputNumber(0);
                        commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_MOVE_GRID);
                    }
                }
                else
                {
                    commandInput.SetInputNumber(0);
                    selectSkillGrid.SetSkillNumber(1);
                    commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
                }
            }

            private void CommandInput2()
            {
                skillInfoObject.SetActive(false);

                if (commandFlow.uniqueFlg)
                {
                    commandInput.SetInputNumber(0);
                    commandFlow.SetStateNumber(beforeState);
                }
                else
                {
                    commandInput.SetInputNumber(0);
                    selectSkillGrid.SetSkillNumber(2);
                    commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_SKILL);
                    commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
                }
            }

            private void CommandInput3()
            {
                skillInfoObject.SetActive(false);

                commandInput.SetInputNumber(0);
                selectSkillGrid.SetSkillNumber(3);
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_SKILL);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_SKILL_GRID);
            }

            private void CommandInput4()
            {
                skillInfoObject.SetActive(false);

                commandManager.SetAttackSignal(new Manager.AttackSignal(false, noneSkill, new FieldIndex(), new FieldIndex(), 0));
                commandInput.SetInputNumber(0);
                commandFlow.SetBeforeState((int)CommandFlow.CommandState.SELECT_SKILL);
                commandFlow.SetStateNumber((int)CommandFlow.CommandState.SELECT_DIRECTION);
            }

            private void CommandInput5()
            {
                skillInfoObject.SetActive(false);

                commandInput.SetInputNumber(0);
                commandFlow.SetStateNumber(commandFlow.GetBeforeState());
            }
        }
    }
}

