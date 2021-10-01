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

    [SerializeField] Text kaitoText;
    [SerializeField] Text seiraText;
    [SerializeField] Text yuText;
    [SerializeField] Text mitsuhaText;

    private void Start()
    {
        
    }

    private void Update()
    {
        NotKaitoSelect();
        NotSeiraSelect();
        NotYuSelect();
        NotMitsuhaSelect();
    }

    public void NotKaitoSelect()
    {
        if (!isSelectKaito)
        {
            kaitoButton.color = new Color(rColor, gColor, bColor);
            kaitoText.color = new Color(rColor, gColor, bColor);
        }
        else
        {
            kaitoButton.color = new Color(defaultColor, defaultColor, defaultColor);
            kaitoText.color = new Color(defaultColor, defaultColor, defaultColor);
        }
            

    }

    public void NotSeiraSelect()
    {
        if (!isSelectSeira)
        {
            seiraButton.color = new Color(rColor, gColor, bColor);
            seiraText.color = new Color(rColor, gColor, bColor);
        }
        else
        {
            seiraButton.color = new Color(defaultColor, defaultColor, defaultColor);
            seiraText.color = new Color(defaultColor, defaultColor, defaultColor);
        }

    }

    public void NotYuSelect()
    {
        if (!isSelectYu)
        {
            yuButton.color = new Color(rColor, gColor, bColor);
            yuText.color = new Color(rColor, gColor, bColor);
        }
        else
        {
            yuButton.color = new Color(defaultColor, defaultColor, defaultColor);
            yuText.color = new Color(defaultColor, defaultColor, defaultColor);
        }
            

    }

    public void NotMitsuhaSelect()
    {
        if (!isSelectMitsuha)
        {
            mitsuhaButton.color = new Color(rColor, gColor, bColor);
            mitsuhaText.color = new Color(rColor, gColor, bColor);
        }
        else
        {
            mitsuhaButton.color = new Color(defaultColor, defaultColor, defaultColor);
            mitsuhaText.color = new Color(defaultColor, defaultColor, defaultColor);
        }
            

    }
}
