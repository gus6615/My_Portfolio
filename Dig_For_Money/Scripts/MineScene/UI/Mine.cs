using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Mine : MonoBehaviour
{
    public static Mine instance;

    public static bool isGotoMainScene;

    public new AudioSource audio;
    [SerializeField] private Animator UIBoxAnimator;
    private bool isOnOffInfo, isOnOffFacility, isOnOffUIBox, isOnOffTeamUI, isOnOffFeedUI, isOnOffFusionUI, isOnOffUpgradeUI, isOnOffDecompositionUI;
    public GameObject infoUIObject, facilityUIObject, teamUIObject, feedUIObject, fusionUIObject, upgradeUIObject, decompositionObject;

    [SerializeField] private FadeEffect teamButton, feedButton, fusionButton, upgradeButton;
    [SerializeField] private FadeEffect facilityButton, facilityLevel, facilityReward, facilityBuf, facilityItem, facilityUpgrade;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        infoUIObject.SetActive(false);
        facilityUIObject.SetActive(false);
        teamUIObject.SetActive(false);
        feedUIObject.SetActive(false);
        fusionUIObject.SetActive(false);
        upgradeUIObject.SetActive(false);
        decompositionObject.SetActive(false);
        audio.mute = !SaveScript.saveData.isSEOn;
        isOnOffUIBox = true;
        UIBoxAnimator.SetBool("isOn", isOnOffUIBox);
        MineFacilityUI.instance.SetData();
        SetTutorial();
    }

    public void GotoMainScene()
    {
        SetAudio(0);
        isGotoMainScene = true;
    }

    private void SetTutorial()
    {
        teamButton.enabled = teamButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 33 }, SaveScript.saveData.mainQuest_list);
        feedButton.enabled = feedButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 34 }, SaveScript.saveData.mainQuest_list);
        fusionButton.enabled = fusionButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 35 }, SaveScript.saveData.mainQuest_list);
        upgradeButton.enabled = upgradeButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 36 }, SaveScript.saveData.mainQuest_list);
        facilityButton.enabled = facilityButton.isReSize = QuestCtrl.CheckFadeUI(new int[] { 38, 39, 40, 41, 42 }, SaveScript.saveData.mainQuest_list);
        facilityReward.enabled = facilityReward.isReSize = QuestCtrl.CheckFadeUI(new int[] { 38 }, SaveScript.saveData.mainQuest_list);
        facilityLevel.enabled = facilityLevel.isReSize = QuestCtrl.CheckFadeUI(new int[] { 39 }, SaveScript.saveData.mainQuest_list);
        facilityBuf.enabled = facilityBuf.isReSize = QuestCtrl.CheckFadeUI(new int[] { 40 }, SaveScript.saveData.mainQuest_list);
        facilityItem.enabled = facilityItem.isReSize = QuestCtrl.CheckFadeUI(new int[] { 41 }, SaveScript.saveData.mainQuest_list);
        facilityUpgrade.enabled = facilityUpgrade.isReSize = QuestCtrl.CheckFadeUI(new int[] { 42 }, SaveScript.saveData.mainQuest_list);
    }

    public void OnOffInfo()
    {
        SetAudio(0);
        isOnOffInfo = !isOnOffInfo;
        infoUIObject.SetActive(isOnOffInfo);
        MineMap.instance.SetActiveSelectedPet(false);
        MineInfo.instance.SetDefaultVariable();
        MineInfo.instance.SetInfoInfo();
    }

    public void OnOffFacility()
    {
        SetAudio(0);
        isOnOffFacility = !isOnOffFacility;
        facilityUIObject.SetActive(isOnOffFacility);
        MineMap.instance.SetActiveSelectedPet(false);
        MineFacilityUI.instance.SetDefaultVariable();
        MineFacilityUI.instance.SetUI();
        SaveScript.stat.SetStat();
    }

    // UIBox OnOff 버튼
    public void OnOffUIBox()
    {
        isOnOffUIBox = !isOnOffUIBox;
        UIBoxAnimator.SetBool("isOn", isOnOffUIBox);
        MineMap.instance.SetActiveSelectedPet(false);
        SetAudio(0);
    }

    public void OnOffTeamUI()
    {
        SetAudio(0);
        isOnOffTeamUI = !isOnOffTeamUI;
        teamUIObject.SetActive(isOnOffTeamUI);
        MineMap.instance.SetActiveSelectedPet(false);
        MineTeamUI.instance.SetDefaultVariable();
        MineTeamUI.instance.TeamUI_Menu();
    }

    public void OnOffFeedUI()
    {
        SetAudio(0);
        isOnOffFeedUI = !isOnOffFeedUI;
        feedUIObject.SetActive(isOnOffFeedUI);
        MineMap.instance.SetActiveSelectedPet(false);
        MineFeedUI.instance.SetDefaultVariable();
        MineFeedUI.instance.FeedUI_Menu();
    }

    public void OnOffFusionUI()
    {
        SetAudio(0);
        isOnOffFusionUI = !isOnOffFusionUI;
        fusionUIObject.SetActive(isOnOffFusionUI);
        MineMap.instance.SetActiveSelectedPet(false);
        MineFusionUI.SetFusionVariable();
        MineFusionUI.instance.SetDefaultVariable();
        MineFusionUI.instance.FusionUI_Menu();
    }

    public void OnOffUpgradeUI()
    {
        SetAudio(0);
        isOnOffUpgradeUI = !isOnOffUpgradeUI;
        upgradeUIObject.SetActive(isOnOffUpgradeUI);
        MineMap.instance.SetActiveSelectedPet(false);
        MineUpgradeUI.instance.SetDefaultVariable();
        MineUpgradeUI.instance.UpgradeUI_Menu();
    }

    public void OnOffDecompositionUI()
    {
        SetAudio(0);
        isOnOffDecompositionUI = !isOnOffDecompositionUI;
        decompositionObject.SetActive(isOnOffDecompositionUI);
        MineMap.instance.SetActiveSelectedPet(false);
        MineDecompositionUI.SetDecompositionVariable();
        MineDecompositionUI.instance.SetDefaultVariable();
        MineDecompositionUI.instance.DecompositionUI_Menu();
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }
}
