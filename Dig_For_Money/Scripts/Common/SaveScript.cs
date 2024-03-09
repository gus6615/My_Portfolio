using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BackEnd;
using LitJson;
using UnityEngine.UI;

public class Stat
{
    // 플레이어 장비 관련
    public float pick01, pick02;
    public float hat01, hat02;
    public float ring01, ring02;
    public float pendant01, pendant02;
    public float sword01, sword02;
    // 특수 능력 관련
    public float exp;
    public float exp2;
    public float petDrop;
    public float library;
    public float itemDrop;
    public float boxManaPercent;
    public float boxMana;
    public float manaOre;
    public float collection;
    public float bosMana;
    public float ultimateOre;
    public float eventMap;
    public float petFusion;
    public float mysticOre;
    public float growthOre;

    public int combo_level;

    public void SetStat()
    {
        float value = 0f;
        // 콤보 시스템 수정
        combo_level = GameFuction.GetComboLevel();

        // 곡괭이
        value += GameFuction.GetReinforcePercent(0, 0);
        value += GameFuction.GetCollectionUpgradeForce(0);
        value += GameFuction.GetBufItemPercent(0) + GameFuction.GetElixirPercent(0);
        if (SaveScript.saveData.hasIcons[4]) value += SaveScript.icons[4].force;

        pick01 = value;

        value = 0f;
        value += GameFuction.GetReinforcePercent(0, 1);
        value += GameFuction.GetCollectionUpgradeForce(1);
        value += GameFuction.GetBufItemPercent(1) + GameFuction.GetElixirPercent(1);
        if (SaveScript.saveData.hasIcons[5]) value += SaveScript.icons[5].force;

        pick02 = value;

        // 모자
        value = 0f;
        value += GameFuction.GetReinforcePercent(1, 0);
        value += GameFuction.GetCollectionUpgradeForce(2);
        value += GameFuction.GetBufItemPercent(2) + GameFuction.GetElixirPercent(2);
        if (SaveScript.saveData.hasIcons[6]) value += SaveScript.icons[6].force;
        
        hat01 = value; 

        value = 0.1f;
        value += GameFuction.GetBufItemPercent(3) + GameFuction.GetElixirPercent(3);
        value += GameFuction.GetReinforcePercent(1, 1);
        value += GameFuction.GetCollectionUpgradeForce(3);

        hat02 = value;

        // 반지
        value = 0f;
        value += GameFuction.GetReinforcePercent(2, 0);
        value += GameFuction.GetManaBufForceForData(1);
        value += GameFuction.GetCollectionUpgradeForce(4);
        for (int i = 0; i < SaveScript.stageNum; i++)
            value += GameFuction.GetCollectionFloorUpgradeForce(i, 0);
        value += GameFuction.GetBufItemPercent(4) + GameFuction.GetElixirPercent(4);
        if (SaveScript.saveData.hasIcons[9]) value += SaveScript.icons[9].force;
        if (SaveScript.saveData.hasIcons[11]) value += SaveScript.icons[11].force;

        ring01 = value;

        value = 0.1f;
        value += GameFuction.GetBufItemPercent(5) + GameFuction.GetElixirPercent(5);
        value += GameFuction.GetReinforcePercent(2, 1);
        value += GameFuction.GetCollectionUpgradeForce(5);

        ring02 = value;

        // 목걸이
        value = 0f;
        value += (int)GameFuction.GetReinforcePercent(3, 0);
        value += (int)(GameFuction.GetBufItemPercent(6) + GameFuction.GetElixirPercent(6));
        value += (int)GameFuction.GetCollectionUpgradeForce(6);
        for (int i = 0; i < SaveScript.stageNum; i++)
            value += (int)GameFuction.GetCollectionFloorUpgradeForce(i, 1);
        value += Mathf.RoundToInt(GameFuction.GetManaBufForceForData(2));
        if (SaveScript.saveData.hasIcons[8]) value += (int)SaveScript.icons[8].force;
        if (SaveScript.saveData.hasIcons[10]) value += (int)SaveScript.icons[10].force;

        pendant01 = value;

        value = 0.5f;
        value += GameFuction.GetReinforcePercent(3, 1);
        value += GameFuction.GetBufItemPercent(7) + GameFuction.GetElixirPercent(7);
        value += GameFuction.GetCollectionUpgradeForce(7);

        pendant02 = value;

        // 검
        value = 0f;
        value += GameFuction.GetCollectionUpgradeForce(8);
        value += GameFuction.GetReinforcePercent(4, 0);
        value += GameFuction.GetBufItemPercent(12) + GameFuction.GetElixirPercent(12);
        if (SaveScript.saveData.hasIcons[7]) value += SaveScript.icons[7].force;

        sword01 = value;

        value = 0.1f;
        value += GameFuction.GetBufItemPercent(13) + GameFuction.GetElixirPercent(13);
        value += GameFuction.GetReinforcePercent(4, 1);
        value += GameFuction.GetCollectionUpgradeForce(9);

        sword02 = value;

        // 특수 능력 관련

        exp = GameFuction.GetManaUpgradeForce(3);
        exp += GameFuction.GetBufItemPercent(10) + GameFuction.GetElixirPercent(10);
        exp += GameFuction.GetManaBufForceForData(3);
        exp += GameFuction.GetCollectionUpgradeForce(10);
        exp += GameFuction.GetCollectionFloorUpgradeForce(0, 2);
        exp += GameFuction.GetComboForce(0);

        manaOre = GameFuction.GetCollectionUpgradeForce(11);
        manaOre += GameFuction.GetCollectionFloorUpgradeForce(2, 2);
        manaOre += GameFuction.GetManaUpgradeForce(5);
        
        exp2 = GameFuction.GetManaUpgradeForce(4);
        exp2 += GameFuction.GetCollectionUpgradeForce(12);
        exp2 += GameFuction.GetCollectionFloorUpgradeForce(1, 2);

        petDrop = GameFuction.GetManaUpgradeForce(8);
        petDrop += GameFuction.GetCollectionUpgradeForce(13);
        petDrop += GameFuction.GetCollectionFloorUpgradeForce(3, 2);

        library = GameFuction.GetManaUpgradeForce(0);
        library += GameFuction.GetCollectionUpgradeForce(15);
        library += GameFuction.GetCollectionFloorUpgradeForce(3, 2);

        itemDrop = GameFuction.GetManaUpgradeForce(7);
        itemDrop += GameFuction.GetCollectionUpgradeForce(15);
        itemDrop += GameFuction.GetCollectionFloorUpgradeForce(3, 2);

        boxMana = GameFuction.GetManaUpgradeForce(6);
        boxMana += GameFuction.GetCollectionFloorUpgradeForce(4, 2);
        boxMana += GameFuction.GetComboForce(2);

        collection = 1f + GameFuction.GetManaUpgradeForce(9) + GameFuction.GetCollectionUpgradeForce(17);
        collection += GameFuction.GetCollectionFloorUpgradeForce(5, 2);

        boxManaPercent = GameFuction.GetCollectionUpgradeForce(16);

        bosMana = 1f + GameFuction.GetManaUpgradeForce(10);
        bosMana += GameFuction.GetCollectionUpgradeForce(18);
        bosMana += GameFuction.GetCollectionFloorUpgradeForce(6, 2);
        bosMana += GameFuction.GetComboForce(2);

        ultimateOre = 1f + GameFuction.GetManaUpgradeForce(11);
        ultimateOre += GameFuction.GetCollectionUpgradeForce(19);
        ultimateOre += GameFuction.GetCollectionFloorUpgradeForce(7, 2);
        ultimateOre += GameFuction.GetComboForce(1);

        eventMap = GameFuction.GetManaUpgradeForce(12);
        eventMap += GameFuction.GetCollectionFloorUpgradeForce(8, 2);
        eventMap += GameFuction.GetCollectionUpgradeForce(20);

        petFusion = GameFuction.GetManaUpgradeForce(13);
        petFusion += GameFuction.GetCollectionFloorUpgradeForce(9, 2);
        petFusion += GameFuction.GetCollectionUpgradeForce(21);

        mysticOre = 1f + GameFuction.GetManaUpgradeForce(14);
        mysticOre += GameFuction.GetCollectionUpgradeForce(22);
        mysticOre += GameFuction.GetCollectionFloorUpgradeForce(10, 2);
        mysticOre += GameFuction.GetCollectionFloorUpgradeForce(11, 2);
        mysticOre += GameFuction.GetCollectionFloorUpgradeForce(12, 2);
        mysticOre += GameFuction.GetComboForce(1);

        growthOre = 1f + GameFuction.GetCollectionUpgradeForce(23);
        growthOre += GameFuction.GetCollectionFloorUpgradeForce(13, 2);

        // 룰렛 및 버프 아이템 등 데이터 수정
        SaveScript.SetDataAsStat();
    }
}

public class SaveData
{
    public long gold, gold2, gold3, gold4, cash; // 돈, 캐시
    public long exp; // 경험치
    public int ticketNum, ticketTime;
    public int rulletTime;
    public bool isRemoveAD; // 현재 광고 제거를 가지고 있는가?
    public string closeDate;
    public string user_Indate, privateData_Indate, publicData_Indate;
    public int equipPick;
    public int equipHat, equipRing, equipPendant, equipSword;
    public long[] hasItemNums; // 보석의 갯수
    public bool isReviewOn;
    public bool isTutorial; // 튜토리얼 완료 여부 
    public bool isBGMOn, isSEOn, isBlockSoundOn;
    public bool isEndGame;

    public bool[] hasPicks, hasHats, hasRings, hasPenants, hasSwords; // 가진 장비
    public bool[] hasIcons;
    public int[] iconsTime;
    public int pick1Upgrades, pick2Upgrades; // 경험치 강화 횟수
    public int hat1Upgrades, hat2Upgrades, ring1Upgrades, ring2Upgrades, Pendant1Upgrades, Pendant2Upgrades, sword1Upgrades, sword2Upgrades; // 경험치 강화 횟수

    public long hasReinforceOre; // 강화석 갯수
    public int[] pickReinforces, hatReinforces, ringReinforces, pendantReinforces, swordReinforces; // 강화석 강화 횟수
    public bool[] isPickReinforceDestroy, isHatReinforceDestroy, isRingReinforceDestroy, isPendantReinforceDestroy, isSwordReinforceDestroy;
    public int[] quastLevels, quastLists, quastGoals;
    public bool[] quastSuccesses;
    public int pickLevel;

    public long[] hasBufItems; // 버프 아이템 갯수
    public long[] hasReinforceItems; // 강화 아이템 갯수
    public int[] bufItemTimes;
    public bool[] isBufItemOns;

    public int[] hasOnMiners, hasOnAdventurers; // 현재 일하는 중인 슬라임들 code
    public int[] hasOnMinerLevels, hasOnAdventurerLevels; // 현재 일하는 중인 슬라임들 레벨
    public long[] hasOnMinerExps, hasOnAdventurerExps; // 현재 일하는 중인 슬라임들 EXP
    public int[] hasMiners, hasAdventurers; // 인벤토리에 존재하는 슬라임들 code
    public int[] hasMinerLevels, hasAdventurerLevels; // 인벤토리에 존재하는 슬라임들 레벨
    public long[] hasMinerExps, hasAdventurerExps; // 인벤토리에 존재하는 슬라임들 EXP
    public int[] hasMinerRewards, hasAdventurerRewards; // 현재 가지고 있는 보상 수
    public int[] hasMinerTimes, hasAdventurerTimes; // 현재 보상까지 남은 시간
    public int[] minerUpgrades, adventurerUpgrades; // 업그레이드

    public long[] achievementAmounts;
    public int[] achievementLevels;
    public int tier_level; // 티어의 종류
    public int tier_achievement; // 현재 티어 기준 수행한 업적 수

    public long manaOre; // 마나석
    public int facility_level; // 펫 시설 레벨
    public int facility_rewardTime; // 펫 시설 보상 남은 시간
    public int[] manaBufTimes;
    public bool[] isManaBuffOns;
    public long[] achievementAmount2; // 2번째 플레이어 업적 데이터

    public int[] manaUpgrades; // 마나 업그레이드

    public bool isRankerChat; // 전설 유저 체크

    public int attendance_month; // 현재 달력 (1 ~ 12)
    public int attendance_day; // 최근 출석한 일 (1 ~ 31, 30, 28 / 만약 0이라면 출석을 한 적이 없음)
    public int attendance_count; // 출석 횟수 (0 ~ 28)

    public int[] collection_levels; // 컬렉션 레벨
    public int[] collection_cards; // 컬렉션 카드 개수

    public bool isAutoUse_1, isAutoUse_2;

    public long[] hasElixirs; // 영약 아이템 갯수
    public int[] elixirTimes; // 영약 시간
    public bool[] isElixirOns; // 영약 ON / OFF

    public long[] hasReinforceItems2; // 특수 강화 아이템 갯수
    public bool isGodChat; // 신화 유저 체크

    public int fpsType; // 0 = 제한 X, 1 = 30fps, 2 = 60fps, 3 = 90fps. 4 = 120fps, 5 = 144fps

    public int mainQuest_list; // 현재 메인 퀘스트 상태 (마지막 + 1에 도달하면 모든 퀘스트를 완료)
    public long mainQuest_goal; // 현재 달성 상태

    public bool[] hasCashEquipment; // 현재 캐시 특수 아이템을 가지고 있는가?

    public bool isGetBonus; // 보너스 보상을 받았는가?
    public bool[] isCashEquipmentOn; // 특수 아이템 ON?

    public long[] achievementAmount3; // 3번째 플레이어 업적 데이터

    public long growthOreNum; // 성장하는 돌 가진 횟수

    public int vendingMachineTime;
    public int[] mailboxes; // (-)Value의 경우 cash, (+)Value의 경우 manaOre, 0은 NULL
    public int mailboxTime;

    public int combo_gauge; // 콤보 게이지

