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
            [SerializeField] private Tilemap damageFieldTilemap;

            [SerializeField] private Tile selectableTile;

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

            public void ClearSelectableTileMap()
            {
                selectableTilemap.ClearAllTiles();
            }

            public void DisplayDamageFieldTilemap()
            {

            }
        }
    }
}

