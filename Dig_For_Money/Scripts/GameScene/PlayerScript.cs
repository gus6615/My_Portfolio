using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    static public PlayerScript instance;
    static public bool isIconTimeSet;

    public Light2D light2D;
    public Animator lightAnimator;
    public Animator animator, handsAnimator;
    public new Rigidbody2D rigidbody;
    public SpriteRenderer[] sprites;
    public new AudioSource audio;
    public Text damageText, infoText;
    private RectTransform damageRT, infoRT;

    public bool isEnd, isCanCtrl;
    public long[] jems;
    public long growthOre, exp, reinforceOre, manaOre;
    public Vector2Int playerPos;
    public long pickHP, pickFullHP;
    public float plusPercentHP, bufPlusPercentHP;
    public bool isJump;
    public float moveData; // 왼쪽 오른쪽 방향 움직임 데이터
    public float moveSpeed, moveSpeedData;
    public float jumpPower, jumpPowerData;
    public float bufMoveSpeed, bufJumpPower;

    public bool isAttacked;
    public bool isNoDamage;
    private int attackedCount, currentAttackedCount;
    private bool isFade;
    public bool isCheckMonster;
    public bool isAttack;
    private bool isCritical;
    private float critical_force;

    // 던전 관련
    public bool isEventOn; // 플레이어가 이벤트에 진입했는가
    public bool isDungeon_0_On;
    public bool isDungeon_1_On;
    public bool isEventMap_On;
    public Vector3Int eventMap_startPos; // 현재 플레이어가 들어간 이벤트 포탈 위치
    public int d_1_currentFloor; // 현재 플레이어가 들어간 고대 던전의 층
    public D_1_Torch d_1_currentTorch; // 현재 플레이어와 닿은 횃불
    IEnumerator fadeLight_enumerator;

    // 임시 데이터
    new GameObject gameObject;
    EventBlock eventBlock;
    Monster monster;
    Monster_Boss monster_boss;
    ReinforceTree tree;
    BoxObject box;
    BreakObject breakObject;
    Golem golem;
    DamageText damage_text;
    DamageSlide damage_slide;
    Collider2D[] cols;

    MonsterAttack monsterAttack;
    ADSlimeAttack ADSlimeAttack;
    KingSlime_Attack kingSlime_Attack;
    D_1_Midboss_Skill0 D_1_Midboss_Skill0;
    D_1_Midboss_Skill1 D_1_Midboss_Skill1;

    public void ExceptBufStat(int type)
    {
        switch (type)
        {
            case 0:
                plusPercentHP -= bufPlusPercentHP;
                break;
            case 1:
                moveSpeed -= bufMoveSpeed;
                moveSpeedData -= bufMoveSpeed;
                break;
            case 2:
                jumpPower -= bufJumpPower;
                jumpPowerData -= bufJumpPower;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        damageRT = damageText.GetComponent<RectTransform>();
        infoRT = infoText.GetComponent<RectTransform>();

        sprites[0].color = SaveScript.toolColors[SaveScript.saveData.equipPick];
        if (SaveScript.saveData.equipHat == -1) sprites[1].gameObject.SetActive(false);
        else sprites[1].color = SaveScript.toolColors[SaveScript.saveData.equipHat];
        if (SaveScript.saveData.equipRing == -1) sprites[2].gameObject.SetActive(false);
        else sprites[2].color = SaveScript.toolColors[SaveScript.saveData.equipRing];
        if (SaveScript.saveData.equipPendant == -1) sprites[3].gameObject.SetActive(false);
        else sprites[3].color = SaveScript.toolColors[SaveScript.saveData.equipPendant];
        sprites[4].color = sprites[5].color = SaveScript.toolColors[SaveScript.saveData.equipSword];

        jems = new long[SaveScript.totalItemNum];
        playerPos = Vector2Int.zero;
        moveSpeedData = 4.0f;
        moveSpeed = moveSpeedData;
        jumpPowerData = 8.1f;
        jumpPower = jumpPowerData;
        attackedCount = 4;
        isCanCtrl = true;
        if (BlindScript.instance.spawnType == 0)
            this.transform.position = Vector3.zero;
        else if (BlindScript.instance.spawnType == 1)
            this.transform.position = new Vector3(0f, MapData.depth[6] + 15f, 0f);
        else if (BlindScript.instance.spawnType == 2)
            this.transform.position = new Vector3(0f, MapData.depth[11] + 15f, 0f);

        plusPercentHP = SaveScript.stat.pick01;

        // 아이템 및 강화 버프 적용
        bufMoveSpeed = GameFuction.GetBufItemPercent(8) + GameFuction.GetElixirPercent(8);
        moveSpeed += bufMoveSpeed;
        moveSpeedData += bufMoveSpeed;
        bufJumpPower = GameFuction.GetBufItemPercent(9) + GameFuction.GetElixirPercent(9);
        jumpPower += bufJumpPower;
        jumpPowerData += bufJumpPower;
        bufPlusPercentHP = GameFuction.GetBufItemPercent(0) + GameFuction.GetElixirPercent(0);

        pickFullHP = (long)(SaveScript.picks[SaveScript.saveData.equipPick].durability + SaveScript.picks[SaveScript.saveData.equipPick].reinforce_basic * plusPercentHP);
        pickHP = pickFullHP;
        damageText.color = new Color(1f, 1f, 1f, 0f);
        infoText.color = new Color(1f, 1f, 1f, 0f);
        audio.mute = !SaveScript.saveData.isSEOn;

        SetLight(0);
        SetButtonMode(false, true, true, true);
    }

    // Update is called once per frame
    private void Update()
    {
        SetAnimator();
        if (damageText.color.a != 0f) 
            damageRT.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + new Vector3(-Mathf.Sign(this.transform.localScale.x) * 0.1f, 0.75f, 0f));
        if (infoText.color.a != 0f) 
            infoRT.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + new Vector3(-Mathf.Sign(this.transform.localScale.x) * 0.1f, -0.75f, 0f));

        if (!isEnd)
        {
            if (pickHP <= 0f)
            {
                // 현재 튜토리얼인 경우엔 플레이어가 죽지 않는다.
                if (SaveScript.saveData.isTutorial)
                    pickHP = 1;
                else
                {
                    isEnd = true;
                    moveData = 0f;
                    MoveCtrl.isMoveStart = false;
                    pickHP = 0;
                }
                PickStateUI.instance.ShowPickState();
            }
            if (isCanCtrl) keyBoardDetect();
            if (isAttacked) Attacked();
        }
    }

    private void FixedUpdate()
    {
        if (!isEnd)
        {
            if (isCanCtrl) Ctrl();
            if (rigidbody.velocity.y < -7.5f)
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, -7.5f);
        }
    }

    public void keyBoardDetect()
    {
        if (!MoveCtrl.isMoveStart) moveData = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.W) && JumpCtrl.jumpCount < 2) isJump = true;
    }

    public void Ctrl()
    {
        RaycastHit2D hitDown;
        if (!MoveCtrl.isMoveStart)
        {
            hitDown = Physics2D.BoxCast(this.transform.position, new Vector2(0.5f, 0.5f), 0f, Vector2.right * Mathf.Sign(this.transform.localScale.x), 0.1f, 256);
            if(!hitDown)
                rigidbody.position += Vector2.right * moveData * moveSpeed * Time.deltaTime;

            if (moveData != 0f)
            {
                transform.localScale = new Vector3(Mathf.Sign(moveData) * 1f, 1f, 1f);
                PlayAniState("isMove", true);
            }
            else
                PlayAniState("isMove", false);
        }

        // 점프
        if (isJump)
        {
            JumpCtrl.jumpCount++;
            JumpCtrl.isJumpStart = false;
            rigidbody.velocity = new Vector2(0f, 0.01f);
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJump = false;
        }

        // 점프 초기화 확인
        hitDown = Physics2D.BoxCast(this.transform.position + new Vector3(-0.0652f * Mathf.Sign(this.transform.localScale.x), -0.5f, 0f), new Vector2(0.475f, 0.1f), 0f, Vector2.down, 0.1f, 256);

        if (hitDown && rigidbody.velocity.y == 0f) JumpCtrl.jumpCount = 0;

        // 공격 버튼 눌린 시점
        if (AttackCtrl.isAttackStart)
        {
            AttackCtrl.isAttackStart = false;
            float criticalPercent = SaveScript.stat.sword02;
            isAttack = true;
            isCritical = false;
            critical_force = 1f;
            moveSpeed -= 1.75f;
            jumpPower -= 1.75f;

            if (GameFuction.GetRandFlag(criticalPercent))
            {
                isCritical = true;
                if (criticalPercent >= 1f) critical_force = GameFuction.GetReinforcePercent_Over2(4);
                else critical_force = 2f;
            }

            if (Random.Range(0, 2) == 0) PlayAniState("isCriticalAttack", true);
            else PlayAniState("isAttack", true);
        }
    }

    public void Attack()
    {
        audio.clip = SaveScript.SEs[19];
        gameObject = CheckMonster();
        if (gameObject == null)
        {
            audio.Play();
            return;
        }
        monster = gameObject.GetComponent<Monster>();
        box = gameObject.GetComponent<BoxObject>();
        breakObject = gameObject.GetComponent<BreakObject>();
        monster_boss = gameObject.GetComponent<Monster_Boss>();
        tree = gameObject.GetComponent<ReinforceTree>();

        float damagePlus = SaveScript.stat.sword01;
        long damage = (long)((SaveScript.swords[SaveScript.saveData.equipSword].forcePercent 
            + SaveScript.swords[SaveScript.saveData.equipSword].reinforce_basic * damagePlus) * critical_force * Random.Range(0.9f, 1.1f));
        if (isCritical)
            audio.clip = SaveScript.SEs[21];

        // 몬스터일 경우
        if (monster != null)
        {
            damage_text = ObjectPool.GetObject<DamageText>(13, ObjectPool.instance.objectUI, Camera.main.WorldToScreenPoint(monster.transform.position + Vector3.up * 1f));
            damage_slide = monster.HPSlider;
            if (damage_slide == null)
            {
                damage_slide = ObjectPool.GetObject<DamageSlide>(14, ObjectPool.instance.objectUI, Camera.main.WorldToScreenPoint(monster.transform.position + Vector3.up * monster.height));
                damage_slide.target = monster.gameObject;
                damage_slide.height = monster.height;
                monster.HPSlider = damage_slide;
            }

            if (monster.kind == 4 && monster.GetComponent<MushroomSlime>().isDefending) 
                damage = (long)(damage * 0.5f);
            damage_text.target = monster.gameObject;
            monster.HP -= damage;
            damage_text.text.text = "-" + GameFuction.GetNumText(damage);

            if (isCritical)
            {
                audio.clip = SaveScript.SEs[22];
                damage_text.text.color = DamageText.redColor;
                if (monster.rigidbody.mass < 2f)
                    monster.rigidbody.AddForce(Vector2.right * 1.5f * Mathf.Sign(this.transform.localScale.x), ForceMode2D.Impulse);
            }
            else
            {
                audio.clip = SaveScript.SEs[20];
                damage_text.text.color = DamageText.blueColor;
            }

            if (monster is Golem)
                audio.clip = SaveScript.SEs[28];
            damage_slide.slider.maxValue = monster.maxHP;
            damage_slide.slider.value = monster.HP;
            if (monster.HP <= 0f && !monster.isDead) monster.Dead();
        }
        // 보스 몬스터일 경우
        if(monster_boss != null)
        {
            damage_text = ObjectPool.GetObject<DamageText>(13, ObjectPool.instance.objectUI, Camera.main.WorldToScreenPoint(monster_boss.transform.position + Vector3.up * 1f));
            damage_text.target = monster_boss.gameObject;
            monster_boss.HP -= damage;
            damage_text.text.text = "-" + GameFuction.GetNumText(damage);

            if (isCritical)
            {
                audio.clip = SaveScript.SEs[22];
                damage_text.text.color = DamageText.redColor;
            }
            else
            {
                audio.clip = SaveScript.SEs[20];
                damage_text.text.color = DamageText.blueColor;
            }

            if (monster_boss.kind == 1)
                audio.clip = SaveScript.SEs[28];

            if (monster_boss.HP <= 0f && !monster_boss.isDead) 
                monster_boss.Dead();

            if (BossHPSlider.currentBoss != monster_boss)
                BossHPSlider.instance.SetHPSlider(monster_boss);
            else
                BossHPSlider.instance.SetHP(monster_boss);
        }
        // 강화석 나무일 경우
        if (tree != null)
        {
            damage_text = ObjectPool.GetObject<DamageText>(13, ObjectPool.instance.objectUI, Camera.main.WorldToScreenPoint(tree.transform.position + Vector3.up * 1f));
            damage_slide = tree.HPSlider;
            tree.animator.SetBool("isAttacked", true);
            if (damage_slide == null)
            {
                damage_slide = ObjectPool.GetObject<DamageSlide>(14, ObjectPool.instance.objectUI, Camera.main.WorldToScreenPoint(tree.transform.position + Vector3.up * tree.height));
                damage_slide.target = tree.gameObject;
                damage_slide.height = tree.height;
                tree.HPSlider = damage_slide;
            }

            damage_text.target = tree.gameObject;
            tree.HP -= damage;
            damage_text.text.text = "-" + GameFuction.GetNumText(damage);

            if (isCritical)
                damage_text.text.color = DamageText.redColor;
            else
                damage_text.text.color = DamageText.blueColor;

            audio.clip = SaveScript.SEs[28];
            damage_slide.slider.maxValue = tree.maxHP;
            damage_slide.slider.value = tree.HP;

            // 강화석 골렘 깨우기
            cols = Physics2D.OverlapCircleAll(this.transform.position, 10f, 4096);
            for (int i = 0; i < cols.Length; i++)
                cols[i].GetComponentInParent<Golem>().HP--;
            if (tree.HP <= 0f && !tree.isDead) tree.Dead();
        }
        // 상자일 경우
        if (box != null) { box.BoxOpen(); }
        // 부수는 물체일 경우
        if (breakObject != null) { breakObject.DamageToObject(damage); }

        audio.Play();
    }

    public void SetInfoText(string info, float startTime, float fadeTime)
    {
        infoText.color = new Color(1f, 0.3f, 0.3f, 1f); // 빨간색
        infoText.text = info;
        infoText.GetComponent<FadeUI>().SetFadeValues(0f, startTime, fadeTime);
        infoRT.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + Vector3.up * 0.75f);
    }

    public void EndAttack()
    {
        PlayAniState("isAttack", false);
        PlayAniState("isCriticalAttack", false);
        isAttack = false;
        moveSpeed = moveSpeedData;
        jumpPower = jumpPowerData; 
    }

    public void SetAnimator()
    {
        if (rigidbody.velocity.y > 0.02f) // 위로 점프 상태
        {
            PlayAniState("isJumpDown", false);
            PlayAniState("isJump", true);
        }
        else if (rigidbody.velocity.y < -0.02f) // 아래로 점프 상태
        {
            PlayAniState("isJumpDown", true);
            PlayAniState("isJump", false);
        }
        else if(rigidbody.velocity.y == 0f) // 정지 상태
        {
            PlayAniState("isJumpDown", false);
            PlayAniState("isJump", false);
        }
    }

    public void Attacked()
    {
        if (!isFade)
        {
            if (sprites[0].color.a > 0.4f)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    Color color = sprites[i].color;
                    color.a -= Time.deltaTime * 3f;
                    sprites[i].color = color;
                }
            }
            else
            {
                isFade = true;
                for (int i = 0; i < sprites.Length; i++)
                {
                    Color color = sprites[i].color;
                    color.a = 0.4f;
                    sprites[i].color = color;
                }
            }
        }
        else
        {
            if (sprites[0].color.a < 1f)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    Color color = sprites[i].color;
                    color.a += Time.deltaTime * 3f;
                    sprites[i].color = color;
                }
            }
            else
            {
                isFade = false;
                currentAttackedCount++;
                for (int i = 0; i < sprites.Length; i++)
                {
                    Color color = sprites[i].color;
                    color.a = 1f;
                    sprites[i].color = color;
                }
            }
        }

        if(currentAttackedCount == attackedCount)
        {
            isAttacked = false;
            currentAttackedCount = 0;
        }
    }

    public GameObject CheckMonster()
    {
        // 몬스터&상자, 강화석 나무
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0f, Vector2.right * Mathf.Sign(this.transform.localScale.x), 1f, 7168);

        if (hit)
        {
            isCheckMonster = true;
            return hit.transform.gameObject;
        }
        else
        {
            isCheckMonster = false;
            return null;
        }
    }

    public void SetButtonMode(bool _eventOn, bool _interactionOn, bool _autoOn, bool _attackOn)
    {
        EventButton.instance.SetButtonEnable(_eventOn);
        Interaction.instance.SetButtonEnable(_interactionOn);
        if (!SaveScript.saveData.isTutorial)
            AutoPlayCtrl.instance.SetButtonEnable(_autoOn);
        AttackCtrl.instance.SetButtonEnable(_attackOn);
    }

    public void SetButtonMode(bool _eventOn, bool _interactionOn)
    {
        EventButton.instance.SetButtonEnable(_eventOn);
        Interaction.instance.SetButtonEnable(_interactionOn);
    }

    public void PlayAniState(string _state, bool _isBool)
    {
        animator.SetBool(_state, _isBool);
        handsAnimator.SetBool(_state, _isBool);
    }

    public void SetLight(int _state)
    {
        void SetLightParam(float inner, float outer, Color color)
        {
            light2D.pointLightInnerRadius = inner;
            light2D.pointLightOuterRadius = outer;
            light2D.color = color;
        }

        lightAnimator.SetInteger("state", _state);
        switch (_state)
        {
            // 주의! 해당 Param이 변경되면 TorchUICtrl의 SetLightModel도 변경되어야 함
            case 0: SetLightParam(2, 26, Color.white); break;
            case 1: SetLightParam(1, 8, new Color(1f, 0.9f, 0.7f)); break;
            case 2: SetLightParam(1, 7, new Color(1f, 0.9f, 0.7f)); break;
        }
    }

    public void SetFadeLight(float inner, float outer)
    {
        if (fadeLight_enumerator != null)
            StopCoroutine(fadeLight_enumerator);
        fadeLight_enumerator = FadeLight(inner, outer);
        StartCoroutine(fadeLight_enumerator);
    }

    IEnumerator FadeLight(float inner, float outer)
    {
        float current_inner = light2D.pointLightInnerRadius;
        float current_outer = light2D.pointLightOuterRadius;
        float inner_gap = inner - current_inner;
        float outer_gap = outer - current_outer;
        float fadeSpeed = 1f;

        while (Mathf.Abs(inner_gap) > 0.05f)
        {
            if (!isDungeon_1_On)
                break;

            light2D.pointLightInnerRadius += inner_gap * Time.deltaTime * fadeSpeed;
            light2D.pointLightOuterRadius += outer_gap * Time.deltaTime * fadeSpeed;
            current_inner = light2D.pointLightInnerRadius;
            current_outer = light2D.pointLightOuterRadius;
            inner_gap = inner - current_inner;
            outer_gap = outer - current_outer;
            yield return null;
        }

        if (isDungeon_1_On)
        {
            light2D.pointLightInnerRadius = inner;
            light2D.pointLightOuterRadius = outer;
        }
    }

    IEnumerator eventBlock_exit()
    {
        while (!BlindScript.isEndSwitchPos)
            yield return null;

        EventButton.instance.ResetEvent();
        SetButtonMode(false, true);
        eventBlock = null;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!isEventOn)
            return;

        if (col.tag == "EventBlock")
        {
            if (eventBlock != null)
            {
                StartCoroutine(eventBlock_exit());
            }
        }
        else if (col.tag == "D_1_Torch")
        {
            if (d_1_currentTorch != null && !d_1_currentTorch.isGetTorch)
            {
                EventButton.instance.ResetEvent();
                SetButtonMode(false, true);
                d_1_currentTorch = null;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        bool isMonster = false;
        bool isSkillDamage = false;
        float damage = 0f;

        if (col.tag == "Monster_Body")
        {
            if (isNoDamage) return; // 무적 판정
            monster = col.GetComponentInParent<Monster>();
            monster_boss = col.GetComponentInParent<Monster_Boss>();
            isMonster = true;
            if (monster != null)
            {
                damage = monster.damage;
                golem = monster as Golem;
                if (golem != null && !golem.isMad) return;
                if (monster.isDead) return;
            }
            else if (monster_boss != null)
            {
                damage = monster_boss.damage;
            }
        }
        else if (col.tag == "Monster_Attack")
        {
            if (isNoDamage) return; // 무적 판정
            isMonster = true;
            monsterAttack = col.GetComponentInParent<MonsterAttack>();
            ADSlimeAttack = monsterAttack as ADSlimeAttack;
            kingSlime_Attack = monsterAttack as KingSlime_Attack;
            D_1_Midboss_Skill0 = monsterAttack as D_1_Midboss_Skill0;
            D_1_Midboss_Skill1 = monsterAttack as D_1_Midboss_Skill1;
            if (ADSlimeAttack != null)
            {
                ObjectPool.ReturnObject<ADSlimeAttack>(8, ADSlimeAttack);
                damage = ADSlimeAttack.damage;
                isSkillDamage = true;
            }
            if (kingSlime_Attack != null)
            {
                damage = kingSlime_Attack.damage;
                Destroy(kingSlime_Attack.attack_col);
                isSkillDamage = true;
            }
            if (D_1_Midboss_Skill0 != null)
            {
                if (D_1_Midboss_Skill0.isSkillDamage)
                {
                    damage = (long)(pickFullHP * D_1_Midboss_Skill0.skillDamage);
                    isSkillDamage = true;
                    D_1_Midboss_Skill0.isSkillDamage = false;
                    AddForce(new Vector2(Mathf.Sign(this.transform.position.x - D_1_Midboss_Skill0.transform.position.x) * 3f, 2.5f));
                }
                else
                    damage = D_1_Midboss_Skill0.damage;
                if (damage < D_1_Midboss_Skill0.damage)
                    damage = D_1_Midboss_Skill0.damage;
            }
            if (D_1_Midboss_Skill1 != null)
            {
                if (D_1_Midboss_Skill1.isSkillDamage)
                {
                    damage = (long)(pickFullHP * D_1_Midboss_Skill1.skillDamage);
                    isSkillDamage = true;
                    D_1_Midboss_Skill1.isSkillDamage = false;
                    SetFaint(2f);
                    AddForce(new Vector2(Mathf.Sign(this.transform.position.x - D_1_Midboss_Skill1.transform.position.x) * 3f, 2.5f));
                }
                else
                    damage = D_1_Midboss_Skill1.damage;
                if (damage < D_1_Midboss_Skill1.damage)
                    damage = D_1_Midboss_Skill1.damage;
            }
        }
        else if (col.tag == "NormalBox")
        {
            // 튜토리얼 (상자 열기)
            if (SaveScript.saveData.isTutorial)
            {
                if (Tutorial.instance.tutorialIndex != 14) return;
                else Tutorial.instance.value++;
            }

            box = col.GetComponentInParent<NormalBox>();
            box.BoxOpen();
        }
        else if (!isEventOn && isDungeon_1_On && col.tag == "D_1_Torch")
        {
            d_1_currentTorch = col.GetComponentInParent<D_1_Torch>();
            if (!d_1_currentTorch.isGetTorch)
            {
                isEventOn = true;
                SetButtonMode(true, false);
                EventButton.mainType = 14;
            }
        }
        else if (!isEventOn && col.tag == "EventBlock")
        {
            if (isDungeon_1_On && !TorchUICtrl.instance.isOn)
                return; // 버그 발생 방지용

            eventBlock = col.GetComponent<EventBlock>();
            if (eventBlock.CheckIsOre())
                return;

            isEventOn = true;
            SetButtonMode(true, false);
            EventButton.mainType = eventBlock.eventMainType;
            EventButton.subType = eventBlock.eventSubType;
            EventButton.roomData = eventBlock.roomData;
            EventButton.portal_vec = eventBlock.portal_vec;
            EventBlock.currentEventBlock = eventBlock;

            switch (EventButton.mainType)
            {
                case 0:
                    if (col.transform.position.x >= 0f)
                        DungeonCreater.dungeon_0_startPos = new Vector3Int((int)EventButton.portal_vec.x, (int)EventButton.portal_vec.y, (int)EventButton.portal_vec.z);
                    else
                        DungeonCreater.dungeon_0_startPos = new Vector3Int((int)EventButton.portal_vec.x, (int)EventButton.portal_vec.y, (int)EventButton.portal_vec.z) + Vector3Int.left;
                    DungeonCreater.dungeon_0_entranceEvent = col.gameObject;
                    DungeonCreater.dungeon_0_type = col.GetComponent<EventBlock>().eventSubType;
                    break;
                case 2:
                case 3:
                    DungeonCreater.dungeon_0_portalPos = new Vector3Int(Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), Mathf.RoundToInt(col.transform.position.z));
                    break;
                case 7:
                    eventMap_startPos = new Vector3Int(Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), Mathf.RoundToInt(col.transform.position.z));
                    break;
                case 9:
                    if (col.transform.position.x >= 0f)
                        DungeonCreater.dungeon_1_startPos = new Vector3Int((int)EventButton.portal_vec.x, (int)EventButton.portal_vec.y, (int)EventButton.portal_vec.z);
                    else
                        DungeonCreater.dungeon_1_startPos = new Vector3Int((int)EventButton.portal_vec.x, (int)EventButton.portal_vec.y, (int)EventButton.portal_vec.z) + Vector3Int.left;
                    DungeonCreater.dungeon_1_entranceEvent = col.gameObject;
                    DungeonCreater.dungeon_1_type = col.GetComponent<EventBlock>().eventSubType;
                    break;
            }
        }
        else return;

        if (isMonster)
        {
            if (isSkillDamage) DamageToPlayer((long)damage, true);
            else DamageToPlayer((long)damage, false);
        }
    }

    public void DamageToPlayer(long damage, bool isSkill)
    {
        if (isNoDamage) return; // 무적 판정
        if (isAttacked && !isSkill) return;

        bool isAvoid = false;
        bool isDiscount = false;
        bool isSpecial = false;
        float hatDiscount = SaveScript.stat.hat01;
        float plusPercent = SaveScript.stat.hat02;
        float multifly = 1f;
        long realDamage; // 실제 데미지
        int hatIndex = SaveScript.saveData.equipHat;
        isAttacked = true;

        // 모자 효과 적용
        if (hatIndex != -1)
        {
            isDiscount = true;
            if (GameFuction.GetRandFlag(plusPercent))
            {
                isSpecial = true;
                if (plusPercent >= 1f) multifly = GameFuction.GetReinforcePercent_Over2(1);
                else multifly = 2f;
            }
        }

        if (isDiscount) realDamage = (long)(damage - Mathf.Round((SaveScript.hats[hatIndex].forcePercent + SaveScript.hats[hatIndex].reinforce_basic * hatDiscount) * multifly));
        else realDamage = damage;

        if (realDamage <= 0f) isAvoid = true;

        if (isAvoid)
        {
            damageText.color = Color.white; // 흰색
            damageText.text = "Miss";
            damageText.GetComponent<FadeUI>().SetFadeValues(0f, 0f, 0.75f);
        }
        else
        {
            if (isSpecial) damageText.color = new Color(0f, 0.5f, 1f, 1f); // 파란색
            else damageText.color = new Color(1f, 0f, 0.5f, 1f); // 빨간색
            if (isSkill) damageText.color = new Color(1f, 0.5f, 0f, 1f); // 주황색

            pickHP -= Mathf.RoundToInt(realDamage);
            PickStateUI.instance.ShowPickState();
            damageText.text = "-" + GameFuction.GetNumText(Mathf.RoundToInt(realDamage));
            damageText.GetComponent<FadeUI>().SetFadeValues(0f, 0f, 0.75f);
        }
    }

    public void AddForce(Vector2 force)
    {
        rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    public void SetFaint(float _time)
    {
        MoveCtrl.instance.SetInit();
        AttackCtrl.instance.SetInit();
        if (AutoPlayCtrl.instance != null && AutoPlayCtrl.instance.isAutoOn)
            AutoPlayCtrl.instance.SetInit();
        animator.SetBool("isMove", false);
        handsAnimator.SetBool("isMove", false);
        SetInfoText("(기절)", 0f, _time);
        StartCoroutine(Faint(_time));
    }

    IEnumerator Faint(float _time)
    {
        isCanCtrl = false;
        yield return new WaitForSeconds(_time);
        isCanCtrl = true;
    }
}
