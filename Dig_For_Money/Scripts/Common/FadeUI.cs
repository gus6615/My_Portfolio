using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 해당 스크립트가 가진 이미지와 텍스트를 [idleTime]초 후에 [fadeOutTime]초 동안 모두 투명화 시킵니다.
/// </summary>
public class FadeUI : MonoBehaviour
{
    public UIBox uiBox;
    private List<float> alpha_images, alpha_texts, alpha_tmptexts;
    private float fadeInTime, idleTime, fadeOutTime;

    private void Start()
    {
        alpha_images = new List<float>();
        alpha_texts = new List<float>();
        alpha_tmptexts = new List<float>();

        for (int i = 0; i < uiBox.images.Length; i++)
            uiBox.images[i].color = new Color(1f, 1f, 1f, 0f);
        for (int i = 0; i < uiBox.texts.Length; i++)
            uiBox.texts[i].color = new Color(0f, 0f, 0f, 0f);
        for (int i = 0; i < uiBox.tmp_texts.Length; i++)
            uiBox.tmp_texts[i].color = new Color(0f, 0f, 0f, 0f);
    }
    
    public void SetFadeValues(float _fadeInTime, float _idleTime, float _fadeOutTime)
    {
        fadeInTime = _fadeInTime;
        if (fadeInTime <= 0f)
            fadeInTime = 0.00001f;
        idleTime = _idleTime;
        fadeOutTime = _fadeOutTime;

        alpha_images.Clear();
        for (int i = 0; i < uiBox.images.Length; i++)
        {
            Color color = uiBox.images[i].color;
            alpha_images.Add(color.a);
            color = new Color(color.r, color.g, color.b, 0f);
            uiBox.images[i].color = color;
        }

        alpha_texts.Clear();
        for (int i = 0; i < uiBox.texts.Length; i++)
        {
            Color color = uiBox.texts[i].color;
            alpha_texts.Add(color.a);
            color = new Color(color.r, color.g, color.b, 0f);
            uiBox.texts[i].color = color;
        }

        alpha_tmptexts.Clear();
        for (int i = 0; i < uiBox.tmp_texts.Length; i++)
        {
            Color color = uiBox.tmp_texts[i].color;
            alpha_tmptexts.Add(color.a);
            color = new Color(color.r, color.g, color.b, 0f);
            uiBox.tmp_texts[i].color = color;
        }

        StopCoroutine("FadeIn");
        StopCoroutine("FadeOut");
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
        bool isEnd = false;

        while (!isEnd)
        {
            for (int i = 0; i < uiBox.images.Length; i++)
            {
                if (uiBox.images[i].color.a < alpha_images[i])
                {
                    Color color = uiBox.images[i].color;
                    color.a += alpha_images[i] * Time.deltaTime / fadeInTime;
                    uiBox.images[i].color = color;
                }
                else isEnd = true;
            }

            for (int i = 0; i < uiBox.texts.Length; i++)
            {
                if (uiBox.texts[i].color.a < alpha_texts[i])
                {
                    Color color = uiBox.texts[i].color;
                    color.a += alpha_texts[i] * Time.deltaTime / fadeInTime;
                    uiBox.texts[i].color = color;
                }
                else isEnd = true;
            }

            for (int i = 0; i < uiBox.tmp_texts.Length; i++)
            {
                if (uiBox.tmp_texts[i].color.a < alpha_tmptexts[i])
                {
                    Color color = uiBox.tmp_texts[i].color;
                    color.a += alpha_tmptexts[i] * Time.deltaTime / fadeInTime;
                    uiBox.tmp_texts[i].color = color;
                }
                else isEnd = true;
            }

            yield return null;
        }

        for (int i = 0; i < uiBox.images.Length; i++)
        {
            Color color = uiBox.images[i].color;
            color = new Color(color.r, color.g, color.b, alpha_images[i]);
            uiBox.images[i].color = color;
        }

        for (int i = 0; i < uiBox.texts.Length; i++)
        {
            Color color = uiBox.texts[i].color;
            color = new Color(color.r, color.g, color.b, alpha_texts[i]);
            uiBox.texts[i].color = color;
        }

        for (int i = 0; i < uiBox.tmp_texts.Length; i++)
        {
            Color color = uiBox.tmp_texts[i].color;
            color = new Color(color.r, color.g, color.b, alpha_tmptexts[i]);
            uiBox.tmp_texts[i].color = color;
        }

        yield return new WaitForSeconds(idleTime);

        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        bool isEnd = false;

        while (!isEnd)
        {
            for (int i = 0; i < uiBox.images.Length; i++)
            {
                if (uiBox.images[i].color.a > 0f)
                {
                    Color color = uiBox.images[i].color;
                    color.a -= alpha_images[i] * Time.deltaTime / fadeOutTime;
                    uiBox.images[i].color = color;
                }
                else isEnd = true;
            }

            for (int i = 0; i < uiBox.texts.Length; i++)
            {
                if (uiBox.texts[i].color.a > 0f)
                {
                    Color color = uiBox.texts[i].color;
                    color.a -= alpha_texts[i] * Time.deltaTime / fadeOutTime;
                    uiBox.texts[i].color = color;
                }
                else isEnd = true;
            }

            for (int i = 0; i < uiBox.tmp_texts.Length; i++)
            {
                if (uiBox.tmp_texts[i].color.a > 0f)
                {
                    Color color = uiBox.tmp_texts[i].color;
                    color.a -= alpha_tmptexts[i] * Time.deltaTime / fadeOutTime;
                    uiBox.tmp_texts[i].color = color;
                }
                else isEnd = true;
            }

            yield return null;
        }
    }
}
