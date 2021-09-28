using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders;
using Glidders.Character;
using Glidders.Photon;

namespace Glidders
{
    namespace Manager
    {
        /// <summary>
        /// 各種Signalを変換可能な形式にして保存します。
        /// </summary>
        public class SignalConverter
        {
            // データ保存に用いるOnlineDataStorage
            OnlineDataStorage onlineDataStorage = new OnlineDataStorage();
            // ランダムに生成されるIDのパターン数
            const int ID_PATTERN = 1000000 - 1;

            // 通信用に最適化した各種キー
            const string MOVE_ROW = "mr";
            const string MOVE_COLUMN = "mc";

            const string IS_ATTACK = "a";
            const string SKILL_ID = "id";
            const string SELECTED_ROW = "sr";
            const string SELECTED_COLUMN = "sc";
            const string DIRECTION_ROW = "dr";
            const string DIRECTION_COLUMN = "dc";
            const string SKILL_NUMBER = "n";

            const string DIRECTION_SIGNAL_ROW = "dsr";
            const string DIRECTION_SIGNAL_COLUMN = "dsc";

            /// <summary>
            /// MoveSignalを保存可能な形式に変換して、対応するIDを取得します。
            /// </summary>
            /// <param name="moveSignal">変換対象のMoveSignal。</param>
            /// <param name="playerNumber">MoveSignalの発信源となるプレイヤー番号。</param>
            /// <returns>MoveSignalと対応するID。</returns>
            public int GetMoveSignalId(MoveSignal moveSignal, int playerNumber)
            {
                // 保存可能な形式のAttackSignal
                SerializedMoveSignal serializedMoveSignal;

                // データを保存可能な形式に変換する
                int id = Convert2Serialized(moveSignal, out serializedMoveSignal);
                // データをオンラインに保存する
                SaveMoveSignal(serializedMoveSignal, id, playerNumber);

                // idを返却する
                return id;
            }

            /// <summary>
            /// MoveSignalを保存可能な形式に変換します。
            /// </summary>
            /// <param name="moveSignal">変換前のMoveSignal。</param>
            /// <param name="playerNumber">MoveSignalの発信源となるプレイヤー番号。</param>
            /// <param name="serializedMoveSignal">変換後のMoveSignal。</param>
            /// <returns></returns>
            private int Convert2Serialized(MoveSignal moveSignal, out SerializedMoveSignal serializedMoveSignal)
            {
                serializedMoveSignal.moveRowDataArray = new int[Rule.maxMoveAmount];
                serializedMoveSignal.moveColumnDataArray = new int[Rule.maxMoveAmount];
                // MoveSignalの値をシリアライズ可能な形式に変換する
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    serializedMoveSignal.moveRowDataArray[i] = moveSignal.moveDataArray[i].rowOffset;
                    serializedMoveSignal.moveColumnDataArray[i] = moveSignal.moveDataArray[i].columnOffset;
                }

                // idの値をランダムに設定する
                int id = Random.Range(0, ID_PATTERN);
                return id;
            }

            /// <summary>
            /// MoveSignalのデータを保存可能な形式でOnlineDataStorageに保存します。
            /// </summary>
            /// <param name="serializedMoveSignal">シリアライズ可能な値に変換されたMoveSignal。</param>
            /// <param name="id">保存用および取得用のランダムな数値。</param>
            /// <param name="playerNumber">MoveSignalの発信源となるプレイヤー番号。</param>
            private void SaveMoveSignal(SerializedMoveSignal serializedMoveSignal, int id, int playerNumber)
            {
                // 各種データをOnlineDataStorageに保存する
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    onlineDataStorage.Add((Owner)playerNumber, id.ToString() + MOVE_ROW + i.ToString(), serializedMoveSignal.moveRowDataArray[i]);
                    onlineDataStorage.Add((Owner)playerNumber, id.ToString() + MOVE_COLUMN + i.ToString(), serializedMoveSignal.moveColumnDataArray[i]);
                }
            }



            /// <summary>
            /// AttackSignalを保存可能な形式に変換して、対応するIDを取得します。
            /// </summary>
            /// <param name="attackSignal">変換対象のAttackSignal。</param>
            /// <param name="playerNumber">AttackSignalの発信源となるプレイヤー番号。</param>
            /// <returns>AttackSignalと対応するID。</returns>
            public int GetAttackSignalId(AttackSignal attackSignal, int playerNumber)
            {
                // 保存可能な形式のAttackSignal
                SerializedAttackSignal serializedAttackSignal;

                // データを保存可能な形式に変換する
                int id = Convert2Serialized(attackSignal, out serializedAttackSignal);
                // データをオンラインに保存する
                SaveAttackSignal(serializedAttackSignal, id, playerNumber);
                
                // idを返却する
                return id;
            }

