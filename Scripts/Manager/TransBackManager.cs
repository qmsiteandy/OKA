﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransBackManager : MonoBehaviour {

    //可以設定玩家進入設定好的collider後傳回上一個recordPoint
    //或也可以直接互叫函式傳回

    public float stayingTime = 0f;  //停留多久後傳回
    private float timer = 0f;

    private Transform playerTrans;

    private GameManager gameManager;



	void Start ()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            timer += Time.deltaTime;
            if (timer >= stayingTime)
            {
                TransBack();
                timer = 0f;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            timer = 0f;
        }
    }

    public void DieTransBack()
    {
        Vector3 backPos = RecordPointManager.Get_playerRecordPos();
        StartCoroutine(TransBack(backPos));
    }

    void TransBack()
    {
        Vector3 backPos = RecordPointManager.Get_playerRecordPos();
        StartCoroutine(TransBack(backPos));
    }
    IEnumerator TransBack(Vector3 TransBackPos)
    {
        gameManager.BlackPanelFade(1f, 0.5f);
        PlayerStatus.canControl = false;

        yield return new WaitForSeconds(0.5f);

        GameObject.Find("Player").GetComponent<PlayerControl>().FallAsleep();
        GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        playerTrans.position = RecordPointManager.Get_playerRecordPos();
        gameManager.BlackPanelFade(0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        GameObject.Find("Player").GetComponent<PlayerControl>().SleepAwake();
        PlayerStatus.canControl = true;
        
    }
}
