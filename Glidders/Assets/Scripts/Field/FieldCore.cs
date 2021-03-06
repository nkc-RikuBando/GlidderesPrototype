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

            [SerializeField] private int maxRow = 9;
            [SerializeField] private int maxColumn = 9;

            [SerializeField] private GameObject inputObject;

            private Inputer.IInput iinput;

            [SerializeField] private Tilemap fieldTilemap;
            [SerializeField] private Tilemap SelectableTilemap;
            [SerializeField] private Tilemap DamageFieldTilemap;

            private FieldSet fieldSet;
            [SerializeField] private TileBase[] tile;
            [SerializeField] private int[] tileCode;
            [SerializeField] private string[] tileName;

            private int[,] fieldDeta;

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
                fieldDeta = new int[maxRow, maxColumn];
                iinput = inputObject.GetComponent<Inputer.IInput>();
                fieldSet = GetComponent<FieldSet>();
                fieldSet.SetFieldTable(ref fieldDeta, fieldTilemap);
            }

            // Update is called once per frame
            void Update()
            {


            }

            public int[,] GetFieldData()
            {
                return fieldDeta;
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

            public void UpdateFieldData()
            {
                for(int i = 0; i < maxRow; i++)
                {
                    for(int j = 0; j < maxColumn; j++)
                    {
                        if (GetLevel(fieldDeta[i, j]) == 0) continue;
                        fieldDeta[i, j]--;
                        if (GetLevel(fieldDeta[i, j]) != 0) continue;
                        fieldDeta[i, j] = GetFieldCode(fieldDeta[i, j]) * 100;
                    }
                }
            }

            public int GetDamageFieldOwner(FieldIndex fieldIndex)
            {
                const int DAMAGE_FIELD_NONE = -1;
                int damageField = fieldDeta[fieldIndex.row, fieldIndex.column] % 100;
                if (GetLevel(damageField) > 0) return GetOwner(damageField);
                return DAMAGE_FIELD_NONE;
            }

            public bool IsPassingGrid(FieldIndex fieldIndex)
            {
                // ?O???b?h?????s?????????p???????????L?q?????????????B?itrue???????s???\?j
                return ((int)(fieldDeta[fieldIndex.row, fieldIndex.column] / 100) > (int)FieldCode.IMPENETABLE);
            }

            private int GetFieldCode(int data)
            {
                return data / 100;
            }

            public FieldIndex GetPlayerPosition(int playerNumber)
            {
                // ?v???C???[?????????????????v???C???[?????W?????p???????????L?q?????????????B
                return playerPositionTable[playerNumber];
            }

            public Vector3 GetTilePosition(FieldIndex fieldIndex)
            {
                // ?O???b?h?????W?????????A?????O???b?h??scene??????transform.position???l?????p???????????L?q?????????????B?i?????v???C???[?????????p?????????j
                return fieldTilemap.GetCellCenterWorld(new Vector3Int(fieldIndex.column,  - fieldIndex.row, 0));
            }

            public FieldIndex GetFieldSize()
            {
                return new FieldIndex(maxRow - 1, maxColumn - 1);
            }

            public void SetPlayerPosition(int playerNumber, FieldIndex position)
            {
                // ?v???C???[???????O???b?h???W?????????A?w?????????v???C???[?????u?????????????????????W???X?V?????????????B
                throw new System.NotImplementedException();
            }

            public void SetDamageField(int playerNumber, int damageFieldLevel, FieldIndex position)
            {
                int add = damageFieldLevel > 0 ? 1 : 0;
                int addLevel = damageFieldLevel + add;
                int newDamageField = default;
                int damageField = fieldDeta[position.row, position.column] % 100;
                int groundData = (int)(fieldDeta[position.row, position.column] / 100) * 100;

                if (GetLevel(damageField) > 0)
                {
                    // ?_???[?W?t?B?[???h??????????????????
                    if (GetOwner(damageField) == playerNumber)
                    {
                        newDamageField = playerNumber * 10 + Mathf.Max(GetLevel(damageField), addLevel);

                    }
                    else // ???l????
                    {
                        int newOwner = addLevel > GetLevel(damageField) ? playerNumber : GetOwner(damageField);
                        int newLevel = Mathf.Max(GetLevel(damageField), addLevel) - Mathf.Min(GetLevel(damageField), addLevel);
                        newDamageField = (newOwner * 10) + newLevel;
                    }
                }
                else newDamageField = (playerNumber * 10) + addLevel;
                fieldDeta[position.row, position.column] = groundData + newDamageField;
            }

            private int GetOwner(int damageField)
            {
                return damageField / 10;
            }

            private int GetLevel(int damageField)
            {
                return damageField % 10;
            }
        }
    }
}
