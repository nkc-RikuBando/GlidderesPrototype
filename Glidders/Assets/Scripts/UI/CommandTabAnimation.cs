using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Glidders
{
    public class CommandTabAnimation : MonoBehaviour
    {
        [SerializeField] SetCommandTab setCommandTab;
        const float SHIFT_X = -11;
        const float SHIFT_Y = -41;
        const float ANIMATION_TIME = 0.5f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ReseetAnimation()
        {
            GameObject[] commandTabs = setCommandTab.GetCommandTabs();
            RectTransform rect = commandTabs[0].GetComponent<RectTransform>();
            Vector2 startPositon = rect.position;
            for (int i = 0; i < commandTabs.Length; i++)
            {
                commandTabs[i].GetComponent<RectTransform>().position = startPositon;
            }
        }

        public void OpenAnimationStart()
        {
            GameObject[] commandTabs = setCommandTab.GetCommandTabs();
            for (int i = 0; i < commandTabs.Length; i++)
            {
                RectTransform rect = commandTabs[i].GetComponent<RectTransform>();
                Vector2 rectPosition = rect.position;
                rectPosition.x += SHIFT_X * i;
                rectPosition.y += SHIFT_Y * i;
                rect.DOMove(rectPosition, ANIMATION_TIME);
            }
        }

        public void CloseAnimationStart()
        {
            GameObject[] commandTabs = setCommandTab.GetCommandTabs();
            RectTransform rect = commandTabs[0].GetComponent<RectTransform>();
            Vector2 startPositon = rect.position;
            for (int i = 0; i < commandTabs.Length; i++)
            {
                commandTabs[i].GetComponent<RectTransform>().DOMove(startPositon, ANIMATION_TIME);
            }

        }
    }
}