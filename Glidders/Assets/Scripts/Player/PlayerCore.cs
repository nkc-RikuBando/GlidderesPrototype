using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Player_namespace
    {
        public class PlayerCore:MonoBehaviour
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

            [SerializeField] private int playerID = 0; // �v���C���[�̔ԍ�
            [SerializeField] private CharacterName characterID; // �L�����N�^�[�̔ԍ�

            public IEnumerator nemusugi()
            {
                bool Sleep = true;

                yield return new WaitForSeconds(10000);

                Sleep = false;
            }

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

            public void ID_Receiver(int playerID,CharacterName character)
            {
                this.playerID = playerID;
                characterID = character;
            }
        }

    }
}