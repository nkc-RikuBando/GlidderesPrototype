using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Field
{
    public class FieldCore : MonoBehaviour
    {
        
        [SerializeField] private GameObject inputObject;

        private Inputer.IInput iinput;

        [SerializeField] private Tilemap tilemap;
        private FieldSet fieldSet;
        [SerializeField] private TileBase[] tile;
        [SerializeField] private int[] fieldCode;
        [SerializeField] private string[] fieldName;

        private int[,] fieldDeta = new int[9, 9];


        // Start is called before the first frame update
        void Start()
        {
            iinput = inputObject.GetComponent<Inputer.IInput>();
            fieldSet = GetComponent<FieldSet>();
            fieldSet.SetFieldTable(ref fieldDeta, tilemap);
        }

        // Update is called once per frame
        void Update()
        {


        }

        public int GetFieldDeta(int x = 0, int y = 0)
        {
            return fieldDeta[x, Mathf.Abs(y)];
        } 

        public Tilemap GetTilemap()
        {
            return tilemap;
        }

        public TileBase[] GetTile()
        {
            return tile;
        }

        public int[] GetGridCode()
        {
            return fieldCode;
        }

        public string[] GetFieldName()
        {
            return fieldName;
        }
    }
}
