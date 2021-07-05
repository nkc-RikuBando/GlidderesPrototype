using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using DG.Tweening;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterMove : MonoBehaviour
        {

            FieldIndex playerPosition; // Player�̃O���b�h��̍��W��ۑ�����
            FieldIndexOffset[] positions; // �ړ��ʂ��O���b�h��̍��W�ŕۑ����Ă���
            IGetFieldInformation fieldInfo_get;
            ISetFieldInformation fieldInfo_set;
            
            private Vector3 targetPos; // �ړ��n�_��transform��̍��W�̕ۑ��p
            private GameObject[] character; // �ړ��Ώۂ̃I�u�W�F�N�g
            private Sequence sequence;

            const int TWEEN_MOVE_TIME = 2;

            public CharacterMove()
            {
                // �C���^�[�t�F�[�X�擾
                fieldInfo_get = GameObject.Find("FieldCore").GetComponent<FieldCore>();
                fieldInfo_set = GameObject.Find("FieldCore").GetComponent<FieldCore>();

                character[0] = GameObject.Find("Kaito"); // �L�����N�^���擾(���I�u�W�F�N�g)
                sequence = DOTween.Sequence(); // �V�[�P���X��������
            }

            // �f�o�b�O�p
            public struct MovePosition
            {
                public int width;
                public int height;
            }
            private Vector2 MoveVce;
            private GameObject[] players;
            const int MAX_PLAYER = 4;


            //{
            //    fieldInfo_get = GameObject.Find("FiledCore").GetComponent<FieldCore>();
            //    fieldInfo_set = GameObject.Find("FieldCore").GetComponent<FieldCore>();
            //}

            /// <summary>
            /// Character�̈ړ������s���郁�\�b�h
            /// </summary>
            public void MoveOrder(MoveSignal moveSignal,int id)
            {
                positions = moveSignal.moveDataArray; // FieldIndex���󂯎��

                // playerPosition = fieldInfo_get.GetPlayerPosition(id); // �Ή��L�����N�^�[�̏����O���b�h��̍��W�ɕϊ�����
                playerPosition = new FieldIndex(2, 3);

                for (int i = 0; i < positions.Length; i++)
                {
                    int hight = playerPosition.row + positions[i].rowOffset; // �c�����̈ړ��ʂ��Z�b�g
                    int width = playerPosition.column + positions[i].columnOffset; // �������̈ړ��ʂ��Z�b�g

                    playerPosition = new FieldIndex(width, hight); // �L�����N�^�[�̃O���b�h��̍��W��V�K�̈ʒu�ɕۑ�

                    // GetTilePosition ���ݒ�̂��߃R�����g�A�E�g�@�������������邱��
                    // targetPos = fieldInfo_get.GetTilePosition(playerPosition); // �ڕW�n�_���ړ��ʂ����������O���b�h��̍��W��Vector3�ɕϊ�

                    targetPos = new Vector3(width,hight,0);

                    // GetDamegeFieldOwner �������̂��߃R�����g�A�E�g�@�������������邱��
                    // TileChecker(); // �^�C�����̃`�F�b�N���s��

                    // �ړ��ʂ��m�F���A�ʏ�ړ����e���|�[�g�ړ������m�F����
                    if (IsDistanceCheck(positions[i].rowOffset) && IsDistanceCheck(positions[i].columnOffset)) Move();
                    else Teleport();

                    // SetPlayerPosition �������̂��߃R�����g�A�E�g�@�������������邱��
                    // fieldInfo_set.SetPlayerPosition(id,playerPosition);
                }

            }

            public void Move()
            {
                Debug.Log("MoveCheck");
                sequence.Append(character[0].transform.DOMove(targetPos, TWEEN_MOVE_TIME).SetEase(Ease.Linear)); // �V�[�P���X�ǉ�
            }

            public void Teleport()
            {
                character[0].transform.position = targetPos;
                #region �������R���[�`��
                //bool check_move = true; // �ړ��m�F�ϐ�
                //while (check_move)
                //{
                //    // �ڕW�n�_�ɓ��������Ƃ��A�ړ��m�F�ϐ���false�ɕύX�@�������Ă��Ȃ��ꍇ�A�e���|�[�g�����s
                //    if (targetPos == character.transform.position) check_move = false;
                //    else character.transform.localPosition = targetPos;

                //    yield return null;
                //}
                #endregion
            }

            // GetDamegeFieldOwner �������̂��߃R�����g�A�E�g�@�������������邱��
            //void TileChecker()
            //{
            //    if (fieldInfo_get.GetDamageFieldOwner(playerPosition) == id) return;
            //    else if (fieldInfo_get.GetDamageFieldOwner(playerPosition) != -1) Debug.Log("�ӂ݂܂���");
            //    else return;
            //}

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
}