using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public Image[] images;
    public Text[] texts;
    public float goalRate;
    public float goalSize;
    public bool isReSize;
    public float defaultSize;
    private bool isEffectOn;
    private Vector3 signVec = Vector3.zero;

    private void OnEnable()
    {
        isEffectOn = false;
        defaultSize = Mathf.Abs(this.transform.localScale.x);
        if (signVec == Vector3.zero)
            signVec = new Vector3(Mathf.Sign(transform.localScale.x), Mathf.Sign(transform.localScale.y), Mathf.Sign(transform.localScale.z));
    }

    private void OnDisable()
    {
        StopCoroutine("EffectAllRewardButton");
        ChangeAlpha(1f);
        this.transform.localScale = signVec * defaultSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEffectOn)
        {
            StopCoroutine("EffectAllRewardButton");
            StartCoroutine("EffectAllRewardButton");
        }
    }

    IEnumerator EffectAllRewardButton()
    {
        float lerpRate = 0f;
        float rate, size;
        isEffectOn = true;

        // 밝아지는 부분
        while (lerpRate <= 1)
        {
            rate = Mathf.Lerp(goalRate, 1f, lerpRate);
            ChangeAlpha(rate);
            size = Mathf.Lerp(defaultSize, goalSize, lerpRate);
            ChangeSize(size);
            lerpRate += Time.deltaTime * 2f;

            yield return null;
        }

        ChangeAlpha(1f);
        ChangeSize(goalSize);
        lerpRate = 0f;

        // 어두워지는 부분
        while (lerpRate <= 1)
        {
            rate = Mathf.Lerp(1f, goalRate, lerpRate);
            ChangeAlpha(rate);
            size = Mathf.Lerp(goalSize, defaultSize, lerpRate);
            ChangeSize(size);
            lerpRate += Time.deltaTime * 2f;

            yield return null;
        }

        ChangeAlpha(goalRate);
        ChangeSize(defaultSize);
        isEffectOn = false;
    }

    private void ChangeAlpha(float data)
    {
        Color color;
        if(sprites != null)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                color = sprites[i].color;
                color.a = data;
                sprites[i].color = color;
            }
        }

        if(images != null)
        {
            for (int i = 0; i < images.Length; i++)
            {
                color = images[i].color;
                color.a = data;
                images[i].color = color;
            }
        }

        if(texts != null)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                color = texts[i].color;
                color.a = data;
                texts[i].color = color;
            }
        }
    }

    private void ChangeSize(float data)
    {
        if (!isReSize) 
            return;
        this.transform.localScale = signVec * data;
    }
}
