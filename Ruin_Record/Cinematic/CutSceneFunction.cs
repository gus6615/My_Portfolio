using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutSceneFunction : MonoBehaviour
{
    protected bool isOn;

    public abstract void OnFuntionEnter();

    public abstract void Play(int actionIdx);

    public abstract void OnFuntionUpdate();

    public abstract void OnFunctionExit();
}
