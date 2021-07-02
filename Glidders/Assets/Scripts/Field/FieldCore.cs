using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Glidders
{
    namespace Field
    {
        public class FieldCore : MonoBehaviour, IGetFieldInformation, ISetFieldInformation
        {

            [SerializeField] private GameObject inputObject;

            private Inputer.IInput iinput;

            [SerializeField] private Tilemap fieldTilemap;
            [SerializeField] private Tilemap SelectableTilemap;
            [SerializeField] private Tilemap DamageFieldTilemap;

            private FieldSet fieldSet;
            [SerializeField] private TileBase[] tile;
            [SerializeField] private int[] tileCode;
            [SerializeField] private string[] tileName;

            private int[,] fieldDeta = new int[9, 9];

            private FieldIndex[] playerPositionTable = new FieldIndex[4];

            private enum FieldCode
            {
                OUTSIDE,
                IMPENETABLE,
                FLOOR,
            }

            // Start is called before the first frame update
            void Start()
            {
                iinput = inputObject.GetComponent<Inputer.IInput>();
                fieldSet = GetComponent<FieldSet>();
                fieldSet.SetFieldTable(ref fieldDeta, fieldTilemap);
            }

            // Update is called once per frame
            void Update()
            {


            }

            public int GetGridCode(FieldIndex fieldIndex)
            {
                return fieldDeta[fieldIndex.row, fieldIndex.column];
            }

            public int GetGridCode(int x, int y)
            {
                return fieldDeta[y, x];
            }

            public Tilemap GetFieldTilemap()
            {
                return fieldTilemap;
            }

            public TileBase[] GetTile()
            {
                return tile;
            }

            public int[] GetFieldCode()
            {
                return tileCode;
            }

            public string[] GetFieldName()
            {
                return tileName;
            }

            public int GetDamageFieldOwner(FieldIndex fieldIndex)
            {
                // ダメージフィールド所有者のプレイヤー番号を返却する処理を記述してください。
                return 0;
            }

            public bool IsPassingGrid(FieldIndex fieldIndex)
            {
                // グリッドの通行可否を返却する処理を記述してください。（trueなら通行可能）
                return (fieldDeta[fieldIndex.row, fieldIndex.column] > (int)FieldCode.IMPENETABLE);
            }

            public FieldIndex GetPlayerPosition(int playerNumber)
            {
                // プレイヤー番号をもとにそのプレイヤーの座標を返却する処理を記述してください。
                return playerPositionTable[playerNumber];
            }

            public Vector3 GetTilePosition(FieldIndex fieldIndex)
            {
                // グリッドの座標をもとに、そのグリッドのscene上でのtransform.positionの値を返却する処理を記述してください。（主にプレイヤーの移動に用いるため）
                return Vector2.zero;
            }

            public void SetPlayerPosition(int playerNumber, FieldIndex position)
            {
                // プレイヤー番号とグリッド座標をもとに、指定されたプレイヤーの位置情報を送られてきた座標に更新してください。
                throw new System.NotImplementedException();
            }
        }
    }
}
