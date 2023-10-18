using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using System.Text.RegularExpressions;
using LitJson;
using System;

public class UserData
{
    public string nickname;
    public string owner_indate;
};

public class BackEndLoginManager : MonoBehaviour
{
    static public string gm_inDate = "2022-02-21T01:15:41.012Z"; // 개발자 뒤끝 서버 아이디
    static public BackEndLoginManager instance;
    static public UserData userData; // 본인 로그인 정보
    static public bool isLoginOn;
    static public bool isGM = false; // 출시할 때 GM 꺼야함!

    private new AudioSource audio;
    public GameObject loginingCanvas;
    public GameObject authCanvas, createNicknameCanvas, loginCanvas, findIDCanvas, getPWCanvas, changePWCanvas;
    public InputField paInput_auth, paInput_login, idInput_auth, idInput_login, nickNameInput, emailInput;
    public InputField emailInput_findID, IDInput_getPW, emailInput_getPW, PWInput_old, PWInput_new, IDInput_CPW;
    private Text loginingText;
    public Text authInfoText, createNicknameInfoText, loginInfoText;
    public Text findID_infoText, getPW_infoText, changePW_infoText;

    private bool isCanOnButton;
    private bool isCheckLogin, isNotConnected;
    private bool isLogining, isLoginingFrame; // 로그인 중인가?, 로그인 텍스트 다음 프레임인가?

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            audio = this.GetComponent<AudioSource>();
            loginingText = loginingCanvas.GetComponentInChildren<Text>();
            userData = new UserData();

            DontDestroyOnLoad(this.gameObject);
            Init();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        // 로그인 중...
        if (isLogining && isLoginingFrame)
            StartCoroutine("Logining");

        // 인터넷 연결 확인
        if ((Application.internetReachability == NetworkReachability.NotReachable || isNotConnected) && isCheckLogin)
            StartCoroutine("LoginRealtime");

