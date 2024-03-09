using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private const float CREATE_DISTANCE = 15f;

    private float moveSpeed;
    private float scale;
    private float height;

    private void OnEnable()
    {
        moveSpeed = Random.Range(0.1f, 0.2f);
        scale = Random.Range(1f, 1.25f);
        height = Random.Range(1.75f, 3.5f);

        this.transform.position = new Vector3(-CREATE_DISTANCE, height, 0);
        this.transform.localScale = Vector3.one * scale;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += Time.deltaTime * Vector3.right * moveSpeed;
        if (this.transform.position.x > CREATE_DISTANCE)
            Destroy(this.gameObject);
    }

    public void SetPosition(Vector3 _pos)
    {
        this.transform.position = _pos;
    }
}
