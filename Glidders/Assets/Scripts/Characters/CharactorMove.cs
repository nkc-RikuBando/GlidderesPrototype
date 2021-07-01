using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;


namespace Glidders
{
    public class CharactorMove : MonoBehaviour
    {

        FieldIndex playerPosition; // Playerのグリッド上の座標を保存する
        FieldIndex[] positions; // 移動量をグリッド上の座標で保存しておく
        IGetFieldInformation fieldInfo = GameObject.Find("FiledCore").GetComponent<FieldCore>(); // インターフェース取得

        private Vector3 targetPos; // 移動地点のtransform上の座標の保存用
        // デバッグ用
        public struct MovePosition
        {
            public int width;
            public int height;
        }
        private Vector2 MoveVce;
        private GameObject[] players;

        /// <summary>
        /// Characterの移動を実行するメソッド
        /// </summary>
        /// <param name="fields"></param>
        public void MoveOrder(FieldIndex[] fields)
        {
            positions = fields; // FieldIndexを受け取る

            for (int i = 0; i < players.Length; i++)
            {
                playerPosition = fieldInfo.GetPlayerPosition(i); // 対応キャラクターの情報をグリッド上の座標に変換する

                for (int j = 0; j < positions.Length; i++) 
                {
                    int hight = playerPosition.row + positions[j].row; // 縦方向の移動量をセット
                    int width = playerPosition.column + positions[j].column; // 横方向の移動量をセット

                    targetPos = fieldInfo.GetTilePosition(new FieldIndex(width, hight)); // 目標地点を移動量を加味したグリッド上の座標をVector3に変換
                    Vector3 vec = targetPos - fieldInfo.GetTilePosition(playerPosition); // 目標地点から自分の座標を減算し、移動のベクトルを求める

                    TileChecker(); // タイル情報のチェックを行う

                    // 移動量を確認し、通常移動かテレポート移動かを確認する
                    if (IsDistanceCheck(hight) && IsDistanceCheck(width)) Move(vec);
                    else Teleport();
                }

                IEnumerable Move(Vector3 vec)
                {
                    bool check_move = false; // 移動確認変数
                    while (check_move)
                    {
                        // 目標地点に到着したとき、移動確認変数をfalseに変更 到着していない場合、移動を実行
                        if (targetPos == players[i].transform.position) check_move = false;
                        else players[i].transform.position += vec;

                        yield return null;
                    }
                }
                IEnumerable Teleport()
                {
                    bool check_move = false; // 移動確認変数
                    while (check_move)
                    {
                        // 目標地点に到着したとき、移動確認変数をfalseに変更　到着していない場合、テレポートを実行
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
        /// 移動量確認関数
        /// </summary>
        /// <param name="distance">移動量</param>
        /// <returns></returns>
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