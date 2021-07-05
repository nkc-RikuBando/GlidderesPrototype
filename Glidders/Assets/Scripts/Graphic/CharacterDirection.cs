using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Glidders
{
    namespace Graphic
    {
        public class CharacterDirection : MonoBehaviour
        {
            private Animator characterAnimator;
            const string IS_BACK = "IsBack";

            // Start is called before the first frame update
            void Start()
            {
            }

            private void Awake()
            {
                characterAnimator = GetComponent<Animator>();
            }

            // Update is called once per frame
            void Update()
            {

            }

            public void SetDirection(FieldIndexOffset direction)
            {
                if (IsBack(direction)) characterAnimator.SetBool(IS_BACK, true);
                else characterAnimator.SetBool(IS_BACK, false);

                Vector2 leftScale = new Vector2(1, 1);
                Vector2 rightScale = new Vector2(-1, 1);
                if (IsLeft(direction)) gameObject.transform.localScale = leftScale;
                else gameObject.transform.localScale = rightScale;
            }

            private bool IsBack(FieldIndexOffset direction)
            {
                if (direction == FieldIndexOffset.right) return true;
                if (direction == FieldIndexOffset.up) return true;
                return false;
            }

            private bool IsLeft(FieldIndexOffset direction)
            {
                if (direction == FieldIndexOffset.left) return true;
                if (direction == FieldIndexOffset.up) return true;
                return false;
            }
        }
    }
}

