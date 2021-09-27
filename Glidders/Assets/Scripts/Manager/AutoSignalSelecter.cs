using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;
using Glidders.Field;
using System;
using System.Linq;

namespace Glidders
{
    namespace Manager
    {
        public class AutoSignalSelecter
        {
            CharacterCore character; // キャラクタの基礎情報
            CharacterData charaData; // キャラクタの可変情報
            bool cantMove;
            IGetFieldInformation fieldInformation; // フィールド情報調整
            FieldIndex index; // 最終的な到達マスを検証用に保存する
            List<FieldIndexOffset> moveIndex = new List<FieldIndexOffset>(); // 最終的な到達マスをまとめたリスト
            List<FieldIndexOffset> wayIndex = new List<FieldIndexOffset>();  // 到達マスにたどり着くまでの道のりをまとめたリスト
            List<UniqueSkillScriptableObject> skillList; // 使用可能なスキルを使用優先度順にしたリスト
            public AutoSignalSelecter(IGetFieldInformation fieldInformation)
            {
                this.fieldInformation = fieldInformation;
            }

            public CharacterData SignalSet(CharacterData signalSetCharaData,CharacterData mainTarget)
            {
                #region 旧処理
                //CharacterScriptableObject character = signalSetCharaData.thisObject.GetComponent<CharacterCore>().characterScriptableObject; // 変更点？

                //if (signalSetCharaData.moveSignal.moveDataArray[0] == FieldIndexOffset.zero)
                //{
                //    for (int i = 0; i < character.moveAmount; i++)
                //    {
                //        switch(Random.Range(0,4))
                //        {
                //            case 0:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.up;
                //                break;
                //            case 1:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.down;
                //                break;
                //            case 2:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.right;
                //                break;
                //            case 3:
                //                signalSetCharaData.moveSignal.moveDataArray[i] = FieldIndexOffset.left;
                //                break;
                //        }
                //    }
                //}

                //if (!signalSetCharaData.attackSignal.skillData)
                //{
                //    AttackSignal attackSignal;
                //    FieldIndexOffset fieldIndexOffset = new FieldIndexOffset(1, 1);

                //    int skillRandomNumber = Random.Range(0, 3);

                //    attackSignal.skillData = character.skillDataArray[skillRandomNumber]; ;
                //    attackSignal.skillNumber = skillRandomNumber;
                //    attackSignal.isAttack = true;

                //    switch (Random.Range(0, 4))
                //    {
                //        case 0:
                //            fieldIndexOffset = FieldIndexOffset.up;
                //            break;
                //        case 1:
                //            fieldIndexOffset = FieldIndexOffset.down;
                //            break;
                //        case 2:
                //            fieldIndexOffset = FieldIndexOffset.right;
                //            break;
                //        case 3:
                //            fieldIndexOffset = FieldIndexOffset.left;
                //            break;
                //    }

                //    attackSignal.direction = fieldIndexOffset;
                //    attackSignal.selectedGrid = signalSetCharaData.index + fieldIndexOffset;

                //    signalSetCharaData.attackSignal = attackSignal;
                //}

                // signalSetCharaData.attackSignal.skillData = character.skillDataArray[skillRandomNumber];
                #endregion
                moveIndex = new List<FieldIndexOffset>();

                character = signalSetCharaData.thisObject.GetComponent<CharacterCore>();
                charaData = signalSetCharaData;
                // 打てない技を除外したリストを作成
                skillList = new List<UniqueSkillScriptableObject>(character.characterScriptableObject.skillDataArray);
                skillList.Add(character.characterScriptableObject.uniqueSkillData);
                skillList.RemoveAll(x => x.energy > charaData.energy);
                skillList.OrderByDescending(x => x.energy / x.damage);

                for (int i = 0; i < skillList.Count;i++)
                {
                    Debug.Log($"skill({i}) is ({skillList[i].skillName})");
                }

                int moveAmount = character.GetMoveAmount();

                for (int row = moveAmount * -1; row <= moveAmount;row++)
                {
                    for (int column = moveAmount * -1;column <= moveAmount;column++)
                    {
                        if (Mathf.Abs(row) + Mathf.Abs(column) > moveAmount) continue;
                        moveIndex.Add(new FieldIndexOffset(row, column));
                    }
                }

                for (int i = 0;i < moveIndex.Count;i++)
                {
                    index = moveIndex[i] + charaData.index;
                    if (index.row < 0 || index.row > Rule.maxMoveAmount || index.column < 0 || index.column > Rule.maxMoveAmount) continue; // 移動先が画面外であるなら処理をスキップ
                    if (!fieldInformation.IsPassingGrid(index)) continue; // 通れる状況にないなら処理スキップ

                    if (!SkillindexCheck(skillList, mainTarget,moveIndex[i])) continue; // スキルが当たらないなら処理をスキップ

                    // Debug.Log($"スキル設定(charaData.Number({charaData.playerNumber}))");

                    wayIndex = WayIndexChecker(moveIndex[i]); // 移動の道のりを確認

                    // Debug.Log($"道のり設定(charaData.Number({charaData.playerNumber}))");

                    wayIndex = GlidChecker(wayIndex); // 移動できるかどうかを確認

                    if (cantMove) continue;

                    for (int j = wayIndex.Count; j < Rule.maxMoveAmount;j++)
                    {
                        wayIndex.Add(FieldIndexOffset.zero);
                    }

                    charaData.moveSignal.moveDataArray = wayIndex.ToArray();

                    return charaData;
                }

                return signalSetCharaData;
            }

