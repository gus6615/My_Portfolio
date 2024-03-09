using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public enum RULLET_TYPE
{
    RULLET,
    VENDING_MACNINE
}

public class Rullet : MonoBehaviour
{
    [SerializeField] private RULLET_TYPE type;
    public GameObject rollImage;
    private Order[] elements;
    private int[][] weights = new int[][]
    {
        new int[] { 1, 1, 1, 1, 1, 1, 1, 1 },
        new int[] { 2, 1, 2, 3, 1, 1, 2, 3, 1}
    };
    private int total_weight;
    private float radius_standard;

    public bool isStart;
    private bool isStartCorutine; // 룰렛이 돌아가기 시작
    private bool isStop; // 룰렛이 돌아가다 멈추기 시작
    public bool isEnd; // 룰렛이 멈춤
    private float rollForce; // 룰렛이 돌아가는 정도
    public int selectedOrder, selectedOrder2; // 룰렛이 선택한 요소의 index

    // Start is called before the first frame update
    void Start()
    {
        elements = rollImage.GetComponentsInChildren<Order>();
        total_weight = weights[(int)type].Sum();
        radius_standard = 360f / total_weight;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            if (!isStartCorutine)
                StartCoroutine(StartRullet());
            StartRoll();
        }
        else if (isStop)
        {
            StopRoll();
        }
    }

    IEnumerator StartRullet()
    {
        isStartCorutine = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
        isStart = false;
        isStop = true;
    }

    public void StartRoll()
    {
        rollForce = 360f * Time.deltaTime;

        rollImage.transform.Rotate(new Vector3(0, 0, -rollForce));
    }

    public void StopRoll()
    {
        rollForce *= 0.99f;
        if(rollForce < 0.05f)
        {
            rollForce = 0f;
            EndRoll();
        }

        rollImage.transform.Rotate(new Vector3(0, 0, -rollForce));
    }

    public void EndRoll()
    {
        float rullet_rotation = rollImage.transform.rotation.eulerAngles.z % 360f;
        float current_angle = 360f;
        int element_index = 0;

        for (int i = weights[(int)type].Length - 1; i >= 0; i--)
        {
            if (current_angle - weights[(int)type][i] * radius_standard < rullet_rotation && rullet_rotation <= current_angle)
            {
                element_index = i;
                break;
            }

            current_angle -= weights[(int)type][i] * radius_standard;
        }
        
        selectedOrder = elements[element_index].order;
        selectedOrder2 = elements[element_index].order2;

        isStop = false;
        isEnd = true;
        isStartCorutine = false;
    }
}
