using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class D_1_Torch : MonoBehaviour
{
    static private float fadeTime = 0.5f;

    [SerializeField] private new AudioSource audio;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D detectCol;
    [SerializeField] private Light2D light2D;
    public int type;
    public bool isGetTorch;
    private bool isLeft;

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        audio.mute = !SaveScript.saveData.isSEOn;

        isGetTorch = false;
        isLeft = false;
        detectCol.enabled = true;
        light2D.enabled = true;
        sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[5 + 7 * type].sprite;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        StartCoroutine("FadeTorch");
    }

    public void GetTorch()
    {
        StopCoroutine("FadeTorch");
        QuestCtrl.instance.SetMainQuestAmount(new int[] { 56 });
        PlayerScript.instance.SetButtonMode(false, true);
        sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[4 + 7 * type].sprite;
        detectCol.enabled = false;
        light2D.enabled = false;
        isGetTorch = true;
        audio.clip = SaveScript.SEs[42];
        audio.Play();
    }

    IEnumerator FadeTorch()
    {
        yield return new WaitForSeconds(fadeTime);

        if (isLeft)
            sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[6 + 7 * type].sprite;
        else
            sprite.sprite = MapData.instance.dungeon_1_DecoX64Tiles[5 + 7 * type].sprite;
        isLeft = !isLeft;
        StartCoroutine("FadeTorch");
    }
}
