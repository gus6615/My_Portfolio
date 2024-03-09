using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    static public CameraCtrl instance;

    public float cameraWidth, cameraHeight; // 카메라의 넓이의 반, 높이의 반
    private float moveSpeed; // 카메라가 플레이어를 쫒아가는 속도

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        cameraWidth = Camera.main.orthographicSize * Camera.main.pixelWidth / Camera.main.pixelHeight;
        cameraHeight = Camera.main.orthographicSize;
        moveSpeed = 2f;

        if (BlindScript.instance.spawnType == 0)
            this.transform.position = new Vector3(0, 0, -10f);
        else if (BlindScript.instance.spawnType == 1)
            this.transform.position = new Vector3(0f, MapData.depth[6] + 15f, -10f);
        else if (BlindScript.instance.spawnType == 2)
            this.transform.position = new Vector3(0f, MapData.depth[11] + 15f, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        GoToPlayer();
    }

    private void GoToPlayer()
    {
        Vector2 gap = PlayerScript.instance.transform.position - transform.position;

        if(PlayerScript.instance.isDungeon_0_On || PlayerScript.instance.isDungeon_1_On || PlayerScript.instance.isEventMap_On)
            transform.position += (Vector3)gap * moveSpeed * 4f * Time.deltaTime;
        else
        {
            transform.position += (Vector3)gap * moveSpeed * Time.deltaTime;
            if (transform.position.y > 0f)
                transform.position = new Vector3(transform.position.x, 0f, -10f);
        }
    }
}
