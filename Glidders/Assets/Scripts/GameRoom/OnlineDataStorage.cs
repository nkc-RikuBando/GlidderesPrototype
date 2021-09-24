using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Security.Cryptography;
using System.Text;

namespace Glidders
{
    namespace Photon
    {
        public class OnlineDataStorage : MonoBehaviourPunCallbacks
        {
            private Player[] players;   // Photon�ŊǗ������PlayerID���X�g
            private int[] playerIDs;    // �Ǝ��ŊǗ�����PlayerID���X�g

            // �J�X�^���v���p�e�B�ɗp����L�[�̒ʐM�ʂ��팸���邽�߂̃N���X
            private CustomPropertyKey customPropertyKey = new CustomPropertyKey();

            public OnlineDataStorage()
            {
                playerIDs = new int[]{ 0, 1 };
                players = PhotonNetwork.PlayerList;

                // �J�X�^���v���p�e�B��hashtable���쐬���Ă���
                foreach (Player player in players)
                {
                    var ht = new ExitGames.Client.Photon.Hashtable();
                    player.SetCustomProperties(ht);
                }
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }

            /// <summary>
            /// Photon��PlayerList�ɑΉ�����playerID�z���ݒ肵�܂��B
            /// </summary>
            /// <param name="playersID">�ݒ肷��PlayerID�z��B</param>
            public OnlineDataStorage(int[] playerIDs)
            {
                this.playerIDs = playerIDs;
                players = PhotonNetwork.PlayerList;

                // �J�X�^���v���p�e�B��hashtable���쐬���Ă���
                foreach (Player player in players)
                {
                    var ht = new ExitGames.Client.Photon.Hashtable();
                    player.SetCustomProperties(ht);
                }
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }



            /// <summary>
            /// playerID�z���Photon��PlayerList��Ή������Đݒ肵�܂��B
            /// </summary>
            /// <param name="playerIDs">�ݒ肷��PlayerID�z��B</param>
            /// <param name="players">�ݒ肷��Photon��PlayerList�z��B</param>
            public OnlineDataStorage(int[] playerIDs, Player[] players)
            {
                this.playerIDs = playerIDs;
                this.players = players;

                /*// �J�X�^���v���p�e�B��hashtable���쐬���Ă���
                foreach (Player player in players)
                {
                    var ht = new ExitGames.Client.Photon.Hashtable();
                    player.SetCustomProperties(ht);
                }
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);*/
            }

            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͉����s���܂���B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă����ꍇ��false�B</returns>
            public bool Add(Owner owner, string key, int value)
            {
                return AddToCustomProperty<int>(owner, key, value);
            }

            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͉����s���܂���B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă����ꍇ��false�B</returns>
            public bool Add(Owner owner, string key, float value)
            {
                return AddToCustomProperty<float>(owner, key, value);
            }

            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͉����s���܂���B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă����ꍇ��false�B</returns>
            public bool Add(Owner owner, string key, string value)
            {
                return AddToCustomProperty<string>(owner, key, value);
            }



            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͒l���X�V���܂��B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă���A�l���X�V�����ꍇ��false�B</returns>
            public bool AddAndUpdate(Owner owner, string key, int value)
            {
                return AddAndUpdateToCustomProperty<int>(owner, key, value);
            }

            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͒l���X�V���܂��B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă���A�l���X�V�����ꍇ��false�B</returns>
            public bool AddAndUpdate(Owner owner, string key, float value)
            {
                return AddAndUpdateToCustomProperty<float>(owner, key, value);
            }

            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͒l���X�V���܂��B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă���A�l���X�V�����ꍇ��false�B</returns>
            public bool AddAndUpdate(Owner owner, string key, string value)
            {
                return AddAndUpdateToCustomProperty<string>(owner, key, value);
            }



            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͉����s���܂���B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă����ꍇ��false�B</returns>
            private bool AddToCustomProperty<T>(Owner owner, string key, T value)
            {
                bool flg;
                object outValue;
                switch(owner)
                {
                    case Owner.ROOM:
                        // ���[�����擾����
                        var room = PhotonNetwork.CurrentRoom;

                        // �V�����L�[������ł������Ȃ�A�J�X�^���v���p�e�B�ɒl��ǉ�����
                        var roomTable = room.CustomProperties;
                        string roomHashedKey = customPropertyKey.Key(key);
                        flg = roomTable.TryGetValue(roomHashedKey, out outValue);
                        if (flg) return false;
                        else roomTable.Add(roomHashedKey, value);
                        room.SetCustomProperties(roomTable);
                        return true;

                    default:
                        // Player���擾����
                        var player = ConvertOwner2Player(owner);

                        // �V�����L�[������ł������Ȃ�A�J�X�^���v���p�e�B�ɒl��ǉ�����
                        var playerTable = player.CustomProperties;
                        string playerHashedKey = customPropertyKey.Key(key);
                        flg = playerTable.TryGetValue(playerHashedKey, out outValue);
                        if (flg) return false;
                        else playerTable.Add(playerHashedKey, value);
                        player.SetCustomProperties(playerTable);
                        return true;
                }
            }

