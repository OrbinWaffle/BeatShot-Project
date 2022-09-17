using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour, ISyncable
{
    [SerializeField] private int beatsPerShot = 4;
    [SerializeField] private Transform curs;
    public bool isControlling = true;
    PlayerController PM;
    Plane Plane = new Plane(Vector3.up, 0);
    int beat = 1;
    void Start()
    {
        PM = GetComponentInChildren<PlayerController>();
        Cursor.visible = false;
    }
    void Update()
    {
        OnMouse();
        //OnAttack();
        //OnAttack();
    }
    public void OnSync()
    {
        if(!isControlling){return;}
        beat--;
        if(beat <= 0)
        {
            PM.Attack();
            beat = beatsPerShot;
        }
        /*if(Input.GetButtonDown("Fire1") || Input.GetButton("Fire1"))
        {
            PM.Attack();
        }*/
    }
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
    void OnAttack()
    {
        if(!isControlling){return;}
        if(Input.GetButton("Fire1"))
        {
            PM.Attack();
        }
    }
}
