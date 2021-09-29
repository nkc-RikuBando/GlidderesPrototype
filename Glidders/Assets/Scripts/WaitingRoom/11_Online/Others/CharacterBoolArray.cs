using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBoolArray : MonoBehaviour
{
    public bool[] characterBool = new bool[10];

    public bool isKaito { get; set; }
    public bool isSeira { get; set; }
    public bool isYu { get; set; }
    public bool isMitsuha { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i < characterBool.Length; i++)
        {
            characterBool[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
