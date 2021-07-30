using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Glidders
{
    namespace Field
    {
        public class DisplayTileMap : MonoBehaviour
        {
            [SerializeField] private IGetFieldInformation getFieldInformation;

            [SerializeField] private Tilemap selectableTilemap;
            [SerializeField] private Tilemap attackTilemap;
            [SerializeField] private Tilemap damageFieldTilemap;

            [SerializeField] private Tile selectableTile;
            [SerializeField] private Tile attackTile;

            [SerializeField] private Tile[] damageFieldTile;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void DisplaySelectableTileMap(bool[,] selectableGridTable)
            {
                selectableTilemap.ClearAllTiles();
                for(int i = 0; i < selectableGridTable.GetLength(0); i++)
                {
                    for(int j = 0; j < selectableGridTable.GetLength(1); j++)
                    {
                        if (selectableGridTable[i, j])
                        {
                            var position = new Vector3Int(j, -i, 0);
                            selectableTilemap.SetTile(position, selectableTile);
                        }
                    }
                }
            }

            public void DisplaySelectableTile(FieldIndex index)
            {
                Vector3Int position = new Vector3Int(index.column, -index.row, 0);
                selectableTilemap.SetTile(position, selectableTile);
            }

            public void ClearSelectableTileMap()
            {
                selectableTilemap.ClearAllTiles();
            }

            public void DisplayAttackTilemap(FieldIndex index)
            {
                Vector3Int position = new Vector3Int(index.column, -index.row, 0);
                attackTilemap.SetTile(position, attackTile);
            }

            public void ClearAttackTilemap()
            {
                attackTilemap.ClearAllTiles();
            }

            public void DisplayDamageFieldTilemap(FieldIndex index, int playerNumber)
            {
                Vector3Int position = new Vector3Int(index.column, -index.row, 0);
                if (playerNumber < 0) damageFieldTilemap.SetTile(position, null);
                else damageFieldTilemap.SetTile(position, damageFieldTile[playerNumber]);
            }

            public void ClearDamageFieldTilemap()
            {
                damageFieldTilemap.ClearAllTiles();
            }

            public void DisplayDamageFieldTilemap(int[,] indexTable)
            {
                Debug.Log("‚¤‚¥‚¢‚¤‚¥‚¢‚¤‚¥‚¢");
                for (int i = 0; i < indexTable.GetLength(0); i++)
                {
                    for (int j = 0; j < indexTable.GetLength(1); j++)
                    {
                        if (indexTable[i, j] % 10 > 0)
                        {
                            Vector3Int position = new Vector3Int(j, -i, 0);
                            damageFieldTilemap.SetTile(position, damageFieldTile[(indexTable[i, j] % 100) / 10]);
                        }
                    }
                }
            }
        }
    }
}

