using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        public class CoreManager : MonoBehaviour
        {
            CharacterMove characterMove;
            SignalManager signalManager;

            public bool moveStart { get; set; }
            public bool attackStart { get; set; }
            public List<MoveSignal> MoveSignalList { get; set; }
            public List<AttackSignal> AttackSignalList { get; set; }

            // �f�o�b�O�p
            MoveSignal moveSignal = new MoveSignal();

            // Start is called before the first frame update
            void Start()
            {
                MoveSignalList = new List<MoveSignal>(); // ���X�g������
                moveSignal.moveDataArray = new FieldIndexOffset[2]; // �z�񐔏�����
                moveSignal.moveDataArray[0] = new FieldIndexOffset(0, 2); // �f�o�b�O�p�@���ڂ̈ړ���
                moveSignal.moveDataArray[1] = new FieldIndexOffset(1, 0); // �f�o�b�O�p�@���ڂ̈ړ���
                MoveSignalList.Add(moveSignal); // �ړ������i�[
                characterMove = new CharacterMove();
                signalManager = new SignalManager();

                // �f�o�b�O�pCharacter�ړ�����
                moveStart = true;

                if (moveStart)
                {
                    foreach (var x in MoveSignalList)
                    {
                        characterMove.MoveOrder(x, 0);
                    }
                }
            }

            // Update is called once per frame
            void Update()
            {
            }

            public void MoveDataReceiver()
            {

            }

            public void AttackDataReceiver()
            {

            }
        }

    }
}