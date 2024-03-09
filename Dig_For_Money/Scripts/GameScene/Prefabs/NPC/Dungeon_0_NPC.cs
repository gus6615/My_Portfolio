using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_0_NPC : MonoBehaviour
{
    public Animator animator;
    private float actionTime;
    private bool isAction;
    public int type;

    private void OnEnable()
    {
        animator.Play("Idle", -1, 0f);
        animator.SetInteger("ActionType", 0);
        StartCoroutine("Init");
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetInteger("ActionType") == 2)
            transform.position += Vector3.left * Time.deltaTime;
        else
        {
            if (!isAction)
                StartAction();
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        isAction = false;
        actionTime = Random.Range(3f, 7f);
    }

    public void SetIdle()
    {
        if (animator.GetInteger("ActionType") != 2)
        {
            actionTime = Random.Range(3f, 7f);
            animator.SetInteger("ActionType", 0);
            StartCoroutine("SetAction");
        }
    }

    public void StartAction()
    {
        if (animator.GetInteger("ActionType") != 2)
        {
            isAction = true;
            animator.SetInteger("ActionType", 1);
        }
    }

    public void Delete()
    {
        ObjectPool.ReturnObject<Dungeon_0_NPC>(5, this);
    }

    IEnumerator SetAction()
    {
        yield return new WaitForSeconds(actionTime);
        isAction = false;
    }
}
