using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Field;
using DG.Tweening;
using System;

namespace Glidders
{
    namespace Manager
    {
        public delegate void tweenList();
        public class CharacterMove
        {
            public event tweenList MoveEvent = delegate () { }; 

            const int PLAYER_MOVEAMOUNT_MAX = 2;
            const int TWEEN_MOVETIME = 1;

            private FieldIndexOffset thisMoveOffset;

            private bool[] moveList = new bool[4];

            public CharacterMove()
            {
                for (int i = 0;i < moveList.Length;i++)
                {
                    moveList[i] = true;
                }
            }

            public IEnumerator MoveOrder(CharacterData[] characterDatas)
            {
                // 各プレイヤーの移動情報をもとに、フェーズごとの移動を実行

                for (int i = 0; i < PLAYER_MOVEAMOUNT_MAX;i++)
                {
                    for (int j =0;j < characterDatas.Length;j++)
                    {
                        moveList[j] = false;

                        thisMoveOffset = characterDatas[j].moveSignal.moveDataArray[i];

                        characterDatas[j].index += thisMoveOffset;

                        if (TeleportChecker(thisMoveOffset)) Teleport(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.up) MoveUp(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.down) MoveDown(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.left) MoveLeft(characterDatas[j].thisObject, j);
                        else if (thisMoveOffset == FieldIndexOffset.right) MoveRight(characterDatas[j].thisObject, j);
                        else Stay(j);
                    }

                    while (!moveList[0] || !moveList[1])
                    {
                        yield return new WaitForSeconds(TWEEN_MOVETIME);
                    }

                    CollisionObject();
                }

                #region ローカル関数
                bool TeleportChecker(FieldIndexOffset index)
                {
                    return false;
                }

                void Stay(int j)
                {
                    moveList[j] = true;
                }

                void MoveUp(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(new Vector3(-5,2.5f), TWEEN_MOVETIME).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => moveList[j] = true);
                }

                void MoveDown(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(new Vector3(5,-2.5f), TWEEN_MOVETIME).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => moveList[j] = true);
                }

                void MoveLeft(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(new Vector3(-5,2.5f), TWEEN_MOVETIME).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => moveList[j] = true);
                }

                void MoveRight(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(new Vector3(5,2.5f), TWEEN_MOVETIME).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => moveList[j] = true);
                }

                void Teleport(GameObject thisObject,int j)
                {
                    thisObject.transform.DOMove(thisObject.transform.position, TWEEN_MOVETIME).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => moveList[j] = true);
                }

                void CollisionObject()
                {
                    for (int j = 0; j < characterDatas.Length; j++)
                    {
                        if (characterDatas[j].canAct) continue;

                        for (int I = 0; I < characterDatas.Length; I++)
                        {
                            if (I == j) continue;

                            if (characterDatas[j].index == characterDatas[I].index)
                            {
                                characterDatas[j].canAct = false;
                                for (int J = I + 1; J < PLAYER_MOVEAMOUNT_MAX;J++)
                                {
                                    characterDatas[j].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                                characterDatas[I].canAct = false;
                                for (int J = I + 1; J < PLAYER_MOVEAMOUNT_MAX; J++)
                                {
                                    characterDatas[I].moveSignal.moveDataArray[J] = FieldIndexOffset.zero;
                                }
                            }
                        }
                    }
                }
                #endregion
                // 移動先マス目状況の判定
            }

        }
    }
}