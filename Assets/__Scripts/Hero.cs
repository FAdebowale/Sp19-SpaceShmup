﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; //Singleton
    [Header("Set in Inspector")]
    //These fields control movement of ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;
    //This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGo = null;

    void Awake()
    {
        if(S == null)
        {
            S = this; //Set the singleton
        } else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //Change transform.position based on axis
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        //rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //Allow ship to fire
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TempFire();
        }

    }

    void TempFire()
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        rigidB.velocity = Vector3.up * projectileSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        Transform rooT = other.gameObject.transform.root;
        GameObject go = rooT.gameObject;
        //print("Triggered:" + go.name);

        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Enemy") //If shield was triggered by enemy
        {
            shieldLevel--; //Decrease shield level by 1
            Destroy(go);
        }
        else
        {
            print("Triggered by non-Enemy:" + go.name);
        }
    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            //If the shield going to be set to less than zero
            if(value < 0)
            {
                Destroy(this.gameObject);
                //Tell Main.S to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);            }
        }
    }
}
