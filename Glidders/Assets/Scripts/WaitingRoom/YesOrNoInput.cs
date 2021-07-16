using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YesOrNoInput : MonoBehaviour
{
    private int inputNumber = 0;
    private int selectNumber = 0;

    public void SetInputNumber(int setNumber)
    {
        inputNumber = setNumber;
    }

    public int GetInputNumber()
    {
        return inputNumber;
    }

    public void SetSelectNumber(int setNumber)
    {
        selectNumber = setNumber;
    }

    public int GetSelectNumber()
    {
        return selectNumber;
    }
}
