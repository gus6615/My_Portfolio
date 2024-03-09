using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;

public class Channel
{
    public string groupName;
    public string alias;
    public string inDate;
    public int maxUserCount;
    public int joinedUserCount;
    public string serverAddress;
    public ushort serverPort;

    public Channel(JsonData json)
    {
        groupName = json["groupName"].ToString();
        alias = json["alias"].ToString();
        inDate = json["inDate"].ToString();
        maxUserCount = int.Parse(json["maxUserCount"].ToString());
        joinedUserCount = int.Parse(json["joinedUserCount"].ToString());
        serverAddress = json["serverAddress"].ToString();
        serverPort = ushort.Parse(json["serverPort"].ToString());
    }
}

public class Chat : MonoBehaviour
{
    static public Chat instance;
    static public List<Channel> channels = new List<Channel>();
    static public string opendChatStr, closedChatStr;
    static public List<string> userInfoNameList = new List<string>(); // 현재 접속 중인 유저 이름들
    static public List<int> userInfoRankList = new List<int>(); // 현재 접속 중인 유저의 ranks;
    static public List<string> userMessageList = new List<string>();
    static public List<string> userMessageNameList = new List<string>();
    static public List<int> userMessageRankList = new List<int>();
    static public List<string> blockedUserList = new List<string>();
    static public List<int> blockedUserRankList = new List<int>();

