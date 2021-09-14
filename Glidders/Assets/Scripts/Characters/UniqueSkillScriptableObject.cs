using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glidders.Buff;

namespace Glidders
{
    namespace Character
    {
        // プロジェクトウィンドウで作成可能にする
        [CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "CreateSkillData")]
        public class UniqueSkillScriptableObject : ScriptableObject
        {
            // ユニークスキルの識別情報
            public string skillName;                    // スキル名称
            public int energy;                          // エネルギー
            public int priority;                        // 優先度
            public Sprite skillIcon;                    // スキルアイコン
            public AnimationClip skillAnimation;        // アニメーションクリップ
            public SkillTypeEnum skillType;             // スキルの種類（攻撃技か補助技か）

            public UniqueSkillMoveType moveType;        // 移動の種類
            public FieldIndexOffset[] MoveSelectRange;  // 移動先マス

            public bool isAttack;                       // 攻撃するかどうか
            public int damage;                          // ダメージ
            public int power;                           // 威力(ダメージフィールド)
            public FieldIndexOffset[] AttackSelectRange;// 選択可能マス
            public FieldIndexOffset[] AttackRange;      // 攻撃範囲マス

            public List<BuffViewData> giveBuff;         // 付与されるバフ
        }

        public enum UniqueSkillMoveType
        {
            NONE,
            FREE,
            FIXED,
        }
    }
}
