using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Glidders.Field;
using Glidders.Graphic;
using DG.Tweening;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        public delegate void tweenList();
        public class CharacterMove
        {
            // 定数
            const int PLAYER_AMOUNT = 4; // プレイヤーの総数
            const int PLAYER_MOVEAMOUNT_MAX = 5; // 各種キャラクターたちの移動回数
            const float TWEEN_MOVETIME = 0.5f; // Dotweenによる挙動にかける時間

            private static Vector3 targetPosition; // 目標地点を保存する変数
            private FieldIndexOffset thisMoveOffset; // オブジェクトの移動量
            private IGetFieldInformation getFieldInformation; // FieldCoreのインターフェース
            private ISetFieldInformation setFieldInformation; // FieldCoreのインターフェース
            private CharacterDirection[] characterDirections; // 各キャラクタの向き変更クラス

            private bool[] moveList = new bool[PLAYER_AMOUNT]; // 動けるかどうかをCharacterごとに管理する

            public CharacterMove(IGetFieldInformation getInfo,ISetFieldInformation setInfo,CharacterDirection[] directions)
            {
                // コンストラクタでGetComoponentしてあるオブジェクトを取得
                characterDirections = directions;
                getFieldInformation = getInfo; 
                setFieldInformation = setInfo; 

                for (int i = 0;i < moveList.Length;i++)
                {
                    // 動けるかどうかの変数を全てtrueにする
                    moveList[i] = true;
                }
            }

            [PunRPC]
            public IEnumerator MoveOrder(CharacterData[] characterDatas, Action phaseCompleteAction)
            {
                // 各プレイヤーの移動情報をもとに、フェーズごとの移動を実行

                for (int i = 0; i < PLAYER_MOVEAMOUNT_MAX;i++)
                {
                    for (int j =0;j < characterDatas.Length;j++)
                    {
                        moveList[j] = false;　// 動けるかどうか　を　false　にする

                        thisMoveOffset = characterDatas[j].moveSignal.moveDataArray[i]; // この移動に使うFieldIndexOffsetを保存する

                        // Debug.Log($"現在位置({characterDatas[j].index.row} , {characterDatas[j].index.column})  移動量({thisMoveOffset.rowOffset.ToString()},{thisMoveOffset.columnOffset.ToString()})");

                        characterDatas[j].index += thisMoveOffset; // インデックスの位置を書換える

                        Debug.Log($"{characterDatas[j].thisObject.name} の FieldIndexは{characterDatas[j].index.row} , {characterDatas[j].index.column}");

                        // フィールド情報を判定し、移動先が進行不能エリアである場合、移動をスキップする
                        if (!getFieldInformation.IsPassingGrid(characterDatas[j].index))
                        {
                            Debug.Log($"{characterDatas[j].playerName}はindexが{characterDatas[j].index.row},{characterDatas[j].index.column}のため進行を停止しました");
                            characterDatas[j].index -= thisMoveOffset; // インデックスに対して行った変更を元に戻す
                            Stay(j); // 今回の移動はしないことを命令
                            continue;
                        }

                        targetPosition = getFieldInformation.GetTilePosition(characterDatas[j].index); // インデックス座標をVector3に書き換える

                        characterDirections[j].SetDirection(thisMoveOffset); // 向き変換の指令を出す

                        // Debug.Log($"targetPositionは({targetPosition.x},{targetPosition.y} Indexは({characterDatas[j].index.row},{characterDatas[j].index.column})");

                        // 移動座標を元にその移動関数を呼び出す
                        if (TeleportChecker(thisMoveOffset)) Teleport(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.up) MoveUp(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.down) MoveDown(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.left) MoveLeft(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.right) MoveRight(characterDatas[j].thisObject, j);
                        else Stay(j);
                    }

                    // Tweenにかける時間　もしくは　Tweenが動き終わったらコルーチンを停止する
                    while (!moveList[0] || !moveList[1] || !moveList[2] || !moveList[3])
                    {
                        yield return new WaitForSeconds(TWEEN_MOVETIME);
                    }

                    // 衝突しているかどうかを判定する関数
                    CollisionObject();
                }

                for (int i = 0; i < characterDatas.Length; i++)
                {
                    GlidChecker();
                    // Debug.Log($"最終移動地点({characterDatas[i].index.row}{characterDatas[i].index.column})");
                    // Fieldに対してインデックスを返す
                    // setFieldInformation.SetPlayerPosition(i, characterDatas[i].index); // 最終座標をFieldに返却する
                }
                
                phaseCompleteAction();

                #region ローカル関数
                bool TeleportChecker(FieldIndexOffset index)
                {
                    if (index.rowOffset > 1 || index.rowOffset < -1 || index.columnOffset > 1 ||index.columnOffset < -1) return true; // 移動量が2以上であるならばテレポート移動に切り替える
                    return false;
                }

                void Stay(int j)
                {
                    // 移動関連を行わず、移動変数をtrueにする
                    moveList[j] = true;
                }

                void MoveUp(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveDown(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveLeft(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveRight(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition, TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void Teleport(GameObject thisObject,int j)
                {
                    // 座標を直接書換え、移動変数をtrueにする
                    thisObject.transform.position = targetPosition;
                    moveList[j] = true;
                }

                void CollisionObject()
                {
                    for (int j = 0; j < characterDatas.Length; j++)
                    {
                        if (!characterDatas[j].canAct) continue; // キャラクタが行動不能であるならば処理をスキップ

                        for (int I = 0; I < characterDatas.Length; I++)
                        {
                            if (I == j) continue; // 参照するオブジェクトがかぶるため処理をスキップ

                            // Debug.Log("j = " + characterDatas[j].index.column.ToString() + characterDatas[j].index.row.ToString() + "; I = " + characterDatas[I].index.column.ToString()  + characterDatas[I].index.row.ToString());
                            if (characterDatas[j].index == characterDatas[I].index)
                            {
                                // Debug.Log(characterDatas[j].thisObject.name + "と" + characterDatas[I].thisObject.name + "はぶつかった");

                                // 衝突しているならば、対象の二つのオブジェクトに対して、移動量を全て0に書き換えたうえで行動不能にする
                                for (int J = 0; J < PLAYER_MOVEAMOUNT_MAX; J++)
                                {
                                    characterDatas[j].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                                for (int J = 0; J < PLAYER_MOVEAMOUNT_MAX; J++)
                                {
                                    characterDatas[I].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                                characterDatas[j].canAct = false;
                                characterDatas[I].canAct = false;
                            }
                        }
                    }
                }

                // phaseCompleteAction();

                void GlidChecker()
                {
                    // フィールド情報を判定する関数です
                }
                #endregion
                // 移動先マス目状況の判定
            }

        }
    }
}