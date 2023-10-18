using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineDecomposition : MonoBehaviour
{
    static public MineDecomposition instance;
    public GameObject slotPrefab;
    public Transform slotPanel;
    public Button passClickPanel;
    public GameObject animator_result;
    public Text resultText;
    public new AudioSource audio;

    SlotAni[] slots;
    SlotAni slot;
    Sprite[] sprites;

    private void Start()
    {
        instance = this;
        animator_result.SetActive(false);
        audio.mute = !SaveScript.saveData.isSEOn;
    }

    public void SetDecompositionAll()
    {
        foreach (var index in MineDecompositionUI.decomposition_forms)
        {
            int code = -1;
            switch (MineDecompositionUI.menuIndex)
            {
                case 0: 
                    sprites = MineSlime.miner_defaultSprites;
                    code = SaveScript.saveData.hasMiners[index];
                    break;
                case 1: 
                    sprites = MineSlime.adventurer_defaultSprites;
                    code = SaveScript.saveData.hasAdventurers[index];
                    break;
            }

            slot = Instantiate(slotPrefab, slotPanel).GetComponent<SlotAni>();
            slot.uIBox.images[1].sprite = sprites[code];
        }
    }

    // DecompositionUI 분해 완료
    public void DecompositionUI_End()
    {
        int code = MineDecompositionUI.selectPetCode;
        if (code < 0f) code = 0;
        int manaNum = MineDecompositionUI.GetManaOreNum(code);

        slots = slotPanel.GetComponentsInChildren<SlotAni>();
        for (int i = 0; i < slots.Length; i++)
            Destroy(slots[i].gameObject);

        passClickPanel.gameObject.SetActive(false);
        animator_result.SetActive(true);
        resultText.text = "- 마나석 " + GameFuction.GetNumText(manaNum) + " 개 -";

        MineDecompositionUI.selectPetIndex = -1;
        MineDecompositionUI.instance.selectedPetImage.gameObject.SetActive(false);
        MineDecompositionUI.instance.selectPlusImage.gameObject.SetActive(true);
        MineDecompositionUI.instance.decompositionButton.SetActive(false);
        SaveScript.saveData.manaOre += manaNum;
        AchievementCtrl.instance.SetAchievementAmount(23, manaNum);

        switch (MineDecompositionUI.menuIndex)
        {
            case 0: MinerSlime.SortPetInven(); break;
            case 1: AdventurerSlime.SortPetInven(); break;
        }

        audio.clip = SaveScript.SEs[4];
        audio.Play();
    }

    // DecompositionUI 분해 완료
    public void DecompositionUI_AllEnd()
    {
        int code = -1;
        int manaNum = 0;

        foreach (var index in MineDecompositionUI.decomposition_forms)
        {
            switch (MineDecompositionUI.menuIndex)
            {
                case 0: 
                    code = SaveScript.saveData.hasMiners[index];
                    SaveScript.saveData.hasMiners[index] = -1;
                    break;
                case 1: 
                    code = SaveScript.saveData.hasAdventurers[index];
                    SaveScript.saveData.hasAdventurers[index] = -1;
                    break;
            }

            manaNum += MineDecompositionUI.GetManaOreNum(code);
        }

        passClickPanel.gameObject.SetActive(false);
        animator_result.SetActive(true);
        resultText.text = "- 마나석 " + GameFuction.GetNumText(manaNum) + " 개 -";
        SaveScript.saveData.manaOre += manaNum;
        AchievementCtrl.instance.SetAchievementAmount(23, manaNum);

        audio.clip = SaveScript.SEs[4];
        audio.Play();
    }

    public void Audio_Slot()
    {
        audio.clip = SaveScript.SEs[30];
        audio.Play();
    }

    public void PassAni()
    {
        passClickPanel.gameObject.SetActive(false);
        MineDecompositionUI.instance.decomposition_animator.Play("DecompositionUI_Decomposition", -1, 0.85f);
        DecompositionUI_End();
    }

    public void PassAllAni()
    {
        passClickPanel.gameObject.SetActive(false);
        MineDecompositionUI.instance.decomposition_animator.Play("DecompositionUI_DecompositionAll", -1, 0.85f);
        DecompositionUI_AllEnd();
    }
}
