using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class D_0_NPCUICtrl : MonoBehaviour
{
    public static D_0_NPCUICtrl instance;
    private static float[] specialPercent = { 0f, 0.05f, 0.1f, 0.25f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // StageNum 변경
    private static float[] bufItemTypePercent = { 0.4f, 0.4f, 0.2f };
    private static float[] bufItemPercent = { 0.08f, 0.08f, 0.08f, 0.08f, 0.08f, 0.08f, 0.08f, 0.08f, 0.08f, 0.08f, 0.03f, 0.01f, 0.08f, 0.08f };
    private static float[] reinforcePercent = { 0.05f, 0.2f, 0.2f, 0.05f, 0.05f, 0.2f, 0.15f, 0.1f };
    private static float[] reinforce2Percent = { 0.85f, 0.1f, 0.05f, 0.005f, 0.0005f, 0.000001f };
    private static int[] manaNumAsType = { 10, 20, 30, 50, 100, 500, 1000, 3000, 5000, 10000, 30000, 100000, 500000 }; // StageNum 변경
    private static float[][] petPercents = // StageNum 변경
    {
        new float[] { 0.7f, 0.3f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[] { 0.35f, 0.5f, 0.15f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[] { 0f, 0.35f, 0.55f, 0.1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f }, // B
        new float[] { 0f, 0f, 0.5f, 0.45f, 0.05f, 0f, 0f, 0f, 0f, 0f, 0f }, // A
        new float[] { 0f, 0f, 0f, 0.9f, 0.1f, 0.025f, 0f, 0f, 0f, 0f, 0f }, // 7층 - A ~ S (5)
        new float[] { 0f, 0f, 0f, 0.85f, 0.15f, 0.03f, 0.001f, 0f, 0f, 0f, 0f }, // 8층 - S ~ SS (10)
        new float[] { 0f, 0f, 0f, 0f, 0f, 1f, 0.005f, 0.0005f, 0.0001f, 0f, 0f }, // 9층 - SS (20)
        new float[] { 0f, 0f, 0f, 0f, 0f, 1f, 0.01f, 0.001f, 0.0001f, 0f, 0f }, // 10층 - SS ~ SSS (40)
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0.001f, 0.00005f, 0.000001f, 0f }, // 11층 - SSS (90)
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0.0015f, 0.0001f, 0.000005f, 0f }, // 12층 - SSS ~ U (300)
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0.0025f, 0.0002f, 0.000025f, 0f }, // 13층 - U (650)
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0.00025f, 0.00005f, 0.000001f }, // 14층 - U ~ UU (1500)
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 0.0005f, 0.000075f, 0.000002f }, // 15층 - U ~ UU (5000)
    };
    private static long[] petPrices = { 3000, 5000, 10000, 30000, 100000, 300000, 1000000, 3000000, 50000000, 1000000000, 999999999 };

    // 던전 NPC 상점 전용 변수
    public GameObject NPCShopObject;
    public Text NPCShopReinforceText;
    public Transform NPCShopPanel;
    public GameObject NPCSelectPrefab;
    public Dungeon_0_NPC npc;
    private int petType;

    // 임시 데이터들
    Order[] datas;
    Order data;
    Image[] images;
    Text[] texts;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
        NPCShopObject.SetActive(false);
    }

    public void InitNPCShop(Dungeon_0_NPC _npc)
    {
        float plusPercent = (1f + GameFuction.GetManaUpgradeForce(1)) * (1f + SaveScript.npcPercentsAsType[DungeonCreater.dungeon_0_type]);
        int itemNum = Random.Range(4, 7);
        bool isManaOre = false;
        bool isPet = false;
        if (GameFuction.GetRandFlag(0.7f)) isManaOre = true;
        if (GameFuction.GetRandFlag(0.3f)) isPet = true;
        petType = Random.Range(0, 2);

        npc = _npc;
        NPCShopObject.SetActive(true);
        NPCShopPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        datas = NPCShopPanel.GetComponentsInChildren<Order>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);

        // 마나석
        if (isManaOre)
        {
            data = Instantiate(NPCSelectPrefab, NPCShopPanel).GetComponent<Order>();
            data.GetComponentInChildren<Button>().onClick.AddListener(BuyNPCItem);
            data.order = 4;
            data.order2 = manaNumAsType[npc.type] * Random.Range(1, 4);
        }

        // 펫
        if (isPet)
        {
            data = Instantiate(NPCSelectPrefab, NPCShopPanel).GetComponent<Order>();
            data.GetComponentInChildren<Button>().onClick.AddListener(BuyNPCItem);
            data.order = 5;
            data.order2 = GameFuction.GetRandFlag(GameFuction.GetArrayPlus(plusPercent, petPercents[DungeonCreater.dungeon_0_type]));
        }

        for (int i = 0; i < itemNum; i++)
        {
            data = Instantiate(NPCSelectPrefab, NPCShopPanel).GetComponent<Order>();
            data.GetComponentInChildren<Button>().onClick.AddListener(BuyNPCItem);

            if (GameFuction.GetRandFlag(specialPercent[DungeonCreater.dungeon_0_type]))
            {
                if (i < itemNum / 2) // 영약 아이템
                {
                    data.order = 2;
                    data.order2 = GameFuction.GetRandFlag(bufItemPercent);
                }
                else if (i < itemNum) // 제련석 & 고대 유물
                {
                    data.order = 3;
                    data.order2 = GameFuction.GetRandFlag(GameFuction.GetArrayPlus(plusPercent, reinforce2Percent));
                }
                continue;
            }

            if (i < itemNum / 2) // 물약 아이템
            {
                data.order = 0;
                data.order2 = GameFuction.GetRandFlag(bufItemPercent) * SaveScript.bufItemTypeNum 
                    + GameFuction.GetRandFlag(GameFuction.GetArrayPlus(plusPercent, bufItemTypePercent));
            }
            else if (i < itemNum) // 주문서 - 강화석
            {
                data.order = 1;
                data.order2 = GameFuction.GetRandFlag(reinforcePercent);
            }
        }

        SetNPCShop();
    }

    public void SetNPCShop()
    {
        NPCShopReinforceText.text = GameFuction.GetNumText(SaveScript.saveData.hasReinforceOre);

        datas = NPCShopPanel.GetComponentsInChildren<Order>();
        for (int i = 0; i < datas.Length; i++)
        {
            images = datas[i].GetComponentsInChildren<Image>();
            texts = datas[i].GetComponentsInChildren<Text>();

            texts[1].text = GameFuction.GetNumText(NPCItemPrice(datas[i].order, datas[i].order2));
            if (SaveScript.saveData.hasReinforceOre >= NPCItemPrice(datas[i].order, datas[i].order2))
                texts[1].color = Color.green;
            else
                texts[1].color = Color.red;

            switch (datas[i].order)
            {
                case 0:
                    images[2].sprite = SaveScript.bufItems[datas[i].order2].sprite;
                    texts[0].text = SaveScript.bufItems[datas[i].order2].name;
                    break;
                case 1:
                    images[2].sprite = SaveScript.reinforceItems[datas[i].order2].sprite;
                    texts[0].text = SaveScript.reinforceItems[datas[i].order2].name;
                    break;
                case 2:
                    images[2].sprite = SaveScript.elixirs[datas[i].order2].sprite;
                    texts[0].text = SaveScript.elixirs[datas[i].order2].name;
                    break;
                case 3:
                    images[2].sprite = SaveScript.reinforceItems2[datas[i].order2].sprite;
                    texts[0].text = SaveScript.reinforceItems2[datas[i].order2].name;
                    break;
                case 4:
                    texts[0].text = "마나석 상자 [ " + GameFuction.GetNumText(datas[i].order2) + " 개 ]";
                    break;
                case 5:
                    if (petType == 0)
                    {
                        images[2].sprite = MineSlime.miner_faceSprites[datas[i].order2];
                        texts[0].text = "[" + MineSlime.qualityNames[datas[i].order2] + "등급] " + MinerSlime.names[datas[i].order2];
                    }
                    else
                    {
                        images[2].sprite = MineSlime.adventurer_faceSprites[datas[i].order2];
                        texts[0].text = "[" + MineSlime.qualityNames[datas[i].order2] + "등급] " + AdventurerSlime.names[datas[i].order2];
                    }
                    break;
            }
        }
    }

    public void CloseNPCShop()
    {
        npc.animator.SetInteger("ActionType", 2);
        npc = null;
        NPCShopObject.SetActive(false);
    }

    public void BuyNPCItem()
    {
        data = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Order>();

        if (SaveScript.saveData.hasReinforceOre >= NPCItemPrice(data.order, data.order2))
        {
            switch (data.order)
            {
                case 0: 
                    SaveScript.saveData.hasBufItems[data.order2]++;
                    AchievementCtrl.instance.SetAchievementAmount(17, 1);
                    break;
                case 1: 
                    SaveScript.saveData.hasReinforceItems[data.order2]++;
                    AchievementCtrl.instance.SetAchievementAmount(18, 1);
                    break;
                case 2:
                    SaveScript.saveData.hasElixirs[data.order2]++;
                    AchievementCtrl.instance.SetAchievementAmount(17, 1);
                    break;
                case 3:
                    SaveScript.saveData.hasReinforceItems2[data.order2]++;
                    AchievementCtrl.instance.SetAchievementAmount(18, 1);
                    break;
                case 4:
                    AchievementCtrl.instance.SetAchievementAmount(23, data.order2);
                    SaveScript.saveData.manaOre += data.order2;
                    PlayerScript.instance.manaOre += data.order2;
                    break;
                case 5:
                    int index;
                    if(petType == 0)
                    {
                        if((index = MinerSlime.FindEmptyPetInven()) == -1)
                        {
                            DungeonUICtrl.instance.AudioPlay(0);
                            PetInven.instance.ShowPetInven(0, data.order2);
                            return;
                        }
                        else
                        {
                            SaveScript.saveData.hasMiners[index] = data.order2;
                            SaveScript.saveData.hasMinerLevels[index] = 1;
                            SaveScript.saveData.hasMinerExps[index] = 0;
                            AchievementCtrl.instance.SetAchievementAmount(19, 1);
                            MinerSlime.SortPetInven();
                        }
                    }
                    else
                    {
                        if ((index = AdventurerSlime.FindEmptyPetInven()) == -1)
                        {
                            DungeonUICtrl.instance.AudioPlay(0);
                            PetInven.instance.ShowPetInven(1, data.order2);
                            return;
                        }
                        else
                        {
                            SaveScript.saveData.hasAdventurers[index] = data.order2;
                            SaveScript.saveData.hasAdventurerLevels[index] = 1;
                            SaveScript.saveData.hasAdventurerExps[index] = 0;
                            AchievementCtrl.instance.SetAchievementAmount(19, 1);
                            MinerSlime.SortPetInven();
                        }
                    }
                    break;
            }

            Buy();
        }
        else
            DungeonUICtrl.instance.AudioPlay(2);
    }

    public void Buy()
    {
        DungeonUICtrl.instance.AudioPlay(4);
        SaveScript.saveData.hasReinforceOre -= NPCItemPrice(data.order, data.order2);
        AchievementCtrl.instance.SetAchievementAmount(16, 1);
        Destroy(data.gameObject);
        SetNPCShop();
    }
    
    public long NPCItemPrice(int order1, int order2)
    {
        long price = 0;

        switch (order1)
        {
            case 0:
                int bufType = order2 % SaveScript.bufItemTypeNum;

                if (order2 / SaveScript.bufItemTypeNum == 11) price = 5000 + 5000 * bufType;
                else if (order2 / SaveScript.bufItemTypeNum == 10) price = 2000 + 1000 * bufType;
                else price = 200 + 400 * bufType;
                break;
            case 1:
                switch (order2)
                {
                    case 0: price = 200; break;
                    case 1: price = 500; break;
                    case 2: price = 1500; break;
                    case 3: price = 150; break;
                    case 4: price = 150; break;
                    case 5: price = 1000; break;
                    case 6: price = 3000; break;
                    case 7: price = 7000; break;
                }
                break;
            case 2:
                if (order2 == 11) price = 50000;
                else if (order2  == 10) price = 20000;
                else price = 3000;
                break;
            case 3:
                switch (order2)
                {
                    case 0: price = 20000; break;
                    case 1: price = 50000; break;
                    case 2: price = 300000; break;
                    case 3: price = 1000000; break;
                    case 4: price = 100000000; break;
                    case 5: price = 100000000000; break;
                }
                break;
            case 4: price = 100 * order2; break;
            case 5: price = petPrices[order2]; break;
        }

        return price;
    }
}
