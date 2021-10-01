using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Graphic
    {
        public class DeleteSkillCutIn : MonoBehaviour
        {
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void EndAnimation()
            {
                Time.timeScale = 1.0f;
                gameObject.SetActive(false);
            }
        }

    }
}

