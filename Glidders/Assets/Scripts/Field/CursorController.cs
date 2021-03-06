using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Glidders
{
    public class CursorController : MonoBehaviour, IGetCursorPosition
    {
        [SerializeField] private GameObject inputObject;
        private Inputer.IInput iinput;

        [SerializeField] private Tilemap tilemap;

        // Start is called before the first frame update
        void Start()
        {
            iinput = inputObject.GetComponent<Inputer.IInput>();
            iinput.SetBorder();
        }

        // Update is called once per frame
        void Update()
        {
            CursorMove();
        }

        private void CursorMove()
        {
            if (!iinput.IsCursorInside())
            {
                return;
            }
            Vector3Int cellPosition = tilemap.WorldToCell(iinput.CursorPositon());
            Vector3 cursorPosition = tilemap.CellToWorld(cellPosition);
            cursorPosition.y += tilemap.cellSize.y / 2;
            transform.position = cursorPosition;
        }

        public Vector3Int GetCursorPosition()
        {
            return tilemap.WorldToCell(iinput.CursorPositon());
        }

        public FieldIndex GetCursorIndex()
        {
            Vector3Int cursorPositionOnGrid = tilemap.WorldToCell(iinput.CursorPositon());
            return new FieldIndex(Mathf.Abs(cursorPositionOnGrid.y), cursorPositionOnGrid.x);
        }
    }
}