        if (Backend.IsInitialized)
            Backend.AsyncPoll();
    }

    // 뒤끝 서버 연결
    public void BackEndLogin()
    {
        var bro = Backend.Initialize(true);
        if (bro.IsSuccess())
        {
            isLogining = false;
            loginingCanvas.SetActive(false);
            isNotConnected = false;
            StartCoroutine(AutoLogin());
        }
        else
        {
            Debug.LogError("BackEnd 서버 접속 실패");
            isLogining = true;
            loginingCanvas.SetActive(true);
            isNotConnected = true;
        }
    }

    // 실시간 서버 연결
    IEnumerator LoginRealtime()
    {
        isCheckLogin = false;
        BackEndLogin();
        yield return new WaitForSeconds(1f);
        isCheckLogin = true;
    }

    public void AllClose()
    {
        loginingCanvas.SetActive(false);
        loginCanvas.SetActive(false);
        authCanvas.SetActive(false);
        createNicknameCanvas.SetActive(false);
        findIDCanvas.SetActive(false);
        getPWCanvas.SetActive(false);
        changePWCanvas.SetActive(false);
    }

    public void SetAudio(int _se)
    {
        audio.clip = SaveScript.SEs[_se];
        audio.Play();
    }

    public void Init()
    {
        AllClose();
        isCanOnButton = true;
        isCheckLogin = true;
        isLogining = true;
        isLoginingFrame = true;
        loginingCanvas.SetActive(true);
        loginCanvas.SetActive(true);

        if (!Backend.IsInitialized)
            BackEndLogin();
        else
        {
            isLogining = false;
            loginingCanvas.SetActive(false);
        }
    }

    private IEnumerator Logining()
    {
        isLoginingFrame = false;
        yield return new WaitForSeconds(0.25f);
        loginingText.text += ".";
        if (loginingText.text.Contains("...."))
            loginingText.text = "현재 네트워크 연결 중 입니다.";
        isLoginingFrame = true;
    }

    // 로그인
    public void OnClickLogin()
    {
        if (isCanOnButton)
        {
            loginInfoText.text = "로그인 중입니다...";
            SetAudio(0);
            StartCoroutine("OnClickHold");
            StartCoroutine(Login());
        }
    }

    IEnumerator Login()
    {
        yield return new WaitForSeconds(0.1f);

        BackendReturnObject BRO = Backend.BMember.CustomLogin(idInput_login.text, paInput_login.text);
        string error = BRO.GetErrorCode();
        Debug.Log("로그인 <error> = " + error);

        // 로그인 실패 처리
        switch (error)
        {
            // 아이디 또는 비밀번호가 틀렸을 경우 & 서버 점검
            case "BadUnauthorizedException":
                loginInfoText.text = "아이디 또는 비밀번호가 틀렸습니다.";
                if (BRO.GetMessage().Contains("serverStatus"))
                    loginInfoText.text = "현재 서버 점검 중입니다. 잠시후 접속해주세요.";
                break;
            //  이 경우 콘솔에서 입력한 차단된 사유가 에러코드가 된다.
            case "BadPlayer":
                loginInfoText.text = "차단된 유저입니다.";
                break;
            // 아이디 및 비밀번호 미입력 상태
            case "UndefinedParameterException":
                loginInfoText.text = "아이디 및 비밀번호를 입력해주세요.";
                break;
            // 연결 끊김
            case "HttpRequestException":
                isNotConnected = true;
                break;
            default:
                Debug.Log("로그인 완료");
                if (ConfirmNickname())
                {
                    isLoginOn = true;
                    StartCoroutine(BlindScript.instance.Fade_Game());
                }
                break;
        }
    }

    // 회원가입
    public void OnClickSignUp()
    {
        if (isCanOnButton)
        {
            authInfoText.text = "플레이어 데이터 생성 중...";
            SetAudio(0);
            StartCoroutine("OnClickHold");
            StartCoroutine(SignUp());
        }
    }

    IEnumerator SignUp()
    {
        yield return new WaitForSeconds(0.1f);

        // 회원 가입을 한뒤 결과를 BackEndReturnObject 타입으로 반환한다.
        if (CheckUserInfo())
        {
            BackendReturnObject BRO = Backend.BMember.CustomSignUp(idInput_auth.text, paInput_auth.text, "User");
            string error = BRO.GetErrorCode();
            Debug.Log("회원가입 <error> = " + error);

            switch (error)
            {
                case "BadUnauthorizedException":
                    authInfoText.text = "현재 서버 점검 중입니다. 잠시후 접속해주세요.";
                    break;

                case "DuplicatedParameterException":
                    authInfoText.text = "중복된 아이디가 존재합니다! 다른 아이디를 입력해주세요.";
                    break;

                case "UndefinedParameterException": // 아이디 및 비밀번호 미입력 상태
                    authInfoText.text = "아이디 및 비밀번호를 입력해주세요.";
                    break;

                case "HttpRequestException": // 연결 끊김
                    isNotConnected = true;
                    break;

                default:
                    authCanvas.SetActive(false);
                    createNicknameCanvas.SetActive(true);
                    nickNameInput.text = "";
                    emailInput.text = "";

                    Backend.BMember.CustomLogin(idInput_auth.text, paInput_auth.text);
                    SaveScript.saveData = new SaveData();
                    InsertTable();
                    break;
            }
        }
    }

    // 자동 로그인
    IEnumerator AutoLogin()
    {
        while (!BlindScript.isStartGame)
            yield return null;

        BackendReturnObject bro = Backend.BMember.LoginWithTheBackendToken();
        string error = bro.GetErrorCode();
        if (!bro.IsSuccess())
        {
            StartCoroutine(BlindScript.instance.Fade_LoginFail());
            Debug.Log("자동 로그인 <error> = " + bro.GetMessage());
        }

        switch (error)
        {
            // 토근 기간 만료
            case "GoneResourceException":
                loginInfoText.text = "토큰의 기한이 지나 자동 로그인을 실패했습니다.";
                break;

            // 토근 조건부 만료
            case "BadUnauthorizedException":
                loginInfoText.text = "토큰이 만료되어 자동 로그인에 실패했습니다.";
                break;

            case "BadPlayer":  //  이 경우 콘솔에서 입력한 차단된 사유가 에러코드가 된다.
                loginInfoText.text = "차단된 유저입니다. 자동 로그인을 실패했습니다.";
                break;

            case "NotFoundException": // 토큰 초기화 안됨
                loginInfoText.text = "서버 토큰 초기화에 실패했습니다. 다시 로그인 해주세요.";
                loginCanvas.SetActive(true);
                break;

            case "UndefinedParameterException": // 빈 토큰
                loginCanvas.SetActive(true);
                break;

            case "HttpRequestException": // 연결 끊김
                loginInfoText.text = "서버와의 연결이 끊겼습니다. 다시 로그인 해주세요.";
                isNotConnected = true;
                break;

            case "ForbiddenException":
                loginInfoText.text = "현재 서버 점검 중입니다. 죄송합니다.";
                break;

            case "BadGateway":
                loginInfoText.text = "서버 접근 오류로 인해 자동 로그인에 실패했습니다. 다시 로그인 해주세요.";
                break;

            case "ServerErrorException":
                loginInfoText.text = "서버와의 연결에 에러가 발생했습니다. 다시 로그인 해주세요.";
                break;

            default:
                if (ConfirmNickname())
                {
                    isLoginOn = true;
                    StartCoroutine(BlindScript.instance.Fade_Game());
                }
                break;
        }
    }

    private bool CheckUserInfo()
    {
        string id = idInput_auth.text;
        string pw = paInput_auth.text;

        if (id == "" || pw == "")
        {
            authInfoText.text = "※ 빈칸이 없는지 확인해주세요!";
            return false;
        }

        // 아이디 체크
        if (id.Length < 6 || id.Length > 20)
        {
            authInfoText.text = "※ 아이디의 길이를 6자리 ~ 20자리 사이로 정해주세요!";
            return false;
        }

        if (Regex.IsMatch(id, "^[0-9a-zA-Z가-힣]*$") == false)
        {
            authInfoText.text = "※ 아이디는 영어, 한글, 숫자만 가능합니다! (특수문자 및 공백 불가능)";
            return false;
        }

        // 비밀번호 체크
        if (pw.Length < 4 || pw.Length > 20)
        {
            authInfoText.text = "※ 비밀번호의 길이를 4자리 ~ 20자리 사이로 정해주세요!";
            return false;
        }

        if (Regex.IsMatch(pw, "^[0-9a-zA-Z가-힣]*$") == false)
        {
            authInfoText.text = "※ 비밀번호는 영어, 한글, 숫자만 가능합니다! (특수문자 및 공백 불가능)";
            return false;
        }

        return true;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(email,
                  @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
                  RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch
        {
            return false;
        }
    }

    private bool CheckUserInfo2()
    {
        string nickname = nickNameInput.text;
        string email = emailInput.text;

        if (nickname == "")
        {
            createNicknameInfoText.text = "※ 빈칸이 없는지 확인해주세요!";
            return false;
        }

        // 길이 예외 처리
        if (nickname.Length < 2 || nickname.Length > 8)
        {
            createNicknameInfoText.text = "※ 닉네임의 길이를 2자리 ~ 8자리 사이로 정해주세요!";
            return false;
        }

        if (Regex.IsMatch(nickname, "^[0-9a-zA-Z가-힣]*$") == false)
        {
            createNicknameInfoText.text = "※ 닉네임에는 영어, 한글, 숫자만 가능합니다! (특수문자 및 공백 불가능)";
            return false;
        }

        // 이메일 형식 체크
        if (!IsValidEmail(email))
        {
            createNicknameInfoText.text = "※ 이메일을 정확히 입력해주세요!";
            return false;
        }

        BackendReturnObject BRO = Backend.BMember.UpdateCustomEmail(email);
        string error = BRO.GetErrorCode();
        Debug.Log("이메일 체크 <error> = " + error);

        if (BRO.IsSuccess())
        {
            Debug.Log("이메일 주소 저장 완료");
        }
        else return false;

        return true;
    }

    // 닉네임 및 이메일 생성
    public void OnCreateNickname()
    {
        if (isCanOnButton)
        {
            string nickname = nickNameInput.text;
            createNicknameInfoText.text = "※ 플레이어 정보 기록 중...";
            SetAudio(0);
            StartCoroutine("OnClickHold");
            StartCoroutine(CreateNickname(nickname));
        }
    }

    IEnumerator CreateNickname(string nickname)
    {
        yield return new WaitForSeconds(0.1f);

        if (CheckUserInfo2())
        {
            BackendReturnObject BRO = Backend.BMember.CheckNicknameDuplication(nickname);

            switch (BRO.GetStatusCode())
            {
                // 성공
                case "204":
                    isLoginOn = true;
                    SelectNickName();
                    SetUserInfo();
                    SaveScript.SaveData_Syn();
                    StartCoroutine(BlindScript.instance.Fade_Game());
                    break;
                // 중복된 닉네임
                case "409":
                    createNicknameInfoText.text = "※ 이 닉네임은 현재 사용중입니다!";
                    break;
                // 앞/뒤 공백이 있는 경우
                case "400":
                    createNicknameInfoText.text = "※ 앞/뒤에 공백이 존재합니다! 공백을 제거해주세요.";
                    break;
                // 연결 끊김
                case "HttpRequestException":
                    isNotConnected = true;
                    break;
                // 그 외
                default:
                    createNicknameInfoText.text = "※ 알 수 없는 문제가 발생했습니다. 에러 코드: " + BRO.GetErrorCode();
                    break;
            }
        }
    }

    public void SelectNickName()
    {
        BackendReturnObject BRO = Backend.BMember.CreateNickname(nickNameInput.text);

        if (BRO.IsSuccess())
        {
            //createNicknameCanvas.SetActive(false);
        }
        else
        {
            createNicknameInfoText.text = "[에러 발생] : " + BRO.GetErrorCode();
        }
    }

    public bool ConfirmNickname()
    {
        if (SetUserInfo())
        {
            if (userData.nickname == null)
            {
                BlindScript.instance.loadObject.SetActive(false);
                createNicknameCanvas.SetActive(true);
                return false;
            }
        }
        else return false;
        return true;
    }

    public void GotoLogin()
    {
        AllClose();
        loginCanvas.SetActive(true);
        idInput_login.text = "";
        paInput_login.text = "";
        nickNameInput.text = "";
        loginInfoText.text = "";
    }

    public void OpenAuth()
    {
        SetAudio(0);
        authCanvas.SetActive(true);
        idInput_auth.text = "";
        paInput_auth.text = "";
        authInfoText.text = "";
    }

    public void OpenFindID()
    {
        SetAudio(0);
        findIDCanvas.SetActive(true);
        emailInput_findID.text = "";
        findID_infoText.text = "입력하신 이메일로 아이디를 전송합니다.";
        findID_infoText.color = Color.white;
    }

    public void OpenGetPW()
    {
        SetAudio(0);
        getPWCanvas.SetActive(true);
        IDInput_getPW.text = "";
        emailInput_getPW.text = "";
        getPW_infoText.text = "입력하신 이메일로 임시 비밀번호를 전송합니다.";
        getPW_infoText.color = Color.white;
    }

    public void OpenChangePW()
    {
        SetAudio(0);
        changePWCanvas.SetActive(true);
        IDInput_CPW.text = "";
        PWInput_old.text = "";
        PWInput_new.text = "";
        changePW_infoText.text = "비밀번호를 재설정합니다. 비밀번호를 잊으신 경우에는 임시 비밀번호를 발급받으세요.";
        changePW_infoText.color = Color.white;
    }

    // ID 찾기
    public void FindID()
    {
        if (isCanOnButton)
        {
            string email = emailInput_findID.text;
            SetAudio(0);
            findID_infoText.color = new Color(0.8f, 0.4f, 0.4f, 1f);
            StartCoroutine("OnClickHold");

            if (email == "")
            {
                findID_infoText.text = "빈칸이 없는지 확인해주세요!";
                return;
            }

            string error = Backend.BMember.FindCustomID(email).GetErrorCode();
            Debug.Log("아이디 찾기 <error> : " + error);

            switch (error)
            {
                case "NotFoundException": // 해당 이메일을 가진 ID가 없을 경우
                    findID_infoText.text = "입력하신 이메일을 가진 아이디가 없습니다.";
                    break;

                case " Too Many Request": // 24시간 이내 5번 초과
                    findID_infoText.text = "하루에 5번 초과하여 아이디를 찾을 수 없습니다.";
                    break;

                case "InvalidParameterValue":
                    findID_infoText.text = "잘못된 이메일을 입력하셨습니다.";
                    break;

                case "HttpRequestException": // 연결 끊김
                    isNotConnected = true;
                    break;

                default: // 성공
                    findID_infoText.text = "입력하신 이메일로 아이디를 전송했습니다. 이메일을 확인해주세요.";
                    findID_infoText.color = Color.white;
                    emailInput_findID.text = "";
                    break;
            }
        }
    }

    // 임시 PW 발급
    public void GetPW()
    {
        if (isCanOnButton)
        {
            string id = IDInput_getPW.text;
            string email = emailInput_getPW.text;
            SetAudio(0);
            getPW_infoText.color = new Color(0.8f, 0.4f, 0.4f, 1f);
            StartCoroutine("OnClickHold");

            if (id == "" || email == "")
            {
                getPW_infoText.text = "빈칸이 없는지 확인해주세요!";
                return;
            }

            string error = Backend.BMember.ResetPassword(id, email).GetErrorCode();
            Debug.Log("비밀번호 재발급 <error> : " + error);

            switch (error)
            {
                case "NotFoundException": // 아이디 및 이메일을 가진 ID가 없을 경우
                    getPW_infoText.text = "잘못된 아이디 및 이메일을 입력하셨습니다.";
                    break;

                case " Too Many Request": // 24시간 이내 5번 초과
                    getPW_infoText.text = "하루에 5번 초과하여 비밀번호를 재설정할 수 없습니다.";
                    break;

                case "BadParameterException": // 잘못된 이메일을 입력한 경우
                    getPW_infoText.text = "잘못된 이메일을 입력하셨습니다.";
                    break;

                case "InvalidParameterValue": // 잘못된 형식의 이메일
                    getPW_infoText.text = "잘못된 형식의 이메일입니다. 운영자 측으로 문의 바랍니다.";
                    break;

                case "HttpRequestException": // 연결 끊김
                    isNotConnected = true;
                    break;

                default: // 성공
                    getPW_infoText.text = "입력하신 이메일로 임시 비밀번호를 전송했습니다. 이메일을 확인해주세요.";
                    getPW_infoText.color = Color.white;
                    IDInput_getPW.text = "";
                    emailInput_getPW.text = "";
                    break;
            }
        }
    }

    // PW 변경
    public void ChangePW()
    {
        if (isCanOnButton)
        {
            string id = IDInput_CPW.text;
            string oldPW = PWInput_old.text;
            string newPW = PWInput_new.text;
            audio.clip = SaveScript.SEs[2];
            changePW_infoText.color = new Color(0.8f, 0.4f, 0.4f, 1f);
            StartCoroutine("OnClickHold");

            if (id == "" || oldPW == "" || newPW == "")
            {
                changePW_infoText.text = "빈칸이 없는지 확인해주세요!";
                return;
            }

            // 비밀번호 체크
            if (newPW.Length < 6 || newPW.Length > 20)
            {
                changePW_infoText.text = "※ 비밀번호의 길이를 6자리 ~ 20자리 사이로 정해주세요!";
            }

            if (Regex.IsMatch(newPW, "^[0-9a-zA-Z가-힣]*$") == false)
            {
                changePW_infoText.text = "※ 비밀번호는 영어, 한글, 숫자만 가능합니다! (특수문자 및 공백 불가능)";
            }

            string error = Backend.BMember.CustomLogin(id, oldPW).GetErrorCode();
            Debug.Log("로그인 <error> = " + error);

            // 로그인 실패 처리
            switch (error)
            {
                // 아이디 또는 비밀번호가 틀렸을 경우
                case "BadUnauthorizedException":
                    changePW_infoText.text = "아이디 또는 기존 비밀번호가 틀렸습니다.";
                    break;

                case "BadPlayer":  //  이 경우 콘솔에서 입력한 차단된 사유가 에러코드가 된다.
                    changePW_infoText.text = "차단된 유저입니다. 비밀번호를 변경할 수 없습니다.";
                    break;

                case "HttpRequestException": // 연결 끊김
                    isNotConnected = true;
                    break;

                default:
                    string error2 = Backend.BMember.UpdatePassword(oldPW, newPW).GetErrorCode();
                    Debug.Log("비밀번호 재설정 <error> : " + Backend.BMember.UpdatePassword(oldPW, newPW).GetErrorCode());

                    switch (error2)
                    {
                        case "BadParameterException": // 잘못된 비밀번호를 입력한 경우
                            changePW_infoText.text = "잘못된 기존 비밀번호를 입력하셨습니다.";
                            break;

                        case "HttpRequestException": // 연결 끊김
                            isNotConnected = true;
                            break;

                        default: // 성공
                            changePW_infoText.text = "성공적으로 비밀번호가 변경되었습니다. 잠시후 자동 로그인이 실시됩니다.";
                            changePW_infoText.color = Color.white;
                            IDInput_CPW.text = "";
                            PWInput_new.text = "";
                            PWInput_old.text = "";
                            audio.clip = SaveScript.SEs[0];
                            Invoke("GotoGame", 1f);
                            break;
                    }
                    break;
            }
            audio.Play();
        }
    }

    private void GotoGame()
    {
        changePWCanvas.SetActive(false);
    }

    public bool SetUserInfo()
    {
        BackendReturnObject BRO = Backend.BMember.GetUserInfo(); // 나의 정보 가져오기

        if (BRO.IsSuccess())
        {
            if (BRO.GetReturnValuetoJSON()["row"]["nickname"] != null)
                userData.nickname = GameFuction.ToHangleFromUniCode(BRO.GetReturnValuetoJSON()["row"]["nickname"].ToString());
            userData.owner_indate = BRO.GetReturnValuetoJSON()["row"]["inDate"].ToString();
            return true;
        }
        else
        {
            Debug.Log("SetUserInfo 에러 발생: " + BRO.GetErrorCode());
            return false;
        }
    }

    IEnumerator OnClickHold()
    {
        isCanOnButton = false;
        yield return new WaitForSeconds(1f);
        isCanOnButton = true;
    }

    private void InsertTable()
    {
        // Param은 뒤끝 서버와 통신을 할 떄 넘겨주는 파라미터 클래스 입니다. 
        SaveScript.saveData.user_Indate = Backend.UserInDate;
        SaveScript.saveData.closeDate = Backend.Utils.GetServerTime().GetReturnValuetoJSON()["utcTime"].ToString();
        Param publicParam = new Param();
        Param privateParam = new Param();
        SaveScript.SettingPublicParam(publicParam);
        SaveScript.SettingPrivateParam(privateParam);

        BackendReturnObject BRO1 = Backend.GameData.Insert("publicData", publicParam);
        string error1 = BRO1.GetErrorCode();
        Debug.Log("public param save <error> : " + error1);
        SaveScript.saveData.publicData_Indate = BRO1.GetInDate();

        switch (error1)
        {
            case "NotFoundException":
                Debug.Log("존재하지 않는 tableName인 경우");
                break;

            case "PreconditionFailed":
                Debug.Log("비활성화 된 tableName인 경우");
                break;

            case "ServerErrorException":
                Debug.Log("하나의 row( column들의 집합 )이 400KB를 넘는 경우");
                break;

            default: // 성공
                break;
        }

        BackendReturnObject BRO2 = Backend.GameData.Insert("privateData", privateParam);
        string error2 = BRO2.GetErrorCode();
        Debug.Log("private param save <error> : " + error2);
        SaveScript.saveData.privateData_Indate = BRO2.GetInDate();
        Debug.Log(BRO2.GetInDate() + " / " + BRO2.GetStatusCode());

        switch (error2)
        {
            case "NotFoundException":
                Debug.Log("존재하지 않는 tableName인 경우");
                break;

            case "PreconditionFailed":
                Debug.Log("비활성화 된 tableName인 경우");
                break;

            case "ServerErrorException":
                Debug.Log("하나의 row( column들의 집합 )이 400KB를 넘는 경우");
                break;

            default: // 성공
                break;
        }

        SaveScript.instance.SaveData_Asyn(true);
    }

    private void CheckLock()
    {
        Where where = new Where();
        where.Equal("owner_inDate", gm_inDate);
        BackendReturnObject BRO = Backend.GameData.Get("lockdown", where);

        if (BRO.IsSuccess())
        {
            SetLockData(BRO.GetReturnValuetoJSON()["rows"][0]);
        }
        else
        {
            Debug.LogError("LockDown 정보 반환 실패: " + BRO.GetErrorCode());
        }
    }

    private void SetLockData(JsonData tableData)
    {
        SaveScript.isGameLock = bool.Parse(tableData["game_lock"][0].ToString());
        SaveScript.isShopLock = bool.Parse(tableData["shop_lock"][0].ToString());
        SaveScript.isInvenLock = bool.Parse(tableData["inven_lock"][0].ToString());
        SaveScript.isMineLock = bool.Parse(tableData["mine_lock"][0].ToString());
        SaveScript.isGMLock = bool.Parse(tableData["isGM"][0].ToString());
    }

    public void SetGame()
    {
        SaveScript.LoadData();
        CheckLock();
        // 운영자 모드
        if (SaveScript.isGMLock && isGM)
            SaveScript.saveData = GM.GetSaveDataAsFloor(14);

        Chat.instance.SetHandler();
        SaveScript.stat.SetStat();
        SaveScript.saveRank.StartSetRankData();
        SaveScript.SetFPS();
        MainScript.instance.SetBasicInfo();
        MainPlayer.instance.Init();
        MainInfoUI.instance.GetTempNotice();
        if (SaveScript.saveData.attendance_day != SaveScript.dateTime.Day)
            MainAttendance.instance.OnUI();
        for (int i = 0; i < SaveScript.achievementNum; i++)
            AchievementCtrl.instance.SetAchievementGoal(i);
        MainAchievementUI.instance.SetReceiveCanInfo();
        MainQuestUI.instance.SetCanInfoActive();
        MainBonusUI.instance.OnOffCanvas();
        QuestCtrl.CheckAllQuest();
        QuestCtrl.instance.SetUI(false);
    }
}