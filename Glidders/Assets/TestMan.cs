using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;
using Glidders;

public class TestMan : MonoBehaviour
{
    // Start is called before the first frame update
    public SkillScriptableObject a;
    void Start()
    {
        Debug.Log("name = " + a.skillName);

        foreach (FieldIndexOffset offset in a.gridList)
        {
            Debug.Log("rowOffset = " + offset.rowOffset + ",  columnOffset = " + offset.columnOffset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}