using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveCtrl : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static MoveCtrl instance;

    private Vector2 centerVec;
    private Image leftImage, rightImage;
    private Color downColor, upColor;

    static public bool isMoveStart;

    void Awake()
    {
        instance = this;

        centerVec = transform.GetComponent<RectTransform>().position;
        leftImage = transform.GetComponentsInChildren<Image>()[0];
        rightImage = transform.GetComponentsInChildren<Image>()[1];
        downColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        upColor = Color.white;
    }

    public void OnDrag(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) 
            AutoPlayCtrl.instance.SetInit();
        if (PlayerScript.instance.isEnd || !PlayerScript.instance.isCanCtrl)
            return;

        isMoveStart = true;
        if (e.pointerEnter != null)
        {
            if (e.pointerEnter.tag == "LeftMove")
            {
                leftImage.color = downColor;
                rightImage.color = upColor;
                PlayerScript.instance.moveData = -1f;
            }
            else if (e.pointerEnter.tag == "RightMove")
            {
                leftImage.color = upColor;
                rightImage.color = downColor;
                PlayerScript.instance.moveData = 1f;
            }
        }
        else
        {
            if ((e.position - centerVec).x < 0f)
            {
                leftImage.color = downColor;
                rightImage.color = upColor;
                PlayerScript.instance.moveData = -1f;
            }
            else
            {
                leftImage.color = upColor;
                rightImage.color = downColor;
                PlayerScript.instance.moveData = 1f;
            }
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) 
            AutoPlayCtrl.instance.SetInit();
        if (PlayerScript.instance.isEnd || !PlayerScript.instance.isCanCtrl)
            return;

        isMoveStart = false;
        leftImage.color = upColor;
        rightImage.color = upColor;
        PlayerScript.instance.moveData = 0f;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) 
            AutoPlayCtrl.instance.SetInit();
        if (PlayerScript.instance.isEnd || !PlayerScript.instance.isCanCtrl)
            return;

        isMoveStart = true;
        if (e.pointerEnter != null)
        {
            if (e.pointerEnter.tag == "LeftMove")
            {
                leftImage.color = downColor;
                rightImage.color = upColor;
                PlayerScript.instance.moveData = -1f;
            }
            else if (e.pointerEnter.tag == "RightMove")
            {
                leftImage.color = upColor;
                rightImage.color = downColor;
                PlayerScript.instance.moveData = 1f;
            }
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn) 
            AutoPlayCtrl.instance.SetInit();
        if (PlayerScript.instance.isEnd || !PlayerScript.instance.isCanCtrl)
            return;

        isMoveStart = false;
        if (e.pointerEnter != null)
        {
            leftImage.color = upColor;
            rightImage.color = upColor;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PlayerScript.instance.isEnd)
        {
            if (isMoveStart)
            {
                if (!Physics2D.BoxCast(PlayerScript.instance.transform.position, new Vector2(0.5f, 0.5f), 0f, Vector2.right * Mathf.Sign(PlayerScript.instance.transform.localScale.x), 0.1f, 256))
                    PlayerScript.instance.rigidbody.position += Vector2.right * PlayerScript.instance.moveData * PlayerScript.instance.moveSpeed * Time.deltaTime;
                MoveAnimator();
            }
        }
        else
        {
            PlayerScript.instance.moveData = 0f;
            PlayerScript.instance.animator.SetBool("isMove", false);
            PlayerScript.instance.handsAnimator.SetBool("isMove", false);
        }
    }

    public void MoveAnimator()
    {
        if (PlayerScript.instance.moveData != 0f)
        {
            PlayerScript.instance.transform.localScale = new Vector3(Mathf.Sign(PlayerScript.instance.moveData) * 1f, 1f, 1f);
            PlayerScript.instance.animator.SetBool("isMove", true);
            PlayerScript.instance.handsAnimator.SetBool("isMove", true);
        }
        else
        {
            PlayerScript.instance.animator.SetBool("isMove", false);
            PlayerScript.instance.handsAnimator.SetBool("isMove", false);
        }
    }

    public void SetInit()
    {
        isMoveStart = false;
        leftImage.color = upColor;
        rightImage.color = upColor;
        PlayerScript.instance.moveData = 0f;
    }
}
