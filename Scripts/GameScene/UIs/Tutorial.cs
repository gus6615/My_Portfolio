using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    static public bool isEndTutorial;
    public int tutorialIndex;
    public bool istutorialChange;

    public GameObject tutorialInfo;
    public GameObject pauseButton;
    public FadeEffect[] jumpObjects;
    public FadeEffect[] moveObjects;
    public FadeEffect attackObject, interactionObject;
    public Text tutorialText;
    public GameObject tutorialButton;

    public int value;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        pauseButton.gameObject.SetActive(false);
        jumpObjects[0].gameObject.SetActive(false);
        jumpObjects[1].gameObject.SetActive(false);
        moveObjects[0].gameObject.SetActive(false);
        moveObjects[1].gameObject.SetActive(false);
        interactionObject.gameObject.SetActive(false);
        attackObject.gameObject.SetActive(false);
        PickStateUI.instance.UIObject.SetActive(false);

        tutorialIndex = 0;
        istutorialChange = true;
        if (SaveScript.saveData.isTutorial)
            isEndTutorial = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (istutorialChange && SaveScript.saveData.isTutorial)
        {
            istutorialChange = false;

            switch (tutorialIndex)
            {
                case 5:
                    jumpObjects[0].gameObject.SetActive(true);
                    jumpObjects[1].gameObject.SetActive(true);
                    SetFadeButtons(1);
                    SetTutorialInfo("잘하셨습니다! 이번에는 점프를 해볼까요? '점프'키를 2번 눌러 이단 점프를 해보세요!", false);
                    break;
                case 6:
                    SetTutorialInfo("좋아요! 이번에는 땅파는 방법에 대해 알아보겠습니다!", true);
                    break;
                case 7:
                    interactionObject.gameObject.SetActive(true);
                    SetFadeButtons(2);
                    SetTutorialInfo("우측 하단에 있는 조이스틱을 바깥으로 원하는 방향으로 '쭉 당겨' 땅을 파보세요!", false);
                    break;
                default:
                    StartCoroutine(StartTutorial(tutorialIndex));
                    break;
            }
        }

        switch (tutorialIndex)
        {
            case 4:// 이동을 완료했을 때
                if ((int)PlayerScript.instance.transform.position.x != 0f && !istutorialChange) 
                    TutorialButton();
                break;
            case 7: // 땅 블럭은 한 칸이상 부쉈을 때
                if (value > 0f && !istutorialChange)
                    TutorialButton();
                break;
            case 8: // 보석 4개 이상 먹은 경우
                if (value > 3 && !istutorialChange)
                    TutorialButton();
                break;
            case 11: // 레어 등급 이상 광물
                if (value > 0 && !istutorialChange)
                    TutorialButton();
                break;
            case 14: // 던전 상자 열기
                if (value > 0 && !istutorialChange)
                    TutorialButton();
                break;
        }
    }

    public void SetTutorialInfo(string str, bool isButtonOn)
    {
        tutorialInfo.SetActive(true);
        tutorialText.text = str;
        tutorialButton.SetActive(isButtonOn);
        if (isButtonOn) PlayerScript.instance.moveData = 0f;
    }

    public void TutorialButton()
    {
        value = 0;
        tutorialIndex++;
        istutorialChange = true;
    }

    public void SkipButton()
    {
        EndTutorial();
    }

    private void EndTutorial()
    {
        isEndTutorial = true;
        PrintUI.isGotoMainScene = true;
    }

    private void SetFadeButtons(int _order)
    {
        moveObjects[0].enabled = moveObjects[1].enabled = false;
        jumpObjects[0].enabled = jumpObjects[1].enabled = false;
        interactionObject.enabled = attackObject.enabled = false;

        switch (_order)
        {
            case 0: moveObjects[0].enabled = moveObjects[1].enabled = true; break;
            case 1: jumpObjects[0].enabled = jumpObjects[1].enabled = true; break;
            case 2: interactionObject.enabled = true; break;
            case 3: attackObject.enabled = true; break;
        }
    }

    IEnumerator StartTutorial(int index)
    {
        switch (index)
        {
            case 0:
                SetTutorialInfo("여기는 '튜토리얼' 공간입니다 :)", true);
                break;
            case 1:
                PickStateUI.instance.UIObject.SetActive(true);
                SetTutorialInfo("왼쪽 상단의 곡괭이 그림은 '체력'을 나타냅니다!", true);
                break;
            case 2:
                pauseButton.gameObject.SetActive(true);
                SetTutorialInfo("그 아래의 버튼은 '일시 정지' 버튼입니다!", true);
                break;
            case 3:
                moveObjects[0].gameObject.SetActive(true);
                moveObjects[1].gameObject.SetActive(true);
                SetFadeButtons(0);
                SetTutorialInfo("좌측 하단 화살표 버튼은 '이동' 버튼이랍니다!", true);
                break;
            case 4:
                SetTutorialInfo("이동 버튼을 꾹 눌러 이동해보세요 :)", false);
                break;
            case 8:
                SetFadeButtons(-1);
                SetTutorialInfo("잘하셨습니다! 이제 빛나지 않는 광물 4개를 캐보세요!", false);
                break;
            case 9:
                SetTutorialInfo("잘하셨습니다! 이번에는 빛나는 광물에 대해 알아보도록 하겠습니다!", true);
                break;
            case 10:
                SetTutorialInfo("광물마다 고유의 등급이 존재해요! 높은 등급을 캐기 위해선 곡괭이의 강화가 중요하답니다!", true);
                break;
            case 11:
                SaveScript.saveData.pickReinforces[0] = 4;
                PickStateUI.instance.ShowPickState();
                PlayerScript.instance.audio.clip = SaveScript.SEs[9];
                PlayerScript.instance.audio.Play();
                SetTutorialInfo("임시로 곡괭이의 등급을 '레어' 등급으로 강화해드렸어요! 이제 빛나는 광물을 발견하여 얻어보세요 :)", false);
                break;
            case 12:
                SetTutorialInfo("잘하셨습니다! 이번에는 '던전'에 대해 알아보도록 하겠습니다!", true);
                break;
            case 13:
                SetTutorialInfo("던전에는 몬스터와 보물 상자가 존재합니다. 보물 상자에선 좋은 광물을 많이 얻을 수 있어요!", true);
                break;
            case 14:
                SetTutorialInfo("이제 던전을 찾아 몬스터를 처치하고 보물 상자를 열어보세요!", false);
                attackObject.gameObject.SetActive(true);
                SetFadeButtons(3);
                break;
            case 15:
                yield return new WaitForSeconds(3f);
                SetFadeButtons(-1);
                SetTutorialInfo("잘하셨습니다! 이렇게 던전을 노리면 빠르게 많은 광물을 얻을 수 있답니다!", true);
                break;
            case 16:
                SetTutorialInfo("참고로 땅 속 깊이 들어가고 싶다면 비싼 곡괭이를 구매하시면 됩니다!", true);
                break;
            case 17:
                SetTutorialInfo("튜토리얼이 끝났으니 로비로 돌아가도록 하겠습니다 :)", true);
                break;
            case 18:
                EndTutorial();
                break;
        }
    }
}
