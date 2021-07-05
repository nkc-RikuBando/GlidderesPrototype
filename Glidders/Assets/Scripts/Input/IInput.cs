using UnityEngine;

namespace Glidders
{
    namespace Inputer
    {
        interface IInput
        {
            Vector3 CursorPositon();
            bool IsCursorInside();
            bool IsClickDown();
            bool IsClickUp();
            void SetBorder();
            void PointingRaycast();
        }
    }
}
