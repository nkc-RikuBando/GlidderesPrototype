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

            FieldIndex playerPosition; // Playerのグリッド上の座標を保存する
            FieldIndexOffset[] positions; // 移動量をグリッド上の座標で保存しておく
            IGetFieldInformation fieldInfo_get;
            ISetFieldInformation fieldInfo_set;
            
            private Vector3 targetPos; // 移動地点のtransform上の座標の保存用
            private GameObject[] character; // 移動対象のオブジェクト
            private Sequence sequence;

            const int TWEEN_MOVE_TIME = 2;

            public CharacterMove()
            {
                // インターフェース取得
                fieldInfo_get = GameObject.Find("FieldCore").GetComponent<FieldCore>();
                fieldInfo_set = GameObject.Find("FieldCore").GetComponent<FieldCore>();

                character[0] = GameObject.Find("Kaito"); // キャラクタを取得(仮オブジェクト)
                sequence = DOTween.Sequence(); // シーケンスを初期化
            }

            // デバッグ用
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
            /// Characterの移動を実行するメソッド
            /// </summary>
            public void MoveOrder(MoveSignal moveSignal,int id)
            {
                positions = moveSignal.moveDataArray; // FieldIndexを受け取る

                // playerPosition = fieldInfo_get.GetPlayerPosition(id); // 対応キャラクターの情報をグリッド上の座標に変換する
                playerPosition = new FieldIndex(2, 3);

                for (int i = 0; i < positions.Length; i++)
                {
                    int hight = playerPosition.row + positions[i].rowOffset; // 縦方向の移動量をセット
                    int width = playerPosition.column + positions[i].columnOffset; // 横方向の移動量をセット

                    playerPosition = new FieldIndex(width, hight); // キャラクターのグリッド上の座標を新規の位置に保存

                    // GetTilePosition 未設定のためコメントアウト　実装時解除すること
                    // targetPos = fieldInfo_get.GetTilePosition(playerPosition); // 目標地点を移動量を加味したグリッド上の座標をVector3に変換

                    targetPos = new Vector3(width,hight,0);

                    // GetDamegeFieldOwner 未実装のためコメントアウト　実装時解除すること
                    // TileChecker(); // タイル情報のチェックを行う

                    // 移動量を確認し、通常移動かテレポート移動かを確認する
                    if (IsDistanceCheck(positions[i].rowOffset) && IsDistanceCheck(positions[i].columnOffset)) Move();
                    else Teleport();

                    // SetPlayerPosition 未実装のためコメントアウト　実装時解除すること
                    // fieldInfo_set.SetPlayerPosition(id,playerPosition);
                }

            }

            public void Move()
            {
                Debug.Log("MoveCheck");
                sequence.Append(character[0].transform.DOMove(targetPos, TWEEN_MOVE_TIME).SetEase(Ease.Linear)); // シーケンス追加
            }

            public void Teleport()
            {
                character[0].transform.position = targetPos;
                #region 未実装コルーチン
                //bool check_move = true; // 移動確認変数
                //while (check_move)
                //{
                //    // 目標地点に到着したとき、移動確認変数をfalseに変更　到着していない場合、テレポートを実行
                //    if (targetPos == character.transform.position) check_move = false;
                //    else character.transform.localPosition = targetPos;

                //    yield return null;
                //}
                #endregion
            }

            // GetDamegeFieldOwner 未実装のためコメントアウト　実装時解除すること
            //void TileChecker()
            //{
            //    if (fieldInfo_get.GetDamageFieldOwner(playerPosition) == id) return;
            //    else if (fieldInfo_get.GetDamageFieldOwner(playerPosition) != -1) Debug.Log("ふみました");
            //    else return;
            //}

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
}