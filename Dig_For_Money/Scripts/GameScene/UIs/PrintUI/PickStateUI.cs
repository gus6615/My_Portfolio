using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickStateUI : MonoBehaviour
{
    public static PickStateUI instance;

    public GameObject UIObject;
    public Text pickStateText;
    public Image qualityImage, pickImage;
    public Slider pickSlider;
    public Image pickSliderFillImage;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        ShowPickState();
    }

    /// <summary>
    /// 현재 장착 중인 곡괭이의 상태를 UI로 나타냅니다.
    /// </summary>
    public void ShowPickState()
    {
        pickSlider.maxValue = PlayerScript.instance.pickFullHP;
        pickSlider.value = PlayerScript.instance.pickHP;
        pickStateText.text = GameFuction.GetNumText(PlayerScript.instance.pickHP) + " / " + GameFuction.GetNumText(PlayerScript.instance.pickFullHP);

        qualityImage.color = SaveScript.qualityColors[GameFuction.GetQualityOfEquipment(SaveScript.saveData.pickReinforces[SaveScript.saveData.equipPick])];
        float standardNum = pickSlider.maxValue / SaveScript.pickStateNum;

        if (pickSlider.value >= standardNum * 2)
        {
            pickSliderFillImage.color = Color.yellow;
            pickImage.sprite = SaveScript.picks[SaveScript.saveData.equipPick].sprites[0];
        }
        else if (pickSlider.value >= standardNum)
        {
            pickSliderFillImage.color = new Color(1f, 0.5f, 0f, 1f);
            pickImage.sprite = SaveScript.picks[SaveScript.saveData.equipPick].sprites[1];
        }
        else
        {
            pickSliderFillImage.color = Color.red;
            pickImage.sprite = SaveScript.picks[SaveScript.saveData.equipPick].sprites[2];
        }
    }
}
