using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JumpCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image jumpImage;
    private Color upColor, downColor;

    static public bool isJumpStart;
    static public int jumpCount;

    // Start is called before the first frame update
    void Start()
    {
        jumpImage = GetComponentInChildren<Image>();
        downColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        upColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerScript.instance.isEnd)
        {
            if (isJumpStart && jumpCount < 2)
            {
                PlayerScript.instance.isJump = true;
            }
            // 튜토리얼 체크
            if (SaveScript.saveData.isTutorial)
                if (Tutorial.instance.tutorialIndex == 5 && jumpCount == 2)
                    Tutorial.instance.TutorialButton();
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) AutoPlayCtrl.instance.SetInit();
        if (!PlayerScript.instance.isEnd)
        {
            if(!isJumpStart)
                isJumpStart = true;
            jumpImage.color = downColor;
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) AutoPlayCtrl.instance.SetInit();
        if (!PlayerScript.instance.isEnd)
        {
            if(isJumpStart)
                isJumpStart = false;
            jumpImage.color = upColor;
        }
    }
}
