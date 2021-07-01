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
        private Vector3 targetPos;
        private Vector2 MoveVce;
        private GameObject[] players;
        public void MoveOrder(FieldIndex[] fields)
        {
            positions = fields;

            for (int i = 0; i < players.Length; i++)
            {
                playerPosition = fieldInfo.GetPlayerPosition(i);
                // targetPos = PlayerPos - fieldInfo.GetTilePosition()

                for (int j = 0; j < positions.Length; i++)
                {
                    int hight = playerPosition.row + positions[j].row;
                    int width = playerPosition.column + positions[j].column;

                    targetPos = fieldInfo.GetTilePosition(new FieldIndex(width, hight));
                    Vector3 vec = targetPos - fieldInfo.GetTilePosition(playerPosition);

                    TileChecker();
                    if (IsDistanceCheck(hight) && IsDistanceCheck(width)) Move(vec);
                    else Teleport();
                }
                IEnumerable Move(Vector3 vec)
                {
                    bool check_move = false;
                    while (check_move)
                    {
                        if (targetPos == players[i].transform.position) check_move = false;
                        else players[i].transform.position += vec;

                        yield return null;
                    }
                }
                IEnumerable Teleport()
                {
                    bool check_move = false;
                    while (check_move)
                    {
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