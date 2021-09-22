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
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        //PhotonNetwork.ConnectUsingSettings();

        int[] playerIDs = { 0, 1 };
        storage = new OnlineDataStorage(playerIDs, PhotonNetwork.PlayerList);
    }

    /*/ �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
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
