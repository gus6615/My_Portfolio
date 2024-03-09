using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionSlot : MonoBehaviour
{
    public Image slotImage, jemImage;
    public Text qualityText, levelText;
    public Button button;
    public int jemCode;
    private float noneAlpha = 0.3f;

    public void SetInit(bool isActive)
    {
        int quality = SaveScript.jems[jemCode].quality;
        slotImage.color = SaveScript.qualityColors_weak[quality];
        button.enabled = isActive;
        jemImage.sprite = SaveScript.jems[jemCode].jemSprite;
        levelText.text = "Lv." + SaveScript.saveData.collection_levels[jemCode];
        switch (quality)
        {
            case 0: qualityText.text = "N"; break;
            case 1: qualityText.text = "R"; break;
            case 2: qualityText.text = "E"; break;
            case 3: qualityText.text = "U"; break;
            case 4: qualityText.text = "R"; break;
            case 5: qualityText.text = "U"; break;
            case 6: qualityText.text = "M"; break;
        }
        if (SaveScript.saveData.collection_levels[jemCode] == 0)
            SetNone();
    }

    public void SetNone()
    {
        SetAlpha(slotImage, noneAlpha);
        SetAlpha(jemImage, noneAlpha);
        SetAlpha(qualityText, noneAlpha);
        SetAlpha(levelText, noneAlpha);
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void SetAlpha(Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
