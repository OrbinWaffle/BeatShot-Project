using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float force = 1f;
    [SerializeField] private float bounceForce = 1f;
    [SerializeField] private float joltTime = 1f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minVelocity = .01f;
    [SerializeField] private float stickOut = .1f;
    [SerializeField] private LayerMask LM;
    private int MAXTRIES = 10;
    private bool moving;
    private float lastMovingTime;
    private Transform player;
    private Rigidbody RB;
    private SphereCollider COL;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        RB = GetComponent<Rigidbody>();
        COL = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerVector = Vector3.Normalize(player.position - transform.position);
        if(RB.velocity.magnitude < maxSpeed)
        {
            RB.AddForce(playerVector * force, ForceMode.Force);
        }
        if(RB.velocity.magnitude < minVelocity)
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
        }
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
}
