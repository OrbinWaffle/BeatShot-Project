
//This script controls the player's movement and firing

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISyncable
{
    [Tooltip("How fast the player rotates to meet the cursor. Higher values = higher speed.")]
    [SerializeField] private float rotatorSpeed = 0f;
    [Tooltip("The amount of beats for the dodge cooldown.")]
    [SerializeField] private int dodgeCooldown = 1;
    [Tooltip("Length of dodge invincibility")]
    [SerializeField] private float invulnTime = 0.5f;
    [Tooltip("How far the player is pushed back with each shot.")]
    [SerializeField] private float recoil = 1f;
    [Tooltip("How far the player moves when dodging.")]
    [SerializeField] private float dodgeSpeed = 1f;
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
    [Tooltip("The sound that will play on a dodge.")]
    [SerializeField] private AudioClip dodgeSound;
    [Tooltip("The rotator object.")]
    [SerializeField] private Transform rotator;
    [Tooltip("The point at which the projectiles will be generated.")]
    [SerializeField] private Transform firePoint;
    private UIManager UIM;
    [Tooltip("Sound the player makes when shooting.")]
    [SerializeField] private AudioClip shotSound;
    [Tooltip("Funni sound to let the player know they are a failure and have awful rhythm.")]
    [SerializeField] private AudioClip lmaoUrBadSound;
    [SerializeField] private Animator occluderAnim;
    private float timeOfLastShot;
    private int currentDodgeCooldown = 0;
    ParticleSystem PS;
    AudioSource audSource;
    Rigidbody RB;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        audSource = GetComponent<AudioSource>();
        PS = GetComponent<ParticleSystem>();
        UIM = UIManager.mainUIM;
    }
    public void OnSync()
    {
        Debug.Log(currentDodgeCooldown);
        if(currentDodgeCooldown > 0)
        {
            --currentDodgeCooldown;
        }
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
    public void Attack(bool doSync)
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
            //Calling an initial sync for this beat, provided it was not called already.
            if(doSync)
            {
                Debug.Log("CALLING INITIAL SYNC");
                proj.GetComponent<ProjectileController>().OnSync();
                proj.GetComponent<SyncedAnimation>().OnSync();
            }
        }
        timeOfLastShot = Time.time;
    }
    public void Attack()
    {
        Attack(false);
    }
    public void OnDodge(Vector3 dir, bool doSync)
    {
        if(currentDodgeCooldown > 0)
        {
            return;
        }
        isInvincible = true;
        Invoke("DisableInvuln", invulnTime);
        currentDodgeCooldown = dodgeCooldown;
        if(dir.normalized == Vector3.zero){dir = rotator.forward;}
        RB.AddForce(dir.normalized * dodgeSpeed, ForceMode.Impulse);
        PS.Play();
        audSource.clip = dodgeSound;
        audSource.Play();
        occluderAnim.SetTrigger("dodge");
        if(doSync)
        {
            --currentDodgeCooldown;
        }
        Invoke("DisableParticle", .25f);
    }
    void DisableParticle()
    {
        PS.Stop();
    }
    //What the player should do if they attempt to perform an off-beat action.
    public void OnMiss()
    {
        CameraController.main.PlayMiss();
        audSource.clip = lmaoUrBadSound;
        audSource.Play();
    }

    public void OnDeath() {
        if(isInvincible)
        {
            return;
        }
        UIM.PlayerDied();
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        //Removing self from the Rhythm Manager, so that it does not attempt to call on a null object.
        RhythmManager.mainRM.DeathNotification();
        RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<PlayerInput>());
        RhythmManager.mainRM.RemoveSyncable(gameObject.GetComponent<SyncedAnimation>());
        GetComponentInParent<PlayerInput>().isControlling = false;
        this.gameObject.SetActive(false);
    }
    public void DisableInvuln()
    {
        isInvincible = false;
    }
}
