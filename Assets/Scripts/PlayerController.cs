
//This script controls the player's movement and firing

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("How fast the player rotates to meet the cursor. Higher values = higher speed.")]
    [SerializeField] private float rotatorSpeed = 0f;
    [Tooltip("How far the player is pushed back with each shot.")]
    [SerializeField] private float recoil = 1f;
    [Tooltip("The minimum amount of time between shots. Only really has effect when manual control on PlayerInput is enabled.")]
    [SerializeField] private float timeBetweenShots = 0f;
    [Tooltip("The number of projectiles fired out with each shot. More projectiles will give a shotgun-like effect.")]
    [SerializeField] private int numOfShots = 1;
    [Tooltip("The spread, in degrees, that the shots will be constrained to if there is more than one projectile.")]
    [SerializeField] private float spreadOfShots = 90f;
    [Tooltip("If this is set to true, the player cannot die.")]
    [SerializeField] private bool isInvincible = false;
    [Tooltip("The projectile that will be fired.")]
    [SerializeField] private GameObject projectile;
    [Tooltip("The particle effect that will be generated on shot.")]
    [SerializeField] private GameObject shotParticle;
    [Tooltip("The particle effect that will be generated on death.")]
    [SerializeField] private GameObject deathParticle;
    [Tooltip("The rotator object.")]
    [SerializeField] private Transform rotator;
    [Tooltip("The point at which the projectiles will be generated.")]
    [SerializeField] private Transform firePoint;
    [Tooltip("The UI manager.")]
    [SerializeField] private UIManager UIM;
    [Tooltip("Sound the player makes when shooting.")]
    [SerializeField] private AudioClip shotSound;
    [Tooltip("Funni sound to let the player know they are a failure and have awful rhythm.")]
    [SerializeField] private AudioClip lmaoUrBadSound;
    private float timeOfLastShot;
    AudioSource audSource;
    Rigidbody RB;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        audSource = GetComponent<AudioSource>();
    }
    //Whenever this method is called, the rotator will rotate to face the Vector3 mousePos.
    public void Aim(Vector3 mousePos)
    {
        Vector3 aimPos = new Vector3(mousePos.x, transform.position.y, mousePos.z);
        Vector3 lookVec = (aimPos - rotator.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(lookVec, Vector3.up);
        rotator.rotation = Quaternion.Slerp(rotator.rotation, lookRot, rotatorSpeed * Time.deltaTime);
    }
    //Whenever this method is called, the player will fire a shot.
    public void Attack()
    {
        //If it is too soon to fire a shot, don't
        if(Time.time < timeOfLastShot + timeBetweenShots){return;}
        RB.AddForce(-rotator.forward * recoil, ForceMode.Impulse);
        Instantiate(shotParticle, firePoint.position, rotator.rotation);
        audSource.clip = shotSound;
        audSource.Play();
        //A loop to generate multiple projectiles if numOfShots > 1.
        for(int i = 0; i < numOfShots; i++)
        {
            //Will increment the rotation so the shots come out in an even spread.
            float rotation = (-spreadOfShots/2) + (spreadOfShots/numOfShots) * (i + 1);
            if(numOfShots == 1)
            {
                rotation = 0;
            }
            GameObject proj = Instantiate(projectile, firePoint.position, rotator.rotation * Quaternion.Euler(0, rotation, 0));
            //Adding the projectiles to the Rhythm Manager, since they must be synced with the music.
            RhythmManager.mainRM.AddSyncable(proj.GetComponent<ProjectileController>());
            RhythmManager.mainRM.AddSyncable(proj.GetComponent<SyncedAnimation>());
            //Calling an initial sync for this beat.
            proj.GetComponent<ProjectileController>().OnSync();
            proj.GetComponent<SyncedAnimation>().OnSync();
        }
        timeOfLastShot = Time.time;
    }
    //What the player should do if they attempt to perform an off-beat action.
    public void OnMiss()
    {
        audSource.clip = lmaoUrBadSound;
        audSource.Play();
    }
    //Will call whenever the object this script is on touches anything.
    void OnCollisionEnter(Collision other)
    {
        //Will kill the player only if the object I collided with has the "Projectile" tag.
        if(other.collider.tag == "Projectile" && !isInvincible)
        {
            UIM.PlayerDied();
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            //Removing self from the Rhythm Manager, so that it does not attempt to call on a null object.
            RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<PlayerInput>());
            RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<SyncedAnimation>());
            GetComponentInParent<PlayerInput>().isControlling = false;
            this.gameObject.SetActive(false);
        }
    }
}