    public SaveData()
    {
        ticketNum = 5;
        ticketTime = 0;
        rulletTime = 0;
        privateData_Indate = "";
        publicData_Indate = "";
        gold = gold2 = gold3 = gold4 = cash = 0;
        exp = 0;
        equipPick = 0;
        equipSword = 0;
        equipHat = equipRing = equipPendant = -1;
        hasItemNums = new long[SaveScript.totalItemNum];
        isReviewOn = false;
        isTutorial = true;
        isBGMOn = true;
        isSEOn = true;
        isBlockSoundOn = true;
        isEndGame = false;
        isRemoveAD = false;

        hasPicks = new bool[SaveScript.pickNum];
        hasPicks[0] = true;
        pick1Upgrades = 0;
        pick2Upgrades = 0;
        hasHats = new bool[SaveScript.hatNum];
        hat1Upgrades = 0;
        hat2Upgrades = 0;
        hasRings = new bool[SaveScript.RingNum];
        ring1Upgrades = 0;
        ring2Upgrades = 0;
        hasPenants = new bool[SaveScript.PendantNum];
        Pendant1Upgrades = 0;
        Pendant2Upgrades = 0;
        hasSwords = new bool[SaveScript.swordNum];
        hasSwords[0] = true;
        sword1Upgrades = 0;
        sword2Upgrades = 0;
        hasIcons = new bool[SaveScript.iconNum];
        iconsTime = new int[SaveScript.iconNum];

        pickReinforces = new int[SaveScript.pickNum];
        hatReinforces = new int[SaveScript.hatNum];
        ringReinforces = new int[SaveScript.RingNum];
        pendantReinforces = new int[SaveScript.PendantNum];
        swordReinforces = new int[SaveScript.swordNum];
        hasReinforceOre = 0;
        isPickReinforceDestroy = new bool[SaveScript.pickNum];
        isHatReinforceDestroy = new bool[SaveScript.hatNum];
        isRingReinforceDestroy = new bool[SaveScript.RingNum];
        isPendantReinforceDestroy = new bool[SaveScript.PendantNum];
        isSwordReinforceDestroy = new bool[SaveScript.swordNum];

        quastLevels = new int[SaveScript.hasQuestNum];
        quastLists = new int[SaveScript.hasQuestNum];
        for (int i = 0; i < quastLists.Length; i++)
            quastLists[i] = i;
        quastGoals = new int[SaveScript.hasQuestNum];
        quastSuccesses = new bool[SaveScript.hasQuestNum];
        pickLevel = 0;

        hasBufItems = new long[SaveScript.bufItemNum];
        hasReinforceItems = new long[SaveScript.reinforceItemNum];
        bufItemTimes = new int[SaveScript.bufItemNum];
        isBufItemOns = new bool[SaveScript.bufItemNum];

        hasOnMiners = new int[SaveScript.mineSlotMaxNum];
        hasOnMinerLevels = new int[SaveScript.mineSlotMaxNum];
        hasOnMinerExps = new long[SaveScript.mineSlotMaxNum];
        hasMiners = new int[SaveScript.mineInvenMaxNum];
        hasMinerLevels = new int[SaveScript.mineInvenMaxNum];
        hasMinerExps = new long[SaveScript.mineInvenMaxNum];
        hasMinerRewards = new int[SaveScript.mineSlotMaxNum];
        hasMinerTimes = new int[SaveScript.mineSlotMaxNum];
        hasOnAdventurers = new int[SaveScript.mineSlotMaxNum];
        hasOnAdventurerLevels = new int[SaveScript.mineSlotMaxNum];
        hasOnAdventurerExps = new long[SaveScript.mineSlotMaxNum];
        hasAdventurers = new int[SaveScript.mineInvenMaxNum];
        hasAdventurerLevels = new int[SaveScript.mineInvenMaxNum];
        hasAdventurerExps = new long[SaveScript.mineInvenMaxNum];
        hasAdventurerRewards = new int[SaveScript.mineSlotMaxNum];
        hasAdventurerTimes = new int[SaveScript.mineSlotMaxNum];
        minerUpgrades = new int[SaveScript.mineUpgradeNum];
        adventurerUpgrades = new int[SaveScript.mineUpgradeNum];
        for (int i = 0; i < hasOnMiners.Length; i++)
            hasOnMiners[i] = hasMiners[i] = -1;
        for (int i = 0; i < hasOnAdventurers.Length; i++)
            hasOnAdventurers[i] = hasAdventurers[i] = -1;

        achievementAmounts = new long[SaveScript.achievementNum];
        achievementLevels = new int[SaveScript.achievementNum];
        for (int i = 0; i < SaveScript.achievementNum; i++)
            achievementLevels[i] = 0;
        tier_level = 0;
        tier_achievement = 0;

        manaOre = 0;
        facility_level = 0;
        facility_rewardTime = SaveScript.facility_rewardTime;
        manaBufTimes = new int[SaveScript.manaBufNum];
        isManaBuffOns = new bool[SaveScript.manaBufNum];
        achievementAmount2 = new long[2];

        manaUpgrades = new int[SaveScript.manaUpgradeNum];
        isRankerChat = false;
        attendance_month = -1;
        attendance_day = 0;
        attendance_count = 0;

        collection_levels = new int[SaveScript.totalItemNum];
        collection_cards = new int[SaveScript.totalItemNum];

        hasElixirs = new long[SaveScript.bufItemCodeNum];
        elixirTimes = new int[SaveScript.bufItemCodeNum];
        isElixirOns = new bool[SaveScript.bufItemCodeNum];

        // gold3 추가 (1.5.0 업데이트)
        hasReinforceItems2 = new long[SaveScript.reinforceItem2Num];
        isGodChat = false;

        fpsType = 1;
        mainQuest_list = 0;
        mainQuest_goal = 0;

        hasCashEquipment = new bool[SaveScript.cashEquipmentNum];

        isGetBonus = true;
        isCashEquipmentOn = new bool[SaveScript.cashEquipmentNum];

        achievementAmount3 = new long[2];

        growthOreNum = 0;

        vendingMachineTime = 0;
        mailboxes = new int[SaveScript.mailboxNum];
        mailboxTime = SaveScript.mailboxTime;

        combo_gauge = 0;

        // gold4 추가 (1.5.0 업데이트)
    }
}

public class SaveScript : MonoBehaviour
{
    private const int ASYNCSAVE_TIME = 10;
    private const int AUTOSAVE_TIME = 60 * 3;
    private const int PROCESS_COUNT = 5;

    static public SaveScript instance;
    static public Stat stat;
    static public SaveRank saveRank;
    static public SaveData saveData;
    static private bool isAutoSave, isAsyncSave;
    static public string rankUuid_gold, rankUuid_gold2, rankUuid_gold3, rankUuid_gold4;
    static public string pause_time;
    static public DateTime dateTime;
    static private int processCount;

    static public AudioClip[] SEs;
    static public AudioClip[] BGMs;

    static public int blockTileNum, brokenTileNum;

    static public int ticketNum, ticketTime;
    static public int rulletTime;
    static public int iconNum;
    static public int iconTime;
    static public int pickNum;
    static public int pickStateNum;
    static public int hatNum, RingNum, PendantNum, swordNum;
    static public int accessoryNum;
    static public int totalItemNum;
    static public int qualityNum;
    static public int minCreateJemNuM, maxCreateJemNum;
    static public float createMinePercent, createDungeon_0_Percent, createDungeon_1_Percent, createKingSlimeRoomPercent;

    // StageNum에 따라 추가됨
    // Jem, Pick & Accessory, Quest(각 퀘스트에 대해 설정 필요),GM, CashAnimator-oreMin, MainQuestUI, Icon, Item,
    // Monster + Boss, MonsterAttack, NPC, 모든 Box 종류, 모든 BreakObject, AncientTresure, MainVendingMachine
    static public int stageNum = 15;
    public static int contentNum = 19;
    public static int mainQuestNum = 141; // 층마다 10개씩 증가
    static public int[] stageItemNums = { 4, 5, 7, 7, 6, 7, 6, 6, 6, 6, 7, 7, 7, 7, 7 };
    static public float[] resistences = { 0.1f, 0.2f, 0.3f, 0.5f, 0.7f, 1f, 1.5f, 2f, 3f, 5f, 10f, 20f, 50f, 100f, 300f };
    static public float[] libraryPercentsAsType = { 0f, 2f, 4f, 10f, 0f, 5f, 8f, 15f, 30f, 100f, 1000f, 1500f, 2000f }; // 중간에 0은 7층을 의미
    static public float[] npcPercentsAsType = { 0f, 0.5f, 1f, 2f, 3f, 5f, 7f, 10f, 14f, 18f, 24f, 30f, 40f };
    static public int[] digDamageAsFloor = { 1, 1, 1, 1, 2, 3, 5, 10, 30, 100, 500, 1000, 3000, 5000, 10000 };
    static public long[] special_block_hps = { 15000, 40000, 100000, 300000, 800000, 5000000, 10000000, 20000000, 40000000 };
    static public int[] mailboxManaOres = { 50, 100, 150, 200, 300, 500, 1000, 2000, 5000, 10000, 30000, 50000, 100000, 300000, 1000000 };
    static public long[] growthOre_prices = 
    { 
        100000000, 1000000000, 10000000000, 1000000000000, 100000000000000, 1,
        1000, 2000000, 5000000000, 25000000000000,
        10, 100000, 1000000000, 10000000000000,
        1 // Henny
    };

    static public List<Jem> jems;
    static public List<Pick> picks;
    static public List<Hat> hats;
    static public List<Ring> rings;
    static public List<Pendant> pendants;
    static public List<Sword> swords;
    static public List<Icon> icons;
    static public bool isChangedIcons; // 어느 버프가 끝났을 경우 true가 된다.

    static public Color[] stageColors; // 층 색상
    static public Color[] toolColors; // 나무, 돌, 철, 금, 다이아, 칠흑 색상
    static public Color[] qualityColors, qualityColors_weak; // 노멀 ~ 레전드리 색상
    static public Color[] monsterColors; // 몬스터 색상
    static public string[] qualityColors_weekTmp =
    {
        "<color=#C8C8C8>", "<color=#AFE1FF>", "<color=#FFAFFF>", "<color=#FFFFAF>", "<color=#FFAFAF>", "<color=#AFFFAF>", "<color=#AFFFFF>",
    };
    static public string[] qualityColors_strongTmp =
{
        "<color=#323232>", "<color=#006496>", "<color=#960096>", "<color=#E1E100>", "<color=#AF3232>", "<color=#009600>", "<color=#194B96>",
    };
    static public string[] qualityNames_kr =
    {
        "노멀", "레어", "에픽", "유니크", "레전드리", "얼티밋", "미스틱"
    }; // 노멀 ~ 레전드리 이름 (한글)
    static public string[] qualityNames_en =
    {
        "NORMAL", "RARE", "EPIC", "UNIQUE", "LEGENDARY", "ULTIMATE", "MYSTIC"
    }; // 노멀 ~ 레전드리 이름 (영어)

    private bool isSetTime;
    private int clickCount;

    static public Canvas quitCanvas;
    static public Canvas endCanvas;
    static public Canvas autoCanvas;
    static public Canvas errorCanvas;
    static private Animator auto_animator;

