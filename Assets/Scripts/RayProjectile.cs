using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayProjectile : MonoBehaviour
{
    [SerializeField] private bool doDelay = false;
    LineRenderer lr;
    [SerializeField] float lifetime = 1f;
    [SerializeField] LayerMask lm;
    // Start is called before the first frame update
    [SerializeField] GameObject particle;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        if(doDelay)
        {
            Invoke("CastRay", RhythmManager.mainRM.rhythmLeeway);
        }
        else
        {
            CastRay();
        }
    }

    void CastRay()
    {
        lr.enabled = true;
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity);
        Vector3[] a = new Vector3[2];
        a[0] = transform.position;
        a[1] = hit.point;
        lr.SetPositions(a);
        if(hit.collider.gameObject.tag == "Player") {
            hit.collider.gameObject.GetComponent<PlayerController>().OnDeath();
        }

        Quaternion particleRotation = Quaternion.LookRotation(hit.normal);
        Instantiate(particle, hit.point, particleRotation);

        Invoke("Die", lifetime);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
