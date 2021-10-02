using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Character;

namespace Glidders
{
    public class Offline_CharacterSelect : MonoBehaviour
    {
        [SerializeField] string[] characterName = { "カイト", "セイラ","ユウ", "ミツハ" };
        [SerializeField] private CommandInput commandInput;
        private Sprite sprite;

        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;
        [SerializeField] GameObject cpuPanel;
        [SerializeField] GameObject finalConf;

        [SerializeField] Image characterImage;
        [SerializeField] Sprite[] characterSprites;
        [SerializeField] Image characterIconImage;
        [SerializeField] Sprite[] characterIconSprites;
        [SerializeField] Image characterColorImage;
        [SerializeField] Sprite[] characterColorSprites;
        [SerializeField] Text textDispCharctor;

        [SerializeField] Text[] SkillName;
        [SerializeField] Image[] SkillIcon;

        [SerializeField] GameObject characterDisp;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private delegate void CharacterTouchFunction();
        private CharacterTouchFunction[] characterTouchFunctionTable;

        int setCharacterID = 0;
        int setPlayerNum = 0;
        string setCharacterName;

        private CharacterBoolManager characterBoolManager;
        private SingletonData singletonData;


        UniqueSkillScriptableObject[] kaitoUniqueSkillScriptableObjectArray;
        UniqueSkillScriptableObject[] seiraUniqueSkillScriptableObjectArray;
        UniqueSkillScriptableObject[] yuUniqueSkillScriptableObjectArray;
        UniqueSkillScriptableObject[] mitsuhaUniqueSkillScriptableObjectArray;

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

        private enum SelectCharacter
        {
            SELECT_NOT_CHARACTER,
            SELECT_CHARACTER_KAITO,
            SELECT_CHARACTER_SEIRA,
            SELECT_CHARACTER_YU,
            SELECT_CHARACTER_MITSUHA,
            SELECT_CHARACTER_RANDAM,

            CHARACTER_NUMBER
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

            characterTouchFunctionTable = new CharacterTouchFunction[(int)SelectCharacter.CHARACTER_NUMBER];
            characterTouchFunctionTable[(int)SelectCharacter.SELECT_NOT_CHARACTER] = CharacterNotTouch;
            characterTouchFunctionTable[(int)SelectCharacter.SELECT_CHARACTER_KAITO] = CharacterTouch1;
            characterTouchFunctionTable[(int)SelectCharacter.SELECT_CHARACTER_SEIRA] = CharacterTouch2;
            characterTouchFunctionTable[(int)SelectCharacter.SELECT_CHARACTER_YU] = CharacterTouch3;
            characterTouchFunctionTable[(int)SelectCharacter.SELECT_CHARACTER_MITSUHA] = CharacterTouch4;
            characterTouchFunctionTable[(int)SelectCharacter.SELECT_CHARACTER_RANDAM] = CharacterTouchRandam;

            characterBoolManager = GameObject.Find("CharacterSelectManager").GetComponent<CharacterBoolManager>();
            singletonData = GameObject.Find("MatchDataSingleton").GetComponent<SingletonData>();

            characterDisp.SetActive(false);

            kaitoUniqueSkillScriptableObjectArray = new UniqueSkillScriptableObject[Rule.skillCount + Rule.uniqueSkillCount];
            kaitoUniqueSkillScriptableObjectArray[0] = ScriptableObjectDatabase.GetSkill("S0101");
            kaitoUniqueSkillScriptableObjectArray[1] = ScriptableObjectDatabase.GetSkill("S0102");
            kaitoUniqueSkillScriptableObjectArray[2] = ScriptableObjectDatabase.GetSkill("S0103");
            kaitoUniqueSkillScriptableObjectArray[3] = ScriptableObjectDatabase.GetSkill("US01");

            seiraUniqueSkillScriptableObjectArray = new UniqueSkillScriptableObject[Rule.skillCount + Rule.uniqueSkillCount];
            seiraUniqueSkillScriptableObjectArray[0] = ScriptableObjectDatabase.GetSkill("S0201");
            seiraUniqueSkillScriptableObjectArray[1] = ScriptableObjectDatabase.GetSkill("S0202");
            seiraUniqueSkillScriptableObjectArray[2] = ScriptableObjectDatabase.GetSkill("S0203");
            seiraUniqueSkillScriptableObjectArray[3] = ScriptableObjectDatabase.GetSkill("US02");

            yuUniqueSkillScriptableObjectArray = new UniqueSkillScriptableObject[Rule.skillCount + Rule.uniqueSkillCount];
            yuUniqueSkillScriptableObjectArray[0] = ScriptableObjectDatabase.GetSkill("S0301a");
            yuUniqueSkillScriptableObjectArray[1] = ScriptableObjectDatabase.GetSkill("S0302a");
            yuUniqueSkillScriptableObjectArray[2] = ScriptableObjectDatabase.GetSkill("S0303a");
            yuUniqueSkillScriptableObjectArray[3] = ScriptableObjectDatabase.GetSkill("US03a");

