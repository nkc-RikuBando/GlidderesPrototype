using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorController : MonoBehaviour
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
}
