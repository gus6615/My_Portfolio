using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CashItemAnimator : MonoBehaviour
{
    public static CashItemAnimator instance;

    // 결과 관련 변수들
    public GameObject cashResult_prefab;
    public GameObject[] slotPrefabs;
    public GridLayoutGroup slotPanel, resultPanel;
    public UIBox result_multi, result_single;

    // 애니메이션 관련 변수들
    public GameObject passClickPanel;
    public Sprite[] oreOpenBoxSprites;
    public Sprite[] oreClosedBoxSprites;
    public Sprite[] oreSprites;
    public Sprite[] itemBoxUpSprites; // 아이템 상자 Up Sprites
    public Sprite[] itemBoxDownSprites; // 아이템 상자 Down Sprites
    public Sprite[] eggSprites_0, eggSprites_1, eggSprites_2, eggSprites_3; // n 번째 알 Sprites
    public Sprite[] CapsuleUpSprites, CapsuleDownSprites; // 캡슐 sprite
    public Sprite RCapsulePrimitive, MCapsulePrimitive;
    public Sprite[] RCapsuleResultSprites, MCapsuleResultSprites; // 캡슐 보상 sprites
    public Sprite[] ribbonSprites, openRandomBoxSprites, closedRandomBoxSprites;
    public Sprite[] reinforceSprites, manaOreSprites;
    public new AudioSource audio;

    // 데이터 변수들
    static public long[] oreMinNums = { 1, 3, 5, 10, 20, 40, 100, 300, 1000, 3000, 10000, 50000, 200000, 500000, 1000000 };
    static public float[] orePlusPercents = { 10f, 30f, 50f }; 
    static public float[] ultimateOrePercents = { 0f, 0.2f, 1f };
    static public float[] mysticOrePercents = { 0f, 0f, 0.05f };
    static public float[] elixirPercent = { 0f, 0.35f, 1f };
    static public float[] reinforce2Percent = { 0f, 0.35f, 1f };
    static private int bufItemCount;
    static private int reinforceItemCount;
    static public float[] capsulePercent = { 0.6f, 0.35f, 0.04f, 0.01f };
    static public int[][] R_capsuleForces =
    {
        new int[] { 1000, 5000, 30000, 100000 },
        new int[] { 10000, 50000, 300000, 1000000 },
        new int[] { 1000000, 5000000, 30000000, 100000000 },
    };
    static public int[][] M_capsuleForces =
    {
        new int[] { 100, 500, 3000, 10000 },
        new int[] { 1000, 5000, 30000, 100000 },
        new int[] { 10000, 50000, 300000, 1000000 },
    };
    static public float[][] randomBoxPercents =
    {
        new float[] { 0.005f, 0.295f, 0.3f, 0.2f, 0.2f },
        new float[] { 0.01f, 0.29f, 0.3f, 0.2f, 0.2f },
        new float[] { 0.02f, 0.28f, 0.3f, 0.2f, 0.2f }
    };
    static public float[][] randomBox_bufPercent =
    {
        new float[] { 0.15f, 0.25f, 0.4f, 0.2f },
        new float[] { 0f, 0.1f, 0.3f, 0.6f },
        new float[] { 0f, 0f, 0f, 1f },
    };
    static public float[][] randomBox_reinforcePercent =
{
        new float[] { 0.95f, 0.05f, 0f },
        new float[] { 0.49f, 0.5f, 0.01f },
        new float[] { 0f, 0.95f, 0.05f},
    };
    static public float[] randomBox_capsurePercent = { 0.45f, 0.3f, 0.25f };
    static public int[][] randomBox_reinforceNums =
    {
        new int[] { 100, 500, 3000 },
        new int[] { 1000, 5000, 30000 },
        new int[] { 100000, 500000, 3000000 },
    };
    static public int[][] randomBox_manaNums =
{
        new int[] { 10, 50, 300 },
        new int[] { 100, 500, 3000 },
        new int[] { 1000, 5000, 30000 },
    };

    private Color[] slotColors = new Color[CashItemShop.PACKAGE_NUM];
    public int itemType, itemCode;
    private long[] oreNums = new long[SaveScript.totalItemNum];
    private int[] reinforceOreNum = new int[CashItemShop.PACKAGE_NUM], manaOreNum = new int[CashItemShop.PACKAGE_NUM];
    private int[] bufItemNums;
    private int[] reinforceItemNums;
    private int[] petCode = new int[CashItemShop.PACKAGE_NUM];
    private int[] petType = new int[CashItemShop.PACKAGE_NUM];
    private int[] capsule_code = new int[CashItemShop.PACKAGE_NUM];
    private int[] random_type = new int[CashItemShop.PACKAGE_NUM];
    private int[] random_code = new int[CashItemShop.PACKAGE_NUM];
    private int[] random_code2 = new int[CashItemShop.PACKAGE_NUM];

    Mine_RewardObject[] items;
    Mine_RewardObject item;
    CashSlotAni[] slots;
    CashSlotAni slot;
    Sprite sprite;
    Color color;
    string text;

    private void Start()
    {
        instance = this;
        bufItemCount = SaveScript.bufItemNum + SaveScript.bufItemCodeNum;
        reinforceItemCount = SaveScript.reinforceItemNum + SaveScript.reinforceItem2Num;
        bufItemNums = new int[bufItemCount];
        reinforceItemNums = new int[reinforceItemCount];

        result_single.gameObject.SetActive(false);
        result_multi.gameObject.SetActive(false);
        passClickPanel.SetActive(false);
    }

    /// <summary>
    /// 애니메이션 초기 설정 함수
    /// </summary>
    public void Setting()
    {
        itemType = CashItemShop.instance.menuIndex;
        itemCode = CashItemShop.instance.itemIndex;

        for (int i = 0; i < oreNums.Length; i++) oreNums[i] = 0;
        for (int i = 0; i < bufItemCount; i++) bufItemNums[i] = 0;
        for (int i = 0; i < reinforceItemCount; i++) reinforceItemNums[i] = 0;
        for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
        {
            reinforceOreNum[i] = manaOreNum[i] = -1;
            petCode[i] = petType[i] = capsule_code[i] = -1;
            random_code[i] = random_code2[i] = random_type[i] = -1;
        }
        result_single.gameObject.SetActive(false);
        result_multi.gameObject.SetActive(false);
        Invoke("SetPassPanel", 0.5f);

        SetResultData(); // 데이터 생성
        
        // Light Color Setting
        int colorIndex = 0;
        int bestIndex = 0;
        switch (itemCode)
        {
            case 0:
                for (int i = 0; i < oreNums.Length; i++)
                {
                    if (oreNums[i] > 0)
                    {
                        if (bestIndex < SaveScript.jems[i].quality)
                            bestIndex = SaveScript.jems[i].quality;
                    }
                }
                break;
            case 1:
                for (int i = 0; i < bufItemCount; i++)
                {
                    if (bufItemNums[i] > 0)
                    {
                        if (i >= SaveScript.bufItemNum)
                            bestIndex = 4;
                        else
                        {
                            BufItem.GetColorByCode(i, out colorIndex);
                            if (bestIndex < colorIndex) bestIndex = colorIndex;
                        }
                    }
                }
                for (int i = 0; i < reinforceItemCount; i++)
                {
                    if (reinforceItemNums[i] > 0)
                    {
                        if (i >= SaveScript.reinforceItemNum + 3)
                            bestIndex = 5;
                        if (i >= SaveScript.reinforceItemNum)
                            bestIndex = 4;
                        else
                        {
                            ReinforceItem.GetColorByCode(i, out colorIndex);
                            if (bestIndex < colorIndex) bestIndex = colorIndex;
                        }
                    }
                }
                break;
            case 2:
                for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
                {
                    if (petCode[i] != -1)
                    {
                        slotColors[i] = MineSlime.GetColorByCode(petCode[i], out colorIndex);
                        if (bestIndex < colorIndex) bestIndex = colorIndex;
                    }
                }
                break;
            case 3:
            case 4:
                for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
                {
                    if (capsule_code[i] != -1)
                    {
                        slotColors[i] = Item.colors[capsule_code[i]];
                        colorIndex = capsule_code[i];
                        if (bestIndex < colorIndex) bestIndex = colorIndex;
                    }
                }
                break;
            case 5:
                for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
                {
                    slotColors[i] = Color.white;
                    if (random_type[i] == 0)
                    {
                        slotColors[i] = SaveScript.qualityColors_weak[SaveScript.qualityColors_weak.Length - 1];
                        break;
                    }
                }
                break;
        }

        if (CashItemShop.instance.isPackage)
            slotPanel.childAlignment = TextAnchor.UpperLeft;
        else
            slotPanel.childAlignment = TextAnchor.MiddleCenter;
        StartCoroutine("CreateCashSlot");
    }

    IEnumerator CreateCashSlot()
    {
        int count = 1;
        if (CashItemShop.instance.isPackage)
            count = CashItemShop.PACKAGE_NUM;

        for (int i = 0; i < count; i++)
        {
            slot = Instantiate(slotPrefabs[itemCode], slotPanel.transform).GetComponent<CashSlotAni>();
            if (CashItemShop.instance.isPackage)
            {
                slot.transform.localScale = Vector3.one * 0.5f;
                slot.SetAudioVolume(0.15f);
            }
            else
            {
                slot.transform.localScale = Vector3.one;
                slot.SetAudioVolume(0.7f);
            }

            switch (itemCode)
            {
                case 0: // 광물 상자
                    slot.uibox.images[2].color = slot.uibox.images[3].color = slotColors[i];
                    slot.uibox.images[0].sprite = oreClosedBoxSprites[itemType];
                    slot.uibox.images[1].sprite = oreOpenBoxSprites[itemType];
                    slot.uibox.images[4].sprite = oreSprites[itemType];
                    break;
                case 1: // 아이템 상자
                    slot.uibox.images[1].color = slot.uibox.images[2].color = slotColors[i];
                    slot.uibox.images[0].sprite = itemBoxDownSprites[itemType];
                    slot.uibox.images[3].sprite = itemBoxUpSprites[itemType];
                    break;
                case 2: // 펫 상자
                    slot.uibox.images[0].color = slot.uibox.images[1].color = slot.uibox.images[2].color = slotColors[i];
                    slot.uibox.images[3].sprite = eggSprites_0[itemType];
                    break;
                case 3: // 강화석 캡슐
                    slot.uibox.images[1].color = slot.uibox.images[2].color = slotColors[i];
                    slot.uibox.images[0].sprite = CapsuleDownSprites[itemType];
                    slot.uibox.images[4].sprite = CapsuleUpSprites[itemType];
                    slot.uibox.images[3].sprite = RCapsulePrimitive;
                    break;
                case 4: // 마나석 캡슐
                    slot.uibox.images[1].color = slot.uibox.images[2].color = slotColors[i];
                    slot.uibox.images[0].sprite = CapsuleDownSprites[itemType];
                    slot.uibox.images[4].sprite = CapsuleUpSprites[itemType];
                    slot.uibox.images[3].sprite = MCapsulePrimitive;
                    break;
                case 5: // 랜덤 박스
                    slot.uibox.images[3].color = slotColors[i];
                    slot.uibox.images[0].sprite = closedRandomBoxSprites[itemType];
                    slot.uibox.images[2].sprite = openRandomBoxSprites[itemType];
                    slot.uibox.images[1].sprite = ribbonSprites[itemType];
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 아이템 결과를 출력하고 데이터를 적용
    /// </summary>
    public void Result()
    {
        int bestIndex = -1, currentIndex = -1;

        audio.clip = SaveScript.SEs[4];
        if (itemCode == 3 || itemCode == 4)
        {
            for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
            {
                if (bestIndex < (currentIndex = capsule_code[i]))
                    bestIndex = currentIndex;
                audio.clip = SaveScript.SEs[31 + bestIndex];
            }
        }
        else if (itemCode == 5)
        {
            for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
            {
                if (random_type[i] == 0)
                {
                    audio.clip = SaveScript.SEs[33];
                    break;
                }
            }
        }

        audio.Play();
        passClickPanel.SetActive(false);
        StopCoroutine("CreateCashSlot");
        PrintResultUI();
    }

    /// <summary>
    /// 아이템 결과 UI를 출력
    /// </summary>
    private void PrintResultUI()
    {
        slots = slotPanel.GetComponentsInChildren<CashSlotAni>();
        for (int i = 0; i < slots.Length; i++)
            Destroy(slots[i].gameObject);

        if (CashItemShop.instance.isPackage)
        {
            result_multi.gameObject.SetActive(true);
            result_multi.texts[0].text = "[ " + SaveScript.cashItems[CashItem.GetItemCode(itemType, itemCode)].name + " x 10 결과 ]";
        }
        else
        {
            if (GameFuction.HasElement(new int[] { 0, 1 }, itemCode))
            {
                result_multi.gameObject.SetActive(true);
                result_multi.texts[0].text = "[ " + SaveScript.cashItems[CashItem.GetItemCode(itemType, itemCode)].name + " 결과 ]";
            }
            else
            {
                result_single.gameObject.SetActive(true);
                result_single.texts[0].text = "[ " + SaveScript.cashItems[CashItem.GetItemCode(itemType, itemCode)].name + " 결과 ]";
            }
        }
        Shop.instance.SetBasicInfo();

        items = resultPanel.GetComponentsInChildren<Mine_RewardObject>();
        for (int i = 0; i < items.Length; i++)
            Destroy(items[i].gameObject);

        switch (itemCode)
        {
            case 0:
                for (int j = 0; j < oreNums.Length; j++)
                {
                    if (oreNums[j] > 0)
                    {
                        item = Instantiate(cashResult_prefab, resultPanel.transform).GetComponent<Mine_RewardObject>();
                        item.numText.text = "x " + GameFuction.GetNumText(oreNums[j]);
                        item.spriteImage.sprite = SaveScript.jems[j].jemSprite;
                        item.qualityImage.color = SaveScript.qualityColors[SaveScript.jems[j].quality];
                    }
                }
                break;
            case 1:
                for (int j = 0; j < bufItemCount; j++)
                {
                    if (bufItemNums[j] > 0)
                    {
                        item = Instantiate(cashResult_prefab, resultPanel.transform).GetComponent<Mine_RewardObject>();
                        item.numText.text = "x " + bufItemNums[j];

                        if (j >= SaveScript.bufItemNum)
                        {
                            item.spriteImage.sprite = SaveScript.elixirs[j - SaveScript.bufItemNum].sprite;
                            item.qualityImage.color = SaveScript.elixirs[j - SaveScript.bufItemNum].color;
                        }
                        else
                        {
                            item.spriteImage.sprite = SaveScript.bufItems[j].sprite;
                            item.qualityImage.color = SaveScript.bufItems[j].color;
                        }
                    }
                }
                for (int j = 0; j < reinforceItemCount; j++)
                {
                    if (reinforceItemNums[j] > 0)
                    {
                        item = Instantiate(cashResult_prefab, resultPanel.transform).GetComponent<Mine_RewardObject>();
                        item.numText.text = "x " + reinforceItemNums[j];

                        if (j >= SaveScript.reinforceItemNum)
                        {
                            item.spriteImage.sprite = SaveScript.reinforceItems2[j - SaveScript.reinforceItemNum].sprite;
                            item.qualityImage.color = SaveScript.reinforceItems2[j - SaveScript.reinforceItemNum].color;
                        }
                        else
                        {
                            item.spriteImage.sprite = SaveScript.reinforceItems[j].sprite;
                            item.qualityImage.color = SaveScript.reinforceItems[j].color;
                        }
                    }
                }
                break;
            case 2:
                for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
                {
                    if (petType[i] == -1)
                        continue;

                    if (petType[i] == 1) // 광부 슬라임
                    {
                        sprite = MineSlime.miner_faceSprites[petCode[i]];
                        color = MineSlime.GetColorByCode(petCode[i]);
                        text = "[ " + MineSlime.qualityNames[petCode[i]] + " ]";
                    }
                    else // 모험가 슬라임
                    {
                        sprite = MineSlime.adventurer_faceSprites[petCode[i]];
                        color = MineSlime.GetColorByCode(petCode[i]);
                        text = "[ " + MineSlime.qualityNames[petCode[i]] + " ]";
                    }

                    if (CashItemShop.instance.isPackage)
                    {
                        item = Instantiate(cashResult_prefab, resultPanel.transform).GetComponent<Mine_RewardObject>();
                        item.spriteImage.sprite = sprite;
                        item.qualityImage.color = color;
                        item.numText.text = text;
                    }
                    else
                    {
                        if (petType[i] == 1) // 광부 슬라임
                            text = "'[ " + MineSlime.qualityNames[petCode[i]] + " ] " + MinerSlime.names[petCode[i]] + "를 획득하였습니다!";
                        else // 모험가 슬라임
                            text = "'[ " + MineSlime.qualityNames[petCode[i]] + " ] " + AdventurerSlime.names[petCode[i]] + "를 획득하였습니다!"; ;
                        result_single.images[0].sprite = sprite;
                        result_single.texts[1].text = text;
                    }
                }
                break;
            case 3:
            case 4:
                for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
                {
                    if (itemCode == 3)
                    {
                        if (reinforceOreNum[i] == -1)
                            continue;

                        sprite = RCapsuleResultSprites[capsule_code[i]];
                        color = Item.colors[capsule_code[i]];
                        text = "x " + GameFuction.GetNumText(reinforceOreNum[i]);
                    }
                    else
                    {
                        if (manaOreNum[i] == -1)
                            continue;

                        sprite = MCapsuleResultSprites[capsule_code[i]];
                        color = Item.colors[capsule_code[i]];
                        text = "x " + GameFuction.GetNumText(manaOreNum[i]);
                    }

                    if (CashItemShop.instance.isPackage)
                    {
                        item = Instantiate(cashResult_prefab, resultPanel.transform).GetComponent<Mine_RewardObject>();
                        item.spriteImage.sprite = sprite;
                        item.qualityImage.color = color;
                        item.numText.text = text;
                    }
                    else
                    {
                        if (itemCode == 3) text = "'강화석 x " + GameFuction.GetNumText(reinforceOreNum[i]) + "개'를 획득하였습니다!";
                        else text = "'마나석 x " + GameFuction.GetNumText(manaOreNum[i]) + "개'를 획득하였습니다!";
                        result_single.images[0].sprite = sprite;
                        result_single.texts[1].text = text;
                    }
                }
                break;
            case 5:
                for (int i = 0; i < CashItemShop.PACKAGE_NUM; i++)
                {
                    if (random_type[i] == -1)
                        continue;

                    color = Color.white;
                    switch (random_type[i])
                    {
                        case 0:
                            sprite = SaveScript.cashEquipments[random_code[i]].sprite;
                            text = "[ " + SaveScript.cashEquipments[random_code[i]].name + " ]";
                            color = SaveScript.qualityColors_weak[SaveScript.qualityColors_weak.Length - 1];
                            break;
                        case 1:
                            sprite = reinforceSprites[random_code[i]];
                            text = "x " + GameFuction.GetNumText(randomBox_reinforceNums[itemType][random_code[i]]);
                            break;
                        case 2:
                            sprite = manaOreSprites[random_code[i]];
                            text = "x " + GameFuction.GetNumText(randomBox_manaNums[itemType][random_code[i]]);
                            break;
                        case 3:
                            switch (random_code[i])
                            {
                                case 0:
                                case 1:
                                case 2:
                                    sprite = SaveScript.bufItems[random_code2[i]].sprite;
                                    text = "x 1";
                                    break;
                                case 3:
                                    sprite = SaveScript.elixirs[random_code2[i]].sprite;
                                    text = "x 1";
                                    break;
                            }
                            break;
                        case 4:
                            switch (random_code[i])
                            {
                                case 0:
                                    sprite = SaveScript.reinforceItems[random_code2[i]].sprite;
                                    text = "x 1";
                                    break;
                                case 1:
                                case 2:
                                    sprite = SaveScript.reinforceItems2[random_code2[i]].sprite;
                                    text = "x 1";
                                    break;
                            }
                            break;
                    }

                    if (CashItemShop.instance.isPackage)
                    {
                        item = Instantiate(cashResult_prefab, resultPanel.transform).GetComponent<Mine_RewardObject>();
                        item.spriteImage.sprite = sprite;
                        item.qualityImage.color = color;
                        item.numText.text = text;
                    }
                    else
                    {
                        switch (random_type[i])
                        {
                            case 0: text = "[ " + SaveScript.cashEquipments[random_code[i]].name + " ] 를 획득하였습니다!"; break;
                            case 1: text = "'강화석 x " + GameFuction.GetNumText(randomBox_reinforceNums[itemType][random_code[i]]) + "개'를 획득하였습니다!"; break;
                            case 2: text = "'마나석 x " + GameFuction.GetNumText(randomBox_manaNums[itemType][random_code[i]]) + "개'를 획득하였습니다!"; break;
                            case 3:
                                switch (random_code[i])
                                {
                                    case 0:
                                    case 1:
                                    case 2: text = "'" + SaveScript.bufItems[random_code2[i]].name + "' 를 획득하였습니다!"; break;
                                    case 3: text = "'" + SaveScript.elixirs[random_code2[i]].name + "' 를 획득하였습니다!"; break;
                                }
                                break;
                            case 4:
                                switch (random_code[i])
                                {
                                    case 0: text = "'" + SaveScript.reinforceItems[random_code2[i]].name + "' 를 획득하였습니다!"; break;
                                    case 1:
                                    case 2: text = "'" + SaveScript.reinforceItems2[random_code2[i]].name + "' 를 획득하였습니다!"; break;
                                }
                                break;
                        }

                        result_single.images[0].sprite = sprite;
                        result_single.texts[1].text = text;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 아이템 결과 데이터 적용
    /// </summary>
    private void SetResultData()
    {
        bool isChatSend = false;
        int count = 1;
        if (CashItemShop.instance.isPackage)
        {
            count = CashItemShop.PACKAGE_NUM;
            SaveScript.saveData.cash -= (int)(count * (1 - CashItemShop.DC_PERCENT) * SaveScript.cashItems[itemType * SaveScript.cashItemNums[0] + itemCode].price);
        }
        else
            SaveScript.saveData.cash -= SaveScript.cashItems[itemType * SaveScript.cashItemNums[0] + itemCode].price;

        switch (itemCode)
        {
            case 0: // 광물 상자
                for (int i = 0; i < count; i++)
                {
                    int maxJemCode = 0;
                    int minJemCode = 0;
                    for (int j = 0; j < SaveScript.saveData.pickLevel; j++)
                        minJemCode += SaveScript.stageItemNums[j];
                    maxJemCode = minJemCode + SaveScript.stageItemNums[SaveScript.saveData.pickLevel];
                    slotColors[i] = Color.white;

                    for (int j = minJemCode; j < maxJemCode; j++)
                    {
                        long rand = 0;

                        if (SaveScript.jems[j].quality < 5)
                            rand = (long)(oreMinNums[SaveScript.saveData.pickLevel] * Random.Range(1f, 10f) * orePlusPercents[itemType]);
                        else if (SaveScript.jems[j].quality == 5 && GameFuction.GetRandFlag(ultimateOrePercents[itemType]))
                        {
                            rand = (long)(oreMinNums[SaveScript.saveData.pickLevel] * Random.Range(0.1f, 0.3f) * orePlusPercents[itemType]);
                            slotColors[i] = SaveScript.qualityColors_weak[5];
                        }
                        else if (SaveScript.jems[j].quality == 6 && GameFuction.GetRandFlag(mysticOrePercents[itemType]))
                        {
                            rand = (long)(oreMinNums[SaveScript.saveData.pickLevel] * Random.Range(0.05f, 0.1f) * orePlusPercents[itemType]);
                            slotColors[i] = SaveScript.qualityColors_weak[6];
                        }
                        oreNums[j] += rand;
                        SaveScript.saveData.hasItemNums[j] += rand;
                    }
                    AchievementCtrl.instance.SetAchievementAmount(30, 1);
                }
                break;
            case 1:
                for (int j = 0; j < count; j++)
                {
                    int index;
                    int color_best = 0, color_cur = 0;
                    int num = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        switch (itemType)
                        {
                            case 0: num = Random.Range(1, 3); break;
                            case 1: num = Random.Range(2, 5); break;
                            case 2: num = Random.Range(4, 7); break;
                        }

                        if (GameFuction.GetRandFlag(elixirPercent[itemType]))
                        {
                            index = SaveScript.bufItemNum + Random.Range(0, SaveScript.bufItemCodeNum);
                            bufItemNums[index] += num;
                            SaveScript.saveData.hasElixirs[index - SaveScript.bufItemNum] += num;
                            color_cur = 4;
                            if (color_best < color_cur)
                                color_best = color_cur;
                        }
                        else
                        {
                            index = BufItem.GetCodeByCashItem(itemType);
                            bufItemNums[index] += num;
                            SaveScript.saveData.hasBufItems[index] += num;
                            BufItem.GetColorByCode(index, out color_cur);
                            if (color_best < color_cur)
                                color_best = color_cur;
                        }
                        AchievementCtrl.instance.SetAchievementAmount(17, bufItemNums[i]);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        if (GameFuction.GetRandFlag(reinforce2Percent[itemType]))
                        {
                            int code = ReinforceItem2.GetCodeByCashItem(itemType);
                            index = SaveScript.reinforceItemNum + code;
                            num = Random.Range(1, 3);
                            reinforceItemNums[index] += num;

                            if (code > 4 && !isChatSend)
                            {
                                // 10000성 이상 제련석 채팅 알림
                                isChatSend = true;
                                Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이 <" + SaveScript.reinforceItems2[code].name + "> 을 얻었습니다!", 2);
                            }

                            SaveScript.saveData.hasReinforceItems2[index - SaveScript.reinforceItemNum] += num;
                            if (code > 2) color_cur = 5;
                            else color_cur = 4;
                            if (color_best < color_cur)
                                color_best = color_cur;
                        }
                        else
                        {
                            index = ReinforceItem.GetCodeByCashItem(itemType);
                            num = Random.Range(1, 3);
                            reinforceItemNums[index] += num;
                            SaveScript.saveData.hasReinforceItems[index] += num;
                            ReinforceItem.GetColorByCode(index, out color_cur);
                            if (color_best < color_cur)
                                color_best = color_cur;
                        }
                        AchievementCtrl.instance.SetAchievementAmount(18, num);
                    }
                    AchievementCtrl.instance.SetAchievementAmount(31, 1);
                    slotColors[j] = Item.colors[color_best];
                }
                break;
            case 2:
                for (int i = 0; i < count; i++)
                {
                    petType[i] = Random.Range(1, 3);
                    AchievementCtrl.instance.SetAchievementAmount(19, 1);
                    AchievementCtrl.instance.SetAchievementAmount(32, 1);
                    // 퀘스트
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { 32 });
                    switch (petCode[i])
                    {
                        case 7: QuestCtrl.instance.SetMainQuestAmount(new int[] { 106 }); break;
                        case 8: QuestCtrl.instance.SetMainQuestAmount(new int[] { 107 }); break;
                        case 9: QuestCtrl.instance.SetMainQuestAmount(new int[] { 108 }); break;
                    }

                    if (petType[i] == 1) // 광부 슬라임
                    {
                        int index = MinerSlime.FindEmptyPetInven();
                        petCode[i] = MinerSlime.GetCodeByCashItem(itemType);
                        SaveScript.saveData.hasMiners[index] = petCode[i];
                        SaveScript.saveData.hasMinerLevels[index] = 1;
                        SaveScript.saveData.hasMinerExps[index] = 0;
                        MinerSlime.SortPetInven();
                    }
                    else // 모험가 슬라임
                    {
                        int index = AdventurerSlime.FindEmptyPetInven();
                        petCode[i] = AdventurerSlime.GetCodeByCashItem(itemType);
                        SaveScript.saveData.hasAdventurers[index] = petCode[i];
                        SaveScript.saveData.hasAdventurerLevels[index] = 1;
                        SaveScript.saveData.hasAdventurerExps[index] = 0;
                        AdventurerSlime.SortPetInven();
                    }
                }
                break;
            case 3: // 강화석 캡슐
                for (int i = 0; i < count; i++)
                {
                    capsule_code[i] = GameFuction.GetRandFlag(capsulePercent);
                    reinforceOreNum[i] = R_capsuleForces[itemType][capsule_code[i]];
                    SaveScript.saveData.hasReinforceOre += reinforceOreNum[i];
                    AchievementCtrl.instance.SetAchievementAmount(22, reinforceOreNum[i]);
                    AchievementCtrl.instance.SetAchievementAmount(33, 1);
                    // 뒤끝챗
                    if (capsule_code[i] > 2)
                        Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이 < "
                            + SaveScript.cashItems[CashItem.GetItemCode(itemType, itemCode)].name + " > 에서 강화석을 '" + GameFuction.GetNumText(reinforceOreNum[i]) + "'개 얻었습니다!", 2);
                }
                break;
            case 4: // 마나석 캡슐
                for (int i = 0; i < count; i++)
                {
                    capsule_code[i] = GameFuction.GetRandFlag(capsulePercent);
                    manaOreNum[i] = M_capsuleForces[itemType][capsule_code[i]];
                    SaveScript.saveData.manaOre += manaOreNum[i];
                    AchievementCtrl.instance.SetAchievementAmount(23, manaOreNum[i]);
                    AchievementCtrl.instance.SetAchievementAmount(33, 1);
                    // 뒤끝챗
                    if (capsule_code[i] > 2)
                        Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이 < "
                            + SaveScript.cashItems[CashItem.GetItemCode(itemType, itemCode)].name + " > 에서 마나석을 '" + GameFuction.GetNumText(manaOreNum[i]) + "'개 얻었습니다!", 2);
                }
                break;
            case 5: // 랜덤 박스
                for (int i = 0; i < count; i++)
                {
                    int equip = 0;
                    random_type[i] = GameFuction.GetRandFlag(randomBoxPercents[itemType]);
                    if (random_type[i] == 0)
                    {
                        for (int j = 0; j < SaveScript.cashEquipmentNum; j++)
                        {
                            if (SaveScript.saveData.hasCashEquipment[j])
                                equip++;
                        }
                    }
                    // 이미 모든 특수 아이템을 가지고 있기 때문에 다른 아이템 획득
                    if (equip == SaveScript.cashEquipmentNum)
                        random_type[i] = Random.Range(1, randomBoxPercents[itemType].Length);
                    switch (random_type[i])
                    {
                        case 0: // 특수 아이템
                            do
                            {
                                random_code[i] = Random.Range(0, SaveScript.cashEquipmentNum);
                                if (!SaveScript.saveData.hasCashEquipment[random_code[i]])
                                    break;
                            } while (true);
                            SaveScript.saveData.hasCashEquipment[random_code[i]] = true;
                            SaveScript.saveData.isCashEquipmentOn[random_code[i]] = true;
                            // 뒤끝챗
                            Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이 < " + SaveScript.cashEquipments[random_code[i]].name + " > 을 얻었습니다!", 2);
                            break;
                        case 1: // 강화석
                            random_code[i] = GameFuction.GetRandFlag(randomBox_capsurePercent);
                            SaveScript.saveData.hasReinforceOre += randomBox_reinforceNums[itemType][random_code[i]];
                            AchievementCtrl.instance.SetAchievementAmount(22, randomBox_reinforceNums[itemType][random_code[i]]);
                            break;
                        case 2: // 마나석
                            random_code[i] = GameFuction.GetRandFlag(randomBox_capsurePercent);
                            SaveScript.saveData.manaOre += randomBox_manaNums[itemType][random_code[i]];
                            AchievementCtrl.instance.SetAchievementAmount(23, randomBox_manaNums[itemType][random_code[i]]);
                            break;
                        case 3: // 초급, 중급, 고급, 전설 물약
                            random_code[i] = GameFuction.GetRandFlag(randomBox_bufPercent[itemType]);
                            switch (random_code[i])
                            {
                                case 0:
                                case 1:
                                case 2:
                                    random_code2[i] = Random.Range(0, SaveScript.bufItemNum);
                                    SaveScript.saveData.hasBufItems[random_code2[i]]++;
                                    break;
                                case 3:
                                    random_code2[i] = Random.Range(0, SaveScript.bufItemCodeNum);
                                    SaveScript.saveData.hasElixirs[random_code2[i]]++;
                                    break;
                            }
                            AchievementCtrl.instance.SetAchievementAmount(17, 1);
                            break;
                        case 4: // 주문서, 제련석, 고대 유물
                            random_code[i] = GameFuction.GetRandFlag(randomBox_reinforcePercent[itemType]);
                            switch (random_code[i])
                            {
                                case 0:
                                    random_code2[i] = Random.Range(0, SaveScript.reinforceItemNum);
                                    SaveScript.saveData.hasReinforceItems[random_code2[i]]++;
                                    break;
                                case 1:
                                    random_code2[i] = Random.Range(0, SaveScript.reinforceItem2Num - 3);
                                    SaveScript.saveData.hasReinforceItems2[random_code2[i]]++;
                                    break;
                                case 2:
                                    random_code2[i] = 3;
                                    SaveScript.saveData.hasReinforceItems2[random_code2[i]]++;
                                    break;
                            }
                            AchievementCtrl.instance.SetAchievementAmount(18, 1);
                            break;
                    }
                    AchievementCtrl.instance.SetAchievementAmount(34, 1);
                }
                break;
        }

        SaveScript.instance.SaveData_Asyn(true);
    }

    private void SetPassPanel()
    {
        passClickPanel.SetActive(true);
    }

    public void PassAni()
    {
        passClickPanel.SetActive(false);
        Result();
        CashItemShop.instance.animator.Play("PackageOpen", -1, 0.875f);
    }
}
