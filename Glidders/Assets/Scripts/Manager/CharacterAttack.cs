using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Glidders.Field;
using Glidders.Graphic;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const int PLAYER_AMOUNT = 4; // playerの総数
            const int YIELD_TIME = 1; // 処理を停止する時間

            public List<CharacterData> sampleSignals; // 受け取った配列をリストとして扱うためのリスト
            public int[] addPoint = new int[PLAYER_AMOUNT]; // 追加するポイント量

            private Animator[] animators = new Animator[Rule.maxPlayerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private CameraController cameraController;
            private int defalutNumber = 0;

            private List<GameObject> setTargetObject;
            private CharacterDirection[] characterDirections;
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap,CharacterDirection[] directions, CameraController cameraController)
            {
                displayTile = displayTileMap;
                characterDirections = directions;
                fieldCore = core;
                this.cameraController = cameraController;
                this.animators = animators; // GetComponent済みのアニメーター配列をそのまま入れる
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas, AnimationClip clip,Action phaseCompleteAction)
            {
                sampleSignals = new List<CharacterData>(); // リスト内部初期化
                // 追加ポイント量初期化
                for (int i = 0;i < addPoint.Length;i++)
                {
                    addPoint[i] = 0;
                    cameraController.RemoveTarget(characterDatas[i].thisObject.transform);
                }

                // リストに受け取った配列を格納
                for (int i = 0; i < characterDatas.Length;i++)
                {
                    sampleSignals.Add(characterDatas[i]);
                }
                var signalList = sampleSignals.OrderByDescending(x => x.attackSignal.skillData.priority); // 攻撃順にリストを入れ替える  

                foreach (var x in signalList)
                {
                    // Debug.Log($"{x.playerName}の{x.thisObject.name}の{x.attackSignal.skillData.skillName}は{x.attackSignal.skillData.damage}のダメージ値を持っています");

                    if (!x.canAct) continue; // 自身が攻撃できない状況にある場合、処理をスキップする
                    if (!x.attackSignal.isAttack) continue; // 攻撃をしないという情報が入っているとき、処理をスキップする

                    AnimationPlaying(x.thisObject);

                    yield return new WaitForSeconds(clip.length); // 仮で受け取ったアニメーションクリップの再生時間分のコルーチンを実行

                    // 攻撃マス数分処理を回す
                    for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                    {
                        for (int j = 0; j < Rule.maxPlayerCount; j++)
                        {
                            if (sampleSignals[j].thisObject == x.thisObject) defalutNumber = j;
                        }

                        FieldIndexOffset index = FieldIndexOffset.zero;

                        #region スキルの向きに基づく結果になるようにFieldIndexを調整する処理
                        if (x.attackSignal.direction == FieldIndexOffset.left)
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // indexに現在のFieldIndexOffsetを代入
                            index = new FieldIndexOffset(index.columnOffset, index.rowOffset); // スキル方向の縦と横を入れ替える
                        }
                        else if (x.attackSignal.direction == FieldIndexOffset.right)
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // indexに現在のFieldIndexOffsetを代入
                            index = new FieldIndexOffset(index.columnOffset, index.rowOffset) * -1; // スキル方向の縦と横を入れ替えたのち、負の方向に変換
                        }
                        else if (x.attackSignal.direction == FieldIndexOffset.up)
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // indexに現在のFieldIndexOffsetを代入
                        }
                        else
                        {
                            index = x.attackSignal.skillData.attackFieldIndexOffsetArray[i] * -1; // indexに現在のFieldIndexOffsetを代入したのち、負の方向に変換
                        }
                        #endregion

                        // Debug.Log($"攻撃座標 ({x.attackSignal.selectedGrid.row},{x.attackSignal.selectedGrid.column})");
                        // Debug.Log($"index({index.rowOffset},{index.columnOffset})");

                        FieldIndex attackPosition = x.attackSignal.selectedGrid + index; // 攻撃指定位置に、攻撃範囲を足した量を攻撃位置として保存

                        if (attackPosition.row > 0 && attackPosition.row < 8 && attackPosition.column > 0 && attackPosition.column < 8)
                        {
                            fieldCore.SetDamageField(defalutNumber, sampleSignals[defalutNumber].attackSignal.skillData.power, attackPosition);
                            displayTile.DisplayDamageFieldTilemap(attackPosition, fieldCore.GetDamageFieldOwner(attackPosition));
                        }
                        AttackDamage(x, attackPosition); // 攻撃のダメージを発生する関数

                        // Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                    }

                    yield return new WaitForSeconds(YIELD_TIME); // 指定秒数停止
                }

                // 持っているポイントを各キャラに追加
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                    characterDirections[i].SetDirection(characterDatas[i].direcionSignal.direction);
                    
                }

                phaseCompleteAction(); // 処理完了を通知

                // Debug.Log("処理終了");
            }

            /// <summary>
            /// ダメージ　および　ポイントの増加を行う関数
            /// </summary>
            /// <param name="character">キャラクター情報</param>
            /// <param name="attackPosition">受け取る攻撃位置</param>
            private void AttackDamage(CharacterData character,FieldIndex attackPosition)
            {
                for (int i = 0;i < sampleSignals.Count;i++)
                {
                    if (sampleSignals[i].thisObject == character.thisObject) continue; // 自分の位置に攻撃判定があっても処理しない

                    if (sampleSignals[i].index == attackPosition) // 攻撃判定の位置と対象の位置が等しい場合
                    {
                        for (int j = 0; j < sampleSignals.Count; j++) // 自分のキャラデータを取得するため再度for文
                        {
                            // 自分のキャラデータだった場合、追加ポイントを増やす
                            if (sampleSignals[j].thisObject == character.thisObject)
                            {
                                addPoint[i] -= sampleSignals[j].attackSignal.skillData.damage;
                                addPoint[j] += sampleSignals[j].attackSignal.skillData.damage;

                                //setTargetObject[0] = sampleSignals[i].thisObject;
                                //setTargetObject[1] = sampleSignals[i].thisObject;
                                //CameraPositionSeter(setTargetObject); // カメラ調整関数
                            }
                        }
                        animators[i].SetTrigger("Damage");

                        // Debug.Log($"{character.thisObject.name}の{character.attackSignal.skillData.name}は{sampleSignals[i].thisObject.name}にヒットし、{character.attackSignal.skillData.damage}のポイントを得た");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            private void CameraPositionSeter(List<GameObject> gameObjects)
            {
                for (int i = 0;i < gameObjects.Count;i++)
                {
                    cameraController.AddTarget(gameObjects[i].transform);
                }
                // Debug.Log("カメラ調整関数正常動作");
            }

            private void AnimationPlaying(GameObject animationObject)
            {
                // Debug.Log("アニメーション再生関数正常動作");
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    if (sampleSignals[i].thisObject == animationObject)
                    {
                        animators[i].SetTrigger("Act" + sampleSignals[i].attackSignal.skillNumber);
                    }
                }
            }
        }

    }
}
