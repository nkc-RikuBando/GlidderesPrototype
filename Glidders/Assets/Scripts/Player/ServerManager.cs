using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Manager;

namespace Glidders
{
    namespace Manager
    {
        public class ServerManager : MonoBehaviour
        {
            [SerializeField] private GameObject managerObject;
            [SerializeField] private GameObject objects;

            [SerializeField] private GameObject[] players = new GameObject[Rule.maxPlayerCount];

            private CoreManager coreManager;
            private void Awake()
            {
                managerObject = Instantiate(managerObject);

                coreManager = managerObject.GetComponent<CoreManager>();

                for (int i = 0;i < players.Length;i++)
                {
                    players[i] = Instantiate(players[i]);
                    coreManager.CharacterDataReceber(players[i], i);
                }

                objects = GameObject.Find("");
            }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }

    }
}