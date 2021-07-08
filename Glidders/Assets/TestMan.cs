using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Glidders.Character;
using Glidders;

public class TestMan : MonoBehaviour
{
    // Start is called before the first frame update
    public SkillScriptableObjectWork a;
    void Start()
    {
        /*//Debug.Log("name = " + a.skillName);

        //foreach (bool offset in a.selectRangeArray)
        //{
        //    Debug.Log("rowOffset = " + offset);
        //}

        Debug.Log(a.skillName);

        int cnt = 0;
        Debug.Log(a.selectGridList.Count);
        foreach (bool grid in a.selectRangeArray)
        {
            if(grid)cnt++;
        }
        Debug.Log(cnt);*/

        FieldIndex testA = FieldIndex.zero;
        testA += FieldIndexOffset.right;
        Debug.Log(testA.row + " " + testA.column);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
