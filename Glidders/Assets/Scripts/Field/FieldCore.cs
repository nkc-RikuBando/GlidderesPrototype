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
                // �_���[�W�t�B�[���h���L�҂̃v���C���[�ԍ���ԋp���鏈�����L�q���Ă��������B
                return 0;
            }

            public bool IsPassingGrid(FieldIndex fieldIndex)
            {
                // �O���b�h�̒ʍs�ۂ�ԋp���鏈�����L�q���Ă��������B�itrue�Ȃ�ʍs�\�j
                return (fieldDeta[fieldIndex.row, fieldIndex.column] > (int)FieldCode.IMPENETABLE);
            }

            public FieldIndex GetPlayerPosition(int playerNumber)
            {
                // �v���C���[�ԍ������Ƃɂ��̃v���C���[�̍��W��ԋp���鏈�����L�q���Ă��������B
                return playerPositionTable[playerNumber];
            }

            public Vector3 GetTilePosition(FieldIndex fieldIndex)
            {
                // �O���b�h�̍��W�����ƂɁA���̃O���b�h��scene��ł�transform.position�̒l��ԋp���鏈�����L�q���Ă��������B�i��Ƀv���C���[�̈ړ��ɗp���邽�߁j
                return Vector2.zero;
            }

            public void SetPlayerPosition(int playerNumber, FieldIndex position)
            {
                // �v���C���[�ԍ��ƃO���b�h���W�����ƂɁA�w�肳�ꂽ�v���C���[�̈ʒu���𑗂��Ă������W�ɍX�V���Ă��������B
                throw new System.NotImplementedException();
            }
        }
    }
}
