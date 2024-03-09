using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.EventSystems;

public class MainRankUI : MonoBehaviour
{
    public static MainRankUI instance;

    public Canvas rankObject, rankSlotObject;
    public Image rankerRankImage, normalRankImage;
    public Text mainRankText;
    public GameObject mySlotObject;
    private Image[] mySlotQuailityImages, mySlotToolImages;
    private Text[] mySlotUpgradeTexts;
    public GameObject userSlotObject;
    public Image userTierBack, userTierInBack;
    public Image userTierImage;
    public Text userTierText;
    private Image[] userSlotQuailityImages, userSlotToolImages;
    private Text[] userSlotUpgradeTexts;
    public Image[] myPlayerImages, userPlayerImages;
    public Image myRankerImage, myNormalImage, userRankerImage, userNormalImage;
    public Text myRankText, myNameText, myGoldText, userRankText, userNameText, userGoldText;
    public GameObject normalSlotPanel;
    public Text rankPageText;
    public GameObject rankPreviousButton, rankNextButton, rankDoublePreviousButton, rankDoubleNextButton;
    public GameObject readMoreUI;
    private Sprite[] medalSprites;
    private int rankPage, topRankPage;
    private bool isRankUIOn, isTopRankUIOn;
    private Color[] tierColors = { new Color(0.5f, 0.2f, 0f), new Color(0.4f, 0.4f, 0.4f), new Color(0.9f, 0.9f, 0.4f),
        new Color(0.2f, 1f, 0.6f), new Color(0.4f, 1f, 1f), new Color(0.6f, 0.2f, 0.6f), new Color(0.6f, 0.2f, 0.2f), new Color(0.5f, 0.6f, 0.8f), new Color(0.3f, 0.5f, 0.8f)};

    public GameObject topRanker_Object;
    public GameObject topRanker_123th, topRanker_etc;
    public GameObject topRanker_previousButton, topRanker_nextButton;

    private Order[] rankSlots;
    private Text[][] rankSlot_texts;
    private Image[][] rankSlot_images;
    private Button[] rankSlot_buttons;

    // 임시 변수
    Order[] datas;
    Text[] texts;
    Image[] images;
    Button searchButton;
    RankData rankData;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        mySlotQuailityImages = new Image[SaveScript.accessoryNum];
        mySlotToolImages = new Image[SaveScript.accessoryNum];
        mySlotUpgradeTexts = new Text[SaveScript.accessoryNum];
        datas = mySlotObject.GetComponentsInChildren<Order>();
        for (int i = 0; i < datas.Length; i++)
        {
            texts = datas[i].GetComponentsInChildren<Text>();
            images = datas[i].GetComponentsInChildren<Image>();
            mySlotQuailityImages[i] = images[1];
            mySlotToolImages[i] = images[2];
            mySlotUpgradeTexts[i] = texts[0];
        }

        userSlotQuailityImages = new Image[SaveScript.accessoryNum];
        userSlotToolImages = new Image[SaveScript.accessoryNum];
        userSlotUpgradeTexts = new Text[SaveScript.accessoryNum];
        datas = userSlotObject.GetComponentsInChildren<Order>();
        for (int i = 0; i < datas.Length; i++)
        {
            texts = datas[i].GetComponentsInChildren<Text>();
            images = datas[i].GetComponentsInChildren<Image>();
            userSlotQuailityImages[i] = images[1];
            userSlotToolImages[i] = images[2];
            userSlotUpgradeTexts[i] = texts[0];
        }
        medalSprites = Resources.LoadAll<Sprite>("Images/Medals");

        rankSlots = normalSlotPanel.GetComponentsInChildren<Order>();
        rankSlot_texts = new Text[rankSlots.Length][];
        rankSlot_images = new Image[rankSlots.Length][];
        rankSlot_buttons = new Button[rankSlots.Length];
        for (int i = 0; i < rankSlots.Length; i++)
        {
            rankSlot_texts[i] = rankSlots[i].GetComponentsInChildren<Text>();
            rankSlot_images[i] = rankSlots[i].GetComponentsInChildren<Image>();
            rankSlot_buttons[i] = rankSlots[i].GetComponentInChildren<Button>();
        }

