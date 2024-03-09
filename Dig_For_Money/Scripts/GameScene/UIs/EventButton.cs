using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    static public EventButton instance;

    public new AudioSource audio;
    public Image image;

    static public int mainType; // 현재 클릭한 EventBlock의 MainType
    static public int subType; // 현재 클릭한 EventBlock의 SubType
    static public RoomData roomData; // 현재 클릭한 EventBlock의 roomData
    static public Vector3 portal_vec;
    static public bool isNotUse;
    private int storyType; // 스토리 진행 방식
    private int dungeon_0_libraryNum;
    private int exp;
    private int exp_type;
    public bool isCtrl;

    void Awake()
    {
        instance = this;

        audio.mute = !SaveScript.saveData.isSEOn;
        dungeon_0_libraryNum = SaveScript.reinforceItemNum + SaveScript.reinforceItem2Num + 4;
        isCtrl = true;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (isCtrl && !isNotUse && BlindScript.isEndChange && !PlayerScript.instance.isEnd && PlayerScript.instance.isCanCtrl && PlayerScript.instance.isEventOn)
        {
            if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) AutoPlayCtrl.instance.SetInit();
            OpenEventBox();
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (!isNotUse && BlindScript.isEndChange && !PlayerScript.instance.isEnd && PlayerScript.instance.isCanCtrl && PlayerScript.instance.isEventOn)
        {
            if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) AutoPlayCtrl.instance.SetInit();
        }
    }

    public void SetButtonEnable(bool isEnable)
    {
        isCtrl = isEnable;
        image.color = (isEnable) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
        image.raycastTarget = isEnable;
    }

    public void OpenEventBox()
    {
        isNotUse = true;

        switch (mainType)
        {
            case 0: // 유적 던전 들어가기
                if (PlayerScript.instance.rigidbody.velocity.y == 0f)
                {
                    EventUI.instance.eventText.text = "유적 던전으로 들어가시겠습니까?";
                    EventUI.instance.eventBox.SetActive(true);
                }
                else
                    ResetEvent();
                break;
            case 1: // 유적 던전 나가기
                if (PlayerScript.instance.rigidbody.velocity.y == 0f)
                {
                    EventUI.instance.eventText.text = "유적 던전에서 나가시겠습니까?\n" + "[ 한번 나가면 던전 입구는 폐쇄됩니다. ]";
                    EventUI.instance.eventBox.SetActive(true);
                }
                else
                    ResetEvent();
                break;
            case 2: // 아래층 내려가기
                if (PlayerScript.instance.rigidbody.velocity.y == 0f)
                    StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.dungeon_0_portalPos + new Vector3Int(-3, -7, 0), 1f, 0.5f));
                else
                    ResetEvent();
                break;
            case 3: // 위층 올라가기
                if (PlayerScript.instance.rigidbody.velocity.y == 0f)
                    StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.dungeon_0_portalPos + new Vector3Int(4, 7, 0), 1f, 0.5f));
                else
                    ResetEvent();
                break;
            case 4: // 도서관 조사
                float plusPercent = (1f + SaveScript.stat.library) * (1f + SaveScript.libraryPercentsAsType[DungeonCreater.dungeon_0_type]);
                AchievementCtrl.instance.SetAchievementAmount(15, 1);
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 21 });
                QuestCtrl.instance.SetSubQuestAmount(10);
                EventUI.instance.eventBox.SetActive(true);
                EventUI.instance.eventNextButton.SetActive(true);
                EventUI.instance.eventNoButton.SetActive(false);
                EventUI.instance.eventYesButton.SetActive(false);
                Time.timeScale = 0f;
                storyType = -1;

                if (GameFuction.GetRandFlag(0.005f))
                    storyType = dungeon_0_libraryNum - 1;
                if (GameFuction.GetRandFlag(0.01f))
                    storyType = dungeon_0_libraryNum - 2;
                if (GameFuction.GetRandFlag(0.025f))
                    storyType = dungeon_0_libraryNum - 3;
                if (GameFuction.GetRandFlag(0.175f))
                    storyType = dungeon_0_libraryNum - 4;

                if (storyType == -1)
                {
                    if (DungeonCreater.dungeon_0_type > 3)
                    {
                        // 7층 이상 (제련석 & 유물)
                        if (DungeonCreater.dungeon_0_type == 4 && GameFuction.GetRandFlag(0.5f))
                            storyType = Random.Range(SaveScript.reinforceItemNum - 2, SaveScript.reinforceItemNum);
                        else
                            storyType = SaveScript.reinforceItemNum + GameFuction.GetRandFlag(GameFuction.GetArrayPlus(plusPercent, ReinforceItem2.dropPercents));
                    }
                    else
                    {
                        // 3 ~ 6층 (하급 ~ 고급 주문서)
                        for (int i = 0; i < ReinforceItem.rewardOrderCode.Length; i++)
                        {
                            if (GameFuction.GetRandFlag(ReinforceItem.dropPercents[ReinforceItem.rewardOrderCode[i]] * plusPercent))
                            {
                                storyType = ReinforceItem.rewardOrderCode[i];
                                if (storyType == 2 && GameFuction.GetRandFlag(0.5f))
                                    storyType = 5;
                                break;
                            }
                        }
                    }
                }

                if (storyType == -1)
                    storyType = dungeon_0_libraryNum - 4;
                EventNextButton();
                break;
            case 5: // 특수 던전 NPC 상인 
                D_0_NPCUICtrl.instance.InitNPCShop(EventBlock.currentEventBlock.GetComponentInParent<Dungeon_0_NPC>());
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 22 });
                QuestCtrl.instance.SetSubQuestAmount(11);
                break;
            case 6: // 특수 던전 맵 끝 포탈
                if (PlayerScript.instance.rigidbody.velocity.y == 0f)
                    StartCoroutine(BlindScript.instance.switchPos(EventBlock.currentEventBlock.portal_vec, 1f, 0.5f));
                else
                    ResetEvent();
                break;
            case 7: // 이벤트 맵 포탈
                if (Mathf.Abs(PlayerScript.instance.rigidbody.velocity.y) == 0f)
                {
                    EventUI.instance.eventText.text = "이벤트 맵에 입장하시겠습니까?";
                    EventUI.instance.eventBox.SetActive(true);
                }
                else
                    ResetEvent();
                break;
            case 9: // 고대 던전 입장
                if (PlayerScript.instance.rigidbody.velocity.y == 0f)
                {
                    EventUI.instance.eventText.text = "고대 던전으로 들어가시겠습니까?";
                    EventUI.instance.eventBox.SetActive(true);
                }
                else
                    ResetEvent();
                break;
            case 10: // 고대 던전 나가기
                if (TorchUICtrl.instance.isOn && PlayerScript.instance.rigidbody.velocity.y == 0f)
                {
                    EventUI.instance.eventText.text = "고대 던전에서 나가시겠습니까?\n" + "[ 한번 나가면 던전 입구는 폐쇄됩니다. ]";
                    EventUI.instance.eventBox.SetActive(true);
                }
                else
                    ResetEvent();
                break;
            case 11: // 고대 던전 방 들어가기
                if (TorchUICtrl.instance.isOn && PlayerScript.instance.rigidbody.velocity.y == 0f)
                {
                    // 방 생성
                    StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.instance.dungeon_1_room_startVec + new Vector3Int(3, -11, 0), 1f, 0.5f));
                    DungeonCreater.dungeon_1_roomData = roomData;
                    DungeonCreater.instance.Dungeon_1_CreateRoom(roomData);
                }
                else
                    ResetEvent();
                break;
            case 12: // 고대 던전 방 나가기
                if (TorchUICtrl.instance.isOn && PlayerScript.instance.rigidbody.velocity.y == 0f)
                {
                    StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.dungeon_1_roomData.pos, 1f, 0.5f));
                    if (DungeonCreater.dungeon_1_roomData.type != 3)
                    {
                        MapData.instance.GetTileMap(DungeonCreater.dungeon_1_roomData.pos, 5).SetTile(DungeonCreater.dungeon_1_roomData.pos, 
                            MapData.instance.dungeon_1_DecoX64Tiles[10 + 7 * DungeonCreater.dungeon_1_type]);
                        ObjectPool.ReturnObject<EventBlock>(12, DungeonCreater.dungeon_1_roomData.eventBlock);
                    }
                }
                else
                    ResetEvent();
                break;
            case 13: // 다음 층으로 이동하기
                if (TorchUICtrl.instance.isOn && PlayerScript.instance.rigidbody.velocity.y == 0f)
                {
                    StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.instance.dungeon_1_bodyStartVec 
                        + new Vector3Int(3, -(2 + 12 * ++PlayerScript.instance.d_1_currentFloor), 0), 1f, 0.5f));
                }
                else
                    ResetEvent();
                break;
            case 14: // 고대 던전 횟불 클릭
                PlayerScript.instance.d_1_currentTorch.GetTorch();
                StartCoroutine(TorchUICtrl.instance.AddTime());
                ResetEvent();
                break;
            case 15: // 고대 던전 다시 1층으로 이동
                if (TorchUICtrl.instance.isOn && Mathf.Abs(PlayerScript.instance.rigidbody.velocity.y) == 0f)
                {
                    EventUI.instance.eventText.text = "1층으로 이동하시겠습니까?\n(던전을 나가기 위해선 앞쪽 포탈을 이용해주세요!)";
                    EventUI.instance.eventBox.SetActive(true);
                }
                else
                    ResetEvent();
                break;
        }
    }

    public void EventYesButton()
    {
        audio.clip = SaveScript.SEs[0];
        audio.Play();

        switch (mainType)
        {
            case 0: // 유적 던전 입장
                if (PlayerScript.instance.rigidbody.velocity.y != 0f) return;
                EventUI.instance.eventBox.SetActive(false);
                AchievementCtrl.instance.SetAchievementAmount(13, 1);
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 18 });
                DungeonCreater.instance.CreateDungeon_0_Body(DungeonCreater.dungeon_0_type);
                MapData.instance.GetTileMap(DungeonCreater.dungeon_0_startPos, 5).SetTile(DungeonCreater.dungeon_0_startPos, MapData.instance.dungeon_0_DecoX64Tiles[42 + DungeonCreater.dungeon_0_type * 2]); // 열린 문 설치
                StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.instance.dungeon_0_bodyStartVec + Vector3Int.down * 4, 4, 1.5f, 1.5f));
                break;
            case 1: // 유적 던전 나가기
                if (PlayerScript.instance.rigidbody.velocity.y != 0f) return;
                EventUI.instance.eventBox.SetActive(false);
                StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.dungeon_0_startPos, BlindScript.instance.GetStageBGM(), 1.5f, 1.5f));
                break;
            case 7: // 이벤트 맵 입장
                EventUI.instance.eventBox.SetActive(false);
                EventMap.instance.CreateMap();
                EventMap.instance.StopDelete();
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 23 });
                QuestCtrl.instance.SetSubQuestAmount(7);
                StartCoroutine(BlindScript.instance.switchPos(EventMap.instance.bodyStartVec, 7, 1.5f, 1.5f));
                break;
            case 9: // 고대 던전 입장
                if (PlayerScript.instance.rigidbody.velocity.y != 0f) return;
                CashEquipmentCtrl.instance.eventBlocks.Remove(EventBlock.currentEventBlock);
                EventUI.instance.eventBox.SetActive(false);
                AchievementCtrl.instance.SetAchievementAmount(14, 1);
                QuestCtrl.instance.SetMainQuestAmount(new int[] { 53 });
                DungeonCreater.instance.CreateDungeon_1_Body(DungeonCreater.dungeon_1_type);
                StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.instance.dungeon_1_bodyStartVec + new Vector3Int(3, -2, 0), 9, 1.5f, 1.5f));
                break;
            case 10: // 고대 던전 나가기
                if (PlayerScript.instance.rigidbody.velocity.y != 0f) return;
                EventUI.instance.eventBox.SetActive(false);
                StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.dungeon_1_startPos, BlindScript.instance.GetStageBGM(), 1.5f, 1.5f));
                break;
            case 15: // 고대 던전 1층으로 다시 이동
                EventUI.instance.eventBox.SetActive(false);
                StartCoroutine(BlindScript.instance.switchPos(DungeonCreater.instance.dungeon_1_bodyStartVec + new Vector3Int(3, -2, 0), 1f, 0.5f));
                break;
        }
    }

    public void EventNoButton()
    {
        audio.clip = SaveScript.SEs[0];
        audio.Play();
        ResetEvent();
    }

    public void ResetEvent()
    {
        PlayerScript.instance.isEventOn = false;
        mainType = subType = -1;
        isNotUse = false;
        EventUI.instance.eventBox.SetActive(false);
        Time.timeScale = 1f;
    }

    public void EventNextButton()
    {
        audio.clip = SaveScript.SEs[0];
        audio.Play();
        exp = -1;

        switch (mainType)
        {
            case 4: // 도서관 조사하기
                switch (storyType)
                {
                    case 0: EventUI.instance.eventText.text = "'초록 원'이 그려진 낡은 양피지가 책 사이에 끼어있다.\n"; break;
                    case 1: EventUI.instance.eventText.text = "'푸른 원'이 그려진 낡은 양피지가 책 사이에 끼어있다.\n"; break;
                    case 2: EventUI.instance.eventText.text = "'붉은 원'이 그려진 낡은 양피지가 책 사이에 끼어있다.\n"; break;
                    case 3: EventUI.instance.eventText.text = "'검은 파편'이 세겨진 낡은 양피지가 책 사이에 끼어있다.\n"; break;
                    case 4: EventUI.instance.eventText.text = "'검은 화살표'가 세겨진 낡은 양피지가 책 사이에 끼어있다.\n"; break;
                    case 5: EventUI.instance.eventText.text = "'하얀 천사'가 세겨진 검은 양피지가 책 사이에 끼어있다.\n"; break;
                    case 6: EventUI.instance.eventText.text = "'푸른 화살표'가 세겨진 고급 양피지가 책 사이에 끼어있다.\n"; break;
                    case 7: EventUI.instance.eventText.text = "'붉은 화살표'가 세겨진 고급 양피지가 책 사이에 끼어있다.\n"; break;
                    case 8: EventUI.instance.eventText.text = "'생명의 힘'이 느껴지는 신비한 돌이 놓여있다.\n"; break;
                    case 9: EventUI.instance.eventText.text = "'바다의 힘'이 느껴지는 신비한 돌이 놓여있다.\n"; break;
                    case 10: EventUI.instance.eventText.text = "'불의 힘'이 느껴지는 신비한 돌이 놓여있다.\n"; break;
                    case 11: EventUI.instance.eventText.text = "'따뜻한 기운'이 느껴지는 황홀한 유물이 놓여있다.\n"; break;
                    case 12: EventUI.instance.eventText.text = "'차가운 기운'이 느껴지는 황홀한 유물이 놓여있다.\n"; break;
                    case 13: EventUI.instance.eventText.text = "'신비로운 기운'이 느껴지는 황홀한 유물이 놓여있다.\n"; break;
                    case 14: EventUI.instance.eventText.text = "'특수 던전의 기원'이라는 책이 있다.\n"; break;
                    case 15: EventUI.instance.eventText.text = "'어느 광부의 슬라임 친구'라는 책이 있다.\n"; break;
                    case 16: EventUI.instance.eventText.text = "'붉은 지옥의 메아리'이라는 책이 있다.\n"; break;
                    case 17: EventUI.instance.eventText.text = "'초록 유적의 수호자'라는 책이 있다.\n"; break;
                }

                storyType += dungeon_0_libraryNum;

                switch (storyType)
                {
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        EventUI.instance.eventItemInfo.SetActive(true);
                        EventUI.instance.eventItemImage.sprite = SaveScript.reinforceItems[storyType - dungeon_0_libraryNum].sprite;

                        if (GameFuction.GetRandFlag(GameFuction.GetCollectionUpgradeForce(14)))
                        {
                            EventUI.instance.eventText.text += "<color=#AA2424>< " + SaveScript.reinforceItems[storyType - dungeon_0_libraryNum].name 
                                + " > <color=#784600>을 얻었습니다. <color=#2424AA>(추가 획득 성공!)";
                            SaveScript.saveData.hasReinforceItems[storyType - dungeon_0_libraryNum] += 2;
                            AchievementCtrl.instance.SetAchievementAmount(18, 2);
                        }
                        else
                        {
                            EventUI.instance.eventText.text += "<color=#AA2424>< " + SaveScript.reinforceItems[storyType - dungeon_0_libraryNum].name 
                                + " > <color=#784600>을 얻었습니다.";
                            SaveScript.saveData.hasReinforceItems[storyType - dungeon_0_libraryNum] += 1;
                            AchievementCtrl.instance.SetAchievementAmount(18, 1);
                        }
                        break;
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                        EventUI.instance.eventItemInfo.SetActive(true);
                        EventUI.instance.eventItemImage.sprite = SaveScript.reinforceItems2[storyType - (dungeon_0_libraryNum + SaveScript.reinforceItemNum)].sprite;

                        if (GameFuction.GetRandFlag(GameFuction.GetCollectionUpgradeForce(14)))
                        {
                            EventUI.instance.eventText.text += "<color=#AA2424>< " + SaveScript.reinforceItems2[storyType - (dungeon_0_libraryNum + SaveScript.reinforceItemNum)].name
                                + " > <color=#784600>을 얻었습니다. <color=#2424AA>(추가 획득 성공!)";
                            SaveScript.saveData.hasReinforceItems2[storyType - (dungeon_0_libraryNum + SaveScript.reinforceItemNum)] += 2;
                            AchievementCtrl.instance.SetAchievementAmount(18, 2);
                        }
                        else
                        {
                            EventUI.instance.eventText.text += "<color=#AA2424>< " + SaveScript.reinforceItems2[storyType - (dungeon_0_libraryNum + SaveScript.reinforceItemNum)].name
                                + " > <color=#784600>을 얻었습니다.";
                            SaveScript.saveData.hasReinforceItems2[storyType - (dungeon_0_libraryNum + SaveScript.reinforceItemNum)] += 1;
                            AchievementCtrl.instance.SetAchievementAmount(18, 1);
                        }
                        break;
                    case 32: exp = GameFuction.GetRealExp(30 + 15 * DungeonCreater.dungeon_0_type, out exp_type); break;
                    case 33: exp = GameFuction.GetRealExp(100 + 50 * DungeonCreater.dungeon_0_type, out exp_type); break;
                    case 34: exp = GameFuction.GetRealExp(300 + 150 * DungeonCreater.dungeon_0_type, out exp_type); break;
                    case 35: exp = GameFuction.GetRealExp(1000 + 500 * DungeonCreater.dungeon_0_type, out exp_type); break;
                    default:
                        EventUI.instance.eventBox.SetActive(false);
                        EventUI.instance.eventNextButton.SetActive(false);
                        EventUI.instance.eventNoButton.SetActive(true);
                        EventUI.instance.eventYesButton.SetActive(true);
                        EventUI.instance.eventItemInfo.SetActive(false);

                        MapData.instance.GetTileMap(Vector3Int.RoundToInt(EventBlock.currentEventBlock.transform.position), 10).SetTile(new Vector3Int((int)EventBlock.currentEventBlock.transform.position.x,
                            (int)EventBlock.currentEventBlock.transform.position.y, (int)EventBlock.currentEventBlock.transform.position.z), null);

                        ObjectPool.ReturnObject<EventBlock>(12, EventBlock.currentEventBlock.GetComponent<EventBlock>());
                        audio.clip = SaveScript.SEs[8];
                        audio.Play();
                        ResetEvent();
                        break;
                }
                break;
        }

        if (exp != -1)
        {
            switch (exp_type)
            {
                case 0: EventUI.instance.eventText.text += "EXP를 <color=#327D32>< " + GameFuction.GetNumText(exp) + " > <color=#784600>획득하였습니다."; break;
                case 1: EventUI.instance.eventText.text += "EXP를 <color=#324B7D>< " + GameFuction.GetNumText(exp) + "(x2) > <color=#784600>획득하였습니다."; break;
                case 2: EventUI.instance.eventText.text += "EXP를 <color=#7D3264>< " + GameFuction.GetNumText(exp) + "(x4) > <color=#784600>획득하였습니다."; break;
            }
            PrintUI.instance.ExpInfo(exp, exp_type);
            exp = -1;
        }
    }
}