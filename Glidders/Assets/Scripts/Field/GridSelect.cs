using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Glidders
{
    namespace Field
    {
        public class GridSelect : MonoBehaviour
        {
            [SerializeField] private FieldCore fieldCore;
            [SerializeField] private CursorController cursorController;
            [SerializeField] private GameObject inputObject;
            private Inputer.IInput iinput;

            // Start is called before the first frame update
            void Start()
            {
                iinput = inputObject.GetComponent<Inputer.IInput>();
            }

            // Update is called once per frame
            void Update()
            {
                DisplayGrid();
            }

            private void DisplayGrid()
            {
                if (!iinput.IsCursorInside()) return;
                if (!iinput.IsClick()) return;
                var fieldName = fieldCore.GetFieldName();
                Debug.Log(fieldName[GetSelectFieldCode() / 100]);
            }

            private int GetSelectFieldCode()
            {
                Vector3Int cursorPosition = cursorController.GetCursorPosition();
                if (!fieldCore.GetTilemap().HasTile(cursorPosition)) return 0;
                return fieldCore.GetFieldDeta(cursorPosition.x, cursorPosition.y);
            }
        }
    }

}
