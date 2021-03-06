﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringHealing : MonoBehaviour {

    public float healingTotalTime = 3f;
    public int TotalAmount = 100;
    public ParticleSystem healingFX;

    private int leftAmount; //剩餘治癒量
    private float healingCycle = 0.25f;
    private int amountPerTime;
    private bool isHealing = false;
    private float timer = 0f;

    private float springDepth;
    Transform springTrans;
    private float springDecreaseSpeed;

    private PlayerControl playerControl;
    private Vector3 playerPos;
    
	// Use this for initialization
	void Start ()
    {
        leftAmount = TotalAmount;
        amountPerTime = TotalAmount / (int)(healingTotalTime / healingCycle); 

        ParticleSystem.MainModule main = healingFX.main;
        main.duration = healingTotalTime;

        springDepth = GetComponent<BoxCollider2D>().size.y;
        springTrans = transform.parent.GetChild(0).GetComponent<Transform>();
        springDecreaseSpeed = springDepth / healingTotalTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isHealing)
        {
            if(timer >= healingCycle)
            {
                timer = 0f;

                playerControl.TakeHeal(amountPerTime, amountPerTime);
                leftAmount -= amountPerTime;

                if (leftAmount <= 0) HealingOver();
            }
            else
            {
                timer += Time.deltaTime;
            }

            springTrans.position -= new Vector3(0f, springDecreaseSpeed * Time.deltaTime, 0f);
        }        
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && !isHealing)
        {
            playerControl = collider.GetComponent<PlayerControl>();
            playerPos = collider.transform.position;

            isHealing = true;

            OpenFX();
        }
    }
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && isHealing)
        {
            playerControl = collider.GetComponent<PlayerControl>();

            playerPos = collider.transform.position;
            healingFX.transform.position = new Vector3(playerPos.x, healingFX.transform.position.y, healingFX.transform.position.z);
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && isHealing)
        {
            playerControl = null;

            isHealing = false;

            CloseFX();
        }
    }

    void HealingOver()
    {
        isHealing = false;
        transform.parent.gameObject.SetActive(false);

        for (int x = 0; x < 3; x++)
        {
            Renderer m_playerRenderer = playerControl.transform.GetChild(x).GetComponent<Renderer>();
            m_playerRenderer.sortingLayerName = "Player"; m_playerRenderer.sortingOrder = 0;
        }
    }

    void OpenFX()
    {
        healingFX.Play();
    }
    void CloseFX()
    {
        healingFX.Stop();
    }
}
