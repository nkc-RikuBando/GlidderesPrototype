using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Glidders.Field;
using Glidders.Graphic;
using Glidders.Buff;
using Glidders.Character;
using Photon;
using Photon.Pun;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            private int defalutNumber = 0; // Linqによって入れ替わった要素番号を、元の番号を検知し保存する変数
            private int targetObjectLength; // 関数TargetSettingに使われる変数　変更前のsetTargetObjectの総量を保存する変数
            private float damage; // 総ダメージ量を保管する変数

            public List<CharacterData> sampleSignals; // 受け取った配列をリストとして扱うためのリスト
            public int[] addPoint = new int[Rule.maxPlayerCount]; // 追加するポイント量

            // 各クラス
            private Animator[] animators = new Animator[Rule.maxPlayerCount];
            private Text[] texts = new Text[Rule.maxPlayerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private CameraController cameraController;
            private FieldIndexOffset index;
            private FieldIndex attackPosition;

            private List<GameObject> setTargetObject = new List<GameObject>();
            private CharacterDirection[] characterDirections;
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap,CharacterDirection[] directions, CameraController cameraController,Text[] texts)
            {
                // GetComponent済みの各クラスをそのまま入れる
                displayTile = displayTileMap;
                characterDirections = directions;
                fieldCore = core;
                this.cameraController = cameraController;
                this.animators = animators;
                this.texts = texts;
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas,Action phaseCompleteAction)
            {
                sampleSignals = new List<CharacterData>(); // リスト内部初期化
                cameraController.ClearTarget(); // 全ての追従対象を消去する

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

                // var sampleList = sampleSignals.RemoveAll(x => x.attackSignal);
                var signalList = sampleSignals.OrderByDescending(x => x.attackSignal.skillData.priority); // 攻撃順にリストを入れ替える  

                foreach (var x in signalList)
                {
                    // Debug.Log($"{x.playerName}の{x.thisObject.name}の{x.attackSignal.skillData.skillName}は{x.attackSignal.skillData.damage}のダメージ値を持っています");

                    if (!x.canAct) continue; // 自身が攻撃できない状況にある場合、処理をスキップする
                    if (!x.attackSignal.isAttack) continue; // 攻撃をしないという情報が入っているとき、処理をスキップする

                    // ここの位置に攻撃の種類で条件分岐(攻撃、移動、バフ)
                    // 攻撃は従来の処理　移動はローカル関数 SkillMove バフは関数 BuffSeter という分岐を作る
                    if (x.attackSignal.skillData.skillType == SkillTypeEnum.SUPPORT)
                    {
                        BuffSeter(x);
                        setTargetObject.Add(x.thisObject);
                    }
                    else
                    {
                        AnimationPlaying(x.thisObject); // アニメーションの再生を行う関数を呼び出す

                        cameraController.ClearTarget(); // 全てのカメラ追従対象を消去する
                        setTargetObject.Clear(); // カメラ追従対象に設定されているオブジェクトを初期化する

                        // 攻撃マス数分処理を回す
                        for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                        {
                            for (int j = 0; j < Rule.maxPlayerCount; j++)
                            {
                                if (sampleSignals[j].thisObject == x.thisObject) defalutNumber = j; // 入れ替わったデータが本来どこにあったのかを取得
                            }

                            #region スキルの向きに基づく結果になるようにFieldIndexを調整する処理

                            // デバッグ用処理　キャラクタ1の方向詳細
                            //if (x.playerName == "だだだだだだだだだだ!!!!!")
                            //{
                            //    Debug.Log($"attackDirection({x.attackSignal.direction.rowOffset},{x.attackSignal.direction.columnOffset})");
                            //    Debug.Log($"DirectionSignal({x.direcionSignal.direction.rowOffset},{x.direcionSignal.direction.columnOffset})");
                            //}

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

                            attackPosition = x.attackSignal.selectedGrid + index; // 攻撃指定位置に、攻撃範囲を足した量を攻撃位置として保存

                            if (attackPosition.row > 0 && attackPosition.row < 8 && attackPosition.column > 0 && attackPosition.column < 8)
                            {
                                // Debug.Log($"バフ計算前　： {sampleSignals[defalutNumber].attackSignal.skillData.power} | PlayerName {characterDatas[defalutNumber].playerName}");
                                int fieldPower = (int)BuffFieldCheck(sampleSignals[defalutNumber].attackSignal.skillData.power, sampleSignals[defalutNumber]);
                                // Debug.Log($"バフ計算後　： {fieldPower}");
                                fieldCore.SetDamageField(defalutNumber, fieldPower, attackPosition);
                                displayTile.DisplayDamageFieldTilemap(attackPosition, fieldCore.GetDamageFieldOwner(attackPosition));
                                // Debug.Log($"index({i}) = ({attackPosition.row},{attackPosition.column})はダメージフィールド生成処理として正常に作動しました");
                            }
                            AttackDamage(x, attackPosition); // 攻撃のダメージを発生する関数

                            // 攻撃の処理が終わったときに対象がまだ設定されていないなら自身のみを設定
                            if (i == x.attackSignal.skillData.attackFieldIndexOffsetArray.Length - 1 && setTargetObject.Count == 0) setTargetObject.Add(x.thisObject);
                            // Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                        }
                    }

                    CameraPositionSeter(setTargetObject); // カメラ調整関数
                    // yield return new WaitForSeconds(YIELD_TIME); // 指定秒数停止
                    yield return new WaitForSeconds(x.attackSignal.skillData.skillAnimation.length); // スキルデータについているクリップの再生時間分処理停止

                    TextReseter(ref texts);
                }

                // 持っているポイントを各キャラに追加 最終向き情報を反映
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                    characterDirections[i].SetDirection(characterDatas[i].direcionSignal.direction);
                }

                phaseCompleteAction(); // 処理完了を通知

                // Debug.Log("処理終了");

                #region ローカル関数
                void SkillMove(ref CharacterData characterData)
                {
                    // やりたいこと
                    // 1.スキルデータから移動位置を抜き出す
                    // 2.その位置へのindexの書き換え　transform上の移動も忘れずに
                    // 3.その位置が他のキャラクタとかぶっていないかを検知　被っているなら自身を行動不能にする
                    // 4.ダメージフィールドの検知　あるならばダメージ処理を行う
                    // 5.必要に応じてスキルアニメーションを再生

                    FieldIndex debugIndex = new FieldIndex(0, 0); // 適当な値　本来は設定されたindexを使うこと

                    characterData.index = debugIndex;
                    characterData.thisObject.transform.position = fieldCore.GetTilePosition(characterData.index);

                    for (int i = 0; i < Rule.maxPlayerCount; i++)
                    {
                        if (characterDatas[i].thisObject == characterData.thisObject) return;

                        if (characterDatas[i].index == characterData.index)
                        {
                            characterData.canAct = false;
                            break;
                        }
                    }
                }
                #endregion
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
                                damage = BuffDamageCheck(sampleSignals[j].attackSignal.skillData.damage, sampleSignals[i], sampleSignals[j]);// バフダメージを計算する関数を経由し、最終ダメージを出す

                                TextMove(i, (int)Mathf.Round(damage));

                                // 最終ダメージの加減算を攻撃側、守備側に反映する
                                addPoint[i] -= (int)Mathf.Round(damage);
                                addPoint[j] += (int)Mathf.Round(damage);
                                
                                TargetSeting(sampleSignals[i].thisObject, sampleSignals[j].thisObject); // カメラの追従対象を設定する関数を呼ぶ
                            }
                        }
                        animators[i].SetTrigger("Damage");

                        // Debug.Log($"{character.thisObject.name}の{character.attackSignal.skillData.name}は{sampleSignals[i].thisObject.name}にヒットし、{damage}のポイントを得た");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            /// <summary>
            /// スキルによるバフをする関数
            /// </summary>
            private void BuffSeter(CharacterData characterData)
            {
                // やりたいこと　
                // 1.スキルデータから追加するバフを抜き出す
                // 2.すでにそのバフが存在するかどうかを検知　あるなら延長　ないなら追加
                // 3.スキルアニメーションの再生を行う

                int count = characterData.buffView.Count + 1;

                for (int i = count; i < characterData.attackSignal.skillData.giveBuff.Count + count;i++)
                {
                    characterData.buffView.Add(characterData.attackSignal.skillData.giveBuff[i]);
                    characterData.buffValue.Add(new List<BuffValueData>());
                    characterData.buffTurn.Add(new List<int>());

                    for (int j = 0;j < characterData.attackSignal.skillData.giveBuff[i].buffValueList.Count;j++)
                    { 
                        characterData.buffValue[i].Add(characterData.attackSignal.skillData.giveBuff[i].buffValueList[j]);
                        characterData.buffTurn[i].Add(characterData.attackSignal.skillData.giveBuff[i].buffValueList[j].buffDuration);
                    }
                }

                //for (int i = 0; i < characterData.attackSignal.skillData.giveBuff.buffValueList.Count; i++)
                //{
                //    characterData.buffTurn.Add(characterData.attackSignal.skillData.giveBuff.buffValueList[i].buffDuration);
                //}

                // characterData.attackSignal.skillData.
                // characterData.buffTurn.Add(0);
                // characterData.buffView.Add();
                // characterData.buffValue.Add();
            }

            /// <summary>
            /// カメラの追従対象を設定する
            /// </summary>
            /// <param name="Diffence">攻撃を受けた側のオブジェクト</param>
            /// <param name="Attack">攻撃をあたえた側のオブジェクト</param>
            private void TargetSeting(GameObject Diffence,GameObject Attack)
            {
                targetObjectLength = setTargetObject.Count; // SetTargetObjectの変更前の総量
                if (targetObjectLength == 0)
                {
                    // もし総量が0であるなら、双方を追従対象にする
                    setTargetObject.Add(Diffence); 
                    setTargetObject.Add(Attack);
                }
                else
                {
                    // 総量が1を超えていて、設定されているなら追従対象がかぶっていないかを検知し、いなければ追加する
                    for (int i = 0; i < targetObjectLength; i++)
                    {
                        if (setTargetObject[i] != Diffence)
                        {
                            setTargetObject.Add(Diffence);
                        }

                        if (setTargetObject[i] != Attack)
                        {
                            setTargetObject.Add(Attack);
                        }
                    }
                }

            }

            /// <summary>
            /// ダメージフィールドのターン数のバフの加減算を計算する関数
            /// </summary>
            /// <param name="defaultPower">元威力</param>
            /// <param name="attackSideData">攻撃サイドのキャラクタデータ</param>
            /// <returns>バフを加味した威力</returns>
            private float BuffFieldCheck(int defaultPower,CharacterData attackSideData)
            {
               float totalFieldPower = defaultPower; // 最終威力を保管するローカル関数

                for (int i = 0;i < attackSideData.buffView.Count; i++) // ついているバフの数だけ処理を回す
                {
                    for (int j = 0;j < attackSideData.buffValue[i].Count;j++) // 現在乗っているバフの数だけ処理を回す
                    {
                        if (attackSideData.buffValue[i][j].buffedStatus == StatusTypeEnum.POWER) // バフの内容が威力なら処理を実行
                        {
                            switch(attackSideData.buffValue[i][j].buffType)
                            {
                                case BuffTypeEnum.PLUS:
                                    totalFieldPower += attackSideData.buffValue[i][j].buffScale;
                                    break;
                                case BuffTypeEnum.MULTIPLIED:
                                    totalFieldPower *= attackSideData.buffValue[i][j].buffScale;
                                    break;
                            }
                        }
                    }
                }

                return Mathf.Round(totalFieldPower);
            }

            /// <summary>
            /// バフによるダメージの加減算を処理する関数
            /// </summary>
            /// <param name="defaultDamage">処理したいダメージ量</param>
            /// <param name="deffenceSideData">守備サイド側のキャラクタデータ</param>
            /// <param name="attackSideData">攻撃サイド側のキャラクタデータ</param>
            /// <returns>バフを加味した最終ダメージ量</returns>
            private float BuffDamageCheck(int defaultDamage,CharacterData deffenceSideData,CharacterData attackSideData)
            {
                float totalDamage = defaultDamage; // 総ダメージ量を保管するローカル変数

                // 攻撃サイドの処理　ダメージをあたえるバフの場合のみ処理
                for (int i = 0;i < attackSideData.buffView.Count;i++) // 現在乗っているバフの量分回すfor文
                {
                    for (int j = 0;j < attackSideData.buffValue[i].Count;j++) // 現在乗っているバフ効果の総量分回すfor文
                    {
                        if (attackSideData.buffValue[i][j].buffedStatus == StatusTypeEnum.DAMAGE) // 現在処理中のバフ効果がダメージ上昇関連だった場合のみ処理
                        {
                            switch (attackSideData.buffValue[i][j].buffType)
                            {
                                case BuffTypeEnum.PLUS:
                                    totalDamage += attackSideData.buffValue[i][j].buffScale;
                                    break;
                                case BuffTypeEnum.MULTIPLIED:
                                    totalDamage *= attackSideData.buffValue[i][j].buffScale;
                                    break;
                            }
                        }
                    }
                }

                // 守備サイドの処理　ダメージを減算するバフの時のみ処理
                for (int i = 0;i < deffenceSideData.buffView.Count;i++)// 現在乗っているバフの量分回すfor文
                {
                    for (int j = 0;j < deffenceSideData.buffValue[i].Count;j++) // 現在乗っているバフ効果の総量分回すfor文
                    {
                        if (deffenceSideData.buffValue[i][j].buffedStatus == StatusTypeEnum.DEFENSE) // 現在処理中のバフ効果がダメージ減算関連だった場合のみ処理
                        {
                            switch (deffenceSideData.buffValue[i][j].buffType)
                            {
                                case BuffTypeEnum.PLUS:
                                    totalDamage -= deffenceSideData.buffValue[i][j].buffScale;
                                    break;
                                case BuffTypeEnum.MULTIPLIED:
                                    totalDamage *= deffenceSideData.buffValue[i][j].buffScale;
                                    break;
                            }
                        }
                    }
                }
                return totalDamage;
            }

            /// <summary>
            /// カメラの追従対象を反映する関数
            /// </summary>
            /// <param name="gameObjects">追従させる対象の入った配列</param>
            private void CameraPositionSeter(List<GameObject> gameObjects)
            {
                for (int i = 0;i < gameObjects.Count;i++)
                {
                    cameraController.AddTarget(gameObjects[i].transform); // カメラ追従対象に設定されているオブジェクトを順に追加
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

            private void TextMove(int targetNumber,int damagePoint)
            {
                texts[targetNumber].text = damagePoint.ToString();
                texts[targetNumber].rectTransform.localScale = new Vector3(-1,1,1);
            }

            private void TextReseter(ref Text[] texts)
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = "";
                }
            }
        }

    }
}
