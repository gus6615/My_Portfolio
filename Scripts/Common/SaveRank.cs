using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;

public struct RankData
{
    public string owner_indate;
    public int[] equipments; // 현재 착요 중인 모든 장비
    public int[] equipmentUpgrades; // 현재 착용 중인 장비의 강화 성
    public int[] equipmentQualities; // 현재 착용 중인 모든 장비의 등급
    public string nickname;
    public int rank, rank2, rank3, rank4;
    public long gold, gold2, gold3, gold4;
    public int tier_level, tier_achievement;
};

public class SaveRank : MonoBehaviour
{
    public const int NO_RANK = 99999999;

    public int totalUserNum;
    public int userNum_4, userNum_3, userNum_2;
    public RankData myRankData = new RankData(); // 본인 데이터
    public List<RankData> rankData = new List<RankData>(); // 1등 ~ 1000등까지
    public bool isDone, isError;
    private bool isDoneSet2, isDoneSet3, isDoneSet4;

    BackendReturnObject BRO, BRO2;
    Where where = new Where();
    string key;
    List<JsonData> BROList_4 = new List<JsonData>();
    List<JsonData> BROList_3 = new List<JsonData>();
    List<JsonData> BROList_2 = new List<JsonData>();
    List<JsonData> BRO2List_4 = new List<JsonData>();
    List<JsonData> BRO2List_3 = new List<JsonData>();
    List<JsonData> BRO2List_2 = new List<JsonData>();

    IEnumerator SetRankData()
    {
        while (!BlindScript.isStartGame)
            yield return null;

        int i;
        bool isEndSearch2 = false, isEndSearch3 = false, isEndSearch4 = false;

        // Public 리스트 검색
        where.Clear();
        where.Greater("gold4", 0);
        do
        {
            if (key.Equals(""))
                BRO2 = Backend.GameData.Get("publicData", where, 100);
            else
                BRO2 = Backend.GameData.Get("publicData", where, 100, key);
            key = BRO2.FirstKeystring();

            for (i = 0; i < BRO2.Rows().Count; i++)
                BRO2List_4.Add(BRO2.GetReturnValuetoJSON()["rows"][i]);
        } while (BRO2.HasFirstKey());

        where.Clear();
        where.Greater("gold3", 0);
        do
        {
            if (key.Equals(""))
                BRO2 = Backend.GameData.Get("publicData", where, 100);
            else
                BRO2 = Backend.GameData.Get("publicData", where, 100, key);
            key = BRO2.FirstKeystring();

            for (i = 0; i < BRO2.Rows().Count; i++)
            {
                if (BRO2.Rows()[i].ContainsKey("gold4") && long.Parse(BRO2.Rows()[i]["gold4"]["N"].ToString()) > 0) continue;
                BRO2List_3.Add(BRO2.GetReturnValuetoJSON()["rows"][i]);
            }
        } while (BRO2.HasFirstKey());

        where.Clear();
        where.Greater("gold2", 0);
        do
        {
            if (key.Equals(""))
                BRO2 = Backend.GameData.Get("publicData", where, 100);
            else
                BRO2 = Backend.GameData.Get("publicData", where, 100, key);
            key = BRO2.FirstKeystring();

            for (i = 0; i < BRO2.Rows().Count; i++)
            {
                if (BRO2.Rows()[i].ContainsKey("gold4") && long.Parse(BRO2.Rows()[i]["gold4"]["N"].ToString()) > 0) continue;
                if (BRO2.Rows()[i].ContainsKey("gold3") && long.Parse(BRO2.Rows()[i]["gold3"]["N"].ToString()) > 0) continue;
                BRO2List_2.Add(BRO2.GetReturnValuetoJSON()["rows"][i]);
            }
        } while (BRO2.HasFirstKey());

        // 랭킹 리스트 검색
        i = 0;
        while (i < 10)
        {
            if (!isDoneSet2 && !isDoneSet3 && !isDoneSet4) // 전설 유저
                BRO = Backend.URank.User.GetRankList(SaveScript.rankUuid_gold4, 100, 100 * i);
            else if (!isDoneSet2 && !isDoneSet3) // 신화 유저
                BRO = Backend.URank.User.GetRankList(SaveScript.rankUuid_gold3, 100, 100 * i);
            else if (!isDoneSet2) // 영웅 유저
                BRO = Backend.URank.User.GetRankList(SaveScript.rankUuid_gold2, 100, 100 * i);

            if (!isDoneSet2 && !isDoneSet3 && !isDoneSet4)
            {
                // 전설 유저 탐색
                if (BRO.Rows().Count == 0) isEndSearch4 = true;
                for (int j = 0; j < BRO.Rows().Count; j++)
                {
                    if (long.Parse(BRO.Rows()[j]["score"]["N"].ToString()) > 0)
                    {
                        userNum_4++;
                        BROList_4.Add(BRO.GetReturnValuetoJSON()["rows"][j]);
                        if (userNum_4 + userNum_3 + userNum_2 >= 1000)
                            break;
                    }
                    else
                    {
                        isEndSearch4 = true;
                        break;
                    }
                }
            }
            else if (!isDoneSet2 && !isDoneSet3)
            {
                // 신화 유저 탐색
                if (BRO.Rows().Count == 0) isEndSearch3 = true;
                for (int j = 0; j < BRO.Rows().Count; j++)
                {
                    if (long.Parse(BRO.Rows()[j]["score"]["N"].ToString()) > 0)
                    {
                        userNum_3++;
                        BROList_3.Add(BRO.GetReturnValuetoJSON()["rows"][j]);
                        if (userNum_4 + userNum_3 + userNum_2 >= 1000)
                            break;
                    }
                    else
                    {
                        isEndSearch3 = true;
                        break;
                    }
                }
            }
            else if (!isDoneSet2)
            {
                // 영웅 유저 탐색
                if (BRO.Rows().Count == 0) isEndSearch2 = true;
                for (int j = 0; j < BRO.Rows().Count; j++)
                {
                    if (long.Parse(BRO.Rows()[j]["score"]["N"].ToString()) > 0)
                    {
                        userNum_2++;
                        BROList_2.Add(BRO.GetReturnValuetoJSON()["rows"][j]);
                        if (userNum_4 + userNum_3 + userNum_2 >= 1000)
                            break;
                    }
                    else
                    {
                        isEndSearch2 = true;
                        break;
                    }
                }
            }

            if (userNum_4 + userNum_3 + userNum_2 >= 1000)
                break;

            i++;
            if (isEndSearch4)
            {
                isEndSearch4 = false;
                isDoneSet4 = true;
                i = 0;
            }
            if (isEndSearch3)
            {
                isEndSearch3 = false;
                isDoneSet3 = true;
                i = 0;
            }
            if (isEndSearch2)
            {
                isEndSearch2 = false;
                isDoneSet2 = true;
                i = 0;
            }
        }

        SetRankList();
    }

