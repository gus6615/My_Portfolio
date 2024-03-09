using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpInfo : MonoBehaviour
{
    private static Color[] exp_colors = { new Color(0.4f, 1f, 0.4f), new Color(0.4f, 0.8f, 1f), new Color(1f, 0.4f, 0.8f) };

    [SerializeField] private Text text;
    private Color color;
    private float startTime, fadeSpeed;
    private bool isStart;
    public int amount;
    public int type;

    // Start is called before the first frame update
    void Start()
    {
        startTime = 0.5f;
        fadeSpeed = 1.25f;

        this.transform.position = Camera.main.WorldToScreenPoint(PlayerScript.instance.transform.position + Vector3.up * 0.75f);
        color = exp_colors[type];
        text.color = color;
        switch (type)
        {
            case 0: text.text = "+ " + GameFuction.GetNumText(amount) + " EXP"; break;
            case 1: text.text = "+ " + GameFuction.GetNumText(amount) + " EXP (x2)"; break;
            case 2: text.text = "+ " + GameFuction.GetNumText(amount) + " EXP (x4)"; break;
        }

        StartCoroutine("FadeStart");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Camera.main.WorldToScreenPoint(PlayerScript.instance.transform.position + Vector3.up * 0.75f);

        if (isStart)
        {
            if (color.a > 0f)
            {
                color.a -= Time.deltaTime * fadeSpeed;
                text.color = color;
            }
            else
            {
                color.a = 0f;
                text.color = color;
                isStart = false;
                Destroy(this.gameObject);
            }
        }
    }

    IEnumerator FadeStart()
    {
        yield return new WaitForSeconds(startTime);
        isStart = true;
    }
}
