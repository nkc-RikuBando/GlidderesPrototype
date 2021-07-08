using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Graphic
    {
        public class PlayEffect : MonoBehaviour
        {
            [SerializeField] GameObject[] skillEffects;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void PlaySkillEffect(int skillNumber)
            {
                GameObject effectObject = Instantiate(skillEffects[skillNumber - 1]);
                effectObject.transform.position = gameObject.transform.position;
                effectObject.transform.localScale = gameObject.transform.localScale;
            }
        }
    }
}