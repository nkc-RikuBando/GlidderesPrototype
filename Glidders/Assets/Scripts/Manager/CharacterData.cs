using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Manager
    {
        /// <summary>
        /// �L�����N�^�����i�[����\����
        /// </summary>
        public struct CharacterData
        {
            public GameObject thisObject;
            public FieldIndex index;
            public MoveSignal moveSignal;
            public AttackSignal attackSignal;
            public bool canAct { get; set; }
            public int point { get; set; }
        }
    }
}