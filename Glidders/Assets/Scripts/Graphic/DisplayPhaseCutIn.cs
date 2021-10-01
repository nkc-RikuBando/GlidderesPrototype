using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;


namespace Glidders
{
    namespace Graphic
    {
        public class DisplayPhaseCutIn : MonoBehaviour
        {
            [SerializeField] private Sprite commandPhaseSprite;
            [SerializeField] private Sprite actionPhaseSprite;

            [SerializeField] private AnimationClip commandPhaseAnimation;
            //[SerializeField] private AnimationClip actionPhaseAnimation;

            private Animation cutInAnimation;
            private Image cutInImage;

            enum Mode
            {
                COMMAND,
                ACTION
            }

            // Start is called before the first frame update
            void Start()
            {
                cutInImage = GetComponent<Image>();
                cutInAnimation = GetComponent<Animation>();
            }

            // Update is called once per frame
            void Update()
            {

            }

            private void StartCutIn()
            {
                cutInAnimation.Play();
            }

            public void StartCommandPhaseCutIn()
            {
                cutInImage.sprite = commandPhaseSprite;
                StartCutIn();
            }

            public void StartActionPhaseCutIn()
            {
                cutInImage.sprite = actionPhaseSprite;
                StartCutIn();
            }
        }

    }
}
