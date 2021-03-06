﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemyControl : Enemy_base {

    [Header("Move Settings")]
    public float moveRangeX = 3.5f;    //移動範圍半徑
    public float moveRangeYmax = 0.2f;
    public float moveRangeYmin = -1f;
    public float moveSpeed = 1.5f;  //移動速度
    private float posNowX = 0f;      //目前移動相對中點的位置
    private float posNowY = 0f;
    private bool goRight = true;    //是否往右走
    public float trackSpeed = 1.5f;
    public float closeRange = 1.25f;
    public Vector2 centerPos;      //移動的區域中點
    private BoxCollider2D BodyCollider;

    [Header("Track Settings")]
    GameObject target;
    public float FollowRadius = 4.5f;
    private LayerMask playerFilter;
    public bool isTracking = false;

    //public float xchange;
    //public float ychange;

    // Use this for initialization
    void Awake () {
        BaseAwake();    //父腳本的Awake

        centerPos = transform.position; //移動中點設為最初的位置

        playerFilter = LayerMask.GetMask("Player");

        BodyCollider = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!enemy_dead.isDead)
        {
            FindPlayer();

            if (!isTracking || isFreeze == true)
            {
                MoveAround();
            }
            else
            {
                if (isTracking)
                {
                    Tracking();
                }
                else
                {
                    MoveAround();
                }
            }
        }
    }

    #region ================↓來回移動↓================
    void MoveAround()
    {
        if (!isAttacking)
        {
            if (posNowX >= moveRangeX || posNowX <= -moveRangeX)
            {
                posNowX = Mathf.Clamp(posNowX, -moveRangeX, moveRangeX); //設定posNow到範圍邊界
                goRight = !goRight; //往回走
                if (goRight)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }

            if (goRight) posNowX += moveSpeed * Time.deltaTime;
            else if (!goRight) posNowX -= moveSpeed * Time.deltaTime;

            transform.position = new Vector3(centerPos.x + posNowX, centerPos.y + posNowY, 0f); //設定腳色的位置
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    #endregion ================↑來回移動↑================

    #region ================↓追蹤主角↓================
    void FindPlayer()
    {
        bool traceMode = isTracking;

        isTracking = Physics2D.OverlapCircle(this.transform.position, FollowRadius, playerFilter);

        if (traceMode == false && isTracking == true)
        {
            Collider2D playerCol = Physics2D.OverlapCircle(this.transform.position, FollowRadius, playerFilter);
            target = playerCol.gameObject;
        }
    }

    void Tracking()
    {
        if (target != null && !isAttacking && isFreeze == false)
        {

            Vector3 diff = new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, 0);
            if (Mathf.Abs(diff.x) <= closeRange) return;

            posNowX = Mathf.Lerp(posNowX, target.transform.position.x - centerPos.x, trackSpeed * Time.deltaTime);
            posNowX = Mathf.Clamp(posNowX, -moveRangeX, moveRangeX);
            posNowY = Mathf.Lerp(posNowY, target.transform.position.y - centerPos.y, trackSpeed * Time.deltaTime);
            posNowY = Mathf.Clamp(posNowY, moveRangeYmin, moveRangeYmax);
            transform.position = new Vector3(centerPos.x + posNowX, centerPos.y + posNowY, 0f);

            //設定怪物面向哪邊
            float face = Mathf.Sign(diff.x);
            goRight = (face >= 0) ? true : false;
            Vector3 faceVec = new Vector3(face, 1, 1);
            transform.localScale = faceVec;

            //設定怪物的角度
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            if (face >= 0)
            {
                transform.eulerAngles = Vector3.forward * angle;
            }
            else
            {
                if(angle > 90 && angle < 180)
                {
                    transform.eulerAngles = Vector3.forward * (angle - 180);
                }
                else if(angle < -90 && angle > -180)
                {
                    transform.eulerAngles = Vector3.forward * (angle + 180);
                }
            }
        }

    }
    #endregion ================↑追蹤主角↑================

    #region ================↓受到攻擊↓================
    //覆寫TakeDamage
    public override void TakeDamage(int damage)
    {
        if (!enemy_dead.isDead)
        {
            enemy_dead.health -= damage;
            if (enemy_dead.health <= 0)
            {
                enemy_dead.health = 0;
                enemy_dead.isDead = true;
                animator.SetTrigger("Dead");
                isAttacking = false;
            }
            else if (isAttacking == false)
            {
                animator.SetTrigger("Injury");
                StartCoroutine(Freeze(0.5f));
            }
            StartCoroutine(ChangeColor(new Color(1f, 0.3962386f, 0.3726415f), 0.1f));
        }
    }
    #endregion ================↑受到攻擊↑================

    public void BodyColliderClose()
    {
        BodyCollider.enabled = false;

        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void BodyColliderOpen()
    {
        BodyCollider.enabled = true;

        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
