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
        /// �e��Signal��ϊ��\�Ȍ`���ɂ��ĕۑ����܂��B
        /// </summary>
        public class SignalConverter
        {
            // �f�[�^�ۑ��ɗp����OnlineDataStorage
            OnlineDataStorage onlineDataStorage = new OnlineDataStorage();
            // �����_���ɐ��������ID�̃p�^�[����
            const int ID_PATTERN = 1000000 - 1;

            // �ʐM�p�ɍœK�������e��L�[
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
            /// MoveSignal��ۑ��\�Ȍ`���ɕϊ����āA�Ή�����ID���擾���܂��B
            /// </summary>
            /// <param name="moveSignal">�ϊ��Ώۂ�MoveSignal�B</param>
            /// <param name="playerNumber">MoveSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            /// <returns>MoveSignal�ƑΉ�����ID�B</returns>
            public int GetMoveSignalId(MoveSignal moveSignal, int playerNumber)
            {
                // �ۑ��\�Ȍ`����AttackSignal
                SerializedMoveSignal serializedMoveSignal;

                // �f�[�^��ۑ��\�Ȍ`���ɕϊ�����
                int id = Convert2Serialized(moveSignal, out serializedMoveSignal);
                // �f�[�^���I�����C���ɕۑ�����
                SaveMoveSignal(serializedMoveSignal, id, playerNumber);

                // id��ԋp����
                return id;
            }

            /// <summary>
            /// MoveSignal��ۑ��\�Ȍ`���ɕϊ����܂��B
            /// </summary>
            /// <param name="moveSignal">�ϊ��O��MoveSignal�B</param>
            /// <param name="playerNumber">MoveSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            /// <param name="serializedMoveSignal">�ϊ����MoveSignal�B</param>
            /// <returns></returns>
            private int Convert2Serialized(MoveSignal moveSignal, out SerializedMoveSignal serializedMoveSignal)
            {
                serializedMoveSignal.moveRowDataArray = new int[Rule.maxMoveAmount];
                serializedMoveSignal.moveColumnDataArray = new int[Rule.maxMoveAmount];
                // MoveSignal�̒l���V���A���C�Y�\�Ȍ`���ɕϊ�����
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    serializedMoveSignal.moveRowDataArray[i] = moveSignal.moveDataArray[i].rowOffset;
                    serializedMoveSignal.moveColumnDataArray[i] = moveSignal.moveDataArray[i].columnOffset;
                }

                // id�̒l�������_���ɐݒ肷��
                int id = Random.Range(0, ID_PATTERN);
                return id;
            }

            /// <summary>
            /// MoveSignal�̃f�[�^��ۑ��\�Ȍ`����OnlineDataStorage�ɕۑ����܂��B
            /// </summary>
            /// <param name="serializedMoveSignal">�V���A���C�Y�\�Ȓl�ɕϊ����ꂽMoveSignal�B</param>
            /// <param name="id">�ۑ��p����ю擾�p�̃����_���Ȑ��l�B</param>
            /// <param name="playerNumber">MoveSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            private void SaveMoveSignal(SerializedMoveSignal serializedMoveSignal, int id, int playerNumber)
            {
                // �e��f�[�^��OnlineDataStorage�ɕۑ�����
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    onlineDataStorage.Add((Owner)playerNumber, id.ToString() + MOVE_ROW + i.ToString(), serializedMoveSignal.moveRowDataArray[i]);
                    onlineDataStorage.Add((Owner)playerNumber, id.ToString() + MOVE_COLUMN + i.ToString(), serializedMoveSignal.moveColumnDataArray[i]);
                }
            }



            /// <summary>
            /// AttackSignal��ۑ��\�Ȍ`���ɕϊ����āA�Ή�����ID���擾���܂��B
            /// </summary>
            /// <param name="attackSignal">�ϊ��Ώۂ�AttackSignal�B</param>
            /// <param name="playerNumber">AttackSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            /// <returns>AttackSignal�ƑΉ�����ID�B</returns>
            public int GetAttackSignalId(AttackSignal attackSignal, int playerNumber)
            {
                // �ۑ��\�Ȍ`����AttackSignal
                SerializedAttackSignal serializedAttackSignal;

                // �f�[�^��ۑ��\�Ȍ`���ɕϊ�����
                int id = Convert2Serialized(attackSignal, out serializedAttackSignal);
                // �f�[�^���I�����C���ɕۑ�����
                SaveAttackSignal(serializedAttackSignal, id, playerNumber);
                
                // id��ԋp����
                return id;
            }

            /// <summary>
            /// AttackSignal��ۑ��\�Ȍ`���ɕϊ����܂��B
            /// </summary>
            /// <param name="attackSignal">�ϊ��O��AttackSignal�B</param>
            /// <param name="playerNumber">AttackSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            /// <param name="serializedAttackSignal">�ϊ����AttackSignal�B</param>
            /// <returns></returns>
            private int Convert2Serialized(AttackSignal attackSignal, out SerializedAttackSignal serializedAttackSignal)
            {
                // AttackSignal�̒l���V���A���C�Y�\�Ȍ`���ɕϊ�����
                serializedAttackSignal = new SerializedAttackSignal();
                serializedAttackSignal.isAttack = attackSignal.isAttack ? 1 : 0;
                serializedAttackSignal.uniqueSkillScriptableObjectId = attackSignal.skillData.id;
                serializedAttackSignal.selectedGrid_row = attackSignal.selectedGrid.row;
                serializedAttackSignal.selectedGrid_column = attackSignal.selectedGrid.column;
                serializedAttackSignal.direction_rowOffset = attackSignal.direction.rowOffset;
                serializedAttackSignal.direction_columnOffset = attackSignal.direction.columnOffset;
                serializedAttackSignal.skillNumber = attackSignal.skillNumber;

                // id�̒l�������_���ɐݒ肷��
                int id = Random.Range(0, ID_PATTERN);
                return id;
            }

            /// <summary>
            /// AttackSignal�̃f�[�^��ۑ��\�Ȍ`����OnlineDataStorage�ɕۑ����܂��B
            /// </summary>
            /// <param name="serializedAttackSignal">�V���A���C�Y�\�Ȓl�ɕϊ����ꂽAttackSignal�B</param>
            /// <param name="id">�ۑ��p����ю擾�p�̃����_���Ȑ��l�B</param>
            /// <param name="playerNumber">AttackSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            private void SaveAttackSignal(SerializedAttackSignal serializedAttackSignal, int id, int playerNumber)
            {
                // �e��f�[�^��OnlineDataStorage�ɕۑ�����
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + IS_ATTACK, serializedAttackSignal.isAttack);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SKILL_ID, serializedAttackSignal.uniqueSkillScriptableObjectId);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SELECTED_ROW, serializedAttackSignal.selectedGrid_row);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SELECTED_COLUMN, serializedAttackSignal.selectedGrid_column);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_ROW, serializedAttackSignal.direction_rowOffset);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_COLUMN, serializedAttackSignal.direction_columnOffset);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + SKILL_NUMBER, serializedAttackSignal.skillNumber);
            }



            /// <summary>
            /// DirectionSignal��ۑ��\�Ȍ`���ɕϊ����āA�Ή�����ID���擾���܂��B
            /// </summary>
            /// <param name="directionSignal">�ϊ��Ώۂ�DirectionSignal�B</param>
            /// <param name="playerNumber">DirectionSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            /// <returns>DirectionSignal�ƑΉ�����ID�B</returns>
            public int GetDirectionSignalId(DirecionSignal directionSignal, int playerNumber)
            {
                // �ۑ��\�Ȍ`����AttackSignal
                SerializedDirectionSignal serializedDirectionSignal;

                // �f�[�^��ۑ��\�Ȍ`���ɕϊ�����
                int id = Convert2Serialized(directionSignal, out serializedDirectionSignal);
                // �f�[�^���I�����C���ɕۑ�����
                SaveDirectionSignal(serializedDirectionSignal, id, playerNumber);

                // id��ԋp����
                return id;
            }

            /// <summary>
            /// DirectionSignal��ۑ��\�Ȍ`���ɕϊ����܂��B
            /// </summary>
            /// <param name="directionSignal">�ϊ��O��DirectionSignal�B</param>
            /// <param name="playerNumber">DirectionSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            /// <param name="serializedDirectionSignal">�ϊ����DirectionSignal�B</param>
            /// <returns></returns>
            private int Convert2Serialized(DirecionSignal directionSignal, out SerializedDirectionSignal serializedDirectionSignal)
            {
                // DirectionSignal�̒l���V���A���C�Y�\�Ȍ`���ɕϊ�����
                serializedDirectionSignal.direction_rowOffset = directionSignal.direction.rowOffset;
                serializedDirectionSignal.direction_columnOffset = directionSignal.direction.columnOffset;

                // id�̒l�������_���ɐݒ肷��
                int id = Random.Range(0, ID_PATTERN);
                return id;
            }

            /// <summary>
            /// DirectionSignal�̃f�[�^��ۑ��\�Ȍ`����OnlineDataStorage�ɕۑ����܂��B
            /// </summary>
            /// <param name="serializedDirectionSignal">�V���A���C�Y�\�Ȓl�ɕϊ����ꂽDirectionSignal�B</param>
            /// <param name="id">�ۑ��p����ю擾�p�̃����_���Ȑ��l�B</param>
            /// <param name="playerNumber">AttackSignal�̔��M���ƂȂ�v���C���[�ԍ��B</param>
            private void SaveDirectionSignal(SerializedDirectionSignal serializedDirectionSignal, int id, int playerNumber)
            {
                // �e��f�[�^��OnlineDataStorage�ɕۑ�����
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_SIGNAL_ROW, serializedDirectionSignal.direction_rowOffset);
                onlineDataStorage.Add((Owner)playerNumber, id.ToString() + DIRECTION_SIGNAL_COLUMN, serializedDirectionSignal.direction_columnOffset);
            }





            /// <summary>
            /// ID��p���đΉ�����Signal�f�[�^���擾���܂��B
            /// </summary>
            /// <param name="id">�擾����Signal�f�[�^�ɑΉ�����ID�B</param>
            /// <param name="playerNumber">Signal�f�[�^�����L����v���C���[�ԍ��B</param>
            /// <returns>�擾����Signal�f�[�^�B</returns>
            public MoveSignal GetMoveSignalData(int id, int playerNumber)
            {
                SerializedMoveSignal serializedMoveSignal = GetSerializedMoveFromStorage(id, playerNumber);
                return Convert2Signal(serializedMoveSignal);
            }

            /// <summary>
            /// SerializedSignal�𐳂���Signal�f�[�^�ɕϊ����܂��B
            /// </summary>
            /// <param name="serializedMoveSignal">�ϊ��O��SerializedSignal�f�[�^�B</param>
            /// <returns>�ϊ���̐�������Ԃ�Signal�f�[�^�B</returns>
            private MoveSignal Convert2Signal(SerializedMoveSignal serializedMoveSignal)
            {
                // MoveSignal�̃R���X�g���N�^�ɑ���z����쐬����
                FieldIndexOffset[] fieldIndexOffsets = new FieldIndexOffset[Rule.maxMoveAmount];
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    fieldIndexOffsets[i].rowOffset = serializedMoveSignal.moveRowDataArray[i];
                    fieldIndexOffsets[i].columnOffset = serializedMoveSignal.moveColumnDataArray[i];
                }

                // Signal�𐶐����ĕԋp����
                return new MoveSignal(fieldIndexOffsets);
            }

            /// <summary>
            /// SerializedSignal���擾���܂��B
            /// </summary>
            /// <param name="id">SerializedSignal�ɑΉ�����ID�B</param>
            /// <param name="playerNumber">SerializedSignal���擾����v���C���[�ԍ��B</param>
            /// <returns>�擾����SerializedSignal�B</returns>
            private SerializedMoveSignal GetSerializedMoveFromStorage(int id, int playerNumber)
            {
                // �ԋp�p�̃f�[�^�^�𐶐�����
                SerializedMoveSignal returnData = new SerializedMoveSignal();
                returnData.moveRowDataArray = new int[Rule.maxMoveAmount];
                returnData.moveColumnDataArray = new int[Rule.maxMoveAmount];
                // �e��f�[�^��OnlineDataStorage����擾����
                for (int i = 0; i < Rule.maxMoveAmount; ++i)
                {
                    returnData.moveRowDataArray[i] = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + MOVE_ROW + i.ToString());
                    returnData.moveColumnDataArray[i] = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + MOVE_COLUMN + i.ToString());
                }
                return returnData;
            }



            /// <summary>
            /// ID��p���đΉ�����Signal�f�[�^���擾���܂��B
            /// </summary>
            /// <param name="id">�擾����Signal�f�[�^�ɑΉ�����ID�B</param>
            /// <param name="playerNumber">Signal�f�[�^�����L����v���C���[�ԍ��B</param>
            /// <returns>�擾����Signal�f�[�^�B</returns>
            public AttackSignal GetAttackSignalData(int id, int playerNumber)
            {
                SerializedAttackSignal serializedAttackSignal = GetSerializedAttackFromStorage(id, playerNumber);
                return Convert2Signal(serializedAttackSignal);
            }

            /// <summary>
            /// SerializedSignal�𐳂���Signal�f�[�^�ɕϊ����܂��B
            /// </summary>
            /// <param name="serializedMoveSignal">�ϊ��O��SerializedSignal�f�[�^�B</param>
            /// <returns>�ϊ���̐�������Ԃ�Signal�f�[�^�B</returns>
            private AttackSignal Convert2Signal(SerializedAttackSignal serializedAttackSignal)
            {
                // MoveSignal�̃R���X�g���N�^�ɑ���f�[�^���쐬����
                FieldIndex selectedGrid;
                selectedGrid.row = serializedAttackSignal.selectedGrid_row;
                selectedGrid.column = serializedAttackSignal.selectedGrid_column;
                FieldIndexOffset direction;
                direction.rowOffset = serializedAttackSignal.direction_rowOffset;
                direction.columnOffset = serializedAttackSignal.direction_columnOffset;
                UniqueSkillScriptableObject skillData = ScriptableObjectDatabase.GetSkill(serializedAttackSignal.uniqueSkillScriptableObjectId);

                // Signal�𐶐����ĕԋp����
                return new AttackSignal(skillData, selectedGrid, direction, serializedAttackSignal.skillNumber);
            }

            /// <summary>
            /// SerializedSignal���擾���܂��B
            /// </summary>
            /// <param name="id">SerializedSignal�ɑΉ�����ID�B</param>
            /// <param name="playerNumber">SerializedSignal���擾����v���C���[�ԍ��B</param>
            /// <returns>�擾����SerializedSignal�B</returns>
            private SerializedAttackSignal GetSerializedAttackFromStorage(int id, int playerNumber)
            {
                // �ԋp�p�̃f�[�^�^�𐶐�����
                SerializedAttackSignal returnData = new SerializedAttackSignal();
                // �e��f�[�^��OnlineDataStorage����擾����
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
            /// ID��p���đΉ�����Signal�f�[�^���擾���܂��B
            /// </summary>
            /// <param name="id">�擾����Signal�f�[�^�ɑΉ�����ID�B</param>
            /// <param name="playerNumber">Signal�f�[�^�����L����v���C���[�ԍ��B</param>
            /// <returns>�擾����Signal�f�[�^�B</returns>
            public DirecionSignal GetDirectionSignalData(int id, int playerNumber)
            {
                SerializedDirectionSignal serializedDirectionSignal = GetSerializedDirectionFromStorage(id, playerNumber);
                return Convert2Signal(serializedDirectionSignal);
            }

            /// <summary>
            /// SerializedSignal�𐳂���Signal�f�[�^�ɕϊ����܂��B
            /// </summary>
            /// <param name="serializedMoveSignal">�ϊ��O��SerializedSignal�f�[�^�B</param>
            /// <returns>�ϊ���̐�������Ԃ�Signal�f�[�^�B</returns>
            private DirecionSignal Convert2Signal(SerializedDirectionSignal serializedDirectionSignal)
            {
                // DirectionSignal�̃R���X�g���N�^�ɑ���f�[�^���쐬����
                FieldIndexOffset fieldIndexOffset;
                fieldIndexOffset.rowOffset = serializedDirectionSignal.direction_rowOffset;
                fieldIndexOffset.columnOffset = serializedDirectionSignal.direction_columnOffset;

                // Signal�𐶐����ĕԋp����
                return new DirecionSignal(fieldIndexOffset);
            }

            /// <summary>
            /// SerializedSignal���擾���܂��B
            /// </summary>
            /// <param name="id">SerializedSignal�ɑΉ�����ID�B</param>
            /// <param name="playerNumber">SerializedSignal���擾����v���C���[�ԍ��B</param>
            /// <returns>�擾����SerializedSignal�B</returns>
            private SerializedDirectionSignal GetSerializedDirectionFromStorage(int id, int playerNumber)
            {
                // �ԋp�p�̃f�[�^�^�𐶐�����
                SerializedDirectionSignal returnData = new SerializedDirectionSignal();
                // �e��f�[�^��OnlineDataStorage����擾����
                returnData.direction_rowOffset = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + DIRECTION_ROW);
                returnData.direction_columnOffset = onlineDataStorage.Get<int>((Owner)playerNumber, id.ToString() + DIRECTION_COLUMN);
                return returnData;
            }
        }

        public struct SerializedMoveSignal
        {
            // �ړ����̐��Ԃ�̏c�Ɖ�
            public int[] moveRowDataArray;
            public int[] moveColumnDataArray;

            // ���̃f�[�^
            //public FieldIndexOffset[] moveDataArray;    // �ړ������i�[�����z��
        }

        public struct SerializedAttackSignal
        {
            // �U���������ǂ���
            public int isAttack;
            // ���j�[�N�X�L���̎���ID
            public string uniqueSkillScriptableObjectId;
            // �X�L���g�p���ɑI�������ʒu�̏c�Ɖ�
            public int selectedGrid_row;
            public int selectedGrid_column;
            // �X�L�����������̏c�Ɖ�
            public int direction_rowOffset;
            public int direction_columnOffset;
            // �X�L���ԍ�
            public int skillNumber;

            // ���̃f�[�^
            //public bool isAttack;                      // �U���������ǂ����B
            //public UniqueSkillScriptableObject skillData;    // �g�p����X�L����񂪊i�[���ꂽUniqueSkillScriptableObject�B
            //public FieldIndex selectedGrid;            // �X�L���g�p���ɑI�������ʒu�B
            //public FieldIndexOffset direction;         // �X�L�����������B
            //public int skillNumber;
        }

        public struct SerializedDirectionSignal
        {
            // �����̏c�Ɖ�
            public int direction_rowOffset;
            public int direction_columnOffset;

            // ���̃f�[�^
            //public FieldIndexOffset direction;
        }
    }
}
