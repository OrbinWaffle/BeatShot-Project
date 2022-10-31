
//This script records user input and forwards it to the Player Controller

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour, ISyncable
{
    [Tooltip("Amount of beats it will take before a shot is fired.")]
    [SerializeField] private int beatsPerShot = 4;
    [Tooltip("If this is set to true, the player is allowed to fire whenever they want, separate of the beat.")]
    [SerializeField] private bool manualControl = false;
    [Tooltip("The custom cursor object.")]
    [SerializeField] private Transform curs;
    [Tooltip("If this is false, the player cannot input anything.")]
    public bool isControlling = true;
    PlayerController PM;
    Plane Plane = new Plane(Vector3.up, 0);
    //Beat counter for beatsPerShot
    int beat = 1;
    private bool usingAxis;
    void Start()
    {
        PM = GetComponentInChildren<PlayerController>();
        Cursor.visible = false;
        curs = GameObject.FindGameObjectWithTag("Cursor").transform;
    }
    void Update()
    {
        OnMouse();
        OnAttack();
        OnDodge();
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            usingAxis = false;
        }
    }
    public void OnSync()
    {
        /*if(!isControlling){return;}
        beat--;
        if(beat <= 0)
        {
            if(!manualControl){PM.Attack();}
            beat = beatsPerShot;
        }*/
    }
    //Casts a ray from the mouse's screen position into the world, then sends this position to the Player Controller.
    void OnMouse()
    {
        float dist;
        curs.position = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Plane.Raycast(ray, out dist))
        {
            Vector3 mousePos = ray.GetPoint(dist);
            PM.Aim(mousePos);
        }
    }
    //If the Fire button is pressed, request a rhythm evaluation from the Rhythm Manager.
    //Send this evaluation to the player.
    void OnAttack()
    {
        if(!isControlling){return;}
        if(Input.GetButtonDown("Fire1"))
        {
            int rhythmScore;
            float time;
            float trueTime;
            RhythmManager.mainRM.RateTime(Time.time, out rhythmScore, out time, out trueTime);
            if(rhythmScore == 1)
            {
                PM.Attack(trueTime >= 0);
            }
            else if(rhythmScore == 0)
            {
                PM.OnMiss();
            }
        }
    }
    //If the dodge key is pressed, request a rhythm evaluation from the Rhythm Manager.
    //Send this evaluation to the player.
    void OnDodge()
    {
        if(!isControlling){return;}
        if(!usingAxis && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            usingAxis = true;
            int rhythmScore;
            RhythmManager.mainRM.RateTime(Time.time, out rhythmScore);
            if(rhythmScore == 1)
            {
                Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
                PM.OnDodge(dir);
            }
            else if(rhythmScore == 0)
            {
                PM.OnMiss();
            }
        }
    }
}
