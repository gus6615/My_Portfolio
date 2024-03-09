using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMailboxUI : MonoBehaviour
{
    private static MainMailboxUI Instance;
    public static MainMailboxUI instance
    {
        set 
        {
            if (Instance == null)
                Instance = value; 
        }
        get { return Instance; }
    }

    public Canvas mailBoxObject;
    public GameObject mailBoxButton;
    public GameObject emptyText;
    public Transform contentTr;
    public GameObject mailSlotPrefab;
    [SerializeField] private Sprite cashSprite;
    [SerializeField] private Sprite manaOreSprite;

    private bool isOn;

    UIBox[] uIBoxes;
    UIBox uIBox;
    Order[] orders;
    Order order;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mailBoxObject.gameObject.SetActive(false);
    }

    public void OnOffMailbox()
    {
        if (!MainScript.isChangeScene)
        {
            isOn = !isOn;
            mailBoxObject.gameObject.SetActive(isOn);
            MainScript.instance.SetAudio(0);
            
            if (isOn)
                SetMailboxInfo();
            else
                SaveScript.instance.SaveData_Asyn(true);
        }
    }

    public void SetMailboxButton()
    {
        bool isOn = false;

        for (int i = 0; i < SaveScript.mailboxNum; i++)
        {
            if (SaveScript.saveData.mailboxes[i] != 0)
            {
                isOn = true;
                break;
            }
        }

        mailBoxButton.gameObject.SetActive(isOn);
        emptyText.gameObject.SetActive(!isOn);
    }

    private void SetMailboxInfo()
    {
        uIBoxes = contentTr.GetComponentsInChildren<UIBox>();
        for (int i = 0; i < uIBoxes.Length; i++)
            Destroy(uIBoxes[i].gameObject);

        for (int i = 0; i < SaveScript.mailboxNum; i++)
        {
            int value = SaveScript.saveData.mailboxes[i];
            if (value == 0)
                continue;

            uIBox = Instantiate(mailSlotPrefab, contentTr).GetComponent<UIBox>();
            uIBox.button.onClick.AddListener(GetReward);
            order = uIBox.button.GetComponent<Order>();
            order.order = i;
            order.order2 = value;

            if (Mathf.Sign(value) == -1)
            {
                // Cash
                uIBox.images[0].sprite = cashSprite;
                uIBox.tmp_texts[0].SetText("<color=#FF9696>[ 레드 다이아몬드 x " + GameFuction.GetNumText(Mathf.Abs(value)) 
                    + " ]\n" + "<color=white>상품이 도착하였습니다!");
            }
            else
            {
                // ManaOre
                uIBox.images[0].sprite = manaOreSprite;
                uIBox.tmp_texts[0].SetText("<color=#9696FF>[ 마나석 x " + GameFuction.GetNumText(Mathf.Abs(value))
                    + " ]\n" + "<color=white>상품이 도착하였습니다!");
            }
        }

        SetMailboxButton();
    }

    public void GetReward()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            order = EventSystem.current.currentSelectedGameObject.GetComponent<Order>();
            if (order != null)
            {
                int index = order.order;
                int value = order.order2;

                SaveScript.saveData.mailboxes[index] = 0;
                if (Mathf.Sign(value) == -1)
                {
                    // Cash
                    SaveScript.saveData.cash += Mathf.Abs(value);
                }
                else
                {
                    // ManaOre
                    SaveScript.saveData.manaOre += Mathf.Abs(value);
                }

                MainScript.instance.SetAudio(4);
                SetMailboxInfo();
            }
        }
    }

    public void GetAllReward()
    {
        orders = contentTr.GetComponentsInChildren<Order>();
        for (int i = 0; i < orders.Length; i++)
        {
            order = orders[i];
            if (order != null)
            {
                int index = order.order;
                int value = order.order2;

                SaveScript.saveData.mailboxes[index] = 0;
                if (Mathf.Sign(value) == -1)
                {
                    // Cash
                    SaveScript.saveData.cash += Mathf.Abs(value);
                }
                else
                {
                    // ManaOre
                    SaveScript.saveData.manaOre += Mathf.Abs(value);
                }
            }
        }

        MainScript.instance.SetAudio(4);
        SetMailboxInfo();
    }
}
