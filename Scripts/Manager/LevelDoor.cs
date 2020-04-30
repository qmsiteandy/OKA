﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour {

    static public Vector3 lastEnterPos = Vector3.zero;

    [Header("門功能")]
    public string toSceneName;
    public Sprite doorClose;
    public Sprite doorOpen;
    private float enterInputTime = 0f;
    private bool isChange = false;
    private bool isHoldingInput = false;
    private bool thisDoorOpen = false;
    
    [Header("門閃亮FX")]
    private ParticleSystem doorShineFx;
    private float particleStartSpeed;
    private float particleStartAmount;

    [Header("UI顯現")]
    private float fadeUpSpeed = 5f;
    private float fadeDownSpeed = 4f;
    private CanvasGroup canvasGroup;
    private float alpha = 0f;
    private bool isFadingUp = false;

    // Use this for initialization
    void Awake ()
    {
        //---門的樣式
        if (SceneManager.GetSceneByName("toSceneName").buildIndex <= LevelData.get_LevelRecord() + 1)
        {
            this.transform.Find("door_IMG").GetComponent<SpriteRenderer>().sprite = doorOpen;
            thisDoorOpen = true;
        }
        else
        {
            this.transform.Find("door_IMG").GetComponent<SpriteRenderer>().sprite = doorClose;
            thisDoorOpen = false;
        }

        //---門閃亮FX
        doorShineFx = this.transform.Find("doorShineFx").GetComponent<ParticleSystem>();
        particleStartSpeed = doorShineFx.startSpeed;
        particleStartAmount = doorShineFx.emissionRate;


        //----UI顯現
        canvasGroup = transform.Find("canvas").Find("InputNote_group").GetComponent<CanvasGroup>();
        canvasGroup.alpha = alpha;
    }

    void Update()
    {
        if (isFadingUp && canvasGroup.alpha < 1f)
        {
            alpha += fadeUpSpeed * Time.deltaTime;
            if (alpha > 1f) alpha = 1f;
        }
        else if (!isFadingUp && canvasGroup.alpha > 0f)
        {
            alpha -= fadeDownSpeed * Time.deltaTime;
            if (alpha < 0f) alpha = 0f;
        }

        canvasGroup.alpha = alpha;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && thisDoorOpen) isFadingUp = true;
    }
    void OnTriggerStay2D(Collider2D other)
    { 
        if (other.tag == "Player" && isChange == false && thisDoorOpen)
        {
            if (Input.GetButton("Submit") || Input.GetKey(KeyCode.Space))
            {
                PlayerStatus.isSkilling = true;
                isHoldingInput = true;

                enterInputTime += Time.deltaTime;

                if (doorShineFx.isPlaying == false) doorShineFx.Play();
                doorShineFx.startSpeed = particleStartSpeed * (enterInputTime * 1.5f);
                doorShineFx.emissionRate = particleStartAmount * (enterInputTime * 1.2f);

                if (enterInputTime > 1.5f)
                {
                    doorShineFx.Emit(25);
                    doorShineFx.Stop();

                    ChangeScene();
                    lastEnterPos = this.transform.position;
                }
            }
            if ((!Input.GetButton("Submit") && !Input.GetKey(KeyCode.Space)) && isHoldingInput)
            {
                PlayerStatus.isSkilling = false;
                isHoldingInput = false;

                enterInputTime = 0f;

                if (doorShineFx.isPlaying == true) doorShineFx.Stop();
                doorShineFx.startSpeed = particleStartSpeed;
                doorShineFx.emissionRate = particleStartAmount;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && thisDoorOpen) isFadingUp = false;

        PlayerStatus.isSkilling = false;
        enterInputTime = 0f;

        if (doorShineFx.isPlaying == true) doorShineFx.Stop();
        doorShineFx.startSpeed = particleStartSpeed;
        doorShineFx.emissionRate = particleStartAmount;
    }

    public void ChangeScene()
    {
        isChange = true;
        GameObject.Find("GameManager").GetComponent<GameManager>().GoToScene(toSceneName);
    }
}