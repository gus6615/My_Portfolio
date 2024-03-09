using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineMap : MonoBehaviour
{
    const float MAX_X = 8f;

    public static MineMap instance;
    public static bool isSetMineTime;
    public static bool isCanGetReward;
    public static List<int> rewardMinerIndexs = new List<int>();
    public static List<int> rewardAdventurerIndexs = new List<int>(); 

    public float[] minerSlime_createY = new float[] { -2.15f, -2.15f, -2f, -2.15f, -2f, -1.6f, -1.2f, -0.75f, -0.55f, -0.25f, 0.95f };
    public float[] adventurerSlime_createY = new float[] { -2.15f, -2.15f, -2.15f, -2.05f, -1.6f, -1.25f, -1.3f, -1.1f, -0.85f, -0.55f, 0.175f };

    public GameObject[] minerPrefabs, adventurerPrefabs;
    public List<MinerSlime> minerObjects = new List<MinerSlime>();
    public List<AdventurerSlime> adventurerObjects = new List<AdventurerSlime>();
    public Transform petObjectTr;
    public GameObject selectedPet;
    public int selectedPetCode; // 펫 코드
    public int selectedPetType; // 0 = Miner, 1 = Adventurer
    public int selectedPetIndex; // SaveData에 존재하는 pet 관련 변수 index
    static public int minerRewardNum, adventurerRewardNum, minerPlusRewardNum, adventurerPlusRewardNum;

    // 현재 일하는 슬라임 클릭 관련 UI
    public GameObject selectedPetUI;
    public Image selectedPetUI_face, selectedPetUI_reward;
    public Text selectedPetUI_name, selectedPetUI_time, selectedPetUI_rewardNum;
    public Button selectedPetUI_button;
    public Button getAllRewardButton;
    public long[] rewardJems;
    public int[] rewardBufItems, rewardElixirItems;
    public int[] rewardReinforceItems, rewardReinforceItems2;
    public long rewardReinforceOre;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        selectedPetUI.SetActive(false);
        rewardJems = new long[SaveScript.totalItemNum];
        rewardBufItems = new int[SaveScript.bufItemNum];
        rewardElixirItems = new int[SaveScript.bufItemCodeNum];
        rewardReinforceItems = new int[SaveScript.reinforceItemNum];
        rewardReinforceItems2 = new int[SaveScript.reinforceItem2Num];
        rewardReinforceOre = 0;

        SetMainUI();

        for (int i = 0; i < SaveScript.mineSlotMaxNum; i++)
        {
            int code = SaveScript.saveData.hasOnMiners[i];
            MinerSlime minerSlime;
            if (code != -1)
            {
                minerSlime = Instantiate(minerPrefabs[code], new Vector3(Random.Range(-MAX_X, MAX_X), minerSlime_createY[code], 0f), Quaternion.identity, petObjectTr).GetComponent<MinerSlime>();
                minerSlime.code = SaveScript.saveData.hasOnMiners[i];
                minerSlime.level = SaveScript.saveData.hasOnMinerLevels[i];
                minerSlime.exp = SaveScript.saveData.hasOnMinerExps[i];
                minerSlime.index = i;
                minerSlime.Init();
                if (SaveScript.saveData.hasMinerRewards[i] != 0) minerSlime.SetRewardObject(true);
                else minerSlime.SetRewardObject(false);
            }
            else
            {
                minerSlime = new MinerSlime();
                minerSlime.code = -1;
            }
            minerObjects.Add(minerSlime);

            code = SaveScript.saveData.hasOnAdventurers[i];
            AdventurerSlime adventurerSlime;
            if (code != -1)
            {
                adventurerSlime = Instantiate(adventurerPrefabs[code], new Vector3(Random.Range(-MAX_X, MAX_X), adventurerSlime_createY[code], 0f), Quaternion.identity, petObjectTr).GetComponent<AdventurerSlime>();
                adventurerSlime.code = SaveScript.saveData.hasOnAdventurers[i];
                adventurerSlime.level = SaveScript.saveData.hasOnAdventurerLevels[i];
                adventurerSlime.exp = SaveScript.saveData.hasOnAdventurerExps[i];
                adventurerSlime.index = i;
                adventurerSlime.Init();
                if (SaveScript.saveData.hasAdventurerRewards[i] != 0) adventurerSlime.SetRewardObject(true);
                else adventurerSlime.SetRewardObject(false);
            }
            else
            {
                adventurerSlime = new AdventurerSlime();
                adventurerSlime.code = -1;
            }
            adventurerObjects.Add(adventurerSlime);
        }
    }

    private void Update()
    {
        if (isSetMineTime) SetMineTime();

        CheckClickPet();
    }

    // 현재 상황 갱신
    private void SetMineTime()
    {
        isSetMineTime = false;
        CheckReward();
        SetMainUI();

        if (selectedPet != null)
        {
            // SelectPet UI 수정
            if (selectedPetType == 0) selectedPetUI_time.text = "[ " + GameFuction.GetTimeText(SaveScript.saveData.hasMinerTimes[selectedPetIndex]) + " ]";
            else if (selectedPetType == 1) selectedPetUI_time.text = "[ " + GameFuction.GetTimeText(SaveScript.saveData.hasAdventurerTimes[selectedPetIndex]) + " ]";

            for (int i = 0; i < rewardMinerIndexs.Count; i++)
                if (selectedPetType == 0 && rewardMinerIndexs[i] == selectedPetIndex)
                    selectedPetUI_rewardNum.text = "[ 현재 상자 개수 : " + SaveScript.saveData.hasMinerRewards[selectedPetIndex] + " / " + SaveScript.maxPetReward + " ]";

            for (int i = 0; i < rewardAdventurerIndexs.Count; i++)
                if (selectedPetType == 1 && rewardAdventurerIndexs[i] == selectedPetIndex)
                    selectedPetUI_rewardNum.text = "[ 현재 상자 개수 : " + SaveScript.saveData.hasAdventurerRewards[selectedPetIndex] + " / " + SaveScript.maxPetReward + " ]";
        }

        rewardMinerIndexs.Clear();
        rewardAdventurerIndexs.Clear();
    }

    private void SetMainUI()
    {
        isCanGetReward = false;

        for (int i = 0; i < SaveScript.saveData.hasMinerRewards.Length; i++)
        {
            if (SaveScript.saveData.hasMinerRewards[i] != 0)
            {
                isCanGetReward = true;
                break;
            }
        }
        for (int i = 0; i < SaveScript.saveData.hasAdventurerRewards.Length; i++)
        {
            if (SaveScript.saveData.hasAdventurerRewards[i] != 0)
            {
                isCanGetReward = true;
                break;
            }
        }

        if(isCanGetReward)
        {
            getAllRewardButton.gameObject.SetActive(true);
        }
        else
        {
            getAllRewardButton.gameObject.SetActive(false);
        }
    }

    // 터치 및 클릭 시 펫을 클릭 했는지 확인 후 UI ON/OFF
    private void CheckClickPet()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount > 0 && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit)
            {
                MineSlime mineSlime = hit.transform.GetComponent<MineSlime>();
                if (mineSlime != null)
                {
                    MinerSlime miner = mineSlime.GetComponent<MinerSlime>();
                    AdventurerSlime adventurer = mineSlime.GetComponent<AdventurerSlime>();

                    if (miner != null)
                    {
                        selectedPetCode = miner.code;
                        selectedPetIndex = miner.index;
                        selectedPetType = 0;
                    }
                    else if(adventurer != null)
                    {
                        selectedPetCode = adventurer.code;
                        selectedPetIndex = adventurer.index;
                        selectedPetType = 1;
                    }
                    SetActiveSelectedPet(false);
                    selectedPet = mineSlime.gameObject;
                    SetActiveSelectedPet(true);
                }
            }
            else SetActiveSelectedPet(false);
        }
    }

    public void SetActiveSelectedPet(bool _isActive)
    {
        selectedPetUI.SetActive(_isActive);
        if(selectedPet != null) selectedPet.GetComponent<MineSlime>().SetSelectedPet(_isActive);

        if (_isActive)
        {
            selectedPetUI.transform.position = Camera.main.WorldToScreenPoint(selectedPet.transform.position + Vector3.up);
            if(selectedPetType == 0)
            {
                MinerSlime miner = selectedPet.GetComponent<MinerSlime>();
                selectedPetUI_face.sprite = MineSlime.miner_defaultSprites[selectedPetCode];
                selectedPetUI_reward.sprite = MineSlime.rewardSprites[selectedPetCode];
                selectedPetUI_name.text = "Lv. " + miner.level + " [ " + miner.name + " ]";
                selectedPetUI_time.text = "[ " + GameFuction.GetTimeText(SaveScript.saveData.hasMinerTimes[selectedPetIndex]) + " ]";
                selectedPetUI_rewardNum.text = "[ " + "현재 상자 개수 : " + SaveScript.saveData.hasMinerRewards[selectedPetIndex] + " / " + SaveScript.maxPetReward + " ]";
            }
            else if (selectedPetType == 1)
            {
                AdventurerSlime adventurer = selectedPet.GetComponent<AdventurerSlime>();
                selectedPetUI_face.sprite = MineSlime.adventurer_defaultSprites[selectedPetCode];
                selectedPetUI_reward.sprite = MineSlime.rewardSprites[selectedPetCode];
                selectedPetUI_name.text = "Lv. " + adventurer.level + " [ " + adventurer.name + " ]";
                selectedPetUI_time.text = "[ " + GameFuction.GetTimeText(SaveScript.saveData.hasAdventurerTimes[selectedPetIndex]) + " ]";
                selectedPetUI_rewardNum.text = "[ " + "현재 상자 개수 : " + SaveScript.saveData.hasAdventurerRewards[selectedPetIndex] + " / " + SaveScript.maxPetReward + " ]";
            }
        }
        else
        { 
            selectedPet = null;
        }
    }

    public void CheckReward()
    {
        for (int i = 0; i < rewardMinerIndexs.Count; i++)
        {
            for (int j = 0; j < minerObjects.Count; j++)
            {
                if (minerObjects[j] != null && rewardMinerIndexs[i] == minerObjects[j].index)
                {
                    minerObjects[j].StartReward();
                    minerObjects[j].SetRewardObject(true);
                }
            }
        }
        for (int i = 0; i < rewardAdventurerIndexs.Count; i++)
        {
            for (int j = 0; j < adventurerObjects.Count; j++)
            {
                if (adventurerObjects[j] != null && rewardAdventurerIndexs[i] == adventurerObjects[j].index)
                {
                    adventurerObjects[j].StartReward();
                    adventurerObjects[j].SetRewardObject(true);
                }
            }
        }
    }

    // 모든 펫 보상 받기
    public void GetAllReward()
    {
        StopCoroutine("DelayAllReward");
        StartCoroutine("DelayAllReward");

        for (int i = 0; i < SaveScript.saveData.hasMinerRewards.Length; i++)
        {
            if(SaveScript.saveData.hasMinerRewards[i] != 0 && SaveScript.saveData.hasOnMiners[i] != -1)
            {
                int num = SaveScript.saveData.hasMinerRewards[i];
                minerRewardNum += num;

                for (int j = 0; j < num; j++)
                {
                    AddJem(i);
                    if(GameFuction.GetRandFlag(SaveScript.mineUpgradePercents[2] * SaveScript.saveData.minerUpgrades[2]))
                    {
                        AddJem(i);
                        minerRewardNum++;
                        minerPlusRewardNum++;
                    }
                }
                SaveScript.saveData.hasMinerRewards[i] = 0;
                minerObjects[i].SetRewardObject(false);
            }
        }

        for (int i = 0; i < SaveScript.saveData.hasAdventurerRewards.Length; i++)
        {
            if (SaveScript.saveData.hasAdventurerRewards[i] != 0)
            {
                int num = SaveScript.saveData.hasAdventurerRewards[i];
                float[] bufItem_percents = (float[])adventurerObjects[i].bufItem_percents.Clone();
                float[] reinforceItem_percents = (float[])adventurerObjects[i].reinforceItem_percents.Clone();
                adventurerRewardNum += num;

                for (int j = 0; j < num; j++)
                {
                    AddItem(bufItem_percents, reinforceItem_percents, i);
                    if (GameFuction.GetRandFlag(SaveScript.mineUpgradePercents[2] * SaveScript.saveData.adventurerUpgrades[2]))
                    {
                        AddItem(bufItem_percents, reinforceItem_percents, i);
                        adventurerRewardNum++;
                        adventurerPlusRewardNum++;
                    }
                }
                SaveScript.saveData.hasAdventurerRewards[i] = 0;
                adventurerObjects[i].SetRewardObject(false);
            }
        }

        SetActiveSelectedPet(false);
        MineRewardInfo.instance.passClickPanel.SetActive(true);
        MineRewardInfo.instance.rewardAnimation.SetBool("isGetReward", true);
        MineRewardInfo.instance.rewardAnimation.Play("RewardInfo_Reward", -1, 0f);
    }

    // 선택한 펫 보상 받기
    public void GetSelectReward()
    {
        if (selectedPet != null)
        {
            if (selectedPetType == 0)
            {
                if (SaveScript.saveData.hasMinerRewards[selectedPetIndex] == 0) return;
                int num = SaveScript.saveData.hasMinerRewards[selectedPetIndex];
                minerRewardNum += num;

                for (int j = 0; j < num; j++)
                {
                    AddJem(selectedPetIndex);
                    if (GameFuction.GetRandFlag(SaveScript.mineUpgradePercents[2] * SaveScript.saveData.minerUpgrades[2]))
                    {
                        AddJem(selectedPetIndex);
                        minerRewardNum++;
                        minerPlusRewardNum++;
                    }
                }
                SaveScript.saveData.hasMinerRewards[selectedPetIndex] = 0;
                minerObjects[selectedPetIndex].SetRewardObject(false);
            }
            else if(selectedPetType == 1)
            {
                if (SaveScript.saveData.hasAdventurerRewards[selectedPetIndex] == 0) return;
                int num = SaveScript.saveData.hasAdventurerRewards[selectedPetIndex];
                float[] bufItem_percents = (float[])adventurerObjects[selectedPetIndex].bufItem_percents.Clone();
                float[] reinforceItem_percents = (float[])adventurerObjects[selectedPetIndex].reinforceItem_percents.Clone();
                adventurerRewardNum += num;

                for (int j = 0; j < num; j++)
                {
                    AddItem(bufItem_percents, reinforceItem_percents, selectedPetIndex);
                    if (GameFuction.GetRandFlag(SaveScript.mineUpgradePercents[2] * SaveScript.saveData.adventurerUpgrades[2]))
                    {
                        AddItem(bufItem_percents, reinforceItem_percents, selectedPetIndex);
                        adventurerRewardNum++;
                        adventurerPlusRewardNum++;
                    }
                }
                SaveScript.saveData.hasAdventurerRewards[selectedPetIndex] = 0;
                adventurerObjects[selectedPetIndex].SetRewardObject(false);
            }

            SetActiveSelectedPet(false);
            MineRewardInfo.instance.passClickPanel.SetActive(true);
            MineRewardInfo.instance.rewardAnimation.SetBool("isGetReward", true);
            MineRewardInfo.instance.rewardAnimation.Play("RewardInfo_Reward", -1, 0f);
        }
    }

    // 광물 보상 하나 얻기
    private void AddJem(int petIndex)
    {
        int jem_start = Jem.GetStartCode(SaveScript.saveData.pickLevel);
        float code_unit = (float)SaveScript.stageItemNums[SaveScript.saveData.pickLevel] / SaveScript.mineSlimeQualityNum;
        float percent = code_unit * (SaveScript.saveData.hasOnMiners[petIndex] + 1);

        for (int i = SaveScript.stageItemNums[SaveScript.saveData.pickLevel] - 1; i >= 0; i--)
        {
            if ((SaveScript.jems[jem_start + i].quality == 5 && SaveScript.saveData.hasOnMiners[petIndex] < 7) ||
                (SaveScript.jems[jem_start + i].quality == 6 && SaveScript.saveData.hasOnMiners[petIndex] < 10)) 
                continue;

            float multi = percent - i;
            if (multi > 1f) multi = 1f;
            else if (multi < 0f) multi = 0f;

            rewardJems[jem_start + i] += (long)(multi * Random.Range(0.9f, 1.1f) * MinerSlime.GetAmountAsLevel(SaveScript.jems[jem_start + i].quality, 
                SaveScript.saveData.hasOnMiners[petIndex], SaveScript.saveData.hasOnMinerLevels[petIndex]));
        }
    }

    // 아이템 보상 하나 얻기
    private void AddItem(float[] bufItem_percents, float[] reinforceItem_percents, int petIndex)
    {
        int code;
        int index;
        rewardReinforceOre += AdventurerSlime.GetReinforceOre(SaveScript.saveData.hasOnAdventurers[petIndex]);

        if (GameFuction.GetRandFlag(0.5f))
        {
            index = GameFuction.GetRandFlag(bufItem_percents);
            if (index < SaveScript.bufItemTypeNum)
            {
                // 물약
                code = Random.Range(0, SaveScript.bufItemCodeNum) * SaveScript.bufItemTypeNum + index;
                if (code / SaveScript.bufItemTypeNum == 10 && GameFuction.GetRandFlag(0.75f))
                    code = Random.Range(0, 10) * SaveScript.bufItemTypeNum + index;
                if (code / SaveScript.bufItemTypeNum == 11 && GameFuction.GetRandFlag(0.9f))
                    code = Random.Range(0, 10) * SaveScript.bufItemTypeNum + index;
                rewardBufItems[code] += AdventurerSlime.GetBufAmountAsLevel(index, SaveScript.saveData.hasOnAdventurers[petIndex]
                    , SaveScript.saveData.hasOnAdventurerLevels[petIndex]);
            }
            else
            {
                // 영약
                code = Random.Range(0, SaveScript.bufItemCodeNum);
                // 경험치 물약 조건부 생성
                if (code == 10 && GameFuction.GetRandFlag(0.75f))
                    code = Random.Range(0, 10);
                if (code == 11 && GameFuction.GetRandFlag(0.9f))
                    code = Random.Range(0, 10);
                rewardElixirItems[code] += AdventurerSlime.GetBufAmountAsLevel(index, SaveScript.saveData.hasOnAdventurers[petIndex]
                    , SaveScript.saveData.hasOnAdventurerLevels[petIndex]);
            }
        }
        else
        { 
            index = GameFuction.GetRandFlag(reinforceItem_percents);
            if (index < SaveScript.reinforceItemNum)
            {
                // 주문서
                rewardReinforceItems[index] += AdventurerSlime.GetReinforceAmountAsLevel(index, SaveScript.saveData.hasOnAdventurers[petIndex]
                    , SaveScript.saveData.hasOnAdventurerLevels[petIndex]);
            }
            else {
                // 제련석 + 유물
                rewardReinforceItems2[index - SaveScript.reinforceItemNum] += AdventurerSlime.GetReinforceAmountAsLevel(index, SaveScript.saveData.hasOnAdventurers[petIndex]
                    , SaveScript.saveData.hasOnAdventurerLevels[petIndex]);
            }
        }
    }

    public void RefreshPetObject(int _type, int _index)
    {
        int code = 0;
        switch (_type)
        {
            case 0:
                code = SaveScript.saveData.hasOnMiners[_index];
                if (minerObjects[_index].code != -1) { Destroy(minerObjects[_index].gameObject); minerObjects[_index].code = -1; }
                if (code != -1)
                {
                    MinerSlime minerSlime = Instantiate(minerPrefabs[code], new Vector3(Random.Range(-MAX_X, MAX_X), minerSlime_createY[code], 0f), Quaternion.identity, petObjectTr).GetComponent<MinerSlime>();
                    minerSlime.code = SaveScript.saveData.hasOnMiners[_index];
                    minerSlime.level = SaveScript.saveData.hasOnMinerLevels[_index];
                    minerSlime.exp = SaveScript.saveData.hasOnMinerExps[_index];
                    minerSlime.index = _index;
                    minerSlime.Init();
                    if (SaveScript.saveData.hasMinerRewards[_index] != 0) minerSlime.SetRewardObject(true);
                    else minerSlime.SetRewardObject(false);
                    minerObjects[_index] = minerSlime;
                }
                break;
            case 1:
                code = SaveScript.saveData.hasOnAdventurers[_index];
                if (adventurerObjects[_index].code != -1) { Destroy(adventurerObjects[_index].gameObject); adventurerObjects[_index].code = -1; }
                if (code != -1)
                {
                    AdventurerSlime AdventurerSlime = Instantiate(adventurerPrefabs[code], new Vector3(Random.Range(-MAX_X, MAX_X), adventurerSlime_createY[code], 0f), Quaternion.identity, petObjectTr).GetComponent<AdventurerSlime>();
                    AdventurerSlime.code = SaveScript.saveData.hasOnAdventurers[_index];
                    AdventurerSlime.level = SaveScript.saveData.hasOnAdventurerLevels[_index];
                    AdventurerSlime.exp = SaveScript.saveData.hasOnAdventurerExps[_index];
                    AdventurerSlime.index = _index;
                    AdventurerSlime.Init();
                    if (SaveScript.saveData.hasAdventurerRewards[_index] != 0) AdventurerSlime.SetRewardObject(true);
                    else AdventurerSlime.SetRewardObject(false);
                    adventurerObjects[_index] = AdventurerSlime;
                }
                break;
        }
    }

    IEnumerator DelayAllReward()
    {
        getAllRewardButton.enabled = false;
        yield return new WaitForSeconds(1f);
        getAllRewardButton.enabled = true;
    }
}
