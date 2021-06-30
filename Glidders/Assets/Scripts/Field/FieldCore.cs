using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Glidders
{
    namespace Field
    {
        public class FieldCore : MonoBehaviour, IGetFieldInformation
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

            public int GetDamageFieldOwner(FieldIndex fieldIndex)
            {
                // ダメージフィールド所有者のプレイヤー番号を返却する処理を記述してください。
                return 0;
            }

            public bool IsPassingGrid(FieldIndex fieldIndex)
            {
                // グリッドの通行可否を返却する処理を記述してください。（trueなら通行可能）
                return true;
            }

            public FieldIndex GetPlayerPosition(int playerNumber)
            {
                // プレイヤー番号をもとにそのプレイヤーの座標を返却する処理を記述してください。
                throw new System.NotImplementedException();
            }
        }
    }
}
