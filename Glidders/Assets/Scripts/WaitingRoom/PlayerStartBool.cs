using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Glidders
{
    public class PlayerStartBool : MonoBehaviourPunCallbacks
    {
        PhotonView view;
        public bool[] gameStartBool = new bool[Rule.maxPlayerCount];

        public static int myPlayerNum;
        public static string battleStageField;
        private int okPlayerCount = 0;
        bool isStart = false;

        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();

            view.RPC(nameof(PlayerStartBoolCount), RpcTarget.AllBufferedViaServer);//RPC���\�b�h���Ăԁ@PlayerStartBoolCount

            Debug.Log("myPlayerNum = " + myPlayerNum);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        [PunRPC]
        public void PlayerStartBoolCount()
        {
            gameStartBool[myPlayerNum] = isStart; //�����Ă����玩���̔ԍ��̔z��̏ꏊ��false�������
        }

        public void CallMethod(int myPlayerNum) //���̃X�N���v�g����RPC���\�b�h���ĂԂ��߂̃��\�b�h
        {
            view.RPC(nameof(StartConf), RpcTarget.All,(myPlayerNum)); //RPC���\�b�h���Ăԁ@StartConf
        }

        [PunRPC]
        public void StartConf(int cheakPlayerNum) //�����J�n�ł��邩���f���郁�\�b�h
        {
            gameStartBool[cheakPlayerNum] = true; //�����̔ԍ��̔z���true�ɂ���
            //Debug.Log("PhotonNetwork.PlayerList.Length = " + PhotonNetwork.PlayerList.Length);
            for(int i = 0; i <= PhotonNetwork.PlayerList.Length -1;i++) //for���őS��true�����f����
            {
                //Debug.Log("gameStartBool[" + i + "]" + "=" + gameStartBool[i]);
                if (gameStartBool[i] == true) //���f�����l��true��������
                {
                    //Debug.Log("�N���A");
                    ++okPlayerCount; //count����������
                    //Debug.Log(okPlayerCount);
                }
            }
            Debug.Log(PhotonNetwork.PlayerList.Length);

            if (PhotonNetwork.PlayerList.Length >= 2) //�����o�[��2�l�ȏア��K�v�����邩��1�l�Ȃ�V�[���ړ��ł��Ȃ�
            {
                if (okPlayerCount == PhotonNetwork.PlayerList.Length) //count���v���C�l���ƈ�v������
                {
                    //SingletonData.SetMatchingPlayerData();
                    SceneManager.LoadScene(battleStageField); //�V�[���ړ�
                }
                else okPlayerCount = 0; //���Ȃ�������J�E���g��0�ɂȂ�
            }
            else okPlayerCount = 0;
            

        }
    }
}

