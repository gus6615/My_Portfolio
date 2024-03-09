using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdventurerSlime : MineSlime
{
    public static float[] reinforceNumAsQuality = { 0.08f, 0.06f, 0.05f, 0.08f, 0.08f, 0.05f, 0.0035f, 0.0025f, 0.0025f, 0.002f, 0.001f, 0.000001f, 0.000001f, 0.000001f }; // 강화 아이템 갯수
    public static float[] bufNumAsQuality = { 0.1f, 0.07f, 0.05f, 0.005f }; // 물약 아이템 갯수
    public static int plusTimeAsLevel = 60 * 2;
    public static string[] names = { "새총 슬라임", "창 슬라임", "단검 슬라임", "소드 슬라임", "대검 슬라임", "도끼 슬라임", "블레이드 슬라임", "신궁 슬라임", "어쌔신 슬라임", "마법사 슬라임", "사신 슬라임" };
    private static int[] times = { 60 * 60 * 6, 60 * 60 * 7, 60 * 60 * 8, 60 * 60 * 9, 60 * 60 * 10, 60 * 60 * 11, 60 * 60 * 12, 60 * 60 * 15, 60 * 60 * 18, 60 * 60 * 24, 60 * 60 * 48 };
    private static int[] amounts = { 8, 9, 11, 14, 18, 25, 40, 70, 150, 300, 500 }; // 곱해지는 량
    private static int[] reinforces = { 100, 150, 200, 250, 300, 500, 1500, 5000, 30000, 200000, 10000000 };
    private static float[][] reinforceItem_percentss = new float[][] // 제련석까지만 획득
    {
        new float[]{0.4f, 0.1f, 0f, 0.25f, 0.25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0.2f, 0.3f, 0.05f, 0.2f, 0.2f, 0.05f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0.5f, 0.1f, 0.15f, 0.15f, 0.1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0.4f, 0.2f, 0.1f, 0.1f, 0.2f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0.3f, 0.35f, 0f, 0f, 0.35f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0.19f, 0.4f, 0f, 0f, 0.4f, 0.02f, 0f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0f, 0.45f, 0f, 0f, 0.45f, 0.08f, 0.02f, 0f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0f, 0.25f, 0f, 0f, 0.25f, 0.3f, 0.1f, 0.1f, 0f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0f, 0f, 0f, 0f, 0f, 0.3f, 0.3f, 0.3f, 0.1f, 0f, 0f, 0f, 0f },
        new float[]{0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.2f, 0.3f, 0.4f, 0.1f, 0f, 0f, 0f },
         new float[]{0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.94f, 0.01f, 0.0001f, 0.000001f },
    };
    private static float[][] bufItem_percentss = new float[][]
    {
        new float[] { 0.9f, 0.1f, 0f, 0f },
        new float[] { 0.75f, 0.22f, 0.03f, 0f },
        new float[] { 0.5f, 0.4f, 0.1f, 0f },
        new float[] { 0.25f, 0.55f, 0.2f, 0f },
        new float[] { 0f, 0.7f, 0.3f, 0f },
        new float[] { 0f, 0.3f, 0.7f, 0f },
        new float[] { 0f, 0f, 1f, 0f },
        new float[] { 0f, 0f, 0.7f, 0.3f },
        new float[] { 0f, 0f, 0.3f, 0.7f },
        new float[] { 0f, 0f, 0f, 1f },
        new float[] { 0f, 0f, 0f, 1f },
    };

    public int index; // 고정값 X, SaveScript.saveData.hasOnAdventurers 의 위치 인덱스
    public int quality;
    public float[] reinforceItem_percents, bufItem_percents;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        if (!isInit)
        {
            isInit = true;
            miner_faceSprites = Resources.LoadAll<Sprite>("Images/Mine/Pet/MinerPets/Faces");
            adventurer_faceSprites = Resources.LoadAll<Sprite>("Images/Mine/Pet/AdventurerPets/Faces");
            miner_defaultSprites = Resources.LoadAll<Sprite>("Images/Mine/Pet/MinerPets/Defaults");
            adventurer_defaultSprites = Resources.LoadAll<Sprite>("Images/Mine/Pet/AdventurerPets/Defaults");
            rewardSprites = Resources.LoadAll<Sprite>("Images/Mine/Pet/Rewards");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrag || code >= 10) return;
        if (!isReward && !isSelected && isChangeAni) 
            StartCoroutine("SetAni");
        if (isMove)
        {
            this.transform.position += moveVec * Time.deltaTime * 0.5f;
            if (Mathf.Abs(this.transform.position.x) > MAX_X)
            {
                this.transform.position = new Vector3(Mathf.Sign(this.transform.position.x) * MAX_X, this.transform.position.y, 0f);
                animator.SetBool("isMove", false);
                isMove = false;
            }
        }
    }

    public void Init()
    {
        isChangeAni = true;
        quality = code % SaveScript.mineSlimeQualityNum;
        name = names[code];
        time = times[code];
        amount = amounts[code];
        reinforceItem_percents = reinforceItem_percentss[code];
        bufItem_percents = bufItem_percentss[code];

        if (Random.Range(1, 3) == 1)
        {
            this.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            moveVec = Vector2.left;
        }
        else
        {
            this.transform.localScale = new Vector3(-0.75f, 0.75f, 1f);
            moveVec = Vector2.right;
        }
    }

    static public long GetAmountPower(int _code, int _level)
    {
        return (long)(amounts[_code] * (1f + 0.05f * (_level - 1) + SaveScript.mineUpgradePercents[1] * SaveScript.saveData.adventurerUpgrades[1] + MineFacilityUI.instance.amountForce));
    }

    static public int GetTimePower(int _level)
    {
        return plusTimeAsLevel * (_level - 1) + (int)(SaveScript.mineUpgradePercents[0] * 2f * SaveScript.saveData.adventurerUpgrades[0] + SaveScript.saveData.facility_level * 60f);
    }

    static public int GetTimeAsLevel(int _code, int _level)
    {
        int time = times[_code] - plusTimeAsLevel * (_level - 1) - (int)(SaveScript.mineUpgradePercents[0] * 2f * SaveScript.saveData.adventurerUpgrades[0]
            + SaveScript.saveData.facility_level * 60f);
        if (time < 60 * 120) time = 60 * 120;

        return time;
    }

    static public int GetBufAmountAsLevel(int _type, int _code, int _level)
    {
        int num = (int)(bufNumAsQuality[_type] * (amounts[_code] * (1f + 0.05f * (_level - 1)
            + SaveScript.mineUpgradePercents[1] * SaveScript.saveData.adventurerUpgrades[1])));
        if (num < 1f) num = 1;
        return num;
    }

    static public int GetReinforceAmountAsLevel(int _type, int _code, int _level)
    {
        int num = (int)(reinforceNumAsQuality[_type] * (amounts[_code] * (1f + 0.05f * (_level - 1)
            + SaveScript.mineUpgradePercents[1] * SaveScript.saveData.adventurerUpgrades[1])));
        if (num < 1f) num = 1;
        return num;
    }

    static public long GetReinforceOre(int _code)
    {
        return (long)(reinforces[_code] * Random.Range(0.9f, 1.1f));
    }

    static public int GetCodeByCashItem(int type)
    {
        return GameFuction.GetRandFlag(cashPercents[type]);
    }

    static public int FindEmptyPetInven()
    {
        int index = -1;

        for (int i = 0; i < SaveScript.mineInvenMinNum + SaveScript.saveData.adventurerUpgrades[3]; i++)
            if (SaveScript.saveData.hasAdventurers[i] == -1) { index = i; break; };

        return index;
    }

    static public bool FindEmptyPetInven_Package()
    {
        int current = 0;

        for (int i = 0; i < SaveScript.mineInvenMinNum + SaveScript.saveData.adventurerUpgrades[3]; i++)
        {
            if (SaveScript.saveData.hasAdventurers[i] == -1) current++;
            if (current == 10) return true;
        }
        return false;
    }

    static public void SortPetInven()
    {
        int maxCode = -1;
        int maxIndex = -1;

        for (int i = 0; i < SaveScript.mineInvenMaxNum; i++)
        {
            for (int j = i; j < SaveScript.mineInvenMaxNum; j++)
            {
                if (SaveScript.saveData.hasAdventurers[j] > maxCode)
                {
                    maxCode = SaveScript.saveData.hasAdventurers[j];
                    maxIndex = j;
                }
            }

            if(maxIndex != -1)
            {
                int temp = SaveScript.saveData.hasAdventurers[i];
                SaveScript.saveData.hasAdventurers[i] = SaveScript.saveData.hasAdventurers[maxIndex];
                SaveScript.saveData.hasAdventurers[maxIndex] = temp;

                temp = SaveScript.saveData.hasAdventurerLevels[i];
                SaveScript.saveData.hasAdventurerLevels[i] = SaveScript.saveData.hasAdventurerLevels[maxIndex];
                SaveScript.saveData.hasAdventurerLevels[maxIndex] = temp;

                long temp2 = SaveScript.saveData.hasAdventurerExps[i];
                SaveScript.saveData.hasAdventurerExps[i] = SaveScript.saveData.hasAdventurerExps[maxIndex];
                SaveScript.saveData.hasAdventurerExps[maxIndex] = temp2;
                maxCode = maxIndex = -1;
            }
        }
    }

    private void OnMouseDown()
    {
        dragTime = 0f;
        isDrag = false;
    }

    private void OnMouseDrag()
    {
        dragTime += Time.deltaTime;
        if (dragTime > 0.5f)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDrag = true;
            this.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
            MineMap.instance.SetActiveSelectedPet(false);
        }
    }

    private void OnMouseUp()
    {
        dragTime = 0f;
        isDrag = false;
        this.transform.position = new Vector3(this.transform.position.x, MineMap.instance.adventurerSlime_createY[code], 0f);
    }
}
