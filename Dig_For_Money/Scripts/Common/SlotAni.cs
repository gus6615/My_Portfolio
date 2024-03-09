using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotAni : MonoBehaviour
{
    public RectTransform rectTransform;
    public UIBox uIBox;
    public float fadeInTime, fadeIdleTime, fadeOutTime;
    private float startTime;
    private float moveSpeed;
    private float rotateSpeed;
    private bool isUpdate;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Random.Range(0.25f,1f);
        moveSpeed = Random.Range(0.5f, 1f);
        rotateSpeed = Random.Range(180f, 360f);
        this.rectTransform.anchoredPosition = new Vector2(Random.Range(-1000f, 1000f), Random.Range(650f, 850f));
        this.rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0f, 360f)));
        this.rectTransform.localScale = Vector3.one * Random.Range(0.75f, 1.25f);

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

        yield return new WaitForSeconds(startTime);
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