            /// <summary>
            /// �l���T�[�o��ɕۑ����܂��B���Ƀf�[�^�����݂��Ă����ꍇ�͒l���X�V���܂��B
            /// </summary>
            /// <param name="owner">�l�̏��L�ҁB</param>
            /// <param name="key">�l�̃L�[�B</param>
            /// <param name="value">�ۑ�����l�B</param>
            /// <returns>�l�𐳏�ɕۑ��ł����ꍇ��true�A���Ƀf�[�^�����݂��Ă���A�l���X�V�����ꍇ��false�B</returns>
            private bool AddAndUpdateToCustomProperty<T>(Owner owner, string key, T value)
            {
                object outValue;
                bool flg;
                switch (owner)
                {
                    case Owner.ROOM:
                        // ���[�����擾����
                        var room = PhotonNetwork.CurrentRoom;

                        // �V�����L�[������ł������Ȃ�J�X�^���v���p�e�B�ɒl��ǉ����A���ɂ������Ȃ�X�V����
                        var roomTable = room.CustomProperties;
                        string roomHashedKey = customPropertyKey.Key(key);
                        flg = roomTable.TryGetValue(roomHashedKey, out outValue);
                        if (flg) roomTable[roomHashedKey] = value;
                        else roomTable.Add(roomHashedKey, value);
                        room.SetCustomProperties(roomTable);
                        return !flg;

                    default:
                        // Player���擾����
                        var player = ConvertOwner2Player(owner);

                        // �V�����L�[������ł������Ȃ�J�X�^���v���p�e�B�ɒl��ǉ����A���ɂ������Ȃ�X�V����
                        var playerTable = player.CustomProperties;
                        string playerHashedKey = customPropertyKey.Key(key);
                        flg = playerTable.TryGetValue(playerHashedKey, out outValue);
                        if (flg) playerTable[playerHashedKey] = value;
                        else playerTable.Add(playerHashedKey, value);
                        player.SetCustomProperties(playerTable);
                        return !flg;
                }
            }

            /// <summary>
            /// ����̏��L�҂Ɏw�肵���L�[�����񂪓o�^����Ă��邩���m�F���܂��B
            /// </summary>
            /// <typeparam name="T">�L�[������ƑΉ����镶����̃f�[�^�^�B</typeparam>
            /// <param name="owner">�m�F�Ώۂ̏��L�ҁB</param>
            /// <param name="key">�m�F�Ώۂ̃L�[������B</param>
            /// <returns>�������f�[�^�^�Ńf�[�^�����݂���ꍇ��true�A�L�[�����񂪓o�^����Ă��Ȃ��ꍇ�A���L�҂����̃L�[����������L���Ă��Ȃ��ꍇ�A�f�[�^�^���قȂ�ꍇ��false�B</returns>
            private bool CheckDataExisted<T>(Owner owner, string key)
            {
                // �f�[�^���擾����B���̂Ƃ��A�f�[�^���L�[�Ǘ��N���X�ɓo�^����Ă��Ȃ��ꍇ��false��ԋp����
                string hashedKey = customPropertyKey.Key(key);
                object outValue;
                switch(owner)
                {
                    case Owner.ROOM:
                        Debug.Log("try = " + PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(hashedKey, out outValue));
                        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(hashedKey, out outValue)) return false;
                        break;

                    default:
                        var player = ConvertOwner2Player(owner);
                        if (!player.CustomProperties.TryGetValue(hashedKey, out outValue)) return false;
                        break;
                }

