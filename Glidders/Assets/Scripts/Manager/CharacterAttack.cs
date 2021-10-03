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
            const float RETURNTIME_STATE = 0.25f;
            const float RETURNTIME_END = 0.5f;

            private int defalutNumber = 0; // Linqによって入れ替わった要素番号を、元の番号を検知し保存する変数
            private int targetObjectLength; // 関数TargetSettingに使われる変数　変更前のsetTargetObjectの総量を保存する変数
            private float damage; // 総ダメージ量を保管する変数

            public List<CharacterData> sampleSignals; // 受け取った配列をリストとして扱うためのリスト
            public int[] addPoint = new int[ActiveRule.playerCount]; // 追加するポイント量

            // 各クラス
            private Animator[] animators = new Animator[ActiveRule.playerCount];
            private Text[] texts = new Text[ActiveRule.playerCount];

            private FieldCore fieldCore;
            private DisplayTileMap displayTile;
            private CameraController cameraController;
            private FieldIndexOffset index;
            private FieldIndex attackPosition;
            DisplaySkillCutIn skillCutIn;

            private List<GameObject> setTargetObject = new List<GameObject>();
            private CharacterDirection[] characterDirections;

            private List<Vector3> textStatus = new List<Vector3>();
            private List<Vector2> buffStatus = new List<Vector2>();
            public CharacterAttack(Animator[] animators,FieldCore core,DisplayTileMap displayTileMap,CharacterDirection[] directions, CameraController cameraController,Text[] texts,DisplaySkillCutIn skillCutIn)
            {
                // GetComponent済みの各クラスをそのまま入れる
                displayTile = displayTileMap;
                characterDirections = directions;
                fieldCore = core;
                this.cameraController = cameraController;
                this.animators = animators;
                this.texts = texts;
                this.skillCutIn = skillCutIn;
            }

            public IEnumerator AttackOrder(CharacterData[] characterDatas,Action phaseCompleteAction,bool onlineData)
            {
                if (onlineData)
                {
                    SignalConverter signal = new SignalConverter();

                    for (int i = 0; i < characterDatas.Length; i++)
                    {
                        characterDatas[i].attackSignal = signal.GetAttackSignalData(characterDatas[i].attackSignalNumber, characterDatas[i].playerNumber);
                        characterDatas[i].direcionSignal = signal.GetDirectionSignalData(characterDatas[i].directionSignalNumber, characterDatas[i].playerNumber);
                    }
                }

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
                    if (characterDatas[i].canAct && characterDatas[i].attackSignal.isAttack) characterDatas[i].energy -= characterDatas[i].attackSignal.skillData.energy;
                    sampleSignals.Add(characterDatas[i]);
                }

                // sampleSignals.RemoveAll(x => x.attackSignal.skillData == null);
                sampleSignals.OrderBy(x => x.attackSignal.skillData.priority); // 攻撃順にリストを入れ替える  


                foreach (var x in sampleSignals)
                {
                    // Debug.Log($"{x.playerName}の{x.thisObject.name}の{x.attackSignal.skillData.skillName}は{x.attackSignal.skillData.damage}のダメージ値を持っています");

                    if (!x.canAct) continue; // 自身が攻撃できない状況にある場合、処理をスキップする
                    if (!x.attackSignal.isAttack) continue; // 攻撃をしないという情報が入っているとき、処理をスキップする

                    // ここの位置に攻撃の種類で条件分岐(攻撃、移動、バフ)
                    // 攻撃は従来の処理　移動はローカル関数 SkillMove バフは関数 BuffSeter という分岐を作る
                    if (x.attackSignal.skillData.skillType == SkillTypeEnum.SUPPORT)
                    {
                        AnimationPlaying(x.thisObject);

                        setTargetObject.Add(x.thisObject);
                        skillCutIn.StartSkillCutIn((int)x.characterName, x.attackSignal.skillData.skillName);
                    }
                    else
                    {
                        AnimationPlaying(x.thisObject); // アニメーションの再生を行う関数を呼び出す

                        cameraController.ClearTarget(); // 全てのカメラ追従対象を消去する
                        setTargetObject.Clear(); // カメラ追従対象に設定されているオブジェクトを初期化する

                        // 攻撃マス数分処理を回す
                        for (int i = 0; i < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length; i++)
                        {
                            for (int j = 0; j < ActiveRule.playerCount; j++)
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
                            skillCutIn.StartSkillCutIn((int)x.characterName,x.attackSignal.skillData.skillName);
                            AttackDamage(x, attackPosition); // 攻撃のダメージを発生する関数


                            // 攻撃の処理が終わったときに対象がまだ設定されていないなら自身のみを設定
                            if (i == x.attackSignal.skillData.attackFieldIndexOffsetArray.Length - 1 && setTargetObject.Count == 0) setTargetObject.Add(x.thisObject);
                            // Debug.Log($"attackPosition.index({i}) = ({attackPosition.row},{attackPosition.column})");
                        }
                    }

                    CameraPositionSeter(setTargetObject); // カメラ調整関数
                    // yield return new WaitForSeconds(YIELD_TIME); // 指定秒数停止
                    yield return new WaitForSeconds(x.attackSignal.skillData.skillAnimation.length + RETURNTIME_STATE); // スキルデータについているクリップの再生時間分処理停止

                    if (x.attackSignal.skillData.loseBuff != null) // もしスキルを打った際に消す処理があるなら
                    {
                        for (int j = 0; j < x.attackSignal.skillData.loseBuff.Count; j++) // 消すバフの数だけ処理を回す
                        {
                            for (int I = 0; I < x.buffView.Count; I++) // 自身が持っているバフの数だけ処理を回す
                            {
                                if (x.buffView[I] == x.attackSignal.skillData.loseBuff[j])
                                {
                                    for (int J = 0; J < x.buffValue[I].Count; J++) // バフ内容分だけ回し、ターンと内容を消す
                                    {
                                        x.buffValue[I].RemoveAt(J);
                                        x.buffTurn[I].RemoveAt(J);
                                    }

                                    if (x.buffEffectObject[I] != null) UnityEngine.Object.Destroy(x.buffEffectObject[I]);
                                    x.buffEffectObject.RemoveAt(I);

                                    // ※消すバフの内容が形態変化を伴うバフであった場合、形態をもとに戻す
                                    if (x.buffView[I].lowerTransform != null)
                                    {
                                        // キャラクターのScriptableObjectとAnimatorを設定する
                                        x.thisObject.GetComponent<CharacterCore>().characterScriptableObject = x.buffView[I].lowerTransform;
                                        x.thisObject.GetComponent<Animator>().runtimeAnimatorController = x.buffView[I].lowerTransform.characterAnimator;
                                    }

                                    // 全てのバフ関連を消す
                                    x.buffView.RemoveAt(I);
                                    x.buffValue.RemoveAt(I);
                                    x.buffTurn.RemoveAt(I);
                                }
                            }
                        }
                    }

                    if (x.attackSignal.skillData.skillType == SkillTypeEnum.SUPPORT)
                    {
                        BuffChecker(x);
                    }

                    for (int i = 0;i < textStatus.Count;i++)
                    {
                        TextMove((int)textStatus[i].x, (int)textStatus[i].y, textStatus[i].z);
                        characterDatas[(int)textStatus[i].x].totalDamage += (int)textStatus[i].y;
                    }


                    for (int i = 0;i < buffStatus.Count;i++)
                    {
                        BuffSeter((int)buffStatus[i].x, (int)buffStatus[i].y);
                    }

                    yield return new WaitForSeconds(RETURNTIME_END);

                    textStatus = new List<Vector3>();
                    buffStatus = new List<Vector2>();
                    TextReseter(ref texts);
                }

                setTargetObject = new List<GameObject>();
                // 持っているポイントを各キャラに追加 最終向き情報を反映
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    setTargetObject.Add(characterDatas[i].thisObject);
                    characterDatas[i].point += addPoint[i];
                    characterDirections[i].SetDirection(characterDatas[i].direcionSignal.direction);
                }
                CameraPositionSeter(setTargetObject);

                phaseCompleteAction(); // 処理完了を通知

                // Debug.Log("処理終了");

                #region ローカル関数

                void AnimationPlaying(GameObject animationObject)
                {
                    // Debug.Log("アニメーション再生関数正常動作");
                    for (int i = 0; i < ActiveRule.playerCount; i++)
                    {
                        if (sampleSignals[i].thisObject == animationObject)
                        {
                            characterDirections[i].SetDirection(characterDatas[i].attackSignal.direction);
                            animators[i].SetTrigger("Act" + sampleSignals[i].attackSignal.skillNumber);
                        }
                    }
                }

                void BuffSeter(int characterNumber,int i)
                {
                    for (int j = 0;j < characterDatas[characterNumber].attackSignal.skillData.giveBuff.Count;j++)
                    {
                        characterDatas[characterNumber].buffView.Add(characterDatas[characterNumber].attackSignal.skillData.giveBuff[j]); // バフ情報を追加
                        characterDatas[characterNumber].buffTurn.Add(new List<int>()); // バフ経過ターンのListを作成
                        if (characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].effectObjectPrefab != null) // もし対応するバフにオブジェクトがあるなら
                        {
                            characterDatas[characterNumber].buffEffectObject.Add(UnityEngine.Object.Instantiate(characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].effectObjectPrefab, characterDatas[characterNumber].thisObject.transform));
                        }
                        else characterDatas[characterNumber].buffEffectObject.Add(null);

                        // ※形態変化するバフであった場合
                        if (characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].upperTransform != null)
                        {
                            // キャラクターのScriptableObjectとAnimatorを設定する
                            characterDatas[characterNumber].thisObject.GetComponent<CharacterCore>().characterScriptableObject = characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].upperTransform;
                            characterDatas[characterNumber].thisObject.GetComponent<Animator>().runtimeAnimatorController = characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].upperTransform.characterAnimator;
                        }

                        List<BuffValueData> sampleData = new List<BuffValueData>(characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].buffValueList);
                        characterDatas[characterNumber].buffValue.Add(sampleData); // 作っておいたListにバフ内容を記述

                        // バフ情報に入っているバフ内容分だけ処理を回す
                        for (int I = 0; I < characterDatas[characterNumber].attackSignal.skillData.giveBuff[j].buffValueList.Count; I++)
                        {
                            characterDatas[characterNumber].buffTurn[i].Add(0);
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

                                textStatus.Add(new Vector3(i, (int)Mathf.Round(damage), sampleSignals[i].thisObject.transform.localScale.x));

                                // 最終ダメージの加減算を攻撃側、守備側に反映する
                                addPoint[i] -= (int)Mathf.Round(damage);
                                if (ActiveRule.gameRule == 0) addPoint[j] += (int)Mathf.Round(damage);
                                
                                TargetSeting(sampleSignals[i].thisObject, sampleSignals[j].thisObject); // カメラの追従対象を設定する関数を呼ぶ
                            }
                        }

                        // Debug.Log($"{character.thisObject.name}の{character.attackSignal.skillData.name}は{sampleSignals[i].thisObject.name}にヒットし、{damage}のポイントを得た");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }

            /// <summary>
            /// スキルによるバフをする関数
            /// </summary>
            private void BuffChecker(CharacterData characterData)
            {
                bool returnFlg = false;

                int count = characterData.buffView.Count; // 増加処理を行う前のバフ個数を保存

                for (int i = 0; i < characterData.buffView.Count; i++)
                {
                    for (int j = 0; j < characterData.attackSignal.skillData.giveBuff.Count; j++)
                    {
                        if (characterData.buffView[i] == characterData.attackSignal.skillData.giveBuff[j])
                        {
                            for (int I = 0; I < characterData.buffValue[i].Count; I++)
                            {
                                for (int J = 0; J < characterData.attackSignal.skillData.giveBuff[j].buffValueList.Count; J++)
                                {
                                    int plusTurn = characterData.attackSignal.skillData.giveBuff[j].buffValueList[J].buffDuration - 1;
                                    characterData.buffTurn[i][I] += plusTurn;
                                }
                            }
                            returnFlg = true;
                            break;
                        }
                    }
                }

                if (returnFlg) return;

                // 追加するバフの数だけ処理を回す
                for (int i = count; i < characterData.attackSignal.skillData.giveBuff.Count + count;++i)
                {
                    buffStatus.Add(new Vector2(characterData.playerNumber, i));

                    #region 念のため保存　旧式
                    //characterData.buffView.Add(characterData.attackSignal.skillData.giveBuff[i]); // バフ情報を追加
                    //characterData.buffTurn.Add(new List<int>()); // バフ経過ターンのListを作成
                    //if (characterData.attackSignal.skillData.giveBuff[i].effectObjectPrefab != null) // もし対応するバフにオブジェクトがあるなら
                    //{
                    //    characterData.buffEffectObject.Add(UnityEngine.Object.Instantiate(characterData.attackSignal.skillData.giveBuff[i].effectObjectPrefab, characterData.thisObject.transform));
                    //}
                    //else characterData.buffEffectObject.Add(null);

                    //// ※形態変化するバフであった場合
                    //if (characterData.attackSignal.skillData.giveBuff[i].upperTransform != null)
                    //{
                    //    // キャラクターのScriptableObjectとAnimatorを設定する
                    //    characterData.thisObject.GetComponent<CharacterCore>().characterScriptableObject = characterData.attackSignal.skillData.giveBuff[i].upperTransform;
                    //    characterData.thisObject.GetComponent<Animator>().runtimeAnimatorController = characterData.attackSignal.skillData.giveBuff[i].upperTransform.characterAnimator;
                    //}

                    //List<BuffValueData> sampleData = new List<BuffValueData>(characterData.attackSignal.skillData.giveBuff[i].buffValueList);
                    //characterData.buffValue.Add(sampleData); // 作っておいたListにバフ内容を記述

                    //// バフ情報に入っているバフ内容分だけ処理を回す
                    //for (int j = 0; j < characterData.attackSignal.skillData.giveBuff[i].buffValueList.Count; j++)
                    //{
                    //    characterData.buffTurn[i].Add(0);
                    //}
                    #endregion
                }
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


            private void TextMove(int targetNumber,int damagePoint,float x_localScale)
            {
                texts[targetNumber].text = damagePoint.ToString();
                texts[targetNumber].rectTransform.localScale = new Vector3(x_localScale,1,1);
                animators[targetNumber].SetTrigger("Damage");
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
