using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Graphic
    {
        public class PlayEffect : MonoBehaviour
        {
            [SerializeField] GameObject[] skillEffectsFront;
            [SerializeField] GameObject[] skillEffectsBack;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void PlaySkillEffectFront(int skillNumber)
            {
                GameObject effectObject = Instantiate(skillEffectsFront[skillNumber - 1]);
                effectObject.transform.position = gameObject.transform.position;
                effectObject.transform.localScale = gameObject.transform.localScale;
            }

            public void PlaySkillEffectBack(int skillNumber)
            {
                GameObject effectObject = Instantiate(skillEffectsBack[skillNumber - 1]);
                effectObject.transform.position = gameObject.transform.position;
                effectObject.transform.localScale = gameObject.transform.localScale;
            }
        }
    }
}