    readonly string chatName = "땅파서 돈벌기";
    readonly public string[] systemChecks = { "$SYSTEM|&1", "$SYSTEM|&2", "$SYSTEM|&3" }; // 붉은색, 초록색, 푸른색
    readonly public int[] systemIndexs = { -100, -101, -102 };
    readonly public string no_rankCheck = "$NORANK|&1";
    readonly public Color[] systemColors = { new Color(1f, 0.6f, 0.6f, 0.2f), new Color(0.6f, 1f, 0.6f, 0.2f), new Color(0.6f, 0.6f, 1f, 0.2f) };
    readonly int maxMessageNum = 30;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        // Polling
        if (Backend.IsInitialized)
            Backend.Chat.Poll();
    }

    public void SetChatStatus()
    {
        // 뒤끝챗 활성화
        return;
        if (!Backend.IsInitialized || SaveScript.saveRank.isError) return;
        if (Backend.Chat.IsChatConnect(ChannelType.Public))
        {
            Debug.Log("이미 채팅 로그인 되어 있음");
            return;
        }

        BackendReturnObject chatStatusBRO = Backend.Chat.GetChatStatus();
        ResetList();

        if (chatStatusBRO.IsSuccess())
        {
            // string yn = chatStatusBRO.GetReturnValuetoJSON()["chatServerStatus"]["chatServer"].ToString();
            Debug.Log("뒤끝챗 활성화 성공");
            string serverAddress = "";
            string alias = "";
            string inDate = "";
            ushort serverPort = 50000;

            BackendReturnObject channelBRO = Backend.Chat.GetGroupChannelList(chatName);
            if (chatStatusBRO.IsSuccess())
            {
                Debug.Log("뒤끝챗 채널 반환 성공");
                for (int i = 0; i < channelBRO.FlattenRows().Count; i++)
                    channels.Add(new Channel(channelBRO.FlattenRows()[i]));

                for (int i = 0; i < channels.Count; ++i)
                {
                    // 순서대로 190명 이상 들어가 있지 않는 채널을 찾기
                    if (channels[i].joinedUserCount >= 190)
                    {
                        continue;
                    }
                    else
                    {
                        // 들어갈 채널을 찾았으면 정보를 받고 채널 탐색 종료(break)
                        serverAddress = channels[i].serverAddress;
                        alias = channels[i].alias;
                        inDate = channels[i].inDate;
                        serverPort = channels[i].serverPort;
                        break;
                    }
                }

                // 얻은 인자 값으로 입장
                ErrorInfo errorInfo;
                Backend.Chat.JoinChannel(ChannelType.Public, serverAddress, serverPort, chatName, inDate, out errorInfo);
                Debug.Log("서버 정보 : " + errorInfo);
            }
            else
            {
                Debug.Log("뒤끝챗 채널 반환 실패 : " + channelBRO.GetErrorCode());
            }
        }
        else
        {
            Debug.Log("뒤끝챗 활성화 실패 : " + chatStatusBRO.GetErrorCode());
        }
    }

    public void SetBlockedUser()
    {
        JsonData blockList = Backend.Chat.GetBlockUserList();
        blockedUserList.Clear();
        blockedUserRankList.Clear();

        for (int i = 0; i < blockList.Count; i++)
        {
            blockedUserList.Add(blockList[i].ToString());
            blockedUserRankList.Add(SaveScript.saveRank.GetRankIndex(blockList[i].ToString(), 0));
        }
    }

    public void SetUserRankList()
    {
        for (int i = 0; i < userInfoRankList.Count; i++)
            userInfoRankList[i] = SaveScript.saveRank.GetRankIndex(userInfoNameList[i], 0);
        for (int i = 0; i < userMessageRankList.Count; i++)
            userMessageRankList[i] = SaveScript.saveRank.GetRankIndex(userMessageNameList[i], 0);
    }

    /// <summary>
    /// System 채팅을 보내는 함수
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type">1은 얼티밋 알림, 2는 미스틱 알림</param>
    public void SetSystemMessage(string message, int type)
    {
        if (!Backend.Chat.IsChatConnect(ChannelType.Public))
            return;
        Debug.Log(message + systemChecks[type]);
        Backend.Chat.ChatToChannel(ChannelType.Public, message + systemChecks[type]);
    }

    public void SetUserMessageNum()
    {
        if (userMessageList.Count >= maxMessageNum)
        {
            userMessageList.RemoveAt(userMessageList.Count - 1);
            userMessageNameList.RemoveAt(userMessageNameList.Count - 1);
            userMessageRankList.RemoveAt(userMessageRankList.Count - 1);
        }
    }

    public void ResetList()
    {
        userInfoNameList.Clear();
        userInfoRankList.Clear();
    }

    public void SetHandler()
    {
        // 채널 입장 Handler
        Backend.Chat.OnJoinChannel = (JoinChannelEventArgs args) =>
        {
            // 입장에 성공한 경우
            if (args.ErrInfo == ErrorInfo.Success)
            {
                // 내가 접속한 경우 
                if (!args.Session.IsRemote)
                {
                    closedChatStr = "채팅 채널에 접속했습니다.";

                    BackendReturnObject bro = Backend.Chat.GetGroupChannelList(chatName);

                    //채팅 채널 uuid 받아오기
                    string channelIndate = bro.GetReturnValuetoJSON()["rows"][0]["inDate"].ToString();

                    //uuid를 이용하여 해당 일반 채널의 최근 채팅 내역 가져오기
                    BackendReturnObject result = Backend.Chat.GetRecentChat(ChannelType.Public, channelIndate, maxMessageNum);

                    for (int i = 0; i < result.Rows().Count; i++)
                    {
                        string nickname = result.Rows()[i]["nickname"].ToString();
                        string message = result.Rows()[i]["message"].ToString();
                        bool isCreate = false;
                        if (Backend.Chat.IsUserBlocked(nickname)) continue;

                        for (int j = 0; j < systemChecks.Length; j++)
                        {
                            if (message.Contains(systemChecks[j]))
                            {
                                message = message.Substring(0, message.Length - systemChecks[j].Length);
                                userMessageList.Insert(0, message);
                                userMessageNameList.Insert(0, systemChecks[j]);
                                userMessageRankList.Insert(0, systemIndexs[j]);
                                isCreate = true;
                                break;
                            }
                        }

                        if (!isCreate)
                        {
                            userMessageList.Insert(0, message);
                            userMessageNameList.Insert(0, nickname);
                            userMessageRankList.Insert(0, SaveScript.saveRank.GetRankIndex(nickname, 0));
                        }
                    }
                }
                //다른 유저가 접속한 경우
                else
                {
                    closedChatStr = $"[SYSTEM] '{args.Session.NickName}'님이 접속했습니다.";
                    userInfoNameList.Add(args.Session.NickName);
                    userInfoRankList.Add(SaveScript.saveRank.GetRankIndex(args.Session.NickName, 0));
                    userMessageList.Insert(0, $"[SYSTEM] '{args.Session.NickName}'님이 접속했습니다.");
                    userMessageNameList.Insert(0, systemChecks[0]);
                    userMessageRankList.Insert(0, systemIndexs[0]);
                    SetUserMessageNum();
                }
            }
            else
            {
                //에러가 발생했을 경우
                closedChatStr = "입장 도중 에러가 발생했습니다. 잠시후 다시 시도해주세요.";
            }
            if (SceneManager.GetActiveScene().name == "MainScene" || SceneManager.GetActiveScene().name == "GameScene")
                ChatUI.instance.SetChatStr();
        };

        // 채널 퇴장 Handler
        Backend.Chat.OnLeaveChannel = (LeaveChannelEventArgs args) =>
        {
            //퇴장에 성공한 경우
            if (args.ErrInfo == ErrorInfo.Success)
            {
                // 내가 퇴장한 경우 
                if (!args.Session.IsRemote)
                {
                    closedChatStr = "채널에서 퇴장했습니다.";
                }
                // 다른 유저가 퇴장한 경우
                else
                {
                    int index = userInfoNameList.FindIndex(x => x.Contains(args.Session.NickName));
                    userInfoNameList.RemoveAt(index);
                    userInfoRankList.RemoveAt(index);
                }

                if (SceneManager.GetActiveScene().name == "MainScene" || SceneManager.GetActiveScene().name == "GameScene")
                    ChatUI.instance.SetChatStr();
            }
            else
            {
                // 에러가 발생했을 경우
                Debug.Log("퇴장 도중 에러가 발생했습니다 : " + args.ErrInfo.Reason);
            }
        };

        // 최초 입장 시 현재 채팅에 존재하는 유저 조회 Handler
        Backend.Chat.OnSessionListInChannel = (SessionListInChannelEventArgs args) =>
        {
            //성공한 경우
            if (args.ErrInfo == ErrorInfo.Success)
            {
                // 게이머 정보를 참여자 리스트에 추가
                foreach (SessionInfo session in args.SessionList)
                {
                    userInfoNameList.Add(session.NickName);
                    userInfoRankList.Add(SaveScript.saveRank.GetRankIndex(session.NickName, 0));
                }
            }
            else
            {
                //실패한 경우
                Debug.LogError("유저 목록 불러오기 중 에러가 발생했습니다" + args.ErrInfo.Reason);
            }
            if (SceneManager.GetActiveScene().name == "MainScene" || SceneManager.GetActiveScene().name == "GameScene")
                ChatUI.instance.SetChatStr();
        };

        // 채팅 송수신 이벤트
        Backend.Chat.OnChat = (ChatEventArgs args) =>
        {
            if (args.ErrInfo == ErrorInfo.Success)
            {
                bool isCreate = false;

                for (int i = 0; i < systemChecks.Length; i++)
                {
                    if (args.Message.Contains(systemChecks[i]))
                    {
                        string message = args.Message.Substring(0, args.Message.Length - systemChecks[i].Length);
                        userMessageList.Insert(0, message);
                        userMessageNameList.Insert(0, systemChecks[i]);
                        userMessageRankList.Insert(0, systemIndexs[i]);
                        SetUserMessageNum();
                        closedChatStr = message;
                        isCreate = true;
                        break;
                    }
                }

                if (!isCreate)
                {
                    userMessageList.Insert(0, args.Message);
                    userMessageNameList.Insert(0, args.From.NickName);
                    userMessageRankList.Insert(0, SaveScript.saveRank.GetRankIndex(args.From.NickName, 0));
                    SetUserMessageNum();

                    if (args.Message.Contains(no_rankCheck))
                        closedChatStr = args.From.NickName + " : " + args.Message.Split(no_rankCheck)[1];
                    else
                        closedChatStr = args.From.NickName + " : " + args.Message;
                }
            }
            else if (args.ErrInfo.Category == ErrorCode.BannedChat)
            {
                // 도배방지 메세지 
                if (args.ErrInfo.Detail == ErrorCode.BannedChat)
                {
                    userMessageList.Insert(0, "[SYSTEM] 도배로 인해 10초간 채팅 금지");
                    userMessageNameList.Insert(0, systemChecks[0]);
                    userMessageRankList.Insert(0, systemIndexs[0]);
                    SetUserMessageNum();
                }
            }
            if (SceneManager.GetActiveScene().name == "MainScene" || SceneManager.GetActiveScene().name == "GameScene")
                ChatUI.instance.SetChatStr();
        };

        // 운영자 공지
        Backend.Chat.OnNotification = (NotificationEventArgs args) =>
        {
            userMessageList.Insert(0, args.Subject + " " + args.Message);
            userMessageNameList.Insert(0, systemChecks[0]);
            userMessageRankList.Insert(0, systemIndexs[0]);
            SetUserMessageNum();
            if (SceneManager.GetActiveScene().name == "MainScene" || SceneManager.GetActiveScene().name == "GameScene")
                ChatUI.instance.SetChatStr();
        };

        // 비속어 설정
        Backend.Chat.SetFilterUse(true);
        // 도배 방지
        Backend.Chat.SetRepeatedChatBlockMessage("도배는 자제해주시기 바랍니다!");
    }
}