                // �f�[�^���w�肵���f�[�^�^���ǂ������m�F����B�w�肵���f�[�^�^�ł���ꍇ��true�A�قȂ�ꍇ��false��ԋp����
                return outValue is T;

                /*bool checkFlg;  // �f�[�^�̑��݂���уf�[�^�^�̔��f���s���ϐ�

                // �w��̃v���C���[�܂��̓��[���Ƀf�[�^���o�^����Ă��Ȃ��ꍇ�܂��̓f�[�^�^���قȂ�ꍇ��false��ԋp����
                switch(owner)
                {
                    case Owner.ROOM:
                        // �l���擾�������f�[�^�^�ƈ�v���Ă��邩���m�F����
                        checkFlg = PhotonNetwork.CurrentRoom.CustomProperties[propertyKey] is T;
                        break;
                    default:
                        // Player���擾����
                        var player = ConvertOwner2Player(owner);

                        // �l���擾�������f�[�^�^�ƈ�v���Ă��邩���m�F����
                        checkFlg = player.CustomProperties[propertyKey] is T;
                        break;
                }

                return checkFlg;*/
            }



            /// <summary>
            /// �T�[�o�[��ɕۑ������l���擾���܂��Bint,float,string�̒�����f�[�^�^���w�肷��K�v������܂��B
            /// </summary>
            /// <typeparam name="T">�擾����l�̃f�[�^�^�B</typeparam>
            /// <param name="owner">�擾����l�̏��L�ҁB</param>
            /// <param name="key">�擾����l�����ʂ���L�[������B</param>
            /// <returns>�T�[�o�[����擾�����l�B</returns>
            public T Get<T>(Owner owner, string key)
            {
                // �f�[�^�^��int,float,string�̂ǂꂩ�ł��邩���`�F�b�N
                bool typeFlg = false;
                typeFlg |= (typeof(T) == typeof(int));
                typeFlg |= (typeof(T) == typeof(float));
                typeFlg |= (typeof(T) == typeof(string));

                // �f�[�^�^���������Ȃ������ꍇ�A��O�𓊂���
                if (!typeFlg)
                {
                    throw new System.ArgumentException(string.Format(
                        $"OnlineDataStorage���疳���ȃf�[�^�^�Œl���擾���悤�Ƃ��܂����B" +
                        $"\nGet<{typeof(T)}>({owner.ToString()}, {key})��<{typeof(T)}>�^��OnlineDataStorage�ɑΉ����Ă��܂���B"));
                }

                // �T�[�o�[��Ƀf�[�^�����݂��邩�m�F���A���݂��Ȃ���Η�O�𓊂���
                bool savedFlg = CheckDataExisted<T>(owner, key);
                if (!savedFlg)
                {
                    throw new System.NullReferenceException(string.Format(
                        $"OnlineDataStorage���瑶�݂��Ȃ����A�قȂ�f�[�^�^�Ńf�[�^���擾���悤�Ƃ��܂����B" +
                        $"\nGet<{typeof(T)}>({owner.ToString()}, {key})�ɑΉ�����<{typeof(T)}>�^�̃f�[�^�͕ۑ�����Ă��܂���B"));
                }

                // �f�[�^���擾����B
                return GetFromCustomProperty<T>(owner, key);

            }

            /// <summary>
            /// �l���T�[�o�[�ォ��擾���܂��B�����\�b�h����Ă΂�邽�߁A�擾�ł��Ȃ������ꍇ�͍l������Ă��܂���B
            /// </summary>
            /// <typeparam name="T">�擾����l�̃f�[�^�^�B</typeparam>
            /// <param name="owner">�擾����l�̏��L�ҁB</param>
            /// <param name="key">�擾����l�̃L�[������B</param>
            /// <returns>�擾�����l�B</returns>
            private T GetFromCustomProperty<T>(Owner owner, string key)
            {
                T returnValue;

                switch (owner)
                {
                    case Owner.ROOM:
                        // ���[�����擾����
                        var room = PhotonNetwork.CurrentRoom;

                        // �J�X�^���v���p�e�B����l���擾����
                        var roomTable = room.CustomProperties;
                        returnValue = (T)roomTable[customPropertyKey.Key(key)];
                        break;
                    default:
                        // Player���擾����
                        var player = ConvertOwner2Player(owner);

                        // �J�X�^���v���p�e�B����l���擾����
                        var playerTable = player.CustomProperties;
                        returnValue = (T)playerTable[customPropertyKey.Key(key)];
                        break;
                }

                return returnValue;
            }