    // 게임 정보 설정
    public RankData SetRankData(JsonData user_gold, int gold_type, JsonData tableData, int rank)
    {
        RankData data = new RankData();

        data.nickname = user_gold["nickname"]["S"].ToString();
        data.rank = rank;
        if (gold_type == 3)
            data.gold4 = long.Parse(user_gold["score"]["N"].ToString());
        else if (gold_type == 2)
            data.gold3 = long.Parse(user_gold["score"]["N"].ToString());
        else if (gold_type == 1)
            data.gold2 = long.Parse(user_gold["score"]["N"].ToString());
        else
            data.gold = long.Parse(user_gold["score"]["N"].ToString());

        data.owner_indate = tableData["owner_inDate"]["S"].ToString();
        data.equipments = new int[SaveScript.accessoryNum];
        data.equipments[0] = int.Parse(tableData["equipPick"]["N"].ToString());
        data.equipments[1] = int.Parse(tableData["equipHat"]["N"].ToString());
        data.equipments[2] = int.Parse(tableData["equipRing"]["N"].ToString());
        data.equipments[3] = int.Parse(tableData["equipPendant"]["N"].ToString());
        if (tableData.ContainsKey("equipSword")) data.equipments[4] = int.Parse(tableData["equipSword"]["N"].ToString());
        else data.equipments[4] = 0;

        data.equipmentUpgrades = new int[SaveScript.accessoryNum];
        data.equipmentUpgrades[0] = int.Parse(tableData["pickReinforces"]["L"][data.equipments[0]][0].ToString());
        if (data.equipments[1] != -1)
            data.equipmentUpgrades[1] = int.Parse(tableData["hatReinforces"]["L"][data.equipments[1]][0].ToString());
        if (data.equipments[2] != -1)
            data.equipmentUpgrades[2] = int.Parse(tableData["ringReinforces"]["L"][data.equipments[2]][0].ToString());
        if (data.equipments[3] != -1)
            data.equipmentUpgrades[3] = int.Parse(tableData["pendantReinforces"]["L"][data.equipments[3]][0].ToString());
        if (data.equipments[4] != -1)
        {
            if (tableData.ContainsKey("swordReinforces")) data.equipmentUpgrades[4] = int.Parse(tableData["swordReinforces"]["L"][data.equipments[4]][0].ToString());
            else data.equipmentUpgrades[4] = 0;
        }

        data.equipmentQualities = new int[SaveScript.accessoryNum];
        for (int i = 0; i < data.equipmentQualities.Length; i++)
        {
            int quality = GameFuction.GetQualityOfEquipment(data.equipmentUpgrades[i]);
            data.equipmentQualities[i] = quality;
        }
        if (tableData.ContainsKey("tier_level")) data.tier_level = int.Parse(tableData["tier_level"]["N"].ToString());
        else data.tier_level = 0;
        if (tableData.ContainsKey("tier_achievement")) data.tier_achievement = int.Parse(tableData["tier_achievement"]["N"].ToString());
        else data.tier_achievement = 0;

        return data;
    }

