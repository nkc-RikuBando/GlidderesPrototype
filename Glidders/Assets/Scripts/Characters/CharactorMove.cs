using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;


namespace Glidders
{
    public class CharactorMove : MonoBehaviour
    {

        FieldIndex playerPosition; // Player�̃O���b�h��̍��W��ۑ�����
        FieldIndex[] positions; // �ړ��ʂ��O���b�h��̍��W�ŕۑ����Ă���
        IGetFieldInformation fieldInfo = GameObject.Find("FiledCore").GetComponent<FieldCore>(); // �C���^�[�t�F�[�X�擾

        private Vector3 targetPos; // �ړ��n�_��transform��̍��W�̕ۑ��p
        // �f�o�b�O�p
        public struct MovePosition
        {
            public int width;
            public int height;
        }
        private Vector2 MoveVce;
        private GameObject[] players;

        /// <summary>
        /// Character�̈ړ������s���郁�\�b�h
        /// </summary>
        /// <param name="fields"></param>
        public void MoveOrder(FieldIndex[] fields)
        {
            positions = fields; // FieldIndex���󂯎��

            for (int i = 0; i < players.Length; i++)
            {
                playerPosition = fieldInfo.GetPlayerPosition(i); // �Ή��L�����N�^�[�̏����O���b�h��̍��W�ɕϊ�����

                for (int j = 0; j < positions.Length; i++) 
                {
                    int hight = playerPosition.row + positions[j].row; // �c�����̈ړ��ʂ��Z�b�g
                    int width = playerPosition.column + positions[j].column; // �������̈ړ��ʂ��Z�b�g

                    targetPos = fieldInfo.GetTilePosition(new FieldIndex(width, hight)); // �ڕW�n�_���ړ��ʂ����������O���b�h��̍��W��Vector3�ɕϊ�
                    Vector3 vec = targetPos - fieldInfo.GetTilePosition(playerPosition); // �ڕW�n�_���玩���̍��W�����Z���A�ړ��̃x�N�g�������߂�

                    TileChecker(); // �^�C�����̃`�F�b�N���s��

                    // �ړ��ʂ��m�F���A�ʏ�ړ����e���|�[�g�ړ������m�F����
                    if (IsDistanceCheck(hight) && IsDistanceCheck(width)) Move(vec);
                    else Teleport();
                }

                IEnumerable Move(Vector3 vec)
                {
                    bool check_move = false; // �ړ��m�F�ϐ�
                    while (check_move)
                    {
                        // �ڕW�n�_�ɓ��������Ƃ��A�ړ��m�F�ϐ���false�ɕύX �������Ă��Ȃ��ꍇ�A�ړ������s
                        if (targetPos == players[i].transform.position) check_move = false;
                        else players[i].transform.position += vec;

                        yield return null;
                    }
                }
                IEnumerable Teleport()
                {
                    bool check_move = false; // �ړ��m�F�ϐ�
                    while (check_move)
                    {
                        // �ڕW�n�_�ɓ��������Ƃ��A�ړ��m�F�ϐ���false�ɕύX�@�������Ă��Ȃ��ꍇ�A�e���|�[�g�����s
                        if (targetPos == players[i].transform.position) check_move = false;
                        else players[i].transform.position = targetPos;

                        yield return null;
                    }
                }
                void TileChecker()
                {

                }
            }
        }

        /// <summary>
        /// �ړ��ʊm�F�֐�
        /// </summary>
        /// <param name="distance">�ړ���</param>
        /// <returns></returns>
        bool IsDistanceCheck(int distance)
        {
            #region switch��
            //return distance switch
            //{
            //    -1 => true,
            //    0 => true,
            //    1 => true,
            //    _ => false
            //};
            #endregion
            if (distance > 1) return false;
            if (distance < -1) return false;
            return true;
        }
    }

}