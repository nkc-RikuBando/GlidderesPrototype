using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBoolManager : MonoBehaviour
{
    public bool isSelectKaito { get; set; } = true;
    public bool isSelectSeira { get; set; } = true;
    public bool isSelectYu { get; set; } = true;
    public bool isSelectMitsuha { get; set; } = true;


    private SpriteRenderer spriteRenderer;

    [SerializeField] float rColor = 0.5f;
    [SerializeField] float gColor = 0.5f;
    [SerializeField] float bColor = 0.5f;
    private int defaultColor = 1;


    [SerializeField] Image kaitoButton;
    [SerializeField] Image seiraButton;
    [SerializeField] Image yuButton;
    [SerializeField] Image mitsuhaButton;

    private void Start()
    {
        
    }

    private void Update()
    {
        NotKaitoButtoon();
        NotSeiraButton();
        NotYuButton();
        NotMitsuhaButton();
    }

    public void NotKaitoButtoon()
    {
        if (!isSelectKaito)
        {
            kaitoButton.color = new Color(rColor, gColor, bColor);
        }
        else
            kaitoButton.color = new Color(defaultColor,defaultColor,defaultColor);


    }

    public void NotSeiraButton()
    {
        if (!isSelectSeira)
        {
            seiraButton.color = new Color(rColor, gColor, bColor);
        }
        else
            seiraButton.color = new Color(defaultColor, defaultColor, defaultColor);

    }

    public void NotYuButton()
    {
        if (!isSelectYu)
        {
            yuButton.color = new Color(rColor, gColor, bColor);
        }
        else
            yuButton.color = new Color(defaultColor, defaultColor, defaultColor);

    }

    public void NotMitsuhaButton()
    {
        if (!isSelectMitsuha)
        {
            mitsuhaButton.color = new Color(rColor, gColor, bColor);
        }
        else
            mitsuhaButton.color = new Color(defaultColor, defaultColor, defaultColor);

    }
}
