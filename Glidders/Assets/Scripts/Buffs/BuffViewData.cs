using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Buff
    {
        public class BuffViewData : ScriptableObject
        {
            [SerializeField]
            public Sprite buffIcon;                     // �o�t�̃A�C�R��

            [SerializeField]
            public string buffName;                     // �o�t�̖���

            [SerializeField]
            public string buffCaption;                  // �o�t�̐�����

            [SerializeField]
            public List<BuffValueData> buffValueList;   // ���ۂ̃o�t���
        }
    }
}
