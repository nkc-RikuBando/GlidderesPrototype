using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;

namespace Glidders
{
    namespace Manager
    { 
        /// <summary>
        /// キャラクタ情報を格納するストラクト
        /// </summary>
        public struct CharacterData
        {
            public GameObject thisObject;
            public FieldIndex index;
            public MoveSignal moveSignal;
            public AttackSignal attackSignal;
            public bool canAct{ get; set; }
        }

        public class CoreManager : MonoBehaviour,ICharacterDataReceiver
        {
            const int PLAYER_AMOUNT = 2; // プレイヤーの総数
            const int PLAYER_MOVE_DISTANCE = 5; // 移動の総量

            GameObject[] Characters = new GameObject[PLAYER_AMOUNT]; // いったんCoreManager内に記述
            
            // 各クラス
            CharacterMove characterMove;
            CharacterAttack characterAttack;
            IGetFieldInformation getFieldInformation;
            CharacterDirection[] characterDirections = new CharacterDirection[PLAYER_AMOUNT];

            CharacterData[] characterDataList = new CharacterData[PLAYER_AMOUNT]; // データの総量をプレイヤーの総数の分作る

            public bool moveStart { get; set; } // 移動が可能かどうか
            public bool attackStart { get; set; } // 攻撃が可能かどうか
            public List<MoveSignal> MoveSignalList { get; set; } // MoveSignalのリスト
            public List<AttackSignal> AttackSignalList { get; set; } // AttackSignalのリスト

            #region デバッグ用変数
            FieldIndexOffset[,] moveDistance = new FieldIndexOffset[,] 
            { { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0),},
              { new FieldIndexOffset(1, 0), new FieldIndexOffset( 0, -1), new FieldIndexOffset(0, 1), new FieldIndexOffset(-1, 0), new FieldIndexOffset(0, 0)} };

            Character.SkillScriptableObject[] skillScriptableObjects = new Character.SkillScriptableObject[PLAYER_AMOUNT];
            #endregion
            // Start is called before the first frame update
            void Start()
            {
                #region リストの初期化
                for (int i = 0; i < PLAYER_AMOUNT;i++)
                {
                    characterDataList[i] = new CharacterData();
                }

                #region デバッグ用　Moveリスト内部の初期化 および　Moveリスト内部の整理
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    for (int j = 0;j < PLAYER_MOVE_DISTANCE;j++)
                    {
                        characterDataList[i].moveSignal.moveDataArray[j] = moveDistance[i, j];
                    }
                    MoveDataReceiver(characterDataList[i].moveSignal, i);
                }
                characterDataList[0].thisObject = GameObject.Find("Kaito");
                characterDataList[1].thisObject = GameObject.Find("Seira");

                characterDataList[0].index = new FieldIndex(3, 2);
                characterDataList[1].index = new FieldIndex(5, 2);

                characterDataList[0].canAct = true;
                characterDataList[1].canAct = true;
                #endregion


                #region デバッグ用　Attackリストの初期化
                skillScriptableObjects[0].skillName = "まぎんぎり！";
                skillScriptableObjects[1].skillName = "いらふしょん！！";

                skillScriptableObjects[0].energy = 1;
                skillScriptableObjects[1].energy = 2;

                skillScriptableObjects[0].damage = 200;
                skillScriptableObjects[1].damage = 100;

                skillScriptableObjects[0].priority = 3;
                skillScriptableObjects[1].priority = 2;

                skillScriptableObjects[0].power = 2;
                skillScriptableObjects[1].power = 3;
                for (int i = 0; i < characterDataList.Length;i++)
                {
                    characterDataList[i].attackSignal = new AttackSignal(true, skillScriptableObjects[i], new FieldIndex(3, 3), FieldIndexOffset.down);
                }
                #endregion
                #endregion

                for (int i = 0;i < characterDirections.Length;i++)
                {
                    characterDirections[i] = characterDataList[i].thisObject.GetComponent<CharacterDirection>();
                }

                getFieldInformation = GameObject.Find("FieldCore").GetComponent<FieldCore>(); // インターフェースを取得する
                characterMove = new CharacterMove(getFieldInformation,characterDirections); // CharacterMoveの生成　取得したインターフェースの情報を渡す
                characterAttack = new CharacterAttack();
            }

            // Update is called once per frame
            void Update()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    characterAttack.AttackOrder(characterDataList);
                }

                // デバッグ用　Enterキーで実行フラグをtrueに
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    moveStart = true;
                }

                // 移動実行フラグがtrueのとき、Moveクラスに移動を実行させる
                if (moveStart)
                {
                    StartCoroutine(characterMove.MoveOrder(characterDataList));

                    moveStart = false;
                }
            }

            /// <summary>
            /// 指定されたidの配列番号を持った移動信号に渡された移動信号を格納する
            /// </summary>
            /// <param name="signal">渡す移動信号</param>
            /// <param name="characterID">キャラクタID</param>
            public void MoveDataReceiver(MoveSignal signal,int characterID)
            {
                characterDataList[characterID].moveSignal = signal;
            }

            /// <summary>
            /// 指定されたidの配列番号を持った攻撃信号に渡された攻撃信号を格納する
            /// </summary>
            /// <param name="signal">渡す攻撃信号</param>
            /// <param name="characterID">キャラクタID</param>
            public void AttackDataReceiver(AttackSignal signal,int characterID)
            {
                characterDataList[characterID].attackSignal = signal;
            }

            /// <summary>
            /// 指定された配列番号を持ったFieldIndexに対し、初期位置を渡す
            /// </summary>
            /// <param name="fieldIndex">渡すIndex</param>
            /// <param name="characterID">キャラクタID</param>
            public void StartPositionSeter(FieldIndex fieldIndex,int characterID)
            {
                characterDataList[characterID].index = fieldIndex;
            }

            /// <summary>
            /// オブジェクト情報を代入してもらう関数
            /// </summary>
            /// <param name="thisObject">対象のオブジェクト</param>
            /// <param name="characterID">キャラクターID</param>
            public void CharacterDataReceber(GameObject thisObject,int characterID)
            {
                characterDataList[characterID].thisObject = thisObject;

                Characters[characterID] = thisObject;
            }
        }

    }
}