using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;


namespace Glidders
{
    public class CharactorMove : MonoBehaviour
    {

        // �f�o�b�O�p
        public struct MovePosition
        {
            public int width;
            public int height;
        }
        private GameObject[] players;
        private MovePosition[] positions;
        public void MoveOrder(List<int> Movepos)
        {
            for (int i = 0; i < players.Length; i++)
            {
                for (int j = 0; j < positions.Length; i++)
                {
                    int hight = positions[j].height;
                    int width = positions[j].width;
                    TileChecker();
                    if (IsDistanceCheck(hight) && IsDistanceCheck(width)) Move(hight, width);
                    else Teleport(hight, width);
                }
                IEnumerable Move(int MoveHeight, int MoveWidth)
                {
                    bool check_move = false;
                    while (check_move)
                    {
                        players[i].transform.position += new Vector3(MoveWidth, MoveHeight);


                        yield return null;
                    }
                }
                void Teleport(int MoveHeight, int MoveWidth)
                {

                }
                void TileChecker()
                {

                }
            }
        }
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