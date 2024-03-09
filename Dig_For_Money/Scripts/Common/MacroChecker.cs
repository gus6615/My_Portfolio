using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MacroChecker : MonoBehaviour
{
    static public MacroChecker instance;

    public new AudioSource audio;
    public Canvas canvas;
    public GameObject macroObject;
    public GameObject closeGameObject;
    public Transform picture;
    public GameObject manaOrePrefab;
    public InputField inputField;
    public Button inputButton;
    public Text InfoText;

    private int manaOre_count;
    private int fail_count;

    // 임시 데이터
    Order[] orders;
    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        audio.mute = !SaveScript.saveData.isSEOn;
        SetDefaultUI();
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    public void SetDefaultUI()
    {
        canvas.enabled = false;
        macroObject.SetActive(false);
        closeGameObject.SetActive(false);
        InfoText.text = "";
        inputButton.enabled = true;
        manaOre_count = fail_count = 0;
        orders = picture.GetComponentsInChildren<Order>();
        for (int i = 0; i < orders.Length; i++)
            Destroy(orders[i].gameObject);
    }

    public void CheckOnMacro()
    {
        if (SaveScript.playTime > SaveScript.macroTime)
            SetMacro();
    }

    public void SetMacro()
    {
        canvas.enabled = true;
        macroObject.SetActive(true);
        manaOre_count = Random.Range(1, 9);
        fail_count = 0;
        Time.timeScale = 0f;
        SetAudio(0);

        for (int i = 0; i < manaOre_count; i++)
        {
            rect = Instantiate(manaOrePrefab, picture).GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f));
            rect.transform.localScale = Vector3.one * Random.Range(0.7f, 1.2f);
            rect.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 90f));
        }
    }

    public void CheckButton()
    {
        if (fail_count > 1)
        {
            // 매크로 모두 실패 -> 게임 종료
            closeGameObject.SetActive(true);
            SetAudio(2);
            return;
        }

        if (inputField.text == manaOre_count.ToString())
        {
            // 매크로 성공
            StartCoroutine("SuccessMacro");
            SetAudio(4);
        }
        else
        {
            // 매크로 실패
            InfoText.text = "※ 올바르지 않은 답입니다! 다시 입력해주세요.\n( 남은 기회 : " + (2 - fail_count) + "회 )";
            fail_count++;
            SetAudio(2);
        }

        inputField.text = "";
    }

    IEnumerator SuccessMacro()
    {
        InfoText.text = "※ 유저님이 매크로가 아님을 확인했습니다!\n항상 제 게임을 즐겨주셔서 감사합니다 :)";
        inputButton.enabled = false;
        Time.timeScale = 1f;

        yield return new WaitForSeconds(0f);

        StopCoroutine("CheckOnMacro");
        SaveScript.playTime = 0;
        SetDefaultUI();
    }
}