            mitsuhaUniqueSkillScriptableObjectArray = new UniqueSkillScriptableObject[Rule.skillCount + Rule.uniqueSkillCount];
            mitsuhaUniqueSkillScriptableObjectArray[0] = ScriptableObjectDatabase.GetSkill("S0401a");
            mitsuhaUniqueSkillScriptableObjectArray[1] = ScriptableObjectDatabase.GetSkill("S0402a");
            mitsuhaUniqueSkillScriptableObjectArray[2] = ScriptableObjectDatabase.GetSkill("S0403a");
            mitsuhaUniqueSkillScriptableObjectArray[3] = ScriptableObjectDatabase.GetSkill("US04a");

            //kaitoUniqueSkillScriptableObjectArray[0].skillIcon;
            //Debug.Log("isnul = " + (uniqueSkillScriptableObject.skillName));
        }

        // Update is called once per frame
        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
            characterTouchFunctionTable[commandInput.GetSelectNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetInputNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_5);
        }

        private void CommandInput1()
        {
            commandInput.SetInputNumber(0);
            setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_KAITO -1;
            characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_KAITO - 1];
            characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_KAITO - 1];
            characterBoolManager.isSelectKaito = false;
            SetCharacterID();
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput2()
        {
            commandInput.SetInputNumber(0);
            setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_SEIRA -1;
            characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_SEIRA -1];
            characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_SEIRA - 1];
            characterBoolManager.isSelectSeira = false;
            SetCharacterID();
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput3()
        {
            commandInput.SetInputNumber(0);
            setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_YU -1;
            characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_YU - 1];
            characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_YU - 1];
            characterBoolManager.isSelectYu = false;
            SetCharacterID();
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput4()
        {
            commandInput.SetInputNumber(0);
            setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_MITSUHA -1;
            characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_MITSUHA - 1];
            characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_MITSUHA - 1];
            characterBoolManager.isSelectMitsuha = false;
            SetCharacterID();
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput5() //ステージ選択に戻る
        {
            commandInput.SetInputNumber(0);

            //characterIconImage.sprite = null;
            //characterColorImage.sprite = null;
            stagePanel.SetActive(true);
            charctorPanel.SetActive(false);
        }

        public void CharctorAnnouncement() //選手発表
        {
            Debug.Log(setCharacterID);
            textDispCharctor.text = "キャラクター名 \n" + characterName[setCharacterID];
        }

        private void CharacterNotTouch()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCharacter.SELECT_NOT_CHARACTER, (int)SelectCharacter.CHARACTER_NUMBER);
        }

        private void CharacterTouch1()
        {
            commandInput.SetInputNumber(0);

            for (int i = 0; i < kaitoUniqueSkillScriptableObjectArray.Length; i++)
            {
                Debug.Log("i " + i);
                SkillIcon[i].sprite = kaitoUniqueSkillScriptableObjectArray[i].skillIcon;
                SkillName[i].text = kaitoUniqueSkillScriptableObjectArray[i].skillName;
            }

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[0];


        }

        private void CharacterTouch2()
        {
            commandInput.SetInputNumber(0);

            for (int i = 0; i < seiraUniqueSkillScriptableObjectArray.Length; i++)
            {
                SkillIcon[i].sprite = seiraUniqueSkillScriptableObjectArray[i].skillIcon;
                SkillName[i].text = seiraUniqueSkillScriptableObjectArray[i].skillName;
            }

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[1];
        }

        private void CharacterTouch3()
        {
            commandInput.SetInputNumber(0);

            for (int i = 0; i < yuUniqueSkillScriptableObjectArray.Length; i++)
            {
                SkillIcon[i].sprite = yuUniqueSkillScriptableObjectArray[i].skillIcon;
                SkillName[i].text = yuUniqueSkillScriptableObjectArray[i].skillName;
            }

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[2];
        }

        private void CharacterTouch4()
        {
            commandInput.SetInputNumber(0);

            for (int i = 0; i < mitsuhaUniqueSkillScriptableObjectArray.Length; i++)
            {
                SkillIcon[i].sprite = mitsuhaUniqueSkillScriptableObjectArray[i].skillIcon;
                SkillName[i].text = mitsuhaUniqueSkillScriptableObjectArray[i].skillName;
            }

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[3];
        }

        private void CharacterTouchRandam()
        {
            commandInput.SetInputNumber(0);
        }

        private void SetCharacterID()
        {
            Debug.Log(setCharacterID);
            setCharacterName = characterName[setCharacterID];
            singletonData.OfflineGetPlayerData(setPlayerNum, setCharacterName,setCharacterID);
        }

        private void CPUSelect()
        {
            cpuPanel.SetActive(true);
            charctorPanel.SetActive(false);
        }
    }

}