    static public int reinforceNumAsQulity = 4;
    static public int[] reinforceOreNeededNums = { 10, 20, 30, 50, 100, 200 };
    static public float[][] expReinforcePercents = new float[][]
    {
        new float[] { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f, 2.6f, 2.7f, 2.8f, 2.9f, 3.0f, 3.25f, 3.5f, 3.75f, 4.0f}, // 곡괭이
        new float[] { 0.03f, 0.03f, 0.03f, 0.03f, 0.035f, 0.035f, 0.04f, 0.04f, 0.045f, 0.045f, 0.05f, 0.05f, 0.055f, 0.055f, 0.06f, 0.06f, 0.065f, 0.065f, 0.07f, 0.07f, 0.075f, 0.08f, 0.085f, 0.09f, 0.1f},
        new float[] { 1.0f, 1.05f, 1.1f, 1.15f, 1.2f, 1.25f, 1.3f, 1.35f, 1.4f, 1.45f, 1.5f, 1.55f, 1.6f, 1.65f, 1.7f, 1.75f, 1.8f, 1.85f, 1.9f, 1.95f, 2.0f, 2.1f, 2.2f, 2.3f, 2.5f}, // 모자
        new float[] { 0.03f, 0.03f, 0.03f, 0.03f, 0.035f, 0.035f, 0.04f, 0.04f, 0.045f, 0.045f, 0.05f, 0.05f, 0.055f, 0.055f, 0.06f, 0.06f, 0.065f, 0.065f, 0.07f, 0.075f, 0.08f, 0.085f, 0.09f, 0.95f, 0.1f},
        new float[] { 2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f, 2.6f, 2.7f, 2.8f, 2.9f, 3.0f, 3.1f, 3.2f, 3.3f, 3.4f, 3.5f, 3.6f, 3.7f, 3.8f, 3.9f, 4.0f, 4.25f, 4.5f, 4.75f, 5.0f}, // 반지
        new float[] { 0.03f, 0.03f, 0.03f, 0.03f, 0.035f, 0.035f, 0.035f, 0.035f, 0.04f, 0.04f, 0.04f, 0.04f, 0.04f, 0.04f, 0.04f, 0.04f, 0.045f, 0.045f, 0.045f, 0.045f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f},
        new float[] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 12f, 14f, 18f, 22f, 26f, 30f, 34f, 38f, 42f, 46f, 50f, 55f, 60f, 65f, 70f }, // 목걸이
        new float[] { 0.03f, 0.03f, 0.03f, 0.03f, 0.04f, 0.045f, 0.05f, 0.055f, 0.06f, 0.07f, 0.08f, 0.09f, 0.1f, 0.12f, 0.14f, 0.16f, 0.18f, 0.2f, 0.24f, 0.28f, 0.32f, 0.4f, 0.48f, 0.56f, 0.7f},
        new float[] { 1.0f, 1.05f, 1.1f, 1.15f, 1.2f, 1.25f, 1.3f, 1.35f, 1.4f, 1.45f, 1.5f, 1.55f, 1.6f, 1.65f, 1.7f, 1.75f, 1.8f, 1.85f, 1.9f, 1.95f, 2.0f, 2.1f, 2.2f, 2.3f, 2.5f}, // 검
        new float[] { 0.03f, 0.03f, 0.03f, 0.03f, 0.035f, 0.035f, 0.04f, 0.04f, 0.045f, 0.045f, 0.05f, 0.05f, 0.055f, 0.055f, 0.06f, 0.06f, 0.065f, 0.065f, 0.07f, 0.075f, 0.08f, 0.085f, 0.09f, 0.95f, 0.1f},
    };
    static public float[][] pickReinforce1Percents =
    {
        new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f },
        new float[] { 0.55f, 0.6f, 0.65f, 0.7f },
        new float[] { 0.75f, 0.8f, 0.85f, 0.9f, 0.95f, 1.0f, 1.05f, 1.1f, 1.15f },
        new float[] { 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f },
        new float[] { 2.5f }
    };
    static public float[][] pickReinforce2Percents =
    {
        new float[] { 0.01f, 0.01f, 0.01f, 0.01f, 0.01f },
        new float[] { 0.015f, 0.015f, 0.02f, 0.02f },
        new float[] { 0.025f, 0.025f, 0.025f, 0.03f, 0.03f, 0.03f, 0.035f, 0.035f, 0.035f },
        new float[] { 0.04f, 0.04f, 0.04f, 0.045f, 0.045f, 0.045f, 0.05f, 0.05f, 0.05f },
        new float[] { 0.05f }
    };
    static public float[][] hatReinforce1Percents =
    {
        new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f },
        new float[] { 0.51f, 0.52f, 0.53f, 0.54f },
        new float[] { 0.55f, 0.575f, 0.6f, 0.625f, 0.65f, 0.675f, 0.7f, 0.725f, 0.75f },
        new float[] { 0.775f, 0.8f, 0.825f, 0.85f, 0.875f, 0.9f, 0.925f, 0.95f, 0.975f },
        new float[] { 1.0f }
    };
    static public float[][] hatReinforce2Percents =
    {
        new float[] { 0.01f, 0.01f, 0.01f, 0.01f, 0.01f },
        new float[] { 0.015f, 0.015f, 0.02f, 0.02f },
        new float[] { 0.025f, 0.025f, 0.025f, 0.03f, 0.03f, 0.03f, 0.035f, 0.035f, 0.035f },
        new float[] { 0.04f, 0.04f, 0.04f, 0.045f, 0.045f, 0.045f, 0.05f, 0.05f, 0.05f },
        new float[] { 0.05f }
    };
    static public float[][] ringReinforce1Percents =
    {
        new float[] { 1.0f, 1.0f, 1.0f, 1.1f, 1.2f },
        new float[] { 1.3f, 1.4f, 1.5f, 1.6f },
        new float[] { 1.7f, 1.8f, 1.9f, 2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f },
        new float[] { 2.6f, 2.7f, 2.8f, 2.9f, 3.0f, 3.1f, 3.2f, 3.3f, 3.4f },
        new float[] { 3.5f }
    };
    static public float[][] ringReinforce2Percents =
    {
        new float[] { 0.01f, 0.01f, 0.01f, 0.01f, 0.01f },
        new float[] { 0.015f, 0.015f, 0.015f, 0.015f },
        new float[] { 0.02f, 0.02f, 0.02f, 0.02f, 0.02f, 0.02f, 0.02f, 0.02f, 0.02f },
        new float[] { 0.025f, 0.025f, 0.025f, 0.025f, 0.025f, 0.025f, 0.025f, 0.025f, 0.025f },
        new float[] { 0.03f }
    };
    static public float[][] pendantReinforce1Percents =
    {
        new float[] { 1f, 1f, 1f, 1f, 2f },
        new float[] { 3f, 4f, 5f, 6f },
        new float[] { 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f },
        new float[] { 16f, 17f, 18f, 19f, 20f, 22f, 24f, 26f, 28f },
        new float[] { 30f }
    };
    static public float[][] pendantReinforce2Percents =
    {
        new float[] { 0.01f, 0.01f, 0.01f, 0.01f, 0.01f },
        new float[] { 0.015f, 0.015f, 0.02f, 0.02f },
        new float[] { 0.025f, 0.025f, 0.03f, 0.03f, 0.035f, 0.035f, 0.04f, 0.045f, 0.05f },
        new float[] { 0.055f, 0.06f, 0.065f, 0.07f, 0.075f, 0.08f, 0.085f, 0.09f, 0.095f },
        new float[] { 0.1f }
    };
    static public float[][] swordReinforce1Percents =
    {
        new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f },
        new float[] { 0.51f, 0.52f, 0.53f, 0.54f },
        new float[] { 0.55f, 0.575f, 0.6f, 0.625f, 0.65f, 0.675f, 0.7f, 0.725f, 0.75f },
        new float[] { 0.775f, 0.8f, 0.825f, 0.85f, 0.875f, 0.9f, 0.925f, 0.95f, 0.975f },
        new float[] { 1.0f }
    };
    static public float[][] swordReinforce2Percents =
    {
        new float[] { 0.01f, 0.01f, 0.01f, 0.01f, 0.01f },
        new float[] { 0.015f, 0.015f, 0.02f, 0.02f },
        new float[] { 0.025f, 0.025f, 0.025f, 0.03f, 0.03f, 0.03f, 0.035f, 0.035f, 0.035f },
        new float[] { 0.04f, 0.04f, 0.04f, 0.045f, 0.045f, 0.045f, 0.05f, 0.05f, 0.05f },
        new float[] { 0.05f }
    };
    static public float[] reinforceSuccessPercents, reinforceDestroyPercents;

    static public int[] questNums = { 6, 12, 19, 20 };
    static public int hasQuestNum = 4;
    static public SubQuest[][] quests;

    static public int bufItemNum, bufItemCodeNum, bufItemTypeNum, bufItemTime;
    static public int reinforceItemNum = 8;
    static public List<BufItem> bufItems;
    static public List<ReinforceItem> reinforceItems;

    static public int mineSlimeQualityNum = 11;
    static public int mineInvenMinNum = 10;
    static public int mineInvenMaxNum = 30;
    static public int mineUpgradeNum = 6;
    static public int mineSlotMinNum = 3;
    static public int mineSlotMaxNum = 30;
    static public float[] mineUpgradePercents = new float[] { 60f, 0.05f, 0.02f, 1f, 1f, 1f };

    static private bool isDataChange = false;

    static public bool isGameLock = false;
    static public bool isShopLock = false;
    static public bool isInvenLock = false;
    static public bool isMineLock = false;
    static public bool isGMLock = false;

    static public int achievementNum = 40;

    static public float[] reinforceTree_createPercents = { 0.1f, 0.3f, 0.025f }; // { 아무 나무가 생성될 확률, 대형 나무일 확률, 중형 나무일 확률 }
    static public int facility_rewardTime = 12 * 60 * 60;
    static public int manaBufType = 3;
    static public int manaBufNum = 5 * manaBufType;
    static public int manaBufTime = 15 * 60;
    static public List<ManaBuf> manaBufs;
    static public int maxPetReward = 100;

    static public int manaItemNum = 9;
    static public int manaUpgradeNum = 15;
    static public List<ManaItem> manaItems;
    static public List<ManaUpgrade> manaUpgrades;

    static public int[] OreDamageAsQuality = { 1, 2, 3, 5, 10, 100, 1000 };
    static public float[] cardDropPercents = { 0.175f, 0.15f, 0.125f, 0.075f, 0.05f, 0.75f, 1f };
    static public int[] cardLevelsPerFloor = { 20, 40, 60, 80, 100, 200, 300 };
    static public int[][] cardForceAsType =
    {
        new int[] {0, 9, 23, 29, 36, 42, 48, 54, 60, 67, 74, 81}, // 내구도
        new int[] {7, 16, 30}, // 채광속도
        new int[] {1, 10, 24, 31, 37, 43, 49, 55, 61, 68, 75, 82}, // 방어력
        new int[] {4, 17}, // 방어력 확률
        new int[] {3, 12, 26, 33, 39, 45, 51, 57, 63, 70, 77, 84}, // 추가 판매
        new int[] {19}, // 추가 판매 확률
        new int[] {13, 34, 40, 46, 52, 58, 64, 71, 78, 85}, // 광물 추가
        new int[] {6, 20}, // 광물 추가 확률
        new int[] {2, 11, 25, 32, 38, 44, 50, 56, 62, 69, 76, 83}, // 공격력
        new int[] {5, 18}, // 공격력 확률
        new int[] {14}, // 경험치 (10)
        new int[] {15}, // 마나석 광석 추가 획득
        new int[] {8}, // 경험치 2배 추가 획득
        new int[] {21}, // 펫 드랍률
        new int[] {22}, // 고대 도서관 조사에서 얻는 주문서
        new int[] {27}, // 아이템 드랍률
        new int[] {28}, // 상자에서 마나석 획득량
        new int[] {35}, // 광물 컬렉션 획득 확률
        new int[] {41}, // 보스 몬스터 마나석 획득량
        new int[] {47}, // 얼티밋 광물 등장 확률
        new int[] {53}, // 이벤트 포탈 지속 시간
        new int[] {59}, // 펫 합성 확률
        new int[] {65, 66, 72, 73, 79, 80}, // 미스틱 광물 등장 확률
        new int[] {86, 87, 93, 94}, // 성장하는 돌 등장 확률
    };
    static public List<Collection> collections;

    static public int elixirTime = 30 * 60;
    static public List<Elixir> elixirs;

    static public Text exitInfoText;
    static public int playTime = 0;
    static public int macroTime = 60 * 60; // 1시간

    static public int reinforceItem2Num = 6;
    static public List<ReinforceItem2> reinforceItems2;

    static public List<MainQuest> mainQuests;

    static public List<CashItem> cashItems;
    static public List<CashEquipment> cashEquipments;
    static public int[] cashItemNums = { 6, 6, 6 };
    static public int totalCashItemNum;
    static public int cashEquipmentNum = 4;

    static public float growthOre_createPercent;

    static public int vendingMachineTime = 6 * 60 * 60;
    static public int mailboxNum = 10;
    static public int mailboxTime = 9 * 60 * 60;

    static public float manaOreCreatePercent = 0.005f;

    static public int combo_max = 10000;
    static public int combo_unit = 2000;
    static public int[] combo_plus = { 1, 10, 10, 10, 100 };
    static public float[] combo_forces = { 0.5f, 0.5f, 0.5f, 0.5f, 1f };
    static public float[] combo_multiply = { 1f, 2f, 4f };

    // Temp Variables
    static private BackendReturnObject BRO, BRO1, BRO2;
    static private Param param = new Param();
    static private Param param2 = new Param();
    static private Param param3 = new Param();
    static private Param param4 = new Param();
    static private Param privateParam = new Param();
    static private Param publicParam = new Param();

    void Awake()
    { 
        if (instance == null)
        {
            stat = new Stat();
            saveRank = GetComponentInChildren<SaveRank>();
            quitCanvas = GetComponentInChildren<Canvas>();
            endCanvas = GetComponentsInChildren<Canvas>()[1];
            autoCanvas = GetComponentsInChildren<Canvas>()[2];
            errorCanvas = GetComponentsInChildren<Canvas>()[4];
            auto_animator = autoCanvas.GetComponentInChildren<Animator>();

            SEs = Resources.LoadAll<AudioClip>("Sounds/SE");
            BGMs = Resources.LoadAll<AudioClip>("Sounds/BGM");

            ticketNum = 5;
            ticketTime = 3 * 60;
            rulletTime = 15 * 60;
            iconNum = 12;
            iconTime = 30 * 60;
            pickStateNum = 3;
            accessoryNum = 5;
            qualityNum = 7;
            pickNum = hatNum = RingNum = PendantNum = swordNum = stageNum;
            blockTileNum = 6;
            brokenTileNum = 5;
            minCreateJemNuM = 2;
            maxCreateJemNum = 8;
            createMinePercent = 0.05f;
            createDungeon_0_Percent = 0.075f;
            createDungeon_1_Percent = 0.015f;
            createKingSlimeRoomPercent = 0.05f;
            growthOre_createPercent = 0.000025f;

            Debug.Log("SaveScript is done.");

            for (int i = 0; i < stageItemNums.Length; i++)
                totalItemNum += stageItemNums[i];

            jems = new List<Jem>();
            for (int i = 0; i < totalItemNum; i++)
                jems.Add(new Jem(i));

            picks = new List<Pick>();
            for (int i = 0; i < pickNum; i++)
                picks.Add(new Pick(i));

            hats = new List<Hat>();
            for (int i = 0; i < hatNum; i++)
                hats.Add(new Hat(i));

            rings = new List<Ring>();
            for (int i = 0; i < RingNum; i++)
                rings.Add(new Ring(i));

            pendants = new List<Pendant>();
            for (int i = 0; i < PendantNum; i++)
                pendants.Add(new Pendant(i));

            swords = new List<Sword>();
            for (int i = 0; i < swordNum; i++)
                swords.Add(new Sword(i));

            icons = new List<Icon>();
            for (int i = 0; i < iconNum; i++)
                icons.Add(new Icon(i));

            monsterColors = new Color[stageNum];
            monsterColors[0] = new Color(0.6f, 0.5f, 0.4f, 0f);
            monsterColors[1] = new Color(0.5f, 0.4f, 0.4f, 0f);
            monsterColors[2] = new Color(0.6f, 0.6f, 0.6f, 0f);
            monsterColors[3] = new Color(0.4f, 0.4f, 0.4f, 0f);
            monsterColors[4] = new Color(0.6f, 0.4f, 0.6f, 0f);
            monsterColors[5] = new Color(0.7f, 0.5f, 0.5f, 0f);
            monsterColors[6] = new Color(0.5f, 0.7f, 0.5f, 0f);
            monsterColors[7] = new Color(0.5f, 0.7f, 0.7f, 0f);
            monsterColors[8] = new Color(0.5f, 0.5f, 0.7f, 0f);
            monsterColors[9] = new Color(0.8f, 0.5f, 0.8f, 0f);
            monsterColors[10] = new Color(1f, 0.6f, 0.6f, 0f);
            monsterColors[11] = new Color(1f, 1f, 0.7f, 0f);
            monsterColors[12] = new Color(0.6f, 0.6f, 0.5f, 0f);
            monsterColors[13] = new Color(0.2f, 0.2f, 0.2f, 0f);
            monsterColors[14] = new Color(0.7f, 0.8f, 1f, 0f);

            stageColors = new Color[stageNum]; // 배경 색상
            stageColors[0] = new Color(1f, 0.9f, 0.7f);
            stageColors[1] = new Color(0.6f, 0.4f, 0.4f);
            stageColors[2] = new Color(0.7f, 0.7f, 0.7f);
            stageColors[3] = new Color(0.5f, 0.5f, 0.5f);
            stageColors[4] = new Color(0.5f, 0.4f, 0.6f);
            stageColors[5] = new Color(0.7f, 0.4f, 0.4f);
            stageColors[6] = new Color(0.6f, 0.7f, 0.5f);
            stageColors[7] = new Color(0.5f, 0.8f, 0.8f);
            stageColors[8] = new Color(0.5f, 0.5f, 0.8f);
            stageColors[9] = new Color(0.7f, 0.5f, 0.8f);
            stageColors[10] = new Color(0.9f, 0.6f, 0.6f);
            stageColors[11] = new Color(0.9f, 1f, 0.7f);
            stageColors[12] = new Color(0.6f, 0.6f, 0.5f);
            stageColors[13] = new Color(0.3f, 0.3f, 0.3f);
            stageColors[14] = new Color(0.8f, 0.9f, 1f);

            toolColors = new Color[stageNum]; // 장비 색상
            toolColors[0] = new Color(0.5f, 0.25f, 0f);
            toolColors[1] = new Color(0.5f, 0.5f, 0.5f);
            toolColors[2] = new Color(0.9f, 0.9f, 0.9f);
            toolColors[3] = new Color(1f, 1f, 0f);
            toolColors[4] = new Color(0f, 1f, 1f);
            toolColors[5] = new Color(0.2f, 0.2f, 0.2f);
            toolColors[6] = new Color(0.6f, 1f, 0.6f);
            toolColors[7] = new Color(0.3f, 0.6f, 0.6f);
            toolColors[8] = new Color(0.3f, 0.3f, 0.7f);
            toolColors[9] = new Color(0.7f, 0.9f, 0.9f);
            toolColors[10] = new Color(1f, 0.6f, 0.6f);
            toolColors[11] = new Color(1f, 1f, 0.7f);
            toolColors[12] = new Color(1f, 0.8f, 0.9f);
            toolColors[13] = new Color(0.1f, 0.1f, 0.1f);
            toolColors[14] = new Color(0.4f, 0.8f, 0.8f);

            qualityColors = new Color[qualityNum];
            qualityColors[0] = new Color(0.7f, 0.7f, 0.7f, 1f); // 회색
            qualityColors[1] = new Color(0, 1f, 1f, 1f); // 하늘색
            qualityColors[2] = new Color(1f, 0, 1f, 1f); // 보라색
            qualityColors[3] = Color.yellow; // 노란색
            qualityColors[4] = Color.red; // 빨간색
            qualityColors[5] = Color.green; // 녹색
            qualityColors[6] = new Color(0f, 0.6f, 1f); // 푸른색

            qualityColors_weak = new Color[qualityNum];
            qualityColors_weak[0] = new Color(0.7f, 0.7f, 0.7f); // 회색
            qualityColors_weak[1] = new Color(0.5f, 0.9f, 0.9f); // 하늘색
            qualityColors_weak[2] = new Color(1f, 0.6f, 1f); // 보라색
            qualityColors_weak[3] = new Color(1f, 1f, 0.6f); // 노란색
            qualityColors_weak[4] = new Color(1f, 0.6f, 0.6f); // 빨간색
            qualityColors_weak[5] = new Color(0.6f, 1f, 0.6f); // 녹색
            qualityColors_weak[6] = new Color(0.7f, 0.9f, 1f); // 푸른색

            reinforceSuccessPercents = new float[reinforceNumAsQulity * 5];
            reinforceSuccessPercents[0] = 1f;
            reinforceSuccessPercents[1] = 1f;
            reinforceSuccessPercents[2] = 1f;
            reinforceSuccessPercents[3] = 1f;
            reinforceSuccessPercents[4] = 0.95f;
            reinforceSuccessPercents[5] = 0.9f;
            reinforceSuccessPercents[6] = 0.85f;
            reinforceSuccessPercents[7] = 0.8f;
            reinforceSuccessPercents[8] = 0.75f;
            reinforceSuccessPercents[9] = 0.7f;
            reinforceSuccessPercents[10] = 0.65f;
            reinforceSuccessPercents[11] = 0.6f;
            reinforceSuccessPercents[12] = 0.55f;
            reinforceSuccessPercents[13] = 0.5f;
            reinforceSuccessPercents[14] = 0.45f;
            reinforceSuccessPercents[15] = 0.4f;
            reinforceSuccessPercents[16] = 0.35f;
            reinforceSuccessPercents[17] = 0.3f;
            reinforceSuccessPercents[18] = 0.3f;
            reinforceSuccessPercents[19] = 0.3f;

            reinforceDestroyPercents = new float[reinforceNumAsQulity * 5];
            reinforceDestroyPercents[0] = 0f;
            reinforceDestroyPercents[1] = 0f;
            reinforceDestroyPercents[2] = 0f;
            reinforceDestroyPercents[3] = 0f;
            reinforceDestroyPercents[4] = 0f;
            reinforceDestroyPercents[5] = 0f;
            reinforceDestroyPercents[6] = 0f;
            reinforceDestroyPercents[7] = 0f;
            reinforceDestroyPercents[8] = 0.01f;
            reinforceDestroyPercents[9] = 0.01f;
            reinforceDestroyPercents[10] = 0.02f;
            reinforceDestroyPercents[11] = 0.02f;
            reinforceDestroyPercents[12] = 0.03f;
            reinforceDestroyPercents[13] = 0.03f;
            reinforceDestroyPercents[14] = 0.04f;
            reinforceDestroyPercents[15] = 0.04f;
            reinforceDestroyPercents[16] = 0.05f;
            reinforceDestroyPercents[17] = 0.08f;
            reinforceDestroyPercents[18] = 0.1f;
            reinforceDestroyPercents[19] = 0.15f;

            quests = new SubQuest[stageNum][];
            for (int i = 0; i < stageNum; i++)
            {
                quests[i] = new SubQuest[questNums[SubQuest.GetSubQuestType(i)]];
                for (int j = 0; j < quests[i].Length; j++)
                    quests[i][j] = new SubQuest(i, j);
            }

            bufItemCodeNum = 14; // 물약 종류를 나타낸다
            bufItemTypeNum = 3;  // 초급, 중급, 상급을 나타낸다.
            bufItemNum = bufItemCodeNum * bufItemTypeNum;
            bufItemTime = 15 * 60;
            bufItems = new List<BufItem>();
            for (int i = 0; i < bufItemNum; i++)
                bufItems.Add(new BufItem(i));
            reinforceItems = new List<ReinforceItem>();
            for (int i = 0; i < reinforceItemNum; i++)
                reinforceItems.Add(new ReinforceItem(i));
            manaBufs = new List<ManaBuf>();
            for (int i = 0; i < manaBufNum; i++)
                manaBufs.Add(new ManaBuf(i));
            manaItems = new List<ManaItem>();
            for (int i = 0; i < manaItemNum; i++)
                manaItems.Add(new ManaItem(i));
            manaUpgrades = new List<ManaUpgrade>();
            for (int i = 0; i < manaUpgradeNum; i++)
                manaUpgrades.Add(new ManaUpgrade(i));

            collections = new List<Collection>();
            for (int i = 0; i < totalItemNum; i++)
                collections.Add(new Collection(i));

            elixirs = new List<Elixir>();
            for (int i = 0; i < bufItemCodeNum; i++)
                elixirs.Add(new Elixir(i));

            reinforceItems2 = new List<ReinforceItem2>();
            for (int i = 0; i < reinforceItem2Num; i++)
                reinforceItems2.Add(new ReinforceItem2(i));

            mainQuests = new List<MainQuest>();
            for (int i = 0; i < mainQuestNum; i++)
                mainQuests.Add(new MainQuest(i));

            for (int i = 0; i < cashItemNums.Length; i++)
                totalCashItemNum += cashItemNums[i];

            cashItems = new List<CashItem>();
            for (int i = 0; i < totalCashItemNum; i++)
                cashItems.Add(new CashItem(i));

            cashEquipments = new List<CashEquipment>();
            for (int i = 0; i < cashEquipmentNum; i++)
                cashEquipments.Add(new CashEquipment(i));

            exitInfoText = endCanvas.GetComponentInChildren<Text>();
            isAutoSave = isAsyncSave = false;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            rankUuid_gold = "5909d0e0-774f-11ec-ad88-ed8dfc13b025";
            rankUuid_gold2 = "89060690-76e3-11ec-b102-cd7b5f930bd9";
            rankUuid_gold3 = "0fcd6e00-115f-11ed-9542-4db8e0f1457b";
            rankUuid_gold4 = "1ca8d3c0-6d58-11ee-866d-39853ca909e5";

            quitCanvas.enabled = false;
            endCanvas.enabled = false;
            errorCanvas.enabled = false;
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            saveData = new SaveData();
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        CheckGold();
        if(!isSetTime)
            StartCoroutine("SetTime");

        if (SceneManager.GetActiveScene().name == "GameScene" && !isAutoSave)
            StartCoroutine("AutoSave");

        if (Input.GetKeyDown(KeyCode.Escape) && Application.platform == RuntimePlatform.Android)
        {
            clickCount++;

            if(clickCount == 1)
            {
                StartCoroutine(DoubleClick());
            }
            else if(clickCount == 2)
            {
                quitCanvas.enabled = true;
            }
        }

        if (isGMLock && BackEndLoginManager.isGM)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (++saveData.pickLevel == stageNum)
                    saveData.pickLevel = 0;
                Debug.Log("Pick 레벨 : " + saveData.pickLevel);
                saveData = GM.GetSaveDataAsFloor(saveData.pickLevel);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("메인 퀘스트 Goal 1 증가");
                if (saveData.mainQuest_list < mainQuestNum)
                    QuestCtrl.instance.SetMainQuestAmount(new int[] { saveData.mainQuest_list });
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (saveData.mainQuest_list > 0)
                {
                    saveData.mainQuest_list--;
                    saveData.mainQuest_goal = mainQuests[saveData.mainQuest_list].goal - 1;
                    Debug.Log("메인 퀘스트 단계 감소 : " + saveData.mainQuest_list);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (saveData.mainQuest_list < mainQuestNum - 1)
                {
                    saveData.mainQuest_list++;
                    saveData.mainQuest_goal = mainQuests[saveData.mainQuest_list].goal - 1;
                    Debug.Log("메인 퀘스트 단계 증가 : " + saveData.mainQuest_list);
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        GameCloseButton();
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("App Pause : " + pause);
        if (pause)
        {
            // 게임 멈춤
            pause_time = Backend.Utils.GetServerTime().GetReturnValuetoJSON()["utcTime"].ToString();
            if (!saveData.isTutorial)
                SaveData_Syn();
        }
        else
        {
            // 게임 재개
            if(pause_time != null)
            {
                DateTime pauseTime = DateTime.Parse(pause_time);
                DateTime currentTime = DateTime.Parse(Backend.Utils.GetServerTime().GetReturnValuetoJSON()["utcTime"].ToString());
                TimeSpan timeSpan = currentTime - pauseTime;
                int savedData = (int)timeSpan.TotalSeconds;
                int emptyDate = (ticketNum - saveData.ticketNum - 1) * ticketTime + saveData.ticketTime - savedData;
                Debug.Log("지나간 초 : " + savedData + " 초");
                SetDataAsTime(savedData, emptyDate);
                Chat.instance.SetChatStatus();

                // 우편물 처리
                DateTime tempTime = pauseTime;
                int second;
                while (pauseTime.CompareTo(currentTime) < 0)
                {
                    timeSpan = currentTime - pauseTime;
                    second = (int)timeSpan.TotalSeconds;

                    // 9시간 간격 체크 => 마나석 상품
                    if (second >= saveData.mailboxTime)
                    {
                        pauseTime = pauseTime.AddSeconds(saveData.mailboxTime);
                        saveData.mailboxTime = mailboxTime;
                        GameFuction.PushMail(GameFuction.GetMailValue(0));
                    }
                    else
                        pauseTime = pauseTime.AddSeconds(second + 1);

                    // Day 지남 체크 => 레드 다이아 상품
                    if (tempTime.Day != pauseTime.Day)
                    {
                        tempTime = pauseTime;
                        GameFuction.PushMail(GameFuction.GetMailValue(1));
                    }

                    if (GameFuction.GetEmptyMail() == -1)
                        break;
                }

                // 씬 처리
                if (SceneManager.GetActiveScene().name == "GameScene")
                {
                    AttackCtrl.instance.OnPointerUp(null);
                    MoveCtrl.instance.SetInit();
                    if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn)
                        AutoPlayCtrl.instance.SetInit();
                }
                else if (SceneManager.GetActiveScene().name == "InventoryScene")
                {
                    ReinforceItemUse.instance.SetInvenSlots();
                }
                else if (SceneManager.GetActiveScene().name == "MineScene")
                {
                    MineFusionUI.instance.FusionUI_SetDefault();
                }
            }
        }
    }

    static public void SetFPS()
    {
        switch (saveData.fpsType)
        {
            case 0: Application.targetFrameRate = -1; break;
            case 1: Application.targetFrameRate = 144; break;
            case 2: Application.targetFrameRate = 120; break;
            case 3: Application.targetFrameRate = 90; break;
            case 4: Application.targetFrameRate = 60; break;
            case 5: Application.targetFrameRate = 30; break;
        }
    }

    static public void SetDataAsStat()
    {
        for (int i = 0; i < iconNum; i++)
            icons[i].SetData();
        for (int i = 0; i < bufItemNum; i++)
            bufItems[i].SetInfo();
        for (int i = 0; i < bufItemCodeNum; i++)
            elixirs[i].SetData();
        for (int i = 0; i < totalCashItemNum; i++)
            cashItems[i].SetData();
    }

    public void GameCloseButton()
    {
        if (!saveData.isTutorial)
            SaveData_Syn();
        Application.Quit();
    }

    public void CancelButton()
    {
        quitCanvas.enabled = false;
    }

    public void Error_QuitGame()
    {
        Application.Quit();
    }

    IEnumerator DoubleClick()
    {
        yield return new WaitForSeconds(1f);
        clickCount = 0;
    }

    IEnumerator AutoSave()
    {
        isAutoSave = true;

        yield return new WaitForSeconds(AUTOSAVE_TIME);

        isAutoSave = false;
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            SaveData_Asyn(false);
            Debug.Log("AutoSaving...");
        }    
    }

    IEnumerator ReSet_AsyncSave()
    {
        isAsyncSave = true;
        yield return new WaitForSeconds(ASYNCSAVE_TIME);
        isAsyncSave = false;
    }

    static private void SetDataAsTime(int savedData, int emptyDate)
    {
        // 티켓 정리
        if (emptyDate < 0)
        {
            saveData.ticketNum = ticketNum;
            saveData.ticketTime = 0;
        }
        else
        {
            saveData.ticketNum = ticketNum - emptyDate / ticketTime - 1;
            saveData.ticketTime = emptyDate % ticketTime;
        }

        // 룰렛 정리
        if (saveData.rulletTime <= savedData)
            saveData.rulletTime = 0;
        else
            saveData.rulletTime -= savedData;

        // 뽑기 정리
        if (saveData.vendingMachineTime <= savedData)
            saveData.vendingMachineTime = 0;
        else
            saveData.vendingMachineTime -= savedData;

        // 아이콘 정리
        for (int i = 0; i < iconNum; i++)
        {
            if (saveData.hasIcons[i])
            {
                if (saveData.iconsTime[i] <= savedData)
                {
                    saveData.iconsTime[i] = 0;
                    saveData.hasIcons[i] = false;
                    isChangedIcons = true;
                }
                else
                {
                    saveData.iconsTime[i] -= savedData;
                }
            }
        }

        // 마나석 버프 정리
        for (int i = 0; i < manaBufNum; i++)
        {
            if (saveData.isManaBuffOns[i])
            {
                if (saveData.manaBufTimes[i] <= savedData)
                {
                    saveData.manaBufTimes[i] = 0;
                    saveData.isManaBuffOns[i] = false;
                    isChangedIcons = true;
                }
                else
                {
                    saveData.manaBufTimes[i] -= savedData;
                }
            }
        }

        // 영약 정리
        for (int i = 0; i < bufItemCodeNum; i++)
        {
            if (saveData.isElixirOns[i])
            {
                if (saveData.elixirTimes[i] <= savedData)
                {
                    saveData.elixirTimes[i] = 0;
                    saveData.isElixirOns[i] = false;
                    isChangedIcons = true;
                }
                else
                {
                    saveData.elixirTimes[i] -= savedData;
                }
            }
        }

        // 버프 물약 정리
        for (int i = 0; i < bufItemNum; i++)
        {
            if (saveData.isBufItemOns[i])
            {
                if (saveData.bufItemTimes[i] <= savedData)
                {
                    saveData.bufItemTimes[i] = 0;
                    saveData.isBufItemOns[i] = false;
                    isChangedIcons = true;
                }
                else
                {
                    saveData.bufItemTimes[i] -= savedData;
                }
            }
        }

        // 광산 정리
        int mineTime = savedData;
        if (mineTime < saveData.facility_rewardTime)
            saveData.facility_rewardTime -= mineTime;
        else
            saveData.facility_rewardTime = 0;

        for (int i = 0; i < mineSlotMaxNum; i++)
        {
            if (saveData.hasOnMiners[i] != -1)
            {
                mineTime = savedData;
                if (mineTime < saveData.hasMinerTimes[i]) saveData.hasMinerTimes[i] -= mineTime;
                else
                {
                    saveData.hasMinerRewards[i]++;
                    mineTime -= saveData.hasMinerTimes[i];

                    int timeAsLevel = MinerSlime.GetTimeAsLevel(saveData.hasOnMiners[i], saveData.hasOnMinerLevels[i]);
                    while (mineTime >= timeAsLevel)
                    {
                        mineTime -= timeAsLevel;
                        saveData.hasMinerRewards[i]++;
                    }
                    saveData.hasMinerTimes[i] = timeAsLevel - mineTime;
                    if (saveData.hasMinerRewards[i] >= maxPetReward)
                        saveData.hasMinerRewards[i] = maxPetReward;
                }
            }

            if (saveData.hasOnAdventurers[i] != -1)
            {
                mineTime = savedData;
                // 버그 방지
                if (saveData.hasAdventurerTimes[i] > 24 * 60 * 60)
                    saveData.hasAdventurerTimes[i] = 0;
                if (mineTime < saveData.hasAdventurerTimes[i]) saveData.hasAdventurerTimes[i] -= mineTime;
                else
                {
                    saveData.hasAdventurerRewards[i]++;
                    mineTime -= saveData.hasAdventurerTimes[i];

                    int timeAsLevel = AdventurerSlime.GetTimeAsLevel(saveData.hasOnAdventurers[i], saveData.hasOnAdventurerLevels[i]);
                    while (mineTime >= timeAsLevel)
                    {
                        mineTime -= timeAsLevel;
                        saveData.hasAdventurerRewards[i]++;
                    }
                    saveData.hasAdventurerTimes[i] = timeAsLevel - mineTime;
                    if (saveData.hasAdventurerRewards[i] >= maxPetReward)
                        saveData.hasAdventurerRewards[i] = maxPetReward;
                }
            }
        }
    }

    static public void LoadData()
    {
        bool isProcessing = true;
        if (!Backend.IsInitialized) return;

        saveData = new SaveData();
        BRO2 = Backend.GameData.GetMyData("publicData", new Where());
        if (BRO2.IsSuccess())
        {
            JsonData json2 = BRO2.GetReturnValuetoJSON()["rows"][0];
            PublicDataParsing(json2);
        }
        else
        {
            isProcessing = false;
            Debug.Log("서버 공통 에러 발생: " + BRO2.GetMessage());
        }

        BRO1 = Backend.GameData.GetMyData("privateData", new Where());
        if (BRO1.IsSuccess())
        {
            JsonData json1 = BRO1.GetReturnValuetoJSON()["rows"][0];
            PrivateDataParsing(json1);
        }
        else
        {
            isProcessing = false;
            Debug.Log("서버 공통 에러 발생: " + BRO1.GetMessage());
        }

        if (!isProcessing) 
        {
            if (processCount >= PROCESS_COUNT)
            {
                errorCanvas.enabled = true;
                Debug.LogError("Error: Processing is not valid.");
                return;
            }
                
            processCount++;
            LoadData(); 
            return; 
        }
        if (isDataChange) 
        { 
            if (processCount >= PROCESS_COUNT)
            {
                errorCanvas.enabled = true;
                Debug.LogError("Error: DataChanging is not valid.");
                return;
            }

            SaveData_Syn();
            processCount++;
            isDataChange = false; 
            LoadData(); 
            return; 
        }

        maxPetReward = 100 + (int)GameFuction.GetManaUpgradeForce(2);
        processCount = 0;

        BRO = Backend.Utils.GetServerTime();
        if(!BRO.IsSuccess()) BRO = Backend.Utils.GetServerTime();
        string serverUTCTime = BRO.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime currentDateTime = DateTime.Parse(serverUTCTime);
        DateTime closedDateTime = DateTime.Parse(saveData.closeDate);
        TimeSpan timeSpan = currentDateTime - closedDateTime;
        int savedData = (int)timeSpan.TotalSeconds;
        int emptyDate = (ticketNum - saveData.ticketNum - 1) * ticketTime + saveData.ticketTime - savedData;
        dateTime = DateTime.Parse(serverUTCTime);
        EventCtrl.instance.SetInit();
        GameFuction.SetAttendanceData();
        BlindScript.instance.audio.mute = !saveData.isBGMOn;
        Debug.Log("지나간 초 : " + savedData + " 초");
        SetDataAsTime(savedData, emptyDate);

        SetDataAsStat();

        // 업데이트 세팅
        if (saveData.gold != 0)
            saveData.isTutorial = false;

        // 구글 인앱 - 광거 제거 적용
        if(GoogleInApp.instance != null && GoogleInApp.instance.isInitialized) 
            saveData.isRemoveAD = GoogleInApp.instance.HadPurchased_removeAD() || GoogleInApp.instance.HadPurchased_package();
        
        // 날이 다름
        if (closedDateTime.Day != currentDateTime.Day)
        {
            Setquest(); // 퀘스트 정리
            saveData.combo_gauge = 0;
        }

        // 우편물 처리
        DateTime tempTime = closedDateTime;
        int second;
        while (closedDateTime.CompareTo(currentDateTime) < 0)
        {
            timeSpan = currentDateTime - closedDateTime;
            second = (int)timeSpan.TotalSeconds;

            // 9시간 간격 체크 => 마나석 상품
            if (second >= saveData.mailboxTime)
            {
                closedDateTime = closedDateTime.AddSeconds(saveData.mailboxTime);
                saveData.mailboxTime = mailboxTime;
                GameFuction.PushMail(GameFuction.GetMailValue(0));
            }
            else
                closedDateTime = closedDateTime.AddSeconds(second + 1);

            // Day 지남 체크 => 레드 다이아 상품
            if (tempTime.Day != closedDateTime.Day)
            {
                tempTime = closedDateTime;
                GameFuction.PushMail(GameFuction.GetMailValue(1));
            }

            if (GameFuction.GetEmptyMail() == -1)
                break;
        }

        SaveData_Syn();
    }

    static public void SaveData_Syn()
    {
        if (!Backend.IsInitialized) return;
        if (Backend.URank.User.GetRankList(rankUuid_gold2, 1, 1).GetErrorCode() != null)
        {
            Debug.LogError("서버 랭킹 데이터 에러 발생");
            endCanvas.enabled = true;
            exitInfoText.text = "현재 서버 측에 문제가 발생했습니다.\n 잠시 후에 다시 접속해주세요.";
            return;
        }

        if (!isDataChange)
        {
            BRO = Backend.Utils.GetServerTime();
            if(!BRO.IsSuccess()) BRO = Backend.Utils.GetServerTime();
            saveData.closeDate = BRO.GetReturnValuetoJSON()["utcTime"].ToString();
            dateTime = DateTime.Parse(BRO.GetReturnValuetoJSON()["utcTime"].ToString());
            EventCtrl.instance.SetInit();
            GameFuction.SetComboData();
            GameFuction.SetAttendanceData();
        }
        Debug.Log("데이터 저장 <현재 시간> : " + saveData.closeDate);
        MineFusionUI.SetFusionVariable();
        MineDecompositionUI.SetDecompositionVariable();
        ReinforceItemUse.SetReinforceVariable();
        GameFuction.SettingGold();

        // 랭킹 업데이트
        param = new Param();
        long gold = 0, gold2 = 0, gold3 = 0, gold4 = 0;

        if (saveData.gold4 > 0) gold4 = saveData.gold4;
        else if (saveData.gold3 > 0) gold3 = saveData.gold3;
        else if (saveData.gold2 > 0) gold2 = saveData.gold2;
        else gold = saveData.gold;

        if (BackEndLoginManager.userData.nickname != null)
        {
            param.Clear();
            param.Add("gold4", gold4);
            BRO = Backend.URank.User.UpdateUserScore(rankUuid_gold4, "privateData", saveData.privateData_Indate, param);
            if (!BRO.IsSuccess())
                Debug.LogError("랭킹 업데이트_gold4 <error> = " + BRO.GetErrorCode());

            param.Clear();
            param.Add("gold3", gold3);
            BRO = Backend.URank.User.UpdateUserScore(rankUuid_gold3, "privateData", saveData.privateData_Indate, param);
            if (!BRO.IsSuccess())
                Debug.LogError("랭킹 업데이트_gold3 <error> = " + BRO.GetErrorCode());

            param.Clear();
            param.Add("gold2", gold2);
            BRO = Backend.URank.User.UpdateUserScore(rankUuid_gold2, "privateData", saveData.privateData_Indate, param);
            if (!BRO.IsSuccess())
                Debug.LogError("랭킹 업데이트_gold2 <error> = " + BRO.GetErrorCode());

            param.Clear();
            param.Add("gold", gold);
            BRO = Backend.URank.User.UpdateUserScore(rankUuid_gold, "privateData", saveData.privateData_Indate, param);
            if (!BRO.IsSuccess())
                Debug.LogError("랭킹 업데이트_gold <error> = " + BRO.GetErrorCode());
        }
        else
        {
            Debug.Log("랭킹 업데이트 <error> = 닉네임 존재 안함 ");
        }

        privateParam = new Param();
        publicParam = new Param();
        SettingPrivateParam(privateParam);
        SettingPublicParam(publicParam);

        BRO = Backend.GameData.Update("privateData", new Where(), privateParam);
        if (!BRO.IsSuccess())
            Debug.LogError("privateData 서버 저장 <error> = " + BRO.GetErrorCode());

        BRO = Backend.GameData.Update("publicData", new Where(), publicParam);
        if (!BRO.IsSuccess())
            Debug.LogError("publicData 서버 저장 <error> = " + BRO.GetErrorCode());
    }

    public void SaveData_Asyn(bool _isCompulsion)
    {
        if (!Backend.IsInitialized) return;
        if (isAsyncSave && !_isCompulsion) return;
        if (Backend.URank.User.GetRankList(rankUuid_gold2, 1, 1).GetErrorCode() != null)
        {
            Debug.LogError("서버 랭킹 데이터 에러 발생");
            endCanvas.enabled = true;
            exitInfoText.text = "현재 서버 측에 문제가 발생했습니다.\n 잠시 후에 다시 접속해주세요.";
            return;
        }

        if (!isDataChange)
        {
            BRO = Backend.Utils.GetServerTime();
            if (!BRO.IsSuccess()) BRO = Backend.Utils.GetServerTime();
            saveData.closeDate = BRO.GetReturnValuetoJSON()["utcTime"].ToString();
            dateTime = DateTime.Parse(BRO.GetReturnValuetoJSON()["utcTime"].ToString());
            EventCtrl.instance.SetInit();
            GameFuction.SetComboData();
            GameFuction.SetAttendanceData();
        }

        StopCoroutine("ReSet_AsyncSave");
        StartCoroutine("ReSet_AsyncSave");
        Debug.Log("데이터 저장 <현재 시간> : " + saveData.closeDate);
        MineFusionUI.SetFusionVariable();
        MineDecompositionUI.SetDecompositionVariable();
        ReinforceItemUse.SetReinforceVariable();
        GameFuction.SettingGold();
        auto_animator.SetBool("isSave", true);

        privateParam = new Param();
        publicParam = new Param();
        SettingPrivateParam(privateParam);
        SettingPublicParam(publicParam);

        Backend.GameData.Update("privateData", new Where(), privateParam, (callback) => 
        {
            auto_animator.SetBool("isSave", false);
            if (!callback.IsSuccess())
            {
                Debug.LogError("privateData 서버 저장 <error> = " + callback.GetErrorCode());
            }
        });

        Backend.GameData.Update("publicData", new Where(), publicParam, (callback) => 
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("publicData 서버 저장 <error> = " + callback.GetErrorCode());
            }
        });

        // 랭킹 업데이트
        param = new Param();
        param2 = new Param();
        param3 = new Param();
        param4 = new Param();
        long gold = 0, gold2 = 0, gold3 = 0, gold4 = 0;

        if (saveData.gold4 > 0) gold4 = saveData.gold4;
        else if (saveData.gold3 > 0) gold3 = saveData.gold3;
        else if (saveData.gold2 > 0) gold2 = saveData.gold2;
        else gold = saveData.gold;

        if (BackEndLoginManager.userData.nickname != null)
        {
            param4.Add("gold4", gold4);
            Backend.URank.User.UpdateUserScore(rankUuid_gold4, "privateData", saveData.privateData_Indate, param4, (callback) =>
            {
                if (!callback.IsSuccess())
                    Debug.LogError("랭킹 업데이트_gold3 <error> = " + callback.GetErrorCode());
            });

            param3.Add("gold3", gold3);
            Backend.URank.User.UpdateUserScore(rankUuid_gold3, "privateData", saveData.privateData_Indate, param3, (callback) =>
            {
                if (!callback.IsSuccess())
                    Debug.LogError("랭킹 업데이트_gold3 <error> = " + callback.GetErrorCode());
            });

            param2.Add("gold2", gold2);
            Backend.URank.User.UpdateUserScore(rankUuid_gold2, "privateData", saveData.privateData_Indate, param2, (callback) =>
            {
                if (!callback.IsSuccess())
                    Debug.LogError("랭킹 업데이트_gold2 <error> = " + callback.GetErrorCode());
            });

            param.Add("gold", gold);
            Backend.URank.User.UpdateUserScore(rankUuid_gold, "privateData", saveData.privateData_Indate, param, (callback2) =>
            {
                if (!callback2.IsSuccess())
                    Debug.LogError("랭킹 업데이트_gold <error> = " + callback2.GetErrorCode());
            });
        }
        else
        {
            Debug.Log("랭킹 업데이트 <error> = 닉네임 존재 안함 ");
        }
    }

    static private void PrivateDataParsing(JsonData json)
    {
        saveData.gold = long.Parse(json["gold"][0].ToString());
        saveData.exp = long.Parse(json["exp"][0].ToString());
        saveData.ticketNum = int.Parse(json["ticketNum"][0].ToString());
        saveData.ticketTime = int.Parse(json["ticketTime"][0].ToString());
        saveData.rulletTime = int.Parse(json["rulletTime"][0].ToString());
        saveData.closeDate = json["closeDate"][0].ToString();
        saveData.isReviewOn = bool.Parse(json["isReviewOn"][0].ToString());
        saveData.isTutorial = bool.Parse(json["isTutorial"][0].ToString());
        saveData.isBGMOn = bool.Parse(json["isBGMOn"][0].ToString());
        saveData.isSEOn = bool.Parse(json["isSEOn"][0].ToString());
        saveData.isBlockSoundOn = bool.Parse(json["isBlockSoundOn"][0].ToString());
        saveData.isEndGame = bool.Parse(json["isEndGame"][0].ToString());
        for (int i = 0; i < json["jems"]["L"].Count; i++)
            saveData.hasItemNums[i] = long.Parse(json["jems"]["L"][i][0].ToString());
        saveData.hasReinforceOre = long.Parse(json["reinforceOre"][0].ToString());
        for (int i = 0; i < json["hasPicks"]["L"].Count; i++)
            saveData.hasPicks[i] = bool.Parse(json["hasPicks"]["L"][i][0].ToString());
        for (int i = 0; i < json["hasHats"]["L"].Count; i++)
            saveData.hasHats[i] = bool.Parse(json["hasHats"]["L"][i][0].ToString());
        for (int i = 0; i < json["hasRings"]["L"].Count; i++)
            saveData.hasRings[i] = bool.Parse(json["hasRings"]["L"][i][0].ToString());
        for (int i = 0; i < json["hasPendants"]["L"].Count; i++)
            saveData.hasPenants[i] = bool.Parse(json["hasPendants"]["L"][i][0].ToString());
        saveData.pick1Upgrades = int.Parse(json["pick1Upgrades"][0].ToString());
        saveData.pick2Upgrades = int.Parse(json["pick2Upgrades"][0].ToString());
        saveData.hat1Upgrades = int.Parse(json["hat1Upgrades"][0].ToString());
        saveData.hat2Upgrades = int.Parse(json["hat2Upgrades"][0].ToString());
        saveData.ring1Upgrades = int.Parse(json["ring1Upgrades"][0].ToString());
        saveData.ring2Upgrades = int.Parse(json["ring2Upgrades"][0].ToString());
        saveData.Pendant1Upgrades = int.Parse(json["pendant1Upgrades"][0].ToString());
        saveData.Pendant2Upgrades = int.Parse(json["pendant2Upgrades"][0].ToString());
        for (int i = 0; i < json["hasIcons"]["L"].Count; i++)
            saveData.hasIcons[i] = bool.Parse(json["hasIcons"]["L"][i][0].ToString());
        for (int i = 0; i < json["iconsTime"]["L"].Count; i++)
            saveData.iconsTime[i] = int.Parse(json["iconsTime"]["L"][i][0].ToString());
        for (int i = 0; i < json["isPickReinforceDestroy"]["L"].Count; i++)
            saveData.isPickReinforceDestroy[i] = bool.Parse(json["isPickReinforceDestroy"]["L"][i][0].ToString());
        for (int i = 0; i < json["isHatReinforceDestroy"]["L"].Count; i++)
            saveData.isHatReinforceDestroy[i] = bool.Parse(json["isHatReinforceDestroy"]["L"][i][0].ToString());
        for (int i = 0; i < json["isRingReinforceDestroy"]["L"].Count; i++)
            saveData.isRingReinforceDestroy[i] = bool.Parse(json["isRingReinforceDestroy"]["L"][i][0].ToString());
        for (int i = 0; i < json["isPendantReinforceDestroy"]["L"].Count; i++)
            saveData.isPendantReinforceDestroy[i] = bool.Parse(json["isPendantReinforceDestroy"]["L"][i][0].ToString());
        for (int i = 0; i < json["quastLevels"]["L"].Count; i++)
            saveData.quastLevels[i] = int.Parse(json["quastLevels"]["L"][i][0].ToString());
        for (int i = 0; i < json["quastLists"]["L"].Count; i++)
            saveData.quastLists[i] = int.Parse(json["quastLists"]["L"][i][0].ToString());
        for (int i = 0; i < json["quastGoals"]["L"].Count; i++)
            saveData.quastGoals[i] = int.Parse(json["quastGoals"]["L"][i][0].ToString());
        for (int i = 0; i < json["quastSuccesses"]["L"].Count; i++)
            saveData.quastSuccesses[i] = bool.Parse(json["quastSuccesses"]["L"][i][0].ToString());
        saveData.pickLevel = int.Parse(json["pickLevel"][0].ToString());
        for (int i = 0; i < json["bufItemTimes"]["L"].Count; i++)
            saveData.bufItemTimes[i] = int.Parse(json["bufItemTimes"]["L"][i][0].ToString());
        for (int i = 0; i < json["isBufItemOns"]["L"].Count; i++)
            saveData.isBufItemOns[i] = bool.Parse(json["isBufItemOns"]["L"][i][0].ToString());
        for (int i = 0; i < json["bufItems"]["L"].Count; i++)
            saveData.hasBufItems[i] = long.Parse(json["bufItems"]["L"][i][0].ToString());
        for (int i = 0; i < json["reinforceItems"]["L"].Count; i++)
            saveData.hasReinforceItems[i] = long.Parse(json["reinforceItems"]["L"][i][0].ToString());

        // Update 1.1.8 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("sword1Upgrades"))
        {
            for (int i = 0; i < json["isSwordReinforceDestroy"]["L"].Count; i++)
                saveData.isSwordReinforceDestroy[i] = bool.Parse(json["isSwordReinforceDestroy"]["L"][i][0].ToString());
            saveData.sword1Upgrades = int.Parse(json["sword1Upgrades"][0].ToString());
            saveData.sword2Upgrades = int.Parse(json["sword2Upgrades"][0].ToString());
            for (int i = 0; i < json["hasSwords"]["L"].Count; i++)
                saveData.hasSwords[i] = bool.Parse(json["hasSwords"]["L"][i][0].ToString());
        }

        // Update 1.2.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("hasOnMiners"))
        {
            for (int i = 0; i < json["hasOnMiners"]["L"].Count; i++)
                saveData.hasOnMiners[i] = int.Parse(json["hasOnMiners"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasOnAdventurers"]["L"].Count; i++)
                saveData.hasOnAdventurers[i] = int.Parse(json["hasOnAdventurers"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasOnMinerLevels"]["L"].Count; i++)
                saveData.hasOnMinerLevels[i] = int.Parse(json["hasOnMinerLevels"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasOnAdventurerLevels"]["L"].Count; i++)
                saveData.hasOnAdventurerLevels[i] = int.Parse(json["hasOnAdventurerLevels"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasOnMinerExps"]["L"].Count; i++)
                saveData.hasOnMinerExps[i] = long.Parse(json["hasOnMinerExps"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasOnAdventurerExps"]["L"].Count; i++)
                saveData.hasOnAdventurerExps[i] = long.Parse(json["hasOnAdventurerExps"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasMiners"]["L"].Count; i++)
                saveData.hasMiners[i] = int.Parse(json["hasMiners"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasAdventurers"]["L"].Count; i++)
                saveData.hasAdventurers[i] = int.Parse(json["hasAdventurers"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasMinerLevels"]["L"].Count; i++)
                saveData.hasMinerLevels[i] = int.Parse(json["hasMinerLevels"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasAdventurerLevels"]["L"].Count; i++)
                saveData.hasAdventurerLevels[i] = int.Parse(json["hasAdventurerLevels"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasMinerExps"]["L"].Count; i++)
                saveData.hasMinerExps[i] = long.Parse(json["hasMinerExps"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasAdventurerExps"]["L"].Count; i++)
                saveData.hasAdventurerExps[i] = long.Parse(json["hasAdventurerExps"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasMinerRewards"]["L"].Count; i++)
                saveData.hasMinerRewards[i] = int.Parse(json["hasMinerRewards"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasAdventurerRewards"]["L"].Count; i++)
                saveData.hasAdventurerRewards[i] = int.Parse(json["hasAdventurerRewards"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasMinerTimes"]["L"].Count; i++)
                saveData.hasMinerTimes[i] = int.Parse(json["hasMinerTimes"]["L"][i][0].ToString());
            for (int i = 0; i < json["hasAdventurerTimes"]["L"].Count; i++)
                saveData.hasAdventurerTimes[i] = int.Parse(json["hasAdventurerTimes"]["L"][i][0].ToString());
            for (int i = 0; i < json["minerUpgrades"]["L"].Count; i++)
                saveData.minerUpgrades[i] = int.Parse(json["minerUpgrades"]["L"][i][0].ToString());
            for (int i = 0; i < json["adventurerUpgrades"]["L"].Count; i++)
                saveData.adventurerUpgrades[i] = int.Parse(json["adventurerUpgrades"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.2.0 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.2.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("cash"))
        {
            saveData.cash = int.Parse(json["cash"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.2.2 데이터 테이블 업데이트");
            isDataChange = true;
        }
        
        // Update 1.2.3 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("achievementAmounts"))
        {
            for (int i = 0; i < json["achievementAmounts"]["L"].Count; i++)
                saveData.achievementAmounts[i] = long.Parse(json["achievementAmounts"]["L"][i][0].ToString());
            for (int i = 0; i < json["achievementLevels"]["L"].Count; i++)
                saveData.achievementLevels[i] = int.Parse(json["achievementLevels"]["L"][i][0].ToString());

            if(!json.ContainsKey("achievementAmounts"))
            {
                isDataChange = true;
                return;
            }
        }
        else
        {
            Debug.Log("Update 1.2.3 데이터 테이블 업데이트");
            isDataChange = true;

            // 데이터 조정
            Update123();
            if (!saveData.isTutorial)
            {
                saveData.gold += 110000;
                saveData.exp += 300;
                saveData.hasReinforceOre += 110;
            }
        }

        // Update 1.3.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("gold2"))
        {
            saveData.gold2 = long.Parse(json["gold2"][0].ToString());
            saveData.manaOre = long.Parse(json["manaOre"][0].ToString());
            saveData.facility_level = int.Parse(json["facility_level"][0].ToString());
            saveData.facility_rewardTime = int.Parse(json["facility_rewardTime"][0].ToString());
            for (int i = 0; i < json["manaBufTimes"]["L"].Count; i++)
                saveData.manaBufTimes[i] = int.Parse(json["manaBufTimes"]["L"][i][0].ToString());
            for (int i = 0; i < json["isManaBuffOns"]["L"].Count; i++)
                saveData.isManaBuffOns[i] = bool.Parse(json["isManaBuffOns"]["L"][i][0].ToString());
            for (int i = 0; i < json["achievementAmount2"]["L"].Count; i++)
                saveData.achievementAmount2[i] = long.Parse(json["achievementAmount2"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.3.0 데이터 테이블 업데이트");
            isDataChange = true;

            // 데이터 조정
            GameFuction.SettingGold();
            if (saveData.achievementAmounts[24] >= GameFuction.GOLD_UNIT)
            {
                saveData.achievementAmount2[1] = saveData.achievementAmounts[24] / GameFuction.GOLD_UNIT;
                saveData.achievementAmounts[24] = saveData.achievementAmounts[24] % GameFuction.GOLD_UNIT;
            }
        }

        // Update 1.3.6 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("manaUpgrades"))
        {
            for (int i = 0; i < json["manaUpgrades"]["L"].Count; i++)
                saveData.manaUpgrades[i] = int.Parse(json["manaUpgrades"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.3.6 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.3.9 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("isRankerChat"))
        {
            saveData.isRankerChat = bool.Parse(json["isRankerChat"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.3.9 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.4.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("attendance_count"))
        {
            saveData.attendance_count = int.Parse(json["attendance_count"][0].ToString());
            saveData.attendance_day = int.Parse(json["attendance_day"][0].ToString());
            saveData.attendance_month = int.Parse(json["attendance_month"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.4.0 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.4.3 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("collection_cards"))
        {
            for (int i = 0; i < json["collection_cards"]["L"].Count; i++)
                saveData.collection_cards[i] = int.Parse(json["collection_cards"]["L"][i][0].ToString());
            for (int i = 0; i < json["collection_levels"]["L"].Count; i++)
                saveData.collection_levels[i] = int.Parse(json["collection_levels"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.4.3 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.4.7 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("isAutoUse_1"))
        {
            saveData.isAutoUse_1 = bool.Parse(json["isAutoUse_1"][0].ToString());
            saveData.isAutoUse_2 = bool.Parse(json["isAutoUse_2"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.4.7 데이터 테이블 업데이트");
            Update147();
            isDataChange = true;
        }

        // Update 1.4.8 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("hasElixirs"))
        {
            for (int i = 0; i < json["hasElixirs"]["L"].Count; i++)
                saveData.hasElixirs[i] = long.Parse(json["hasElixirs"]["L"][i][0].ToString());
            for (int i = 0; i < json["elixirTimes"]["L"].Count; i++)
                saveData.elixirTimes[i] = int.Parse(json["elixirTimes"]["L"][i][0].ToString());
            for (int i = 0; i < json["isElixirOns"]["L"].Count; i++)
                saveData.isElixirOns[i] = bool.Parse(json["isElixirOns"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.4.8 데이터 테이블 업데이트");
            Update148();
            isDataChange = true;
        }

        // Update 1.5.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("isGodChat"))
        {
            saveData.gold3 = long.Parse(json["gold3"][0].ToString());
            saveData.isGodChat = bool.Parse(json["isGodChat"][0].ToString());
            for (int i = 0; i < json["hasReinforceItems2"]["L"].Count; i++)
                saveData.hasReinforceItems2[i] = long.Parse(json["hasReinforceItems2"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.0 데이터 테이블 업데이트");
            Update150();
            isDataChange = true;
        }

        // Update 1.5.44 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("fpsType"))
        {
            saveData.fpsType = int.Parse(json["fpsType"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.44 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.5.5 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("mainQuest_list"))
        {
            saveData.mainQuest_list = int.Parse(json["mainQuest_list"][0].ToString());
            saveData.mainQuest_goal = int.Parse(json["mainQuest_goal"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.5 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.5.6 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("hasCashEquipment"))
        {
            for (int i = 0; i < json["hasCashEquipment"]["L"].Count; i++)
                saveData.hasCashEquipment[i] = bool.Parse(json["hasCashEquipment"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.6 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.5.7 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("isGetBonus"))
        {
            saveData.isGetBonus = bool.Parse(json["isGetBonus"][0].ToString());
            for (int i = 0; i < json["isCashEquipmentOn"]["L"].Count; i++)
                saveData.isCashEquipmentOn[i] = bool.Parse(json["isCashEquipmentOn"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.7 데이터 테이블 업데이트");
            saveData.isGetBonus = false;
            for (int i = 0; i < saveData.hasCashEquipment.Length; i++)
                saveData.isCashEquipmentOn[i] = saveData.hasCashEquipment[i];
            isDataChange = true;
        }

        // Update 1.5.9 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("achievementAmount3"))
        {
            for (int i = 0; i < json["achievementAmount3"]["L"].Count; i++)
                saveData.achievementAmount3[i] = long.Parse(json["achievementAmount3"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.9 데이터 테이블 업데이트");
            Update159();
            isDataChange = true;
        }

        // Update 1.6.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("growthOreNum"))
        {
            saveData.growthOreNum = long.Parse(json["growthOreNum"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.6.0 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.6.2 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("vendingMachineTime"))
        {
            saveData.vendingMachineTime = int.Parse(json["vendingMachineTime"][0].ToString());
            saveData.mailboxTime = int.Parse(json["mailboxTime"][0].ToString());
            for (int i = 0; i < json["mailboxes"]["L"].Count; i++)
                saveData.mailboxes[i] = int.Parse(json["mailboxes"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.6.2 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.6.4 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("combo_gauge"))
        {
            saveData.combo_gauge = int.Parse(json["combo_gauge"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.6.4 데이터 테이블 업데이트");
            MainBonusUI.instance.CheckBonus();
            isDataChange = true;
        }

        // Update 1.6.5 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("gold4"))
        {
            saveData.gold4 = long.Parse(json["gold4"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.0 데이터 테이블 업데이트");
            isDataChange = true;
        }
    }

    static private void PublicDataParsing(JsonData json)
    {
        saveData.gold = long.Parse(json["gold"][0].ToString());
        saveData.equipPick = int.Parse(json["equipPick"][0].ToString());
        saveData.equipHat = int.Parse(json["equipHat"][0].ToString());
        saveData.equipRing = int.Parse(json["equipRing"][0].ToString());
        saveData.equipPendant = int.Parse(json["equipPendant"][0].ToString());
        saveData.user_Indate = json["user_Indate"][0].ToString();
        saveData.privateData_Indate = json["privateData_Indate"][0].ToString();
        saveData.publicData_Indate = json["publicData_Indate"][0].ToString();
        for (int i = 0; i < json["pickReinforces"]["L"].Count; i++)
            saveData.pickReinforces[i] = int.Parse(json["pickReinforces"]["L"][i][0].ToString());
        for (int i = 0; i < json["hatReinforces"]["L"].Count; i++)
            saveData.hatReinforces[i] = int.Parse(json["hatReinforces"]["L"][i][0].ToString());
        for (int i = 0; i < json["ringReinforces"]["L"].Count; i++)
            saveData.ringReinforces[i] = int.Parse(json["ringReinforces"]["L"][i][0].ToString());
        for (int i = 0; i < json["pendantReinforces"]["L"].Count; i++)
            saveData.pendantReinforces[i] = int.Parse(json["pendantReinforces"]["L"][i][0].ToString());

        // Update 1.1.8 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("equipSword"))
        {
            saveData.equipSword = int.Parse(json["equipSword"][0].ToString());
            for (int i = 0; i < json["swordReinforces"]["L"].Count; i++)
                saveData.swordReinforces[i] = int.Parse(json["swordReinforces"]["L"][i][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.1.8 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.2.3 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("tier_achievement"))
        {
            saveData.tier_level = int.Parse(json["tier_level"][0].ToString());
            saveData.tier_achievement = int.Parse(json["tier_achievement"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.2.3 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.3.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("gold2"))
        {
            saveData.gold2 = long.Parse(json["gold2"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.3.0 데이터 테이블 업데이트");
            isDataChange = true;
        }

        // Update 1.5.0 이후 데이터 테이블 변화 감지
        if (json.ContainsKey("gold3"))
        {
            saveData.gold3 = long.Parse(json["gold3"][0].ToString());
        }
        else
        {
            Debug.Log("Update 1.5.0 데이터 테이블 업데이트");
            isDataChange = true;
        }
    }

    static public void SettingPrivateParam(Param param)
    {
        param.Add("gold", saveData.gold);
        param.Add("exp", saveData.exp);
        param.Add("cash", saveData.cash);
        param.Add("ticketNum", saveData.ticketNum);
        param.Add("ticketTime", saveData.ticketTime);
        param.Add("rulletTime", saveData.rulletTime);
        param.Add("closeDate", saveData.closeDate);
        param.Add("isReviewOn", saveData.isReviewOn);
        param.Add("isTutorial", saveData.isTutorial);
        param.Add("isBGMOn", saveData.isBGMOn);
        param.Add("isSEOn", saveData.isSEOn);
        param.Add("isBlockSoundOn", saveData.isBlockSoundOn);
        param.Add("isEndGame", saveData.isEndGame);
        param.Add("jems", saveData.hasItemNums);
        param.Add("reinforceOre", saveData.hasReinforceOre);
        param.Add("hasPicks", saveData.hasPicks);
        param.Add("hasHats", saveData.hasHats);
        param.Add("hasRings", saveData.hasRings);
        param.Add("hasPendants", saveData.hasPenants);
        param.Add("pick1Upgrades", saveData.pick1Upgrades);
        param.Add("pick2Upgrades", saveData.pick2Upgrades);
        param.Add("hat1Upgrades", saveData.hat1Upgrades);
        param.Add("hat2Upgrades", saveData.hat2Upgrades);
        param.Add("ring1Upgrades", saveData.ring1Upgrades);
        param.Add("ring2Upgrades", saveData.ring2Upgrades);
        param.Add("pendant1Upgrades", saveData.Pendant1Upgrades);
        param.Add("pendant2Upgrades", saveData.Pendant2Upgrades);
        param.Add("hasIcons", saveData.hasIcons);
        param.Add("iconsTime", saveData.iconsTime);
        param.Add("isPickReinforceDestroy", saveData.isPickReinforceDestroy);
        param.Add("isHatReinforceDestroy", saveData.isHatReinforceDestroy);
        param.Add("isRingReinforceDestroy", saveData.isRingReinforceDestroy);
        param.Add("isPendantReinforceDestroy", saveData.isPendantReinforceDestroy);
        param.Add("quastLevels", saveData.quastLevels);
        param.Add("quastLists", saveData.quastLists);
        param.Add("quastGoals", saveData.quastGoals);
        param.Add("quastSuccesses", saveData.quastSuccesses);
        param.Add("pickLevel", saveData.pickLevel);
        param.Add("bufItemTimes", saveData.bufItemTimes);
        param.Add("isBufItemOns", saveData.isBufItemOns);
        param.Add("bufItems", saveData.hasBufItems);
        param.Add("reinforceItems", saveData.hasReinforceItems);

        param.Add("sword1Upgrades", saveData.sword1Upgrades);
        param.Add("sword2Upgrades", saveData.sword2Upgrades);
        param.Add("isSwordReinforceDestroy", saveData.isSwordReinforceDestroy);
        param.Add("hasSwords", saveData.hasSwords);

        param.Add("hasOnMiners", saveData.hasOnMiners);
        param.Add("hasOnAdventurers", saveData.hasOnAdventurers);
        param.Add("hasOnMinerLevels", saveData.hasOnMinerLevels);
        param.Add("hasOnAdventurerLevels", saveData.hasOnAdventurerLevels);
        param.Add("hasOnMinerExps", saveData.hasOnMinerExps);
        param.Add("hasOnAdventurerExps", saveData.hasOnAdventurerExps);
        param.Add("hasMiners", saveData.hasMiners);
        param.Add("hasAdventurers", saveData.hasAdventurers);
        param.Add("hasMinerLevels", saveData.hasMinerLevels);
        param.Add("hasAdventurerLevels", saveData.hasAdventurerLevels);
        param.Add("hasMinerExps", saveData.hasMinerExps);
        param.Add("hasAdventurerExps", saveData.hasAdventurerExps);
        param.Add("hasMinerRewards", saveData.hasMinerRewards);
        param.Add("hasAdventurerRewards", saveData.hasAdventurerRewards);
        param.Add("hasMinerTimes", saveData.hasMinerTimes);
        param.Add("hasAdventurerTimes", saveData.hasAdventurerTimes);
        param.Add("minerUpgrades", saveData.minerUpgrades);
        param.Add("adventurerUpgrades", saveData.adventurerUpgrades);

        param.Add("achievementAmounts", saveData.achievementAmounts);
        param.Add("achievementLevels", saveData.achievementLevels);

        param.Add("gold2", saveData.gold2);
        param.Add("manaOre", saveData.manaOre);
        param.Add("facility_level", saveData.facility_level);
        param.Add("facility_rewardTime", saveData.facility_rewardTime);
        param.Add("isManaBuffOns", saveData.isManaBuffOns);
        param.Add("manaBufTimes", saveData.manaBufTimes);
        param.Add("achievementAmount2", saveData.achievementAmount2);

        param.Add("manaUpgrades", saveData.manaUpgrades);

        param.Add("isRankerChat", saveData.isRankerChat);

        param.Add("attendance_count", saveData.attendance_count);
        param.Add("attendance_day", saveData.attendance_day);
        param.Add("attendance_month", saveData.attendance_month);

        param.Add("collection_cards", saveData.collection_cards);
        param.Add("collection_levels", saveData.collection_levels);

        param.Add("isAutoUse_1", saveData.isAutoUse_1);
        param.Add("isAutoUse_2", saveData.isAutoUse_2);

        param.Add("hasElixirs", saveData.hasElixirs);
        param.Add("elixirTimes", saveData.elixirTimes);
        param.Add("isElixirOns", saveData.isElixirOns);

        param.Add("gold3", saveData.gold3);
        param.Add("isGodChat", saveData.isGodChat);
        param.Add("hasReinforceItems2", saveData.hasReinforceItems2);

        param.Add("fpsType", saveData.fpsType);

        param.Add("mainQuest_list", saveData.mainQuest_list);
        param.Add("mainQuest_goal", saveData.mainQuest_goal);

        param.Add("hasCashEquipment", saveData.hasCashEquipment);

        param.Add("isGetBonus", saveData.isGetBonus);
        param.Add("isCashEquipmentOn", saveData.isCashEquipmentOn);

        param.Add("achievementAmount3", saveData.achievementAmount3);

        param.Add("growthOreNum", saveData.growthOreNum);

        param.Add("vendingMachineTime", saveData.vendingMachineTime);
        param.Add("mailboxTime", saveData.mailboxTime);
        param.Add("mailboxes", saveData.mailboxes);

        param.Add("combo_gauge", saveData.combo_gauge);

        param.Add("gold4", saveData.gold4);
    }

    static public void SettingPublicParam(Param param)
    {
        param.Add("gold", saveData.gold);
        param.Add("equipPick", saveData.equipPick);
        param.Add("equipHat", saveData.equipHat);
        param.Add("equipRing", saveData.equipRing);
        param.Add("equipPendant", saveData.equipPendant);
        param.Add("user_Indate", saveData.user_Indate);
        param.Add("privateData_Indate", saveData.privateData_Indate);
        param.Add("publicData_Indate", saveData.publicData_Indate);
        param.Add("pickReinforces", saveData.pickReinforces);
        param.Add("hatReinforces", saveData.hatReinforces);
        param.Add("ringReinforces", saveData.ringReinforces);
        param.Add("pendantReinforces", saveData.pendantReinforces);

        param.Add("equipSword", saveData.equipSword);
        param.Add("swordReinforces", saveData.swordReinforces);

        param.Add("tier_level", saveData.tier_level);
        param.Add("tier_achievement", saveData.tier_achievement);

        param.Add("gold2", saveData.gold2);

        param.Add("gold3", saveData.gold3);

        param.Add("gold4", saveData.gold4);
    }

    IEnumerator SetTime()
    {
        isSetTime = true;
        playTime++;

        dateTime = dateTime.AddSeconds(1f);
        GameFuction.SetComboData();
        GameFuction.SetAttendanceData();
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            MainAttendance.instance.SetCanAttendanceInfo();
            MainComboUI.instance.SetMainUI();
        }
        else if (SceneManager.GetActiveScene().name == "GameScene")
        {
            MainComboUI.instance.SetMainUI();
        }

        // 우편 설정
        saveData.mailboxTime -= 1;
        if (saveData.mailboxTime <= 0f)
        {
            // 마나석 우편
            GameFuction.PushMail(GameFuction.GetMailValue(0));
            saveData.mailboxTime = mailboxTime;
            if (SceneManager.GetActiveScene().name == "MainScene")
                MainMailboxUI.instance.SetMailboxButton();
        }

        if (dateTime.Day != dateTime.AddSeconds(-1f).Day)
        {
            // 레드 다이아 우편
            GameFuction.PushMail(GameFuction.GetMailValue(1));
            if (SceneManager.GetActiveScene().name == "MainScene")
                MainMailboxUI.instance.SetMailboxButton();
        }

        // 티켓 타임 설정
        if (saveData.ticketNum < 5)
        {
            MainQuestUI.isTicketTimeSet = true;

            if (saveData.ticketNum < ticketNum)
            {
                saveData.ticketTime -= 1;
                if (saveData.ticketTime <= 0f)
                {
                    saveData.ticketNum++;
                    saveData.ticketTime = ticketTime;
                }
            }
            else
            {
                saveData.ticketTime = 0;
            }
        }

        // 룰렛 타임 설정
        MainRulletUI.isRulletTimeSet = true;
        if (saveData.rulletTime > 0 && saveData.rulletTime <= rulletTime)
            saveData.rulletTime -= 1;

        // 뽑기 타임 설정
        MainVendingMachine.isVendingMachineTimeSet = true;
        if (saveData.vendingMachineTime > 0 && saveData.vendingMachineTime <= vendingMachineTime)
            saveData.vendingMachineTime -= 1;

        // 아이콘 타임 설정
        IconInfoUI.isIconTimeSet = true;
        PlayerScript.isIconTimeSet = true;

        // 룰렛 아이콘
        for (int i = 0; i < iconNum; i++)
        {
            if (saveData.hasIcons[i])
            {
                saveData.iconsTime[i] -= 1;
                if(saveData.iconsTime[i] <= 0)
                {
                    saveData.hasIcons[i] = false;
                    isChangedIcons = true;
                }
            }
        }

        // 마나석 버프 아이콘
        for (int i = 0; i < manaBufNum; i++)
        {
            if (saveData.isManaBuffOns[i])
            {
                saveData.manaBufTimes[i] -= 1;
                if (saveData.manaBufTimes[i] <= 0)
                {
                    saveData.isManaBuffOns[i] = false;
                    isChangedIcons = true;
                }
            }
        }

        // 영약 버프 아이콘
        for (int i = 0; i < bufItemCodeNum; i++)
        {
            if (saveData.isElixirOns[i])
            {
                saveData.elixirTimes[i] -= 1;
                if (saveData.elixirTimes[i] <= 0)
                {
                    saveData.isElixirOns[i] = false;
                    isChangedIcons = true;

                    if (i >= 8 && i <= 9)
                    {
                        PlayerScript player = FindObjectOfType<PlayerScript>();

                        if (player != null)
                        {
                            if (i == 8)
                                player.moveSpeed = player.moveSpeedData;
                            else
                                player.jumpPower = player.jumpPowerData;
                        }
                    }
                }
            }
        }

        // 버프 아이템 아이콘
        for (int i = 0; i < bufItemNum; i++)
        {
            if (saveData.isBufItemOns[i])
            {
                saveData.bufItemTimes[i] -= 1;
                if (saveData.bufItemTimes[i] <= 0)
                {
                    saveData.isBufItemOns[i] = false;
                    isChangedIcons = true;

                    if(i >= 24 && i <= 29)
                    {
                        PlayerScript player = FindObjectOfType<PlayerScript>();

                        if(player != null)
                        {
                            if (i >= 24 && i <= 26)
                                player.moveSpeed = player.moveSpeedData;
                            else
                                player.jumpPower = player.jumpPowerData;
                        }
                    }
                }
            }
        }

        // 광산 설정
        MineFacilityUI.isRewardTimeSet = true;
        if(saveData.facility_rewardTime > 0) 
            saveData.facility_rewardTime--;

        MineMap.isSetMineTime = true;
        MainScript.isSetMineTime = true;
        for (int i = 0; i < saveData.hasOnMiners.Length; i++)
        {
            if(saveData.hasOnMiners[i] != -1)
            {
                saveData.hasMinerTimes[i] -= 1;
                if(saveData.hasMinerTimes[i] <= 0)
                {
                    saveData.hasMinerTimes[i] = MinerSlime.GetTimeAsLevel(saveData.hasOnMiners[i], saveData.hasOnMinerLevels[i]);
                    if (saveData.hasMinerRewards[i] < maxPetReward)
                    {
                        saveData.hasMinerRewards[i]++;
                        MineMap.rewardMinerIndexs.Add(i);
                    }
                }
            }
        }

        for (int i = 0; i < saveData.hasOnAdventurers.Length; i++)
        {
            if (saveData.hasOnAdventurers[i] != -1)
            {
                saveData.hasAdventurerTimes[i] -= 1;
                if (saveData.hasAdventurerTimes[i] <= 0)
                {
                    saveData.hasAdventurerTimes[i] = AdventurerSlime.GetTimeAsLevel(saveData.hasOnAdventurers[i], saveData.hasOnAdventurerLevels[i]);
                    if (saveData.hasAdventurerRewards[i] < maxPetReward)
                    {
                        saveData.hasAdventurerRewards[i]++;
                        MineMap.rewardAdventurerIndexs.Add(i);
                    }
                }
            }
        }

        yield return new WaitForSeconds(1f);

        isSetTime = false;
    }

    static public void Setquest()
    {
        int[] lists = GameFuction.GetRandomDatas(0, questNums[SubQuest.GetSubQuestType(saveData.pickLevel)], hasQuestNum);

        for (int i = 0; i < hasQuestNum; i++)
        {
            saveData.quastGoals[i] = 0;
            saveData.quastSuccesses[i] = false;
            saveData.quastLists[i] = lists[i];
        }

        for (int i = 0; i < saveData.quastLevels.Length; i++)
            saveData.quastLevels[i] = saveData.pickLevel;
    }
           
    public void CheckGold()
    {
        if(saveData.gold4 > GameFuction.GOLD_END)
        {
            saveData.gold4 = GameFuction.GOLD_END;
            endCanvas.enabled = true;
            saveData.isEndGame = true;
            SaveData_Asyn(true);
        }
    }

    /// <summary>
    /// 1.2.3 업데이터로 인한 데이터 조정
    /// </summary>
    static public void Update123()
    {
        // 강화석 강화 조정
        for (int i = 0; i < saveData.pickReinforces.Length; i++)
            saveData.achievementAmounts[23] += saveData.pickReinforces[i];
        for (int i = 0; i < saveData.hatReinforces.Length; i++)
            saveData.achievementAmounts[23] += saveData.hatReinforces[i];
        for (int i = 0; i < saveData.ringReinforces.Length; i++)
            saveData.achievementAmounts[23] += saveData.ringReinforces[i];
        for (int i = 0; i < saveData.pendantReinforces.Length; i++)
            saveData.achievementAmounts[23] += saveData.pendantReinforces[i];
        for (int i = 0; i < saveData.swordReinforces.Length; i++)
            saveData.achievementAmounts[23] += saveData.swordReinforces[i];

        // 경험치 강화 조정
        void TempCal(ref int data, int k)
        {
            int sum = 0;
            int temp = 0;

            for (int i = 0; i < data * k; i++)
            {
                if (i == 0) temp = 100;
                else temp = (i * 100 + temp);
                if ((i % k) == 0) sum += temp;
            }
            saveData.exp += sum;
            data = 0;
        }

        TempCal(ref saveData.pick1Upgrades, 1);
        TempCal(ref saveData.pick2Upgrades, 1);
        TempCal(ref saveData.hat1Upgrades, 1);
        TempCal(ref saveData.hat2Upgrades, 1);
        TempCal(ref saveData.ring1Upgrades, 1);
        TempCal(ref saveData.ring2Upgrades, 1);
        TempCal(ref saveData.Pendant1Upgrades, 2);
        TempCal(ref saveData.Pendant2Upgrades, 1);
        TempCal(ref saveData.sword1Upgrades, 1);
        TempCal(ref saveData.sword2Upgrades, 1);
        TempCal(ref saveData.minerUpgrades[0], 1);
        TempCal(ref saveData.minerUpgrades[1], 1);
        TempCal(ref saveData.minerUpgrades[2], 2);
        TempCal(ref saveData.minerUpgrades[3], 3);
        TempCal(ref saveData.adventurerUpgrades[0], 1);
        TempCal(ref saveData.adventurerUpgrades[1], 1);
        TempCal(ref saveData.adventurerUpgrades[2], 2);
        TempCal(ref saveData.adventurerUpgrades[3], 3);
    }

    static public void Update147()
    {
        int[] oldValues = (int[])saveData.manaUpgrades.Clone();
        saveData.manaUpgrades[0] = oldValues[2];
        saveData.manaUpgrades[1] = oldValues[3];
        saveData.manaUpgrades[2] = oldValues[6];
        saveData.manaUpgrades[5] = oldValues[0];
        saveData.manaUpgrades[8] = oldValues[5];
        saveData.manaUpgrades[10] = oldValues[4];
        saveData.manaUpgrades[11] = oldValues[1];
        saveData.manaUpgrades[3] = saveData.manaUpgrades[4] = saveData.manaUpgrades[6] = 0;
    }

    static public void Update148()
    {
        // 강화석 강화 초기화
        int n = 0;
        int reinforceOre = 0;
        int itemNum = 0;

        void GetValues(ref bool[] hasTools, ref int[] reinforces)
        {
            for (int i = 0; i < hasTools.Length; i++)
            {
                if (hasTools[i])
                {
                    n = reinforces[i];
                    reinforces[i] = 0;
                    reinforceOre += (n * (n + 1)) * 5;
                    itemNum += n;
                }
            }
        }

        // 곡괭이
        GetValues(ref saveData.hasPicks, ref saveData.pickReinforces);
        GetValues(ref saveData.hasHats, ref saveData.hatReinforces);
        GetValues(ref saveData.hasRings, ref saveData.ringReinforces);
        GetValues(ref saveData.hasPenants, ref saveData.pendantReinforces);
        GetValues(ref saveData.hasSwords, ref saveData.swordReinforces);

        Debug.Log("얻은 강화석 수 : " + reinforceOre + " / 얻은 아이템 수 : " + itemNum);
        saveData.hasReinforceOre += reinforceOre;
        saveData.hasReinforceItems[6] += itemNum;
    }

    static public void Update150()
    {
        // 채광 펫
        for (int i = 0; i < 4; i++)
        {
            int value = saveData.minerUpgrades[i];
            switch (i)
            {
                case 0:
                case 1: saveData.exp += 100 * value * value; break;
                case 2: saveData.exp += value * (150 * value - 50); break;
                case 3: saveData.exp += value * (350 * value - 250); break;
            }
            saveData.minerUpgrades[i] = 0;
        }

        // 모험가 펫
        for (int i = 0; i < 4; i++)
        {
            int value = saveData.adventurerUpgrades[i];
            switch (i)
            {
                case 0:
                case 1: saveData.exp += 100 * value * value; break;
                case 2: saveData.exp += value * (150 * value - 50); break;
                case 3: saveData.exp += value * (350 * value - 250); break;
            }
            saveData.adventurerUpgrades[i] = 0;
        }
    }

    static public void Update159()
    {
        for (int i = 0; i < saveData.achievementLevels.Length; i++)
            saveData.achievementLevels[i] = 0;
        for (int i = 0; i < saveData.achievementAmounts.Length; i++)
            saveData.achievementAmounts[i] = 0;
        for (int i = 0; i < saveData.achievementAmount2.Length; i++)
            saveData.achievementAmount2[i] = 0;
        saveData.tier_achievement = saveData.tier_level = 0;

        // 경험치 강화
        saveData.achievementAmounts[27] = saveData.pick1Upgrades + saveData.pick2Upgrades + saveData.hat1Upgrades + saveData.hat2Upgrades +
            saveData.ring1Upgrades + saveData.ring2Upgrades + saveData.Pendant1Upgrades + saveData.Pendant2Upgrades + saveData.sword1Upgrades + saveData.sword2Upgrades;
        // 강화석 강화
        for (int i = 0; i < saveData.pickReinforces.Length; i++)
        {
            saveData.achievementAmounts[28] += saveData.pickReinforces[i];
            saveData.achievementAmounts[28] += saveData.hatReinforces[i];
            saveData.achievementAmounts[28] += saveData.ringReinforces[i];
            saveData.achievementAmounts[28] += saveData.pendantReinforces[i];
            saveData.achievementAmounts[28] += saveData.swordReinforces[i];
        }
        // 펫 레벨
        for (int i = 0; i < saveData.hasOnMinerLevels.Length; i++)
        {
            if (saveData.hasOnMinerLevels[i] > 0) saveData.achievementAmounts[36] += saveData.hasOnMinerLevels[i];
            if (saveData.hasMinerLevels[i] > 0) saveData.achievementAmounts[36] += saveData.hasMinerLevels[i];
        }
        for (int i = 0; i < saveData.hasOnAdventurerLevels.Length; i++)
        {
            if (saveData.hasOnAdventurerLevels[i] > 0) saveData.achievementAmounts[36] += saveData.hasOnAdventurerLevels[i];
            if (saveData.hasAdventurerLevels[i] > 0) saveData.achievementAmounts[36] += saveData.hasAdventurerLevels[i];
        }
        // 펫 합성
        for (int i = 0; i < saveData.hasOnMiners.Length; i++)
        {
            if (saveData.hasOnMiners[i] > 0) saveData.achievementAmounts[37] += (int)Mathf.Pow(2, saveData.hasOnMiners[i] - 1);
            if (saveData.hasOnAdventurers[i] > 0) saveData.achievementAmounts[37] += (int)Mathf.Pow(2, saveData.hasOnAdventurers[i] - 1);
        }
        // 펫 강화
        for (int i = 0; i < saveData.minerUpgrades.Length; i++)
            saveData.achievementAmounts[38] += saveData.minerUpgrades[i] + saveData.adventurerUpgrades[i];
    }
}
