using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Glidders
{
    namespace Field
    {
        public class FieldSet : MonoBehaviour
        {
            [SerializeField] private FieldCore fieldCore;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void SetFieldTable(ref int[,] setTable, Tilemap targetTilemap)
            {
                for (int i = 0; i < setTable.GetLength(0); i++)
                {
                    for (int j = 0; j < setTable.GetLength(1); j++)
                    {
                        Vector3Int gridPosition = new Vector3Int(i, -j, 0);
                        setTable[i, j] = GetTileCode(targetTilemap.GetTile(gridPosition)) * 100;
                    }
                }
            }

            private int GetTileCode(TileBase targetTile)
            {
                var tile = fieldCore.GetTile();
                var gridCode = fieldCore.GetFieldCode();

                for (int i = 0; i < tile.Length; i++) if (tile[i] == targetTile) return gridCode[i];
                return 900;
            }
        }

    }
}

