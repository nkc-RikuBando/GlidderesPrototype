using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Glidders
{
    namespace Graphic
    {
        public class FlashingImage : MonoBehaviour
        {
            private Image image;
            [SerializeField] private Color flashingColor = Color.white;
            [SerializeField] private float flashingInterval = 1f;

            // Start is called before the first frame update
            void Start()
            {
                image = GetComponent<Image>();
                image.DOColor(flashingColor, flashingInterval).SetLoops(-1, LoopType.Yoyo);
            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}
