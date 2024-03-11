using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

/// <summary>
/// 맵 관련 처리를 담당하는 컨트롤러 클래스이다.
/// </summary>
public class MapCtrl : MonoBehaviour
{
    /// <summary> MapCtrl 싱글톤 </summary>
    private static MapCtrl instance;
    public static MapCtrl Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }


    /// <summary> 모든 타일맵 렌더러를 탐색하기 위한 부모 오브젝트 </summary>
    [SerializeField] private GameObject tileMapTr;


    /// <summary> 렌더러를 포함한 모든 오브젝트를 저장한 리스트 </summary>
    [SerializeField] private List<SortRenderer> spritesList;


    private void Awake()
    {
        Instance = this;

        spritesList = new List<SortRenderer>();
        spritesList.AddRange(GetComponentsInChildren<SortRenderer>());
    }

    private void SetDepthAllofMapObjects()
    {
        // Y축 정렬
        spritesList.Sort(delegate (SortRenderer a, SortRenderer b)
        {
            if (a.transform.position.y <= b.transform.position.y)
                return 1;
            else
                return -1;
        });

        // 렌더러 우선순위 지정
        int _sortIndex = 0;
        float _currentY = spritesList[0].transform.position.y;

        foreach (var _render in spritesList)
        {
            if (!IsEqualFloat(_currentY, _render.transform.position.y))
                _sortIndex++;

            // *out* _sortIndex는 하나의 렌더링의 우선순위를 설정하고 나온 최종 값을 다시 받아오는 것이다.
            _render.SetSortingOrder(_sortIndex, out _sortIndex);
            _currentY = _render.transform.position.y;
        }
    }


    public SortRenderer FindRender(Transform _transform)
    {
        foreach (var render in spritesList)
        {
            if (render.transform == _transform)
                return render;
        }
        return null;
    }


    public void AddSortRenderer(GameObject _ob)
    {
        SortRenderer render = _ob.GetComponent<SortRenderer>();
        if (render == null)
        {
            Debug.LogWarning("SortRenderer is null");
            return;
        }

        spritesList.Add(render);
    }


    public void RemoveSprite(Transform _transform)
    {
        if (!spritesList.Remove(FindRender(_transform)))
            Debug.LogError($"There is no '{_transform}' Object.");
    }

    public void DestroyObject(GameObject ob)
    {
        RemoveSprite(ob.transform);
        Destroy(ob);
    }

    public bool IsEqualFloat(float a, float b) => Mathf.Abs(a - b) <= 0.01f;
}
