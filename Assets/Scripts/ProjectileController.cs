using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, ISyncable
{
    [SerializeField] private float distance = 1;
    [SerializeField] private float snapSpeed = 1;
    [SerializeField] private int beatsAlive = 4;
    [SerializeField] private int beatsPerMove = 1;
    [SerializeField] private GameObject particle;
    [SerializeField] private LayerMask LM;
    float beatsLeft;
    float wallDist = 0.1f;
    float orgSnapSpeed;
    int beat = 1;
    Queue<Vector3> visitQueue = new Queue<Vector3>();
    void Start()
    {
        orgSnapSpeed = snapSpeed;
        visitQueue.Enqueue(transform.position);
        beatsLeft = beatsAlive;
    }
    void Update()
    {
        //DoMovement();
    }
    public void OnSync()
    {
        if(beatsAlive <= 0)
        {
            Destroy();
            return;
        }
        beat--;
        if(beat <= 0)
        {
            Move();
            beat = beatsPerMove;
        }
    }
    void Move()
    {
        float dist = distance;
        RaycastHit hit;
        float castDist = distance;
        Vector3 castPos = transform.position;
        Vector3 castDir = transform.forward;
        int reflections = 0;
        List<Vector3> points = new List<Vector3>();
        while(Physics.Raycast(castPos, castDir, out hit, castDist, LM.value))
        {
            reflections++;
            if (reflections > 1000)
            {
                Debug.LogError("I HIT THE MAX, SOMETHING IS WRONG");
                break;
            }
            dist = Vector3.Distance(castPos, hit.point);
            //visitQueue.Enqueue(visitQueue.Peek() + castDir * dist);
            points.Add(castPos + castDir * dist);
            castPos = hit.point;
            castDir = Vector3.Reflect(castDir, hit.normal).normalized;
            castDist -= dist;
        }
        //visitQueue.Enqueue(visitQueue.Peek() + castDir * dist);
        points.Add(castPos + castDir * castDist);
        //transform.rotation = Quaternion.LookRotation(castDir, Vector3.up);
        StartCoroutine(DoMovement(points, snapSpeed*(reflections+1)));
        beatsAlive--;
    }
    /*void otherDoMovement()
    {
        if(visitQueue.Count == 1)
        {
            snapSpeed = orgSnapSpeed;
        }
        if(visitQueue.Count > 1 & Vector3.Distance(transform.position, visitQueue.Peek()) < wallDist)
        {
            visitQueue.Dequeue();
        }
        transform.position = Vector3.Lerp(transform.position, visitQueue.Peek(), snapSpeed);
        Debug.Log(visitQueue.ToString());
    }*/
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
            transform.position = Vector3.Lerp(transform.position, pointsTovisit[index], speed * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
    void Destroy()
    {
        RhythmManager.mainRM.RemoveSyncable(this);
        RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<SyncedAnimation>());
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
