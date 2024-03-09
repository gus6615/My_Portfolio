using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinerSlime : MineSlime
{
    public static float[] itemNumAsQuality = { 5f, 3f, 1f, 0.5f, 0.15f, 0.01f, 0.000002f }; // 아이템 별 갯수
    public static int plusTimeAsLevel = 60;
    public static string[] names = { "삽 슬라임", "곡괭이 슬라임", "드릴 슬라임", "TNT 슬라임", "수레 슬라임", "불도저 슬라임", "굴삭기 슬라임", "로더 슬라임", "덤프트럭 슬라임", "채굴기 슬라임", "채굴로봇 슬라임" };
    private static int[] times = { 60 * 60, 60 * 90, 60 * 120, 60 * 150, 60 * 180, 60 * 210, 60 * 240, 60 * 60 * 6, 60 * 60 * 9, 60 * 60 * 12, 60 * 60 * 24 };
    private static long[] amounts = { 50, 100, 200, 500, 1500, 5000, 20000, 100000, 1000000, 10000000, 100000000 }; // 곱해지는 량

    public int index; // 고정값 X, SaveScript.saveData.hasOnMiners 의 위치 인덱스
    public int quality;

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
        if (isDrag || code >= 9) return;
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
        return (long)(amounts[_code] * (1f + 0.05f * (_level - 1) + SaveScript.mineUpgradePercents[1] * SaveScript.saveData.minerUpgrades[1] + MineFacilityUI.instance.amountForce));
    }

    static public int GetTimePower(int _level)
    {
        return plusTimeAsLevel * (_level - 1) + (int)(SaveScript.mineUpgradePercents[0] * SaveScript.saveData.minerUpgrades[0]);
    }

    static public int GetTimeAsLevel(int _code, int _level)
    {
        int time = times[_code] - plusTimeAsLevel * (_level - 1) - (int)(SaveScript.mineUpgradePercents[0] * SaveScript.saveData.minerUpgrades[0]);
        if (time < 60 * 60) time = 60 * 60;
        return time;
    }

    static public long GetAmountAsLevel(int _type, int _code, int _level)
    {
        long num = Mathf.RoundToInt(itemNumAsQuality[_type] * (amounts[_code] * (1f + 0.05f * (_level - 1)
            + SaveScript.mineUpgradePercents[1] * SaveScript.saveData.minerUpgrades[1] + MineFacilityUI.instance.amountForce)));
        num = (long)(num * Random.Range(0.9f, 1.1f));
        if (num < 1f) num = 1;
        return num;
    }

    static public int GetCodeByCashItem(int type)
    {
        return GameFuction.GetRandFlag(cashPercents[type]);
    }

    static public int FindEmptyPetInven()
    {
        int index = -1;

        for (int i = 0; i < SaveScript.mineInvenMinNum + SaveScript.saveData.minerUpgrades[3]; i++)
            if (SaveScript.saveData.hasMiners[i] == -1) { index = i; break; };

        return index;
    }

    static public bool FindEmptyPetInven_Package()
    {
        int current = 0;

        for (int i = 0; i < SaveScript.mineInvenMinNum + SaveScript.saveData.minerUpgrades[3]; i++)
        {
            if (SaveScript.saveData.hasMiners[i] == -1) current++;
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
                if (SaveScript.saveData.hasMiners[j] > maxCode)
                {
                    maxCode = SaveScript.saveData.hasMiners[j];
                    maxIndex = j;
                }
            }
            
            if(maxIndex != -1)
            {
                int temp = SaveScript.saveData.hasMiners[i];
                SaveScript.saveData.hasMiners[i] = SaveScript.saveData.hasMiners[maxIndex];
                SaveScript.saveData.hasMiners[maxIndex] = temp;

                temp = SaveScript.saveData.hasMinerLevels[i];
                SaveScript.saveData.hasMinerLevels[i] = SaveScript.saveData.hasMinerLevels[maxIndex];
                SaveScript.saveData.hasMinerLevels[maxIndex] = temp;

                long temp2 = SaveScript.saveData.hasMinerExps[i];
                SaveScript.saveData.hasMinerExps[i] = SaveScript.saveData.hasMinerExps[maxIndex];
                SaveScript.saveData.hasMinerExps[maxIndex] = temp2;
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
        this.transform.position = new Vector3(this.transform.position.x, MineMap.instance.minerSlime_createY[code], 0f);
    }
}
