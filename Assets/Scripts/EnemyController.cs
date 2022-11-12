using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ISyncable
{
    [Tooltip("This enemy will move every beatsPerMove beats.")]
    [SerializeField] private int beatsPerMove = 4;
    [SerializeField] private int beatsPerShot = 4;
    [SerializeField] private float force = 1f;
    [SerializeField] private float recoilForce = 1f;
    [SerializeField] private float bounceForce = 1f;
    [SerializeField] private float joltTime = 1f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minVelocity = .01f;
    [SerializeField] private float stickOut = .1f;
    [SerializeField] private bool shoots = false;
    [SerializeField] private bool randomMovement = false;
    [SerializeField] private bool spawnOnCenter = false;
    [SerializeField] private float sizeOfRay = 1f;
    [SerializeField] private float rotatorSpeed = 1f;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private LayerMask LM;
    [SerializeField] private Transform rotator;
    [SerializeField] private Transform fireSpot;
    [SerializeField] private GameObject projectile;
    private int MAXTRIES = 10;
    private bool moving;
    private float lastMovingTime;
    private Transform player;
    private Rigidbody RB;
    private SphereCollider COL;
    int beat = 1;
    int beatShot = 3;
    void Awake()
    {
        GameObject pl = GameObject.FindGameObjectWithTag("Player");
        if(pl != null)
        {
            player = pl.transform;
        }
        RB = GetComponent<Rigidbody>();
        COL = GetComponent<SphereCollider>();
    }

    void Update() {
        if(RhythmManager.mainRM.isPlayerDead){return;}
        if(shoots)
            RoatateTowardsPlayer();
    }

    public void OnSync()
    {
        if(RhythmManager.mainRM.isPlayerDead){return;}
        beat--;
        beatShot--;
        if(beat <= 0)
        {
            beat = beatsPerMove;
            DoMove();
        }
        if(!shoots) {
            return;
        }
        if(beatShot <= 0)
        {
            beatShot = beatsPerShot;
            doShot();
        }
    }

    void RoatateTowardsPlayer()
    {
        Vector3 direction = Vector3.Normalize(player.position - transform.position);
        Quaternion lookRot = Quaternion.LookRotation(direction, Vector3.up);
        rotator.rotation = Quaternion.Slerp(rotator.rotation, lookRot, rotatorSpeed * Time.deltaTime);
    }

    void doShot()
    {
        GameObject proj;
        if(spawnOnCenter) {
            proj = Instantiate(projectile, rotator.position, rotator.rotation);
        }
        else {
            proj = Instantiate(projectile, fireSpot.position, rotator.rotation);
        }
        if(proj.GetComponent<ProjectileController>() != null) {
            RhythmManager.mainRM.AddSyncable(proj.GetComponent<ProjectileController>());
            proj.GetComponent<ProjectileController>().OnSync();
        }
        // RhythmManager.mainRM.AddSyncable(proj.GetComponent<SyncedAnimation>());
        Vector3 recoil = Vector3.Normalize(transform.position - player.position);
        RB.AddForce(recoil * (force * recoilForce), ForceMode.Impulse);
    }

    void DoMove()
    {
        Vector3 direction;
        if(!randomMovement) {
            direction = Vector3.Normalize(player.position - transform.position);
        }
        else {
            Vector2 randCircle = Random.insideUnitCircle;
            direction = new Vector3(randCircle.x, 0, randCircle.y);
        }
        RB.velocity = Vector3.zero;
        RB.AddForce(direction * force, ForceMode.Impulse);
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
        if(other.collider.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().OnDeath();
        }
    }
    public void OnDeath()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        //Removing self from the Rhythm Manager, so that it does not attempt to call on a null object.
        RhythmManager.mainRM.RemoveSyncable(this);
        RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<SyncedAnimation>());
        Destroy(this.gameObject);
    }
}
