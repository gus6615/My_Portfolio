using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashEquipmentCtrl : MonoBehaviour
{
    private const float DUMP_DISTANCE = 999f;
    private const float DETECT_DISTANCE = 20f;

    public static CashEquipmentCtrl instance;

    [SerializeField] private Animator ultimate_animator, mystic_animator, ancient_animator;
    public List<EventBlock> eventBlocks;

    EventBlock eventBlock;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveScript.saveData.isCashEquipmentOn[1])
            FindUltimate();
        else
            ultimate_animator.SetBool("isFind", false);

        if (SaveScript.saveData.isCashEquipmentOn[2])
            FindMystic();
        else
            mystic_animator.SetBool("isFind", false);

        if (SaveScript.saveData.isCashEquipmentOn[3])
            FindAncient();
        else
            ancient_animator.SetBool("isFind", false);
    }

    private void FindUltimate()
    {
        float nearest_dis = DUMP_DISTANCE;

        ultimate_animator.SetBool("isFind", false);
        ultimate_animator.transform.localPosition = new Vector3(-Mathf.Sign(PlayerScript.instance.transform.localScale.x) * 0.05f, 0f, 0f);
        for (int i = 0; i < eventBlocks.Count; i++)
        {
            if (eventBlocks[i].eventMainType == EventBlock.ULITMATE_CODE)
            {
                if (nearest_dis > Vector3.Distance(PlayerScript.instance.transform.position, eventBlocks[i].transform.position))
                {
                    eventBlock = eventBlocks[i];
                    nearest_dis = Vector3.Distance(PlayerScript.instance.transform.position, eventBlocks[i].transform.position);
                }
            }
        }

        if (nearest_dis <= DETECT_DISTANCE)
        {
            Vector3 dir = eventBlock.transform.position - PlayerScript.instance.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (Mathf.Sign(PlayerScript.instance.transform.localScale.x) < 0)
                angle += 180;
            ultimate_animator.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            ultimate_animator.speed = Mathf.Lerp(2f, 1f, nearest_dis / DETECT_DISTANCE);
            ultimate_animator.SetBool("isFind", true);
        }
    }

    private void FindMystic()
    {
        float nearest_dis = DUMP_DISTANCE;

        mystic_animator.SetBool("isFind", false);
        mystic_animator.transform.localPosition = new Vector3(-Mathf.Sign(PlayerScript.instance.transform.localScale.x) * 0.05f, 0f, 0f);
        for (int i = 0; i < eventBlocks.Count; i++)
        {
            if (eventBlocks[i].eventMainType == EventBlock.MYSTIC_CODE)
            {
                if (nearest_dis > Vector3.Distance(PlayerScript.instance.transform.position, eventBlocks[i].transform.position))
                {
                    eventBlock = eventBlocks[i];
                    nearest_dis = Vector3.Distance(PlayerScript.instance.transform.position, eventBlocks[i].transform.position);
                }
            }
        }

        if (nearest_dis <= DETECT_DISTANCE)
        {
            Vector3 dir = eventBlock.transform.position - PlayerScript.instance.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (Mathf.Sign(PlayerScript.instance.transform.localScale.x) < 0)
                angle += 180;
            mystic_animator.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            mystic_animator.speed = Mathf.Lerp(2f, 1f, nearest_dis / DETECT_DISTANCE);
            mystic_animator.SetBool("isFind", true);
        }
    }

    private void FindAncient()
    {
        float nearest_dis = DUMP_DISTANCE;

        ancient_animator.SetBool("isFind", false);
        ancient_animator.transform.localPosition = new Vector3(-Mathf.Sign(PlayerScript.instance.transform.localScale.x) * 0.05f, 0f, 0f);
        for (int i = 0; i < eventBlocks.Count; i++)
        {
            if (eventBlocks[i].eventMainType == EventBlock.ANCIENT_CODE)
            {
                if (nearest_dis > Vector3.Distance(PlayerScript.instance.transform.position, eventBlocks[i].transform.position))
                {
                    eventBlock = eventBlocks[i];
                    nearest_dis = Vector3.Distance(PlayerScript.instance.transform.position, eventBlocks[i].transform.position);
                }
            }
        }

        if (nearest_dis <= DETECT_DISTANCE)
        {
            Vector3 dir = eventBlock.transform.position - PlayerScript.instance.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (Mathf.Sign(PlayerScript.instance.transform.localScale.x) < 0)
                angle += 180;
            ancient_animator.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            ancient_animator.speed = Mathf.Lerp(2f, 1f, nearest_dis / DETECT_DISTANCE);
            ancient_animator.SetBool("isFind", true);
        }
    }
}
