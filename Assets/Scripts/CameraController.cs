using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSmoothTime = 0;
    [SerializeField] private Transform targetObj;
    Animator anim;
    Vector3 velocity = Vector3.zero;
    public static CameraController main;
    void Start()
    {
        main = this;
        targetObj = GameObject.FindGameObjectWithTag("Player").transform;
        anim = transform.parent.parent.GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 target = targetObj.position;
        transform.parent.parent.position = Vector3.SmoothDamp(transform.parent.parent.position, target, ref velocity, moveSmoothTime);
    }
    public void PlayMiss()
    {
        anim.SetTrigger("miss");
    }
}
