﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class special_l_waterCanon : MonoBehaviour {

    public int damageAmount = 1;
    private Collider2D attackTrigger;
    private ContactFilter2D enemyFilter;
    protected ContactFilter2D canAtkObjFilter;
    private float canonAngle = 0f;

    // Use this for initialization
    void Start ()
    {
        attackTrigger = this.GetComponent<Collider2D>();

        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        canAtkObjFilter.SetLayerMask(LayerMask.GetMask("CanAtkObj"));
    }

    void Damage()
    {
        Collider2D[] enemyColList = new Collider2D[5];
        int enemyCount = attackTrigger.OverlapCollider(enemyFilter, enemyColList);

        if (enemyCount > 0)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                if (enemyColList[i].GetComponent<Enemy_Dead>().isDead == true) continue;

                Enemy_base enemy_Base = enemyColList[i].GetComponent<Enemy_base>();
                enemy_Base.TakeDamage(1);
                enemy_Base.KnockBack(PlayerControl.facingRight ? Vector3.right : Vector3.left, 100f);
            }
        }

        Collider2D[] atkObjColList = new Collider2D[5];
        int ObjCount = attackTrigger.OverlapCollider(canAtkObjFilter, atkObjColList);

        if (ObjCount > 0)
        {
            for (int i = 0; i < ObjCount; i++)
            {
                atkObjColList[i].GetComponent<CanAtkObj>().TakeDamage(1);
            }
        }
    }

    public void SetAngle(float angle)
    {
        canonAngle = angle;
        canonAngle = canonAngle * Mathf.PI / 180f;
    }

    void Finish()
    {
        Destroy(this.transform.parent.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WaterArea"))
        {
            GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Scene"; GetComponentInChildren<SpriteRenderer>().sortingOrder = -1;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WaterArea"))
        {
            GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Skill"; GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
        }
    }
}
