using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    public class StartTest : MonoBehaviour
    {
        [SerializeField] private Graphic.HologramController hologramController;

        // Start is called before the first frame update
        void Start()
        {
            hologramController.CreateHologram(0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
