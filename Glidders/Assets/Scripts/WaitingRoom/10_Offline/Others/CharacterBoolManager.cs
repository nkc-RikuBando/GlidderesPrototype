using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBoolManager : MonoBehaviour
{
    public bool isSelectKaito { get; set; } = true;
    public bool isSelectSeira { get; set; } = true;
    public bool isSelectYu { get; set; } = true;
    public bool isSelectMitsuha { get; set; } = true;


    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        
    }
}
