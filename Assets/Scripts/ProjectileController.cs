
//This script controls the projectiles and their movement.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, ISyncable
{
    [Tooltip("The distance this projectile will travel per move.")]
    [SerializeField] private float distance = 1;
    [Tooltip("The speed at which the projectile will snap to it's target location.")]
    [SerializeField] private float snapSpeed = 1;
    [Tooltip("The amount of beats this projectile will last.")]
    [SerializeField] private int beatsAlive = 4;
    [Tooltip("The amount of beats it will take this projectile to move.")]
    [SerializeField] private int beatsPerMove = 1;
    [Tooltip("The particle effect generated once this projectile is destroyed.")]
    [SerializeField] private GameObject particle;
    [Tooltip("A layermask defining which layers this projectile will bounce off of.")]
    [SerializeField] private LayerMask LM;
    float beatsLeft;
    float wallDist = 0.1f;
    float orgSnapSpeed;
    int beat = 1;
    void Start()
    {
        orgSnapSpeed = snapSpeed;
        beatsLeft = beatsAlive;
    }
    void Update()
    {
        //DoMovement();
    }
    public void OnSync()
    {
        //If we've exceeded our beatsAlive, destroy this object.
        if(beatsAlive <= 0)
        {
            DeleteProjectile();
            return;
        }
        beatsAlive--;
        beat--;
        //If we are on a movement beat, move.
        if(beat <= 0)
        {
            Move();
            beat = beatsPerMove;
        }
    }
    //When this method is called, the projectile will calculate it's trajectory before it begins moving.
    void Move()
    {
        float dist = distance;
        RaycastHit hit;
        float castDist = distance;
        Vector3 castPos = transform.position;
        Vector3 castDir = transform.forward;
        int reflections = 0;
        List<Vector3> points = new List<Vector3>();
        /*
        Cast a ray forward from the projectile's current point. If it hits an object,
        add that point to the points list, then subtract the distance already calculated and
        cast another ray from that point reflected off the surface's normal. Continue
        until the full distance has been calculated.
        */
        while(Physics.Raycast(castPos, castDir, out hit, castDist, LM.value))
        {
            //Increment how many times this projectile has been reflected
            reflections++;
            //Failsafe; if we hit over 1000 reflections, something is probably wrong.
            if (reflections > 1000)
            {
                Debug.LogError("I HIT THE MAX, SOMETHING IS WRONG");
                break;
            }
            dist = Vector3.Distance(castPos, hit.point);
            points.Add(castPos + castDir * dist);
            castPos = hit.point;
            castDir = Vector3.Reflect(castDir, hit.normal).normalized;
            castDist -= dist;
        }
        //Add the final point to the list, which will not be at a collision point.
        points.Add(castPos + castDir * castDist);
        /*
        Once the points list has been filled with all the points we must visit,
        move the projectile to follow these points.
        */
        StartCoroutine(DoMovement(points, snapSpeed*(reflections+1)));
    }
    /*
    A coroutine that actually moves the projectile. It will lerp the projectile to the first point
    in the points list, then once that first point is reached, begin lerping to the next point.
    Continue until the final point in the list is reached.
    */
    IEnumerator DoMovement(List<Vector3> pointsTovisit, float speed)
    {
        int index = 0;
        while(true)
        {
            if(pointsTovisit.Count <= 0)
            {
                break;
            }
            if(Vector3.Distance(transform.position, pointsTovisit[index]) < wallDist && index < pointsTovisit.Count)
            {
                transform.position = pointsTovisit[index];
                index++;
                if(index < pointsTovisit.Count)
                {
                    transform.rotation = Quaternion.LookRotation(pointsTovisit[index] - transform.position, Vector3.up);
                }
            }
            if (index >= pointsTovisit.Count)
            {
                break;
            }
            /*
            The projectile will increase in speed the more ricochets it must complete. As of right now, the projectile
            lerps with smoothing for each individual point. This causes some problems; Wwll likely change later to reduce jitter.
            */
            transform.position = Vector3.Lerp(transform.position, pointsTovisit[index], speed * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
    //Remove this object from the Rhythm Manager and then destroy it.
    void DeleteProjectile()
    {
        RhythmManager.mainRM.RemoveSyncable(this);
        RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<SyncedAnimation>());
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.collider.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyController>().OnDeath();
            DeleteProjectile();
        }
        if(other.collider.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().OnDeath();
            DeleteProjectile();
        }
    }
}
