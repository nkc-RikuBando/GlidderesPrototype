using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Player_namespace
    {
        public class PlayerCore
        {
            // �o�t���X�g
            public enum BuffList
            {
                ATTACKUP, ATTACKDOWN,
                GARDUP, GARDDOWN,
                MOVEUP, MOVEDOWN,
                FIELDUP, FIELDDOWN,
                LENGTH
            }

            [SerializeField] private int playerID = 0; // �L�����N�^�[�̔ԍ�
            [SerializeField] private int characterHP = 100; // �L�����N�^�[��HP

            private List<BuffList> buff = new List<BuffList>();

            /// <summary>
            /// �o�t�𑝂₷
            /// </summary>
            /// <param name="add">�ǉ�����o�t</param>
            public void BuffAdd(BuffList add)
            {
                buff.Add(add);
            }

            /// <summary>
            /// �o�t�����炷
            /// </summary>
            /// <param name="remove">���炷�o�t</param>
            public void BuffRemove(BuffList remove)
            {
                buff.Remove(remove);
            }

            /// <summary>
            /// �o�t������
            /// </summary>
            public void BuffNull()
            {
                buff.Clear();
            }
        }

    }
}