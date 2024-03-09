using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_0_Torch : MonoBehaviour
{
    static private float fadeTime = 0.5f;

    [SerializeField] private SpriteRenderer sprite;
    private bool isLeft;

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        isLeft = true;
        sprite.sprite = MapData.instance.dungeon_0_DecoX32Tiles[0].sprite;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        StartCoroutine("FadeTorch");
    }

    IEnumerator FadeTorch()
    {
        yield return new WaitForSeconds(fadeTime);

        if (isLeft)
            sprite.sprite = MapData.instance.dungeon_0_DecoX32Tiles[1].sprite;
        else
            sprite.sprite = MapData.instance.dungeon_0_DecoX32Tiles[0].sprite;
        isLeft = !isLeft;
        StartCoroutine("FadeTorch");
    }
}
