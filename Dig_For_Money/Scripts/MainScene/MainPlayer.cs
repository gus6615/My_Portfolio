using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    static public MainPlayer instance;
    [SerializeField] private SpriteRenderer[] sprites;
    private Animator animator;

    private void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();
        sprites = GetComponentsInChildren<SpriteRenderer>();

        Init();
    }

    private void Update()
    {
        if (animator.GetInteger("GotoType") == 1)
        {
            transform.position += Vector3.right * Time.deltaTime * 1.5f;
        }
    }

    public void Init()
    {
        sprites[1].color = SaveScript.toolColors[SaveScript.saveData.equipPick];
        if (SaveScript.saveData.equipHat == -1)
            sprites[3].gameObject.SetActive(false);
        else
        {
            sprites[3].gameObject.SetActive(true);
            sprites[3].color = SaveScript.toolColors[SaveScript.saveData.equipHat];
        }

        if (SaveScript.saveData.equipRing == -1)
            sprites[2].gameObject.SetActive(false);
        else
        {
            sprites[2].gameObject.SetActive(true);
            sprites[2].color = SaveScript.toolColors[SaveScript.saveData.equipRing];
        }

        if (SaveScript.saveData.equipPendant == -1)
            sprites[4].gameObject.SetActive(false);
        else
        {
            sprites[4].gameObject.SetActive(true);
            sprites[4].color = SaveScript.toolColors[SaveScript.saveData.equipPendant];
        }
    }

    public void SetIdleAni() // Idle 애니메이션을 랜덤으로 지정
    {
        int randType = Random.Range(1, 4);
        animator.SetInteger("IdleType", randType);
    }

    public void CaneCleAni()
    {
        animator.SetInteger("IdleType", -1);
    }

    public void GotoGameScene()
    {
        MainScript.isGotoGameScene = true;
    }

    public void GotoShopScene()
    {
        MainScript.isGotoShopScene = true;
    }

    public void GotoUpgradeScene()
    {
        MainScript.isGotoUpgradeScene = true;
    }
}
