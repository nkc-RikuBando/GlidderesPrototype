using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inputer
{
    public class MouseInput : CursorPositonCheck, IInput
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector3 CursorPositon()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10.0f;
            return Camera.main.ScreenToWorldPoint(mousePos);

        }

        public bool IsCursorInside()
        {
            return IsCursorInside(Input.mousePosition);
        }


        public bool IsClick()
        {
            return Input.GetMouseButtonDown(0);
        }

        public void PointingRaycast()
        {
            float maxDistance = 100f;
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector3.back, maxDistance);
            if (!hit) return;
            HitRayFromMouse hitRayFromMouse = hit.transform.GetComponent<HitRayFromMouse>();
            if (hitRayFromMouse == null) return;
            if (IsClick()) hitRayFromMouse.HitRaycastIsClick();
            hitRayFromMouse.HitRaycast();
        }
    }
}
