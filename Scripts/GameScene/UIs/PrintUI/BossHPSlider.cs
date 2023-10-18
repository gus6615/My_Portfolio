using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPSlider : MonoBehaviour
{
    static public BossHPSlider instance;
    static public Monster_Boss currentBoss;
    static public Block block;
    static public BreakObject currentBreakObject;

    public Slider slider;
    public Text hpText;
    public bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isActive)
        {
            if ((currentBoss != null && Vector3.Distance(currentBoss.transform.position, PlayerScript.instance.transform.position) > 10f) ||
                (currentBreakObject != null && Vector3.Distance(currentBreakObject.transform.position, PlayerScript.instance.transform.position) > 10f))
                CloseHPSlider();
        }
    }

    public void SetHPSlider(Monster_Boss boss)
    {
        this.gameObject.SetActive(true);
        SetDefaultObject();
        currentBoss = boss;
        isActive = true;
        SetHP(boss);
    }

    public void SetHPSlider(Block _block)
    {
        this.gameObject.SetActive(true);
        SetDefaultObject();
        block = _block;
        isActive = true;
        SetHP(_block);
    }

    public void SetHPSlider(BreakObject _breakObject)
    {
        this.gameObject.SetActive(true);
        SetDefaultObject();
        currentBreakObject = _breakObject;
        isActive = true;
        SetHP(_breakObject);
    }

    public void SetHP(Monster_Boss boss)
    {
        slider.maxValue = boss.maxHP;
        slider.value = boss.HP;
        hpText.text = GameFuction.GetNumText(boss.HP) + " / " + GameFuction.GetNumText(boss.maxHP);
        if (boss.HP <= 0f) CloseHPSlider();
    }

    public void SetHP(Block _block)
    {
        slider.maxValue =_block.maxHP;
        slider.value = _block.HP;
        hpText.text = GameFuction.GetNumText(_block.HP) + " / " + GameFuction.GetNumText(_block.maxHP);
        if (_block.HP <= 0f) CloseHPSlider();
    }

    public void SetHP(BreakObject _breakObject)
    {
        slider.maxValue = _breakObject.maxHP;
        slider.value = _breakObject.HP;
        hpText.text = GameFuction.GetNumText(_breakObject.HP) + " / " + GameFuction.GetNumText(_breakObject.maxHP);
        if (_breakObject.HP <= 0f) CloseHPSlider();
    }

    public void CloseHPSlider()
    {
        this.gameObject.SetActive(false);
        isActive = false;
        SetDefaultObject();
    }

    private void SetDefaultObject()
    {
        currentBoss = null;
        currentBreakObject = null;
    }
}
