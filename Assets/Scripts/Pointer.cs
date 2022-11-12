using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    // Start is called before the first frame update
    LineRenderer lr;
    void Start()
    { 
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity);
        Vector3[] a = new Vector3[2];
        a[0] = transform.position;
        a[1] = hit.point;
        lr.SetPositions(a);
    }
}
