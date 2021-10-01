using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glidders
{
    public class EnemyCharacterSelect : MonoBehaviour
    {
        [SerializeField] string[] characterName = { "カイト", "セイラ", "ミツハ" };
        [SerializeField] private CommandInput commandInput;
        private Sprite sprite;

        [SerializeField] GameObject charctorPanel;
        [SerializeField] GameObject enemyCharacterPenel;
        [SerializeField] GameObject finalConf;
        [SerializeField] Image characterImage;

        [SerializeField] Sprite[] characterSprites;
        [SerializeField] Image characterIconImage;
        [SerializeField] Sprite[] characterIconSprites;
        [SerializeField] Image characterColorImage;
        [SerializeField] Sprite[] characterColorSprites;

        [SerializeField] GameObject characterDisp;

        private delegate void CommandInputFunction();
        private CommandInputFunction[] commandInputFunctionTable;

        private delegate void CharacterTouchFunction();
        private CharacterTouchFunction[] characterTouchFunctionTable;

        private CharacterBoolManager characterBoolManager;

        private SpriteRenderer spriteRenderer;
        private SingletonData singletonData;

        public static int setCharacterID = 0;
        int setPlayerNum = 1;
        string setCharacterName;

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
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            commandInputFunctionTable[commandInput.GetInputNumber()]();
            characterTouchFunctionTable[commandInput.GetSelectNumber()]();
        }

        private void CommandNotInput()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_5);
        }

        private void CommandInput1()
        {
            commandInput.SetInputNumber(0);
            if (characterBoolManager.isSelectKaito)
            {
                setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_KAITO -1;
                characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_KAITO - 1];
                characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_KAITO - 1];
                SetCpuID();
                CharctorAnnouncement();
                FinalConf();
            }
        }

        private void CommandInput2()
        {
            commandInput.SetInputNumber(0);
            if (characterBoolManager.isSelectSeira)
            {
                setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_SEIRA -1;
                characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_SEIRA - 1];
                characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_SEIRA - 1];
                SetCpuID();
                CharctorAnnouncement();
                FinalConf();
            }
        }

        private void CommandInput3()
        {
            commandInput.SetInputNumber(0);
            if (characterBoolManager.isSelectYu)
            {
                setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_YU -1;
                characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_YU - 1];
                characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_YU - 1];
                SetCpuID();
                CharctorAnnouncement();
                FinalConf();
            }
        }

        private void CommandInput4()
        {
            commandInput.SetInputNumber(0);
            if (characterBoolManager.isSelectMitsuha)
            {
                setCharacterID = (int)SelectCharacter.SELECT_CHARACTER_MITSUHA -1;
                characterIconImage.sprite = characterIconSprites[(int)SelectCharacter.SELECT_CHARACTER_MITSUHA - 1];
                characterColorImage.sprite = characterColorSprites[(int)SelectCharacter.SELECT_CHARACTER_MITSUHA - 1];
                SetCpuID();
                CharctorAnnouncement();
                FinalConf();
            }
        }

        private void CommandInput5()
        {
            commandInput.SetInputNumber(0);

            characterBoolManager.isSelectKaito = true;
            characterBoolManager.isSelectSeira = true;
            characterBoolManager.isSelectYu = true;
            characterBoolManager.isSelectMitsuha = true;

            charctorPanel.SetActive(true);
            enemyCharacterPenel.SetActive(false);
        }

        private void CharacterNotTouch()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCharacter.SELECT_NOT_CHARACTER, (int)SelectCommand.COMMAND_INPUT_5);
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

        private void SetCpuID()
        {
            setCharacterName = characterName[setCharacterID];
            singletonData.OfflineGetCpuData(setPlayerNum, setCharacterName, setCharacterID);
        }

        public void CharctorAnnouncement() //選手発表
        {
            //textDispCharctor.text = "キャラクター名 \n" + characterName[setCharacter];
        }

        public void FinalConf()
        {
            finalConf.SetActive(true);
            enemyCharacterPenel.SetActive(false);
        }
    }

}
