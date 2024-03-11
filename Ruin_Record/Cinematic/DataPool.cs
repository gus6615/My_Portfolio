using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct CutSceneAction
{
    [Header("다음 연출로 넘어가는 시간")]
    public float playTime;

    [Header("플레이어 대사 관련")]
    public bool isDialogOn;
    public DialogSet dialogs;

    [Header("카메라 이동 관련")]
    public bool isCameraMoveOn;
    public Vector2 camera_destination;
    public float camera_moveSpeed;
    public bool camera_isMoveSmooth;

    [Header("카메라 줌 관련")]
    public bool isCameraZoomOn;
    public float camera_zoomSize;
    public float camera_zoomSpeed;
    public bool camera_isZoomSmooth;
}