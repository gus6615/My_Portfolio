using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatUI : MonoBehaviour
{
    static public ChatUI instance;

    public new AudioSource audio;
    public GameObject closedChat, openedChat;
    public Transform chatContentPanel, userContentPanel, blockContentPanel;
    public GameObject chatPrefab, userPrefab, systemPrefab;
    public Image userInfo, blockInfo;
    public Image userInfoButton, blockInfoButton;
    public Text closedChatText;
    public InputField chatField;

    private RectTransform chatTr, userTr, blockTr;
    private float current_chatTr_height;
    private bool isOnChat, isOnUserInfo, isOnBlockInfo;

    // Temp 변수들
    ChatForm[] forms;
    ChatForm form;
    Text[] texts;
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetInit();
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    public void SetInit()
    {
        chatTr = chatContentPanel.GetComponent<RectTransform>();
        userTr = userContentPanel.GetComponent<RectTransform>();
        blockTr = blockContentPanel.GetComponent<RectTransform>();
        closedChat.SetActive(true);
        openedChat.SetActive(false);
        userInfo.gameObject.SetActive(false);
        blockInfo.gameObject.SetActive(false);
        chatField.characterLimit = 40;
        SetChatStr();
        if (!Backend.IsInitialized || !Backend.Chat.IsChatConnect(ChannelType.Public))
            closedChatText.text = "채팅 채널에 접속중입니다...";
    }

    public void OnOffChat()
    {
        if (!Backend.Chat.IsChatConnect(ChannelType.Public))
        {
            closedChatText.text = "현재 서버와 연결이 끊겼습니다. 게임을 다시 접속해주시길 바랍니다.";
            SetAudio(2);
            return;
        }

        isOnChat = !isOnChat;
        isOnUserInfo = false;
        isOnBlockInfo = false;
        current_chatTr_height = 3120f;
        openedChat.SetActive(isOnChat);
        closedChat.SetActive(!isOnChat);
        userInfo.gameObject.SetActive(false);
        blockInfo.gameObject.SetActive(false);
        userInfoButton.color = Color.white;
        blockInfoButton.color = Color.white;
        if (ChatForm.currentForm != null)
        {
            ChatForm.currentForm.SetInit();
            ChatForm.currentForm = null;
        }
        StartCoroutine(SetUserPanelPos());
        StartCoroutine(SetChatPanelPos());
        SetAudio(0);
    }

    public void OnOffUserInfo()
    {
        isOnUserInfo = !isOnUserInfo;
        userInfo.gameObject.SetActive(isOnUserInfo);
        isOnBlockInfo = false;
        blockInfo.gameObject.SetActive(false);
        userTr.anchoredPosition = Vector2.zero;
        blockInfoButton.color = Color.white;
        if (isOnUserInfo)
            userInfoButton.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        else
            userInfoButton.color = Color.white;
        SetAudio(0);
    }

    public void OnOffBlockInfo()
    {
        isOnBlockInfo = !isOnBlockInfo;
        blockInfo.gameObject.SetActive(isOnBlockInfo);
        isOnUserInfo = false;
        userInfo.gameObject.SetActive(false);
        blockTr.anchoredPosition = Vector2.zero;
        userInfoButton.color = Color.white;
        if (isOnBlockInfo)
            blockInfoButton.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        else
            blockInfoButton.color = Color.white;
        SetAudio(0);
    }

    public void SendChat()
    {
        if (!Backend.Chat.IsChatConnect(ChannelType.Public))
        {
            Chat.instance.SetChatStatus();
            chatField.text = "현재 채팅 채널과 연결되어 있지 않습니다. 잠시후 다시 접속해주세요.";
            return;
        }
        if (chatField.text == "채팅을 입력해주세요." || chatField.text == "") return;

        SetAudio(0);
        if (SaveScript.saveRank.myRankData.rank == SaveRank.NO_RANK)
            Backend.Chat.ChatToChannel(ChannelType.Public, SaveScript.saveRank.myRankData.nickname + Chat.instance.no_rankCheck + chatField.text);
        else
            Backend.Chat.ChatToChannel(ChannelType.Public, chatField.text);
        chatField.text = "";
    }

    public void SetChatStr()
    {
        // 채팅 유저 정보
        texts = userContentPanel.GetComponentsInChildren<Text>();
        for (int i = 0; i < texts.Length; i++)
            Destroy(texts[i].gameObject);

        // 정렬
        int index;
        int temp;
        string temp2;
        for (int i = 0; i < Chat.userInfoRankList.Count; i++)
        {
            index = i;
            for (int j = i; j < Chat.userInfoRankList.Count; j++)
                if (Chat.userInfoRankList[j] < Chat.userInfoRankList[index])
                    index = j;

            temp = Chat.userInfoRankList[index];
            temp2 = Chat.userInfoNameList[index];
            Chat.userInfoRankList[index] = Chat.userInfoRankList[i];
            Chat.userInfoNameList[index] = Chat.userInfoNameList[i];
            Chat.userInfoRankList[i] = temp;
            Chat.userInfoNameList[i] = temp2;
        }

        for (int i = 0; i < Chat.userInfoNameList.Count; i++)
        {
            text = Instantiate(userPrefab, userContentPanel).GetComponent<Text>();
            if (Chat.userInfoRankList[i] != SaveRank.NO_RANK)
            {
                text.text = Chat.userInfoNameList[i] + " (" + (Chat.userInfoRankList[i] + 1) + "위)\n";
                if (Chat.userInfoRankList[i] < SaveScript.saveRank.userNum_3)
                    text.color = new Color(1f, 0.6f, 0.6f);
                else if (Chat.userInfoRankList[i] < SaveScript.saveRank.userNum_2)
                    text.color = new Color(0.6f, 0.6f, 1f);
                else
                    text.color = new Color(1f, 1f, 1f);
            }
            else
                text.text = Chat.userInfoNameList[i] + "\n";
        }
        // 차단 유저 정보
        texts = blockContentPanel.GetComponentsInChildren<Text>();
        for (int i = 0; i < texts.Length; i++)
            Destroy(texts[i].gameObject);

        for (int i = 0; i < Chat.blockedUserList.Count; i++)
        {
            text = Instantiate(userPrefab, blockContentPanel).GetComponent<Text>();
            text.GetComponent<Button>().onClick.AddListener(UnBlock);
            text.GetComponent<Order>().order = Chat.blockedUserRankList[i];
            if (Chat.blockedUserRankList[i] != SaveRank.NO_RANK)
            {
                text.text = Chat.blockedUserList[i] + " (" + (Chat.blockedUserRankList[i] + 1) + "위)\n";
                if (Chat.blockedUserRankList[i] < SaveScript.saveRank.userNum_3)
                    text.color = new Color(1f, 0.6f, 0.6f);
                else if (Chat.blockedUserRankList[i] < SaveScript.saveRank.userNum_2)
                    text.color = new Color(0.6f, 0.6f, 1f);
                else
                    text.color = new Color(1f, 1f, 1f);
            }
            else
                text.text = Chat.blockedUserList[i] + "\n";
        }

        closedChatText.text = Chat.closedChatStr;
        SetContent();
    }

    public void SetContent()
    {
        forms = chatContentPanel.GetComponentsInChildren<ChatForm>();
        for (int i = 0; i < forms.Length; i++)
            Destroy(forms[i].gameObject);

        for (int i = Chat.userMessageList.Count - 1; i >= 0; i--)
        {
            bool isCreate = false;

            for (int j = 0; j < Chat.instance.systemIndexs.Length; j++)
            {
                if (Chat.userMessageRankList[i] == Chat.instance.systemIndexs[j])
                {
                    // SYSTEM INFO
                    form = Instantiate(systemPrefab, chatContentPanel).GetComponent<ChatForm>();
                    form.infoText.text = Chat.userMessageList[i];
                    form.formImage.color = Chat.instance.systemColors[j];
                    isCreate = true;
                    break;
                }
            }

            if (!isCreate)
            {
                
                if (Chat.userMessageRankList[i] == SaveRank.NO_RANK)
                {
                    // 일반 전용 메세지
                    form = Instantiate(chatPrefab, chatContentPanel).GetComponent<ChatForm>();
                    form.SetForm(Chat.userMessageList[i]);
                }
                else
                {
                    // 랭커 전용 메세지
                    form = Instantiate(chatPrefab, chatContentPanel).GetComponent<ChatForm>();
                    form.SetForm(Chat.userMessageList[i], SaveScript.saveRank.rankData[Chat.userMessageRankList[i]]);
                }
            }
        }
        StartCoroutine(SetUserPanelPos());
        StartCoroutine(SetChatPanelPos());
    }

    public void UnBlock()
    {
        string nickname = (EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text).Split(" ")[0];
        bool isUnblock = Backend.Chat.UnblockUser(nickname);
        SetAudio(0);

        if (isUnblock)
        {
            Debug.Log("차단 해제 성공");
            Chat.instance.SetBlockedUser();
            instance.SetChatStr();
        }
        else
        {
            Debug.Log("차단 해제 실패");
        }
    }

    public void SetChatTr()
    {
        current_chatTr_height = chatTr.anchoredPosition.y;
    }

    IEnumerator SetUserPanelPos()
    {
        yield return null;
        userTr.anchoredPosition = Vector2.zero;
        blockTr.anchoredPosition = Vector2.zero;
    }

    IEnumerator SetChatPanelPos()
    {
        yield return null;
        chatTr.anchoredPosition = new Vector2(0, current_chatTr_height);
    }
}
