using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class HitRayFromMouse : MonoBehaviour
    {
        [SerializeField] private int commandNumber = 0;
        [SerializeField] private CommandInput commandInput;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void HitRaycast()
        {
            commandInput.SetSelectNumber(commandNumber);
        }

        public void HitRaycastIsClick()
        {
            commandInput.SetInputNumber(commandNumber);
        }
    }
}
