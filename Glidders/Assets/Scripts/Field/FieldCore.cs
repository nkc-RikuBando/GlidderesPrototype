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

            public int GetDamageFieldOwner(FieldIndex fieldIndex)
            {
                const int DAMAGE_FIELD_NONE = -1;
                int damageField = fieldDeta[fieldIndex.row, fieldIndex.column] % 100;
                if (GetLevel(damageField) > 0) return GetOwner(damageField);
                return DAMAGE_FIELD_NONE;
            }

            public bool IsPassingGrid(FieldIndex fieldIndex)
            {
                // �O���b�h�̒ʍs�ۂ�ԋp���鏈�����L�q���Ă��������B�itrue�Ȃ�ʍs�\�j
                return ((int)(fieldDeta[fieldIndex.row, fieldIndex.column] / 100) > (int)FieldCode.IMPENETABLE);
            }

            public FieldIndex GetPlayerPosition(int playerNumber)
            {
                // �v���C���[�ԍ������Ƃɂ��̃v���C���[�̍��W��ԋp���鏈�����L�q���Ă��������B
                return playerPositionTable[playerNumber];
            }

            public Vector3 GetTilePosition(FieldIndex fieldIndex)
            {
                // �O���b�h�̍��W�����ƂɁA���̃O���b�h��scene��ł�transform.position�̒l��ԋp���鏈�����L�q���Ă��������B�i��Ƀv���C���[�̈ړ��ɗp���邽�߁j
                return fieldTilemap.GetCellCenterWorld(new Vector3Int(fieldIndex.column,  - fieldIndex.row, 0));

            }

            public FieldIndex GetFieldSize()
            {
                return new FieldIndex(maxRow - 1, maxColumn - 1);
            }

            public void SetPlayerPosition(int playerNumber, FieldIndex position)
            {
                // �v���C���[�ԍ��ƃO���b�h���W�����ƂɁA�w�肳�ꂽ�v���C���[�̈ʒu���𑗂��Ă������W�ɍX�V���Ă��������B
                throw new System.NotImplementedException();
            }

            public void SetDamageField(int playerNumber, int damageFieldLevel, FieldIndex position)
            {
                int add = damageFieldLevel > 0 ? 1 : 0;
                int addLevel = damageFieldLevel + add;
                int newDamageField = default;
                int damageField = fieldDeta[position.row, position.column] % 100;
                if (damageField > 0)
                {
                    if (GetOwner(damageField) == playerNumber) damageField = Mathf.Max(GetLevel(damageField), addLevel);
                    else
                    {
                        int newOwner = addLevel > GetLevel(damageField) ? playerNumber : GetOwner(damageField);
                        int newLevel = Mathf.Max(GetLevel(damageField), addLevel) - Mathf.Min(GetLevel(damageField), addLevel);
                        newDamageField = (newOwner * 10) + newLevel;
                    }
                }
                else newDamageField = playerNumber * 10 + addLevel;
                fieldDeta[position.row, position.column] = newDamageField;
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
