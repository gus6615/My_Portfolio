using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewbieInfo : MonoBehaviour
{
    public const float ARROW_Y_MIN = -1f;
    public const float ARROW_Y_MAX = -1.2f;
    public const float ARROW_MOVESPEED = 2f;
    public static NewbieInfo instance;

    public FadeEffect arrowImage;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if (SaveScript.saveData.pickLevel > 2)
            this.gameObject.SetActive(false);
        else
        {
            StartCoroutine("MoveArrow");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerScript.instance.isDungeon_0_On && !PlayerScript.instance.isEventMap_On 
            && CheckDepth(Mathf.RoundToInt(PlayerScript.instance.transform.position.y)) < SaveScript.saveData.pickLevel)
            arrowImage.gameObject.SetActive(true);
        else
            arrowImage.gameObject.SetActive(false);
    }

    private int CheckDepth(int _depth)
    {
        int stage = MapData.instance.GetStage(_depth);

        if (stage < 3)
            return stage;
        else
            return 999;
    }

    IEnumerator MoveArrow()
    {
        float rate = 0f;
        float y;

        while (rate < 1f)
        {
            y = Mathf.Lerp(ARROW_Y_MAX, ARROW_Y_MIN, rate);
            arrowImage.transform.position = Camera.main.WorldToScreenPoint(PlayerScript.instance.transform.position + new Vector3(-Mathf.Sign(PlayerScript.instance.transform.localScale.x) * 0.1f, y, 0f));
            rate += Time.deltaTime * ARROW_MOVESPEED;
            yield return null;
        }

        arrowImage.transform.position = Camera.main.WorldToScreenPoint(PlayerScript.instance.transform.position + new Vector3(-Mathf.Sign(PlayerScript.instance.transform.localScale.x) * 0.1f, ARROW_Y_MIN, 0f));
        rate = 0f;

        while (rate < 1f)
        {
            y = Mathf.Lerp(ARROW_Y_MIN, ARROW_Y_MAX, rate);
            arrowImage.transform.position = Camera.main.WorldToScreenPoint(PlayerScript.instance.transform.position + new Vector3(-Mathf.Sign(PlayerScript.instance.transform.localScale.x) * 0.1f, y, 0f));
            rate += Time.deltaTime * ARROW_MOVESPEED;
            yield return null;
        }

        arrowImage.transform.position = Camera.main.WorldToScreenPoint(PlayerScript.instance.transform.position + new Vector3(-Mathf.Sign(PlayerScript.instance.transform.localScale.x) * 0.1f, ARROW_Y_MAX, 0f));
        StartCoroutine("MoveArrow");
    }
}
