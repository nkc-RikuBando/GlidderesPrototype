using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Photon;
using Photon.Pun;
using Photon.Realtime;

public class OnlineDataStorageDebugger : MonoBehaviourPunCallbacks
{
    OnlineDataStorage storage;

    // Start is called before the first frame update
    void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        //PhotonNetwork.ConnectUsingSettings();

        int[] playerIDs = { 0, 1 };
        storage = new OnlineDataStorage(playerIDs, PhotonNetwork.PlayerList);
    }

    /*/ マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);

        int[] playerIDs = { 0, 1 };
        storage = new OnlineDataStorage(playerIDs, PhotonNetwork.PlayerList);
    }*/

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            storage.Add(Owner.ROOM, "testValue", 100);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            int v = storage.Get<int>(Owner.ROOM, "testValue");
            Debug.Log("testValue = " + v);
            int x = storage.Get<int>(Owner.ROOM, "dummy");
        }
    }

    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable i_propertiesThatChanged)
    {
        Debug.Log("Changed!");
    }
}
