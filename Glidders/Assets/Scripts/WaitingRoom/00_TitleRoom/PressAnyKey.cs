using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressAnyKey : MonoBehaviour
{
    Color color = new Vector4(0,0,0,1);
    Image image;
    bool up;
    void Start()
    {
        image = this.GetComponent<Image>();
    }

    void Update()
    {
        if (image.color.a >= 1) { up = false; }
        if (image.color.a <= 0.2f) { up = true; }
        if (up == true) { image.color += color * Time.deltaTime; }
        if (up == false) { image.color -= color * Time.deltaTime; }
    }
}