    public void SetRankList()
    {
        BROList_4.Sort(delegate (JsonData A, JsonData B)
        {
            if (long.Parse(A["score"]["N"].ToString()) > long.Parse(B["score"]["N"].ToString()))
                return -1;
            else if (long.Parse(A["score"]["N"].ToString()) < long.Parse(B["score"]["N"].ToString()))
                return 1;
            else
                return B["gamerInDate"]["S"].ToString().CompareTo(A["gamerInDate"]["S"].ToString());
        });
        BROList_3.Sort(delegate (JsonData A, JsonData B)
        {
            if (long.Parse(A["score"]["N"].ToString()) > long.Parse(B["score"]["N"].ToString()))
                return -1;
            else if (long.Parse(A["score"]["N"].ToString()) < long.Parse(B["score"]["N"].ToString()))
                return 1;
            else
                return B["gamerInDate"]["S"].ToString().CompareTo(A["gamerInDate"]["S"].ToString());
        });
        BROList_2.Sort(delegate (JsonData A, JsonData B)
        {
            if (long.Parse(A["score"]["N"].ToString()) > long.Parse(B["score"]["N"].ToString()))
                return -1;
            else if (long.Parse(A["score"]["N"].ToString()) < long.Parse(B["score"]["N"].ToString()))
                return 1;
            else
                return B["gamerInDate"]["S"].ToString().CompareTo(A["gamerInDate"]["S"].ToString());
        });



        BRO2List_4.Sort(delegate (JsonData A, JsonData B)
        {
            if (long.Parse(A["gold4"]["N"].ToString()) > long.Parse(B["gold4"]["N"].ToString()))
                return -1;
            else if (long.Parse(A["gold4"]["N"].ToString()) < long.Parse(B["gold4"]["N"].ToString()))
                return 1;
            else
                return B["owner_inDate"]["S"].ToString().CompareTo(A["owner_inDate"]["S"].ToString());
        });
        BRO2List_3.Sort(delegate (JsonData A, JsonData B)
        {
            if (long.Parse(A["gold3"]["N"].ToString()) > long.Parse(B["gold3"]["N"].ToString()))
                return -1;
            else if (long.Parse(A["gold3"]["N"].ToString()) < long.Parse(B["gold3"]["N"].ToString()))
                return 1;
            else
                return B["owner_inDate"]["S"].ToString().CompareTo(A["owner_inDate"]["S"].ToString());
        });
        BRO2List_2.Sort(delegate (JsonData A, JsonData B)
        {
            if (long.Parse(A["gold2"]["N"].ToString()) > long.Parse(B["gold2"]["N"].ToString()))
                return -1;
            else if (long.Parse(A["gold2"]["N"].ToString()) < long.Parse(B["gold2"]["N"].ToString()))
                return 1;
            else
                return B["owner_inDate"]["S"].ToString().CompareTo(A["owner_inDate"]["S"].ToString());
        });

        Debug.Log(BROList_4.Count + " / " + BROList_3.Count + " / " + BROList_2.Count + " ||| " + BRO2List_4.Count + " / " + BRO2List_3.Count + " / " + BRO2List_2.Count);

        // 어긋나는 데이터가 있는지 체크
        int count = Mathf.Min(BROList_2.Count, BRO2List_2.Count);
        for (int i = 0; i < count; i++)
        {
            if (BROList_2[i]["score"]["N"].ToString() != BRO2List_2[i]["gold2"]["N"].ToString())
                Debug.Log(i + "번째 어긋남, gold2 : " + BROList_2[i]["score"]["N"].ToString() + " / " + BRO2List_2[i]["gold2"]["N"].ToString());
        }

        // 데이터가 불완정한지 체크
        if (BROList_4.Count != BRO2List_4.Count || BROList_3.Count != BRO2List_3.Count || BROList_2.Count != BRO2List_2.Count)
        {
            Debug.LogError("랭킹 데이터 오류! 현재 잘못된 데이터가 포함되어 있으니 뒤끝 서버에서 데이터를 처리해주세요!");
            isError = true;
            isDone = true;
            return;
        }

        for (int i = 0; i < BROList_4.Count; i++)
            rankData.Add(SetRankData(BROList_4[i], 3, BRO2List_4[i], i + 1));
        for (int i = 0; i < BROList_3.Count; i++)
            rankData.Add(SetRankData(BROList_3[i], 2, BRO2List_3[i], i + userNum_4 + 1));
        for (int i = 0; i < BROList_2.Count; i++)
            rankData.Add(SetRankData(BROList_2[i], 1, BRO2List_2[i], i + userNum_4 + userNum_3 + 1));

        // 나의 데이터 설정
        int myRank = SaveScript.saveRank.GetRankIndex(BackEndLoginManager.userData.owner_indate, 1);
        if (myRank == NO_RANK)
        {
            // 랭킹 없음 (gold0 만 존재)
            where.Clear();
            BRO = Backend.GameData.GetMyData("publicData", where, 1);
            BRO2 = Backend.URank.User.GetMyRank(SaveScript.rankUuid_gold);
            myRankData = SetRankData(BRO2.GetReturnValuetoJSON()["rows"][0], 0, BRO.GetReturnValuetoJSON()["rows"][0], NO_RANK);
        }
        else
        {
            // 랭킹 있음
            myRankData = rankData[myRank]; 
        }

        isError = false;
        isDone = true;
        totalUserNum = userNum_4 + userNum_3 + userNum_2;
        Debug.Log("모든 유저 :" + totalUserNum + " / 전설 : " + userNum_4 + " / 신화 : " + userNum_3 + " / 영웅 : " + userNum_2);

        Chat.instance.SetUserRankList();
        Chat.instance.SetChatStatus();
        Chat.instance.SetBlockedUser();
        ChatUI.instance.SetInit();
        MainRankUI.instance.SetMyMainRank();
    }

    public int GetRankIndex(string str, int type)
    {
        int index;

        if (type == 0) // 닉네임으로 찾기
        {
            for (int i = 0; i < Chat.instance.systemChecks.Length; i++)
                if (str == Chat.instance.systemChecks[i])
                    return -100;
            index = rankData.FindIndex(x => x.nickname == str);
        }
        else // owner_indate로 찾기
            index = rankData.FindIndex(x => x.owner_indate == str);
        if (index == -1) index = NO_RANK;

        return index;
    }

    public void StartSetRankData()
    {
        if (!Backend.IsInitialized) return;
        isDoneSet4 = isDoneSet3 = isDoneSet2 = isDone = false;
        totalUserNum = userNum_2 = userNum_3 = userNum_4 = 0;
        key = "";
        rankData.Clear();
        BROList_4.Clear();
        BROList_3.Clear();
        BROList_2.Clear();
        BRO2List_4.Clear();
        BRO2List_3.Clear();
        BRO2List_2.Clear();
        StartCoroutine(SetRankData());
    }
}
