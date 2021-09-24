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
            private Player[] players;   // Photonで管理されるPlayerIDリスト
            private int[] playerIDs;    // 独自で管理するPlayerIDリスト

            // カスタムプロパティに用いるキーの通信量を削減するためのクラス
            private CustomPropertyKey customPropertyKey = new CustomPropertyKey();

            public OnlineDataStorage()
            {
                playerIDs = new int[]{ 0, 1 };
                players = PhotonNetwork.PlayerList;

                // カスタムプロパティのhashtableを作成しておく
                foreach (Player player in players)
                {
                    var ht = new ExitGames.Client.Photon.Hashtable();
                    player.SetCustomProperties(ht);
                }
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }

            /// <summary>
            /// PhotonのPlayerListに対応するplayerID配列を設定します。
            /// </summary>
            /// <param name="playersID">設定するPlayerID配列。</param>
            public OnlineDataStorage(int[] playerIDs)
            {
                this.playerIDs = playerIDs;
                players = PhotonNetwork.PlayerList;

                // カスタムプロパティのhashtableを作成しておく
                foreach (Player player in players)
                {
                    var ht = new ExitGames.Client.Photon.Hashtable();
                    player.SetCustomProperties(ht);
                }
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }



            /// <summary>
            /// playerID配列とPhotonのPlayerListを対応させて設定します。
            /// </summary>
            /// <param name="playerIDs">設定するPlayerID配列。</param>
            /// <param name="players">設定するPhotonのPlayerList配列。</param>
            public OnlineDataStorage(int[] playerIDs, Player[] players)
            {
                this.playerIDs = playerIDs;
                this.players = players;

                /*// カスタムプロパティのhashtableを作成しておく
                foreach (Player player in players)
                {
                    var ht = new ExitGames.Client.Photon.Hashtable();
                    player.SetCustomProperties(ht);
                }
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);*/
            }

            /// <summary>
            /// 値をサーバ上に保存します。既にデータが存在していた場合は何も行いません。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在していた場合はfalse。</returns>
            public bool Add(Owner owner, string key, int value)
            {
                return AddToCustomProperty<int>(owner, key, value);
            }

            /// <summary>
            /// 値をサーバ上に保存します。既にデータが存在していた場合は何も行いません。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在していた場合はfalse。</returns>
            public bool Add(Owner owner, string key, float value)
            {
                return AddToCustomProperty<float>(owner, key, value);
            }

            /// <summary>
            /// 値をサーバ上に保存します。既にデータが存在していた場合は何も行いません。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在していた場合はfalse。</returns>
            public bool Add(Owner owner, string key, string value)
            {
                return AddToCustomProperty<string>(owner, key, value);
            }



            /// <summary>
            /// 値をサーバ上に保存します。既にデータが存在していた場合は値を更新します。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在しており、値を更新した場合はfalse。</returns>
            public bool AddAndUpdate(Owner owner, string key, int value)
            {
                return AddAndUpdateToCustomProperty<int>(owner, key, value);
            }

            /// <summary>
            /// 値をサーバ上に保存します。既にデータが存在していた場合は値を更新します。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在しており、値を更新した場合はfalse。</returns>
            public bool AddAndUpdate(Owner owner, string key, float value)
            {
                return AddAndUpdateToCustomProperty<float>(owner, key, value);
            }

            /// <summary>
            /// 値をサーバ上に保存します。既にデータが存在していた場合は値を更新します。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在しており、値を更新した場合はfalse。</returns>
            public bool AddAndUpdate(Owner owner, string key, string value)
            {
                return AddAndUpdateToCustomProperty<string>(owner, key, value);
            }



            /// <summary>
            /// 値をサーバ上に保存します。既にデータが存在していた場合は何も行いません。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在していた場合はfalse。</returns>
            private bool AddToCustomProperty<T>(Owner owner, string key, T value)
            {
                bool flg;
                object outValue;
                switch(owner)
                {
                    case Owner.ROOM:
                        // ルームを取得する
                        var room = PhotonNetwork.CurrentRoom;

                        // 新しいキー文字列であったなら、カスタムプロパティに値を追加する
                        var roomTable = room.CustomProperties;
                        string roomHashedKey = customPropertyKey.Key(key);
                        flg = roomTable.TryGetValue(roomHashedKey, out outValue);
                        if (flg) return false;
                        else roomTable.Add(roomHashedKey, value);
                        room.SetCustomProperties(roomTable);
                        return true;

                    default:
                        // Playerを取得する
                        var player = ConvertOwner2Player(owner);

                        // 新しいキー文字列であったなら、カスタムプロパティに値を追加する
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
            /// 値をサーバ上に保存します。既にデータが存在していた場合は値を更新します。
            /// </summary>
            /// <param name="owner">値の所有者。</param>
            /// <param name="key">値のキー。</param>
            /// <param name="value">保存する値。</param>
            /// <returns>値を正常に保存できた場合はtrue、既にデータが存在しており、値を更新した場合はfalse。</returns>
            private bool AddAndUpdateToCustomProperty<T>(Owner owner, string key, T value)
            {
                object outValue;
                bool flg;
                switch (owner)
                {
                    case Owner.ROOM:
                        // ルームを取得する
                        var room = PhotonNetwork.CurrentRoom;

                        // 新しいキー文字列であったならカスタムプロパティに値を追加し、既にあったなら更新する
                        var roomTable = room.CustomProperties;
                        string roomHashedKey = customPropertyKey.Key(key);
                        flg = roomTable.TryGetValue(roomHashedKey, out outValue);
                        if (flg) roomTable[roomHashedKey] = value;
                        else roomTable.Add(roomHashedKey, value);
                        room.SetCustomProperties(roomTable);
                        return !flg;

                    default:
                        // Playerを取得する
                        var player = ConvertOwner2Player(owner);

                        // 新しいキー文字列であったならカスタムプロパティに値を追加し、既にあったなら更新する
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
            /// 特定の所有者に指定したキー文字列が登録されているかを確認します。
            /// </summary>
            /// <typeparam name="T">キー文字列と対応する文字列のデータ型。</typeparam>
            /// <param name="owner">確認対象の所有者。</param>
            /// <param name="key">確認対象のキー文字列。</param>
            /// <returns>正しいデータ型でデータが存在する場合はtrue、キー文字列が登録されていない場合、所有者がそのキー文字列を所有していない場合、データ型が異なる場合はfalse。</returns>
            private bool CheckDataExisted<T>(Owner owner, string key)
            {
                // データを取得する。このとき、データがキー管理クラスに登録されていない場合はfalseを返却する
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

                // データが指定したデータ型かどうかを確認する。指定したデータ型である場合はtrue、異なる場合はfalseを返却する
                return outValue is T;

                /*bool checkFlg;  // データの存在およびデータ型の判断を行う変数

                // 指定のプレイヤーまたはルームにデータが登録されていない場合またはデータ型が異なる場合はfalseを返却する
                switch(owner)
                {
                    case Owner.ROOM:
                        // 値が取得したいデータ型と一致しているかを確認する
                        checkFlg = PhotonNetwork.CurrentRoom.CustomProperties[propertyKey] is T;
                        break;
                    default:
                        // Playerを取得する
                        var player = ConvertOwner2Player(owner);

                        // 値が取得したいデータ型と一致しているかを確認する
                        checkFlg = player.CustomProperties[propertyKey] is T;
                        break;
                }

                return checkFlg;*/
            }



            /// <summary>
            /// サーバー上に保存した値を取得します。int,float,stringの中からデータ型を指定する必要があります。
            /// </summary>
            /// <typeparam name="T">取得する値のデータ型。</typeparam>
            /// <param name="owner">取得する値の所有者。</param>
            /// <param name="key">取得する値を識別するキー文字列。</param>
            /// <returns>サーバーから取得した値。</returns>
            public T Get<T>(Owner owner, string key)
            {
                // データ型がint,float,stringのどれかであるかをチェック
                bool typeFlg = false;
                typeFlg |= (typeof(T) == typeof(int));
                typeFlg |= (typeof(T) == typeof(float));
                typeFlg |= (typeof(T) == typeof(string));

                // データ型が正しくなかった場合、例外を投げる
                if (!typeFlg)
                {
                    throw new System.ArgumentException(string.Format(
                        $"OnlineDataStorageから無効なデータ型で値を取得しようとしました。" +
                        $"\nGet<{typeof(T)}>({owner.ToString()}, {key})の<{typeof(T)}>型はOnlineDataStorageに対応していません。"));
                }

                // サーバー上にデータが存在するか確認し、存在しなければ例外を投げる
                bool savedFlg = CheckDataExisted<T>(owner, key);
                if (!savedFlg)
                {
                    throw new System.NullReferenceException(string.Format(
                        $"OnlineDataStorageから存在しないか、異なるデータ型でデータを取得しようとしました。" +
                        $"\nGet<{typeof(T)}>({owner.ToString()}, {key})に対応する<{typeof(T)}>型のデータは保存されていません。"));
                }

                // データを取得する。
                return GetFromCustomProperty<T>(owner, key);

            }

            /// <summary>
            /// 値をサーバー上から取得します。他メソッドから呼ばれるため、取得できなかった場合は考慮されていません。
            /// </summary>
            /// <typeparam name="T">取得する値のデータ型。</typeparam>
            /// <param name="owner">取得する値の所有者。</param>
            /// <param name="key">取得する値のキー文字列。</param>
            /// <returns>取得した値。</returns>
            private T GetFromCustomProperty<T>(Owner owner, string key)
            {
                T returnValue;

                switch (owner)
                {
                    case Owner.ROOM:
                        // ルームを取得する
                        var room = PhotonNetwork.CurrentRoom;

                        // カスタムプロパティから値を取得する
                        var roomTable = room.CustomProperties;
                        returnValue = (T)roomTable[customPropertyKey.Key(key)];
                        break;
                    default:
                        // Playerを取得する
                        var player = ConvertOwner2Player(owner);

                        // カスタムプロパティから値を取得する
                        var playerTable = player.CustomProperties;
                        returnValue = (T)playerTable[customPropertyKey.Key(key)];
                        break;
                }

                return returnValue;
            }



            /// <summary>
            /// Ownerのうち"ROOM"以外を対応するPlayer型に変換します。
            /// </summary>
            /// <param name="owner">変換前のOwnerの値。</param>
            /// <returns>変換後のPlayerの値。</returns>
            private Player ConvertOwner2Player(Owner owner)
            {
                // プレイヤーIDを配列の添え字に変換する
                int workInt = 0;
                while ((int)owner == playerIDs[workInt]) workInt++;

                // 配列からPlayerデータを参照する
                Player player = players[workInt];
                return player;
            }
        }

        /// <summary>
        /// カスタムプロパティのキー管理を行うクラス。93種類までキーを登録可能。
        /// </summary>
        public class CustomPropertyKey : MonoBehaviourPunCallbacks
        {
            // ユーザーが設定したキー文字列をハッシュ化して別の文字列にする。
            // これは暗号化ではなく、通信量削減のために用いられる。
            // 使用方法は、使用したいキー文字列をKey関数にかけるだけである。

            // ハッシュ化のためのもの。ハッシュ化して算出した値をキーとする。
            SHA256 sha256;

            public CustomPropertyKey()
            {
                // ハッシュ関数の設定
                sha256 = SHA256.Create();
            }

            /// <summary>
            /// ユーザー設定の文字列キーを追加します。既に同名のキーが存在した場合や要素数が上限の場合は変更されません。（現在使用していない）
            /// </summary>
            /// <param name="key">追加するキー文字列。</param>
            /// <returns>追加に成功した場合はtrue、データが既に存在した場合や要素数が上限の場合はfalse。</returns>
            /*public bool AddStringKey(string key)
            {
                Debug.Log("addM.key = " + key);

                // 要素数が上限ならfalseを返却する
                if (userKeys.Count + ASCII_MIN == ASCII_MAX) return false;

                // 同名のキーを走査し、見つかった場合はfalseを返却する
                foreach (string k in userKeys)
                {
                    if (key == k) return false;
                }

                // リストにキーを追加する
                userKeys.Add(key);

                // 他プレイヤーのリストにもキーを追加し、リストを同期する
                //SyncAllPlayerCustomPropertyKey(key);
                return true;
            }*/

            /// <summary>
            /// ユーザーが事前に追加したキー文字列を、通信用のキー値に変換します。指定したキー文字列が存在しない場合、"error"の文字列を返却します。
            /// </summary>
            /// <param name="key">変換前のキー文字列。</param>
            /// <returns>変換後の文字列。キー文字列が存在しない場合は"error"の文字列。</returns>
            public string Key(string key)
            {
                // 変換後の方がデータサイズが大きくなってしまうため、現時点ではそのまま返却
                return key;

                // キー文字列をbyte型に変換する
                byte[] encoded = Encoding.UTF8.GetBytes(key);

                // byte型に変換したキー文字列をハッシュ化する
                byte[] hash = sha256.ComputeHash(encoded);

                // ハッシュ化したbyte値を文字列に変換する
                string hashedKey = Encoding.GetEncoding("shift-jis").GetString(hash);

                // ハッシュ化したキー文字列を返却する
                return hashedKey;
            }
        }

        /// <summary>
        /// カスタムプロパティの所有者を設定するためのenum。
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