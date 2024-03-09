using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveAllObject : MonoBehaviour
{
    public static SetActiveAllObject instance;
    public bool isDone;

    GameObject[] all;

    void Awake()
    {
        instance = this;
        all = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < all.Length; i++)
            all[i].SetActive(true);
        isDone = true;
    }
}
