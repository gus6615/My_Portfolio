using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineRewardInfo : MonoBehaviour
{
    public static MineRewardInfo instance, instance2; // 펫 보상, 펫 시설 보상
    private static float[][] reinforcesPercents = 
    { 
        new float[] { 0.6f, 0.3f, 0.065f, 0.025f, 0.01f, 0f }, // 1층 ~ 6층
        new float[] { 0f, 0.3f, 0.35f, 0.25f, 0.1f, 0.005f }, // 7층 ~ 10층
        new float[] { 0f, 0f, 0f, 0.4f, 0.6f, 0.01f }, // 11층 ~ 12층
    };

    // Reward 관련 UI
    public GameObject passClickPanel;
    public GameObject rewardPefab;
    public Animator rewardAnimation;
    public GameObject rewardInfoObject;
    public Text rewardText;
    public Transform rewardInfoPanel;
    public int type; // 0 = 펫 보상, 1 = 펫 시설 보상

    private void Start()
    {
        passClickPanel.SetActive(false);
        rewardInfoObject.SetActive(false);
        if(type == 0)
            instance = this;
        else
            instance2 = this;
    }

    // 저장된 reward 변수를 saveData로 변경 & 애니메이션 설정
    public void GetReward()
    {
        Mine_RewardObject data;
        Mine_RewardObject[] datas = rewardInfoObject.GetComponentsInChildren<Mine_RewardObject>();
        for (int i = 0; i < datas.Length; i++) Destroy(datas[i].gameObject);
        rewardInfoObject.SetActive(true);
        passClickPanel.SetActive(false);

        if (type == 0)
        {
            // 펫 보상
            rewardText.text = "광부 상자 : " + GameFuction.GetNumText(MineMap.minerRewardNum) + "개(보너스 상자 +" + GameFuction.GetNumText(MineMap.minerPlusRewardNum) + "개) " +
                "/ 모험가 상자 : " + GameFuction.GetNumText(MineMap.adventurerRewardNum) + "개(보너스 상자 +" + GameFuction.GetNumText(MineMap.adventurerPlusRewardNum) + "개)";
            AchievementCtrl.instance.SetAchievementAmount(35, MineMap.minerRewardNum + MineMap.adventurerRewardNum);
            QuestCtrl.instance.SetMainQuestAmount(new int[] { 37 }, MineMap.minerRewardNum + MineMap.adventurerRewardNum);
            MineMap.minerRewardNum = MineMap.minerPlusRewardNum = MineMap.adventurerRewardNum = MineMap.adventurerPlusRewardNum = 0;

            if (MineMap.instance.rewardReinforceOre > 0)
            {
                SaveScript.saveData.hasReinforceOre += MineMap.instance.rewardReinforceOre;
                AchievementCtrl.instance.SetAchievementAmount(22, MineMap.instance.rewardReinforceOre);
                data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                data.qualityImage.color = new Color(0f, 0f, 0f, 1f);
                data.numText.text = "x" + GameFuction.GetNumText(MineMap.instance.rewardReinforceOre);
                MineMap.instance.rewardReinforceOre = 0;
            }

            // 광물 아이템 출력
            int count = SaveScript.qualityNum - 1;
            for (int i = 0; i < MineMap.instance.rewardJems.Length; i++)
            {
                if (MineMap.instance.rewardJems[i] != 0 && count == SaveScript.jems[i].quality)
                {
                    SaveScript.saveData.hasItemNums[i] += MineMap.instance.rewardJems[i];
                    data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                    data.spriteImage.sprite = SaveScript.jems[i].jemSprite;
                    data.qualityImage.color = SaveScript.qualityColors_weak[SaveScript.jems[i].quality];
                    data.numText.text = "x" + GameFuction.GetNumText(MineMap.instance.rewardJems[i]);
                    MineMap.instance.rewardJems[i] = 0;
                }

                if (i == MineMap.instance.rewardJems.Length - 1)
                {
                    if (count == 0)
                        break;
                    else
                    {
                        i = -1;
                        count--;
                    }
                }
            }

            // 영약 아이템 출력
            for (int i = 0; i < MineMap.instance.rewardElixirItems.Length; i++)
            {
                if (MineMap.instance.rewardElixirItems[i] != 0)
                {
                    SaveScript.saveData.hasElixirs[i] += MineMap.instance.rewardElixirItems[i];
                    AchievementCtrl.instance.SetAchievementAmount(17, MineMap.instance.rewardElixirItems[i]);
                    data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                    data.spriteImage.sprite = SaveScript.elixirs[i].sprite;
                    data.qualityImage.color = SaveScript.elixirs[i].color;
                    data.numText.text = "x" + GameFuction.GetNumText(MineMap.instance.rewardElixirItems[i]);
                    MineMap.instance.rewardElixirItems[i] = 0;
                }
            }

            // 버프 아이템 출력
            count = SaveScript.bufItemTypeNum - 1;
            for (int i = 0; i < MineMap.instance.rewardBufItems.Length; i++)
            {
                if (MineMap.instance.rewardBufItems[i] != 0 && Item.colors[count] == SaveScript.bufItems[i].color)
                {
                    SaveScript.saveData.hasBufItems[i] += MineMap.instance.rewardBufItems[i];
                    AchievementCtrl.instance.SetAchievementAmount(17, MineMap.instance.rewardBufItems[i]);
                    data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                    data.spriteImage.sprite = SaveScript.bufItems[i].sprite;
                    data.qualityImage.color = SaveScript.bufItems[i].color;
                    data.numText.text = "x" + GameFuction.GetNumText(MineMap.instance.rewardBufItems[i]);
                    MineMap.instance.rewardBufItems[i] = 0;
                }

                if (i == MineMap.instance.rewardBufItems.Length - 1)
                {
                    if (count == 0)
                        break;
                    else
                    {
                        i = -1;
                        count--;
                    }
                }
            }

            // 제련석 아이템 출력
            for (int i = 0; i < MineMap.instance.rewardReinforceItems2.Length; i++)
            {
                if (MineMap.instance.rewardReinforceItems2[i] != 0)
                {
                    SaveScript.saveData.hasReinforceItems2[i] += MineMap.instance.rewardReinforceItems2[i];
                    AchievementCtrl.instance.SetAchievementAmount(18, MineMap.instance.rewardReinforceItems2[i]);
                    data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                    data.spriteImage.sprite = SaveScript.reinforceItems2[i].sprite;
                    data.qualityImage.color = SaveScript.reinforceItems2[i].color;
                    data.numText.text = "x" + GameFuction.GetNumText(MineMap.instance.rewardReinforceItems2[i]);
                    MineMap.instance.rewardReinforceItems2[i] = 0;
                }
            }

            // 강화 아이템 출력
            count = Item.colors.Length - 1;
            for (int i = 0; i < MineMap.instance.rewardReinforceItems.Length; i++)
            {
                if (MineMap.instance.rewardReinforceItems[i] != 0 && Item.colors[count] == SaveScript.reinforceItems[i].color)
                {
                    SaveScript.saveData.hasReinforceItems[i] += MineMap.instance.rewardReinforceItems[i];
                    AchievementCtrl.instance.SetAchievementAmount(18, MineMap.instance.rewardReinforceItems[i]);
                    data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                    data.spriteImage.sprite = SaveScript.reinforceItems[i].sprite;
                    data.qualityImage.color = SaveScript.reinforceItems[i].color;
                    data.numText.text = "x" + GameFuction.GetNumText(MineMap.instance.rewardReinforceItems[i]);
                    MineMap.instance.rewardReinforceItems[i] = 0;
                }

                if (i == MineMap.instance.rewardReinforceItems.Length - 1)
                {
                    if (count == 0)
                        break;
                    else
                    {
                        i = -1;
                        count--;
                    }
                }
            }
        }
        else
        {
            // 펫 시설 보상
            int[] elixirs = new int[SaveScript.bufItemCodeNum];
            int[] reinforces = new int[6]; // 1성, 2성, 3성, 5성, 10성, 100성
            int manaNum = Random.Range(MineFacilityUI.instance.manaOre_min, MineFacilityUI.instance.manaOre_max + 1);
            int elixirNum = -1;
            if(SaveScript.saveData.facility_level / 10 > 0) elixirNum = MineFacilityUI.instance.elixirNum;
            int reinforceNum = -1;
            if (SaveScript.saveData.facility_level / 10 > 1) reinforceNum = MineFacilityUI.instance.reinforceItemNum;

            // 마나석 출력
            AchievementCtrl.instance.SetAchievementAmount(23, manaNum);
            SaveScript.saveData.manaOre += manaNum;
            data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
            data.qualityImage.color = new Color(0f, 0f, 0f, 0f);
            data.spriteImage.sprite = MineFacilityUI.instance.manaSprite.sprite;
            data.qualityImage.color = new Color(0f, 0f, 1f, 1f);
            data.numText.text = "x" + GameFuction.GetNumText(manaNum);

            // 영약 아이템 출력
            if (elixirNum != -1)
            {
                AchievementCtrl.instance.SetAchievementAmount(17, elixirNum);
                for (int i = 0; i < elixirNum; i++)
                    elixirs[GameFuction.GetRandFlag(Elixir.dropPercents)]++;

                for (int i = 0; i < elixirs.Length; i++)
                {
                    if (elixirs[i] > 0)
                    {
                        SaveScript.saveData.hasElixirs[i] += elixirs[i];
                        data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                        data.spriteImage.sprite = SaveScript.elixirs[i].sprite;
                        data.qualityImage.color = SaveScript.elixirs[i].color;
                        data.numText.text = "x" + GameFuction.GetNumText(elixirs[i]);
                    }
                }
            }

            // 강화 아이템 출력
            if (reinforceNum != -1)
            {
                AchievementCtrl.instance.SetAchievementAmount(18, reinforceNum);
                for (int i = 0; i < reinforceNum; i++)
                    reinforces[GameFuction.GetRandFlag(reinforcesPercents[GetRewardType(SaveScript.saveData.pickLevel)])]++;

                for (int i = reinforces.Length - 1; i >= 0; i--)
                {
                    if (reinforces[i] > 0)
                    {
                        data = Instantiate(rewardPefab, rewardInfoPanel).GetComponent<Mine_RewardObject>();
                        data.numText.text = "x" + GameFuction.GetNumText(reinforces[i]);
                        if (i < 2)
                        {
                            // 1성 및 2성
                            SaveScript.saveData.hasReinforceItems[6 + i] += reinforces[i];
                            data.spriteImage.sprite = SaveScript.reinforceItems[6 + i].sprite;
                            data.qualityImage.color = SaveScript.reinforceItems[6 + i].color;
                        }
                        else
                        {
                            // 3성, 5성, 10성
                            SaveScript.saveData.hasReinforceItems2[i - 2] += reinforces[i];
                            data.spriteImage.sprite = SaveScript.reinforceItems2[i - 2].sprite;
                            data.qualityImage.color = SaveScript.reinforceItems2[i - 2].color;
                        }
                    }
                }
            }

            MineFacilityUI.instance.SetUI();
            SaveScript.instance.SaveData_Asyn(false);
        }
    }

    public void PlayOpenBoxAudio()
    {
        Mine.instance.audio.clip = SaveScript.SEs[15];
        Mine.instance.audio.Play();
    }

    public void GetItemAudio()
    {
        Mine.instance.audio.clip = SaveScript.SEs[9];
        Mine.instance.audio.Play();
    }

    public void PassAni()
    {
        passClickPanel.SetActive(false);
        rewardAnimation.Play("RewardInfo_Reward", -1, 0.85f);
        GetReward();
    }

    // reward UI 닫기 버튼
    public void CloseReward()
    {
        if(type == 0)
        {
            instance.rewardAnimation.SetBool("isGetReward", false);
            instance.rewardInfoObject.SetActive(false);
        }
        else
        {
            instance2.rewardAnimation.SetBool("isGetReward", false);
            instance2.rewardInfoObject.SetActive(false);
        }
        Mine.instance.SetAudio(0);
    }

    private int GetRewardType(int level)
    {
        if (level < 6) return 0;
        else if (level < 10) return 1;
        else return 2;
    }
}
