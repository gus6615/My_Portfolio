using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class GameFuction : MonoBehaviour
{
    static public long GOLD_UNIT    = 10000000000000000;
    static public long GOLD_END     = 1000000000000000000;
    static private float dropForce = 0.3f; // 게임 도중 변경될 수 있음

    // Temp 변수들
    static private Sprite sprite;
    static private ItemObject itemObject;
    static private DropPet dropPet;
    static private JemObject jemObject;

    static public void SetDropForce(float count, bool isRarge) 
    {
        float realCount = Math.Abs(count * 2);
        if (isRarge)
        {
            if (realCount > 30) dropForce = 12f / realCount;
            else dropForce = 0.4f;
        }
        else
        {
            if (realCount > 30) dropForce = 9f / realCount;
            else dropForce = 0.3f;
        }
    }

    static public float[] GetArrayPlus(float _plus, params float[] _percents)
    {
        float total = 1f;
        float[] array = (float[])_percents.Clone();
        for (int i = 0; i < array.Length; i++)
            array[i] *= _plus;

        // 데이터 조정
        for (int i = array.Length - 1; i >= 0; i--)
        {
            if (total >= 0f)
            {
                if (total < array[i])
                    array[i] = total;
                total -= array[i];
            }
            else
                array[i] = 0f;
        }
        return array;
    }

    public static bool HasElement(int[] _array, int _element)
    {
        for (int i = 0; i < _array.Length; i++)
            if (_array[i] == _element)
                return true;
        return false;
    }

    public static int[] GetRandomDatas (int _minValue, int _maxValue, int _num)
    {
        int[] return_array = new int[_num];
        int rand;

        for (int i = 0; i < _num; i++)
        {
            do
            {
                rand = UnityEngine.Random.Range(_minValue, _maxValue);
            } while (HasElement(return_array, rand));
            return_array[i] = rand;
        }

        return return_array;
    }

    static public int GetRandFlag(params float[] _percents)
    {
        float[] percents = (float[])_percents.Clone();
        float value = UnityEngine.Random.value;

        // 데이터 설정
        for (int i = 1; i < percents.Length; i++)
            percents[i] += percents[i - 1];

        // 데이터를 (0f ~ 1f)로 정규화 작업
        for (int i = 0; i < percents.Length; i++)
            percents[i] /= percents[percents.Length - 1];

        // 확률 Index 정하기
        for (int i = 1; i < percents.Length; i++)
        {
            if (percents[i - 1] <= value && value < percents[i])
                return i;
        }

        return 0;
    }

    static public bool GetRandFlag(float percent)
    {
        float data = percent;

        if (UnityEngine.Random.value < data)
            return true;
        else
            return false;
    }

    static public long GetNumOreByRound(long num, float count, out float n_count)
    {
        long value = num;
        int std0 = (int)Mathf.Pow(10, value.ToString().Length - 1);
        int std1 = (int)Mathf.Pow(10, value.ToString().Length - 2);
        n_count = count;

        // 최상단 2자리 수만 유지 & 2자리 수가 7 이상이면 올림
        if (std1 != 0)
        {
            value -= value % std1;
            long temp = value % std0 / std1;
            if (temp >= 7)
                value += std1 * (10 - temp);
        }

        if (std0 != 0 && std1 != 0) 
            n_count += (value / std0) + (value % std0 / std1);
        else 
            n_count += (value / 10) + (value % 10);
            
        return value;
    }

    static public string GetNumText(long num)
    {
        if (num >= 10000000000000)
            return (num / 1000000000000) + " T";
        else if (num >= 10000000000)
            return (num / 1000000000) + " G";
        else if (num >= 10000000)
            return (num / 1000000) + " M";
        else if (num >= 10000)
            return (num / 1000) + " K";
        else
            return num.ToString();
    }

    static public Color GetColorEquipment(int[] _equipment, int _type)
    {
        if (_equipment[_type] < SaveScript.reinforceNumAsQulity * 5)
            return SaveScript.qualityColors[_equipment[_type] / SaveScript.reinforceNumAsQulity];
        else if (_equipment[_type] < 10000)
            return SaveScript.qualityColors[5];
        else
            return SaveScript.qualityColors[6];
    }

    static public string GetGoldText(long gold, long gold2, long gold3, long gold4)
    {
        int count = 0;
        string goldText = "";

        if (gold4 > 0)
        {
            if (gold4 >= 10000000000000000)
            {
                goldText = string.Format("{0:#,0}", gold4 / 10000000000000000);
                goldText += "." + gold4 / 100000000000000 % 100;
                goldText += " (TU)";
            }
            else if (gold4 >= 1000000000)
            {
                goldText = string.Format("{0:#,0}", gold4 / 1000000000);
                goldText += "." + gold4 / 10000000 % 100;
                goldText += " (GU)";
            }
            else
            {
                goldText = string.Format("{0:#,0}", gold4);
                goldText += "." + gold4 / 100000000000000 % 100;
                goldText += " (MU)";
            }
        }
        else
        {
            if (gold3 > 0)
            {
                if (gold3 >= 1000000000)
                {
                    goldText = string.Format("{0:#,0}", gold3 / 1000000000);
                    goldText += "." + gold3 / 10000000 % 100;
                    goldText += " (Kenny)";
                }
                else
                {
                    goldText = string.Format("{0:#,0}", gold3);
                    goldText += "." + gold3 / 100000000000000 % 100;
                    goldText += " (Runny)";
                }
            }
            else
            {
                if (gold2 == 0)
                {
                    if (gold == 0)
                        goldText = "0 원";
                    else
                    {
                        if (gold / 1000000000000 % 10000 != 0)
                        {
                            goldText += gold / 1000000000000 % 10000 + "조 ";
                            count++;
                        }
                        if (gold / 100000000 % 10000 != 0)
                        {
                            goldText += gold / 100000000 % 10000 + "억 ";
                            count++;
                        }
                        if (gold / 10000 % 10000 != 0)
                        {
                            goldText += gold / 10000 % 10000 + "만 ";
                            count++;
                        }
                        if (count < 3 && gold % 10000 != 0)
                            goldText += (gold % 10000) + " ";
                        goldText += "원";
                    }
                }
                else
                {
                    if (gold2 / 1000000000000 % 10000 != 0)
                    {
                        goldText += gold2 / 1000000000000 % 10000 + "양 ";
                        count++;
                    }
                    if (gold2 / 100000000 % 10000 != 0)
                    {
                        goldText += gold2 / 100000000 % 10000 + "자 ";
                        count++;
                    }
                    if (gold2 / 10000 % 10000 != 0)
                    {
                        goldText += gold2 / 10000 % 10000 + "해 ";
                        count++;
                    }
                    if (count < 3 && gold2 % 10000 != 0)
                        goldText += (gold2 % 10000) + "경 ";
                    goldText += "원";
                }
            }
        }

        return goldText;
    }

    static public void SettingGold()
    {
        long gold = 0, gold2 = 0, gold3 = 0;

        if (SaveScript.saveData.gold2 >= GOLD_UNIT)
        {
            gold3 = SaveScript.saveData.gold / GOLD_UNIT;
            gold2 = SaveScript.saveData.gold % GOLD_UNIT;
            SaveScript.saveData.gold3 += gold3;
            SaveScript.saveData.gold2 = gold2;
            AchievementCtrl.instance.SetAchievementAmount(20, 0, -gold3 * GOLD_UNIT, gold3);
        }

        if (SaveScript.saveData.gold >= GOLD_UNIT)
        {
            gold2 = SaveScript.saveData.gold / GOLD_UNIT;
            gold = SaveScript.saveData.gold % GOLD_UNIT;
            SaveScript.saveData.gold2 += gold2;
            SaveScript.saveData.gold = gold;
            AchievementCtrl.instance.SetAchievementAmount(20, -gold2 * GOLD_UNIT, gold2, 0);
        }
    }

    static public bool CheckCanBuy(long gold, long gold2, long gold3, long cmpGold, long cmpGold2, long cmpGold3)
    {
        bool isCanBuy = false;
        if (gold3 > 0)
        {
            // 루니 단위
            if (cmpGold3 >= gold3)
                isCanBuy = true;
        }
        else
        {
            if (cmpGold3 > 0)
                isCanBuy = true; // 최소 경 단위인데 이미 루니를 가지고 있음
            else
            {
                // 최소 경 단위인데 루니가 없음
                if (gold2 > 0)
                {
                    // 경 단위
                    if (cmpGold2 >= gold2)
                        isCanBuy = true;
                }
                else
                {
                    // 원 단위
                    if (cmpGold2 > 0 || cmpGold >= gold)
                        isCanBuy = true;
                }
            }
        }
        return isCanBuy;
    }

    static public bool CheckCanBuy(long gold, long gold2, long gold3)
    {
        return CheckCanBuy(gold, gold2, gold3, SaveScript.saveData.gold, SaveScript.saveData.gold2, SaveScript.saveData.gold3);
    }

    static public void Buy(long gold, long gold2, long gold3, ref long cmpGold, ref long cmpGold2, ref long cmpGold3)
    {
        if (CheckCanBuy(gold, gold2, gold3))
        {
            if (cmpGold < gold)
            {
                cmpGold2--;
                cmpGold += GOLD_UNIT;
            }
            if (cmpGold2 < gold2)
            {
                cmpGold3--;
                cmpGold2 += GOLD_UNIT;
            }
            cmpGold -= gold;
            cmpGold2 -= gold2;
            cmpGold3 -= gold3;
        }
    }

    static public void Buy(long gold, long gold2, long gold3)
    {
        Buy(gold, gold2, gold3, ref SaveScript.saveData.gold, ref SaveScript.saveData.gold2, ref SaveScript.saveData.gold3);
    }

    static public string ToHangleFromUniCode(string unicode)
    {
        byte[] bytesForEncoding = Encoding.UTF8.GetBytes("unicode");
        string encodedString = Convert.ToBase64String(bytesForEncoding);
        byte[] decodedBytes = Convert.FromBase64String(encodedString);
        string decodedString = Encoding.UTF8.GetString(decodedBytes);

        return decodedString;
    }

    static public string GetTimeText(int _time)
    {
        string text = "";

        if(_time >= 3600)
        {
            if (_time / 3600 >= 10) text = _time / 3600 + " : ";
            else text = "0" + _time / 3600 + " : ";
        }
        _time %= 3600;

        if (_time >= 60)
        {
            if(_time / 60 >= 10) text += _time / 60 + " : ";
            else text += "0" + _time / 60 + " : ";
        }
        else
        {
            text += "00 : ";
        }
        if (_time % 60 >= 10) text += _time % 60;
        else text += "0" + _time % 60;

        return text;
    }

    static public float GetElixirPercent(int _itemCode)
    {
        if (SaveScript.saveData.isElixirOns[_itemCode])
            return SaveScript.elixirs[_itemCode].forcePercent;
        else
            return 0;
    }

    static public float GetBufItemPercent(int _itemCode)
    {
        float percent = 0f;

        for (int i = 0; i < SaveScript.bufItemTypeNum; i++)
            if (SaveScript.saveData.isBufItemOns[_itemCode * SaveScript.bufItemTypeNum + i])
                percent += SaveScript.bufItems[_itemCode * SaveScript.bufItemTypeNum + i].forcePercent;

        return percent;
    }

    static public int GetTier_EXP(int level)
    {
        int tier;

        if (level < 100) // 브론즈 ~ 다이아 
            tier = level / 5;
        else if (level < 500) // 마스터
            tier = 20 + (level - 100) / 100;
        else // 첼린저
            tier = 24;

        return tier;
    }

    static public float GetOnlyExpPercent(int _equipment, int _type)
    {
        float percent = 0f;
        float full_0 = 0f;
        float full_1 = 0f;
        int equipUpgradeType = 0;
        int tier;

        // 변수 설정
        switch (_equipment)
        {
            case 0: equipUpgradeType = (_type == 0) ? SaveScript.saveData.pick1Upgrades : SaveScript.saveData.pick2Upgrades; break;
            case 1: equipUpgradeType = (_type == 0) ? SaveScript.saveData.hat1Upgrades : SaveScript.saveData.hat2Upgrades; break;
            case 2: equipUpgradeType = (_type == 0) ? SaveScript.saveData.ring1Upgrades : SaveScript.saveData.ring2Upgrades; break;
            case 3: equipUpgradeType = (_type == 0) ? SaveScript.saveData.Pendant1Upgrades : SaveScript.saveData.Pendant2Upgrades; break;
            case 4: equipUpgradeType = (_type == 0) ? SaveScript.saveData.sword1Upgrades : SaveScript.saveData.sword2Upgrades; break;
        }

        tier = GetTier_EXP(equipUpgradeType);
        for (int i = 0; i < tier; i++)
        {
            if (i < 20)
                full_0 += SaveScript.expReinforcePercents[_equipment * 2 + _type][i];
            else
                full_1 += SaveScript.expReinforcePercents[_equipment * 2 + _type][i];
        }
        percent += (full_0 * 5 + full_1 * 100);

        if (equipUpgradeType < 100)
            percent += SaveScript.expReinforcePercents[_equipment * 2 + _type][tier] * (equipUpgradeType % 5);
        else if (equipUpgradeType < 500)
            percent += SaveScript.expReinforcePercents[_equipment * 2 + _type][tier] * (equipUpgradeType % 100);
        else
            percent += SaveScript.expReinforcePercents[_equipment * 2 + _type][tier] * (equipUpgradeType - 500);

        return percent;
    }

    static public int GetTier_R(int level, out int _tier_1)
    {
        int tier_0;
        int tier_1;

        if (level < 20)
        {
            tier_0 = 0;
            tier_1 = level / 4;
        }
        else if (level < 100)
        {
            tier_0 = 1;
            tier_1 = (level - 20) / 20;
        }
        else if (level < 1000)
        {
            tier_0 = 2;
            tier_1 = (level - 100) / 100;
        }
        else if (level < 10000)
        {
            tier_0 = 3;
            tier_1 = (level - 1000) / 1000;
        }
        else
        {
            tier_0 = 4;
            tier_1 = 0;
        } 

        _tier_1 = tier_1;
        return tier_0;
    }

    static public float GetOnly_R_Percent(int _equipment, int _type)
    {
        float percent = 0f;
        int reinforce;
        int _kind, tier_0, tier_1;
        float totalPercent = 0f;
        float[][] equipUpgradeForce;

        // 변수 설정
        switch (_equipment)
        {
            case 0:
                if ((_kind = SaveScript.saveData.equipPick) < 0) return 0f;
                reinforce = SaveScript.saveData.pickReinforces[_kind];
                tier_0 = GetTier_R(reinforce, out tier_1);
                equipUpgradeForce = (_type == 0) ? SaveScript.pickReinforce1Percents : SaveScript.pickReinforce2Percents;
                break;
            case 1:
                if ((_kind = SaveScript.saveData.equipHat) < 0) return 0f;
                reinforce = SaveScript.saveData.hatReinforces[_kind];
                tier_0 = GetTier_R(reinforce, out tier_1);
                equipUpgradeForce = (_type == 0) ? SaveScript.hatReinforce1Percents : SaveScript.hatReinforce2Percents;
                break;
            case 2:
                if ((_kind = SaveScript.saveData.equipRing) < 0) return 0f;
                reinforce = SaveScript.saveData.ringReinforces[_kind];
                tier_0 = GetTier_R(reinforce, out tier_1);
                equipUpgradeForce = (_type == 0) ? SaveScript.ringReinforce1Percents : SaveScript.ringReinforce2Percents;
                break;
            case 3:
                if ((_kind = SaveScript.saveData.equipPendant) < 0) return 0f;
                reinforce = SaveScript.saveData.pendantReinforces[_kind];
                tier_0 = GetTier_R(reinforce, out tier_1);
                equipUpgradeForce = (_type == 0) ? SaveScript.pendantReinforce1Percents : SaveScript.pendantReinforce2Percents;
                break;
            default:
                if ((_kind = SaveScript.saveData.equipSword) < 0) return 0f;
                reinforce = SaveScript.saveData.swordReinforces[_kind];
                tier_0 = GetTier_R(reinforce, out tier_1);
                equipUpgradeForce = (_type == 0) ? SaveScript.swordReinforce1Percents : SaveScript.swordReinforce2Percents;
                break;
        }

        for (int i = 0; i < equipUpgradeForce.Length; i++)
        {
            if (tier_0 > i)
            {
                for (int j = 0; j < equipUpgradeForce[i].Length; j++)
                {
                    switch (i)
                    {
                        case 0: totalPercent += equipUpgradeForce[i][j] * 4; break;
                        case 1: totalPercent += equipUpgradeForce[i][j] * 20; break;
                        case 2: totalPercent += equipUpgradeForce[i][j] * 100; break;
                        case 3: totalPercent += equipUpgradeForce[i][j] * 1000; break;
                    }
                }
            }
        }
        for (int i = 0; i < tier_1; i++)
        {
            switch (tier_0)
            {
                case 0: totalPercent += equipUpgradeForce[tier_0][i] * 4; break;
                case 1: totalPercent += equipUpgradeForce[tier_0][i] * 20; break;
                case 2: totalPercent += equipUpgradeForce[tier_0][i] * 100; break;
                case 3: totalPercent += equipUpgradeForce[tier_0][i] * 1000; break;
            }
        }
        percent += totalPercent;
        if (reinforce < 20)
            percent += equipUpgradeForce[tier_0][tier_1] * (reinforce % 4);
        else if (reinforce < 100)
            percent += equipUpgradeForce[tier_0][tier_1] * (reinforce % 20);
        else if (reinforce < 1000)
            percent += equipUpgradeForce[tier_0][tier_1] * (reinforce % 100);
        else if (reinforce < 10000)
            percent += equipUpgradeForce[tier_0][tier_1] * (reinforce % 1000);
        else
            percent += equipUpgradeForce[tier_0][tier_1] * (reinforce - 10000);

        return percent;
    }

    static public float GetReinforcePercent(int _equipment, int _type)
    {
        return GetOnlyExpPercent(_equipment, _type) + GetOnly_R_Percent(_equipment, _type);
    }

    static public float GetReinforcePercent_Over2(int _equipment)
    {
        switch (_equipment)
        {
            case 0: return (SaveScript.stat.pick02 - 1) / 50f;
            case 1: return 2f + (SaveScript.stat.hat02 - 1) / 50f;
            case 2: return 2f + (SaveScript.stat.ring02 - 1) / 50f;
            case 3: return 1f + (SaveScript.stat.pendant02 - 1) * 200f;
            default: return 2f + (SaveScript.stat.sword02 - 1) / 50f;
        }
    }

    static public float GetReinforcePercent_Over2(int _equipment, float percent)
    {
        switch (_equipment)
        {
            case 0: return percent / 50f;
            case 1: return percent / 50f;
            case 2: return percent / 50f;
            case 3: return percent * 200f;
            default: return percent / 50f;
        }
    }

    /// <summary>
    /// 광물 개수 반환
    /// </summary>
    static public long GetOreNum()
    {
        long num;
        float force;
        float plusPercent;
        float percent = SaveScript.stat.pendant02;
        if (SaveScript.saveData.equipPendant == -1)
        {
            force = 0f;
            plusPercent = SaveScript.stat.pendant01;
        }
        else
        {
            force = SaveScript.pendants[SaveScript.saveData.equipPendant].forcePercent;
            plusPercent = SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * SaveScript.stat.pendant01;
        }

        long min, max = Mathf.RoundToInt(force + plusPercent);
        if (percent < 1f)
            min = 1;
        else
        {
            if (SaveScript.saveData.equipPendant == -1)
                min = Mathf.RoundToInt(SaveScript.pendants[0].reinforce_basic * GetReinforcePercent_Over2(3));
            else
                min = Mathf.RoundToInt(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * GetReinforcePercent_Over2(3));
        }

        if (min >= max)
            num = max;
        else
            num = (long)UnityEngine.Random.Range(min, max + 1);

        return num;
    }

    static public void CreateBufItem(Vector3 point, Transform parent, float createPercent, int bufItem_type, float count, out float n_count)
    {
        int itemCode;
        float plusPercent = 1f + SaveScript.stat.itemDrop;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 6)
            plusPercent *= 2;
        float totalPercent = createPercent * plusPercent;
        if (totalPercent > (int)createPercent + 1)
            totalPercent = (int)createPercent + 1;
        n_count = count;

        // 버프 아이템 생성
        while (totalPercent > 0f)
        {
            if (GetRandFlag(totalPercent))
            {
                itemCode = GetRandFlag(BufItem.dropPercents) * SaveScript.bufItemTypeNum + bufItem_type;

                itemObject = ObjectPool.GetObject<ItemObject>(4, parent, point);
                itemObject.sprite.sprite = SaveScript.bufItems[itemCode].sprite;
                itemObject.itemType = 0;
                itemObject.itemCode = itemCode;
                itemObject.initForceVec = new Vector2(dropForce * n_count++, 2f);
                itemObject.touchTime = 0.5f;
            }
            totalPercent -= 1f;
        }
    }

    static public void CreateReinforceItem(Vector3 point, Transform parent, float createPercent, float count, out float n_count)
    {
        int itemCode;
        float plusPercent = 1f + SaveScript.stat.itemDrop;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 6)
            plusPercent *= 2;
        float totalPercent = createPercent * plusPercent;
        if (totalPercent > (int)createPercent + 1)
            totalPercent = (int)createPercent + 1;
        n_count = count;

        // 강화 아이템 생성
        while (totalPercent > 0f)
        {
            if (GetRandFlag(totalPercent))
            {
                itemCode = GetRandFlag(GetArrayPlus(plusPercent, ReinforceItem.dropPercents));

                itemObject = ObjectPool.GetObject<ItemObject>(4, parent, point);
                itemObject.sprite.sprite = SaveScript.reinforceItems[itemCode].sprite;
                itemObject.itemType = 1;
                itemObject.itemCode = itemCode;
                itemObject.initForceVec = new Vector2(dropForce * n_count++, 2f);
                itemObject.touchTime = 0.5f;
            }
            totalPercent -= 1f;
        }
    }

    static public void CreateElixirItem(Vector3 point, Transform parent, float createPercent, float count, out float n_count)
    {
        int itemCode;
        float plusPercent = 1f + SaveScript.stat.itemDrop;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 6)
            plusPercent *= 2;
        float totalPercent = createPercent * plusPercent;
        if (totalPercent > (int)createPercent + 1)
            totalPercent = (int)createPercent + 1;
        n_count = count;

        // 영약 아이템 생성
        while (totalPercent > 0f)
        {
            if (GetRandFlag(totalPercent))
            {
                itemCode = GetRandFlag(Elixir.dropPercents);

                itemObject = ObjectPool.GetObject<ItemObject>(4, parent, point);
                itemObject.sprite.sprite = SaveScript.elixirs[itemCode].sprite;
                itemObject.itemType = 2;
                itemObject.itemCode = itemCode;
                itemObject.initForceVec = new Vector2(dropForce * n_count++, 2f);
                itemObject.touchTime = 0.5f;
            }
            totalPercent -= 1f;
        }
    }

    static public void CreateReinforce2Item(Vector3 point, Transform parent, float createPercent, float multiply, float count, out float n_count)
    {
        int itemCode;
        float plusPercent = (1f + SaveScript.stat.itemDrop) * multiply;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 6)
            plusPercent *= 2;
        float totalPercent = createPercent * plusPercent;
        if (totalPercent > (int)createPercent + 1)
            totalPercent = (int)createPercent + 1;
        n_count = count;

        // 제련석 & 유물 아이템 생성
        while (totalPercent > 0f)
        {
            if (GetRandFlag(totalPercent))
            {
                itemCode = GetRandFlag(GetArrayPlus(plusPercent, ReinforceItem2.dropPercents));

                itemObject = ObjectPool.GetObject<ItemObject>(4, parent, point);
                itemObject.sprite.sprite = SaveScript.reinforceItems2[itemCode].sprite;
                itemObject.itemType = 3;
                itemObject.itemCode = itemCode;
                itemObject.initForceVec = new Vector2(dropForce * n_count++, 2f);
                itemObject.touchTime = 0.5f;
            }
            totalPercent -= 1f;
        }
    }

    static public void CreateDropReinforceOre(Vector3 point, long num, float count, out float n_count)
    {
        int unit = ReinforceOre.unitNum - 1;
        long standard = (long)Mathf.Pow(10, unit);

        n_count = count;
        while (num > 0)
        {
            while (num < standard)
            {
                unit--;
                standard = (long)Mathf.Pow(10, unit);
            }

            ReinforceOre.CreateReinforceObject(point, new Vector2(dropForce * n_count, 2f), standard);
            num -= standard;
            n_count += 1f;
        }
    }

    static public void CreateDropManaOre(Vector3 point, long num, float count, out float n_count)
    {
        n_count = count;
        CreateDropManaOre(point, num, 2f, count, out n_count);
    }

    static public void CreateDropManaOre(Vector3 point, long num, float upForce, float count, out float n_count)
    {
        int unit = ManaOre.unitNum - 1;
        long standard = (long)Mathf.Pow(10, unit);

        n_count = count;
        while (num > 0)
        {
            while (num < standard)
            {
                unit--;
                standard = (long)Mathf.Pow(10, unit);
            }

            ManaOre.CreateManaObject(point, new Vector2(dropForce * n_count, upForce), standard);
            num -= standard;
            n_count += 1f;
        }
    }

    static public void CreateDropCashOre(Vector3 point, long num, float count, out float n_count)
    {
        n_count = count;
        CreateDropCashOre(point, num, 2f, count, out n_count);
    }

    static public void CreateDropCashOre(Vector3 point, long num, float upForce, float count, out float n_count)
    {
        n_count = count;
        while (num > 0)
        {
            if (num >= 100)
            {
                CashOre.CreateCashObject(point, new Vector2(dropForce * n_count, upForce), 100);
                num -= 100;
            }
            else if (num >= 10)
            {
                CashOre.CreateCashObject(point, new Vector2(dropForce * n_count, upForce), 10);
                num -= 10;
            }
            else
            {
                CashOre.CreateCashObject(point, new Vector2(dropForce * n_count, upForce), 1);
                num -= 1;
            }
            n_count += 1f;
        }
    }

    static public void CreateDropJem(Vector3 point, List<long> jems, float count, out float n_count)
    {
        int quality = 0;
        int unit = Jem.unitNum - 1;
        long standard = (long)Mathf.Pow(10, unit);

        n_count = count;
        for (int i = 0; i < jems.Count; i++)
        {
            if (jems[i] != 0 && SaveScript.jems[i].quality == quality)
            {
                unit = Jem.unitNum - 1;
                standard = (long)Mathf.Pow(10, unit);

                while (jems[i] > 0)
                {
                    while (jems[i] < standard)
                    {
                        unit--;
                        standard = (long)Mathf.Pow(10, unit);
                    }

                    switch (unit)
                    {
                        case 0: sprite = SaveScript.jems[i].jemSprite; break;
                        case 1: sprite = SaveScript.jems[i].jem_unitSprite; break;
                        case 2: sprite = SaveScript.jems[i].jem_unit2Sprite; break;
                        case 3: sprite = SaveScript.jems[i].jem_unit3Sprite; break;
                        case 4: sprite = SaveScript.jems[i].jem_unit4Sprite; break;
                        case 5: sprite = SaveScript.jems[i].jem_unit5Sprite; break;
                        case 6: sprite = SaveScript.jems[i].jem_unit6Sprite; break;
                        case 7: sprite = SaveScript.jems[i].jem_unit7Sprite; break;
                        case 8: sprite = SaveScript.jems[i].jem_unit8Sprite; break;
                        case 9: sprite = SaveScript.jems[i].jem_unit9Sprite; break;
                    }

                    jemObject = ObjectPool.GetObject<JemObject>(0, ObjectPool.instance.objectTr, point);
                    jemObject.sprites[unit].sprite = sprite;
                    jemObject.jemIndex = i;
                    jemObject.jemType = unit;
                    jemObject.touchTime = 0.25f;
                    jemObject.initForceVec = new Vector2(dropForce * n_count, 2f);
                    jems[i] -= standard;

                    n_count += 1f;
                }
            }

            if (i == jems.Count - 1 && quality < 6)
            {
                i = 0;
                quality++;
            }
        }
    }

    static public void CreateDropPet(Vector3 point, Transform parent, float percent, float count, out float n_count)
    {
        float plusPercent = 1f + SaveScript.stat.petDrop;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 6)
            plusPercent *= 2;
        n_count = count;

        for (int i = DropPet.dropPercents.Length - 1; i >= 0; i--)
        {
            if (GetRandFlag(DropPet.dropPercents[i] * percent * plusPercent))
            {
                dropPet = Instantiate(ObjectPool.instance.dropPet, point, Quaternion.identity, parent).GetComponent<DropPet>();
                dropPet.code = i;
                dropPet.initForceVec = new Vector2(dropForce * count, 2f);
                dropPet.touchTime = 0.75f;
                if (GetRandFlag(DropPet.TypePercent)) dropPet.type = 1;
                else dropPet.type = 0;
                n_count += 1f;

                break;
            }
        }
    }

    static public void CreateDropPet(Vector3 point, Transform parent, float[] percents, float count, out float n_count)
    {
        float plusPercent = 1f + SaveScript.stat.petDrop;
        if (EventCtrl.instance.isWeekEventOn && EventCtrl.instance.weekEventType == 6)
            plusPercent *= 2;

        float[] plusPercents = GetArrayPlus(plusPercent, percents);
        int petCode = GetRandFlag(plusPercents);
        n_count = ++count;

        dropPet = Instantiate(ObjectPool.instance.dropPet, point, Quaternion.identity, parent).GetComponent<DropPet>();
        dropPet.code = petCode;
        dropPet.initForceVec = new Vector2(dropForce * count, 2f);
        dropPet.touchTime = 0.75f;
        if (GetRandFlag(DropPet.TypePercent)) dropPet.type = 1;
        else dropPet.type = 0;
    }

    static public int GetQualityOfEquipment(int _star)
    {
        int quality = 0;

        if (_star < SaveScript.reinforceNumAsQulity) quality = 0;
        else if (_star < SaveScript.reinforceNumAsQulity * 2) quality = 1;
        else if (_star < SaveScript.reinforceNumAsQulity * 3) quality = 2;
        else if (_star < SaveScript.reinforceNumAsQulity * 4) quality = 3;
        else if (_star < SaveScript.reinforceNumAsQulity * 5) quality = 4;
        else if (_star < 10000) quality = 5;
        else quality = 6;

        return quality;
    }

    static public int GetEXPUpgradeValue(int x)
    {
        return 100 * (x + 1);
    }

    static public long GetAchievementAmount(double a, int x, double pow, double c)
    {
        return (long)(a * Math.Pow(x, pow) + c);
    }

    static public long GetAchievementAmount_gold(int type)
    {
        long amount = 0;
        double a = 0;
        double pow = 0;
        double c = 0;
        short x = 0;

        if (SaveScript.saveData.achievementLevels[type] < 40)
            { a = 50000; pow = 2; c = 10000; x = 0; }
        else if (SaveScript.saveData.achievementLevels[type] < 80)
            { a = 10; pow = 5; c = 1; x = 40; }
        else
            { a = 10; pow = 5; c = 1; x = 80; }

        amount = GetAchievementAmount(a, SaveScript.saveData.achievementLevels[type] - x, pow, c);
        if (type == 29) amount /= 10;
        if (amount < 1) amount = 1;

        return amount;
    }

    static public int GetManaBufForceForText(int code)
    {
        int force;
        float bufForce = SaveScript.saveData.facility_level * 0.03f;

        switch (code / 3)
        {
            case 0: // 시간
                force = Mathf.RoundToInt(SaveScript.manaBufs[code].force * (1f + bufForce));
                break;
            case 1: // 상점 판매율
                if (SaveScript.saveData.equipRing == -1) force = Mathf.RoundToInt(SaveScript.rings[0].reinforce_basic * SaveScript.manaBufs[code].force * 100 * (1f + bufForce));
                else force = Mathf.RoundToInt(SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * SaveScript.manaBufs[code].force * 100 * (1f + bufForce));
                break;
            case 2: // 광물
                if (SaveScript.saveData.equipPendant == -1) force = Mathf.RoundToInt(SaveScript.pendants[0].reinforce_basic * SaveScript.manaBufs[code].force * (1f + bufForce));
                else force = Mathf.RoundToInt(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * SaveScript.manaBufs[code].force * (1f + bufForce));
                break;
            default: // 경험치 & 강화
                force = Mathf.RoundToInt(SaveScript.manaBufs[code].force * 100);
                break;
        }

        return force;
    }

    static public float GetManaBufForceForData(int code)
    {
        float force = 0f;
        float bufForce = SaveScript.saveData.facility_level * 0.03f;

        for (int i = 0; i < SaveScript.manaBufType; i++)
            if (SaveScript.saveData.isManaBuffOns[code * SaveScript.manaBufType + i])
                force += SaveScript.manaBufs[code * SaveScript.manaBufType + i].force;

        if(!(code == 3 || code == 4)) // 경험치 & 강화 제외
            force *= (1f + bufForce);

        return force;
    }

    static public float GetManaUpgradeForce(int code)
    {
        return SaveScript.saveData.manaUpgrades[code] * SaveScript.manaUpgrades[code].force;
    }

    static public void SetAttendanceData()
    {
        if (SaveScript.dateTime.Month != SaveScript.saveData.attendance_month)
        {
            // 출석 체크 초기화
            SaveScript.saveData.attendance_month = SaveScript.dateTime.Month;
            SaveScript.saveData.attendance_count = 0;
            SaveScript.saveData.attendance_day = 0;
        }
    }

    static public void SetComboData()
    {
        if (SaveScript.dateTime.Day != SaveScript.dateTime.AddSeconds(-1f).Day)
        {
            // 콤보 초기화
            SaveScript.saveData.combo_gauge = 0;
        }
    }

    static public int GetMailValue(int _type)
    {
        switch (_type)
        {
            case 0: // ManaOre
                return SaveScript.mailboxManaOres[SaveScript.saveData.pickLevel];
            case 1: // Cash - 1 Day
                return -30;
        }
        return 0;
    }

    static public int GetEmptyMail()
    {
        for (int i = SaveScript.mailboxNum - 1; i >= 0; i--)
            if (SaveScript.saveData.mailboxes[i] == 0)
                return i;
        return -1;
    }

    static public void PushMail(int _value)
    {
        int index = GetEmptyMail();

        if(index == -1)
        {
            // Full
            for (int i = SaveScript.mailboxNum - 2; i >= 0; i--)
                SaveScript.saveData.mailboxes[i + 1] = SaveScript.saveData.mailboxes[i];
            SaveScript.saveData.mailboxes[0] = _value;
        }
        else
        {
            // Empty
            SaveScript.saveData.mailboxes[index] = _value;
        }
    }

    static public int GetGoalCardNum(int jemCode)
    {
        return SaveScript.saveData.collection_levels[jemCode] + 1;
    }

    static public float GetCollectionUpgradeForce(int type)
    {
        float force = 0f;
        for (int i = 0; i < SaveScript.cardForceAsType[type].Length; i++)
            force += SaveScript.collections[SaveScript.cardForceAsType[type][i]].force * SaveScript.saveData.collection_levels[SaveScript.cardForceAsType[type][i]];

        return force;
    }

    static public float GetCollectionFloorUpgradeForce(int floor, int type)
    {
        float percent = 0f;
        int level = GetFloorLevel(floor);
        switch (type)
        {
            case 0: // 추가 판매
            case 1: // 광물
                if (level >= SaveScript.cardLevelsPerFloor[type])
                    percent += Collection.floor_forces[floor][type];
                break;
            case 2: // 특수 능력
                int count = type;
                while (level >= SaveScript.cardLevelsPerFloor[count])
                {
                    percent += Collection.floor_forces[floor][count++];
                    if (count > 6) break;
                }
                break;
        }
        return percent;
    }

    static public int GetFloorLevel(int floor)
    {
        int level = 0;
        int startIndex = 0;
        int endIndex = 0;

        for (int i = 0; i < floor; i++)
            startIndex += SaveScript.stageItemNums[i];
        endIndex = startIndex + SaveScript.stageItemNums[floor];

        for (int i = startIndex; i < endIndex; i++)
            level += SaveScript.saveData.collection_levels[i];

        return level;
    }

    static public int GetRealExp(int _exp, out int _type)
    {
        int realExp, type = 0;
        float plusExp = 1f + SaveScript.stat.exp;
        float doubleExp = SaveScript.stat.exp2;

        realExp = Mathf.RoundToInt(_exp * plusExp);
        if (GetRandFlag(doubleExp))
        {
            realExp *= 2;
            type++;
        }

        if (EventCtrl.instance.isWeekEventOn)
        {
            realExp *= 2;
            type++;
        }

        _type = type;
        return realExp;
    }

    static public int GetComboLevel()
    {
        int gauge = SaveScript.saveData.combo_gauge;
        int level = 0;

        while (gauge >= SaveScript.combo_unit * (level + 1))
            level++;

        return level;
    }

    static public void AddComboGauge(int _gauge_index)
    {
        if (SaveScript.saveData.isTutorial)
            return; // 튜토리얼의 경우 콤보 쌓기 불가능

        SaveScript.saveData.combo_gauge += SaveScript.combo_plus[_gauge_index];
        if (SaveScript.saveData.combo_gauge > SaveScript.combo_max)
            SaveScript.saveData.combo_gauge = SaveScript.combo_max;

        if (SaveScript.stat.combo_level != GetComboLevel())
        {
            SaveScript.stat.SetStat();
            PrintUI.instance.AudioPlay(34);
            SystemInfoCtrl.instance.SetShowInfo("일일 콤보 레벨이 < Lv. " + SaveScript.stat.combo_level + " > 되었습니다!");
        }
        MainComboUI.instance.SetMainUI();
    }

    static public float GetComboForce(int _type)
    {
        float value = 0f;
        float multifly = SaveScript.combo_multiply[GetPlayerGrade()];
        int level = SaveScript.stat.combo_level;

        switch (_type)
        {
            case 0: // 경험치
                if (level > 0) value += SaveScript.combo_forces[0];
                if (level > 1) value += SaveScript.combo_forces[1];
                break;
            case 1: // 얼티밋 & 미스틱
                if (level > 2) value += SaveScript.combo_forces[2];
                if (level > 3) value += SaveScript.combo_forces[3];
                break;
            case 2: // 마나석 광석 & 상자
                if (level > 4) value += SaveScript.combo_forces[4];
                break;
        }

        return value * multifly;
    }

    static public int GetPlayerGrade()
    {
        if (SaveScript.saveData.pickLevel < 6)
            return 0;
        else if (SaveScript.saveData.pickLevel < 10)
            return 1;
        else
            return 2;
    }

    static public void SetPlayerStat()
    {
        if (PlayerScript.instance != null)
        {
            // HP관련 버프 설정
            if (SaveScript.saveData.isElixirOns[0] || SaveScript.saveData.isBufItemOns[0] || SaveScript.saveData.isBufItemOns[1] || SaveScript.saveData.isBufItemOns[2])
            {
                if (PlayerScript.instance.bufPlusPercentHP != GetBufItemPercent(0) + GetElixirPercent(0))
                {
                    long plusedHP;
                    PlayerScript.instance.ExceptBufStat(0);
                    PlayerScript.instance.bufPlusPercentHP = GetBufItemPercent(0) + GetElixirPercent(0);
                    PlayerScript.instance.plusPercentHP += PlayerScript.instance.bufPlusPercentHP;
                    plusedHP = (long)(SaveScript.picks[SaveScript.saveData.equipPick].durability + SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic
                        * PlayerScript.instance.plusPercentHP) - PlayerScript.instance.pickFullHP;

                    PlayerScript.instance.pickFullHP = (long)(SaveScript.picks[SaveScript.saveData.equipPick].durability + 
                        SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * PlayerScript.instance.plusPercentHP);
                    PlayerScript.instance.pickHP += plusedHP;
                    if (PlayerScript.instance.pickHP > PlayerScript.instance.pickFullHP)
                        PlayerScript.instance.pickHP = PlayerScript.instance.pickFullHP;
                    PickStateUI.instance.ShowPickState();
                    SaveScript.stat.SetStat();
                }
            }
            // 이속 버프 설정
            if (SaveScript.saveData.isElixirOns[8] || SaveScript.saveData.isBufItemOns[24] || SaveScript.saveData.isBufItemOns[25] || SaveScript.saveData.isBufItemOns[26])
            {
                if (PlayerScript.instance.bufMoveSpeed != GetBufItemPercent(8) + GetElixirPercent(8))
                {
                    PlayerScript.instance.ExceptBufStat(1);
                    PlayerScript.instance.bufMoveSpeed = GetBufItemPercent(8) + GetElixirPercent(8);
                    PlayerScript.instance.moveSpeed += PlayerScript.instance.bufMoveSpeed;
                    PlayerScript.instance.moveSpeedData += PlayerScript.instance.bufMoveSpeed;
                    SaveScript.stat.SetStat();
                }
            }
            // 점프력 버프 설정
            if (SaveScript.saveData.isElixirOns[9] || SaveScript.saveData.isBufItemOns[27] || SaveScript.saveData.isBufItemOns[28] || SaveScript.saveData.isBufItemOns[29])
            {
                if (PlayerScript.instance.bufJumpPower != GetBufItemPercent(9) + GetElixirPercent(9))
                {
                    PlayerScript.instance.ExceptBufStat(2);
                    PlayerScript.instance.bufJumpPower = GetBufItemPercent(9) + GetElixirPercent(9);
                    PlayerScript.instance.jumpPower += PlayerScript.instance.bufJumpPower;
                    PlayerScript.instance.jumpPowerData += PlayerScript.instance.bufJumpPower;
                    SaveScript.stat.SetStat();
                }
            }
        }
    }
}
