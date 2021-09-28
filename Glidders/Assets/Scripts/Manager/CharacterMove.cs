using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Field;
using Glidders.Graphic;
using DG.Tweening;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        struct CollsionDataBox
        {
            public bool collsionList;
            public FieldIndex defalutIndex;
            public FieldIndex collisionIndex;
            public FieldIndexOffset collisionDirection;
        }

        public delegate void tweenList();
        public class CharacterMove
        {
            // 定数
            const int DAMAGEFIELD_DAMAGE = 2000; // ダメージフィールドを踏んだ際に発生するダメージ量
            const float TWEEN_MOVETIME = 0.3f; // Dotweenによる挙動にかける時間

            private Vector3[] targetPosition = new Vector3[ActiveRule.playerCount]; // 目標地点を保存する変数
            private FieldIndexOffset[] thisMoveOffset = new FieldIndexOffset[ActiveRule.playerCount]; // オブジェクトの移動量
            private IGetFieldInformation getFieldInformation; // FieldCoreのインターフェース
            private CharacterDirection[] characterDirections; // 各キャラクタの向き変更クラス

            private bool[] moveList = new bool[ActiveRule.playerCount]; // 動けるかどうかをCharacterごとに管理する
            private Text[] texts = new Text[ActiveRule.playerCount];
            private Animator[] animators;
            private CollsionDataBox[] collisionList = new CollsionDataBox[ActiveRule.playerCount];

            public CharacterMove(IGetFieldInformation getInfo,CharacterDirection[] directions,Text[] texts,Animator[] animators)
            {
                // コンストラクタでGetComoponentしてあるオブジェクトを取得
                characterDirections = directions;
                getFieldInformation = getInfo;
                this.texts = texts;
                this.animators = animators;

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

                for (int i = 0; i < Rule.maxMoveAmount;i++)
                {
                    for (int j =0;j < characterDatas.Length;j++)
                    {
                        collisionList[j].defalutIndex = characterDatas[j].index; // キャラのデフォルトの位置を設定
                        collisionList[j].collsionList = true;

                        moveList[j] = false;　// 動けるかどうか　を　false　にする

                        thisMoveOffset[j] = characterDatas[j].moveSignal.moveDataArray[i]; // この移動に使うFieldIndexOffsetを保存する

                        // Debug.Log($"現在位置({characterDatas[j].index.row} , {characterDatas[j].index.column})  移動量({thisMoveOffset.rowOffset.ToString()},{thisMoveOffset.columnOffset.ToString()})");

                        characterDatas[j].index += thisMoveOffset[j]; // インデックスの位置を書換える

                        // Debug.Log($"{characterDatas[j].thisObject.name} の FieldIndexは{characterDatas[j].index.row} , {characterDatas[j].index.column}");

                        // フィールド情報を判定し、移動先が進行不能エリアである場合、移動をスキップする
                        if (!getFieldInformation.IsPassingGrid(characterDatas[j].index))
                        {
                            // Debug.Log($"{characterDatas[j].playerName}はindexが{characterDatas[j].index.row},{characterDatas[j].index.column}のため進行を停止しました");
                            characterDatas[j].index -= thisMoveOffset[j]; // インデックスに対して行った変更を元に戻す
                            Stay(j); // 今回の移動はしないことを命令
                            continue;
                        }

                        targetPosition[j] = getFieldInformation.GetTilePosition(characterDatas[j].index); // インデックス座標をVector3に書き換える

                        // Debug.Log($"targetPositionは({targetPosition.x},{targetPosition.y} Indexは({characterDatas[j].index.row},{characterDatas[j].index.column})");
                    }


                    // 衝突しているかどうかを判定する関数
                    CollisionObject();

                    for (int j = 0; j < characterDatas.Length; j++)
                    {
                        characterDirections[j].SetDirection(thisMoveOffset[j]); // 向き変換の指令を出す

                        // 移動座標を元にその移動関数を呼び出す
                        if (!collisionList[j].collsionList) CollsionMove(characterDatas[j].thisObject,j,collisionList[j]);
                        else if (TeleportChecker(thisMoveOffset[j])) Teleport(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.up) MoveUp(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.down) MoveDown(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.left) MoveLeft(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset[j] == FieldIndexOffset.right) MoveRight(characterDatas[j].thisObject, j);
                        else Stay(j);
                    }

                    GlidChecker();
                    TextMove();

                    // Tweenにかける時間　もしくは　Tweenが動き終わったらコルーチンを停止する
                    while (!ListChecker(moveList)) 
                    {
                        yield return new WaitForSeconds(TWEEN_MOVETIME);
                    }

                    for (int j = 0;j < texts.Length;j++)
                    {
                        texts[j].text = "";
                    }

                }

                phaseCompleteAction();

                void TextMove()
                {
                    for (int i = 0;i < texts.Length;i++)
                    {
                        texts[i].rectTransform.localScale = new Vector3(-1, 1, 1);
                    }
                }

                bool ListChecker(bool[] moveList)
                {
                    for (int i = 0;i < moveList.Length;i++)
                    {
                        if (!moveList[i]) return false;
                    }

                    return true;
                }

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

                void CollsionMove(GameObject thisObject, int j,CollsionDataBox collsionData)
                {
                    Vector3 defaultPos = getFieldInformation.GetTilePosition(collsionData.defalutIndex);
                    Vector3 collisionPos = getFieldInformation.GetTilePosition(collsionData.collisionIndex);

                    targetPosition[j] = (defaultPos + collisionPos) / 2;

                    Sequence sequence = DOTween.Sequence().
                    Append(thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME / 2).SetEase(Ease.Linear)).
                    AppendCallback(() => characterDirections[j].SetDirection(collisionList[j].collisionDirection)).
                    AppendCallback(() => animators[j].SetTrigger("Damage")).
                    Append(thisObject.transform.DOMove(defaultPos, TWEEN_MOVETIME / 2).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true));
                }

                #region 各移動に関連する関数
                void MoveUp(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveDown(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveLeft(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void MoveRight(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(targetPosition[j], TWEEN_MOVETIME).SetEase(Ease.Linear).OnComplete(() => moveList[j] = true);
                }

                void Teleport(GameObject thisObject,int j)
                {
                    // 座標を直接書換え、移動変数をtrueにする
                    thisObject.transform.position = targetPosition[j];
                    moveList[j] = true;
                }
                #endregion

                // 衝突しているかを判定する関数
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

                                collisionList[j].collsionList = false;
                                collisionList[I].collsionList = false;
                                collisionList[j].collisionIndex = characterDatas[j].index;
                                collisionList[I].collisionIndex = characterDatas[I].index;
                                collisionList[j].collisionDirection = thisMoveOffset[j];
                                collisionList[I].collisionDirection = thisMoveOffset[I];

                                characterDatas[j].index = collisionList[j].defalutIndex;
                                characterDatas[I].index = collisionList[I].defalutIndex;


                                // 衝突しているならば、対象の二つのオブジェクトに対して、移動量を全て0に書き換えたうえで行動不能にする
                                for (int J = 0; J < Rule.maxMoveAmount; J++)
                                {
                                    characterDatas[j].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                                for (int J = 0; J < Rule.maxMoveAmount; J++)
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

                // そのグリッドのフィールド状況を判定する関数(現在はダメージフィールドのみ)
                void GlidChecker()
                {
                    for (int i = 0;i < characterDatas.Length;i++)
                    {
                        int owner = getFieldInformation.GetDamageFieldOwner(characterDatas[i].index); // i番目のキャラのindexを取得し、その場所のダメージフィールドの主を取得
                        if (i != owner && owner >= 0) // 主が自分ではなく、かつそこにダメージフィールドがある場合にダメージを受ける処理を追加
                        {
                            // Debug.Log($"index({characterDatas[i].index.row},{characterDatas[i].index.column})のオーナーは{owner}");
                            characterDatas[i].point -= DAMAGEFIELD_DAMAGE;
                            characterDatas[owner].point += DAMAGEFIELD_DAMAGE;
                            texts[i].text = DAMAGEFIELD_DAMAGE.ToString();
                            // Debug.Log($"{characterDatas[i].playerName}は{characterDatas[owner].playerName}のダメージフィールドを踏んでしまった");
                        }
                    }
                }
                #endregion
                // 移動先マス目状況の判定
            }

        }
    }
}