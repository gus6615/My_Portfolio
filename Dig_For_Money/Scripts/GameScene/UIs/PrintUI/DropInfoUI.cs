using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropInfoUI : MonoBehaviour
{
    public static DropInfoUI instance;

    public Image infoBackSpace, infoBackLine, dropImage;
    public Text infoText;
    public FadeUI fadeUI;
    public Sprite manaSprite, cashSprite, growthSprite;
    private Jem currentJem; // 최근에 휙득한 보석
    private int currentDropType; // 최근에 획득한 아이템 종류, -1 = Nothing, 0 = Jem, 1 = ManaOre, 2 = Item, 3 = Pet, 4 = Cash
    private long currentDropNum; // 최근에 휙득한 보석 갯수
    private float infoTime;  // 특정 아이템 알림이 지속되는 시간
    private bool isInfo; // 현재 알림이 지속 중인가?

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        infoTime = 2f;

        infoBackSpace.gameObject.SetActive(false);
        infoBackLine.gameObject.SetActive(false);
        infoText.gameObject.SetActive(false);
        dropImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 드랍 아이템의 초상화 및 색상 설정
    /// </summary>
    private void SetBasicInfo(Sprite dropSprite, Color backColor)
    {
        infoBackSpace.gameObject.SetActive(true);
        infoBackLine.gameObject.SetActive(true);
        infoText.gameObject.SetActive(true);
        dropImage.gameObject.SetActive(true);

        Color backgroundColor = backColor;
        if (backgroundColor.r != 0f)
            backgroundColor.r *= 0.5f;
        if (backgroundColor.b != 0f)
            backgroundColor.b *= 0.5f;
        if (backgroundColor.g != 0f)
            backgroundColor.g *= 0.5f;

        infoBackSpace.color = backgroundColor;
        infoBackLine.color = backColor;
        dropImage.sprite = dropSprite;
        dropImage.color = Color.white;
        infoText.color = backColor;
        fadeUI.SetFadeValues(0f, infoTime, 0.5f);
    }

    /// <summary>
    /// 드랍한 광물의 정보를 UI로 알려줍니다.
    /// </summary>
    /// <param name="jem">광물</param>
    /// <param name="jemType">광물의 단위(1개,10개,100개 단위)</param>
    public void SetJemInfo(Jem jem, int jemType)
    {
        if (isInfo) return;
        if (currentDropType > 0) return;
        if (currentDropType != 0)
            currentDropNum = 0;
        currentDropType = 0;

        if (currentJem != null && jem.itemCode == currentJem.itemCode)
        {
            // 최근에 동일한 광물을 획득했을 경우
            currentDropNum += (int)Mathf.Pow(10, jemType);
        }
        else
        {
            // 최근에 획득한 광물과 다른 경우
            currentJem = jem;
            currentDropNum = (int)Mathf.Pow(10, jemType);
        }

        Color color = SaveScript.qualityColors_weak[jem.quality];
        color.a = 0.8f;
        SetBasicInfo(jem.jemSprite, color);
        infoText.text = "(" + SaveScript.qualityNames_kr[jem.quality] + ") '" + jem.name + "' x" + GameFuction.GetNumText(currentDropNum) + " 획득";
    }

    /// <summary>
    /// 드랍한 버프 및 강화 보조 아이템을 UI로 보여줍니다.
    /// </summary>
    /// <param name="item">아이템</param>
    public void SetItemInfo(Item item)
    {
        if (!item.isInfoOn || currentDropType > 2) return;
        currentDropType = 2;
        SetBasicInfo(item.sprite, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        infoText.text = "'" + item.name + "' 획득";
        infoText.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        StopCoroutine("CheckInfoTime");
        StartCoroutine("CheckInfoTime");
    }

    /// <summary>
    /// 드랍한 마나석을 UI로 보여줍니다.
    /// </summary>
    public void SetManaInfo(long manaNum)
    {
        if (currentDropType > 1) return;
        if (currentDropType != 1) 
            currentDropNum = 0;
        currentDropType = 1;
        currentDropNum = manaNum;

        SetBasicInfo(manaSprite, new Color(0.3f, 0.3f, 1f, 0.8f));
        infoText.text = "마나석 x " + GameFuction.GetNumText(currentDropNum) + " 획득";
        infoText.color = new Color(0.5f, 0.6f, 1f, 0.8f);
        StopCoroutine("CheckInfoTime");
        StartCoroutine("CheckInfoTime");
    }

    /// <summary>
    /// 드랍한 펫을 UI로 보여줍니다.
    /// </summary>
    /// <param name="_type">펫 종류</param>
    /// <param name="_code">펫 코드</param>
    public void SetPetInfo(int _type, int _code)
    {
        if (_code < 4 || currentDropType > 3) return;
        currentDropType = 3;

        switch (_type)
        {
            case 0:
                SetBasicInfo(MineSlime.miner_faceSprites[_code], new Color(0.5f, 0.3f, 0.1f, 0.8f));
                infoText.text = "[" + MineSlime.qualityNames[_code] + "등급] '" + MinerSlime.names[_code] + "' 획득";
                break;
            case 1:
                SetBasicInfo(MineSlime.adventurer_faceSprites[_code], new Color(0.5f, 0.3f, 0.1f, 0.8f));
                infoText.text = "[" + MineSlime.qualityNames[_code] + "등급] '" + AdventurerSlime.names[_code] + "' 획득";
                break;
        }
        
        infoText.color = new Color(0.6f, 0.4f, 0.2f, 0.8f);
        StopCoroutine("CheckInfoTime");
        StartCoroutine("CheckInfoTime");
    }

    /// <summary>
    /// 드랍한 Cash를 UI로 보여줍니다.
    /// </summary>
    public void SetCashInfo(int cashType)
    {
        if (currentDropType > 4) return;
        if (currentDropType != 4)
            currentDropNum = 0;
        currentDropType = 4;
        currentDropNum += (int)Mathf.Pow(10, cashType);

        SetBasicInfo(cashSprite, new Color(1f, 0.5f, 0.5f, 0.8f));
        infoText.text = "레드 다이아(Cash) x" + currentDropNum + " 획득";
        infoText.color = new Color(1f, 0.7f, 0.7f, 0.8f);
        StopCoroutine("CheckInfoTime");
        StartCoroutine("CheckInfoTime");
    }

    /// <summary>
    /// 드랍한 성장하는 돌을 UI로 보여줍니다.
    /// </summary>
    public void SetGrowthOreInfo()
    {
        if (currentDropType > 5) return;
        if (currentDropType != 5)
            currentDropNum = 0;
        currentDropType = 5;
        currentDropNum += 1;

        SetBasicInfo(growthSprite, new Color(0.3f, 0.3f, 0.3f, 0.8f));
        infoText.text = "(언노운) 성장하는 돌 x" + currentDropNum + " 획득";
        infoText.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        StopCoroutine("CheckInfoTime");
        StartCoroutine("CheckInfoTime");
    }

    IEnumerator CheckInfoTime()
    {
        isInfo = true;
        yield return new WaitForSeconds(infoTime);
        isInfo = false;
        currentDropType = 0;
    }
}