            /// <summary>
            /// AttackSignalを保存可能な形式に変換します。
            /// </summary>
            /// <param name="attackSignal">変換前のAttackSignal。</param>
            /// <param name="playerNumber">AttackSignalの発信源となるプレイヤー番号。</param>
            /// <param name="serializedAttackSignal">変換後のAttackSignal。</param>
            /// <returns></returns>
            private int Convert2Serialized(AttackSignal attackSignal, out SerializedAttackSignal serializedAttackSignal)
            {
                // AttackSignalの値をシリアライズ可能な形式に変換する
                serializedAttackSignal = new SerializedAttackSignal();
                serializedAttackSignal.isAttack = attackSignal.isAttack ? 1 : 0;
                serializedAttackSignal.uniqueSkillScriptableObjectId = attackSignal.skillData.id;
                serializedAttackSignal.selectedGrid_row = attackSignal.selectedGrid.row;
                serializedAttackSignal.selectedGrid_column = attackSignal.selectedGrid.column;
                serializedAttackSignal.direction_rowOffset = attackSignal.direction.rowOffset;
                serializedAttackSignal.direction_columnOffset = attackSignal.direction.columnOffset;
                serializedAttackSignal.skillNumber = attackSignal.skillNumber;

                // idの値をランダムに設定する
                int id = Random.Range(0, ID_PATTERN);
                return id;
            }

