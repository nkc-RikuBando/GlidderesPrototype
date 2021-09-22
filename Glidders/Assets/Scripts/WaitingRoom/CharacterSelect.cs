using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Glidders
{
    public class CharacterSelect : MonoBehaviourPunCallbacks
    {
        [SerializeField] string[] characterName = { "カイト", "セイラ", "ミツハ" };
        [SerializeField] private CommandInput commandInput;
        private Sprite sprite;
        PhotonView view;

        [SerializeField] GameObject rulePanel;
        [SerializeField] GameObject stagePanel;
        [SerializeField] GameObject charctorPanel;
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

        private bool isPlayerWait = false;

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

            CHARACTER_NUMBER
        }

        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();

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

            characterDisp.SetActive(false);
            finalConf.SetActive(false);
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
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCommand.COMMAND_NOT_INPUT, (int)SelectCommand.COMMAND_INPUT_4);
        }

        private void CommandInput1()
        {
            commandInput.SetInputNumber(0);
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_KAITO;
            CharctorAnnouncement();
            FinalConf();
        }

        private void CommandInput2()
        {
            commandInput.SetInputNumber(0);
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_SEIRA;
            CharctorAnnouncement();
            FinalConf();
        }

        private void CommandInput3()
        {
            commandInput.SetInputNumber(0);
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_YU;
            CharctorAnnouncement();
            FinalConf();
        }

        private void CommandInput4()
        {
            commandInput.SetInputNumber(0);
            setCharacter = (int)SelectCharacter.SELECT_CHARACTER_MITSUHA;
            CharctorAnnouncement();
            FinalConf();
        }

        private void CommandInput5() //ステージ選択に戻る
        {
            commandInput.SetInputNumber(0);

            stagePanel.SetActive(true);
            charctorPanel.SetActive(false);
        }

        public void CharctorAnnouncement() //選手発表
        {
            textDispCharctor.text = "キャラクター名 \n" + characterName[setCharacter -1];
        }

        private void CharacterNotTouch()
        {
            int selectNumber = commandInput.GetSelectNumber();
            selectNumber = Mathf.Clamp(selectNumber, (int)SelectCharacter.SELECT_NOT_CHARACTER, (int)SelectCharacter.SELECT_CHARACTER_MITSUHA);
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

        public void FinalConf()
        {
            finalConf.SetActive(true);
            charctorPanel.SetActive(false);
        }
    }
}

