using UnityEngine;

namespace Inputer
{
    interface IInput
    {
        Vector3 CursorPositon();
        bool IsCursorInside();
        bool IsClick();
        void SetBorder();
        void PointingRaycast();
    }
}