            /// <summary>
            /// AttackSignalのデータを保存可能な形式でOnlineDataStorageに保存します。
            /// </summary>
            /// <param name="serializedAttackSignal">シリアライズ可能な値に変換されたAttackSignal。</param>
            /// <param name="id">保存用および取得用のランダムな数値。</param>
            /// <param name="playerNumber">AttackSignalの発信源となるプレイヤー番号。</param>
            private void SaveAttackSignal(SerializedAttackSignal serializedAttackSignal, int id, int playerNumber)
            {
                // 各種データをOnlineDataStorageに保存する
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + IS_ATTACK, serializedAttackSignal.isAttack);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SKILL_ID, serializedAttackSignal.uniqueSkillScriptableObjectId);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SELECTED_ROW, serializedAttackSignal.selectedGrid_row);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SELECTED_COLUMN, serializedAttackSignal.selectedGrid_column);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_ROW, serializedAttackSignal.direction_rowOffset);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_COLUMN, serializedAttackSignal.direction_columnOffset);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SKILL_NUMBER, serializedAttackSignal.skillNumber);
            }



            /// <summary>
            /// DirectionSignalを保存可能な形式に変換して、対応するIDを取得します。
            /// </summary>
            /// <param name="directionSignal">変換対象のDirectionSignal。</param>
            /// <param name="playerNumber">DirectionSignalの発信源となるプレイヤー番号。</param>
            /// <returns>DirectionSignalと対応するID。</returns>
            public int GetDirectionSignalId(DirecionSignal directionSignal, int playerNumber)
            {
                // 保存可能な形式のAttackSignal
                SerializedDirectionSignal serializedDirectionSignal;

                // データを保存可能な形式に変換する
                int id = Convert2Serialized(directionSignal, out serializedDirectionSignal);
                // データをオンラインに保存する
                SaveDirectionSignal(serializedDirectionSignal, id, playerNumber);

                // idを返却する
                return id;
            }

            /// <summary>
            /// DirectionSignalを保存可能な形式に変換します。
            /// </summary>
            /// <param name="directionSignal">変換前のDirectionSignal。</param>
            /// <param name="playerNumber">DirectionSignalの発信源となるプレイヤー番号。</param>
            /// <param name="serializedDirectionSignal">変換後のDirectionSignal。</param>
            /// <returns></returns>
            private int Convert2Serialized(DirecionSignal directionSignal, out SerializedDirectionSignal serializedDirectionSignal)
            {
                // DirectionSignalの値をシリアライズ可能な形式に変換する
                serializedDirectionSignal.direction_rowOffset = directionSignal.direction.rowOffset;
                serializedDirectionSignal.direction_columnOffset = directionSignal.direction.columnOffset;

                // idの値をランダムに設定する
                int id = Random.Range(0, ID_PATTERN);
                return id;
            }

            /// <summary>
            /// DirectionSignalのデータを保存可能な形式でOnlineDataStorageに保存します。
            /// </summary>
            /// <param name="serializedDirectionSignal">シリアライズ可能な値に変換されたDirectionSignal。</param>
            /// <param name="id">保存用および取得用のランダムな数値。</param>
            /// <param name="playerNumber">AttackSignalの発信源となるプレイヤー番号。</param>
            private void SaveDirectionSignal(SerializedDirectionSignal serializedDirectionSignal, int id, int playerNumber)
            {
                // 各種データをOnlineDataStorageに保存する
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_SIGNAL_ROW, serializedDirectionSignal.direction_rowOffset);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_SIGNAL_COLUMN, serializedDirectionSignal.direction_columnOffset);
            }





            /// <summary>
            /// IDを用いて対応するSignalデータを取得します。
            /// </summary>
            /// <param name="id">取得するSignalデータに対応するID。</param>
            /// <param name="playerNumber">Signalデータを所有するプレイヤー番号。</param>
            /// <returns>取得したSignalデータ。</returns>
            public MoveSignal GetMoveSignalData(int id, int playerNumber)
            {
                SerializedMoveSignal serializedMoveSignal = GetSerializedMoveFromStorage(id, playerNumber);
                return Convert2Signal(serializedMoveSignal);
            }

            /// <summary>
            /// SerializedSignalを正しいSignalデータに変換します。
            /// </summary>
            /// <param name="serializedMoveSignal">変換前のSerializedSignalデータ。</param>
            /// <returns>変換後の正しい状態のSignalデータ。</returns>
            private MoveSignal Convert2Signal(SerializedMoveSignal serializedMoveSignal)
            {
                // MoveSignalのコンストラクタに送る配列を作成する
                FieldIndexOffset[] fieldIndexOffsets = new FieldIndexOffset[Rule.maxMoveAmount];
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    fieldIndexOffsets[i].rowOffset = serializedMoveSignal.moveRowDataArray[i];
                    fieldIndexOffsets[i].columnOffset = serializedMoveSignal.moveColumnDataArray[i];
                }

                // Signalを生成して返却する
                return new MoveSignal(fieldIndexOffsets);
            }

            /// <summary>
            /// SerializedSignalを取得します。
            /// </summary>
            /// <param name="id">SerializedSignalに対応するID。</param>
            /// <param name="playerNumber">SerializedSignalを取得するプレイヤー番号。</param>
            /// <returns>取得したSerializedSignal。</returns>
            private SerializedMoveSignal GetSerializedMoveFromStorage(int id, int playerNumber)
            {
                // 返却用のデータ型を生成する
                SerializedMoveSignal returnData = new SerializedMoveSignal();
                returnData.moveRowDataArray = new int[Rule.maxMoveAmount];
                returnData.moveColumnDataArray = new int[Rule.maxMoveAmount];
                // 各種データをOnlineDataStorageから取得する
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    returnData.moveRowDataArray[i] = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + MOVE_ROW + i.ToString());
                    returnData.moveColumnDataArray[i] = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + MOVE_COLUMN + i.ToString());
                }
                return returnData;
            }



            /// <summary>
            /// IDを用いて対応するSignalデータを取得します。
            /// </summary>
            /// <param name="id">取得するSignalデータに対応するID。</param>
            /// <param name="playerNumber">Signalデータを所有するプレイヤー番号。</param>
            /// <returns>取得したSignalデータ。</returns>
            public AttackSignal GetAttackSignalData(int id, int playerNumber)
            {
                SerializedAttackSignal serializedAttackSignal = GetSerializedAttackFromStorage(id, playerNumber);
                return Convert2Signal(serializedAttackSignal);
            }

            /// <summary>
            /// SerializedSignalを正しいSignalデータに変換します。
            /// </summary>
            /// <param name="serializedMoveSignal">変換前のSerializedSignalデータ。</param>
            /// <returns>変換後の正しい状態のSignalデータ。</returns>
            private AttackSignal Convert2Signal(SerializedAttackSignal serializedAttackSignal)
            {
                // MoveSignalのコンストラクタに送るデータを作成する
                FieldIndex selectedGrid;
                selectedGrid.row = serializedAttackSignal.selectedGrid_row;
                selectedGrid.column = serializedAttackSignal.selectedGrid_column;
                FieldIndexOffset direction;
                direction.rowOffset = serializedAttackSignal.direction_rowOffset;
                direction.columnOffset = serializedAttackSignal.direction_columnOffset;
                UniqueSkillScriptableObject skillData = ScriptableObjectDatabase.GetSkill(serializedAttackSignal.uniqueSkillScriptableObjectId);

                // Signalを生成して返却する
                return new AttackSignal(skillData, selectedGrid, direction, serializedAttackSignal.skillNumber);
            }

            /// <summary>
            /// SerializedSignalを取得します。
            /// </summary>
            /// <param name="id">SerializedSignalに対応するID。</param>
            /// <param name="playerNumber">SerializedSignalを取得するプレイヤー番号。</param>
            /// <returns>取得したSerializedSignal。</returns>
            private SerializedAttackSignal GetSerializedAttackFromStorage(int id, int playerNumber)
            {
                // 返却用のデータ型を生成する
                SerializedAttackSignal returnData = new SerializedAttackSignal();
                // 各種データをOnlineDataStorageから取得する
                returnData.isAttack = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + IS_ATTACK);
                returnData.uniqueSkillScriptableObjectId = onlineDataStorage.Get<string>((Owner)playerNumber, id.ToString() + SKILL_ID);
                returnData.selectedGrid_row = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + SELECTED_ROW);
                returnData.selectedGrid_column = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + SELECTED_COLUMN);
                returnData.direction_rowOffset = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + DIRECTION_ROW);
                returnData.direction_columnOffset = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + DIRECTION_COLUMN);
                returnData.skillNumber = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + SKILL_NUMBER);
                return returnData;
            }

            /// <summary>
            /// IDを用いて対応するSignalデータを取得します。
            /// </summary>
            /// <param name="id">取得するSignalデータに対応するID。</param>
            /// <param name="playerNumber">Signalデータを所有するプレイヤー番号。</param>
            /// <returns>取得したSignalデータ。</returns>
            public DirecionSignal GetDirectionSignalData(int id, int playerNumber)
            {
                SerializedDirectionSignal serializedDirectionSignal = GetSerializedDirectionFromStorage(id, playerNumber);
                return Convert2Signal(serializedDirectionSignal);
            }

            /// <summary>
            /// SerializedSignalを正しいSignalデータに変換します。
            /// </summary>
            /// <param name="serializedMoveSignal">変換前のSerializedSignalデータ。</param>
            /// <returns>変換後の正しい状態のSignalデータ。</returns>
            private DirecionSignal Convert2Signal(SerializedDirectionSignal serializedDirectionSignal)
            {
                // DirectionSignalのコンストラクタに送るデータを作成する
                FieldIndexOffset fieldIndexOffset;
                fieldIndexOffset.rowOffset = serializedDirectionSignal.direction_rowOffset;
                fieldIndexOffset.columnOffset = serializedDirectionSignal.direction_columnOffset;

                // Signalを生成して返却する
                return new DirecionSignal(fieldIndexOffset);
            }

            /// <summary>
            /// SerializedSignalを取得します。
            /// </summary>
            /// <param name="id">SerializedSignalに対応するID。</param>
            /// <param name="playerNumber">SerializedSignalを取得するプレイヤー番号。</param>
            /// <returns>取得したSerializedSignal。</returns>
            private SerializedDirectionSignal GetSerializedDirectionFromStorage(int id, int playerNumber)
            {
                // 返却用のデータ型を生成する
                SerializedDirectionSignal returnData = new SerializedDirectionSignal();
                // 各種データをOnlineDataStorageから取得する
                returnData.direction_rowOffset = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + DIRECTION_ROW);
                returnData.direction_columnOffset = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + DIRECTION_COLUMN);
                return returnData;
            }
        }

        public struct SerializedMoveSignal
        {
            // 移動情報の数ぶんの縦と横
            public int[] moveRowDataArray;
            public int[] moveColumnDataArray;

            // 元のデータ
            //public FieldIndexOffset[] moveDataArray;    // 移動情報を格納した配列
        }

        public struct SerializedAttackSignal
        {
            // 攻撃したかどうか
            public int isAttack;
            // ユニークスキルの識別ID
            public string uniqueSkillScriptableObjectId;
            // スキル使用時に選択した位置の縦と横
            public int selectedGrid_row;
            public int selectedGrid_column;
            // スキルを撃つ向きの縦と横
            public int direction_rowOffset;
            public int direction_columnOffset;
            // スキル番号
            public int skillNumber;

            // 元のデータ
            //public bool isAttack;                      // 攻撃したかどうか。
            //public UniqueSkillScriptableObject skillData;    // 使用するスキル情報が格納されたUniqueSkillScriptableObject。
            //public FieldIndex selectedGrid;            // スキル使用時に選択した位置。
            //public FieldIndexOffset direction;         // スキルを撃つ向き。
            //public int skillNumber;
        }

        public struct SerializedDirectionSignal
        {
            // 向きの縦と横
            public int direction_rowOffset;
            public int direction_columnOffset;

            // 元のデータ
            //public FieldIndexOffset direction;
        }
    }
}
