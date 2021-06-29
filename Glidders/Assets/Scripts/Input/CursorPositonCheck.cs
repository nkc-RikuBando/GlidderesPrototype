using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Inputer
    {
        public class CursorPositonCheck : MonoBehaviour
        {
            public float minX { get; set; }
            public float maxX { get; set; }
            public float minY { get; set; }
            public float maxY { get; set; }

            // Start is called before the first frame update
            void Start()
            {
            }

            public void SetBorder()
            {
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;
                float rectX = Camera.main.rect.x;
                float rectY = Camera.main.rect.y;
                float rectW = Camera.main.rect.width;
                float rectH = Camera.main.rect.height;

                minX = screenWidth * rectX;
                maxX = screenWidth * (rectX + rectW);
                minY = screenHeight * rectY;
                maxY = screenHeight * (rectY + rectH);
            }

            public bool IsCursorInside(Vector2 cursorPosition)
            {
                if (!(cursorPosition.x > minX && cursorPosition.x < maxX)) return false;
                if (!(cursorPosition.y > minY && cursorPosition.y < maxY)) return false;
                return true;
            }
        }
    }
}

