using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineFusionSlotAni : MonoBehaviour
{
    public RectTransform rectTransform;
    public UIBox uIBox; // order가 -1이면 실패, 1이면 성공
    private float fadeInTime, fadeIdleTime, fadeOutTime;
    private float moveSpeed, rotateSpeed;
    private bool isUpdate;

    // Start is called before the first frame update
    void Start()
    {
        fadeInTime = 0.5f;
        fadeIdleTime = 0.75f;
        fadeOutTime = 1.5f;
        moveSpeed = Random.Range(0.25f, 0.75f);
        rotateSpeed = Random.Range(180f, 360f);
        this.rectTransform.anchoredPosition = new Vector2(Random.Range(-1000f, 1000f), Random.Range(650f, 850f));
        this.rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0f, 360f)));
        this.rectTransform.localScale = Vector3.one * Random.Range(0.5f, 0.75f);

        StartCoroutine("StartAnim");    
    }

    private void Update()
    {
        if (!isUpdate)
            return;

        this.rectTransform.anchoredPosition += new Vector2(0f, -moveSpeed * 1080f * Time.deltaTime);
        this.rectTransform.Rotate(new Vector3(0f, 0f, rotateSpeed * Time.deltaTime));
    }

    IEnumerator StartAnim()
    {
        foreach (var image in uIBox.images)
            image.color = new Color(1f, 1f, 1f, 0f);

        yield return new WaitForSeconds(Random.Range(0f, 1f));
        isUpdate = true;

        // FadeIn
        while (uIBox.images[0].color.a < 1f)
        {
            foreach (var image in uIBox.images)
            {
                Color color = image.color;
                color.a += Time.deltaTime / fadeInTime;
                image.color = color;
            }
            yield return null;
        }

        // FadeIdle
        float time = 0f;
        float colorGoal = 0.4f;
        float colorSpeed = (1f - colorGoal) / fadeIdleTime;
        while (time < fadeIdleTime)
        {
            Color color = uIBox.images[0].color;
            if (uIBox.order == -1) // 실패 = 붉은색
                color.g = color.b -= Time.deltaTime * colorSpeed;
            else // 성공 = 초록색
                color.r = color.b -= Time.deltaTime * colorSpeed;
            uIBox.images[0].color = color;
            time += Time.deltaTime;
            yield return null;
        }

        // FadeOut
        while (uIBox.images[0].color.a > 0f)
        {
            foreach (var image in uIBox.images)
            {
                Color color = image.color;
                color.a -= Time.deltaTime / fadeOutTime;
                image.color = color;
            }
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
