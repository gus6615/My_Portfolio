using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBlock : MonoBehaviour
{
    public const int ULITMATE_CODE = 100;
    public const int MYSTIC_CODE = 101;
    public const int ANCIENT_CODE = 9;
    static public EventBlock currentEventBlock; // 최근 이벤트 블럭
    
    public int eventMainType;
    public int eventSubType;
    public Vector3 portal_vec;
    public RoomData roomData;

    private void OnEnable()
    {
        eventMainType = eventSubType = 0;
        portal_vec = new Vector3(0, 10, 0);
    }

    public bool CheckIsOre()
    {
        return eventMainType == ULITMATE_CODE || eventMainType == MYSTIC_CODE;
    }

    public bool CheckIsAncient()
    {
        return eventMainType == ANCIENT_CODE;
    }

    public bool CheckIsCash()
    {
        return CheckIsOre() || CheckIsAncient();
    }
}
