# 기능: 시네마틱 연출 

![Cinematic1](https://github.com/gus6615/My_Portfolio/assets/57510872/68277323-0ed2-4403-8071-567fe952cd41)

**기능: 시네마틱 연출**
<br><br>
① 특정 이벤트를 수행했을 때 해당 이벤트에 대한 시네마틱 연출이 등장<br>

② 다양한 대사와 이벤트 처리를 수행<br>

### 📝 기술적 방법

![Cinematic2](https://github.com/gus6615/My_Portfolio/assets/57510872/d20cf76a-7775-4766-9907-bf0e49a4acc3)
![Cinematic3](https://github.com/gus6615/My_Portfolio/assets/57510872/fc5eef32-0f30-4dfa-a302-12ebf2eae6fe)

CutSceneSO: 컷씬에 사용되는 이벤트 관리 스크립트 오브젝트이다. 여기서 어떤 대화를 띄울 것인지 설정할 수 있다.<br><br>

![Cinematic4](https://github.com/gus6615/My_Portfolio/assets/57510872/b2fd4985-c695-4428-8f62-f12f751ef173)

추가로 컷씬 액션마다 카메라 관련 연출을 동시에 수행할 수 있다.<br>

![Cinematic5](https://github.com/gus6615/My_Portfolio/assets/57510872/b4250380-c6c1-4b18-b13e-8a87750bb02f)

CutsceneFunction 클래스에서 SO에 담을 수 없는 씬 오브젝트 연출(ex. 특정 물체 이동)을 수행한다.<br>

즉, CutSceneSO와 CutSceneFunction 2개가 하나의 컷씬을 보여준다. 이때, CutSceneCtrl 컨트롤러 클래스에서 CutSceneSO와 CutSceneFunction을 읽어 컷씬을 실행할 수 있다. <br>
