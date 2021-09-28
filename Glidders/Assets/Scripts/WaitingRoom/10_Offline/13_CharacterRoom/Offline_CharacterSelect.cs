using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] Text textDispCharctor;

        [SerializeField] GameObject characterDisp;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private delegate void CharacterTouchFunction();
        private CharacterTouchFunction[] characterTouchFunctionTable;

        public static int setCharacter = 0;

        private CharacterBoolManager characterBoolManager;
        private Offline_SingletonData offline_SingletonData;

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
            offline_SingletonData = GameObject.Find("Offline_SingletonData").GetComponent<Offline_SingletonData>();

            characterDisp.SetActive(false);
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
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_KAITO;
            characterBoolManager.isSelectKaito = false;
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput2()
        {
            commandInput.SetInputNumber(0);
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_SEIRA;
            characterBoolManager.isSelectSeira = false;
            SetCharacterID();
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput3()
        {
            commandInput.SetInputNumber(0);
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_YU;
            characterBoolManager.isSelectYu = false;
            SetCharacterID();
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput4()
        {
            commandInput.SetInputNumber(0);
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_MITSUHA;
            characterBoolManager.isSelectMitsuha = false;
            SetCharacterID();
            CharctorAnnouncement();
            CPUSelect();
        }

        private void CommandInput5() //ステージ選択に戻る
        {
            commandInput.SetInputNumber(0);

            stagePanel.SetActive(true);
            charctorPanel.SetActive(false);
        }

        public void CharctorAnnouncement() //選手発表
        {
            Debug.Log(setCharacter);
            textDispCharctor.text = "キャラクター名 \n" + characterName[setCharacter - 1];
        }

        private void CharacterNotTouch()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCharacter.SELECT_NOT_CHARACTER, (int)SelectCharacter.CHARACTER_NUMBER);
        }

        private void CharacterTouch1()
        {
            commandInput.SetInputNumber(0);

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[0];
        }

        private void CharacterTouch2()
        {
            commandInput.SetInputNumber(0);

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[1];
        }

        private void CharacterTouch3()
        {
            commandInput.SetInputNumber(0);

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[2];
        }

        private void CharacterTouch4()
        {
            commandInput.SetInputNumber(0);

            characterDisp.SetActive(true);
            characterImage.sprite = characterSprites[3];
        }

        private void CharacterTouchRandam()
        {
            commandInput.SetInputNumber(0);
        }

        private void SetCharacterID()
        {
            setCharacter -= 1; 
            offline_SingletonData.SetOfflinePlayerData(setCharacter);
        }

        private void CPUSelect()
        {
            cpuPanel.SetActive(true);
            charctorPanel.SetActive(false);
        }
    }

}