            /// <summary>
            /// Owner�̂���"ROOM"�ȊO��Ή�����Player�^�ɕϊ����܂��B
            /// </summary>
            /// <param name="owner">�ϊ��O��Owner�̒l�B</param>
            /// <returns>�ϊ����Player�̒l�B</returns>
            private Player ConvertOwner2Player(Owner owner)
            {
                // �v���C���[ID��z��̓Y�����ɕϊ�����
                int workInt = 0;
                while ((int)owner == playerIDs[workInt]) workInt++;

                // �z�񂩂�Player�f�[�^���Q�Ƃ���
                Player player = players[workInt];
                return player;
            }
        }

        /// <summary>
        /// �J�X�^���v���p�e�B�̃L�[�Ǘ����s���N���X�B93��ނ܂ŃL�[��o�^�\�B
        /// </summary>
        public class CustomPropertyKey : MonoBehaviourPunCallbacks
        {
            // ���[�U�[���ݒ肵���L�[��������n�b�V�������ĕʂ̕�����ɂ���B
            // ����͈Í����ł͂Ȃ��A�ʐM�ʍ팸�̂��߂ɗp������B
            // �g�p���@�́A�g�p�������L�[�������Key�֐��ɂ����邾���ł���B

            // �n�b�V�����̂��߂̂��́B�n�b�V�������ĎZ�o�����l���L�[�Ƃ���B
            SHA256 sha256;

            public CustomPropertyKey()
            {
                // �n�b�V���֐��̐ݒ�
                sha256 = SHA256.Create();
            }

            /// <summary>
            /// ���[�U�[�ݒ�̕�����L�[��ǉ����܂��B���ɓ����̃L�[�����݂����ꍇ��v�f��������̏ꍇ�͕ύX����܂���B�i���ݎg�p���Ă��Ȃ��j
            /// </summary>
            /// <param name="key">�ǉ�����L�[������B</param>
            /// <returns>�ǉ��ɐ��������ꍇ��true�A�f�[�^�����ɑ��݂����ꍇ��v�f��������̏ꍇ��false�B</returns>
            /*public bool AddStringKey(string key)
            {
                Debug.Log("addM.key = " + key);

                // �v�f��������Ȃ�false��ԋp����
                if (userKeys.Count + ASCII_MIN == ASCII_MAX) return false;

                // �����̃L�[�𑖍����A���������ꍇ��false��ԋp����
                foreach (string k in userKeys)
                {
                    if (key == k) return false;
                }

                // ���X�g�ɃL�[��ǉ�����
                userKeys.Add(key);

                // ���v���C���[�̃��X�g�ɂ��L�[��ǉ����A���X�g�𓯊�����
                //SyncAllPlayerCustomPropertyKey(key);
                return true;
            }*/

            /// <summary>
            /// ���[�U�[�����O�ɒǉ������L�[��������A�ʐM�p�̃L�[�l�ɕϊ����܂��B�w�肵���L�[�����񂪑��݂��Ȃ��ꍇ�A"error"�̕������ԋp���܂��B
            /// </summary>
            /// <param name="key">�ϊ��O�̃L�[������B</param>
            /// <returns>�ϊ���̕�����B�L�[�����񂪑��݂��Ȃ��ꍇ��"error"�̕�����B</returns>
            public string Key(string key)
            {
                // �ϊ���̕����f�[�^�T�C�Y���傫���Ȃ��Ă��܂����߁A�����_�ł͂��̂܂ܕԋp
                return key;

                // �L�[�������byte�^�ɕϊ�����
                byte[] encoded = Encoding.UTF8.GetBytes(key);

                // byte�^�ɕϊ������L�[��������n�b�V��������
                byte[] hash = sha256.ComputeHash(encoded);

                // �n�b�V��������byte�l�𕶎���ɕϊ�����
                string hashedKey = Encoding.GetEncoding("shift-jis").GetString(hash);

                // �n�b�V���������L�[�������ԋp����
                return hashedKey;
            }
        }

        /// <summary>
        /// �J�X�^���v���p�e�B�̏��L�҂�ݒ肷�邽�߂�enum�B
        /// </summary>
        public enum Owner
        {
            PLAYER1,
            PLAYER2,
            PLAYER3,
            PLAYER4,
            ROOM
        }
    }
}