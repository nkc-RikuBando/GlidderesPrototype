using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class CommandInput : MonoBehaviour
    {
        private int inputNumber = 0;
        private int selectNumber = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetInputNumber(int setNumber)
        {
            inputNumber = setNumber;
        }

        public int GetInputNumber()
        {
            return inputNumber;
        }

        public void SetSelectNumber(int setNumber)
        {
            selectNumber = setNumber;
        }

        public int GetSelectNumber()
        {
            return selectNumber;
        }
    }
}

