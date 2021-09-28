using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBoolManager : MonoBehaviour
{
    public bool isSelectKaito { get; set; } = true;
    public bool isSelectSeira { get; set; } = true;
    public bool isSelectYu { get; set; } = true;
    public bool isSelectMitsuha { get; set; } = true;

    [SerializeField] GameObject kaitoButton;
    [SerializeField] GameObject seiraButton;
    [SerializeField] GameObject yuButton;
    [SerializeField] GameObject mitsuhaButton;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    private void Update()
    {
        SeiraButtonColor();
    }

    

    private void SeiraButtonColor()
    {
        if(!isSelectSeira)
        {
            seiraButton.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }

    private void YuButtonColor()
    {
        if (!isSelectYu)
        {
            yuButton.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }

    private void MitsuhaButtonColor()
    {
        if (!isSelectMitsuha)
        {
            mitsuhaButton.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }

}
