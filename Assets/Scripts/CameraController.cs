using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSmoothTime = 0;
    [SerializeField] private Transform targetObj;
    Vector3 velocity = Vector3.zero;
    void Start()
    {
        targetObj = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector3 target = targetObj.position;
        transform.parent.parent.position = Vector3.SmoothDamp(transform.parent.parent.position, target, ref velocity, moveSmoothTime);
    }
}
