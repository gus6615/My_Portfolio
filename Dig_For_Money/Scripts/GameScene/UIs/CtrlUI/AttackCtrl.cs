using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static AttackCtrl instance;

    public Image attackImage;
    private Color upColor, downColor;

    static public bool isAttackStart;
    public bool isButtonDown;
    public bool isCtrl;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        downColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        upColor = Color.white;
        isCtrl = true;
    }

    private void Update()
    {
        if (isButtonDown)
        {
            if (!PlayerScript.instance.isAttack && !isAttackStart)
                isAttackStart = true;
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) 
            AutoPlayCtrl.instance.SetInit();
        if (!isCtrl || PlayerScript.instance.isEnd || !PlayerScript.instance.isCanCtrl)
            return;

        isButtonDown = true;
        attackImage.color = downColor;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) 
            AutoPlayCtrl.instance.SetInit();
        if (!isCtrl || PlayerScript.instance.isEnd || !PlayerScript.instance.isCanCtrl)
            return;

        isButtonDown = false;
        isAttackStart = false;
        attackImage.color = upColor;
    }

    public void SetInit()
    {
        isButtonDown = false;
        isAttackStart = false;
    }

    public void SetButtonEnable(bool isEnable)
    {
        SetInit();
        isCtrl = isEnable;
        attackImage.color = (isEnable) ? upColor : new Color(1f, 1f, 1f, 0f);
        attackImage.raycastTarget = isEnable;
    }
}
