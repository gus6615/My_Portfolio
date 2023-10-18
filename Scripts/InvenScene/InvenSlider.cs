using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenSlider : MonoBehaviour
{
    private Slider slider;
    static public long num;

    // Start is called before the first frame update
    void Start()
    {
        slider = this.GetComponent<Slider>();
    }

    private void Update()
    {
        num = (int)slider.value / 2 * 2;
        if (num < 0) num = 0;

        slider.value = num;
    }
}
