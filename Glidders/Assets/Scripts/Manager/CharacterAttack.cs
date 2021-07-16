using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Glidders.Field;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const int PLAYER_AMOUNT = 4; // playerの総数
            const int YIELD_TIME = 1;

            public List<CharacterData> sampleSignals; // 受け取った配列をリストとして扱うためのリスト
            public int[] addPoint = new int[PLAYER_AMOUNT]; // 追加するポイント量

            private Animator[] animators = new Animator[Rule.maxPlayerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private int count = 0;
            private int defalutNumber = 0;
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap)
            {
                displayTile = displayTileMap;
                fieldCore = core;
                this.animators = animators; // GetComponent済みのアニメーター配列をそのまま入れる
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas, Action phaseCompleteAction)
            {
                sampleSignals = new List<CharacterData>(); // リスト内部初期化
                count = 0;

                // 追加ポイント量初期化
                for (int i = 0;i < addPoint.Length;i++)
                {
                    addPoint[i] = 0;
                }

                // リストに受け取った配列を格納
                for (int i = 0; i < characterDatas.Length;i++)
                {
                    sampleSignals.Add(characterDatas[i]);
                }
                var signalList = sampleSignals.OrderByDescending(x => x.attackSignal.skillData.priority); // 攻撃順にリストを入れ替える  

                foreach (var x in signalList)
                {
                    Debug.Log($"{x.playerName}の{x.thisObject.name}の{x.attackSignal.skillData.skillName}は{x.attackSignal.skillData.damage}のダメージ値を持っています");

                    if (!x.canAct) continue; // 自身が攻撃できない状況にある場合、処理をスキップする
                    if (!x.attackSignal.isAttack) continue; // 攻撃をしないという情報が入っているとき、処理をスキップする

                    // 攻撃マス数分処理を回す
                    for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                    {
                        for (int j = 0; j < Rule.maxPlayerCount;j++)
                        {
                            if (sampleSignals[j].thisObject == x.thisObject) defalutNumber = j;
                        }

                        FieldIndex attackPosition = x.attackSignal.selectedGrid + x.attackSignal.skillData.attackFieldIndexOffsetArray[i]; // 攻撃指定位置に、攻撃範囲を足した量を攻撃位置として保存

                        if (attackPosition.row > 0 && attackPosition.row < 8 && attackPosition.column > 0 && attackPosition.column < 8)
                        {
                            fieldCore.SetDamageField(defalutNumber, sampleSignals[defalutNumber].attackSignal.skillData.power, attackPosition);
                            displayTile.DisplayDamageFieldTilemap(attackPosition,defalutNumber);
                        }
                        AttackDamage(x, attackPosition); // 攻撃のダメージを発生する関数

                        Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                    }

                    CameraPositionSeter();

                    AnimationPlaying(x.thisObject);

                    yield return new WaitForSeconds(YIELD_TIME); // 指定秒数停止

                    count++;
                }

                // 持っているポイントを各キャラに追加
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                }

                phaseCompleteAction();

                Debug.Log("処理終了");
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
                        for (int j = 0; j < sampleSignals.Count;j++) // 自分のキャラデータを取得するため再度for文
                        {
                            // 自分のキャラデータだった場合、追加ポイントを増やす
                            if (sampleSignals[j].thisObject == character.thisObject) addPoint[j] += sampleSignals[j].attackSignal.skillData.damage;
                        }
                        animators[i].SetTrigger("Damage");

                        Debug.Log($"{character.thisObject.name}の{character.attackSignal.skillData.name}は{sampleSignals[i].thisObject.name}にヒットし、{character.attackSignal.skillData.damage}のポイントを得た");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            private void CameraPositionSeter()
            {
                // Debug.Log("カメラ調整関数正常動作");
            }

            private void AnimationPlaying(GameObject animationObject)
            {
                // Debug.Log("アニメーション再生関数正常動作");
                for (int i = 0; i < Rule.maxPlayerCount; i++)
                {
                    if (sampleSignals[i].thisObject == animationObject)
                    {
                        animators[i].SetTrigger("Act3");
                    }
                }
            }
        }

    }
}
