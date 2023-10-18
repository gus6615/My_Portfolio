using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatForm : MonoBehaviour
{
    static public ChatForm currentForm;
    static public bool isBlocking;
    public Image formImage;
    public Image[] playerImages; // pick, hat, ring, pendant
    public Text nameText, infoText;
    public Image blockImage, unblockImage;
    public string nickname;
    public bool isOn;

    public void SetForm(string message)
    {
        string[] str = message.Split(Chat.instance.no_rankCheck);
        if (str.Length != 2)
            return;

        nickname = str[0];
        nameText.text = nickname + " (일반 유저)";
        infoText.text = str[1];
        nameText.color = new Color(1f, 1f, 1f); // 흰색
        blockImage.gameObject.SetActive(false);
        unblockImage.gameObject.SetActive(false);
    }

    public void SetForm(string message, RankData rankdata)
    {
        infoText.text = message;
        blockImage.gameObject.SetActive(false);
        unblockImage.gameObject.SetActive(false);
        nickname = rankdata.nickname;
        playerImages[playerImages.Length - 1].color = Color.white;
        for (int i = 0; i < playerImages.Length - 1; i++)
        {
            if (rankdata.equipments[i] != -1)
                playerImages[i].color = SaveScript.toolColors[rankdata.equipments[i]];
            else
                playerImages[i].color = new Color(1f, 1f, 1f, 0f);
        }

        if (rankdata.rank <= SaveScript.saveRank.userNum_3)
        {
            nameText.text = rankdata.nickname + " (♚신화♚ - " + rankdata.rank + "위)";
            nameText.color = new Color(1f, 0.6f, 0.6f); // 붉은색
        }
        else if (rankdata.rank <= SaveScript.saveRank.userNum_2)
        {
            nameText.text = rankdata.nickname + " (✾전설✾ - " + rankdata.rank + "위)";
            nameText.color = new Color(0.6f, 0.6f, 1f); // 푸른색
        }
        else
        {
            nameText.text = rankdata.nickname + " (일반 유저)";
            nameText.color = new Color(1f, 1f, 1f); // 흰색
        }
    }

    public void OnChatForm()
    {
        if (currentForm != null && currentForm != this)
            currentForm.SetInit();

        ChatUI.instance.SetAudio(0);
        isOn = !isOn;
        if (isOn)
        {
            bool isUnblock = Backend.Chat.IsUserBlocked(nickname);
            currentForm = this;

            if (isUnblock)
            {
                // 차단된 유저
                blockImage.gameObject.SetActive(false);
                unblockImage.gameObject.SetActive(true);
            }
            else
            {
                // 일반 유저
                blockImage.gameObject.SetActive(true);
                unblockImage.gameObject.SetActive(false);
            }
        }
        else
        {
            currentForm = null;
            SetInit();
        }
    }

    public void Block()
    {
        if (isBlocking) return;
        isBlocking = true;
        ChatUI.instance.SetAudio(0);
        Backend.Chat.BlockUser(nickname, blockCallback =>
        {
            // 성공
            if (blockCallback)
            {
                Debug.Log("차단 성공");
                Chat.instance.SetBlockedUser();
                ChatUI.instance.SetChatStr();
                isBlocking = false;
            }
            else
            {
                Debug.Log("차단 실패");
            }
        });
    }

    public void UnBlock()
    {
        if (isBlocking) return;
        bool isUnblock = Backend.Chat.UnblockUser(nickname);
        ChatUI.instance.SetAudio(0);

        if (isUnblock)
        {
            Debug.Log("차단 해제 성공");
            Chat.instance.SetBlockedUser();
            ChatUI.instance.SetChatStr();
        }
        else
        {
            Debug.Log("차단 해제 실패");
        }
    }

    public void SetInit()
    {
        blockImage.gameObject.SetActive(false);
        unblockImage.gameObject.SetActive(false);
        isOn = false;
    }
}
