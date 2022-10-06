
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
    void Start()
    {
        PM = GetComponentInChildren<PlayerController>();
        Cursor.visible = false;
    }
    void Update()
    {
        OnMouse();
        OnAttack();
    }
    public void OnSync()
    {
        if(!isControlling){return;}
        beat--;
        if(beat <= 0)
        {
            if(!manualControl){PM.Attack();}
            beat = beatsPerShot;
        }
        /*if(Input.GetButtonDown("Fire1") || Input.GetButton("Fire1"))
        {
            PM.Attack();
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
    //If the Fire button is pressed and we are given manual control, tell the Player Controller to attack.
    void OnAttack()
    {
        if(!isControlling || !manualControl){return;}
        if(Input.GetButton("Fire1"))
        {
            PM.Attack();
        }
    }
}
