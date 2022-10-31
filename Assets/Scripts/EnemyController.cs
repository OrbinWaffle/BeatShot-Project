using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ISyncable
{
    [Tooltip("This enemy will move every beatsPerMove beats.")]
    [SerializeField] private int beatsPerMove = 4;
    [SerializeField] private float force = 1f;
    [SerializeField] private float bounceForce = 1f;
    [SerializeField] private float joltTime = 1f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minVelocity = .01f;
    [SerializeField] private float stickOut = .1f;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private LayerMask LM;
    private int MAXTRIES = 10;
    private bool moving;
    private float lastMovingTime;
    private Transform player;
    private Rigidbody RB;
    private SphereCollider COL;
    int beat = 1;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        RB = GetComponent<Rigidbody>();
        COL = GetComponent<SphereCollider>();
    }

    public void OnSync()
    {
        beat--;
        if(beat <= 0)
        {
            beat = beatsPerMove;
            DoMove();
        }
    }
    void DoMove()
    {
        Vector3 playerVector = Vector3.Normalize(player.position - transform.position);
        RB.velocity = Vector3.zero;
        RB.AddForce(playerVector * force, ForceMode.Impulse);
        /*if(RB.velocity.magnitude < minVelocity)
        {
            if(moving == true)
            {
                moving = false;
                lastMovingTime = Time.time;
            }
            if(Time.time - lastMovingTime > joltTime)
            {
                lastMovingTime = Time.time;
                DoJolt();
            }
        }
        else
        {
            moving = true;
        }*/
    }
    
    void DoJolt()
    {
        Debug.Log("I AM JOLTING");
        Vector3 bounceVector = Vector3.zero;
        for (int i = 0; i < MAXTRIES; ++i)
        {
            Vector2 randCircle = Random.insideUnitCircle;
            bounceVector = new Vector3(randCircle.x, 0, randCircle.y);

            Debug.Log("I AM NOW TRYING " + bounceVector);
            
            RaycastHit hit;
            bool didHit = false;
            if(Physics.Raycast(transform.position, bounceVector, out hit, COL.radius + stickOut, LM))
            {
                didHit = true;
            }
            if(!didHit)
            {
                Debug.Log("AH IS GOOD");
                break;
            }
            Debug.Log("CRAP IT DIDN WORK");
        }

        RB.AddForce(bounceVector * bounceForce, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.collider.tag == "Projectile")
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            //Removing self from the Rhythm Manager, so that it does not attempt to call on a null object.
            RhythmManager.mainRM.RemoveSyncable(this);
            RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<SyncedAnimation>());
            Destroy(this.gameObject);
        }
    }
}
