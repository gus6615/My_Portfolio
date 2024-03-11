# 기능: 상호작용 대사창

![InteractionDialog](https://github.com/gus6615/My_Portfolio/assets/57510872/b9e26da5-c069-4da8-99ae-a6a02e5477bd)

**기능: 상호작용 대사창**
<br><br>
① **플레이어가 바라보는 방향**에 물체가 있다면 상호작용 대사창을 띄움<br>
② 대사의 개수는 가변적이며 특정 대사에서 **효과음 출력**이 가능<br>
③ 대사가 한글자씩 출력되는 형식이며 **출력 속도 조절** 가능<br>

### 📝 기술적 방법

![InteractionDialog2](https://github.com/gus6615/My_Portfolio/assets/57510872/680e7d56-6616-444c-b0c5-3326fce6f213)

<br>
Scriptable Object로 대사집을 저장하여 관리<br>

![InteractionDialog3](https://github.com/gus6615/My_Portfolio/assets/57510872/b0241b87-203d-4c4f-a22c-367597403ac0)

<br>
Scriptable Object 생성 후 Dialog 구조체 리스트 설정 (가변 데이터라 대사 개수 설정 가능)<br>

① 상호작용 대사는 **남주 및 여주** 모두 수행할 수 있는 기능이므로 **2개의 Dialog**가 존재<br>

② Dialog에는 Type(상호작용, 플레이어), 출력용 오디오, 출력용 Sprite, 대사, 출력 속도가 존재<br>

![InteractionDialog4](https://github.com/gus6615/My_Portfolio/assets/57510872/23dfb743-571b-45eb-af34-33df2dd2f8ea)

<br>
위처럼 일반 상호작용 대화창 뿐만 아니라 인물 프로필 대화창도 구현