using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;


namespace Glidders
{
    public class CharactorMove : MonoBehaviour
    {

        FieldIndex playerPosition;
        FieldIndex[] positions;
        IGetFieldInformation fieldInfo = GameObject.Find("FiledCore").GetComponent<FieldCore>();

        // デバッグ用
        public struct MovePosition
        {
            public int width;
            public int height;
        }
        private Vector2 targetPos;
        private Vector2 MoveVce;
        private GameObject[] players;
        public void MoveOrder(List<int> Movepos)
        {
            for (int i = 0; i < players.Length; i++)
            {
                playerPosition = fieldInfo.GetPlayerPosition(i);
                // targetPos = PlayerPos - fieldInfo.GetTilePosition()

                for (int j = 0; j < positions.Length; i++)
                {
                    int hight = positions[j].row;
                    int width = positions[j].column;



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
                    players[i].transform.position = targetPos;
                }
                void TileChecker()
                {

                }
            }
        }
        bool IsDistanceCheck(int distance)
        {
            #region switch式
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