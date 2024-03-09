using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MineSlime : MonoBehaviour
{
    protected const float MAX_X = 8f;
    static public Sprite[] miner_faceSprites;
    static public Sprite[] adventurer_faceSprites;
    static public Sprite[] miner_defaultSprites;
    static public Sprite[] adventurer_defaultSprites;
    static public Sprite[] rewardSprites;
    static public Color[] colors = { new Color(1f, 1f, 1f), new Color(0.7f, 1f, 1f), new Color(1f, 0.7f, 1f), new Color(1f, 0.5f, 0.5f)
            , new Color(0.5f, 1f, 0.5f), new Color(0.5f, 0.6f, 0.8f) }; // 흰색, 푸른색, 보라색, 붉은색, 초록색, 파란색
    static public string[] qualityNames = new string[] { "D", "C", "B", "A", "S", "SS", "SSS", "U", "UU", "UUU", "M" };
    static public float[][] fusionPercentAsCode = new float[][]
    {
        new float[] { 0f, 0.95f, 0.05f }, // D
        new float[] { 0.05f, 0.925f, 0.025f }, // C
        new float[] { 0.1f, 0.89f, 0.01f }, // B
        new float[] { 0.15f, 0.845f, 0.005f }, // A
        new float[] { 0.2f, 0.799f, 0.001f }, // S
        new float[] { 0.25f, 0.75f, 0f }, // SS
        new float[] { 0.65f, 0.35f, 0f }, // SSS
        new float[] { 0.85f, 0.15f, 0f }, // U
        new float[] { 0.95f, 0.05f, 0f }, // UU
        new float[] { 0.9f, 0.1f, 0f }, // UUU
        new float[] { 1f, 0f, 0f } // M
    };
    public static float[][] cashPercents = new float[][]
    {
        new float[] { 0f, 0.2f, 0.35f, 0.25f, 0.1f, 0.075f, 0.025f, 0f, 0f, 0f, 0f },
        new float[] { 0f, 0f, 0f, 0.35f, 0.25f, 0.2f, 0.125f, 0.05f, 0.025f, 0f, 0f },
        new float[] { 0f, 0f, 0f, 0f, 0.4f, 0.25f, 0.17f, 0.1f, 0.05f, 0.02f, 0.01f }
    };
    static public bool isInit = false;

    public Animator animator;
    public GameObject canRewardObject;

    public new string name;
    public int code;
    public int level;
    public long exp; 
    public int time; // 한 횟수를 행하는 시간
    public long amount; // 한 횟수에 얻는 아이템의 양
    public bool isDrag; // 현재 Drag 상태인가?
    public float dragTime;

    protected bool isChangeAni, isMove, isReward, isSelected;
    protected Vector3 moveVec;

    IEnumerator SetAni()
    {
        isChangeAni = false;
        if (Random.Range(1, 3) == 1)
        {
            isMove = true;
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
        else isMove = false;
        animator.SetBool("isMove", isMove);

        yield return new WaitForSeconds(Random.Range(2f, 3f));
        if(!isReward || !isSelected) isChangeAni = true;
    }

    public void StartReward()
    {
        isReward = true;
        isMove = false;
        isChangeAni = false;
        animator.SetBool("isReward", true);
        animator.SetBool("isMove", false);
    }

    public void EndReward()
    {
        isReward = false;
        animator.SetBool("isReward", false);
        if (!isSelected) isChangeAni = true;
    }

    public void SetSelectedPet(bool _isSelected)
    {
        isSelected = _isSelected;
        if (_isSelected)
        {
            isMove = false;
            animator.SetBool("isMove", false);
        }
        else
        {
            isChangeAni = true;
        }
    }

    public void SetRewardObject(bool _isOn)
    {
        canRewardObject.SetActive(_isOn);
    }

    public long GetExpAsLevel()
    {
        return GameFuction.GetAchievementAmount(2, level, 3, 100);
    }

    static public long GetExpAsLevel(int _level)
    {
        return GameFuction.GetAchievementAmount(2, _level, 3, 100);
    }

    static public Color GetColorByCode(int _code, out int _colorIndex)
    {
        int colorIndex;

        if (_code > 8) colorIndex = 5;
        else if (_code > 6) colorIndex = 4;
        else if (_code > 4) colorIndex = 3;
        else if (_code > 2) colorIndex = 2;
        else if (_code > 0) colorIndex = 1;
        else colorIndex = 0;
        _colorIndex = colorIndex;

        return colors[colorIndex];
    }

    static public Color GetColorByCode(int _code)
    {
        int colorIndex;

        if (_code > 8) colorIndex = 5;
        else if (_code > 6) colorIndex = 4;
        else if (_code > 4) colorIndex = 3;
        else if (_code > 2) colorIndex = 2;
        else if (_code > 0) colorIndex = 1;
        else colorIndex = 0;

        return colors[colorIndex];
    }
}
