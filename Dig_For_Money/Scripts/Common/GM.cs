using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이 클래스는 Game Manager 전용 스크립트이다.
/// </summary>
public class GM 
{
    private static int[] equipment_expUp = { 0, 2, 3, 5, 7, 10, 15, 20, 40, 100, 300, 500, 1000, 3000, 10000 }; // 15 = 7층, 300 = 11층  
    private static int[] equipment_reinUp = { 0, 3, 7, 12, 16, 30, 100, 500, 1000, 3000, 10000, 30000, 50000, 100000, 300000 }; // 100 = 7층, 10000 = 11층, 300000 = 15층
    private static int[] mana_upgrades = { 0, 0, 0, 0, 0, 0, 5, 10, 30, 50, 100, 300, 500, 1000, 5000 };
    private static int[] mana_facility = { 0, 0, 0, 0, 0, 0, 10, 20, 50, 100, 200, 500, 1000, 3000, 10000 };
    private static int[] pet_types = { -1, -1, -1, -1, -1, -1, 2, 3, 4, 6, 7, 8, 9, 9, 10 };
    private static int[] pet_levels = { 0, 0, 0, 0, 0, 0, 5, 10, 30, 50, 100, 300, 500, 1000, 5000, 10000 };
    private static int[] pet_upgrades = { 0, 0, 0, 0, 0, 0, 5, 10, 30, 100, 200, 500, 1000, 5000, 10000 };

    static public SaveData GetSaveDataAsFloor(int _floor)
    {
        SaveData saveData = SaveScript.saveData;

        saveData.isRemoveAD = false;
        saveData.pick1Upgrades = saveData.pick2Upgrades = saveData.hat1Upgrades = saveData.hat2Upgrades =
            saveData.ring1Upgrades = saveData.ring2Upgrades = saveData.Pendant1Upgrades = saveData.Pendant2Upgrades
            = saveData.sword1Upgrades = saveData.sword2Upgrades = equipment_expUp[_floor];
        saveData.pickReinforces = (int[])equipment_reinUp.Clone();
        saveData.hatReinforces = (int[])equipment_reinUp.Clone();
        saveData.ringReinforces = (int[])equipment_reinUp.Clone();
        saveData.pendantReinforces = (int[])equipment_reinUp.Clone();
        saveData.swordReinforces = (int[])equipment_reinUp.Clone();
        saveData.pickLevel = saveData.equipPick = saveData.equipHat = saveData.equipRing = saveData.equipPendant = saveData.equipSword = _floor;

        for (int i = 0; i < SaveScript.stageNum; i++)
        {
            if (i <= saveData.pickLevel)
                saveData.hasPicks[i] = saveData.hasRings[i] = saveData.hasHats[i] = saveData.hasPenants[i] = saveData.hasSwords[i] = true;
            else
                saveData.hasPicks[i] = saveData.hasRings[i] = saveData.hasHats[i] = saveData.hasPenants[i] = saveData.hasSwords[i] = false;
        }

        int totalItemNum = 0;
        for (int i = 0; i <= _floor; i++)
            totalItemNum += SaveScript.stageItemNums[i];
        for (int i = 0; i < saveData.collection_cards.Length; i++)
        {
            if (i < totalItemNum) saveData.collection_cards[i] = saveData.collection_levels[i] = i + 1;
            else saveData.collection_cards[i] = saveData.collection_levels[i] = 0;
        }

        for (int i = 0; i < SaveScript.bufItemNum; i++)
            saveData.isBufItemOns[i] = false;
        for (int i = 0; i < SaveScript.bufItemCodeNum; i++)
            saveData.isElixirOns[i] = false;
        for (int i = 0; i < SaveScript.iconNum; i++)
            saveData.hasIcons[i] = false;

        // 마나 업그레이드 관련
        for (int i = 0; i < SaveScript.manaUpgradeNum; i++)
            saveData.manaUpgrades[i] = mana_upgrades[_floor];
        saveData.facility_level = mana_facility[_floor];

        // 펫 관련
        for (int i = 0; i < 10; i++)
        {
            saveData.hasOnMiners[i] = pet_types[_floor];
            saveData.hasOnMinerLevels[i] = pet_levels[_floor];
            saveData.hasOnMinerExps[i] = 0;

            saveData.hasOnAdventurers[i] = pet_types[_floor];
            saveData.hasOnAdventurerLevels[i] = pet_levels[_floor];
            saveData.hasOnAdventurerExps[i] = 0;

            saveData.hasMiners[i] = saveData.hasAdventurers[i] = -1;
        }

        for (int i = 0; i < 2; i++)
        {
            saveData.minerUpgrades[i] = pet_upgrades[_floor];
            saveData.adventurerUpgrades[i] = pet_upgrades[_floor];
        }

        // 미리 해결될 수 있는 퀘스트들 체크
        QuestCtrl.CheckAllQuest();

        return saveData;
    }
}
