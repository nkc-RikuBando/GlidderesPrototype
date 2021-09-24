using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Buff;

namespace Glidders
{
    namespace Manager
    {
        /// <summary>
        /// �L�����N�^�����i�[����\����
        /// </summary>
        public struct CharacterData
        {
            /// <summary>
            /// ���g�̃I�u�W�F�N�g
            /// </summary>
            public GameObject thisObject;
            /// <summary>
            /// ���g�̃O���b�h��̍��W
            /// </summary>
            public FieldIndex index;
            /// <summary>
            /// �ړ����
            /// </summary>
            public MoveSignal moveSignal;
            /// <summary>
            /// �����ύX���
            /// </summary>
            public DirecionSignal direcionSignal;
            /// <summary>
            /// �U�����
            /// </summary>
            public AttackSignal attackSignal;
            /// <summary>
            /// �L�����N�^�[�̖��O
            /// </summary>
            public CharacterName characterName;
            /// <summary>
            /// �o�t�̏��
            /// </summary>
            public List<BuffViewData> buffView;
            /// <summary>
            /// �o�t�̓��e
            /// </summary>
            public List<List<BuffValueData>> buffValue;
            /// <summary>
            /// �o�t�̌o�߃^�[����
            /// </summary>
            public List<List<int>> buffTurn;
            /// <summary>
            /// ���g�̃G�i�W�[
            /// </summary>
            public int energy { get; set; }
            /// <summary>
            /// �s���̉�
            /// </summary>
            public bool canAct { get; set; }
            /// <summary>
            /// ���g�̃|�C���g��
            /// </summary>
            public int point { get; set; }
            /// <summary>
            /// �v���C���̖��O
            /// </summary>
            public string playerName { get; set; }
            /// <summary>
            /// �v���C���ԍ�
            /// </summary>
            public int playerNumber { get; set; }
        }
    }
}