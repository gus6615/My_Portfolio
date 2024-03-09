using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFusion_Ani : MonoBehaviour
{
    static public ItemFusion_Ani instance;

    public GameObject result_object;
    public Image result_L_item, result_R_item;
    public Image result_L_light, result_R_light;
    public Image result_whiteEffect;
    public Transform result_content;
    public GameObject result_prefab;
    public GameObject passClickPanel;
    public Sprite manaSprite;

    private int manaNum, reinforceNum, elixirNum;
    private int[] reinforceNums, reinforce2Nums;
    private int[] bufItemNums = new int[SaveScript.bufItemTypeNum];
    private float[] elixirPercents = { 0.025f, 0.05f, 0.075f };
    private float[][] bufItemPercents = new float[][]
    {
        new float[] { 0.35f, 0.5f, 0.15f },
        new float[] { 0.1f, 0.6f, 0.3f },
        new float[] { 0f, 0.2f, 0.8f },
    };
    private float[] reinforce2Percents = { 0.00025f, 0.0025f, 0.025f, 0.1f, 0.25f, 0.4f, 0.65f, 1f };
    private float[] reinforce2TypePercents = { 0.8f, 0.15f, 0.05f, 0.0001f, 0.00001f, 0.0000001f };
    private float[][] reinforcePercents = new float[][]
    {
        new float[] { 0.1f, 0.385f, 0.15f, 0.1f, 0.1f, 0.15f, 0.01f, 0.005f }, // 구데기 주문서
        new float[] { 0.03f, 0.16f, 0.35f, 0.03f, 0.03f, 0.35f, 0.035f, 0.015f }, // 중급 주문서
        new float[] { 0f, 0.1f, 0.35f, 0f, 0f, 0.35f, 0.15f, 0.05f }, // 상급 및 창조주 주문서
        new float[] { 0f, 0f, 0.05f, 0f, 0f, 0.05f, 0.5f, 0.4f }, // 1성 주문서
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0.2f, 0.8f }, // 2성 주문서
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f }, // 3성 제련석
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f }, // 5성 제련석
        new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f }, // 10성 제련석
    };

    // 임시 데이터
    Mine_RewardObject data;
    Mine_RewardObject[] datas;

    private void Start()
    {
        instance = this;

        reinforceNums = new int[SaveScript.reinforceItemNum];
        reinforce2Nums = new int[SaveScript.reinforceItem2Num];
        result_object.SetActive(false);
        passClickPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void FusionResult()
    {
        // 결과 출력
        result_object.SetActive(true);
        passClickPanel.SetActive(false);
        ItemFusion.instance.SetAudio(34);

        if (reinforceNum > 0)
        {
            AchievementCtrl.instance.SetAchievementAmount(22, reinforceNum);
            data = Instantiate(result_prefab, result_content).GetComponent<Mine_RewardObject>();
            data.qualityImage.color = new Color(0f, 0f, 0f, 1f);
            data.numText.text = "x" + reinforceNum;
        }

        if (manaNum > 0)
        {
            data = Instantiate(result_prefab, result_content).GetComponent<Mine_RewardObject>();
            data.spriteImage.sprite = manaSprite;
            data.qualityImage.color = new Color(0f, 0f, 1f, 1f);
            data.numText.text = "x" + manaNum;
        }

        if (elixirNum > 0)
        {
            data = Instantiate(result_prefab, result_content).GetComponent<Mine_RewardObject>();
            data.spriteImage.sprite = SaveScript.elixirs[ItemFusion.instance.itemIndex].sprite;
            data.qualityImage.color = SaveScript.elixirs[ItemFusion.instance.itemIndex].color;
            data.numText.text = "x" + elixirNum;
            ItemFusion.instance.SetAudio(33);
        }

        for (int i = bufItemNums.Length - 1; i >= 0; i--)
        {
            if (bufItemNums[i] > 0)
            {
                data = Instantiate(result_prefab, result_content).GetComponent<Mine_RewardObject>();
                data.spriteImage.sprite = SaveScript.bufItems[ItemFusion.instance.itemIndex * SaveScript.bufItemTypeNum + i].sprite;
                data.qualityImage.color = SaveScript.bufItems[ItemFusion.instance.itemIndex * SaveScript.bufItemTypeNum + i].color;
                data.numText.text = "x" + bufItemNums[i];
            }
        }

        for (int i = SaveScript.reinforceItem2Num - 1; i >= 0 ; i--)
        {
            if (reinforce2Nums[i] > 0)
            {
                data = Instantiate(result_prefab, result_content).GetComponent<Mine_RewardObject>();
                data.spriteImage.sprite = SaveScript.reinforceItems2[i].sprite;
                data.qualityImage.color = SaveScript.reinforceItems2[i].color;
                data.numText.text = "x" + reinforce2Nums[i];
                ItemFusion.instance.SetAudio(33);
            }
        }

        for (int i = 0; i < ReinforceItem.rewardOrderCode.Length; i++)
        {
            if (reinforceNums[ReinforceItem.rewardOrderCode[i]] > 0)
            {
                data = Instantiate(result_prefab, result_content).GetComponent<Mine_RewardObject>();
                data.spriteImage.sprite = SaveScript.reinforceItems[ReinforceItem.rewardOrderCode[i]].sprite;
                data.qualityImage.color = SaveScript.reinforceItems[ReinforceItem.rewardOrderCode[i]].color;
                data.numText.text = "x" + reinforceNums[ReinforceItem.rewardOrderCode[i]];
            }
        }

        // 결과 반영
        if (ItemFusion.instance.menuIndex == 0)
        {
            SaveScript.saveData.hasElixirs[ItemFusion.instance.itemIndex] += elixirNum;
            for (int i = 0; i < SaveScript.bufItemTypeNum; i++)
                SaveScript.saveData.hasBufItems[ItemFusion.instance.itemIndex * SaveScript.bufItemTypeNum + i] += bufItemNums[i];
            SaveScript.saveData.hasBufItems[ItemFusion.instance.itemCode] -= ItemFusion.instance.count * 2;
        }
        else
        {
            for (int i = 0; i < reinforce2Nums.Length; i++)
            {
                SaveScript.saveData.hasReinforceItems2[i] += reinforce2Nums[i];
                if (i > 4 && reinforce2Nums[i] > 0)
                {
                    // 10000성 이상 제련석 채팅 알림
                    Chat.instance.SetSystemMessage("[SYSTEM] '" + SaveScript.saveRank.myRankData.nickname + "'님이 <" + SaveScript.reinforceItems2[i].name + "> 을 얻었습니다!", 2);
                }
            }
            for (int i = 0; i < reinforceNums.Length; i++)
                SaveScript.saveData.hasReinforceItems[i] += reinforceNums[i];

            if (ItemFusion.instance.itemCode < SaveScript.reinforceItemNum)
                SaveScript.saveData.hasReinforceItems[ItemFusion.instance.itemCode] -= ItemFusion.instance.count * 2;
            else
                SaveScript.saveData.hasReinforceItems2[ItemFusion.instance.itemCode - SaveScript.reinforceItemNum] -= ItemFusion.instance.count * 2;
        }
        AchievementCtrl.instance.SetAchievementAmount(23, manaNum);
        SaveScript.saveData.manaOre += manaNum;
        SaveScript.saveData.hasReinforceOre += reinforceNum;
        SaveScript.saveData.manaOre -= ItemFusion.instance.manaPrice;
        ItemFusion.instance.SettingFusion();
    }

    public void SetResult()
    {
        // 초기화
        datas = result_content.GetComponentsInChildren<Mine_RewardObject>();
        for (int i = 0; i < datas.Length; i++)
            Destroy(datas[i].gameObject);
        for (int i = 0; i < bufItemNums.Length; i++)
            bufItemNums[i] = 0;
        for (int i = 0; i < SaveScript.reinforceItem2Num; i++)
            reinforce2Nums[i] = 0;
        for (int i = 0; i < SaveScript.reinforceItemNum; i++)
            reinforceNums[i] = 0;
        manaNum = reinforceNum = elixirNum = 0;

        // 결과 결정
        if (ItemFusion.instance.menuIndex == 0)
        {
            for (int i = 0; i < ItemFusion.instance.count; i++)
            {
                if (GameFuction.GetRandFlag(elixirPercents[ItemFusion.instance.itemType]))
                    elixirNum++; // 영약
                else if (GameFuction.GetRandFlag(0.5f))
                {
                    // 일반 물약
                    bufItemNums[GameFuction.GetRandFlag(bufItemPercents[ItemFusion.instance.itemType])]++;
                }
                else if (GameFuction.GetRandFlag(0.2f))
                    manaNum += Random.Range(1, ItemFusion.instance.itemType + 2); // 마나석
                else
                    reinforceNum += (int)(Random.Range(Mathf.Pow(2, ItemFusion.instance.itemType + 2), Mathf.Pow(2, ItemFusion.instance.itemType + 3))); // 강화석
            }
        }
        else
        {
            for (int i = 0; i < ItemFusion.instance.count; i++)
            {
                if (GameFuction.GetRandFlag(reinforce2Percents[ItemFusion.instance.GetReinforceType(ItemFusion.instance.itemCode)]))
                {
                    int code = GameFuction.GetRandFlag(reinforce2TypePercents);
                    reinforce2Nums[code]++;
                }
                else if (GameFuction.GetRandFlag(0.5f))
                {
                    // 일반 주문서
                    reinforceNums[GameFuction.GetRandFlag(reinforcePercents[ItemFusion.instance.GetReinforceType(ItemFusion.instance.itemCode)])]++;
                }
                else if (GameFuction.GetRandFlag(0.2f))
                    manaNum += Random.Range(1, ItemFusion.instance.GetReinforceType(ItemFusion.instance.itemCode) + 1); // 마나석
                else
                    reinforceNum += (int)(Random.Range(Mathf.Pow(2, ItemFusion.instance.GetReinforceType(ItemFusion.instance.itemCode) + 2), 
                        Mathf.Pow(2, ItemFusion.instance.GetReinforceType(ItemFusion.instance.itemCode) + 3))); // 강화석
            }
        }
    }

    public void SetAni()
    {
        Color color = Color.white;

        if (ItemFusion.instance.menuIndex == 0)
        {
            result_L_item.sprite = result_R_item.sprite = SaveScript.bufItems[ItemFusion.instance.itemCode].sprite;
            for (int i = bufItemNums.Length - 1; i >= 0; i--)
            {
                if (bufItemNums[i] > 0)
                {
                    color = Item.colors[i];
                    break;
                }
            }
            if (elixirNum > 0)
                color = SaveScript.elixirs[ItemFusion.instance.itemIndex].color;
        }
        else
        {
            if (ItemFusion.instance.itemCode < SaveScript.reinforceItemNum)
                result_L_item.sprite = result_R_item.sprite = SaveScript.reinforceItems[ItemFusion.instance.itemCode].sprite;
            else
                result_L_item.sprite = result_R_item.sprite = SaveScript.reinforceItems2[ItemFusion.instance.itemCode - SaveScript.reinforceItemNum].sprite;
            for (int i = 0; i < ReinforceItem.rewardOrderCode.Length; i++)
            {
                if (reinforceNums[ReinforceItem.rewardOrderCode[i]] > 0)
                {
                    color = ReinforceItem.GetColorByCode(ReinforceItem.rewardOrderCode[i], out _);
                    break;
                }
            }
            for (int i = reinforce2Nums.Length - 1; i >= 0; i--)
            {
                if (reinforce2Nums[i] > 0)
                {
                    color = SaveScript.reinforceItems2[i].color;
                    break;
                }
            }
        }

        result_L_light.color = result_R_light.color = color;
        result_whiteEffect.color = color;
    }

    public void PassAni()
    {
        ItemFusion.instance.animator.Play("Fusion_Start", -1, 0.925f);
        FusionResult();
    }

    public void Audio_0()
    {
        ItemFusion.instance.SetAudio(36);
    }

    public void Audio_1()
    {
        ItemFusion.instance.SetAudio(37);
    }

    public void CloseButton()
    {
        ItemFusion.instance.SetAudio(0);
        ItemFusion.instance.animator.gameObject.SetActive(false);
        ItemFusion.instance.SettingFusion();
        result_object.SetActive(false);
    }
}
