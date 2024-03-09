using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChatUI : MonoBehaviour
{
    public Canvas openChatObject;

    // Start is called before the first frame update
    void Start()
    {
        openChatObject.gameObject.SetActive(false);
    }


    public void OpenOpenChat()
    {
        MainScript.instance.SetAudio(0);
        openChatObject.gameObject.SetActive(true);
    }

    public void YesOpenChat()
    {
        MainScript.instance.SetAudio(0);
        Application.OpenURL("https://open.kakao.com/o/g9qPdN8c");
        openChatObject.gameObject.SetActive(false);
    }

    public void NoOpenChat()
    {
        MainScript.instance.SetAudio(0);
        openChatObject.gameObject.SetActive(false);
    }
}