            bool SkillindexCheck(List<UniqueSkillScriptableObject> skill,CharacterData mainTarget,FieldIndexOffset moveOffset)
            {
                FieldIndex targetIndex = mainTarget.index;
                FieldIndexOffset testIndexOffset = moveOffset;
                int fixedMoveAmount = 0;
                for (int i = 0;i < skill.Count;i++)
                {
                    targetIndex = mainTarget.index;
                    testIndexOffset = moveOffset;
                    for (int j = 0; j < mainTarget.moveSignal.moveDataArray.Length; j++)
                    {
                        // Debug.Log($"moveSignal({mainTarget.moveSignal.moveDataArray[i].rowOffset},{mainTarget.moveSignal.moveDataArray[i].columnOffset})");
                        targetIndex += mainTarget.moveSignal.moveDataArray[j];
                    }

                    if (skill[i].moveType == UniqueSkillMoveType.FIXED) fixedMoveAmount = skill[i].moveFieldIndexOffsetArray[0].rowOffset + skill[i].moveFieldIndexOffsetArray[0].columnOffset;

                    #region 向き確認
                    FieldIndexOffset indexOffset = new FieldIndexOffset();
                    for (int j = 0; j < 4; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                indexOffset = FieldIndexOffset.up;
                                break;
                            case 1:
                                indexOffset = FieldIndexOffset.down;
                                break;
                            case 2:
                                indexOffset = FieldIndexOffset.left;
                                break;
                            case 3:
                                indexOffset = FieldIndexOffset.right;
                                break;
                        }

                        charaData.attackSignal.direction = indexOffset;

                        for (int I = 0; I < skill[i].attackFieldIndexOffsetArray.Length; I++)
                        {
                            #region index調整
                            FieldIndexOffset directionCheck;
                            if (charaData.attackSignal.direction == FieldIndexOffset.left)
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // indexに現在のFieldIndexOffsetを代入
                                directionCheck = new FieldIndexOffset(directionCheck.columnOffset, directionCheck.rowOffset); // スキル方向の縦と横を入れ替える
                            }
                            else if (charaData.attackSignal.direction == FieldIndexOffset.right)
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // indexに現在のFieldIndexOffsetを代入
                                directionCheck = new FieldIndexOffset(directionCheck.columnOffset, directionCheck.rowOffset) * -1; // スキル方向の縦と横を入れ替えたのち、負の方向に変換
                            }
                            else if (charaData.attackSignal.direction == FieldIndexOffset.up)
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I]; // indexに現在のFieldIndexOffsetを代入
                            }
                            else
                            {
                                directionCheck = skill[i].attackFieldIndexOffsetArray[I] * -1; // indexに現在のFieldIndexOffsetを代入したのち、負の方向に変換
                            }
                            #endregion


                            if (skill[i].moveType == UniqueSkillMoveType.NONE)
                            {
                                testIndexOffset = FieldIndexOffset.zero;
                            }
                            else if (skill[i].moveType == UniqueSkillMoveType.FIXED)
                            {
                                testIndexOffset = indexOffset * fixedMoveAmount;
                                if (!fieldInformation.IsPassingGrid(charaData.index + testIndexOffset)) continue;
                            }

                            //Debug.Log($"haraData.index + moveOffset = {charaData.index.row + moveOffset.rowOffset} , {charaData.index.column + moveOffset.columnOffset} ({directionCheck.rowOffset},{directionCheck.columnOffset})");
                            //Debug.Log($"targetIndex = {targetIndex.row} , {targetIndex.column}");

