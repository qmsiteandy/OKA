﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour {

    private bool isEnergyUsing = true;

    [HideInInspector] public int waterEnergyMax = 100;
    private int waterEnergy;
    [HideInInspector] public int dirtMax = 100;
    private int dirt = 0;

    private float chargeDelay = 2f;
    private int waterPerCharge = 3;
    private float elapsed = 0f;

    //private SpriteRenderer[] playerSprite= { null, null, null };
    private UI_Manager UI_manager;

    private GameManager gameManager;

    private Material dirtyRippeMat;

    void Start ()
    {
        waterEnergy = waterEnergyMax;
        dirtMax = waterEnergyMax;

        //for (int x = 0; x < 3; x++) playerSprite[x] = transform.GetChild(x).GetComponent<SpriteRenderer>();
        //for (int x = 0; x < 3; x++) Oka[x] = transform.GetChild(x).gameObject;
        isEnergyUsing = GameObject.Find("UI_Canvas") != null;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        dirtyRippeMat = this.transform.Find("DirtyRipple_Mask").Find("DirtyRipple").GetComponent<SpriteRenderer>().material;
        dirtyRippeMat.SetFloat("_drityDegree", 0);

        if (isEnergyUsing)
        {
            UI_manager = GameObject.Find("UI_Canvas").GetComponent<UI_Manager>();
            UI_manager.SetWaterUI(waterEnergy);
            SetPurityDegree();
        }
    }

    void Update()
    {
        if (elapsed > chargeDelay) { ModifyWaterEnergy(waterPerCharge); elapsed = 0f; }
        elapsed += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Backspace)) { ModifyWaterEnergy(-10); ModifyDirt(10); }
        if (Input.GetKeyDown(KeyCode.Equals)) { ModifyWaterEnergy(10); ModifyDirt(-10); }
    }

    public void ModifyWaterEnergy(int amount)
    {
        if (!isEnergyUsing) return;

        waterEnergy += amount;
        if (waterEnergy > waterEnergyMax) waterEnergy = waterEnergyMax;
        else if (waterEnergy < 0) waterEnergy = 0;

        if (UI_manager != null) UI_manager.SetWaterUI(waterEnergy);

        if (waterEnergy <= 0) Dead();
    }

    public void ModifyDirt(int amount)
    {
        if (!isEnergyUsing) return;

        dirt += amount;
        if (dirt > dirtMax) dirt = dirtMax;
        else if (dirt < 0) dirt = 0;

        SetPurityDegree();
    }

    void SetPurityDegree()
    {
        float dirtyDegree;
        dirtyDegree = (float)dirt / dirtMax;

        if (UI_manager != null) UI_manager.SetPurityUI(1 - dirtyDegree);
        
        dirtyRippeMat.SetFloat("_drityDegree", dirtyDegree);

        if (dirtyDegree >= 1f) Dead(); //GameOver
    }

    public void ConnectNewLevelUI()
    {
        isEnergyUsing = GameObject.Find("UI_Canvas") != null;
        if (isEnergyUsing) UI_manager = GameObject.Find("UI_Canvas").GetComponent<UI_Manager>();
        else UI_manager = null;
    }

    public void ResetEnegy()
    {
        waterEnergy = waterEnergyMax;
        dirt = 0;
    }

    void Dead()
    {
        gameManager.Dead();
    }
}
