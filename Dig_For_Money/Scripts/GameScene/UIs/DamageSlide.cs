using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageSlide : MonoBehaviour
{
    public Slider slider;
    public GameObject target;
    public float height;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * height);
        if(!target.activeSelf)
            ObjectPool.ReturnObject<DamageSlide>(14, this);
    }
}
