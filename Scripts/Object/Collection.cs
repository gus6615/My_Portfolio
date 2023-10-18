using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection
{
    /// <summary> 컬렉션 info string </summary>
    private static string[] infos =
    {
        "검은 색을 띈 탄소 덩어리이다.\n연료로 많이 사용된다.",
        "흰색 결정을 가진 결정형 광물이다.\n수정이라고도 부른다.",
        "매우 흔한 광물이다.\n대체로 흰색 혹은 분홍색을 띈다.",
        "보석의 일종으로서, 녹주석 중 청록색을 띠는 광물이다. 예쁘다!",

        "열 전도성과 전기 전도성이 매우 높다. 표면이 주황빛을 띄는 것이 특징이다.",
        "부식 저항을 높이는데 사용된다. 많이 사용되는 금속 중 하나이다.",
        "독성이 있는 전이후 금속으로, 자르고 난 단면은 푸르스름한 빛을 띄는 광물이다.",
        "다채로운 색을 가지고 있다. 고대에는 신성한 돌로 여겨졌다.",
        "열전도성과 전기전도성이 가장 큰 금속이다. 황을 포함한 독을 검출할 때 사용하기도 했다.",

        "컴퓨터나 태양전지 등에도 사용되는 반도체의 대표적인 소재이다. 실리콘이라고도 불린다.",
        "탄산염 광물로 이루어진 변성암이다. 주로 조각상의 재료나 건축 자재로 쓰인다.",
        "다양한 분야에서 기계 및 기구를 만드는 데 이용되는 광물이다.",
        "푸른색의 보석으로 강옥의 일종이다. 청옥이라고도 한다.",
        "석명질의 광물 가운데 투명하게 결정된 자줏빛 수정이다.",
        "최고급의 보석 중 하나이다. 매우 비싼 값에 거래가 되고 있다.",
        "초록색의 연옥으로 빛이 매우 아름다워 고급 보석으로 쓰인다.",

        "알루미늄을 풍부하게 가지고 있는 광물이다. 알루미늄과 갈륨의 주 원료이다.",
        "굳고 단단한 백색 또는 회백색의 매우 단단한 금속이다.",
        "매우 무겁고 위험하다. 원자력 발전소에서는 전기 에너지를 얻는 에너지원으로 쓰인다.",
        "붉고 아름다운 보석이다. 서양에서는 건강을 지켜주는 돌로 여겨지기도 했다.",
        "바다의 색을 한 보석이다. 옛날에는 바다의 힘이 깃든 부적으로 소중히 여겨졌다.",
        "다채로운 색을 띠는 광물이다. 쉽게 깨지지 조심해야 한다.",
        "새털처럼 가벼우나, 강도는 강철을 뛰어넘는 훌륭한 귀금속이다.",

        "갈색이나 보랏빛이 감도는 검은색을 띄는 광물이다.",
        "물에 뜰 정도로 가장 밀도가 낮은 광물이다. 피부에 닿으면 부식 위험이 있으니 조심하자.",
        "극도로 강해 거의 파괴되지 않는 광물이다. 그리고 매우 무겁다.",
        "녹이 슬지 않기 때문에 장신구 제작에 많이 사용된다. 노란 빛을 띄는게 특징이다.",
        "무척 단단한 광물로, 순수 천연 광물 중에서는 굳기 제일 크다. 엄청 비싸다.",
        "단단하고 강자성을 띤 은백색 금속이다. 합금과 안료로 사용된다.",

        "엄청난 열에도 녹지 않는 광물이다. 다만 낮은 온도에서 쉽게 깨진다.",
        "장미처럼 예쁜 결정을 담고 있는 광물이다. 표면이 날카로우니 조심하자.",
        "스스로 엄청난 열을 내뿜는 광물이다. 맨손으로 만지면 화상을 입는다.",
        "특이하게 어둠속에서 녹색 빛을 내뿜는 광물이다. 천 년이 지나도 빛난다.",
        "천 년이 지난 제미나이트이다. 매우 불안정한 에너지를 담고 있다.",
        "만 년이 지난 제미나이트이다. 에너지를 모두 소모했지만 엄청난 단단함을 가졌다.",
        "매우 희귀한 수정 중 하나이다. 드래곤의 뼈가 결정화가 되었다는 소문도 있다.",

        "주위의 모든 빛을 흡수하는 특성을 가진 크리스탈이다. 밝은 방에 두면 어둠으로 가득 찬다.",
        "매우 강력한 빛을 발산하는 특성을 가진 크리스탈이다. 맨눈으로 보면 실명할 수 있다.",
        "매우 단단한 특성을 가진 크리스탈이다. 방어구 및 무기 제작에 많이 쓰인다.",
        "엄청난 한기를 머금은 크리스탈이다. 맨손으로 만지면 그대로 얼어버린다.",
        "엄청난 열기를 머금은 크리스탈이다. 용암보다 훨씬 뜨겁다.",
        "영혼석의 결정을 가진 크리스탈이다. 매우 희귀하여 비싸다.",

        "땅의 기운으로 가득찬 마법의 돌이다. 매우 무겁고 단단하다.",
        "공기의 기운으로 가득찬 마법의 돌이다. 깃털처럼 매우 가볍다.",
        "바다의 기운으로 가득찬 마법의 돌이다. 색이 깊고 시원하다.",
        "생명의 기운으로 가득찬 마법의 돌이다. 보기만 해도 힘이 솟는다.",
        "불의 기운으로 가득찬 마법의 돌이다. 매우 뜨겁고 강렬하다.",
        "얼음의 기운으로 가득찬 마법의 돌이다. 매우 차갑고 날카롭다.",

        "마력이 느껴지는 결정체이다. 최근에 만들어진 것으로 보인다.",
        "불순한 기운이 많이 정제된 결정체이다. 순수한 마력이 느껴진다.",
        "오랜 기간 마력을 흡수한 결정체이다. 불안전한 상태이므로 조심해야 한다.",
        "엄청난 마력이 느껴진다. 이정도 에너지를 담고 있는걸 보아 몇 백년은 지난 것 같다.",
        "고대 백룡의 피각이 결정체로 변질된 신비로운 돌이다. 맑고 순수한 힘이 느껴진다.",
        "고대 흑룡의 피각이 결정체로 변질된 신비로운 돌이다. 탁하고 불순한 힘이 느껴진다.",

        "별의 힘이 깃든 노란 보석이다. 별의 잔해물 중 하나이다.",
        "달의 힘이 깃든 주황 보석이다. 달의 힘이 느껴진다.",
        "태양의 힘이 깃든 붉은 보석이다. 엄청난 에너지가 느껴진다.",
        "우주의 힘이 깃든 어두운 보석이다. 은하수의 잔여물이다.",
        "강력한 원자력으로 인해 붕괴되어 생성된 보석이다. 오래 만지면 몸이 붕괴할 수 있다.",
        "모든 물질의 기원이 되는 결정체이다. 순수하고 맑은 기운이 몸을 정화시키는 기분이다.",

        "망령의 육체가 단단해져 만들어진 돌이다. 뭔가 꺼림칙한 느낌이 든다.",
        "오랜 시간 뜨거운 용암과 불이 함께 식어 만들어진 암석이다. 분명 식었는데 만질 수 없을 정도로 뜨겁다.",
        "뜨거운 열을 흡수해 빛을 내는 돌이다. 밝은 빛 때문에 제대로 바라 볼 수 없다.",
        "영혼들의 속죄에 사용되는 돌이다. 생각과 마음이 깨끗해지는 느낌이 든다.",
        "연옥이 섞인 용암이 굳은 돌로 엄청 단단하다. 연옥의 죄인들을 가두는 감옥의 재료로 쓰인다.",
        "가끔 영혼들이 암석과 융화하는 경우로 만들어진다. 의지를 가지고 있어 가끔 이계로 나가곤 한다.",
        "연옥으로 이루어진 광석이다. 연옥석은 연옥 그 자체이자 고귀한 상징을 가진다.",

        "육천의 첫째 하늘(사황천)을 구성하는 암석이다. 영혼을 가둬 보호하는 용도로 사용된다.",
        "육천의 둘째 하늘(도리천)을 구성하는 암석이다. 모든 생명의 근원이 되는 돌이다.",
        "육천의 셋째 하늘(야마천)을 구성하는 암석이다. 돌 주위의 시간이 다르게 흘러가는 것 처럼 보인다.",
        "육천의 넷째 하늘(도솔천)을 구성하는 암석이다. 묘하게 정신이 환락에 빠지는 기분이 든다.",
        "육천의 다섯째 하늘(화락천)을 구성하는 암석이다. 상대방의 생각과 감정을 조종할 수 있는 능력이 있다.",
        "육천의 여섯째 하늘(타화자재천)을 구성하는 암석이다. 소유한 자를 세상에서 가장 행복하게 해준다는 소문이 있다.",
        "모든 천석이 결정체로 녹아 만들어진 고귀한 결정이다. 이것을 얻으면 무릉도원을 만들 수 있다는 전설이 있다.",

        "순수한 영혼과 육체를 구성하면서 나온 부산물이다. 흔히 찌꺼기가 뭉쳐 만들어진 퇴적 광물이다. 잿빛을 띄고 있으며 불순한 기운이 느껴진다.",
        "육체에서 걸러진 활력이 뭉쳐 결정이 된 광물이다. 작지만 엄청난 에너지가 느껴진다.",
        "영혼에서 걸러진 안정과 안식이 뭉쳐 결정이 된 광물이다. 마치 작은 꽃이 피어나는 모양을 띄고 있다.",
        "천계 근처에서 보이는 순수하고 고귀한 결정체이다. 매우 정교하고 예민한 광물이라 심장 소리에도 으스러진다는 소문이 있다.",
        "순수한 육체의 근원이 모여 만들어진 결정체이다. 황홀한 붉은 빛을 띄고 있다.",
        "순수한 영혼의 근원이 모여 만들어진 결정체이다. 청량한 푸른 빛을 띄고 있다.",
        "순수한 육체와 영혼이 하나가 되어 천계를 지나갈 수 있도록 해주는 결정체이다.",

        "4가지 속성 중, 물 속성을 담당하는 물질로 이루어진 푸른 결정체이다.",
        "4가지 속성 중, 불 속성을 담당하는 물질로 이루어진 붉은 결정체이다.",
        "4가지 속성 중, 공기 속성을 담당하는 물질로 이루어진 잿빛 결정체이다.",
        "4가지 속성 중, 땅 속성을 담당하는 물질로 이루어진 구릿빛 결정체이다.",
        "현재 상태를 유지하기 위한 공간 속성을 담당하는 물질이다. 주황빛을 띄고 있다.",
        "현재 상태를 유지하기 위한 시간 속성을 담당하는 물질이다. 푸른빛을 띄고 있다.",
        "매우 불완전한 원초적 상태를 가진 물질이다. 마치 모든 것을 집어 삼키는 듯 보인다.",

        "현세계 마나가 결합된 금속 광물이다. 상층에서 마나가 이세계로 흘러들어온 걸로 보인다.",
        "이세계 마나가 결합된 금속 광물이다. 이세계 순수 광물 중 하나이다.",
        "따듯한 기운이 느껴지는 암석 광물이다. 이세계 광물 중 중급 광물에 속한다.",
        "강력한 아우라가 겉은 맴도는 암석 광물이다. 이세계 광물 중 고급 광물에 속한다.",
        "현세계와 이세계 마나가 섞인 금속 광물이다. 고위의 마법 재료로 사용된다고 한다.",
        "현세계와 이세계 마나가 완벽한 비율로 조화를 이루는 금속 광물이다. 초월 마법 재료로 사용된다고 한다.",
        "금속인지 액체인지 알 수 없을 정도로 형체가 다양한 광물이다. 이세계 최고 강도를 자랑한다.",
    };
    /// <summary> 컬렉션 효과 정보 </summary>
    private static string[] force_infos =
    {
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ",
        "모자 특수 효과 확률 < ", "검 특수 효과 확률 < ", "광물 추가 획득 확률 < ", "채광 속도 < ", "경험치 2배 추가 획득 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "추가 경험치 획득률 < ", "마나석 광석에서 얻는 마나석 < ",
        "채광 속도 < ", "모자 특수 효과 확률 < ", "검 특수 효과 확률 < ", "상점 특수 판매 효과 < ", "광물 추가 획득 확률 < ", "펫 드랍률 < ", "도서관 추가 주문서 획득 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "아이템 드랍률 < ", "상자에서 얻는 마나석 드랍률 < ",
        "곡괭이 내구도 < ", "채광 속도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "광물 컬렉션 획득 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "보스 몬스터 마나석 획득량 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "얼티밋 광물 등장 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "이벤트 맵 지속 시간 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "펫 합성 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "미스틱 광물 등장 확률 < ", "미스틱 광물 등장 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "미스틱 광물 등장 확률 < ", "미스틱 광물 등장 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "미스틱 광물 등장 확률 < ", "미스틱 광물 등장 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "성장하는 돌 등장 확률 < ", "성장하는 돌 등장 확률 < ",
        "곡괭이 내구도 < ", "모자 방어력 < ", "검 공격력 < ", "상점 추가 판매율 < ", "광물 < ", "성장하는 돌 등장 확률 < ", "성장하는 돌 등장 확률 < ",
    };
    /// <summary> 컬렉션 효과 수치 </summary>
    private static float[] forces =
    {
        0.2f, 0.2f, 0.2f, 0.6f,
        0.005f, 0.005f, 0.005f, 0.005f, 0.01f,
        0.4f, 0.3f, 0.3f, 0.8f, 1f, 0.02f, 1f,
        0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.02f, 0.1f,
        0.6f, 0.4f, 0.4f, 1.0f, 0.02f, 0.03f,
        0.8f, 0.02f, 0.8f, 0.8f, 1.2f, 2f, 0.03f,
        1.0f, 0.5f, 0.5f, 1.5f, 5f, 0.03f,
        1.2f, 0.6f, 0.6f, 1.8f, 10f, 0.03f,
        1.4f, 0.7f, 0.7f, 2.1f, 20f, 0.2f,
        1.6f, 0.8f, 0.8f, 2.4f, 40f, 0.0005f,
        1.8f, 0.9f, 0.9f, 2.7f, 80f, 0.03f, 0.05f,
        2.0f, 1.0f, 1.0f, 3.0f, 150f, 0.04f, 0.07f,
        2.2f, 1.1f, 1.1f, 3.3f, 250f, 0.05f, 0.09f,
        2.4f, 1.2f, 1.2f, 3.6f, 400f, 0.01f, 0.02f,
        2.6f, 1.3f, 1.3f, 3.9f, 700f, 0.01f, 0.02f,
    };
    /// <summary> 층 컬렉션 효과 정보 </summary>
    private static string[][] floor_infos =
    {
        new string[] { "상점 추가 판매율 < ", "광물 < ", "추가 경험치 획득량 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "경험치 2배 추가 획득 확률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "마나석 광석에서 얻는 마나석 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "아이템 및 펫 드랍률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "상자에서 얻는 마나석 획득량 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "광물 컬렉션 획득 확률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "보스 몬스터 마나석 획득량 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "얼티밋 광물 등장 확률 < ",  },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "이벤트 맵 지속 시간 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "펫 합성 확률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "미스틱 광물 등장 확률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "미스틱 광물 등장 확률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "미스틱 광물 등장 확률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "성장하는 돌 등장 확률 < ", },
        new string[] { "상점 추가 판매율 < ", "광물 < ", "성장하는 돌 등장 확률 < ", },
    };
    /// <summary> 층 컬렉션 효과 수치 </summary>
    public static float[][] floor_forces =
    {
        new float[] { 20f, 10f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 40f, 20f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f },
        new float[] { 60f, 30f, 5f, 10f, 15f, 30f, 50f },
        new float[] { 80f, 40f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 100f, 50f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 140f, 100f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 200f, 250f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 300f, 500f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 500f, 2000f, 3f, 5f, 7f, 10f, 20f },
        new float[] { 1000f, 5000f, 0.01f, 0.02f, 0.03f, 0.04f, 0.05f },
        new float[] { 2000f, 20000f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 5000f, 50000f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 10000f, 100000f, 0.2f, 0.3f, 0.5f, 1f, 2f },
        new float[] { 20000f, 200000f, 0.1f, 0.2f, 0.3f, 0.5f, 1f },
        new float[] { 40000f, 400000f, 0.1f, 0.2f, 0.3f, 0.5f, 1f },
    };
    private static Sprite[] sprites = Resources.LoadAll<Sprite>("Images/JemImages/Jem_cards");

    public int itemCode;
    public int floor, index;
    public string info, force_info;
    public float force;
    public Sprite sprite;

    public Collection(int _itemCode)
    {
        itemCode = _itemCode;
        info = infos[itemCode];
        force_info = force_infos[itemCode];
        force = forces[itemCode];
        sprite = sprites[itemCode];
    }

    public static string GetJemForceInfo(int jemCode)
    {
        string str = force_infos[jemCode];
        int level = SaveScript.saveData.collection_levels[jemCode];

        if (GameFuction.HasElement(new int[] { 13, 34, 40, 46, 52, 58, 64, 71, 78, 85, 92 }, jemCode))
        {
            if (SaveScript.saveData.equipPendant == -1) str += SaveScript.pendants[0].reinforce_basic * forces[jemCode] * level + "개 > 추가 획득";
            else str += SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * forces[jemCode] * level + "개 > 추가 획득";
        }
        else if (GameFuction.HasElement(new int[] { 53 }, jemCode))
            str += forces[jemCode] * level + "초 > 증가";
        else if (GameFuction.HasElement(new int[] { 15 }, jemCode))
            str += forces[jemCode] * level + "개 > 추가 획득";
        else
        {
            if (GameFuction.HasElement(new int[] { 0, 9, 23, 29, 36, 42, 48, 54, 60, 67, 74, 81, 88 }, jemCode)) // 곡괭이
                str += Mathf.RoundToInt(SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * forces[jemCode] * level * 100f) / 100f + " > 증가";
            else if (GameFuction.HasElement(new int[] { 1, 10, 24, 31, 37, 43, 49, 55, 61, 68, 75, 82, 89 }, jemCode)) // 모자
            {
                if (SaveScript.saveData.equipHat == -1) str += Mathf.RoundToInt(SaveScript.hats[0].reinforce_basic * forces[jemCode] * level * 100f) / 100f + " > 증가";
                else str += Mathf.RoundToInt(SaveScript.hats[SaveScript.saveData.equipHat].reinforce_basic * forces[jemCode] * level * 100f) / 100f + " > 증가";
            }
            else if (GameFuction.HasElement(new int[] { 3, 12, 26, 33, 39, 45, 51, 57, 63, 70, 77, 84, 91 }, jemCode)) // 반지
            {
                if (SaveScript.saveData.equipRing == -1) str += Mathf.RoundToInt(SaveScript.rings[0].reinforce_basic * forces[jemCode] * level * 100f * 100f) / 100f + "% > 증가";
                else str += Mathf.RoundToInt(SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * forces[jemCode] * level * 100f * 100f) / 100f + "% > 증가";
            }
            else if (GameFuction.HasElement(new int[] { 2, 11, 25, 32, 38, 44, 50, 56, 62, 69, 76, 83, 90 }, jemCode)) // 검
                str += Mathf.RoundToInt(SaveScript.swords[SaveScript.saveData.equipSword].reinforce_basic * forces[jemCode] * level * 100f) / 100f + " > 증가";
            else // 그 외
                str += Mathf.RoundToInt(forces[jemCode] * level * 100f * 100f) / 100f + "% > 증가";
        }

        // 특수 능력 확률 관련
        if (GameFuction.HasElement(new int[] { 4, 17 }, jemCode))
        {
            // 모자
            if (SaveScript.stat.hat02 >= 1f)
                str = "특수 회피력 < " + (Mathf.RoundToInt(GameFuction.GetReinforcePercent_Over2(1, forces[jemCode] * level) * 10000f) / 10000f * 100) + "% > 증가";
        }
        else if (GameFuction.HasElement(new int[] { 19 }, jemCode))
        {
            // 반지
            if (SaveScript.stat.ring02 >= 1f)
                str = "특수 판매력 < " + (Mathf.RoundToInt(GameFuction.GetReinforcePercent_Over2(2, forces[jemCode] * level) * 10000f) / 10000f * 100) + "% > 증가";
        }
        else if (GameFuction.HasElement(new int[] { 6, 20 }, jemCode))
        {
            // 목걸이
            if (SaveScript.stat.pendant02 >= 1f)
                str = "최소 획득 광물 < " + (Mathf.RoundToInt(GameFuction.GetReinforcePercent_Over2(3, forces[jemCode] * level) * 10000f) / 10000f) + "개 > 증가";
        }
        else if (GameFuction.HasElement(new int[] { 5, 18 }, jemCode))
        {
            // 검
            if (SaveScript.stat.sword02 >= 1f)
                str = "크리티컬 데미지 < " + (Mathf.RoundToInt(GameFuction.GetReinforcePercent_Over2(4, forces[jemCode] * level) * 10000f) / 10000f * 100) + "% > 증가";
        }

        return str;
    }

    public static string GetFloorForceInfo(int floor, int index)
    {
        string str = floor_infos[floor][Mathf.Clamp(index, 0, 2)];
        if (index == 0)
        {
            if (SaveScript.saveData.equipRing == -1) str += GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.rings[0].reinforce_basic * floor_forces[floor][index] * 100f)) + "% > 증가";
            else str += GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.rings[SaveScript.saveData.equipRing].reinforce_basic * floor_forces[floor][index] * 100f)) + "% > 증가";
        }
        else if (floor == 8 && index > 1)
            str += floor_forces[floor][index] + "초 > 증가";
        else if (index == 1)
        {
            if (SaveScript.saveData.equipPendant == -1) str += GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.pendants[0].reinforce_basic * floor_forces[floor][index])) + "개 > 추가 획득";
            else str += GameFuction.GetNumText(Mathf.RoundToInt(SaveScript.pendants[SaveScript.saveData.equipPendant].reinforce_basic * floor_forces[floor][index])) + "개 > 추가 획득";
        }
        else if (floor == 2 && index > 1)
            str += floor_forces[floor][index] + "개 > 추가 획득";
        else
            str += Mathf.RoundToInt((floor_forces[floor][index] * 100f) * 10f) / 10f + "% > 증가";

        return str;
    }
}
