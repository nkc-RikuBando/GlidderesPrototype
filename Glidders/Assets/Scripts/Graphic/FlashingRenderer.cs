using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Glidders
{
    public class FlashingRenderer : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        [SerializeField] private Color flashingColor = new Color();
        [SerializeField] private float flashingInterval = 1;

        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.DOColor(flashingColor, flashingInterval).SetLoops(-1, LoopType.Yoyo);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