        rankObject.gameObject.SetActive(false);
        rankSlotObject.enabled = false;
        readMoreUI.SetActive(false);
        rankerRankImage.gameObject.SetActive(false);
        normalRankImage.gameObject.SetActive(false);
        myRankerImage.gameObject.SetActive(false);
        myNormalImage.gameObject.SetActive(false);
    }

    // 나의 메인 랭킹 정보 설정
    public void SetMyMainRank()
    {
        if (!BackEndLoginManager.isLoginOn || SaveScript.saveRank.isError) return;
        if (Backend.URank.User.GetRankList(SaveScript.rankUuid_gold2, 1, 1).GetErrorCode() != null)
        {
            Debug.LogError("서버 랭킹 데이터 에러 발생");
            SaveScript.endCanvas.enabled = true;
            SaveScript.exitInfoText.text = "현재 서버 측에 문제가 발생했습니다.\n 잠시 후에 다시 접속해주세요.";
            return;
        }

        if (SaveScript.saveRank.myRankData.rank != SaveRank.NO_RANK) mainRankText.text = "랭킹 확인\n(" + SaveScript.saveRank.myRankData.rank + "위)";
        else mainRankText.text = "랭킹 확인";
        if (SaveScript.saveRank.myRankData.rank > -1 && SaveScript.saveRank.myRankData.rank <= 3)
        {
            rankerRankImage.gameObject.SetActive(true);
            normalRankImage.gameObject.SetActive(false);
            rankerRankImage.sprite = SetMedalSprite(SaveScript.saveRank.myRankData);
        }
        else
        {
            rankerRankImage.gameObject.SetActive(false);
            normalRankImage.gameObject.SetActive(true);
            normalRankImage.sprite = SetMedalSprite(SaveScript.saveRank.myRankData);
        }
    }
    public void OnOffRank()
    {
        if (MainScript.isChangeScene || SaveScript.saveRank.isError)
        {
            MainScript.instance.SetAudio(2);
            return;
        }

        if (!isRankUIOn)
        {
            SetMyRank();
            SetRankPage(rankPage);
            SetTopRankPage(topRankPage);
            isTopRankUIOn = false;
            topRanker_Object.SetActive(false);
        }
        isRankUIOn = !isRankUIOn;
        rankObject.gameObject.SetActive(isRankUIOn);
        rankSlotObject.enabled = isRankUIOn;
        rankPage = 0;
        topRankPage = 0;
        MainScript.instance.SetAudio(0);
    }

    public void OnOffTopRanker()
    {
        isTopRankUIOn = !isTopRankUIOn;
        topRanker_Object.SetActive(isTopRankUIOn);
        topRankPage = 0;
        if (isTopRankUIOn)
            SetTopRankPage(topRankPage);
        MainScript.instance.SetAudio(0);
    }

    public void SetTopRankPage(int _topRankPage)
    {
        int rank;
        datas = normalSlotPanel.GetComponentsInChildren<Order>();
        if (_topRankPage == 0)
        {
            rank = 1;
            datas = topRanker_123th.GetComponentsInChildren<Order>();
            topRanker_123th.SetActive(true);
            topRanker_etc.SetActive(false);
        }
        else
        {
            rank = 4 * _topRankPage;
            datas = topRanker_etc.GetComponentsInChildren<Order>();
            topRanker_123th.SetActive(false);
            topRanker_etc.SetActive(true);
        }

        for (int i = 0; i < datas.Length; i++)
        {
            texts = datas[i].GetComponentsInChildren<Text>();
            images = datas[i].GetComponentsInChildren<Image>();
            searchButton = datas[i].GetComponentInChildren<Button>();
            texts[0].text = "<공석>";
            texts[1].text = texts[2].text = "";
            if (_topRankPage == 0) images[1].enabled = true;
            else images[1].enabled = false;
            images[5].enabled = images[6].enabled = images[7].enabled = images[8].enabled = images[9].enabled = false;
            searchButton.GetComponent<Image>().enabled = false;
            searchButton.enabled = false;

            if (rank > SaveScript.saveRank.userNum_3) continue;
            rankData = SaveScript.saveRank.rankData[rank - 1];

            texts[0].text = rankData.nickname + " (" + SetRankText(rankData) +" )";
            texts[1].text = GameFuction.GetGoldText(rankData.gold, rankData.gold2, rankData.gold3, rankData.gold4);
            texts[2].text = "보기";
            images[1].enabled = true;
            images[1].sprite = SetMedalSprite(rankData);
            searchButton.GetComponent<Image>().enabled = true;
            searchButton.enabled = true;
            if (rank < 4) images[1].rectTransform.sizeDelta = medalSprites[0].rect.size;
            else images[1].rectTransform.sizeDelta = medalSprites[3].rect.size;

            // 플레이어 의상 설정
            images[5].enabled = images[6].enabled = images[7].enabled = images[8].enabled = images[9].enabled = true;
            for (int j = 0; j < SaveScript.accessoryNum; j++)
            {
                if (rankData.equipments[j] != -1)
                {
                    if (j != 4)
                    {
                        images[j + 6].color = SaveScript.toolColors[rankData.equipments[j]];
                        images[j + 6].enabled = true;
                    }
                }
                else
                {
                    if (j != 4) images[j + 6].enabled = false;
                }
            }
            rank++;
        }
        SetTopPage(_topRankPage);
    }

    public void SetTopPage(int _rankTopPage)
    {
        topRanker_previousButton.SetActive(true);
        topRanker_nextButton.SetActive(true);
        if (_rankTopPage == 0)
        {
            topRanker_previousButton.SetActive(false);
        }
        if (_rankTopPage == Mathf.CeilToInt((SaveScript.saveRank.userNum_3 - 3) / 4f) || SaveScript.saveRank.userNum_3 <= 3)
        {
            topRanker_nextButton.SetActive(false);
        }
    }

    // 자신의 랭킹 정보 설정
    public void SetMyRank()
    {
        rankData = SaveScript.saveRank.myRankData;
        Sprite toolSprite = null;
        Sprite destroyedSprite = null;

        for (int i = 0; i < SaveScript.accessoryNum; i++)
        {
            switch (i)
            {
                case 0:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.picks[rankData.equipments[i]].sprites[i];
                    destroyedSprite = Pick.destroyedSprite;
                    break;
                case 1:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.hats[rankData.equipments[i]].sprite;
                    destroyedSprite = Hat.destroyedSprite;
                    break;
                case 2:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.rings[rankData.equipments[i]].sprite;
                    destroyedSprite = Ring.destroyedSprite;
                    break;
                case 3:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.pendants[rankData.equipments[i]].sprite;
                    destroyedSprite = Pendant.destroyedSprite;
                    break;
                case 4:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.swords[rankData.equipments[i]].sprite;
                    destroyedSprite = Sword.destroyedSprite;
                    break;
            }

            if (rankData.equipments[i] != -1)
            {
                mySlotToolImages[i].sprite = toolSprite;
                mySlotQuailityImages[i].color = SaveScript.qualityColors[rankData.equipmentQualities[i]];
                if (i != 4)
                {
                    myPlayerImages[i].color = SaveScript.toolColors[rankData.equipments[i]];
                    myPlayerImages[i].enabled = true;
                }
                mySlotUpgradeTexts[i].text = "(+" + GameFuction.GetNumText(rankData.equipmentUpgrades[i]) + "강)";
            }
            else
            {
                mySlotToolImages[i].sprite = destroyedSprite;
                mySlotQuailityImages[i].color = new Color(1f, 1f, 1f, 0f);
                if (i != 4) myPlayerImages[i].enabled = false;
                mySlotUpgradeTexts[i].text = "";
            }
        }

        if (rankData.rank > 0 && rankData.rank < 4)
        {
            myRankerImage.gameObject.SetActive(true);
            myNormalImage.gameObject.SetActive(false);
            myRankerImage.sprite = SetMedalSprite(rankData);
        }
        else
        {
            myRankerImage.gameObject.SetActive(false);
            myNormalImage.gameObject.SetActive(true);
            myNormalImage.sprite = SetMedalSprite(rankData);
        }

        myRankText.text = SetRankText(rankData);
        myNameText.text = rankData.nickname;
        myGoldText.text = GameFuction.GetGoldText(rankData.gold, rankData.gold2, rankData.gold3, rankData.gold4);
    }

    // rankPage에 따라 랭킹 페이지를 설정
    public void SetRankPage(int _rankPage)
    {
        int rank = 10 * _rankPage + 1;

        for (int i = 0; i < rankSlots.Length; i++)
        {
            rankSlot_texts[i][0].text = rankSlot_texts[i][1].text = rankSlot_texts[i][2].text = rankSlot_texts[i][3].text = "";
            rankSlot_images[i][1].enabled = false;
            rankSlot_buttons[i].GetComponent<Image>().enabled = false;
            rankSlot_buttons[i].enabled = false;

            // 설정
            if (rank > SaveScript.saveRank.totalUserNum) continue;
            rankData = SaveScript.saveRank.rankData[rank - 1];
            rankSlot_texts[i][0].text = SetRankText(rankData);
            rankSlot_texts[i][1].text = rankData.nickname;
            rankSlot_texts[i][2].text = GameFuction.GetGoldText(rankData.gold, rankData.gold2, rankData.gold3, rankData.gold4);
            rankSlot_texts[i][3].text = "보기";
            rankSlot_images[i][1].enabled = true;
            rankSlot_images[i][1].sprite = SetMedalSprite(rankData);
            if (rank < 4) rankSlot_images[i][1].rectTransform.sizeDelta = medalSprites[0].rect.size;
            else rankSlot_images[i][1].rectTransform.sizeDelta = medalSprites[3].rect.size;
            rankSlot_buttons[i].GetComponent<Image>().enabled = true;
            rankSlot_buttons[i].enabled = true;
            rank++;
        }
        SetPage(_rankPage);
    }

    // rankPage에 따라 페이지 버튼 설정
    public void SetPage(int _rankPage)
    {
        rankPreviousButton.SetActive(true);
        rankDoublePreviousButton.SetActive(true);
        rankNextButton.SetActive(true);
        rankDoubleNextButton.SetActive(true);
        if (_rankPage == 0)
        {
            rankPreviousButton.SetActive(false);
            rankDoublePreviousButton.SetActive(false);
        }
        if (_rankPage == Mathf.CeilToInt(SaveScript.saveRank.totalUserNum / 10f) - 1 || SaveScript.saveRank.totalUserNum <= 10)
        {
            rankNextButton.SetActive(false);
            rankDoubleNextButton.SetActive(false);
        }

        if (SaveScript.saveRank.totalUserNum <= 10)
            rankPageText.text = "[ 1 / 1 ]";
        else
            rankPageText.text = "[ " + (_rankPage + 1) + " / " + Mathf.CeilToInt(SaveScript.saveRank.totalUserNum / 10f) + " ]";
    }

    // 메달 설정
    public Sprite SetMedalSprite(RankData rankData)
    {
        if (rankData.rank == SaveRank.NO_RANK)
        {
            if (rankData.gold > 1000000000000) // 1조 이상
                return medalSprites[7];
            else if (rankData.gold > 100000000) // 1억 이상
                return medalSprites[8];
            else // 1억 미만
                return medalSprites[9];
        }
        else if (rankData.rank == 1)
            return medalSprites[0]; // 1등
        else if (rankData.rank == 2)
            return medalSprites[1]; // 2등
        else if (rankData.rank == 3)
            return medalSprites[2]; // 3등
        else if (rankData.gold3 > 0) 
            return medalSprites[3]; // 신화
        else
        {
            // 전설
            if (rankData.gold2 > 1000000000000) // 1양 이상
                return medalSprites[4];
            else if (rankData.gold2 > 100000000) // 1자 이상
                return medalSprites[5];
            else // 1자 이하
                return medalSprites[6];
        }
    }

    public string SetRankText(RankData rankData)
    {
        if (rankData.rank == SaveRank.NO_RANK)
        {
            // 랭킹 없음
            if (rankData.gold > 1000000000000) // 1조 이상
                return "§ 능숙한 광부 §";
            else if (rankData.gold > 100000000) // 1억 이상
                return "§ 견습 광부 §";
            else // 1억 미만
                return "§ 미숙한 광부 §";
        }
        else return rankData.rank + "위";
    }

    public void OnTopPreviousRank()
    {
        topRankPage--;
        SetTopRankPage(topRankPage);
    }

    public void OnTopNextRank()
    {
        topRankPage++;
        SetTopRankPage(topRankPage);
    }

    public void OnPreviousRank()
    {
        rankPage--;
        SetRankPage(rankPage);
    }

    public void OnNextRank()
    {
        rankPage++;
        SetRankPage(rankPage);
    }

    public void OnDoublePreviousRank()
    {
        if (rankPage >= 10)
            rankPage -= 10;
        else
            rankPage = 0;
        SetRankPage(rankPage);
    }

    public void OnDoubleNextRank()
    {
        if (rankPage <= Mathf.CeilToInt(SaveScript.saveRank.totalUserNum / 10f) - 11)
            rankPage += 10;
        else
            rankPage = Mathf.CeilToInt(SaveScript.saveRank.totalUserNum / 10f) - 1;
        SetRankPage(rankPage);
    }

    // 다른 유저 랭킹 정보 보기
    public void OnSearchOther()
    {
        int order1 = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Order>().order;
        int order2 = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Order>().order2;
        int rank;
        if (order2 == -1) rank = 10 * rankPage + order1 + 1; // 모든 유저
        else if (order2 == -2) rank = order1 + 1; // 1, 2, 3등
        else  rank = 4 * topRankPage + order1; // 그 외 topRanker
        readMoreUI.SetActive(true);
        rankData = SaveScript.saveRank.rankData[rank - 1];

        // 티어 출력
        userTierBack.color = tierColors[rankData.tier_level / 5];
        userTierInBack.color = tierColors[rankData.tier_level / 5] * 0.6f;
        userTierImage.sprite = AchievementCtrl.instance.tierSprites[rankData.tier_level];
        userTierText.color = tierColors[rankData.tier_level / 5] * 1.2f;
        userTierText.text = AchievementCtrl.tierNames[rankData.tier_level];

        // 장비 출력
        Sprite toolSprite = null;
        Sprite destroyedSprite = null;
        for (int i = 0; i < SaveScript.accessoryNum; i++)
        {
            switch (i)
            {
                case 0:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.picks[rankData.equipments[i]].sprites[i];
                    destroyedSprite = Pick.destroyedSprite;
                    break;
                case 1:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.hats[rankData.equipments[i]].sprite;
                    destroyedSprite = Hat.destroyedSprite;
                    break;
                case 2:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.rings[rankData.equipments[i]].sprite;
                    destroyedSprite = Ring.destroyedSprite;
                    break;
                case 3:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.pendants[rankData.equipments[i]].sprite;
                    destroyedSprite = Pendant.destroyedSprite;
                    break;
                case 4:
                    if (rankData.equipments[i] != -1) toolSprite = SaveScript.swords[rankData.equipments[i]].sprite;
                    destroyedSprite = Sword.destroyedSprite;
                    break;
            }

            if (rankData.equipments[i] != -1)
            {
                userSlotToolImages[i].sprite = toolSprite;
                userSlotQuailityImages[i].color = SaveScript.qualityColors[rankData.equipmentQualities[i]];
                if (i != 4)
                {
                    userPlayerImages[i].color = SaveScript.toolColors[rankData.equipments[i]];
                    userPlayerImages[i].enabled = true;
                }
                userSlotUpgradeTexts[i].text = "(+" + GameFuction.GetNumText(rankData.equipmentUpgrades[i]) + "강)";
            }
            else
            {
                userSlotToolImages[i].sprite = destroyedSprite;
                userSlotQuailityImages[i].color = new Color(1f, 1f, 1f, 0f);
                if (i != 4) userPlayerImages[i].enabled = false;
                userSlotUpgradeTexts[i].text = "";
            }
        }

        if (rankData.rank > 0 && rankData.rank < 4)
        {
            userRankerImage.gameObject.SetActive(true);
            userNormalImage.gameObject.SetActive(false);
            userRankerImage.sprite = SetMedalSprite(rankData);
        }
        else
        {
            userRankerImage.gameObject.SetActive(false);
            userNormalImage.gameObject.SetActive(true);
            userNormalImage.sprite = SetMedalSprite(rankData);
        }

        userRankText.text = SetRankText(rankData);
        userNameText.text = rankData.nickname;
        userGoldText.text = GameFuction.GetGoldText(rankData.gold, rankData.gold2, rankData.gold3, rankData.gold4);
    }

    public void CloseSearchOther()
    {
        readMoreUI.SetActive(false);
    }
}