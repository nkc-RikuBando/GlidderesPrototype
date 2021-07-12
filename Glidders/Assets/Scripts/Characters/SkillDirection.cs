using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Character
    {
        public class SkillDirection : MonoBehaviour
        {
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public static FieldIndexOffset ChangeSkillDirection(FieldIndexOffset offset, FieldIndexOffset direction)
            {
                if (IsFront(direction)) return new FieldIndexOffset(-offset.rowOffset, -offset.columnOffset);
                if (IsBack(direction)) return offset;
                if (IsLeft(direction)) return new FieldIndexOffset(-offset.columnOffset, offset.rowOffset);
                if (IsRight(direction)) return new FieldIndexOffset(offset.columnOffset, -offset.rowOffset);
                return offset;
            }

            public static FieldIndexOffset[] ChangeSkillDirection(FieldIndexOffset[] offset, FieldIndexOffset direction)
            {
                FieldIndexOffset[] returnOffset = offset;
                for(int i = 0; i < offset.Length; i++)
                {
                    returnOffset[i] = ChangeSkillDirection(offset[i], direction);
                }
                return returnOffset;
            }

            public static bool IsLeft(FieldIndexOffset direction)
            {
                if (!(direction.columnOffset < 0)) return false;
                if (Mathf.Abs(direction.columnOffset) <= Mathf.Abs(direction.rowOffset)) return false;
                return true;
            }

            public static bool IsRight(FieldIndexOffset direction)
            {
                if (!(direction.columnOffset > 0)) return false;
                if (Mathf.Abs(direction.columnOffset) <= Mathf.Abs(direction.rowOffset)) return false;
                return true;
            }

            public static bool IsBack(FieldIndexOffset direction)
            {
                if (direction.rowOffset > 0) return false;
                if (Mathf.Abs(direction.columnOffset) > Mathf.Abs(direction.rowOffset)) return false;
                return true;
            }

            public static bool IsFront(FieldIndexOffset direction)
            {
                if (direction.rowOffset <= 0) return false;
                if (Mathf.Abs(direction.columnOffset) > Mathf.Abs(direction.rowOffset)) return false;
                return true;
            }


        }
    }
}