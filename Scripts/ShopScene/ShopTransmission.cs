using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTransmission : MonoBehaviour
{
    static public ShopTransmission instance;

    public new AudioSource audio;
    public Animator animator;

    // 애니메이션 관련 변수들
    public GameObject passClickPanel;
    public Image nextItem, previousItem;
    public Image leftLight, rightLight;
    public Image whiteEffect;
    public GameObject resultObject;
    public Image result_quality, result_item;
    public Text result_name;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        this.gameObject.SetActive(false);
        resultObject.SetActive(false);
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public void SetAudio(int _SE)
    {
        audio.clip = SaveScript.SEs[_SE];
        audio.Play();
    }

    public void SetAni()
    {
        animator.Play("Transmission", -1, 0.88f);
        this.gameObject.SetActive(true);
        passClickPanel.SetActive(true);

        switch (GameItemShop.instance.menuIndex)
        {
            case 0:
                result_item.sprite = nextItem.sprite = SaveScript.picks[GameItemShop.listIndex].sprites[0];
                previousItem.sprite = SaveScript.picks[GameItemShop.listIndex - 1].sprites[0];
                whiteEffect.color = leftLight.color = rightLight.color = result_quality.color =
                    SaveScript.qualityColors[GameFuction.GetQualityOfEquipment(SaveScript.saveData.pickReinforces[GameItemShop.listIndex - 1])];
                result_name.text = SaveScript.picks[GameItemShop.listIndex].name + " (+" + SaveScript.saveData.pickReinforces[GameItemShop.listIndex - 1] + "성)";
                break;
            case 1:
                result_item.sprite = nextItem.sprite = SaveScript.hats[GameItemShop.listIndex].sprite;
                previousItem.sprite = SaveScript.hats[GameItemShop.listIndex - 1].sprite;
                whiteEffect.color = leftLight.color = rightLight.color = result_quality.color =
                    SaveScript.qualityColors[GameFuction.GetQualityOfEquipment(SaveScript.saveData.hatReinforces[GameItemShop.listIndex - 1])];
                result_name.text = SaveScript.hats[GameItemShop.listIndex].name + " (+" + SaveScript.saveData.hatReinforces[GameItemShop.listIndex - 1] + "성)";
                break;
            case 2:
                result_item.sprite = nextItem.sprite = SaveScript.rings[GameItemShop.listIndex].sprite;
                previousItem.sprite = SaveScript.rings[GameItemShop.listIndex - 1].sprite;
                whiteEffect.color = leftLight.color = rightLight.color = result_quality.color =
                    SaveScript.qualityColors[GameFuction.GetQualityOfEquipment(SaveScript.saveData.ringReinforces[GameItemShop.listIndex - 1])];
                result_name.text = SaveScript.rings[GameItemShop.listIndex].name + " (+" + SaveScript.saveData.ringReinforces[GameItemShop.listIndex - 1] + "성)";
                break;
            case 3:
                result_item.sprite = nextItem.sprite = SaveScript.pendants[GameItemShop.listIndex].sprite;
                previousItem.sprite = SaveScript.pendants[GameItemShop.listIndex - 1].sprite;
                whiteEffect.color = leftLight.color = rightLight.color = result_quality.color =
                    SaveScript.qualityColors[GameFuction.GetQualityOfEquipment(SaveScript.saveData.pendantReinforces[GameItemShop.listIndex - 1])];
                result_name.text = SaveScript.pendants[GameItemShop.listIndex].name + " (+" + SaveScript.saveData.pendantReinforces[GameItemShop.listIndex - 1] + "성)";
                break;
            case 4:
                result_item.sprite = nextItem.sprite = SaveScript.swords[GameItemShop.listIndex].sprite;
                previousItem.sprite = SaveScript.swords[GameItemShop.listIndex - 1].sprite;
                whiteEffect.color = leftLight.color = rightLight.color = result_quality.color =
                    SaveScript.qualityColors[GameFuction.GetQualityOfEquipment(SaveScript.saveData.swordReinforces[GameItemShop.listIndex - 1])];
                result_name.text = SaveScript.swords[GameItemShop.listIndex].name + " (+" + SaveScript.saveData.swordReinforces[GameItemShop.listIndex - 1] + "성)";
                break;
        }
    }

    public void Result()
    {
        // Transmission
        resultObject.SetActive(true);
        passClickPanel.SetActive(false);
        SetAudio(9);
        switch (GameItemShop.instance.menuIndex)
        {
            case 0:
                if (GameItemShop.listIndex > 9) GameFuction.Buy(0, 0, SaveScript.picks[GameItemShop.listIndex].price);
                else if (GameItemShop.listIndex > 5) GameFuction.Buy(0, SaveScript.picks[GameItemShop.listIndex].price, 0);
                else GameFuction.Buy(SaveScript.picks[GameItemShop.listIndex].price, 0, 0);
                SaveScript.saveData.hasPicks[GameItemShop.listIndex] = true;
                SaveScript.saveData.equipPick = GameItemShop.listIndex;
                SaveScript.saveData.pickReinforces[GameItemShop.listIndex] = SaveScript.saveData.pickReinforces[GameItemShop.listIndex - 1];

                // pickLevel 조정
                int pickIndex = 0;
                for (int i = 0; i < SaveScript.pickNum; i++)
                    if (SaveScript.saveData.hasPicks[i])
                        pickIndex = i;
                SaveScript.saveData.pickLevel = pickIndex;
                SaveScript.SetDataAsStat();
                break;
            case 1:
                if (GameItemShop.listIndex > 9) GameFuction.Buy(0, 0, SaveScript.hats[GameItemShop.listIndex].price);
                else if (GameItemShop.listIndex > 5) GameFuction.Buy(0, SaveScript.hats[GameItemShop.listIndex].price, 0);
                else GameFuction.Buy(SaveScript.hats[GameItemShop.listIndex].price, 0, 0);
                SaveScript.saveData.hasHats[GameItemShop.listIndex] = true;
                SaveScript.saveData.equipHat = GameItemShop.listIndex;
                SaveScript.saveData.hatReinforces[GameItemShop.listIndex] = SaveScript.saveData.hatReinforces[GameItemShop.listIndex - 1];
                break;
            case 2:
                if (GameItemShop.listIndex > 9) GameFuction.Buy(0, 0, SaveScript.rings[GameItemShop.listIndex].price);
                else if (GameItemShop.listIndex > 5) GameFuction.Buy(0, SaveScript.rings[GameItemShop.listIndex].price, 0);
                else GameFuction.Buy(SaveScript.rings[GameItemShop.listIndex].price, 0, 0);
                SaveScript.saveData.hasRings[GameItemShop.listIndex] = true;
                SaveScript.saveData.equipRing = GameItemShop.listIndex;
                SaveScript.saveData.ringReinforces[GameItemShop.listIndex] = SaveScript.saveData.ringReinforces[GameItemShop.listIndex - 1];
                break;
            case 3:
                if (GameItemShop.listIndex > 9) GameFuction.Buy(0, 0, SaveScript.pendants[GameItemShop.listIndex].price);
                else if (GameItemShop.listIndex > 5) GameFuction.Buy(0, SaveScript.pendants[GameItemShop.listIndex].price, 0);
                else GameFuction.Buy(SaveScript.pendants[GameItemShop.listIndex].price, 0, 0);
                SaveScript.saveData.hasPenants[GameItemShop.listIndex] = true;
                SaveScript.saveData.equipPendant = GameItemShop.listIndex;
                SaveScript.saveData.pendantReinforces[GameItemShop.listIndex] = SaveScript.saveData.pendantReinforces[GameItemShop.listIndex - 1];
                break;
            case 4:
                if (GameItemShop.listIndex > 9) GameFuction.Buy(0, 0, SaveScript.swords[GameItemShop.listIndex].price);
                else if (GameItemShop.listIndex > 5) GameFuction.Buy(0, SaveScript.swords[GameItemShop.listIndex].price, 0);
                else GameFuction.Buy(SaveScript.swords[GameItemShop.listIndex].price, 0, 0);
                SaveScript.saveData.hasSwords[GameItemShop.listIndex] = true;
                SaveScript.saveData.equipSword = GameItemShop.listIndex;
                SaveScript.saveData.swordReinforces[GameItemShop.listIndex] = SaveScript.saveData.swordReinforces[GameItemShop.listIndex - 1];
                break;
        }

        // 퀘스트
        GameItemShop.CheckEquipmentQuest();
    }

    public void PassAni()
    {
        passClickPanel.SetActive(false);
        animator.Play("Transmission", -1, 0.88f);
        Result();
    }

    public void CloseButton()
    {
        this.gameObject.SetActive(false);
        resultObject.SetActive(false);
        GameItemShop.instance.SetMenu();
        Shop.instance.SetBasicInfo();
        Shop.instance.SetAudio(0);
    }
}
