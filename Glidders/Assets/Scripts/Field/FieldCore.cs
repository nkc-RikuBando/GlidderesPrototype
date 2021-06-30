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
                // �_���[�W�t�B�[���h���L�҂̃v���C���[�ԍ���ԋp���鏈�����L�q���Ă��������B
                return 0;
            }

            public bool IsPassingGrid(FieldIndex fieldIndex)
            {
                // �O���b�h�̒ʍs�ۂ�ԋp���鏈�����L�q���Ă��������B�itrue�Ȃ�ʍs�\�j
                return true;
            }

            public FieldIndex GetPlayerPosition(int playerNumber)
            {
                // �v���C���[�ԍ������Ƃɂ��̃v���C���[�̍��W��ԋp���鏈�����L�q���Ă��������B
                throw new System.NotImplementedException();
            }
        }
    }
}
