using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public static Color blueColor = new Color(0f, 0.6f, 1f, 1f);
    public static Color redColor = new Color(1f, 0f, 0.2f, 1f);
    public Text text;
    public GameObject target;
    private float distance;

    private void OnEnable()
    {
        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        distance = 0f;
        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        while(text.color.a > 0f)
        {
            Color color = text.color;
            color.a -= Time.deltaTime;
            text.color = color;
            distance += Time.deltaTime * 0.75f;
            this.transform.position = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * (1f + distance));

            yield return new WaitForSeconds(Time.deltaTime);
        }

        ObjectPool.ReturnObject<DamageText>(13, this);
    }
}