                            if (skill[i].skillType == SkillTypeEnum.ATTACK)
                            {
                                if ((charaData.index + testIndexOffset) + directionCheck + indexOffset == targetIndex)
                                {
                                    charaData.attackSignal.selectedGrid = charaData.index + indexOffset + testIndexOffset;
                                    charaData.attackSignal.skillNumber = i;
                                    charaData.attackSignal.skillData = skill[i];
                                    return true;
                                }
                            }
                            else if (skill[i].skillType == SkillTypeEnum.SUPPORT)
                            {
                                charaData.attackSignal.selectedGrid = charaData.index + indexOffset;
                                charaData.attackSignal.skillNumber = i;
                                charaData.attackSignal.skillData = skill[i];
                                return true;
                            }
                        }
                    }
                    #endregion

                }
                return false;
            }

            List<FieldIndexOffset> WayIndexChecker (FieldIndexOffset sample)
            {
                // Debug.Log($"WayIndexChecker(player{charaData.playerNumber})");
                List<FieldIndexOffset> fieldIndexOffsets = new List<FieldIndexOffset>();

                for (int i = sample.rowOffset;i != 0;)
                {
                    if (i < 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.up);
                        i++;
                    }
                    else if (i > 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.down);
                        i--;
                    }
                    else i = 0;
                }
                
                for (int i = sample.columnOffset;i != 0;)
                {
                    if (i > 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.right);
                        i--;
                    }
                    else if (i < 0)
                    {
                        fieldIndexOffsets.Add(FieldIndexOffset.left);
                        i++;
                    }
                    else i = 0;
                }

                return fieldIndexOffsets;
            }

            List<FieldIndexOffset> GlidChecker(List<FieldIndexOffset> wayIndex)
            {
                cantMove = false;
                FieldIndex testIndex = FieldIndex.zero; // 移動マスを加味した移動座標を保存する
                List<FieldIndexOffset> fieldIndexOffsets = new List<FieldIndexOffset>(); // 値を返す用のリスト
                for (int i = 0;i < wayIndex.Count;i++)
                {
                    testIndex = wayIndex[i] + charaData.index; // キャラデータの座標と今回の追加座標を加味した座標に設定
                    if (!CanProceed(testIndex)) // 通れるかどうかの関数に対応
                    {
                        fieldIndexOffsets = new List<FieldIndexOffset>(); // リストを初期化
                        continue; // 通れないので今回の値をスキップする
                    }
                    else fieldIndexOffsets.Add(wayIndex[i]); // 通れるのであれば値を返す用のリストに格納する

                    if (wayIndex.Count <= 1) return fieldIndexOffsets; // 入れ替える数がここまでなら値を返す
                    
                    for (int j = 0;j < wayIndex.Count;j++)
                    {
                        if (j == i) continue; // 前回参照した値と同じなら処理をスキップ

                        testIndex += wayIndex[j]; // 移動座標に今回の移動分を加算

                        // 以下大体同じなので割愛

                        if (!CanProceed(testIndex))
                        {
                            fieldIndexOffsets = new List<FieldIndexOffset>();
                            continue;
                        }
                        else fieldIndexOffsets.Add(wayIndex[j]);

                        if (wayIndex.Count <= 2) return fieldIndexOffsets;

                        for (int I = 0;I < wayIndex.Count;I++)
                        {
                            if (I == i || I == j) continue;

                            testIndex += wayIndex[I];
                            if (!CanProceed(testIndex))
                            {
                                fieldIndexOffsets = new List<FieldIndexOffset>();
                                continue;
                            }
                            else fieldIndexOffsets.Add(wayIndex[I]);

                            if (wayIndex.Count <= 3) return fieldIndexOffsets;

                            for (int J = 0;J < wayIndex.Count;J++)
                            {
                                if (J == i || J == j || J == I) continue;

                                testIndex += wayIndex[J];
                                if (!CanProceed(testIndex))
                                {
                                    fieldIndexOffsets = new List<FieldIndexOffset>();
                                    continue;
                                }
                                else fieldIndexOffsets.Add(wayIndex[J]);

                                return fieldIndexOffsets;
                            }
                        }
                    }
                }

                cantMove = true;
                return wayIndex;

                bool CanProceed(FieldIndex testIndex_)
                {
                    // Debug.Log("進行不可");
                    if (!fieldInformation.IsPassingGrid(testIndex_)) return false; // 進もうとしている先が進行不可ならばfalseを返す
                    // Debug.Log("進行可/通行不可");
                    if (fieldInformation.GetDamageFieldOwner(testIndex_) != charaData.playerNumber && fieldInformation.GetDamageFieldOwner(testIndex_) >= 0) return false; // 進もうとしている先に自分以外のダメージフィールドが存在するならfalseを返す
                    
                    return true; // 上記の条件に該当しない場合進行可能とみなしtrueを返す
                }
            }
        }
    }
}