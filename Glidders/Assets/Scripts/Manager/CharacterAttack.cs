using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Glidders
{
    namespace Manager
    {
        public class CharacterAttack
        {
            const int PLAYER_AMOUNT = 2; // playerの総数

            // public AttackSignal[] signals = new AttackSignal[CHARACTER_AMOUNT];
            public List<CharacterData> sampleSignals; // 受け取った配列をリストとして扱うためのリスト
            public int[] addPoint = new int[PLAYER_AMOUNT]; // 追加するポイント量
            public CharacterAttack()
            {

            }

            public void AttackOrder(ref CharacterData[] characterDatas)
            {
                sampleSignals = new List<CharacterData>(); // リスト内部初期化

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
                
                AttackIndexCheck(); // 攻撃の場所を確定する関数

                // 持っているポイントを各キャラに追加
                for (int i = 0;i < characterDatas.Length;i++)
                {
                    characterDatas[i].point += addPoint[i];
                }
            }
            /// <summary>
            /// 攻撃の場所を確定し、ダメージの発生関数を呼ぶ
            /// </summary>
            private void AttackIndexCheck()
            {
                var signalList = sampleSignals.OrderByDescending(x => x.attackSignal.skillData.priority); // 攻撃順にリストを入れ替える  

                foreach (var x in signalList)
                { 
                    // Debug.Log($"sampleListの{x.attackSignal.skillData.skillName}は{x.attackSignal.skillData.damage}のダメージをあたえるぜ");

                    if (!x.canAct) return; // 自身が攻撃できない状況にある場合、処理をスキップする
                    if (!x.attackSignal.isAttack) return; // 攻撃をしないという情報が入っているとき、処理をスキップする

                    // 攻撃マス数分処理を回す
                    for (int j = 0;j < x.attackSignal.skillData.attackFieldIndexOffsetArray.Length;j++)
                    {
                        FieldIndex attackPosition = x.attackSignal.selectedGrid + x.attackSignal.skillData.attackFieldIndexOffsetArray[j]; // 攻撃指定位置に、攻撃範囲を足した量を攻撃位置として保存

                        AttackDamage(x, attackPosition); // 攻撃のダメージを発生する関数

                        Debug.Log($"attackPosition.index({j}) = ({attackPosition.row},{attackPosition.column})");
                    }
                }
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
                        Debug.Log($"{character.thisObject.name}の{character.attackSignal.skillData.name}は{sampleSignals[i].thisObject.name}にヒットし、{character.attackSignal.skillData.damage}のポイントを得た");
                    }

                    // Debug.Log($"sampleSignals[{i}]({sampleSignals[i].index.row},{sampleSignals[i].index.column}) || attackPosition({attackPosition.row},{attackPosition.column})");
                }
            }
        }

    }
}
