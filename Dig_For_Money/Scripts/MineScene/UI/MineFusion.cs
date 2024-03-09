using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineFusion : MonoBehaviour
{
    static public MineFusion instance;
    public GameObject fusionResult, fusionResultAll;
    public GameObject fusionUIBoxPrefab;
    public Transform fusionResultAllPanel;
    public Button passClickPanel;
    public Image petImage;
    public Text infoText, petText;
    public new AudioSource audio;

    public GameObject fusionAllSlotPrefab;
    public Transform fusionAllSlotPanel;
    private List<bool> fusionAllSuccesses;

    MineFusionSlotAni[] slots;
    MineFusionSlotAni slot;
    MineFusionUIBox[] boxes;
    MineFusionUIBox box;
    Sprite[] sprites;

    private void Start()
    {
        instance = this;
        fusionResult.SetActive(false);
        fusionResultAll.SetActive(false);
        fusionAllSuccesses = new List<bool>();
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    // FusionUI 합성 완료
    public void FusionUI_EndFusion()
    {
        // Error
        if (MineFusionUI.fusionLeftPetCode == -1 || MineFusionUI.fusionRightPetCode == -1)
        {
            fusionResult.SetActive(true);
            passClickPanel.gameObject.SetActive(false);
            MineFusionUI.fusionLeftPetIndex = MineFusionUI.fusionRightPetIndex = -1;
            infoText.text = "Error!";
            petText.text = "죄송합니다. 알 수 없는 오류로 인해 합성에 실패했습니다. 펫 보관함을 확인하시고 문제가 있다면 개발자에게 문의해주시길 바랍니다.";
            audio.clip = SaveScript.SEs[2];
            petImage.sprite = null;

            return;
        }

        int code = (MineFusionUI.fusionLeftPetCode + MineFusionUI.fusionRightPetCode) / 2;
        if (code < 0f) code = 0; 
        int level = Mathf.Max(MineFusionUI.fusionLeftPetLevel, MineFusionUI.fusionRightPetLevel);
        if (level < 1f) level = 1;

        float[] percents = MineFusionUI.instance.GetSuccessPercents(MineFusionUI.fusionLeftPetCode, MineFusionUI.fusionRightPetCode);
        int up = GameFuction.GetRandFlag(percents);
        code += up;
        if (code > SaveScript.mineSlimeQualityNum - 1) 
            code = SaveScript.mineSlimeQualityNum - 1;

        MineFusionUI.fusionLeftPetIndex = MineFusionUI.fusionRightPetIndex = -1;
        fusionResult.SetActive(true);
        passClickPanel.gameObject.SetActive(false);
        switch (up)
        {
            case 0: infoText.text = "등급업 실패..."; audio.clip = SaveScript.SEs[11]; break;
            case 1: infoText.text = "등급업 성공!"; audio.clip = SaveScript.SEs[4]; break;
            case 2: infoText.text = "더블 등급업 성공!!"; audio.clip = SaveScript.SEs[5]; break;
        }

        switch (MineFusionUI.menuIndex)
        {
            case 0:
                petImage.sprite = MineSlime.miner_defaultSprites[code];
                petText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + level;
                break;
            case 1:
                petImage.sprite = MineSlime.adventurer_defaultSprites[code];
                petText.text = "[" + MineSlime.qualityNames[code] + "] Lv." + level;
                break;
        }

        GetResultPet(code, level);
        MineFusionUI.Check_M_PetQuest();
        MinerSlime.SortPetInven();
        AdventurerSlime.SortPetInven();
        audio.Play();
        SaveScript.instance.SaveData_Asyn(true);
    }

    // FusionUI 합성 완료
    public void FusionUI_EndFusionAll()
    {
        boxes = fusionResultAllPanel.GetComponentsInChildren<MineFusionUIBox>();
        for (int i = 0; i < boxes.Length; i++)
            Destroy(boxes[i].gameObject);
        fusionResultAllPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        for (int i = MineFusionUI.fusionForms.Count - 1; i >= 0; i--)
        {
            FusionForm form = MineFusionUI.fusionForms[i];
            box = Instantiate(fusionUIBoxPrefab, fusionResultAllPanel).GetComponent<MineFusionUIBox>();
            box.leftPet.images[1].sprite = sprites[form.leftCode];
            box.leftPet.texts[0].text = "[" + MineSlime.qualityNames[form.leftCode] + "] (Lv." + form.leftLevel + ")";
            box.rightPet.images[1].sprite = sprites[form.rightCode];
            box.rightPet.texts[0].text = "[" + MineSlime.qualityNames[form.rightCode] + "] (Lv." + form.rightLevel + ")";
            box.resultPet.images[1].sprite = sprites[form.resultCode];
            box.resultPet.texts[0].text = "[" + MineSlime.qualityNames[form.resultCode] + "] (Lv." + form.resultLevel + ")";
            box.resultPet.images[1].color = new Color(0.1f, 0.1f, 0.1f);
            box.percentImage.color = new Color(0.3f, 0.1f, 0.1f, 0.9f);
            box.percentText.text = "합성 실패";
            box.percentText.color = new Color(1f, 0.7f, 0.7f);

            switch (MineFusionUI.menuIndex)
            {
                case 0:
                    SaveScript.saveData.hasMiners[form.leftIndex] = SaveScript.saveData.hasMiners[form.rightIndex] = -1;
                    SaveScript.saveData.hasMinerLevels[form.leftIndex] = SaveScript.saveData.hasMinerLevels[form.rightIndex] = -1;
                    SaveScript.saveData.hasMinerExps[form.leftIndex] = SaveScript.saveData.hasMinerExps[form.rightIndex] = 0;
                    break;
                case 1:
                    SaveScript.saveData.hasAdventurers[form.leftIndex] = SaveScript.saveData.hasAdventurers[form.rightIndex] = -1;
                    SaveScript.saveData.hasAdventurerLevels[form.leftIndex] = SaveScript.saveData.hasAdventurerLevels[form.rightIndex] = -1;
                    SaveScript.saveData.hasAdventurerExps[form.leftIndex] = SaveScript.saveData.hasAdventurerExps[form.rightIndex] = 0;
                    break;
            }

            if (fusionAllSuccesses[i])
            {
                box.resultPet.images[1].color = new Color(1f, 1f, 1f);
                box.percentImage.color = new Color(0.1f, 0.3f, 0.1f, 0.9f);
                box.percentText.text = "합성 성공";
                box.percentText.color = new Color(0.7f, 1f, 0.7f);

                GetResultPet(form.resultCode, form.resultLevel);
            }
            else
            {
                box.resultPet.images[1].color = new Color(0.1f, 0.1f, 0.1f);
                box.percentImage.color = new Color(0.3f, 0.1f, 0.1f, 0.9f);
                box.percentText.text = "합성 실패";
                box.percentText.color = new Color(1f, 0.7f, 0.7f);

                GetResultPet(form.leftCode, form.resultLevel);
            }
        }

        fusionResultAll.SetActive(true);
        passClickPanel.gameObject.SetActive(false);
        audio.clip = SaveScript.SEs[4];
        audio.Play();

        MineFusionUI.Check_M_PetQuest();
        MinerSlime.SortPetInven();
        AdventurerSlime.SortPetInven();
        SaveScript.instance.SaveData_Asyn(true);
    }

    public void SetFusionAll()
    {
        slots = fusionAllSlotPanel.GetComponentsInChildren<MineFusionSlotAni>();
        for (int i = 0; i < slots.Length; i++)
            Destroy(slots[i].gameObject);

        fusionAllSuccesses.Clear();
        foreach (var form in MineFusionUI.fusionForms)
        {
            float[] percents = MineFusionUI.instance.GetSuccessPercents(form.leftCode, form.rightCode);
            int up = GameFuction.GetRandFlag(percents);
            if (up > 0) up = 1;

            switch (MineFusionUI.menuIndex)
            {
                case 0: sprites = MineSlime.miner_defaultSprites; break;
                case 1: sprites = MineSlime.adventurer_defaultSprites; break;
            }

            slot = Instantiate(fusionAllSlotPrefab, fusionAllSlotPanel).GetComponent<MineFusionSlotAni>();
            slot.uIBox.images[1].sprite = sprites[form.leftCode];
            slot.uIBox.images[2].sprite = sprites[form.rightCode];

            if (up == 0)
            {
                fusionAllSuccesses.Add(false);
                slot.uIBox.order = -1;
            }
            else
            {
                fusionAllSuccesses.Add(true);
                slot.uIBox.order = 1;
            }
        }
    }

    private void GetResultPet(int _resultCode, int _resultLevel)
    {
        int emptyIndex = -1;

        switch (MineFusionUI.menuIndex)
        {
            case 0:
                emptyIndex = MinerSlime.FindEmptyPetInven();
                SaveScript.saveData.hasMiners[emptyIndex] = _resultCode;
                SaveScript.saveData.hasMinerLevels[emptyIndex] = _resultLevel;
                SaveScript.saveData.hasMinerExps[emptyIndex] = 0;
                break;
            case 1:
                emptyIndex = AdventurerSlime.FindEmptyPetInven();
                SaveScript.saveData.hasAdventurers[emptyIndex] = _resultCode;
                SaveScript.saveData.hasAdventurerLevels[emptyIndex] = _resultLevel;
                SaveScript.saveData.hasAdventurerExps[emptyIndex] = 0;
                break;
        }

        // 퀘스트
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 32 });
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 35 });
        switch (_resultCode)
        {
            case 7: QuestCtrl.instance.SetMainQuestAmount(new int[] { 106 }); break;
            case 8: QuestCtrl.instance.SetMainQuestAmount(new int[] { 107 }); break;
            case 9: QuestCtrl.instance.SetMainQuestAmount(new int[] { 108 }); break;
        }
    }

    public void Audio_Slot()
    {
        audio.clip = SaveScript.SEs[0];
        audio.Play();
    }

    public void PassAni()
    {
        Debug.Log("PassAni");
        passClickPanel.gameObject.SetActive(false);
        MineFusionUI.instance.fusionUI_animator.Play("FusionUI_Idle", -1, 0.9f);
        FusionUI_EndFusion();
    }

    public void PassAllAni()
    {
        Debug.Log("PassAllAni");
        passClickPanel.gameObject.SetActive(false);
        MineFusionUI.instance.fusionUI_animator.Play("FusionUI_All", -1, 0.9f);
        FusionUI_EndFusionAll();
    }
}
